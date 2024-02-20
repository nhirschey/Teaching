(**
---
title: Revisiting Mean-variance optimal risky weights
category: Lectures
categoryindex: 1
index: 13
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

# Predictability

We'll revisit some old topics but also touch on new ones.
Objectives:


- Simple vol managed
- Mean-variance efficient version
    - known mean and known variance
    - known mean and predicted variance
    - predicted mean and known variance
    - predicted mean and predicted variance
- Adding leverage constraints
- Effect of different volatility predictions.

*)

#r "nuget: NovaSBE.Finance, 0.5.0"
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: ExcelProvider, 2.0.0"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"

open System
open NovaSBE.Finance
open NovaSBE.Finance.Utils
open FSharp.Stats
open Plotly.NET
open Plotly.NET.Interactive

let ff3 = French.getFF3 French.Monthly
let ff3Daily = French.getFF3 French.Daily

(**
## Vol Managed Portfolios

### Simple vol managed

Following Moreia and Muir (2017) this strategy is $w_t = \frac{c}{\hat{\sigma}^2_t}$ where $c$ is a constant that sets the strategie's standard deviation to that of the buy-hold portfolio.

The variance prediction is based on realized variance the prior month.
*)

type ManagedObs =
    { Month: DateTime 
      RiskyWeight: float 
      Return: float }

/// Realized variance from month t-1 to predict month t. The key is month t, the value is predicted variance
let mmVarPred =
    ff3Daily
    |> Array.groupBy (fun x -> DateTime(x.Date.Year, x.Date.Month, 1))
    |> Array.map (fun (month, dayObs) ->
        let mu = dayObs |> Array.averageBy (fun x -> x.MktRf)
        let realizedVar = 
            dayObs 
            |> Array.map (fun x -> (x.MktRf - mu) ** 2.0) 
            |> Array.sum
        month.AddMonths(1), realizedVar)
    |> Map

let minMonth = mmVarPred.Keys |> Seq.min
let maxMonth = mmVarPred.Keys |> Seq.max
    
let simpleManaged =
    ff3
    |> Array.filter (fun x -> x.Date >= minMonth && x.Date <= maxMonth)
    |> Array.map (fun x ->
        let var = mmVarPred[x.Date]
        let w = 1.0 / var
        { Month = x.Date
          RiskyWeight = w
          Return = w * x.MktRf })


let buyHold =
    ff3
    |> Array.filter (fun x -> x.Date >= minMonth && x.Date <= maxMonth)
    |> Array.map (fun x ->
        { Month = x.Date
          RiskyWeight = 1.0
          Return = x.MktRf })

