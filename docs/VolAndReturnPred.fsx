(**

---
title: Volatility and Return predictions
category: Lectures
categoryindex: 1
index: 7
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


# Objectives


## Loading Fama and French data
*)


#r "nuget: FSharp.Data, 5.0.2"
#r "nuget: NovaSBE.Finance, 0.4.0"
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"

open System
open FSharp.Data
open FSharp.Stats
open Plotly.NET
open NovaSBE.Finance.French

let ff3 = 
    getFF3 Frequency.Daily 
    |> Array.toList


(**
## Faster code
Let's revisit our exponentially weighted volatility predicton from before.

This is the original, working with lists, with some minor changes for clarity.
*)

type ReturnObs = { Date: DateTime; Return: float}

type VolatilityPrediction = 
    { /// First date the prediction is valid for
      Date: DateTime
      /// The volatility prediction
      PredictedVol: float}

let expRealizedVolList (width: int) (lambda: float) (data: list<ReturnObs>) =
    data
    // Descending gets us ordered t, t-1, t-2, ...
    |> List.sortByDescending (fun x -> x.Date)
    |> List.windowed (width + 1)
    |> List.map (fun window ->
        let dayToPredict = window[0]
        let trainingDays = window[1..]
        let mu = 
            trainingDays 
            |> List.map (fun x -> x.Return) 
            |> List.average
        let sd =
            [ for t = 1 to width do
                let w = (1.0 - lambda)*lambda**(float t - 1.0)
                w * (trainingDays[t-1].Return - mu)**2.0 ]
            |> List.sum
            |> sqrt

        { VolatilityPrediction.Date = dayToPredict.Date; PredictedVol = sd })
    |> List.rev

(**
let's time it.
*)

let rets = ff3 |> List.map (fun x -> { Date = x.Date; Return = x.MktRf})

#if FSX
#time "on"
#endif // FSX

let vList = expRealizedVolList 500 0.94 rets

(**
That's kinda slow.

Part of the reason is that it's iterating through lists, 
and iterating through large
lists can be slow.

Try it with array.
*)


let xvArray (width: int) (lambda: float) (data: array<ReturnObs>) =
    data
    // Descending gets us ordered t, t-1, t-2, ...
    |> Array.sortByDescending (fun x -> x.Date)
    |> Array.windowed (width + 1)
    |> Array.map (fun window ->
        let dayToPredict = window[0]
        let trainingDays = window[1..]
        let mu = 
            trainingDays 
            |> Array.map (fun x -> x.Return) 
            |> Array.average
        let sd =
            [| for t = 1 to width do
                let w = (1.0 - lambda)*lambda**(float t - 1.0)
                w * (trainingDays[t-1].Return - mu)**2.0 |]
            |> Array.sum
            |> sqrt

        { VolatilityPrediction.Date = dayToPredict.Date; PredictedVol = sd })
    |> Array.rev

(**
Data to test it.
*)
let retsArray = rets |> List.toArray

(**
Test it.
*)

let vArray = xvArray 500 0.94 retsArray

(**
That's somewhat faster.

We can be even faster by reducing allocations.
*)

let xvArrayFewerAlloc (width: int) (lambda: float) (data: array<ReturnObs>) =
    data
    |> Array.sortByDescending (fun x -> x.Date)
    |> Array.windowed (width + 1)
    |> Array.map (fun window ->
        let mu = window[1..] |> Array.averageBy (fun x -> x.Return)
        let mutable acc = 0.0
        for t = 1 to width do 
            let w = (1.0 - lambda)*lambda**(float t - 1.0)
            acc <- acc + w * (window[t].Return - mu)**2.0
        { VolatilityPrediction.Date = window[0].Date; PredictedVol = sqrt acc })
    |> Array.rev

(**
Test it.
*)

let vFewerAlloc = xvArrayFewerAlloc 500 0.94 retsArray 

(** Even fewer allocations. *)
let xvArrayFewerAlloc2 (width: int) (lambda: float) (data: array<ReturnObs>) =
    let data = data |> Array.sortByDescending (fun x -> x.Date)
    data[..data.Length-1-width]
    |> Array.mapi (fun i x ->
        let mu = data[i+1..i+width] |> Array.averageBy (fun x -> x.Return)
        let mutable acc = 0.0
        for t = 1 to width do 
            let w = (1.0 - lambda)*lambda**(float t - 1.0)
            acc <- acc + w * (data[i+t].Return - mu)**2.0
        { VolatilityPrediction.Date = x.Date; PredictedVol = sqrt acc })
    |> Array.rev

let vFewerAlloc2 = xvArrayFewerAlloc2 500 0.94 retsArray 


(** Now make the fewer alloc version parallel.*)

let xvFewerAllocParallel (width: int) (lambda: float) (data: array<ReturnObs>) =
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

(**
Test it.
*)

let vFewerParallel = xvFewerAllocParallel 500 0.94 retsArray

(** Now make the second fewer alloc version parallel.*)

