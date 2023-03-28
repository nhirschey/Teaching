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


#r "nuget: FSharp.Data"
#r "nuget: NovaSBE.Finance"
#r "nuget: FSharp.Stats"
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
      Vol: float}

let expRealizedVolList (width: int) (lambda: float) (data: list<ReturnObs>) =
    data
    // Descending gets us ordered t, t-1, t-2, ...
    |> List.sortByDescending (fun x -> x.Date)
    |> List.windowed (width + 1)
    |> List.map (fun window ->
        let train = window[1..]
        let mu = train |> List.map (fun x -> x.Return) |> List.average
        let sd =
            [ for t = 1 to width do
                let w = (1.0 - lambda)*lambda**(float t - 1.0)
                w * (train[t-1].Return - mu)**2.0 ]
            |> List.sum
            |> sqrt
        let predictionDay = window[0]
        { VolatilityPrediction.Date = predictionDay.Date; Vol = sd })
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

Part of the reason is that it's iterating through lists, and iterating through large
lists can be slow.

Try it with array.
*)


let expRealizedVolArray (width: int) (lambda: float) (data: array<ReturnObs>) =
    data
    // Descending gets us ordered t, t-1, t-2, ...
    |> Array.sortByDescending (fun x -> x.Date)
    |> Array.windowed (width + 1)
    |> Array.map (fun window ->
        let train = window[1..]
        let mu = train |> Array.map (fun x -> x.Return) |> Array.average
        let sd =
            [ for t = 1 to width do
                let w = (1.0 - lambda)*lambda**(float t - 1.0)
                w * (train[t-1].Return - mu)**2.0 ]
            |> List.sum
            |> sqrt
        let predictionDay = window[0]
        { VolatilityPrediction.Date = predictionDay.Date; Vol = sd })
    |> Array.rev

(**
Data to test it.
*)
let retsArray = rets |> List.toArray

(**
Test it.
*)

let vArray = expRealizedVolArray 500 0.94 retsArray

(**
That's somewhat faster.

We can be even faster by reducing allocations.
*)

let expRealizedVolArrayFewerAlloc (width: int) (lambda: float) (data: array<ReturnObs>) =
    data
    |> Array.sortByDescending (fun x -> x.Date)
    |> Array.windowed (width + 1)
    |> Array.map (fun window ->
        let mu = window[1..] |> Array.averageBy (fun x -> x.Return)
        let mutable acc = 0.0
        for t = 1 to width do 
            let w = (1.0 - lambda)*lambda**(float t - 1.0)
            acc <- acc + w * (window[t].Return - mu)**2.0
        { VolatilityPrediction.Date = window[0].Date; Vol = sqrt acc })
    |> Array.rev

(**
Test it.
*)

let vFewerAlloc = expRealizedVolArrayFewerAlloc 500 0.94 retsArray

(** Now make the fewer alloc version parallel.*)

let expRealizedVolParallel (width: int) (lambda: float) (data: array<ReturnObs>) =
    data
    |> Array.sortByDescending (fun x -> x.Date)
    |> Array.windowed (width + 1)
    |> Array.Parallel.map (fun window ->
        let mu = window[1..] |> Array.averageBy (fun x -> x.Return)
        let mutable acc = 0.0
        for t = 1 to width do 
            let w = (1.0 - lambda)*lambda**(float t - 1.0)
            acc <- acc + w * (window[t].Return - mu)**2.0
        { VolatilityPrediction.Date = window[0].Date; Vol = sqrt acc })
    |> Array.rev

(**
Test it.
*)

let vParallel = expRealizedVolParallel 500 0.94 retsArray

(** Compare results.*)
(vList |> List.toArray) = vParallel

(**
## Return predictions
*)

let avgReturnAccumulatorList (xs: list<ReturnObs>) =
    let mutable acc = 0.0
    [ for i=0 to (xs.Length-2) do
        acc <- acc + xs[i].Return
        { Date = xs[i+1].Date; Return = acc / float (i + 1)}]

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

let commonDates =
    let vDates = vParallel |> Array.map (fun x -> x.Date) |> set
    let rDates = retPredictions |> List.map (fun x -> x.Date) |> set
    Set.intersect vDates rDates


let filteredVols = 
    vParallel
    |> Array.filter (fun x -> commonDates.Contains x.Date)
    |> Array.map (fun x -> x.Date, x.Vol)
    |> dict
let filteredRets =
    retPredictions
    |> List.filter (fun x -> commonDates.Contains x.Date)
    |> List.map (fun x -> x.Date, x.Return)
    |> dict


let rmse (actualVsPredicted: list< float * float>) =
    [ for (actual, pred) in actualVsPredicted do
        (actual - pred)**2.0 ]
    |> List.average
    |> sqrt
