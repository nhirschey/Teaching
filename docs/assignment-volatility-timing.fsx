(**
---
title: Volatility Timing
category: Assignments
categoryindex: 2
index: 8
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


| Student Name | Student Number  | 
| -----------  | --------------  |
| **1:**       |                 |
| **2:**       |                 |


This is an assignment. You may work in pairs (two students).  You will find sections labeled **Task** asking you to do each piece of analysis. Please make sure that you complete all of these tasks. I included some tests to help you see if you are calculating the solution correctly, but if you cannot get the test to pass submit your best attempt and you may recieve partial credit.

All work that you submit should be your own. Make use of the course resources and example code on the course website. It should be possible to complete all the requested tasks using information given below or somewhere on the course website.

*)

#r "nuget:FSharp.Data"
#r "nuget: FSharp.Stats"
#r "nuget: Plotly.NET, 2.0.0-preview.17"
(*** condition: ipynb ***)
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.17"

(** *)

open System
open FSharp.Data
open Plotly.NET
open FSharp.Stats

(** for testing. *)
#r "nuget: FsUnit.Xunit"
#r "nuget: xunit, 2.*"
open Xunit
open FsUnit.Xunit
open FsUnitTyped

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
## Load Data

First, make sure that you're referencing the correct files.

Here I'm assuming that you have a class folder with this 
notebook and these files in it. The folder hierarchy would 
look like below where you have the below files and folders accessible.

- `Common.fsx` is on the course website.
- `notebook.ipynb` is this notebook.

```code
/class
    Common.fsx
    notebook.ipynb                
```
*)

let [<Literal>] ResolutionFolder = __SOURCE_DIRECTORY__
Environment.CurrentDirectory <- ResolutionFolder

#load "Common.fsx"
open Common

(**
We get the Fama-French 3-Factor asset pricing model data.
*)

let ff3 = 
    French.getFF3 Frequency.Daily
    |> Seq.toList
    |> List.filter (fun x -> x.Date < DateTime(2022,3,1))

let annualizeDailyStdDev dailyStdDev = sqrt(252.0) * dailyStdDev

(**
As an example, I'll first calculate the standard deviation of the `MktRf` factor and assign it to a value named `stdDevMkt`.
*)

let stdDevMkt =
    ff3
    |> Seq.stDevBy (fun x -> x.MktRf)

(** 
The following test will pass if I calculate it correctly.
*)

// Test.
stdDevMkt 
|> should (equalWithin 0.005) 0.01

(** 
The test following test will fail if I calculate it incorrectly. In this failing example I report an annualized standard deviation instead of a daily standard deviation.
*)

(***do-not-eval***)
let stdDevMktFAIL =
    ff3
    |> Seq.stDevBy (fun x -> x.MktRf)
    |> annualizeDailyStdDev

// Test
if false then // make this `if true` to run the test.
    stdDevMktFAIL
    |> should (equalWithin 0.005) 0.01

(**
## Start of the assignment

> **Task:** Calculate the standard deviation of the `Hml` factor's daily returns. Assign it to a value named `stdDevHml`.
*)
(***hide***)
let stdDevHml =
    ff3
    |> Seq.stDevBy (fun x -> x.Hml)

(** Write your solution in the cell below.*)
// Solution here

// Test
stdDevHml 
|> should (equalWithin 0.005) 0.006

(**
> **Task:** Calculate the annualized standard deviation of the `Hml` factor's daily returns. Assign it to a value named `stdDevHmlAnnualized`.
*)
(***hide***)
let stdDevHmlAnnualized =
    stdDevHml |> annualizeDailyStdDev

(** Write your solution in the cell below.*)
// Solution here

// Test
stdDevHmlAnnualized
|> should (equalWithin 0.005) 0.098
(**
> **Task:** Assign the daily returns of the `Hml` factor to a value named `hml` that is a `list` of `ReturnObs`.
*)

type ReturnObs = 
    { 
        Name: string
        Date: DateTime 
        Return: float 
    }

(***hide***)
let hml = 
    ff3 
    |> List.map (fun x -> 
        { Name = "hml"
          Date = x.Date
          Return = x.Hml})

(** Write your solution in the cell below.*)
// Solution here

// Tests
hml[..1] |> should be ofExactType<list<ReturnObs>>

hml[0].Name |> shouldEqual "hml"

hml |> shouldHaveLength 25187

hml
|> List.averageBy (fun x -> x.Return)
|> should (equalWithin 0.0001) 0.00015

(**
> **Task:** Calculate the daily returns of the `Hml` factor with `2x` leverage applied to the portfolio every day. Assign it to a value named `hml2x` that is a `list` of `ReturnObs`.
*)

(***hide***)
let hml2x =
    ff3
    |> List.map (fun x -> 
        { 
            Name = "hml2x"
            Date = x.Date
            Return = x.Hml * 2.0
        })

(** Write your solution in the cell below.*)
// Solution here

// Tests
hml2x[..1] |> should be ofExactType<list<ReturnObs>>

hml2x[0].Name |> shouldEqual "hml2x"

hml2x |> shouldHaveLength 25187

hml2x
|> List.averageBy (fun x -> x.Return)
|> should (equalWithin 0.0001) 0.0003

(**
> **Task:** Calculate the average annualized daily return of the `Hml` factor and assign it to a value name `hmlAvg`.
*)

(***hide***)
let hmlAvg = 
    let daily = 
        hml |> Seq.averageBy (fun x -> x.Return)
    daily * 252.0

(** Write your solution in the cell below.*)
// solution here

// tests
hmlAvg |> should (equalWithin 0.005) 0.04

(**
> **Task:** Calculate the average annualized daily return of the 2x leveraged `Hml` factor and assign it to a value name `hml2xAvg`.
*)