let xvFewerAlloc2Parallel (width: int) (lambda: float) (data: array<ReturnObs>) =
    let data = data |> Array.sortByDescending (fun x -> x.Date)
    data[..data.Length-1-width]
    |> Array.Parallel.mapi (fun i x ->
        let mu = data[i+1..i+width] |> Array.averageBy (fun x -> x.Return)
        let mutable acc = 0.0
        for t = 1 to width do 
            let w = (1.0 - lambda)*lambda**(float t - 1.0)
            acc <- acc + w * (data[i+t].Return - mu)**2.0
        { VolatilityPrediction.Date = x.Date; PredictedVol = sqrt acc })
    |> Array.rev

(** Test it. *)
let vFewer2Parallel = xvFewerAlloc2Parallel 500 0.94 retsArray


(** Compare results.*)
(vList |> List.toArray) = vFewerAlloc2

[ vArray; vFewerAlloc; vFewerAlloc2; vFewerParallel; vFewer2Parallel]
|> List.forall (fun x -> x = (vList |> List.toArray ))

(**
## Return predictions
*)

type ReturnPrediction = { Date: DateTime; PredictedReturn: float }

let avgReturnAccumulatorList (xs: list<ReturnObs>) =
    let mutable acc = 0.0
    [ for i=0 to (xs.Length-2) do
        acc <- acc + xs[i].Return
        let avgReturn = acc / float (i + 1)
        { Date = xs[i+1].Date; PredictedReturn = avgReturn }]

(**
Test it.
*)

let testData =
    [{ Date = DateTime(1999,1,1); Return = 1.0 }
     { Date = DateTime(1999,1,2); Return = 2.0 }
     { Date = DateTime(1999,1,3); Return = -3.0 } 
     { Date = DateTime(1999,1,4); Return = 0.0 }]

avgReturnAccumulatorList testData

(** For the whole dataset*)

let retPredictions = avgReturnAccumulatorList rets

(**
## Combining predictions
*)

(**
Now form portfolios based on these predictions.
*)

type VolAndReturnPrediction = { Date: DateTime; PredictedVol: float; PredictedReturn: float }

let combinePredictions (predReturns: seq<ReturnPrediction>) (predVols: seq<VolatilityPrediction>) =
    let predVols = 
        predVols 
        |> Seq.map (fun x -> x.Date, x.PredictedVol) 
        |> Map
    [ for retObs in predReturns do 
        if predVols.ContainsKey retObs.Date then
            { Date = retObs.Date 
              PredictedVol = predVols[retObs.Date]
              PredictedReturn = retObs.PredictedReturn } ]

(** Let's see how it works *)

combinePredictions retPredictions[1000..1005] vFewer2Parallel

(** Now let's have a function that creates returns off of that.*)

let managedPortfolio gamma predVols predReturns (xs: list<ReturnObs>) =
    let preds = 
        combinePredictions predReturns predVols
        |> List.map (fun x -> x.Date, x)
        |> Map
    [ for x in xs do 
        if preds.ContainsKey x.Date then
            let pred = preds[x.Date]
            let w = pred.PredictedReturn / (gamma* pred.PredictedVol ** 2.0)
            { Date = x.Date
              Return = w * x.Return } ]

(** Let's see how it works *)

managedPortfolio 3.0 vFewer2Parallel retPredictions[1000..1005] rets

(** Doing it for the full sample.*)

let result = 
    managedPortfolio 3.0 vFewer2Parallel retPredictions rets

(** Now calculate mean-variance utility of the portfolio.*)
let avgReturn = result |> List.averageBy (fun x -> x.Return)
let varResult = result |> varBy (fun x -> x.Return)

(avgReturn - (3.0/2.0) * varResult)*252.0

(** Compare to just buy and hold*)

let managedMinDate = result |> List.map (fun x -> x.Date) |> List.min
let buyHoldPeriod = rets |> List.filter (fun x -> x.Date >= managedMinDate)
let avgBuyHold = buyHoldPeriod |> List.averageBy (fun x -> x.Return)
let varBuyHold = buyHoldPeriod |> varBy (fun x -> x.Return)

(avgBuyHold - (3.0/2.0) * varBuyHold)*252.0

(** Now our managed portfolio. *)

let avgManagedReturn = result |> List.averageBy (fun x -> x.Return)
let varManagedResult = result |> varBy (fun x -> x.Return)

(avgManagedReturn - (3.0/2.0) * varManagedResult)*252.0

(** Why the difference? *)

(** Try scaling managed to full sample variance. *)
let c = sqrt varBuyHold / sqrt varManagedResult
(c * avgManagedReturn - (3.0/2.0) * c ** 2.0 * varManagedResult)*252.0

(** Another way of seeing it.*)

let mvu gamma mu sigma =
    mu - 0.5 * gamma * sigma ** 2.0

mvu 3.0 (avgBuyHold * 252.0) (sqrt (varBuyHold * 252.0))

let w_star = avgBuyHold / (3.0 * varBuyHold)

mvu 3.0 (w_star * avgBuyHold * 252.0) (sqrt (w_star ** 2.0 * varBuyHold * 252.0))