(**
---
title: Optimization/SGD
category: Lectures
categoryindex: 1
index: 6
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)
*)

#r "nuget: FSharp.Stats"
#r "nuget: FSharp.Data"
#r "nuget: DiffSharp-lite"
#r "nuget: Quotes.YahooFinance, 0.0.1-alpha.4"

open System
open FSharp.Data
open Quotes.YahooFinance

open FSharp.Stats
open DiffSharp

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

(*** condition: ipynb ***)
#if IPYNB
// Set dotnet interactive formatter to plaintext
Formatter.Register(fun (x:obj) (writer: TextWriter) -> fprintfn writer "%120A" x )
Formatter.SetPreferredMimeTypesFor(typeof<obj>, "text/plain")
// Make plotly graphs work with interactive plaintext formatter
Formatter.SetPreferredMimeTypesFor(typeof<GenericChart.GenericChart>,"text/html")
#endif // IPYNB


(**
# Optimization

Optimization is the process of maximizing or minimizing an objective function. Our objective is to find mean-variance efficient portfolios. . They are portfolios A common objective function to maximize or minimize is functions related to choosing mean-variance efficient portfolios. These are portfolios This can be done with a variety of optimization methods and objective functions.We are going to focus on stochastic gradient descent as the optimization method and We're now going to see how to do mean-variance portfolio optimization.
The objective is to find the portfolio with the greatest return per
unit of standard deviation.

In particular, we're going to identify the tangency portfolio. 
The tangency portfolio is the portfolio fully invested 
in risky assets that has the maximum achievable sharpe ratio. 
When there is a risk-free rate, the efficient frontier 
of optimal portfolios is some combination of
the tangency portfolio and the risk-free asset. 
Investors who want safe portfolios hold a lot 
of bonds and very little of the tangency portfolio. 
Investors who want riskier portfolios hold little risk-free bonds and
a lot of the tangency portfolio (or even lever the tangency portoflio). 

Now one thing to keep in mind is that often you think 
of this as the optimal weight per security.
But one well known problem is that trying to do this naively does not work well.
And by naively I mean taking a stock's average return and covariances in the sample and using that to estimate optimal weights. 
In large part, this is because it is hard to estimate a stock's future returns.
I know. Big shock, right?

However, there are ways to do portfolio optimization that works better.
We can do it by creating large groups 
of stocks with similar characteristics. 
For example, a factor portfolio. 
Then you estimate the expected return and covariance matrix using the factor.
That tends to give you better portfolios 
because the characteristics that you're using 
to form the portfolios help you estimate 
the return and covariances of the stocks in it.
*)

(**
## Download some stock returns
*)
let tickers = 
    [ 
        "VBR" // Vanguard Small-cap Value ETF
        "VUG" // Vanguard Growth ETF
        "VTI" // Vanguard Total Stock Market ETF
        "BND" // Vanguard Total Bond Market ETF
    ]

let tickPrices = 
    YahooFinance.History(
        tickers,
        startDate = DateTime(2010,1,1),
        interval = Monthly)

tickPrices[0..3]

(** Convert prices to returns. *)

/// Record to hold returns.
type StockReturn =
    { Symbol : string 
      Date : DateTime
      Return : float }

let pricesToReturns (symbol, adjPrices: list<Quote>) =
    adjPrices
    |> List.sortBy (fun x -> x.Date)
    |> List.pairwise
    |> List.map (fun (yesterday, today) ->
        let r = (log today.AdjustedClose) - (log yesterday.AdjustedClose) 
        { Symbol = symbol 
          Date = today.Date 
          Return = r })

let testPriceObs = 
    tickPrices
    |> List.filter (fun x -> x.Symbol = tickers[0])
    |> List.truncate 4

(** Looking at the results of grouping test prices by symbol. *)
testPriceObs
|> List.groupBy (fun x -> x.Symbol)


(** Same but calculating return observations. *)
testPriceObs
|> List.groupBy (fun x -> x.Symbol)
|> List.collect pricesToReturns

(** Now for all of the price data.*)
let tickReturns =
    tickPrices
    |> List.groupBy (fun x -> x.Symbol)
    |> List.collect pricesToReturns

(** A naive estimate of future returns is average past returns. *)
let tickAvgReturns =
    tickReturns
    |> List.groupBy (fun x -> x.Symbol)
    |> List.map (fun (sym, xs) ->
        let avgRet = xs |> List.averageBy (fun x -> x.Return)
        sym, avgRet)
    |> Map

(** Version indexed by ticker and date. *)
let bySymbolDate = 
    tickReturns
    |> List.map (fun x -> 
        let key = x.Symbol, x.Date
        key, x) 
    |> Map

let dates = 
    tickReturns
    |> List.map (fun x -> x.Date)
    |> List.distinct
    |> List.sort

let tickTensor =
    [ for ticker in tickers do
        [ for date in dates do
            match Map.tryFind (ticker, date) bySymbolDate with
            | None -> failwith $"Oops, no data for {ticker} on {date}"
            | Some obs -> obs.Return ] ]


tickTensor
|> matrix
|> Matrix.rowSampleCovarianceMatrixOf

type Tensor with
    /// <summary>
    /// Estimates the covariance matrix of the given tensor. The tensor's first
    /// dimension should index variables and the second dimension should
    /// index observations for each variable.
    /// </summary>
    /// <param name="correction">Difference between the sample size and the sample degrees of freedom. Defaults to 1 (Bessel's correction).</param>
    /// <returns></returns>
    member t.covariance(?correction:int) =
        if t.dim <> 2 then failwith "expects dim 0 is variables and dim 1 is observations"
        let correction = defaultArg correction 1
        let d = t.shape[1] - correction
        let deMeaned = t - t.mean(dim=1).unsqueeze(1)
        dsharp.matmul(deMeaned,deMeaned.transpose()) / d
    
let t = dsharp.tensor(tickTensor)
t.covariance()



let portfolioReturn (weights:Tensor) (avgRet:Tensor) = (weights * avgRet).sum()
let portfolioVariance (weights:Tensor) (cov:Tensor) = 
    weights.matmul(cov).matmul(weights)

let ew = dsharp.tensor([ 1.0 .. 4.0]).div(4.0)
let covMatrix = t.covariance()
/// correctly ordered average return tensor
let avgRetT = dsharp.tensor([ for t in tickers do tickAvgReturns[t] ])

portfolioReturn ew avgRetT
portfolioVariance ew covMatrix

let loss target w =
    let ret = portfolioReturn w avgRetT
    let var = portfolioVariance w covMatrix
    (ret - target)** 2.0 + var

let target = dsharp.tensor(15.0)
Optim.optim.sgd(loss target, x0= ew,iters=5)
dsharp.grad 


// solve A * x = b for x
let w' = dsharp.solve(covMatrix,avgRetT)
let w = w'.div(w'.sum())
