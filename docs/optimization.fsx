(**
---
title: Optimization
category: Lectures
categoryindex: 1
index: 9
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)
*)

#r "nuget: FSharp.Stats"
#r "nuget: FSharp.Data"
#r "nuget: DiffSharp-lite"

#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"

(** *)
#load "Common.fsx"
#r "nuget: Quotes.YahooFinance, 0.0.5"

open System
open FSharp.Data
open Common
open Quotes.YahooFinance

open FSharp.Stats
open Plotly.NET
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
# Portfolio Optimization

We're now going to see how to do mean-variance portfolio optimization.
The objective is to find the portfolio with the greatest return per
unit of standard deviation.

In particular, we're going to identify the tangency portfolio. 
The tangency portfolio is the portfolio fully invested 
in risky assets that has the maximum achievable sharpe ratio. 
When there is a risk-free rate, the efficient frontier 
of optimal portfolios is some combination of
the tangency portfolio and the risk-free asset. 
Investors who want safe portfolios hold a lot 
of risk-free bonds and very little of the tangency portfolio. 
Investors who want riskier portfolios hold little risk-free bonds and
a lot of the tangency portfolio (or even lever the tangency portoflio). 

One thing to keep in mind is that often you think 
of this as the optimal weight per security.
But one well known problem is that trying to do this naively does not work well.
And by naively I mean taking a stock's average return and covariances in the sample and using that to estimate optimal weights. 
In large part, this is because it is hard to estimate a stock's future returns.
I know. Big shock, right?

However, there are ways to do portfolio optimization that work better.
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
A type to hold our data.
*)

type StockData =
    { Symbol : string 
      Date : DateTime
      Return : float }

(**
We get the Fama-French 3-Factor asset pricing model data.
*)

let ff3 = French.getFF3 Frequency.Monthly |> Array.toList

// Transform to a StockData record type.
let ff3StockData =
    [ 
       ff3 |> List.map(fun x -> {Symbol="HML";Date=x.Date;Return=x.Hml})
       ff3 |> List.map(fun x -> {Symbol="MktRf";Date=x.Date;Return=x.MktRf})
       ff3 |> List.map(fun x -> {Symbol="Smb";Date=x.Date;Return=x.Smb})
    ] |> List.concat

