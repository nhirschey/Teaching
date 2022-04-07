(**
---
title: Performance Evaluation
category: Lectures
categoryindex: 1
index: 7
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


# Performance evaluation

We're going to evaluate portfolio performance. The common way to do this is to estimate a portfolio's return adjusted for risk using a factor model with tradeable risk factors. 

What's a risk factor? These risk factors are portfolios and the idea is that the expected excess return on these risk factors is compensation to investors for bearing the risk inherent in holding those portfolios. For the return variation in these factors to be "risky", it should be something that investors cannot easily diversify. If it was easy to diversify, then investors could put a small bit of the asset in their portfolio and capture the return without affecting portfolio volatility. That would imply being able to increase return without adding risk. Hence the requirement that a factor constitute return variation that is hard to diversify away.

The greater the riskiness of the factor, the greater the factor's expected return (i.e., the risk-return tradeoff). For example, most people feel that stocks are riskier than bonds and indeed stocks have historically had higher returns than bonds.

The risk adjustment involves estimating a portfolio's $\beta$'s on different risk factors. These $\beta$'s constitute the exposure of the portfolio to the risk factor. If the factor return goes up by 1%, then the portfolio's return goes up by $\beta \times 1\%$. 

We can estimate these $\beta$'s by OLS regressions of the portfolio's returns on contemporaneous returns of the risk factors. The slope coefficients on the risk factors are the portfolio's betas on the risk factors. The regression intercept is known as $\alpha$. It represents the average return of the portfolio that is not explained by the portfolio's $\beta$'s on the risk factors. This alpha is the risk-adjusted return. 

Intuitively, $\alpha$ is the average return on a portfolio long the investment you are evaluating and short a portfolio with the same factor risk as that portfolio. If the factors and factor betas accurately measure the portfolio's risk, then the alpha is the portfolio's return that is unrelated to the portfolio's risk. Investors like positive alphas because that implies that the portfolio's return is higher than what investors require for bearing the portfolio's risk.

One thing to keep in mind is that throughout this discussion, we have discussed things from the perspective of arbitrage. That is, like a trader. We have not made any assumptions about utility functions or return distributions. This is the Arbitrage Pricing Theory (APT) of Stephen Ross (1976). He was motivated by the observation that

> "... on theoretical grounds it is difficult to justify either the assumption [in mean-variance anlysis and CAPM] of normality in returns...or of quadratic preferences...and on empirical grounds the conclusions as well as the assumptions of the theory have also come under attack."

The APT way of thinking is less restrictive than economically motivated equilibrium asset pricing models. Which is nice. But it has the cost that it does not tell us as much. With the APT we cannot say precisely what a security's return should be. We can only say that if we go long a portfolio and short the portfolio that replicates its factor exposure, then the alpha shouldn't be *too* big. But if we're thinking like a trader, that's perhaps most of what we care about anyway.


*)

#r "nuget: FSharp.Stats"
#r "nuget: FSharp.Data"
#r "nuget: Plotly.NET, 2.0.0-preview.17"
#r "nuget: Plotly.NET, 2.0.0-preview.17"

#load "Common.fsx"
#load "YahooFinance.fsx"

open System
open FSharp.Data
open Common
open YahooFinance

open FSharp.Stats
open Plotly.NET

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__


(*** condition: fsx ***)
#if FSX
fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))
fsi.AddPrinter<YearMonth>(fun ym -> $"{ym.Year}-{ym.Month}")
#endif // FSX

(*** condition: ipynb ***)
#if IPYNB
// Set dotnet interactive formatter to plaintext
Formatter.Register(fun (x:obj) (writer: TextWriter) -> fprintfn writer "%120A" x )
Formatter.SetPreferredMimeTypesFor(typeof<obj>, "text/plain")
// Make plotly graphs work with interactive plaintext formatter
Formatter.SetPreferredMimeTypesFor(typeof<GenericChart.GenericChart>,"text/html")
#endif // IPYNB


(**
We get the Fama-French 3-Factor asset pricing model data.
*)

