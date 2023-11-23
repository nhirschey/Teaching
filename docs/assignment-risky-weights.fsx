(**
---
title: Risky Weights
category: Assignments
categoryindex: 2
index: 3
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


Group Name: 

| Student Name | Student Number  | 
| -----------  | --------------  |
| **1:**       |                 |
| **2:**       |                 | 
| **3:**       |                 | 
| **4:**       |                 | 
| **5:**       |                 |

This is an assignment. You may work in groups. Please write your group and group member names above. You will find sections labeled **Task** asking you to do each piece of analysis. Please make sure that you complete all of these tasks. I included some tests to help you see if you are calculating the solution correctly, but if you cannot get the test to pass submit your best attempt and you may recieve partial credit.

All work that you submit should be your own. Make use of the course resources and example code on the course website. It should be possible to complete all the requested tasks using information given below or somewhere on the course website.

*)

#r "nuget: FSharp.Data, 5.0.2"
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: Quotes.YahooFinance"
#r "nuget: Plotly.NET, 3.*"
(*** condition: ipynb ***)
#r "nuget: Plotly.NET.Interactive, 3.*"

(** *)

open System
open FSharp.Data
open Plotly.NET
open FSharp.Stats
open Quotes.YahooFinance


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

(**
## Load Data

*)


(**
We get the SPY ETF
*)

type MonthlyReturn = { Date: DateTime; Return: float }
let spy = 
    YahooFinance.History("SPY", 
                         startDate = DateTime(2010,1,1), 
                         endDate = DateTime(2023, 2, 28),
                         interval = Monthly)
    |> List.sortBy (fun month -> month.Date)
    |> List.pairwise
    |> List.map (fun (m0, m1) -> 
        { Date = m1.Date
          Return = (m1.AdjustedClose - m0.AdjustedClose) / m0.AdjustedClose })

(**
Load risk-free rate
*)

// 4-week Treasury Bill: Secondary Market Rate
type DTB4WK = CsvProvider<"https://fred.stlouisfed.org/graph/fredgraph.csv?id=DTB4WK",
                           Schema="Date,RiskFreeRate (float)",
                           MissingValues=".">


// We'll take the 4-week interest rate at the start of the month as the risk-free rate for that month.
// Then we'll put it in a dictionary for efficient lookup.
let rf =
    DTB4WK.GetSample().Rows
    |> Seq.toList
    |> List.filter (fun x -> not (Double.IsNaN x.RiskFreeRate))
    |> List.groupBy (fun day -> day.DATE.Year, day.DATE.Month)
    |> List.map (fun (month, daysInMonth) ->
        let firstDay = 
            daysInMonth 
            |> List.sortBy (fun day -> day.DATE)
            |> List.head
        let date = DateTime(firstDay.DATE.Year, firstDay.DATE.Month, 1)
        // discount basis assumes 30 days in a month, 360 days per year.
        let ret = (30.0 / 360.0) * firstDay.RiskFreeRate / 100.0 
        date, ret)
    |> dict

(** Look at an example, the index is date*)
rf[DateTime(2010,1,1)] 

(** Calculating excess returns of spy returns:*)

let excessSpy =
    [ for x in spy do 
        { Date = x.Date
          Return = x.Return - rf.[x.Date] } ]


(**
As an example, I'll first calculate the standard deviation of the the excess returns of SPY and assign it to a value named `stdDevExcessSpy`.
*)

let stdDevExcessSpy =
    excessSpy
    |> stDevBy (fun x -> x.Return)

(** 
The following test will pass if I calculate it correctly.
*)

// Test.
stdDevExcessSpy
|> should (equalWithin 0.005) 0.04

(** 
The test following test will fail if I calculate it incorrectly. In this failing example I report an annualized standard deviation instead of a monthly standard deviation.
*)

(***do-not-eval***)
let stdDevFAIL =
    let monthlyStDev = 
        excessSpy
        |> stDevBy (fun x -> x.Return)
    monthlyStDev * (sqrt 12.0)

// Test
if false then // make this `if true` to run the test.
    stdDevFAIL
    |> should (equalWithin 0.005) 0.04

(**
## Start of the assignment

> **Task:** What is the cumulative return of SPY from the beginning to the end of the sample period? In other words, if you invest
\$1 in SPY at the beginning of the sample, how many *additional* dollars would you have by the end? Assign it to a value named `cumulativeSpyReturn`.
*)

(***hide***)
let cumulativeReturn =
    let mutable cr = 0.0
    [ for x in spy do 
        cr <- (1.0 + cr) * (1.0 + x.Return) - 1.0
        cr ]
    |> List.last

(** Write your solution in the cell below.*)
// Solution here

// Test
cumulativeReturn 
|> should (equalWithin 0.1) 3.75

(**
> **Task:** What is the cumulative *excess* return of SPY from the beginning to the end of the sample period? Assign it to a value named `cumulativeExcessSpyReturn`.
*)

(***hide***)
let cumulativeExcessReturn =
    let mutable cr = 0.0
    [ for x in excessSpy do 
        cr <- (1.0 + cr) * (1.0 + x.Return) - 1.0
        cr ]
    |> List.last

(** Write your solution in the cell below.*)
// Solution here

// Test
cumulativeExcessReturn 
|> should (equalWithin 0.1) 3.3

(**
> **Task:** Plot the cumulative *excess* return of SPY from the beginning to the end of the sample period. The date should be the x-axis and the cumulative excess return should be the y-axis.
*)