(** Let's compare the unnormalized managed vol port and the buy and hold.*)

let annualizedStDev (xs: seq<ManagedObs>) =
    xs
    |> Seq.map (fun x -> x.Return)
    |> stDev
    |> (*) (sqrt 12.0)

(** unnormalized managed vol. *)
let unnormalizedStDev =
    simpleManaged
    |> annualizedStDev
unnormalizedStDev

(** buy and hold. *)
let buyHoldStDev =
    buyHold
    |> annualizedStDev

(** Normalized managed vol*)
let mmConstant = buyHoldStDev / unnormalizedStDev
mmConstant

(** Normalized the strategy. *)
let simpleManaged2 =
    simpleManaged
    |> Array.map (fun x -> 
        { x with 
            RiskyWeight = mmConstant * x.RiskyWeight
            Return = mmConstant * x.Return })

simpleManaged2
|> annualizedStDev

(** Plot all portfolios. *)

let accumulateFromOne (xs: seq<ManagedObs>) =
    let sortedXs = xs |> Seq.sortBy (fun x -> x.Month)
    let mutable cr = 1.0
    [| for x in sortedXs do 
        cr <- cr * (1.0 + x.Return )
        x.Month, cr |]

[ "Buy and Hold", buyHold
  "Unnormalized Managed Vol", simpleManaged
  "Normalized Managed Vol", simpleManaged2 ]
|> List.map (fun (name, port) ->
    port
    |> accumulateFromOne
    |> Chart.Line
    |> Chart.withTraceInfo(Name=name))
|> Chart.combine
|> Chart.withYAxisStyle(AxisType=StyleParam.AxisType.Log)

(** Unnormalized doesn't make any sense.*)
[ "Buy and Hold", buyHold
  "Managed Vol", simpleManaged2 ]
|> List.map (fun (name, port) ->
    port
    |> accumulateFromOne
    |> Chart.Line
    |> Chart.withTraceInfo(Name=name))
|> Chart.combine
|> Chart.withYAxisStyle(AxisType=StyleParam.AxisType.Log)

(** Look at sharpe ratios. *)

let sharpe (xs: seq<ManagedObs>) =
    let sd = xs |> annualizedStDev
    let mean = xs |> Seq.averageBy (fun x -> 12.0 * x.Return)
    mean / sd

$"Buy and Hold: {buyHold |> sharpe}"
$"Unnormalized Managed Vol: {simpleManaged |> sharpe}"
$"Managed Vol: {simpleManaged2 |> sharpe}"

(** By decade. *)
let sharpeByDecade (xs: seq<ManagedObs>) =
    xs
    |> Seq.groupBy (fun x -> 10 * (x.Month.Year / 10))
    |> Seq.map (fun (decade, decadeObs) ->
        {| Decade = decade; Sharpe = sharpe decadeObs|})
    |> Array.ofSeq

let buyHoldDecades = buyHold |> sharpeByDecade
let simpleManaged2Decades = simpleManaged2 |> sharpeByDecade

for i = 0 to buyHoldDecades.Length-1 do 
    printfn $"Decade: {buyHoldDecades[i].Decade}"
    printfn $"  Buy-Hold/Managed: %.2f{buyHoldDecades[i].Sharpe} / %.2f{simpleManaged2Decades[i].Sharpe}"

(**
Check alphas
*)

let ff3Map = ff3 |> Array.map (fun x -> x.Date, x) |> Map

let regData =
    simpleManaged2
    |> Array.filter (fun x -> x.Month <= DateTime(2015,12,31) && x.Month >= DateTime(1927,1,1))
    |> Array.map (fun x ->
        {| Month = x.Month
           Managed = x.Return * 12.0 
           MktRf = ff3Map[x.Month].MktRf * 12.0 |})

open NovaSBE.Finance.Ols

Ols("Managed ~ MktRf", regData).fit().summary()

(**
### Mean-variance optimal managed vols

Mean-variance utility
*)

let mvUtility gamma mu sigma =
    mu - 0.5 * gamma * sigma ** 2.0

/// Annualized buy-and-hold sample return.
let buyHoldAvg = buyHold |> Array.averageBy (fun x -> 12.0 * x.Return)

printfn $"Mean: {buyHoldAvg}"
printfn $"StDev: {buyHoldStDev}"

mvUtility 3.0 buyHoldAvg buyHoldStDev

(** Function to to it for our managed portfolios. *)

let managedMVUtility gamma (xs: seq<ManagedObs>) =
    let avg = xs |> Seq.averageBy (fun x -> 12.0 * x.Return)
    let stDev = xs |> annualizedStDev
    mvUtility gamma avg stDev

let portSummary name port =
    printfn $"_________\n{name}\nNumber of months: {Seq.length port}"
    printfn $"Sharpe: {port |> sharpe}"
    printfn $"MV Utility: {managedMVUtility 3.0 port}"
    printfn $"annaluzed stdev: {port |> annualizedStDev}"

portSummary "Buy and Hold" buyHold
portSummary "Unnormalized Managed Vol" simpleManaged
portSummary "Normalized Managed Vol" simpleManaged2

(**
Mean-variance optimal weight.
*)

let mvWeight gamma mu sigma =
    mu / (gamma * sigma ** 2.0)

mvWeight 3.0 buyHoldAvg buyHoldStDev

(**
Managed portfolio assuming we knew the full-sample mean and standard deviation ahead of time.
We cannot use this to trade, because it uses future information.
But it's a good benchmark.
*)
let w_staticMuSigma = mvWeight 3.0 buyHoldAvg buyHoldStDev
let managedStaticMuSigma =
    buyHold
    |> Array.map (fun x -> 
        { x with 
            RiskyWeight = w_staticMuSigma
            Return = w_staticMuSigma * x.Return })

portSummary "Managed, known mu and sigma" managedStaticMuSigma
portSummary "Managed, base case" simpleManaged2

(**
Let's now try with a static mean return estimate, but we'll use the rolling variance estimate.

*)

/// Calculates a mean-variance optimal port given gamma and estimates of mu's and sigma's.
let managedMVPort gamma muEstimates varEstimates =
    ff3
    |> Array.choose (fun x ->
        let sd = varEstimates |> Map.tryFind x.Date |> Option.map sqrt
        let mu = muEstimates |> Map.tryFind x.Date
        match sd, mu with
        | Some sd, Some mu ->
            let w = mvWeight 3.0 mu sd
            let result = 
                { Month = x.Date
                  RiskyWeight = w
                  Return = w * x.MktRf }
            Some result
        | _ -> None)


(** Full sample 'estimates'*)
let muEstFull = [ for x in ff3 do x.Date, buyHoldAvg / 12.0 ] |> Map
let sigmaEstFull = [ for x in ff3 do x.Date, buyHoldStDev / sqrt 12.0 ] |> Map

portSummary "Managed, static mu and static sigma" managedStaticMuSigma
portSummary "Managed, static mu and rolling sigma" (managedMVPort 3.0 muEstFull sigmaEstFull)

(** Now actual static mu but rolling sigma estimates. *)
let managedStaticMuRollingSigma = managedMVPort 3.0 muEstFull mmVarPred

portSummary "Managd, static mu and rolling sigma" managedStaticMuRollingSigma
portSummary "Managed, base case" simpleManaged2

(** How volatile is that portfolio?*)
let sigmaStaticMuRollingSigma = managedStaticMuRollingSigma |> annualizedStDev
sigmaStaticMuRollingSigma

(** Let's try rescaling our static mu rolling sigma.*)

let managedStaticMuRollingSigma2 = 
    managedStaticMuRollingSigma
    |> Array.map (fun x -> 
        let c = buyHoldStDev / sigmaStaticMuRollingSigma
        { x with 
            RiskyWeight = x.RiskyWeight * c
            Return = x.Return * c })

portSummary "Managed, static mu and rolling sigma" managedStaticMuRollingSigma2
portSummary "Managed base case" simpleManaged2

(** Why is the ex-post rescaling so critical to the utility calculation?*)
let managedHiVolDays =
    managedStaticMuRollingSigma
    |> Array.sortByDescending (fun x -> abs x.Return)
    |> Array.take 10
managedHiVolDays

(** Let's compare predicted and realized vols on those bad miss days.*)
[ for x in managedHiVolDays do
    let sd = sqrt (mmVarPred[x.Month]*12.0)
    let realized = (sqrt 12.0) * abs ff3Map[x.Month].MktRf
    {| Month = x.Month 
       RiskyWeight = x.RiskyWeight
       Predicted = sd
       Realized = realized |} ]

(** We might be able to control this ex-ante by using reasonable leverage limits.*)
let managedMVPortLimited gamma muEstimates varEstimates =
    ff3
    |> Array.choose (fun x ->
        let sd = varEstimates |> Map.tryFind x.Date |> Option.map sqrt
        let mu = muEstimates |> Map.tryFind x.Date
        match sd, mu with
        | Some sd, Some mu ->
            let w = mvWeight 3.0 mu sd
            let w2 = 
                if w > 2.0 then 2.0 
                elif w < 0.0 then 0.0 
                else w
            let result = 
                { Month = x.Date
                  RiskyWeight = w2
                  Return = w2 * x.MktRf }
            Some result
        | _ -> None)

let managedStaticMuRollingSigma3 = managedMVPortLimited 3.0 muEstFull mmVarPred


portSummary "Managed, static mu and rolling sigma" managedStaticMuRollingSigma3
portSummary "Managed base case" simpleManaged2
portSummary "Buy-and-hold" buyHold

(** Rolling 'mu estimates', because our portfolios still have a "look-ahead" bias becuase they know what
future returns will be.*)
let muExpandingEstimate =
    let mutable acc = 0.0
    [| for i = 0 to ff3.Length-1 do
        acc <- acc + ff3[i].MktRf
        ff3[i].Date, acc / (float i + 1.0) |]

muExpandingEstimate
|> Chart.Line


(** How big of a burn-in period to use? Let's just use post-war period.*)
let muExpandingMap = muExpandingEstimate |> Map
let managedEstAll = 
    managedMVPortLimited 3.0 muExpandingMap mmVarPred
    |> Array.filter (fun x -> x.Month >= DateTime(1945,1,1))

let buyHold1945 = buyHold |> Array.filter (fun x -> x.Month >= DateTime(1945,1,1))
let simpleManaged21945 = 
    let since1945 = simpleManaged2 |> Array.filter (fun x -> x.Month >= DateTime(1945,1,1))
    let sd = since1945 |> annualizedStDev
    let sdBuyHold = buyHold1945 |> annualizedStDev
    let c = sdBuyHold / sd
    since1945
    |> Array.map (fun x -> 
        { x with 
            RiskyWeight = x.RiskyWeight * c 
            Return = x.Return * c })

portSummary "Managed, rolling mu and rolling sigma" managedEstAll
portSummary "Managed base case" simpleManaged21945
portSummary "Buy-and-hold" buyHold1945

(** Comparin decades.*)
let buyHold1945Decades = buyHold1945 |> sharpeByDecade
let managedEstAllDecades = managedEstAll |> sharpeByDecade

for i = 0 to buyHold1945Decades.Length-1 do 
    printfn $"Decade: {buyHold1945Decades[i].Decade}"
    printfn $"  Buy-Hold/Managed: %.2f{buyHold1945Decades[i].Sharpe} / %.2f{managedEstAllDecades[i].Sharpe}"

(**
### Comparing Vol forecasts
*)

type ReturnObs = { Date: DateTime; Return: float}

type VolatilityPrediction = 
    { /// First date the prediction is valid for
      Date: DateTime
      /// The volatility prediction
      PredictedVol: float }

/// <summary>Calculates realized volatity of a return observation using an exponential weight.</summary>
/// <param name="width">The window width for calculating realized vol.</param>
/// <param name="lambda">The exponential weight</param>
/// <param name="data">The input data</param>
/// <returns>A prediction and date the prediction is valid for.</returns>
let expRealizedVol (width: int) (lambda: float) (data: array<ReturnObs>) =
    data
    |> Array.sortByDescending (fun x -> x.Date)
    |> Array.windowed (width + 1)
    |> Array.Parallel.map (fun window ->
        let mu = window[1..] |> Array.averageBy (fun x -> x.Return)
        let mutable acc = 0.0
        for t = 1 to width do 
            let w = (1.0 - lambda)*lambda**(float t - 1.0)
            acc <- acc + w * (window[t].Return - mu)**2.0
        { VolatilityPrediction.Date = window[0].Date; PredictedVol = sqrt acc })
    |> Array.rev

let expVarPred =
    ff3Daily
    |> Array.map (fun x -> { Date = x.Date; Return = x.MktRf })
    |> expRealizedVol 500 0.94 
    |> Array.groupBy (fun x -> DateTime(x.Date.Year, x.Date.Month, 1))
    |> Array.map (fun (month, xs) ->
        let last = xs |> Array.sortBy (fun x -> x.Date) |> Array.last
        month.AddMonths(1), 22.0 * last.PredictedVol ** 2.0)
    |> Map

(**
Compare vol accuracies
*)
let checkVols =
    [| for dt in expVarPred.Keys |> Seq.filter (fun x -> x >= DateTime(1945,1,1) ) do
        if expVarPred.ContainsKey (dt.AddMonths(1)) then
           {| Date = dt
              ExpVar = expVarPred[dt]
              Var22d = mmVarPred[dt]
              Actual = mmVarPred[dt.AddMonths(1)]|}|]

(** Exponential vol*)
Ols("Actual~ExpVar", checkVols).fit().summary()
(** Rolling vol*)
Ols("Actual~Var22d", checkVols).fit().summary()

(** Compare ports*)
let managedExpVar = 
    managedMVPortLimited 3.0 muExpandingMap expVarPred
    |> Array.filter (fun x -> x.Month >= DateTime(1945,1,1))

portSummary "Managed, rolling mu and exp. vol" managedExpVar
portSummary "Managed, rolling mu and rolling vol" managedEstAll
portSummary "Buy and hold" buyHold1945