let ff3 = French.getFF3 Frequency.Monthly

(**
Let's get a portfolio to analyze.
*)

type Return = { YearMonth : DateTime; Return : float }
        

let vbr = 
    YahooFinance.PriceHistory("VBR",
                              startDate=DateTime(2010,1,1),
                              endDate=DateTime(2021,12,31),
                              interval=Interval.Monthly)
    |> List.sortBy (fun x -> x.Date)
    |> List.pairwise
    |> List.map (fun (yesterday, today) -> 
        { YearMonth = DateTime(today.Date.Year,today.Date.Month,1)
          Return = today.AdjustedClose/yesterday.AdjustedClose - 1.0 })
    |> List.toArray

(** A function to accumulate simple returns. *)
let cumulativeReturn (xs: seq<DateTime * float>) =
    let h::t = xs |> Seq.sortBy fst |> Seq.toList
    /// cr0 is a cumulative return through dt0.
    /// r1 is the return only for period dt1.
    let accumulate (dt0, cr0) (dt1, r1) =
        let cr1 = (1.0 + cr0) * (1.0 + r1) - 1.0
        (dt1, cr1)
    (h, t) ||> List.scan accumulate    

(** Plot of vbr cumulative return. *)
let vbrChart =
    vbr
    |> Array.map (fun x -> x.YearMonth, x.Return)
    |> cumulativeReturn
    |> Chart.Line

(***condition:ipynb***)
#if IPYNB
vbrChart
#endif // IPYNB

(***condition:fsx***)
#if FSX
vbrChart |> Chart.show
#endif // FSX

(***hide***)
vbrChart
|> GenericChart.toChartHTML
(*** include-it-raw ***)

(**
For regression, it is helpful to have the portfolio
return data merged into our factor model data.
*)
type RegData =
    { Date : DateTime
      Portfolio : float
      MktRf : float 
      Hml : float 
      Smb : float }

// ff3 indexed by month
// We're not doing date arithmetic, so I'll just
// use DateTime on the 1st of the month to represent a month
let ff3ByMonth = 
    ff3
    |> Array.map(fun x -> DateTime(x.Date.Year, x.Date.Month,1), x)
    |> Map

let regData =
    vbr 
    |> Array.map(fun port ->
        let monthToFind = DateTime(port.YearMonth.Year,port.YearMonth.Month,1)
        match Map.tryFind monthToFind ff3ByMonth with
        | None -> failwith "probably you messed up your days of months"
        | Some ff3 -> 
            { Date = monthToFind
              Portfolio = port.Return - ff3.Rf
              MktRf = ff3.MktRf 
              Hml = ff3.Hml 
              Smb = ff3.Smb })

(** One way to evaluate performance is Sharpe ratios. *)

/// Calculates sharpe ratio of a sequence of excess returns
let sharpe (xs: float seq) =
    (Seq.mean xs) / (Seq.stDev xs)

let annualizeMonthlySharpe monthlySharpe = sqrt(12.0) * monthlySharpe
    

(** Our portfolio. *)
regData
|> Array.map (fun x -> x.Portfolio)
|> sharpe
|> annualizeMonthlySharpe


(** The market. *)
regData
|> Array.map (fun x -> x.MktRf)
|> sharpe
|> annualizeMonthlySharpe

(** The HML factor. *)
regData
|> Array.map (fun x -> x.Hml)
|> sharpe
|> annualizeMonthlySharpe

