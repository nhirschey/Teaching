(**
---
title: Portfolios
category: Practice Quizzes
categoryindex: 2
index: 7
---
*)

(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](../img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)
*)

(*** hide,define-output:preDetails ***)
"""
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

"""

(*** hide,define-output:postDetails ***)
"""

</details>
</span>
</p>
</div>
"""

(**
# Some good things to reference

[Anonymous Records](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/anonymous-records#syntax). You can read the above link for details, but the point of these is quite simple.

Records have been our main type for holding data for an observation. We've typically defined these ahead of time with a name before using them. This is good for important types that you will use frequently.

If you're using a particular record in only a few lines of code, then it can feel cumbersome to define the type beforehand. Anonymous records are a good solution in these circumstances. They are records that you can essentially use like regular records that we've been using, but you don't have to define the name of the record ahead of time.

I rarely use anonymous records, but you might find them useful for exploratory data manipulation. They're also kind of nice for these short problems because I don't need to define a record for each problem.
*)

(**
# Anonymous records
*)

(**
## Question 1

1. Create a *record* named `ExampleRec` that has an `X` field of type int and a `Y` field of type int. Create an example `ExampleRec` and assign it to a value named `r`.
2. Create an *anonymous record* that has an `X` field of type int and a `Y` field of type int. Create an example of the anonymous record and assign it to a value named `ar`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ExampleRec, define-output: ExampleRec ***)

// a regular named record
type ExampleRec = { X : int; Y : int }
let r = { X = 1; Y = 2}
// an anonymous record. The difference is
// 1. We did not define the type ahead of time.
// 2. We put the pipe symbole "|" inside the curly braces.
let ar = {| X = 1; Y = 2|}
// Note that they are not the same type, so if you
// compare them they will be different even though
// the X and Y fields have the same values.
// For example, running `r = ar` 
// will give a compiler error

(*** condition:html, include:ExampleRec ***)
(*** condition:html, include-fsi-output:ExampleRec ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Imagine you have this list
*)

open System
type ArExample = { Date : DateTime; Value: float}
let arr = [ { Date = DateTime(1990,1,1); Value = 1.25}
            { Date = DateTime(1990,1,2); Value = 2.25}
            { Date = DateTime(1991,1,1); Value = 3.25} ]

(**
1. Group the observations by a tuple of `(year,month)` and find the 
minimum value for each group. Report the result as a tuple of the group
and the minimum value [so it will be `((year, month), minValue)`].
2. Now, the same thing with anonymous records.
Group the observations by an Anonymous Record `{| Year = year; Month= month|}` and find the 
minimum value for each group. Report the result as an Anonymous record with a Group
field for the group and a value field for the minimum value [so it will be
`{| Group = {| Year = year; Month= month|}; Value = minValue |}`].
*)

(*** include-it-raw:preDetails ***)
(*** define: RecordsAndTransformations, define-output: RecordsAndTransformations ***)

// 1.

// Option 1: here I will explicitly put year and month in the final result
arr 
|> List.groupBy(fun x -> x.Date.Year, x.Date.Month)
|> List.map(fun ((year, month), xs) ->
    let minValue = 
        xs 
        |> List.map(fun x -> x.Value)
        |> List.min
    (year, month), minValue) // explicitly put it in the result

// or Option 1:
// since I'm just returning the grouping variable, there's really
// no need to deconstruct it into year, month at any point.
arr 
|> List.groupBy(fun x -> x.Date.Year, x.Date.Month)
|> List.map(fun (group, xs) -> // match group to (year,month) together
    let minValue = 
        xs 
        |> List.map(fun x -> x.Value)
        |> List.min
    group, minValue)

// 2. Now using anonymous records
// This is where anonymous records can be useful.
// For example, sometimes grouping by many things, 
// using anonymous records like this make it more clear what the different
// grouping variables are because they have names.
// It's like a middle ground between tuples with no clear naming structure
// and regular named records that are very explicit.

arr 
|> List.groupBy(fun x -> {| Year = x.Date.Year; Month = x.Date.Month |})
|> List.map(fun (group, xs) -> 
    let minValue = 
        xs 
        |> List.map(fun x -> x.Value)
        |> List.min
    {| Group = group; Value = minValue |})

(*** condition:html, include:RecordsAndTransformations ***)
(*** condition:html, include-fsi-output:RecordsAndTransformations ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
# Portfolio Returns
*)

(**
## Question 1
Imagine that you have the following positions in your portfolio.
For each position you have a weight and a return.
What is the return of the entire portfolio?
*)

type PortReturnPos = { Id: string;  Weight: float; Return: float}
let stockPos = { Id = "stock"; Weight = 0.25; Return = 0.1 }
let bondPos = { Id = "bond"; Weight = 0.75; Return = 0.05}

(*** include-it-raw:preDetails ***)
(*** define: PortfolioRet1, define-output: PortfolioRet1 ***)

// Remember that portfolio returns are a weighted average
// of the returns of the stocks in the portfolio. The weights
// are the position weights.

// Option 1:
let stockAndBondPort = 
    stockPos.Weight*stockPos.Return + bondPos.Weight*bondPos.Return

// Option 2: or, doing the multiplication and summation with collections
let weightXreturn =
    [ for pos in [ stockPos; bondPos ] do pos.Weight * pos.Return ]
// look at it
weightXreturn
// now sum
let stockAndBondPort2 = weightXreturn |> List.sum
// check
stockAndBondPort = stockAndBondPort2 // evaluates to true

(*** condition:html, include:PortfolioRet1 ***)
(*** condition:html, include-fsi-output:PortfolioRet1 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Imagine that you have the following positions in your portfolio.
For each position you have a weight and a return.
What is the return of the entire portfolio?
*)

let positions =
    [ { Id = "stock"; Weight = 0.25; Return = 0.12 }
      { Id = "bond"; Weight = 0.25; Return = 0.22 }
      { Id = "real-estate"; Weight = 0.5; Return = -0.15 } ]

(*** include-it-raw:preDetails ***)
(*** define: EntirePortRet, define-output: EntirePortRet ***)

// Option 1:
let threeAssetPortfolioReturn =
    positions
    |> List.map(fun pos -> pos.Weight*pos.Return)
    |> List.sum

// Option 2:
let threeAssetPortfolioReturn2 =
    positions
    |> List.sumBy(fun pos -> pos.Weight*pos.Return)

// Option 3:
let threeAssetPortfolioReturn3 =
    [ for pos in positions do pos.Weight * pos.Return ]
    |> List.sum

(*** condition:html, include:EntirePortRet ***)
(*** condition:html, include-fsi-output:EntirePortRet ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Imagine that you have the following positions in your portfolio.
For each position you have a weight and a return.
What is the return of the entire portfolio?
*)

let positionsWithShort =
    [ { Id = "stock"; Weight = 0.25; Return = 0.12 }
      { Id = "bond"; Weight = -0.25; Return = 0.22 }
      { Id = "real-estate"; Weight = 1.0; Return = -0.15 } ]

(*** include-it-raw:preDetails ***)
(*** define: EntirePortRetWithShort, define-output: EntirePortRetWithShort ***)

let positionsWithShortReturn =
    positionsWithShort
    |> List.map(fun pos -> pos.Weight*pos.Return)
    |> List.sum


(*** condition:html, include:EntirePortRetWithShort ***)
(*** condition:html, include-fsi-output:EntirePortRetWithShort ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
# Sharpe Ratios
*)

(**
## Question 1
Imagine that you have the following array of *annual* returns in
excess of the risk-free rate. What is the *annualized* Sharpe ratio?
*)

let rets = [ 0.1; -0.4; 0.2; 0.15; -0.03 ]
//Note that the units are such that 0.1 is 10%.

(*** include-it-raw:preDetails ***)
(*** define: AnnualizedSR, define-output: AnnualizedSR ***)

#r "nuget: FSharp.Stats, 0.5.0"
open FSharp.Stats

let retsAvg = rets |> List.average
// we get stDev from FSharp.Stats
let retsStdDev = rets |> stDev 
let retsSharpeRatio = retsAvg / retsStdDev

(*** condition:html, include:AnnualizedSR ***)
(*** condition:html, include-fsi-output:AnnualizedSR ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Imagine that you have the following array of *monthly* returns in
excess of the risk-free rate. What is the *annualized* Sharpe ratio?

```fsharp
let rets = [ 0.1; -0.4; 0.2; 0.15; -0.03 ]
//Note that the units are such that 0.1 is 10%.
```
*)

(*** include-it-raw:preDetails ***)
(*** define: MonthlyRetAnnualSR, define-output: MonthlyRetAnnualSR ***)

// remember that to annualize an arithmetic return,
// we do return * (# compounding periods per year)
// to annualize a standard deviation, 
// we do sd * sqrt(# compounding periods per year)

let monthlyRetsAnnualizedAvg = 12.0*(rets |> List.average)
// or
let monthlyRetsAnnualizedAvg2 = 
    rets 
    |> List.average 
    // now we're going to use a lambda expression.
    // this is the same idea as when we do Array.map(fun x -> ...)
    // except now we're only piping a float, not an array so
    // we're leaving off the "Array.map" 
    |> (fun avg -> 12.0 * avg) 
// or, in two steps
let monthlyRetsAvg = rets |> List.average
let monthlyRetsAnnualizedAvg3 = 12.0*monthlyRetsAvg

// now the standard deviation
let monthlyRetsAnnualizedSd = 
    rets 
    |> stDev
    |> fun monthlySd -> sqrt(12.0) * monthlySd
//or, in two steps
let monthlyRetsSd = rets |> stDev
let monthlyRetsAnnualizedSd2 = sqrt(12.0)*monthlyRetsSd

// SharpeRatio
let annualizedSharpeFromMonthly =
    monthlyRetsAnnualizedAvg / monthlyRetsAnnualizedSd

// or, since 12.0/sqrt(12.0) = sqrt(12.0) then
// (monthlyRetsAvg *12.0)/(monthlyRetsSd*sqrt(12.0)) = 
//      sqrt(12.0)*(monthlyRetsAvg/monthlyRetsSd)
let annualizedSharpeFromMonthly2 =
    sqrt(12.0) * (monthlyRetsAvg / monthlyRetsSd)

// check
// we have to round because floating point math gives us slightly different #'s
// recall from the fundamentals lecture how floating point math is inexact.
Math.Round(annualizedSharpeFromMonthly,6) = Math.Round(annualizedSharpeFromMonthly2,6) // true

(*** condition:html, include:MonthlyRetAnnualSR ***)
(*** condition:html, include-fsi-output:MonthlyRetAnnualSR ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Imagine that you have the following array of *daily* returns in
excess of the risk-free rate. What is the *annualized* Sharpe ratio?

```fsharp
let rets = [ 0.1; -0.4; 0.2; 0.15; -0.03 ]
//Note that the units are such that 0.1 is 10%.
```
*)

(*** include-it-raw:preDetails ***)
(*** define: dailyRetAnnualSR, define-output: dailyRetAnnualSR ***)

// Convention for daily is 252 trading days per year.
// so annualize daily by multiplying by sqrt(252.0)
let annualizedSharpeFromDaily =
    let avgRet = rets |> List.average
    let stdevRet = rets |> stDev
    sqrt(252.0) * (avgRet/stdevRet)
// or in multiple steps
let dailyAvgRet = rets |> List.average
let dailyStDevRet = rets |> stDev
let annualizedSharpeFromDaily2 =
    sqrt(252.0) * (dailyAvgRet/dailyStDevRet)

(*** condition:html, include:dailyRetAnnualSR ***)
(*** condition:html, include-fsi-output:dailyRetAnnualSR ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
# Risky weights

## Question 1

Assume that you are a mean-variance investor where your
utility function has the form

$$
U = \mu - \frac{\gamma}{2}\sigma^2
$$

You plan to allocate only between a risky asset and the risk-free asset.
Your risk aversion parameter $\gamma=3$. For the risky asset, $\mu=0.1$ and $\sigma=0.2$. What is your optimal weight in the risky asset?
*)

(*** include-it-raw:preDetails ***)
(*** define: riskyWeight, define-output: riskyWeight ***)

let mu = 0.1
let sigma = 0.2
let gamma = 3.0
let riskyWeight = mu / (gamma * sigma ** 2.0)

(*** condition:html, include:riskyWeight ***)
(*** condition:html, include-fsi-output:riskyWeight ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.

