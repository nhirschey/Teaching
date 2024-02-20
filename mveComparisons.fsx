(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=mveComparisons.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//mveComparisons.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//mveComparisons.ipynb)

# Objectives

## Loading Fama and French data

*)
#r "nuget: FSharp.Data, 5.0.2"
#r "nuget: NovaSBE.Finance, 0.5.0"
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"

open System
open FSharp.Data
open FSharp.Stats
open Plotly.NET
open NovaSBE.Finance.French

let ff3 = getFF3 Frequency.Daily 
(**
Some volatility functions.

First the input/output data types.

*)
type ReturnObs = { Date: DateTime; Return: float}

type VolatilityPrediction = 
    { /// First date the prediction is valid for
      Date: DateTime
      /// The volatility prediction
      PredictedVol: float }
(**
Some test data with increasing dates and random returns.

*)
let seed = 99
Random.SetSampleGenerator(Random.RandThreadSafe(seed))

// Let's start with this sample of returns
let rnorm =  Distributions.Continuous.Normal.Init 0.01 1.0
let testData = 
    [| for i=1 to 100 do 
        { Date = DateTime(2010, 1, 1).AddDays(float i); 
          Return = rnorm.Sample() }
    |]
(**
The first example we'll use is an expanding window of volatility.

*)
/// <summary>Calculates sample average expanding window volatility.</summary>
let expandingWindowVol (data: array<ReturnObs>) =
    [| for i=2 to data.Length-1 do 
        let sd = data[..i-1] |> Array.map (fun x -> x.Return) |> stDev
        { VolatilityPrediction.Date = data[i].Date; PredictedVol = sd }
    |]
(**
Let's try it with our test data.

*)
let expandingWindowVolsTest = expandingWindowVol testData
(**
First few observations.

*)
expandingWindowVolsTest[..3]
(**
Last few observations.

*)
expandingWindowVolsTest[expandingWindowVolsTest.Length-4..]
(**
Rolling window volatility.

*)
/// <summary>Calculates realized volatity of a return observation 
/// over a rolling window of past observations.</summary>
/// <param name="width">The window width for calculating realized vol.</param>
/// <param name="data">The input data</param>
/// <returns>A prediction and date the prediction is valid for.</returns>
let rollingVol (width: int) (data: array<ReturnObs>) =
    data
    |> Array.sortBy (fun x -> x.Date)
    |> Array.windowed (width + 1)
    |> Array.map (fun window ->
        let sd =
            window 
            |> Array.take width 
            |> Array.map (fun x -> x.Return) 
            |> stDev
        let last = window |> Array.last
        { VolatilityPrediction.Date = last.Date; PredictedVol = sd })
(**
Calculated using our test data

*)
let rollingVolTest = rollingVol 22 testData
(**
First few observations.

*)
rollingVolTest[..3]
(**
Last few observations.

*)
rollingVolTest[rollingVolTest.Length-4..]
(**
Exponential weight volatility.

*)
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
(**
Calculated using our test data.

*)
let expRealizedVolTest = expRealizedVol 50 0.94 testData
(**
First few observations.

*)
expRealizedVolTest[..3]
(**
Last few observations.

*)
expRealizedVolTest[expRealizedVolTest.Length-4..]
(**
Now a plot of all of them together.

*)
let combinedTestData = 
    [ expandingWindowVolsTest, "expanding window"
      rollingVolTest, "realized"
      expRealizedVolTest, "exp realized" ]

[ for (vols, name) in combinedTestData do
    let dataForChart = vols |> Array.map (fun x -> x.Date, x.PredictedVol)
    Chart.Line(dataForChart,Name=name) ]
|> Chart.combine
(**
## Return predictions

*)
type ReturnPrediction = { Date: DateTime; PredictedReturn: float }

let avgReturnAccumulator (xs: array<ReturnObs>) =
    let mutable acc = 0.0
    [| for i=0 to (xs.Length-2) do
        acc <- acc + xs[i].Return
        let avgReturn = acc / float (i + 1)
        { Date = xs[i+1].Date; PredictedReturn = avgReturn } |]
(**
Test it.

*)
testData
|> avgReturnAccumulator
|> Array.map (fun x -> x.Date, x.PredictedReturn)
|> Chart.Line
(**
## Combining predictions

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
(**
Now let's have a function that creates returns off of that.

*)
let managedPortfolio gamma predVols predReturns (xs: seq<ReturnObs>) =
    let preds = 
        combinePredictions predReturns predVols
        |> Seq.map (fun x -> x.Date, x)
        |> Map
    [| for x in xs do 
        if preds.ContainsKey x.Date then
            let pred = preds[x.Date]
            let w = pred.PredictedReturn / (gamma* pred.PredictedVol ** 2.0)
            { Date = x.Date
              Return = w * x.Return } |]
(**
Let's compare results for our 3 volatility predictions.

First, expected returns.

*)
let retObs = ff3 |> Array.map (fun x -> { Date = x.Date; Return = x.MktRf })

let expectedRet = 
    retObs
    |> avgReturnAccumulator
(**
Plot it.

*)
expectedRet
|> Array.map (fun x -> x.Date, x.PredictedReturn)
|> Chart.Line
(**
Optimal portfolios using our expected returns and expected vols.

*)
let expandingVols = expandingWindowVol retObs
let rollingVols = rollingVol 22 retObs
let exponentialVols = expRealizedVol 500 0.94 retObs
(**
The portfolios.

*)
let expandingVolPort  = managedPortfolio 3.0 expandingVols expectedRet retObs
let rollingVolPort = managedPortfolio 3.0 rollingVols expectedRet retObs
let exponentialVolPort = managedPortfolio 3.0 exponentialVols expectedRet retObs
(**
Let's plot these.

*)
let retAsTuple (xs: array<ReturnObs>) = xs |> Array.map (fun x -> x.Date, x.Return)

let cumulativeReturns (xs: array<DateTime * float>) =
    let xs = xs |> Array.sortBy (fun (dt, r) -> dt)
    let mutable accRet = 0.0
    [| for (dt, r) in xs do
        let cr = (1.0 + accRet) * (1.0 + r) - 1.0
        let broke = -1.0 + epsilon
        accRet <- max broke cr
        (dt, accRet) |]

open Plotly.NET.LayoutObjects

let plotCumulativeReturns name (xs: array<DateTime * float>) =
    let xsFromOne = 
        xs
        |> Array.map (fun (dt, r) -> dt, 1.0 + r)
    Chart.Line(xsFromOne, Name=name)
    |> Chart.withYAxisStyle(AxisType = StyleParam.AxisType.Log)

[ expandingVolPort, "expandingVol"
  rollingVolPort, "rollingVol"
  exponentialVolPort, "exponentialVol" ]
|> List.map (fun (port, name) ->
    port
    |> retAsTuple
    |> cumulativeReturns
    |> plotCumulativeReturns name)
|> Chart.combine
|> Chart.show

expandingVolPort
|> retAsTuple
|> cumulativeReturns
|> plotCumulativeReturns "expandingVol"
|> Chart.show