(**
[Accord.NET](http://accord-framework.net/) is a .NET (C#/F#/VB.NET) machine learning library. 

*)

#r "nuget: Accord"
#r "nuget: Accord.Statistics"

open Accord
open Accord.Statistics.Models.Regression.Linear


(** 
The OLS trainer is documented [here](https://github.com/accord-net/framework/wiki/Regression) with an example in C#. 

We'll use it in a a more F# way
*) 

type RegressionOutput =
    { Model : MultipleLinearRegression 
      TValuesWeights : float array
      TValuesIntercept : float 
      R2: float }

/// Type alias for x, y regression data 
type XY = (float array) array * float array

let fitModel (x: (float array) array, y: float array) =
    let ols = new OrdinaryLeastSquares(UseIntercept=true)
    let estimate = ols.Learn(x,y)
    let mse = estimate.GetStandardError(x,y)
    let se = estimate.GetStandardErrors(mse, ols.GetInformationMatrix())
    let tvaluesWeights = 
        estimate.Weights
        |> Array.mapi(fun i w -> w / se.[i])
    let tvalueIntercept = estimate.Intercept / (se |> Array.last)
    let r2 = estimate.CoefficientOfDetermination(x,y)
    { Model = estimate
      TValuesWeights = tvaluesWeights
      TValuesIntercept = tvalueIntercept  
      R2 = r2 }

let capmModelData = 
    regData
    |> Array.map(fun obs -> [|obs.MktRf|], obs.Portfolio)
    |> Array.unzip 

let ff3ModelData = 
    regData
    |> Array.map(fun obs -> [|obs.MktRf; obs.Hml; obs.Smb |], obs.Portfolio)
    |> Array.unzip

(**
Now we can estimate our models.
*)
let capmEstimate = capmModelData |> fitModel
let ff3Estimate = ff3ModelData |> fitModel


(**
CAPM results.
*)

capmEstimate.Model
(*** include-fsi-output***)

capmEstimate.TValuesIntercept
(*** include-fsi-output***)

capmEstimate.R2 
(*** include-fsi-output***)

(**
Fama-French 3-Factor model results
*)

ff3Estimate.Model
(*** include-fsi-output***)

ff3Estimate.TValuesIntercept
(*** include-fsi-output***)

ff3Estimate.R2
(*** include-fsi-output***)

(**
You will probably see that the CAPM $R^2$ is lower than the
Fama-French $R^2$. This means that you can explain more of the
portfolio's returns with the Fama-French model. Or in trader terms,
you can hedge the portfolio better with the multi-factor model.
*)

(**
We also want predicted values so that we can get regression residuals for calculating
the information ratio. 

*)

type Prediction = { Label : float; Score : float}

let makePredictions 
    (estimate:MultipleLinearRegression) 
    (x: (float array) array, y: float array) =
    (estimate.Transform(x), y)
    ||> Array.zip
    |> Array.map(fun (score, label) -> { Score = score; Label = label })

let residuals (xs: Prediction array) = xs |> Array.map(fun x -> x.Label - x.Score)

let capmPredictions = makePredictions capmEstimate.Model capmModelData
let ff3Predictions = makePredictions ff3Estimate.Model ff3ModelData

capmPredictions |> Array.take 3
(*** include-output ***)

capmPredictions |> residuals |> Array.take 3
(*** include-fsi-output ***)

let capmResiduals = residuals capmPredictions
let ff3Residuals = residuals ff3Predictions

(**
In general I would write a function to do this. Function makes it a bit
simpler to follow. It's hard for me to read the next few lines and understand
what everything is. Too much going on.
*)

let capmAlpha = 12.0 * capmEstimate.Model.Intercept 
let capmStDevResiduals = sqrt(12.0) * (Seq.stDev capmResiduals)
let capmInformationRatio = capmAlpha / capmStDevResiduals
(*** include-fsi-output***)

let ff3Alpha = 12.0 * ff3Estimate.Model.Intercept 
let ff3StDevResiduals = sqrt(12.0) * (Seq.stDev ff3Residuals)
let ff3InformationRatio = ff3Alpha / ff3StDevResiduals
(*** include-fsi-output***)

// Function version

let informationRatio monthlyAlpha (monthlyResiduals: float array) =
    let annualAlpha = 12.0 * monthlyAlpha
    let annualStDev = sqrt(12.0) * (Seq.stDev monthlyResiduals)
    annualAlpha / annualStDev 

informationRatio capmEstimate.Model.Intercept capmResiduals
(*** include-fsi-output***)

informationRatio ff3Estimate.Model.Intercept ff3Residuals
(*** include-fsi-output***)

