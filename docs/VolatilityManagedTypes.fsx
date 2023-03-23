(**

---
title: Volatility Managed, but with different vols
category: Lectures
categoryindex: 1
index: 6
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


# Objectives
Last time we looked at volatility managed portfolios, but we hard coded the volatility predictions. This time, we're going to add some flexibility to use different types of volatility predictions.

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
## Realized volatility

First, our volatility measure from last time. When we take a sample volatility from the past, this is known as realized volatility. The realized volatility measure is recognized as a high quality estimate of future volatility (see [Anderson, Bollerslev, Diebold, Labys (2003)](https://scholar.google.com/citations?view_op=view_citation&hl=en&user=MpRgtycAAAAJ&citation_for_view=MpRgtycAAAAJ:O3NaXMp0MMsC) and [Anderson, Bollerslev, Diebold, Ebens (2001)](https://scholar.google.com/citations?view_op=view_citation&hl=en&user=MpRgtycAAAAJ&citation_for_view=MpRgtycAAAAJ:YFjsv_pBGBYC)).
*)

type ReturnObs = { Date: DateTime; Return: float}

type VolatilityPrediction = 
    { /// First date the prediction is valid for
      Date: DateTime
      /// The volatility prediction
      Vol: float}

/// <summary>Calculates realized volatity of a return observation</summary>
/// <param name="width">The window width for calculating realized vol.</param>
/// <param name="data">The input data</param>
/// <returns>A prediction and date the prediction is valid for.</returns>
let realizedVol (width: int) (data: list<ReturnObs>) =
    data
    |> List.sortBy (fun x -> x.Date)
    |> List.windowed (width + 1)
    |> List.map (fun window ->
        let sd =
            window 
            |> List.take width 
            |> List.map (fun x -> x.Return) 
            |> stDev
        let last = window |> List.last
        { VolatilityPrediction.Date = last.Date; Vol = sd })

(**
> **Practice:** Calculate the realized volatility of the Fama and French data using a window width of 252 days. Plot the realized volatility over time.
*)

// Answer here

(**
## Exponentially weighted moving realized volatility

Another common measure is an exponetially weighted moving average of volatility. This is a simple moving average, but the weights are exponentially decaying. To understand, it's helpful to compare to the simple realized volatility measure which is an equal weight average of past vols.

The equal weight realized volatility is calculated as follows:

$$
\sigma_t = \sqrt{\sum_{i=1}^T \frac{1}{T} r_{t-i}^2}
$$

where $r_t$ is the return at time $t$ and $T$ is the window width. You can think of $1/T$ as the weight of each observation.

The exponentially weighted moving average is calculated as follows:

$$
\sigma_t = \sqrt{\sum_{i=1}^T \frac{\lambda^{t-1}}{\sum_{i=1}^T \lambda^{t-1}} r_{t-i}^2}
$$

where $\lambda$ is a parameter that you can specify or estimate. It is common to use the RiskMetrics estimate of $\lambda=0.94$. With that $\lambda$ the most recent observation has a weight of 0.94, the next most recent observation has a weight of 0.94 * 0.94, and so on. Note that if you set $\lambda=1$, then you get the equal weight realized volatility.

To simplify this term, as $T$ gets large, we can approximate the sum as:

$$
\sigma_t = \sqrt{\sum_{i=1}^T (1-\lambda)\lambda^{t-1} r_{t-i}^2}
$$

What do the weights look like?
*)

[ for i = 1.0 to 500.0 do i, (1.0 - 0.94)*0.94**(i-1.0)]
|> Chart.Line

(**
A function to calculate the exponentially weighted moving average of realized volatility.
*)

/// <summary>Calculates exponentially weighted return obs</summary>
/// <param name="width">The window width for calculating realized vol.</param>
/// <param name="lambda">The lambda parameter for the exponentially weighted moving average</param>
/// <param name="data">The input data</param>
/// <returns>A prediction and date the prediction is valid for.</returns>
let expRealizedVol (width: int) (lambda: float) (data: list<ReturnObs>) =
    data
    |> List.sortBy (fun x -> x.Date)
    |> List.windowed (width + 1)
    |> List.map (fun window ->
        // List.rev makes most recent be first, oldest last
        let train = window |> List.take width |> List.rev
        let mu = train |> List.map (fun x -> x.Return) |> List.average
        let sd =
            [ for i = 0 to train.Length-1 do
                let w = (1.0 - lambda)*lambda**(float i)
                w * (train[i].Return - mu)**2.0 ]
            |> List.sum
            |> sqrt
        let last = window |> List.last
        { VolatilityPrediction.Date = last.Date; Vol = sd })

(**
> **Practice:** Calculate the exponentially weighted realized volatility of the Fama and French data using a window width of 252 days. Plot the realized volatility over time.
*)

// Answer here


(**
## Accuracy comparison

Let's compare accuracy.
*)

let retData = ff3 |> List.map (fun x -> { Date = x.Date; Return = x.MktRf })
let rv22 = realizedVol 22 retData

let exp500 = expRealizedVol 500 0.94 retData

(**
There are fewer observations in the exponentially weighted realized volatility measure, so we'll need to trim the realized volatility measure.
*)
rv22 |> List.minBy (fun x -> x.Date)
exp500 |> List.minBy (fun x -> x.Date)

(** now filter *)

let minDate = exp500 |> List.map (fun x -> x.Date) |> List.min
let rv22Filtered= rv22 |> List.filter (fun x -> x.Date >= minDate)

(** check accuracy *)

let rmse (pred: list<VolatilityPrediction>) =
    let actual = ff3 |> List.map (fun x -> x.Date, x.MktRf) |> dict
    let actualVsPred = 
        [ for p in pred do 
            abs actual[p.Date], p.Vol]
    let rmse =
        [ for (actual, pred) in actualVsPred do
            (actual - pred)**2.0 ]
        |> List.average
        |> sqrt
    rmse


$"Realized vol rmse is {rmse rv22Filtered}"
$"Exp weighted Realized vol rmse is {rmse exp500}"

(**
> **Practice:** Plot a bin scatterplot with two lines, one for realized volatility and one for the exponentially weighted realized volatility. What do you notice?
*)

(**
> **Practice:** Calculate the RMSE of the realized volatility and the exponentially weighted realized volatility for different window widths. What is the best window width?
*)