(**
Let's get our factor data.
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

tickPrices[..3]

(**
A function to calculate returns from Price observations
*)
let pricesToReturns (symbol, adjPrices: list<Quote>) =
    adjPrices
    |> List.sortBy (fun x -> x.Date)
    |> List.pairwise
    |> List.map (fun (day0, day1) ->
        let r = day1.AdjustedClose / day0.AdjustedClose - 1.0 
        { Symbol = symbol 
          Date = day1.Date 
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

(**
And let's convert to excess returns
*)
let rf = Map [ for x in ff3 do x.Date, x.Rf ]

let standardInvestmentsExcess =
    let maxff3Date = ff3 |> List.map(fun x -> x.Date) |> List.max
    tickReturns
    |> List.filter(fun x -> x.Date <= maxff3Date)
    |> List.map(fun x -> 
        match Map.tryFind x.Date rf with 
        | None -> failwith $"why isn't there a rf for {x.Date}"
        | Some rf -> { x with Return = x.Return - rf })

(**
If we did it right, the `VTI` return should be pretty similar to the `MktRF`
return from Ken French's website.
*)

standardInvestmentsExcess
|> List.filter(fun x -> x.Symbol = "VTI" && x.Date.Year = 2021)
|> List.map(fun x -> x.Date.Month, round 4 x.Return)
|> List.take 3
(*** include-fsi-output***)

ff3 
|> List.filter(fun x -> x.Date.Year = 2021)
|> List.map(fun x -> x.Date.Month, round 4 x.MktRf)
|> List.take 3
(*** include-fsi-output***)

(**
Let's put our stocks in a map keyed by symbol
*)

let stockData =
    standardInvestmentsExcess
    |> List.groupBy(fun x -> x.Symbol)
    |> Map

(**
Look at an one
*)
stockData["VBR"][..3]

(**
Let's create a function that calculates covariances
for two securities using mutually overlapping data.
*)

let getCov xId yId (stockData: Map<string,StockData list>) =
    let xRet = 
        stockData[xId] 
        |> List.map (fun x -> x.Date,x.Return) 
        |> Map
    let yRet = 
        stockData[yId]
        |> List.map (fun y -> y.Date, y.Return)
        |> Map
    let overlappingDates =
        [ xRet.Keys |> set
          yRet.Keys |> set]
        |> Set.intersectMany
    [ for date in overlappingDates do xRet[date], yRet[date]]
    |> Seq.covOfPairs

getCov "VBR" "VTI" stockData

(**
A covariance matrix.
*)
(***hide***)
(*
let covariances =
    [ for rowTick in tickers do 
        [ for colTick in tickers do
            getCov rowTick colTick stockData ]]
    |> matrix
covariances
*)

(** *)
let covariances =
    [ for rowTick in tickers do 
        [ for colTick in tickers do
            getCov rowTick colTick stockData ]]
    |> dsharp.tensor

(**
Mean/Average returns
*)
(***hide***)
(*
let means = 
    [ for ticker in tickers do 
        stockData[ticker]
        |> List.averageBy (fun x -> x.Return)]
    |> vector
*)
(** *)
let means =
    [ for ticker in tickers do 
        stockData[ticker]
        |> List.averageBy (fun x -> x.Return)]
    |> dsharp.tensor

(**
This solution method for finding the tangency portfolio comes from Hilliar, Grinblatt, and Titman 2nd European Edition, Example 5.3. 

Since it has the greatest possible Sharpe ratio, that means that you cannot rebalance the portfolio and increase the return per unit of standard deviation.

The solution method relies on the fact that covariance is like marginal variance. At the tangency portfolio, it must be the case that the ratio of each asset's risk premium to it's covariance with the tangency portfolio, $(r_i-r_f)/cov(r_i,r_p)$, is the same. Because that ratio is the return per unit of "marginal variance" and if it was not equal for all assets, then you could rebalance and increase the portfolio's return while holding the portfolio variance constant.

In the below algebra, we solve for the portfolio that has covariances with each asset equal to the asset's risk premium. Then we relever to have a portfolio weight equal to 1.0 (we can relever like this because everything is in excess returns) and then we are left with the tangency portfolio.
*)

(***hide***)
(*
// solve A * x = b for x
let w' = Algebra.LinearAlgebra.SolveLinearSystem covariances means
let w = w' |> Vector.map(fun x -> x /  Vector.sum w')
*)

(** solve A * x = b for x *)
let w' = dsharp.solve(covariances,means)
let w = w' / w'.sum()

(*** include-it ***)

(**
Portfolio variance
*)
(***hide***)
(*
let portVariance = w.Transpose * covariances * w
*)
(** *)

let portVariance = w.matmul(covariances).matmul(w)

(**
Portfolio standard deviation
*)
let portStDev = portVariance.sqrt()

(** Portfolio mean *)
let portMean = dsharp.matmul(w, means)

// equivalent to
(w * means).sum()

(** Annualized Sharpe ratio *)
sqrt(12.0)*(portMean/portStDev)
(*** include-it ***)

(**
You can use other methods for constrained optimization.

https://github.com/mathnet/mathnet-numerics/blob/ab1ac92ccab575d51f6967cd785254a46210b4dd/src/Numerics.Tests/OptimizationTests/NonLinearCurveFittingTests.cs#L361
*)

(**
## Comparing mean-variance efficient to 60/40.

Now let's form the mean-variance efficient portfolios
based on the above optimal weights and compare them to
a 60/40 portfolio over our sample. A 60% stock and 40%
bond portfolio is a common investment portfolio.

Our weights are sorted by `symbols`. Let's put them into a
Map collection for easier referencing.
*)

let weights =
    Seq.zip tickers (w.toArray1D<float>())
    |> Map.ofSeq

(**
Next, we'd like to get the symbol data grouped by date.
*)

let stockDataByDate =
    stockData.Values
    |> Seq.toList
    |> List.collect id // combine all different StockData symbols into one list.
    |> List.groupBy(fun x -> x.Date) // group all symbols on the same date together.
    |> List.sortBy fst // sort by the grouping variable, which here is Date.

(**
Now if we look we do not have all the symbols on all the dates.
This is because our ETFs (VTI, BND) did not start trading until later in our sample
and our strategy ended at the end of the year, while we have stock quotes
for VTI and BND trading recently.

Compare the first month of data
*)

let firstMonth =
    stockDataByDate 
    |> List.head // first date group
    |> snd // convert (date, StockData list) -> StockData list
// look at it
firstMonth
(*** include-it ***)
// How many stocks?
firstMonth.Length
(*** include-it ***)

(**
to the last month of data
*)
// Last item
let lastMonth =
    stockDataByDate 
    |> List.last // last date group
    |> snd // convert (date, StockData list) -> StockData list
// look at it
lastMonth
(*** include-it ***)
// How many stocks?
lastMonth.Length
(*** include-it ***)

(**
What's the first month when you have all 3 assets?
*)

let allAssetsStart =
    stockDataByDate
    // find the first array element where there are as many stocks as you have symbols
    |> List.find(fun (month, stocks) -> stocks.Length = tickers.Length)
    |> fst // convert (month, stocks) to month

let allAssetsEnd =
    stockDataByDate
    // find the last array element where there are as many stocks as you have symbols
    |> List.findBack(fun (month, stocks) -> stocks.Length = tickers.Length)
    |> fst // convert (month, stocks) to month

(**
Ok, let's filter our data between those dates.
*)

let stockDataByDateComplete =
    stockDataByDate
    |> List.filter(fun (date, stocks) -> 
        date >= allAssetsStart &&
        date <= allAssetsEnd)

(**
Double check that we have all assets in all months for this data.
*)

let checkOfCompleteData =
    stockDataByDateComplete
    |> List.map snd
    |> List.filter(fun x -> x.Length <> tickers.Length) // discard rows where we have all symbols.

if not (List.isEmpty checkOfCompleteData) then 
        failwith "stockDataByDateComplete has months with missing stocks"

(**
Now let's make my mve and 60/40 ports

To start, let's take a test month so that it is easy to see
what we are doing.
*)

let testMonth =
    stockDataByDateComplete
    |> List.find(fun (date, stocks) -> date = allAssetsStart)
    |> snd

let testBnd = testMonth |> List.find(fun x -> x.Symbol = "BND")
let testVti = testMonth |> List.find(fun x -> x.Symbol = "VTI")
let testVBR = testMonth |> List.find(fun x -> x.Symbol = "VBR")

testBnd.Return*weights.["BND"] +
testVti.Return*weights.["VTI"] +
testVBR.Return*weights.["VBR"]
(*** include-it ***)

(** Or, same thing but via iterating through the weights.*)
[ for KeyValue(symbol, weight) in weights do
    let symbolData = testMonth |> List.find(fun x -> x.Symbol = symbol)
    symbolData.Return*weight]
|> List.sum    

(*** include-it ***)

(**
Now in a function that takes weights and monthData as input
*)
let portfolioMonthReturn weights monthData =
    weights
    |> Map.toList
    |> List.map(fun (symbol, weight) ->
        let symbolData = 
            // we're going to be more safe and use tryFind here so
            // that our function is more reusable
            match monthData |> List.tryFind(fun x -> x.Symbol = symbol) with
            | None -> failwith $"You tried to find {symbol} in the data but it was not there"
            | Some data -> data
        symbolData.Return*weight)
    |> List.sum    
    
portfolioMonthReturn weights testMonth

(**
Here's a thought. We just made a function that takes
weights and a month as input. That means that it should
work if we give it different weights.

Let's try to give it 60/40 weights.
*)

let weights6040 = Map [("VTI",0.6);("BND",0.4)]

weights6040.["VTI"]*testVti.Return +
weights6040.["BND"]*testBnd.Return

(*** include-it ***)

(**
Now compare to
*)
portfolioMonthReturn weights6040 testMonth
(*** include-it ***)

(**
Now we're ready to make our mve and 60/40 portfolios.
*)

let portMve =
    stockDataByDateComplete
    |> List.map(fun (date, data) -> 
        { Symbol = "MVE"
          Date = date
          Return = portfolioMonthReturn weights data })

let port6040 = 
    stockDataByDateComplete
    |> List.map(fun (date, data) -> 
        { Symbol = "60/40"
          Date = date 
          Return = portfolioMonthReturn weights6040 data} )


(**
It is nice to plot cumulative returns.
*)

(** 
A function to accumulate returns.
*)


let cumulateReturns (xs: list<StockData>) =
    let folder (prev: StockData) (current: StockData) =
        let newReturn = prev.Return * (1.0+current.Return)
        { current with Return = newReturn}
    
    match xs |> List.sortBy (fun x -> x.Date) with
    | [] -> []
    | h::t ->
        ({ h with Return = 1.0+h.Return}, t) 
        ||> List.scan folder
    

(**
Ok, cumulative returns.
*)
let portMveCumulative = 
    portMve
    |> cumulateReturns

let port6040Cumulative = 
    port6040
    |> cumulateReturns


let chartMVE = 
    portMveCumulative
    |> List.map(fun x -> x.Date, x.Return)
    |> Chart.Line
    |> Chart.withTraceInfo(Name="MVE")

let chart6040 = 
    port6040Cumulative
    |> List.map(fun x -> x.Date, x.Return)
    |> Chart.Line
    |> Chart.withTraceInfo(Name="60/40")

let chartCombined =
    [ chartMVE; chart6040 ]
    |> Chart.combine

(*** condition: fsx ***)
#if FSX
chartCombined |> Chart.show
#endif // FSX

(*** condition: ipynb ***)
#if IPYNB
chartCombined
#endif // IPYNB

(*** hide ***)
chartCombined |> GenericChart.toChartHTML
(*** include-it-raw ***)

(**
Those are partly going to differ because they have different volatilities.
It we want to have a sense for which is better per unit of volatility,
then it can make sense to normalize volatilities.
*)

(**
First compare the MVE vol
*)

portMve
|> List.map(fun x -> x.Return)
|> Seq.stDev
|> fun vol -> sqrt(12.0) * vol

(*** include-it ***)

(**
To the 60/40 vol.
*)

port6040
|> List.map(fun x -> x.Return)
|> Seq.stDev
|> fun vol -> sqrt(12.0)*vol

(*** include-it ***)

(**
Ok, cumulative returns of the normalized vol returns.
*)

let normalize10pctVol xs =
    let vol = xs |> List.map(fun x -> x.Return) |> Seq.stDev
    let annualizedVol = vol * sqrt(12.0)
    xs 
    |> List.map(fun x -> { x with Return = x.Return * (0.1/annualizedVol)})

let portMveCumulativeNormlizedVol = 
    portMve
    |> normalize10pctVol
    |> cumulateReturns

let port6040CumulativeNormlizedVol = 
    port6040
    |> normalize10pctVol 
    |> cumulateReturns


let chartMVENormlizedVol = 
    portMveCumulativeNormlizedVol
    |> List.map(fun x -> x.Date, x.Return)
    |> Chart.Line
    |> Chart.withTraceInfo(Name="MVE")

let chart6040NormlizedVol = 
    port6040CumulativeNormlizedVol
    |> List.map(fun x -> x.Date, x.Return)
    |> Chart.Line
    |> Chart.withTraceInfo(Name="60/40")

let chartCombinedNormlizedVol =
    [ chartMVENormlizedVol; chart6040NormlizedVol ]
    |> Chart.combine

(*** condition: fsx ***)
#if FSX
chartCombinedNormlizedVol |> Chart.show
#endif // FSX

(*** condition: ipynb ***)
#if IPYNB
chartCombinedNormlizedVol
#endif // IPYNB

(*** hide ***)
chartCombinedNormlizedVol |> GenericChart.toChartHTML
(*** include-it-raw ***)

(**
## Key points to keep in mind.
The mean-variance efficient portfolio will always look like the best portfolio in the sample period in which you estimated the weights. This is because we found it by literally looking for the portfolio with the highest sharpe ratio in that sample period.

A more meaningful comparison would be to estimate mean-variance efficient weights based on past data and see how those weights perform in future data. For instance, estimate weights 2000-2010, and use those weights to determine the portfolio that you're going to hold in 2011. Finally, compare it to 60/40 in 2011. That is an "out of sample" test because your test period (2011) is different from the period when the weights were estimated (2000-2010). Then repeat, always using data *before* the holding period as your training period to estimate the weights for the test holding period. 

It is also important to remember that 10-20 years is not long enough to get a good estimate of a portfolio's expected return. 

One way to see this is to compare equity returns 2000-2010 and 2010-2020.
*)

ff3
|> Seq.filter(fun x -> 
    x.Date >= DateTime(2000,1,1) &&
    x.Date <= DateTime(2009,12,31))
|> Seq.averageBy(fun x ->
    12.0*x.MktRf)

(*** include-it ***)

ff3
|> Seq.filter(fun x -> 
    x.Date >= DateTime(2010,1,1) &&
    x.Date <= DateTime(2019,12,31))
|> Seq.averageBy(fun x ->
    12.0*x.MktRf)

(*** include-it ***)

(**
Neither of those 10-year periods is a good estimate of expected market returns.Thus it does not make sense to try forming a mean-variance efficient portfolio using the trailing 10-year period for estimating forward-looking returns.

If we look at US returns 1900-2012, the data indicates that equity excess returns were about 5.5%, and bond excess returns were about 1%. Covariances over shorter periods are more reasonable, so we can use the recent sample to estimate covariances and the long sample for means.
*)

let symStockBond = ["VTI";"BND"]
let covStockBond =
    [ for x in symStockBond do
        [ for y in symStockBond do
            getCov x y stockData ]]
    |> dsharp.tensor

let meansStockBond = dsharp.tensor([ 0.055/12.0; 0.01/12.0])

let wStockBond =
    let w' = dsharp.solve(covStockBond, meansStockBond)
    w' / w'.sum()

wStockBond
(*** include-it ***)

let stockBondSharpeAndSD (weights:Tensor) =
    let sbVar = weights.matmul(covStockBond).matmul(weights)
    let sbStDev = sqrt(12.0)*sqrt(sbVar)
    let sbMean = 12.0 * (weights.matmul(meansStockBond))
    sbMean/sbStDev, sbStDev

stockBondSharpeAndSD wStockBond
(*** include-it ***)

stockBondSharpeAndSD (dsharp.tensor([0.6;0.4]))
(*** include-it ***)
