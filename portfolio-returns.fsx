(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=portfolio-returns.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//portfolio-returns.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//portfolio-returns.ipynb)

## A portfolio's return.

A portfolio's return is the weighted average return of the portfolio's positions.

\begin{equation}
 r_p = \Sigma^N_{i=1} w_i r_i,
\end{equation}

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
weightsXreturn(* output: 
[0.025; 0.15]*)
let exPortfolioReturn = weightsXreturn |> List.sum 
exPortfolioReturn(* output: 
0.175*)
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
System.IO.File.Exists("YahooFinance.fsx")(* output: 
true*)
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
IO.File.Exists("YahooFinance.fsx")(* output: 
true*)
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
myEnd(* output: 
6/14/2022 6:16:01 PM*)
(**
Our `YahooFinance` module has code for requesting price histories of stocks.

*)
let bnd = YahooFinance.PriceHistory("BND",startDate=myStart,endDate=myEnd,interval = Interval.Daily)
let vti = YahooFinance.PriceHistory("VTI",startDate=myStart,endDate=myEnd,interval = Interval.Daily)
(**
This returns several data items for each point in time.

*)
vti[0..3](* output: 
[{ Symbol = "VTI"
   Date = 1/4/2010 12:00:00 AM
   Open = 56.860001
   High = 57.380001
   Low = 56.84
   Close = 57.310001
   AdjustedClose = 45.630634
   Volume = 2251500.0 }; { Symbol = "VTI"
                           Date = 1/5/2010 12:00:00 AM
                           Open = 57.34
                           High = 57.540001
                           Low = 57.110001
                           Close = 57.529999
                           AdjustedClose = 45.80579
                           Volume = 1597700.0 }; { Symbol = "VTI"
                                                   Date = 1/6/2010 12:00:00 AM
                                                   Open = 57.5
                                                   High = 57.720001
                                                   Low = 57.41
                                                   Close = 57.610001
                                                   AdjustedClose = 45.869495
                                                   Volume = 2120300.0 };
 { Symbol = "VTI"
   Date = 1/7/2010 12:00:00 AM
   Open = 57.549999
   High = 57.889999
   Low = 57.290001
   Close = 57.849998
   AdjustedClose = 46.060589
   Volume = 1656700.0 }]*)
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
Chart.Line(vtiAdjPrices)(* output: 
No value returned by any evaluator*)
(**
Ok, back to the main objective. We need to calculate returns.
We calculate returns from sequential days,
so we need to make sure that our data is sorted correctly
from the oldest to the newest data. We can do this with `List.Sort`.

*)
[1; 7; 10; 2; -1] |> List.sort(* output: 
[-1; 1; 2; 7; 10]*)
(**
Sort it by the date field.

*)
let sortedBnd = bnd |> List.sortBy (fun x -> x.Date)
(**
The first three observations.

*)
sortedBnd[0..2](* output: 
No value returned by any evaluator*)
(**
The last 3 observations.

*)
sortedBnd[(sortedBnd.Length-3)..](* output: 
No value returned by any evaluator*)
(**
Great, they are properly sorted. Now I want sequential pairs of data.
`List.pairwise` is good for this.

*)
[1 .. 5] |> List.pairwise(* output: 
[(1, 2); (2, 3); (3, 4); (4, 5)]*)
let sequentialBnd = bnd |> List.pairwise
sequentialBnd[0](* output: 
({ Symbol = "BND"
   Date = 1/4/2010 12:00:00 AM
   Open = 78.599998
   High = 78.730003
   Low = 78.540001
   Close = 78.68
   AdjustedClose = 55.680157
   Volume = 1098100.0 }, { Symbol = "BND"
                           Date = 1/5/2010 12:00:00 AM
                           Open = 78.889999
                           High = 79.0
                           Low = 78.790001
                           Close = 78.910004
                           AdjustedClose = 55.842953
                           Volume = 814600.0 })*)
sequentialBnd[1](* output: 
({ Symbol = "BND"
   Date = 1/5/2010 12:00:00 AM
   Open = 78.889999
   High = 79.0
   Low = 78.790001
   Close = 78.910004
   AdjustedClose = 55.842953
   Volume = 814600.0 }, { Symbol = "BND"
                          Date = 1/6/2010 12:00:00 AM
                          Open = 78.970001
                          High = 78.980003
                          Low = 78.699997
                          Close = 78.879997
                          AdjustedClose = 55.82172
                          Volume = 981300.0 })*)
(**
Take the first pair to see how to calculate returns.

Extract the first and second elements of the tuple using pattern matching.

*)
let (bndA, bndB) = sequentialBnd[0]
bndA(* output: 
{ Symbol = "BND"
  Date = 1/4/2010 12:00:00 AM
  Open = 78.599998
  High = 78.730003
  Low = 78.540001
  Close = 78.68
  AdjustedClose = 55.680157
  Volume = 1098100.0 }*)
bndB(* output: 
{ Symbol = "BND"
  Date = 1/5/2010 12:00:00 AM
  Open = 78.889999
  High = 79.0
  Low = 78.790001
  Close = 78.910004
  AdjustedClose = 55.842953
  Volume = 814600.0 }*)
(**
Remember that with continuous compounding, $FV_T = PV_t \times e^{r}$
where $FV$ is the future value, $PV$ is the present value, $r$ is return
between period $t$ and $T$.

If we take the log of both sides of the equation, we get

\begin{equation}
 log(FV) = log(PV) + r \rightarrow log(FV) - log (PV) = r
\end{equation}

This $r$ is known as the log return.
So to find the log return between two periods we can take the
difference of the log prices (where the prices are adjusted for dividends).

*)
(log bndB.AdjustedClose) - (log bndA.AdjustedClose)(* output: 
No value returned by any evaluator*)
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
bndAvgReturn(* output: 
No value returned by any evaluator*)
let vtiAvgReturn = vtiReturn |> List.average
vtiAvgReturn(* output: 
No value returned by any evaluator*)
(**
* Portfolio returns for different weights.

*)
let differentReturns =
  [ for w in [0.0 .. 0.2 .. 1.0] -> w, w*bndAvgReturn + (1.0-w)*vtiAvgReturn ]

differentReturns(* output: 
No value returned by any evaluator*)
Chart.Line(differentReturns)

