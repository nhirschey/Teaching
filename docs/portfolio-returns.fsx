(**

---
title: Portfolio Returns
category: Lectures
categoryindex: 1
index: 3
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

## A portfolio's return.

A portfolio's return is the weighted average return of the portfolio's positions.

$$ r_p = \Sigma^N_{i=1} w_i r_i,$$
where $r$ is return, $i$ indexes stocks, and $w$ is portfolio weights.
*)

type PositionsWithReturn =
    { Id: string 
      Weight: float 
      Return: float }

let exPortfolio =
    [ { Id = "A"; Weight = 0.25; Return = 0.1 }
      { Id = "B"; Weight = 0.75; Return = 0.2 } ]

let weightsXreturn = [ for pos in exPortfolio -> pos.Weight * pos.Return ]
weightsXreturn
(***include-value:weightsXreturn***)

let exPortfolioReturn = weightsXreturn |> List.sum 
exPortfolioReturn
(***include-value: exPortfolioReturn***)


(**
We are now going to look at returns of actual stock and bond portfolios.
The two portfolios are [VTI](https://investor.vanguard.com/etf/profile/VTI) and
[BND](https://investor.vanguard.com/etf/profile/BND). 
These are value-weighted exchange traded funds (ETFs).
VTI tracks a stock market index and BND tracks a bond market index.
They are good proxies for the return of the overall US stock and bond markets.

We are going to load some helper code that allows us to download and plot this data.
This will introduce using `#load` to load scripts with external code,
the `nuget` package manager for loading external libraries,
and how to open namespaces.

When you type `#load "Script.fsx"` in the REPL,
F# interactive compiles the code in `Script.fsx` and puts it into
a code module with the same name as the script.

We are going to use a helper script called `YahooFinance.fsx` that includes
code for requesting price histories from yahoo. To download it,
go to the [YahooFinance](YahooFinance.html) page and click the "download script"
button at the top. Make sure that you have saved it in 
the same directory as this file.

If you have downloaded it correctly then the following code will evaluate to `true`.
*)

System.IO.File.Exists("YahooFinance.fsx")
(***include-it***)

(**
Assuming that the above code evaluated to `true` we can now load it into our session.
*)

#load "YahooFinance.fsx"

(**
Namespaces are a hierarchical way of organizing code.
In the above checking for the existence of a file we have a hierarchy of
`System.IO` where the period `.` separates the `System` and `IO` namespaces.
If we `open` a namespace, then we have access to the code inside the namespace directly.

It is common to open the `System` namespace.
*)

open System

(**
Now we can leave `System` off when accessing code in the `System` namespace.
*)

IO.File.Exists("YahooFinance.fsx")
(***include-it***)

(**
We also want to open the `YahooFinance` module from `YahooFinance.fsx`,
which is similar to a namespace.
*)

open YahooFinance

(**
We are ready to request some data. Let's define our start and end dates.
`DateTime` is a type in the `System` namespace.
We have opened that namespace so we can access the type directly.
*)

let myStart = DateTime(2010,1,1)
let myEnd = DateTime.UtcNow
myEnd
(***include-it***)

(**
Our `YahooFinance` module has code for requesting price histories of stocks.
*)

let bnd = YahooFinance.PriceHistory("BND",startDate=myStart,endDate=myEnd,interval = Interval.Daily)
let vti = YahooFinance.PriceHistory("VTI",startDate=myStart,endDate=myEnd,interval = Interval.Daily)

(**
This returns several data items for each point in time.
*)

(***do-not-eval***)
vti[0..3]
(***hide***)
vti.[0..3]
(***include-it***)

(**
The adjusted close is adjusted for stock splits and dividends.
This adjustment is done so that you can calculate returns from the price changes.

Let's see what it looks like to plot it. 
We're going to use the [plotly.NET](https://plotly.net) library for this.
We download the code from the [nuget.org](http://www.nuget.org) package manager.

This is equivalent to loading libraries with `pip` or `conda` in python
or `install.packages` in R.

*)

#r "nuget: Plotly.NET, 2.0.0-preview.17"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.17"


open Plotly.NET

(**
Above we are loading an exact version by using a "," and version number.

Plot prices as a line chart.
*)

let vtiAdjPrices = [ for period in vti -> period.Date, period.AdjustedClose ]

(***do-not-eval***)
Chart.Line(vtiAdjPrices)
(***hide***)
Chart.Line(vtiAdjPrices)
|> GenericChart.toChartHTML
(***include-it-raw***)

(**
Ok, back to the main objective. We need to calculate returns.
We calculate returns from sequential days,
so we need to make sure that our data is sorted correctly
from the oldest to the newest data. We can do this with `List.Sort`.

*)

[1; 7; 10; 2; -1] |> List.sort
(***include-it***)

(** Sort it by the date field. *)
let sortedBnd = bnd |> List.sortBy (fun x -> x.Date)

(** The first three observations. *)
(***do-not-eval***)
sortedBnd[0..2]
(***hide***)
sortedBnd.[0..2]
(***include-it***)

(** The last 3 observations. *)
(***do-not-eval***)
sortedBnd[(sortedBnd.Length-3)..]
(***hide***)
sortedBnd.[(sortedBnd.Length-3)..]
(***include-it***)

(** 
Great, they are properly sorted. Now I want sequential pairs of data.
`List.pairwise` is good for this.
*)

[1 .. 5] |> List.pairwise
(***include-it***)

let sequentialBnd = bnd |> List.pairwise
(***do-not-eval***)
sequentialBnd[0]
(***hide***)
sequentialBnd.[0]
(***include-it***)
(***do-not-eval***)
sequentialBnd[1]
(***hide***)
sequentialBnd.[1]
(***include-it***)

(**
Take the first pair to see how to calculate returns.

Extract the first and second elements of the tuple using pattern matching. *)

(***do-not-eval***)
let (bndA, bndB) = sequentialBnd[0]
bndA
(***hide***)
let (bndA', bndB') = sequentialBnd.[0]
bndA'
(***include-it***)
(***do-not-eval***)
bndB
(***hide***)
bndB'
(***include-it***)

(**
Remember that with continuous compounding, $FV_T = PV_t \times e^{r}$
where $FV$ is the future value, $PV$ is the present value, $r$ is return
between period $t$ and $T$.

If we take the log of both sides of the equation, we get

$$ log(FV) = log(PV) + r \rightarrow log(FV) - log (PV) = r$$

This $r$ is known as the log return. 
So to find the log return between two periods we can take the 
difference of the log prices (where the prices are adjusted for dividends).

*)
(log bndB.AdjustedClose) - (log bndA.AdjustedClose)
(***include-it***)


(**
Putting it all together.
*)
let bndReturn = 
    bnd
    |> List.sortBy (fun x -> x.Date)
    |> List.pairwise
    |> List.map (fun (a, b) -> (log b.AdjustedClose) - (log a.AdjustedClose))

let vtiReturn =
    vti
    |> List.sortBy (fun x -> x.Date)
    |> List.pairwise
    |> List.map (fun (a, b) -> (log b.AdjustedClose) - (log a.AdjustedClose))


let bndAvgReturn = bndReturn |> List.average
bndAvgReturn
(***include-it***)

let vtiAvgReturn = vtiReturn |> List.average
vtiAvgReturn
(***include-it***)

(*** Portfolio returns for different weights. *)

let differentReturns =
  [ for w in [0.0 .. 0.2 .. 1.0] -> w, w*bndAvgReturn + (1.0-w)*vtiAvgReturn ]

differentReturns
(***include-it***)

Chart.Line(differentReturns)