(***hide***)
let cumulativeExcessReturnPlot =
    let mutable cr = 1.0
    [ for x in excessSpy do 
        cr <- cr * (1.0 + x.Return)
        x.Date, cr ]
    |> List.map (fun (date, cr) -> date, cr - 1.0)
    |> Chart.Line
    |> Chart.withTitle("Example Plot")
    |> Chart.withXAxisStyle(TitleText="Date")
    |> Chart.withYAxisStyle(TitleText="Cumulative Excess Return")        

(***hide***)
cumulativeExcessReturnPlot |> GenericChart.toChartHTML
(***include-it-raw***)


(** Write your solution in the cell below.*)
// Solution here

(**
> **Task:** Calculate the standard deviation of the `excessSPY` monthly returns from 2010-01 to 2019-12 (inclusive). Assign it to a value named `stdDev2010s`.
*)
(***hide***)
let stdDev2010s =
    excessSpy
    |> List.filter (fun x -> 
        x.Date >= DateTime(2010,1,1) && 
        x.Date <= DateTime(2019,12,31))
    |> stDevBy (fun x -> x.Return)

(** Write your solution in the cell below.*)
// Solution here

// Test
stdDev2010s 
|> should (equalWithin 0.005) 0.035

(**
> **Task:** Calculate the average monthly excess returns of SPY from 2010-01 to 2019-12 (inclusive). Assign it to a value named `mu2010s`.
*)
(***hide***)
let mu2010s =
    excessSpy
    |> List.filter (fun x -> 
        x.Date >= DateTime(2010,1,1) && 
        x.Date <= DateTime(2019,12,31))
    |> List.averageBy (fun x -> x.Return)


(** Write your solution in the cell below.*)
// Solution here

// Test
mu2010s
|> should (equalWithin 0.005) 0.01

(**
> **Task:** If you are a mean-variance investor with a risk aversion parameter $\gamma=3$, what is the optimal weight of SPY in your portfolio over the period 2010-1 to 2019-12? Use your $\mu$ and $\sigma$ from the same period. Assign it to a value named `optimalWeight2010s`.
*)

(***hide***)
let optimalWeight2010s =
    let gamma = 3.0
    mu2010s / (gamma * stdDev2010s ** 2.0)


(** Write your solution in the cell below.*)
// Solution here

// Test
optimalWeight2010s
|> should (equalWithin 0.1) 2.75

(**
> **Task:** Given that optimal weight in SPY for the 2010s, do you think the 2010s were a good decade to be invested in SPY? Why or why not? Explain using your estimate of the `optimalWeight2010s` as part of your justification.
*)

(** Write your solution in the cell below.*)
// Solution here


(**
> **Task:** The `optimalWeight2010s` is close to 2.75. Use a weight of 2.75 to invest in SPY excess returns from 2020-01 to the end of the sample (inclusive). What is the cumulative excess return of this portfolio? Assign it to a value named `cumulativeExcessReturn2020s`.
*)

(***hide***)
let cumulativeExcessReturn2020s =
    let mutable cr = 1.0
    let ex2020s = 
        excessSpy
        |> List.filter (fun x -> x.Date >= DateTime(2020,1,1))
    for x in ex2020s do 
        cr <- cr * (1.0 + 2.75 * x.Return)
    cr - 1.0

(** Write your solution in the cell below.*)
// Solution here

// Test
cumulativeExcessReturn2020s
|> should (equalWithin 0.05) 0.35

(**
> **Task:** Plot the cumulative *excess* return of an investment in SPY levered to 2.75 from the 2020-01 to the end of the sample. The date should be the x-axis and the cumulative excess return should be the y-axis.
*)

(***hide***)
let cumulativeExcessReturnPlot2020s =
    let mutable cr = 1.0
    let ex2020s = 
        excessSpy
        |> List.filter (fun x -> x.Date >= DateTime(2020,1,1))
    [ for x in ex2020s do 
        cr <- cr * (1.0 + 2.75 * x.Return)
        x.Date, cr - 1.0]
    |> Chart.Line
    |> Chart.withTitle("Example Plot")
    |> Chart.withXAxisStyle(TitleText="Date")
    |> Chart.withYAxisStyle(TitleText="Cumulative Excess Return")        

(** Write your solution in the cell below.*)
// Solution here


(**
> **Task:** If you are a mean-variance investor with a risk aversion parameter $\gamma=3$, what is the optimal weight of SPY in your portfolio over the period 2020-1 to the end of the sample? Use $\mu$ and $\sigma$ estimated from 2020-01 to the end of the sample to form your estimate. Assign it to a value named `optimalWeight2020s`.
*)

(***hide***)
let mu2020s = 
    excessSpy
    |> List.filter (fun x -> x.Date >= DateTime(2020,1,1))
    |> List.averageBy (fun x -> x.Return)
let sigma2020s =
    excessSpy
    |> List.filter (fun x -> x.Date >= DateTime(2020,1,1))
    |> stDevBy (fun x -> x.Return)
let optimalWeight2020s =
    let gamma = 3.0
    mu2020s / (gamma * sigma2020s ** 2.0)

(** Write your solution in the cell below.*)
// Solution here

// Test
optimalWeight2020s
|> should (equalWithin 0.1) 0.7

(**
> **Task:** Why is the optimal weight from the 2010s so different from the 2020s? Be specific and justify your answer using the data. What do we learn from this?
*)

(***hide***)
// The answer shoud discuss differences in mus and sigmas.


(** Write your solution in the cell below.*)
// Solution here