(***hide***)
let hml2xAvg = 
    let daily = 
        hml2x |> Seq.averageBy (fun x -> x.Return)
    daily * 252.0

(** Write your solution in the cell below.*)
// solution here

// tests
hml2xAvg |> should (equalWithin 0.005) 0.075


(**
Here is some code that you will use for the next task.
*)
type GrowthObs =
    { 
        Name: string
        Date: DateTime
        Growth: float
    }

let cumulativeGrowth (xs: list<ReturnObs>) =
    let sorted = xs |> List.sortBy (fun x -> x.Date)
    let calcGrowth (prior: GrowthObs) (current: ReturnObs) =
        { Name = current.Name 
          Date = current.Date
          Growth = prior.Growth * (1.0 + current.Return) }        
    match sorted with
    | [] -> []
    | h::t ->
        let firstOb = 
            { Name = h.Name 
              Date = h.Date
              Growth = 1.0 + h.Return }
        (firstOb, t) ||> List.scan calcGrowth

(**
> **Task:** Calculate the cumulative growth of \$1 invested in HML at the start of the sample. Assign it to a value named `hmlGrowth` that is a `list` of `GrowthObs`.
*)


(***hide***)
let hmlGrowth = 
    hml |> cumulativeGrowth

(** Write your solution in the cell below.*)
// Solution here

// Tests
hmlGrowth[..1] |> should be ofExactType<list<GrowthObs>>

hmlGrowth
|> List.map (fun x -> x.Growth)
|> List.last
|> should (equalWithin 1.0) 27.0

(**
> **Task:** Calculate the cumulative growth of \$1 invested in 2x levered HML at the start of the sample. Assign it to a value named `hml2xGrowth` that is a `list` of `GrowthObs`.
*)

(***hide***)
let hml2xGrowth = 
    hml2x |> cumulativeGrowth

(** Write your solution in the cell below.*)
// Solution here

// Tests
hml2xGrowth[..1] |> should be ofExactType<list<GrowthObs>>

hml2xGrowth
|> List.map (fun x -> x.Growth)
|> List.last
|> should (equalWithin 1.0) 286.0

(**
Here is an example of a plot of the cumulative growth of \$1 invested in the market.
*)

let mkt = 
    [ for x in ff3 do
        { Name = "market"
          Date = x.Date
          Return = x.MktRf } ]

let marketGrowthChart = 
    mkt
    |> cumulativeGrowth
    |> List.map (fun x -> x.Date, x.Growth)
    |> Chart.Line
    |> Chart.withYAxisStyle (AxisType = StyleParam.AxisType.Log)

(***condition:ipynb***)
#if IPYNB
marketGrowthChart
#endif // IPYNB

(***hide***)
marketGrowthChart |> GenericChart.toChartHTML
(***include-it-raw***)


(**
> **Task:** Plot the cumulative growth of \$1 invested in HML and \$1 invested in the Market as a combined line chart using a log scale for the y-axis.
*)

(***hide,do-not-eval***)
let mktHmlChart = 
    [ Chart.Line (mkt 
                  |> cumulativeGrowth
                  |> List.map (fun x -> x.Date, x.Growth),
                  Name = "market")
      Chart.Line (hml 
                  |> cumulativeGrowth
                  |> List.map (fun x -> x.Date, x.Growth),
                  Name = "hml")]
    |> Chart.combine

mktHmlChart |> Chart.show

(** Write your solution in the cell below.*)
// Solution here

(**
> **Task:** Apply a constant levarage to the HML and market factors for the full sample such that the daily returns have a full-sample annualized standard deviation equal to 10%. Remember, *constant leverage* means that leverage is exactly the same every day. Assign the results to values named `hml10` and `mkt10` that are lists of `ReturnObs`.
*)

(***hide***)
let dailyEquivalentOf10pctAnnual = 0.1 / sqrt(252.0)
let hml10 = 
    [ for day in hml do 
        { day with Return = day.Return * (dailyEquivalentOf10pctAnnual / stdDevHml) } ]

let mkt10 =
    [ for day in mkt do 
        { day with Return = day.Return * (dailyEquivalentOf10pctAnnual / stdDevMkt)}]

(** Write your solution in the cell below.*)
// Solution here


// Tests
hml10[..1] |> should be ofExactType<list<ReturnObs>>
mkt10[..1] |> should be ofExactType<list<ReturnObs>>

hml10 
|> stDevBy (fun x -> x.Return) 
|> annualizeDailyStdDev
|> should (equalWithin 1e-6) 0.1

mkt10
|> stDevBy (fun x -> x.Return) 
|> annualizeDailyStdDev
|> should (equalWithin 1e-6) 0.1

(**
> **Task:** Plot the cumulative growth of \$1 invested in `hml10` and `mkt10` as a combined line chart using a log scale for the y-axis.
*)

(***hide,do-not-eval***)
let mktHmlChart10 = 
    [ Chart.Line (mkt10 
                  |> cumulativeGrowth
                  |> List.map (fun x -> x.Date, x.Growth),
                  Name = "market")
      Chart.Line (hml10 
                  |> cumulativeGrowth
                  |> List.map (fun x -> x.Date, x.Growth),
                  Name = "hml")]
    |> Chart.combine

mktHmlChart10 |> Chart.show

(** Write your solution in the cell below.*)
// Solution here

(**
> **Task:** Explain how to intepret the dramatic difference between the two plots that you have just created. Why is the plot of \$1 invested in unlevered HML and Market factors so different from the plot of \$1 invested in the versions that are levered to have a 10% annualized standard deviation?
*)

(** 
Write your answer here.
*)
