(**
---
title: Signal Exploration
category: Assignments
categoryindex: 2
index: 5
---

[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](../img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


Group Name: 

| Student Name | Student Number  | 
| -----------  | --------------  |
| **1:**       |                 |
| **2:**       |                 | 
| **3:**       |                 | 
| **4:**       |                 | 
| **5:**       |                 |

This is an assignment. You should work in groups. Please write your group and group member names above. You will find sections labeled **Task** asking you to do each piece of analysis. Please make sure that you complete all of these tasks. I included some tests to help you see if you are calculating the solution correctly, but if you cannot get the test to pass submit your best attempt and you may recieve partial credit.

All work that you submit should be your own. Make use of the course resources and example code on the course website. It should be possible to complete all the requested tasks using information given below or somewhere on the course website.

*)

(**

Load libraries.
*)

#r "nuget: FSharp.Data"
#r "nuget: FSharp.Stats"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"
#r "nuget: MathNet.Numerics"
#r "nuget: MathNet.Numerics.FSharp"

open System
open FSharp.Data
open FSharp.Stats
open Plotly.NET
open MathNet.Numerics.Statistics

(**
### First, make sure that you're referencing the correct files.

You're going to write code to explore an investment signal. The investment signals can be downloaded from moodle. You are free to use whichever signal you want. It does not have to be the signal that you are using for your final project.

Here I'm assuming that you have a class folder with this `signal-exploration.ipynb` notebook and a `data` folder inside of it. The folder hierarchy would look like below where you
have the below files and folders accessible:

```code
/class
    signal-exploration.ipynb
    id_and_return_data.csv
    zero_trades_252d.csv
    
```

First, make sure that our working directory is the source file directory.
*)

let [<Literal>] ResolutionFolder = __SOURCE_DIRECTORY__
Environment.CurrentDirectory <- ResolutionFolder

(**
We assume the `id_and_return_data.csv` file and the signal csv file  are in the `data` folder. In this example the signal file is `zero_trades_252d.csv`. You should replace that file name with your signal file name.
*)

let [<Literal>] IdAndReturnsFilePath = "id_and_return_data.csv"
let [<Literal>] MySignalFilePath = "zero_trades_252d.csv"

(**
## Input data
First, let's verify that the csv file exists where we think it will be.
*)

if IO.File.Exists(ResolutionFolder + "/" + IdAndReturnsFilePath) &&
   IO.File.Exists(ResolutionFolder + "/" + MySignalFilePath) then 
    printfn "Success!!"
else
    let filesThere = IO.Directory.EnumerateFiles(
        ResolutionFolder,
        searchPattern = "*",
        searchOption=IO.SearchOption.AllDirectories)
    printfn "We did not find the files. Here are the files in your source directory.\n"
    filesThere |> Seq.iteri (printfn "%i. %A")

(**
Assuming that you got "Success!!" above this code below will work.
*)
(**
If my paths are correct, then this code should read the first few lines of the files.
If it doesn't show the first few lines, fix the above file paths.
*)

IO.File.ReadLines(IdAndReturnsFilePath) |> Seq.truncate 5
(** *)
IO.File.ReadLines(MySignalFilePath) |> Seq.truncate 5
(** *)

(** Assuming the paths are defined correctly and you saw the first 5 rows above,
we can now read the data using the CSV provider that parses the fields in the file.

First define the Csv types from the sample files:
*)

type IdAndReturnsType = 
    CsvProvider<Sample=IdAndReturnsFilePath,
                ResolutionFolder=ResolutionFolder>

type MySignalType = 
    CsvProvider<MySignalFilePath,
                ResolutionFolder=ResolutionFolder>

(** Now read in the data.*)
let idAndReturnsCsv = IdAndReturnsType.GetSample()

let mySignalCsv = MySignalType.GetSample()
    

(**
Columns in the `idAndReturnsCsv` are:
*)
idAndReturnsCsv.Headers

(**
Columns in the `mySignalCsv` are:
*)
mySignalCsv.Headers

(**
There are a lot of columns in the id and returns csv. You can look at the data documentation to figure out what they are.

Put the rows into a list (we're more familiar with lists).
*)

let idAndReturnsRows = idAndReturnsCsv.Rows |> Seq.toList
let mySignalRows = mySignalCsv.Rows |> Seq.toList

(**
### Distribution of unique stocks in the id and returns data

To get you started, I will walk you through some simple analysis of the id and returns data.

Count the total number of stocks.

First, look at a few ids
*)

idAndReturnsRows
|> List.map (fun row -> row.Id)
|> List.truncate 5

(** Now count all of them. *)
idAndReturnsRows
|> List.map (fun row -> row.Id)
|> List.distinct
|> List.length

(**
Number of stocks each month.

First look at the date column
*)

idAndReturnsRows
|> List.map (fun row -> row.Eom)
|> List.truncate 5

(** Group by month, then count per month. *)

let idAndReturnStocksPerMonth =
    let byMonth =
        idAndReturnsRows
        |> List.groupBy (fun row -> row.Eom)
        |> List.sortBy (fun (month, rows) -> month)
    [ for (month, rows) in byMonth do
        let nStocks = 
            rows
            |> List.map (fun row -> row.Id)
            |> List.distinct
            |> List.length
        month, nStocks ]

(** Look at a first few months. *)
idAndReturnStocksPerMonth
|> List.sortBy (fun (month, nStocks) -> month) 
|> List.truncate 5

(** Look at the last few.*)
idAndReturnStocksPerMonth
|> List.sortByDescending (fun (month, nStocks) -> month)
|> List.truncate 5

(** Create a column chart showing the number of stocks per month (Plotly.net column chart [docs](https://plotly.net/02_1_bar-and-column-charts.html)). *)
idAndReturnStocksPerMonth
|> Chart.Column

(** Add some lables to the axes (Plotly.net axis styling [docs](https://plotly.net/01_0_axis-styling.html)). *)
idAndReturnStocksPerMonth
|> List.sortBy (fun (month, nStocks) -> month)
|> Chart.Column
|> Chart.withXAxisStyle (TitleText="Month")
|> Chart.withYAxisStyle (TitleText="Number of Stocks")

(** We have some different size groups already assigned in the data: *)
idAndReturnsRows
|> List.countBy (fun row -> row.SizeGrp)

(**
Let's make a plot with separate bars for each group in 2015. You can read more about multiple charts in the Plotly.net [docs](https://plotly.net/01_2_multiple-charts.html).

We'll write a function. We need to give a type hint so that
it knows the type of the input data. If we didn't include the type hint, we'd get an error saying 'Lookup of indeterminate type ..' because it doesn't know the data type of the 'rows' input. The type hint the  `: list<IdAndReturnsType.Row>` part of the function definition.
This is saying we have a list of rows from the CsvProvider type that we defined earlier for this csv file data.
*)

let countIdAndReturnsRows (rows: list<IdAndReturnsType.Row>) =
    let byMonth =
        rows
        |> List.groupBy (fun row -> row.Eom)
        |> List.sortBy (fun (month, rows) -> month)
    [ for (month, rows) in byMonth do
        let nStocks = 
            rows
            |> List.map (fun row -> row.Id)
            |> List.distinct
            |> List.length
        month, nStocks ]

(** Look at the function output. It is a list of tuples where each tuple is a pair of month (`DateTime`) and the count (`int`). *)

idAndReturnsRows
|> countIdAndReturnsRows
|> List.truncate 3

(** Just for large caps.*)

let stockCountsLarge =
    let toPlot = 
        idAndReturnsRows
        |> List.filter (fun row -> 
            row.SizeGrp = "large" && 
            row.Eom.Year = 2015)
        |> countIdAndReturnsRows
    Chart.Column(toPlot, Name = "Large caps")

stockCountsLarge

(** Just for small caps.*)

let stockCountsSmall =
    let toPlot = 
        idAndReturnsRows
        |> List.filter (fun row -> 
            row.SizeGrp = "small" &&
            row.Eom.Year = 2015)
        |> countIdAndReturnsRows
    Chart.Column(toPlot, Name = "Small caps")

stockCountsSmall


(** combined: *)
[ stockCountsLarge; stockCountsSmall ]
|> Chart.combine

(** Now all groups *)

let stockCountsAllSizes =
    idAndReturnsRows
    |> List.filter (fun row -> row.Eom.Year = 2015)
    |> List.groupBy (fun row -> row.SizeGrp)
    |> List.map (fun (sizeGrp, rows) -> 
        let toPlot = countIdAndReturnsRows rows
        sizeGrp, toPlot)

// first few observations of all size Groups
stockCountsAllSizes
|> List.map (fun (sizeGroup, xs) ->
    sizeGroup, xs |> List.truncate 3)

(** A combined chart. *)

stockCountsAllSizes
|> List.map (fun (sizeGrp, toPlot) -> 
    Chart.Column(toPlot, Name = sizeGrp))
|> Chart.combine

(** Same, but stacking each chart on top of eachother. *)   

stockCountsAllSizes
|> List.map (fun (sizeGrp, toPlot) -> 
    Chart.Column(toPlot, Name = sizeGrp))
|> Chart.SingleStack()

(**
You should now have some a good idea of how to work with this data.

### Distribution of unique stocks in the your signal data

Do similar analysis as above, but for the your signal data.

> **Task:** Complete this function. It takes a list of `MySignalType.Row` as input and should return a list of the month and the integer count of unique stock ids that month (`list<DateTime * int>`).
*)

// solution here
let countMySignalRows (rows: list<MySignalType.Row>) =
    failwith "I am not implemented yet"
    
(**
> **Task:** Create a column chart showing the number of stocks per month in your signal data csv file.
*)

// solution here

(**
You may have some stocks with missing data. If you have some stocks with missing signal data, the below code will return the first 3 observations.
If you do not have missing data it will return an empty list.
*)

mySignalRows
|> List.choose (fun row -> 
    // Choose the rows where row.Signal is None.
    match row.Signal with
    | None -> Some row
    | Some signal -> None )
|> List.truncate 3

(** We can create a list that only contains stocks with non-missing signals. We define a record type to hold this data. The main change is making signal have `float` type instead of `Option<float>` because we're removing missing data.*)

type NonMissingSignal =
    {
        Id: string
        Eom: DateTime
        Signal: float
    }

let myNonMissingSignals =
    mySignalRows
    |> List.choose (fun row -> 
        match row.Signal with
        | None -> None
        | Some signal -> 
            Some { Id = row.Id; Eom = row.Eom; Signal = signal })

(**
> **Task:** Complete this function. It takes a list of `NonMissingSignal` records as input and should return a list of the month and the integer count of unique stock ids that month (`list<DateTime * int>`).

*)
// solution here
let countMyNonMissingSignalRows (rows: list<NonMissingSignal>) =
    failwith "I am not implemented yet"
    
(**
> **Task:** Create a column chart showing the number of stocks per month in your signal data that *do not* have missing signals.
*)
// solution here

(**
### Distribution of the signal

> **Task:** Compute the minimum, maximum, median, standard deviation, and average of the non-missing signals in your dataset.
*)
// solution here.


(**
It can also be useful to compute percentiles of the signal. You can calculate percentils using `MathNet.Numerics.Statistics` quantile function.
*)

// 10th, 50th, and 90th percentiles
let pctlExamples = [0.1; 0.5; 0.9]

// you must have an array of values
let pctlExamplesData = 
    [ 10.0; -20.0; 0.1; -5.0; 7.0; 4.0]
    |> List.toArray 

(** Compute the percentiles. *)
let pctlExamplesComputed =    
    [ for pctl in pctlExamples do
        Statistics.quantileFunc pctlExamplesData pctl ]
pctlExamplesComputed

(**
> **Task:** Compute the 1st, 10th, 50th, 90th, and 99th percentiles of the non-missing signals in your dataset. Once these percentiles are calculated them, assign the signals to the values below. Explain what you learn about the distribution. Is it uniformly distributed, a skewed distribution, are there outliers, etc.?
*)
// solution here

let signalP01: float = failwith "I am not implemented yet"
let signalP10: float = failwith "I am not implemented yet"
let signalP50: float = failwith "I am not implemented yet"
let signalP90: float = failwith "I am not implemented yet"
let signalP99: float = failwith "I am not implemented yet"

(**
> **Task:** Create a [histogram](https://plotly.net/04_0_histograms.html) showing the distribution of the signal for all stocks in your dataset that have non-missing signals. Limit the data to 2015 to make it easier to plot. Explain what you learn about the distribution. Is it uniformly distributed, are there outliers, etc. How do you see this in the plot, and is there anything new that you learned relative to the percentiles?
*)
// solution here.


(**
[Winsorizing](https://en.wikipedia.org/wiki/Winsorizing) is a technique to remove the influence of outliers from a dataset. Let's create a winsorized version of your data.

Assuming that you have defined the percentile above correctly, this will create a winsorized version of your signal dataset. It is winsorized at the 1st and 99th percentiles.
*)

let winsorizeSignals (signalOb: NonMissingSignal) =
    let newSignal =
        if signalOb.Signal < signalP01 then 
            signalP01
        elif signalOb.Signal > signalP99 then
            signalP99
        else
            signalOb.Signal
    // copy and update the observation with the
    // winsorized signal.
    { signalOb with Signal = newSignal }

(** Test on a random signal *)
winsorizeSignals myNonMissingSignals[99]

(** do for all *)
let myWinsorizedSignals =
    myNonMissingSignals
    |> List.map winsorizeSignals

(**
> **Task:** Create a [histogram](https://plotly.net/04_0_histograms.html) showing the distribution of the *winsorized signals* for all stocks in your dataset. Limit the data to 2015 to make it easier to plot. Explain what you learn about the distribution. Is it uniformly distributed, are there outliers, etc. How do you see this in the plot, and is there anything new that you learned relative to the percentiles and non-winsorized histogram?
*)
// solution here.

(**
> **Task:** Create a map collection called `byStockMonthIdAndReturnMap` where the key is a tuple of stock id as string and month as DateTime (`string * DateTime`) and the value is an `IdAndReturnsType.Row`. 

*Note:* I have added a type constraint of `: Map<(string * DateTime), IdAndReturnsType.Row>` to make sure that the type of the map is correct. If you fill in code below, you will get a type mismatch error until your code is correct. You don't generally need these type constraints, but I am putting it here to make the compiler check that you produce the output that I am asking for. 

*Hint:* we did things like this in the momentum signal lecture. There's also a practice quiz on map collections.
  
*)
// solution here
let byStockMonthIdAndReturnMap: Map<string * DateTime, IdAndReturnsType.Row> =
    // fill in code here
    failwith "you haven't created your map collection."

(**
> **Task:** Create a [histogram](https://plotly.net/04_0_histograms.html) showing the distribution of the *winsorized signals* for only *small-cap stocks* in your dataset. Limit the data to 2015 to make it easier to plot.

*Hint:* if you have a stock and it's signal in a particular month, the `byStockMonthIdAndReturnMap` is useful for looking up thinks about the stock that month.)
*)
// solution here

(**
> **Task:** Create a [histogram](https://plotly.net/04_0_histograms.html) showing the distribution of the *winsorized signals* for only *large-cap stocks* in your dataset. Limit the data to 2015 to make it easier to plot.

*)
// solution here

(**
> **Task:** Compare and contrast the histograms for the *small-cap* and *large-cap* stocks. Are there any differences? If we wanted to sort stocks based on the signal, do you think that we would end up with stocks that have different average sizes in the low and high signal portfolios? 
*)

(**
### Towards portfolios.

> **Task:** Using your winsorized list of signals, group your stocks by month. Assign this result to a value named `byStockMonthSignals` that is a list of `DateTime * list<NonMissingSignal>` tuples. The first thing in the tuple is the month and the second thing is a list of `NonMissingSignal` records for all stocks in that month.
*)

// solution here
let byStockMonthSignals: list<DateTime * list<NonMissingSignal>> =
    failwith "I am not implemented yet"

(**
Now assuming `byStockMonthSignals` is correct, we'll sort the stocks each month from smallest to largest based on the signal that month. Then split the stocks into 3 equal-sized portfolios (aka terciles) based on the sorted signal. We'll create a `SortedPort` record for each portfolio and assign the list to a value named `terciles`. 
*)

type SortedPort =
    { Portfolio: int
      Eom: DateTime
      Stocks: list<NonMissingSignal> }

let terciles =
    byStockMonthSignals
    |> List.collect (fun (eom, signals) ->
        let sortedSignals =
            signals
            |> List.sortBy (fun signalOb -> signalOb.Signal)
            |> List.splitInto 3
        sortedSignals
        |> List.mapi (fun i p -> 
            { Portfolio = i + 1
              Eom = eom
              Stocks = p }))

(** look at the first portfolio *)
terciles[0]

(** look at the last portfolio *)
terciles |> List.last

(**
> **Task:** Using `terciles`, compute the average signal in each tercile portfolio each month. Plot a combined (`Chart.combine`) line chart (`Chart.line`) showing the average signal for each tercile portfolio from the start to the end of the sample. What do you learn? Is the average signal in each tercile constant throughout the sample, or does it vary over time? 
*)

(**
> **Task:** Using `byStockMonthSignals`, sort the stocks each month from smallest to largest based on the signal that month. Then split the stocks into 5 equal-sized portfolios (aka quintiles) based on the sorted signal. Create a `SortedPort` record for each portfolio and assign the list to a value named `quintiles`.*)

// solution here
let quintiles: list<SortedPort> =
    failwith "I am not implemented yet"

(**
> **Task:** Filter `quintiles` to the quintile portfolio of stocks each month that has the lowest signal value. This should be stocks where `SortedPort.Portfolio = 1`. Assign the filtered list to a value named `bottomQuintile`.
*)

// solution here
let bottomQuintile: list<SortedPort> =
    failwith "I am not implemented yet"

(**
> **Task:** Create a list named `bottomQuintileReturn` that contains the return of the bottom quintile portfolio each month. The portfolio return for a given month should be calculated using equal weights on every stock in the portfolio that month. The result should be given as a list of `SortedPortfolioReturn` records. **Additionally,** the month of the return should be lagged one month relative to the portfolio formation month. That means that if you formed a portfolio based on a signal known as of the end of February 2022 (Eom = DateTime(2022,02,28)), the portfolio return during the first month that you hold it will be calculated using stock returns during March 2022 (MonthOfReturn = DateTime(2022,03,31)). 

Quick example getting end of month addition:
*)
let endOfFebruary = DateTime(2022,02,28)

let addOneEom (eom: DateTime) =
    DateTime(eom.Year, eom.Month, 1).AddMonths(2).AddDays(-1.0)

addOneEom endOfFebruary
(** That will give you the end of March. So in summary, if the signal that you use to form portfolios comes from February 2022 (signal EOM = DateTime(2022,2,28)), make sure that you get returns from March 2022 (return EOM = DateTime(2022,3,31)). *)

type SortedPortfolioReturn =
    { 
        Portfolio: int
        MonthOfReturn: DateTime
        AvgReturn: float
    }

let bottomQuintileReturn: list<SortedPortfolioReturn> =
    failwith "I am not implemented yet"

(**
> **Task:** Plot a line chart of the cumulative return of the bottom quintile portfolio during the sample. For reference you will find the [plotting returns](https://nhirschey.github.io/Teaching/Momentum-Class.html#Plotting-returns) section of the momentum class lecture useful. It provides an example of calculating a portfolio's cumulative returns using `List.scan`.
*)

