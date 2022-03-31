(**
---
title: Signal Portfolio
category: Lectures
categoryindex: 1
index: 6
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

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

Here I'm assuming that you have a class folder with this `signal-exploration.ipynb` notebook and a `data` folder inside of it. The folder hierarchy would look like below where you
have the below files and folders accessible:

```code
/class
    Portfolio.fsx
    Common.fsx
    signal-portfolio.ipynb
    /data
        id_and_return_data.csv
        zero_trades_252d.csv
    
```

First, make sure that our working directory is the source file directory.
*)

let [<Literal>] ResolutionFolder = __SOURCE_DIRECTORY__
Environment.CurrentDirectory <- ResolutionFolder

(**
### We will use the portfolio module
Make sure that you load this code
*)

#load "Portfolio.fsx"
open Portfolio

(**
### We will use the Common.fsx file
Make sure that this loads correctly.
*)

#load "Common.fsx"
open Common


(**
### Data files
We assume the `id_and_return_data.csv` file and the signal csv file  are in the `data` folder. In this example the signal file is `zero_trades_252d.csv`. You should replace that file name with your signal file name.
*)

let [<Literal>] IdAndReturnsFilePath = "data/id_and_return_data.csv"
let [<Literal>] MySignalFilePath = "data/zero_trades_252d.csv"
let strategyName = "Zero-trades"

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
                // The schema parameter is not required,
                // but I am using it to override some column types
                // to make filtering easier.
                // If I didn't do this these particular columns 
                // would have strings of "1" or "0", but explicit boolean is nicer.
                Schema="obsMain(string)->obsMain=bool,exchMain(string)->exchMain=bool",
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
We want to be able to look up idAndReturn data
and signal data using a security's ID and month.
To do that, we create a Map collection where the key
is a tuple of the security id and month.

- In this dataset, we'll use `row.Id` as the identifier. We'll assign it to
the `Other` SecurityId case, because it's a dataset specific one.
- In this dataset, the Eom variable defines the "end of month".
- The returns are for the month ending in EOM.
- The signals are "known" as of EOM. So you can use them on/after EOM. We'll
form portfolios in the month ending EOM; that's the `FormationMonth`.
*)
let msfBySecurityIdAndMonth =
    idAndReturnsRows
    |> List.map(fun row -> 
        let id = Other row.Id
        let month = DateTime(row.Eom.Year,row.Eom.Month,1)
        let key = id, month
        key, row)
    |> Map    

let signalBySecurityIdAndMonth =
    mySignalRows
    |> List.choose(fun row -> 
        // we'll use choose to drop the security if the signal is None.
        // The signal is None when it is missing.
        match row.Signal with
        | None -> None // choose will drop these None observations
        | Some signal ->
            let id = Other row.Id
            let month = DateTime(row.Eom.Year,row.Eom.Month,1)
            let key = id, month
            // choose will convert Some(key,signal) into
            // (key,signal) and keep that.
            Some (key, signal))
    |> Map    

(**
The `securitiesByFormationMonth` that we'll use to define our investment universe.

*)
let securitiesByFormationMonth =
    idAndReturnsRows
    |> List.groupBy(fun x -> DateTime(x.Eom.Year, x.Eom.Month,1))
    |> List.map(fun (ym, obsThisMonth) -> 
        let idsThisMonth = [ for x in obsThisMonth do Other x.Id ]
        ym, idsThisMonth)
    |> Map

let getInvestmentUniverse formationMonth =
    match Map.tryFind formationMonth securitiesByFormationMonth with
    | Some securities -> 
        { FormationMonth = formationMonth 
          Securities = securities }
    | None -> failwith $"{formationMonth} is not in the date range"
   
(** Checking universe *)
let testUniverseObs = getInvestmentUniverse (DateTime(2015,4,1))

(** Formation month.*)
testUniverseObs.FormationMonth

(** First few securities *)
testUniverseObs.Securities[0..4]

(**
Now I want to be able to get my signal. 
We're going to assume here that a "high" signal
predicts high returns. If you have a signal where
a "high" signal predicts low returns, you can
multiply the signal by `-1.0` below.
*)

let getMySignal (securityId, formationMonth) =
    match Map.tryFind (securityId, formationMonth) signalBySecurityIdAndMonth with
    | None -> None
    | Some signal ->
        Some { SecurityId = securityId 
               // if a high signal means low returns,
               // use `-signal` here instead of `signal`
               Signal = signal }

(** Test it *)
[ for securityId in testUniverseObs.Securities[0..4] do
    let testObs = (securityId, testUniverseObs.FormationMonth)
    getMySignal testObs ]

(** A function to do it for the whole investment universe. *)
let getMySignals (investmentUniverse: InvestmentUniverse) =
    let listOfSecuritySignals =
        investmentUniverse.Securities
        |> List.choose(fun security -> 
            getMySignal (security, investmentUniverse.FormationMonth))    
    
    { FormationMonth = investmentUniverse.FormationMonth 
      Signals = listOfSecuritySignals }


(** 
And I should be able to get my market capitalization
*)

msfBySecurityIdAndMonth
|> Map.toList
|> List.take 3
|> List.map (fun ((id, month), row) -> id, row.MarketEquity)

(** Now a function to do this.*)

let getMarketCap (security, formationMonth) =
    match Map.tryFind (security, formationMonth) msfBySecurityIdAndMonth with
    | None -> None
    | Some row -> 
        match row.MarketEquity with
        | None -> None
        | Some me -> Some (security, me)



(**
And I should be able to get my returns.
*)

let getSecurityReturn (security, formationMonth) =
    // If the security has a missing return, assume that we got 0.0.
    // Note: If we were doing excess returns, we would need 0.0 - rf.
    let missingReturn = 0.0
    match Map.tryFind (security, formationMonth) msfBySecurityIdAndMonth with
    | None -> security, missingReturn
    | Some x ->  
        match x.Ret with 
        | None -> security, missingReturn
        | Some r -> security, r

(**
And we should do a few restrictions. These come from the data documentation
section 1.2, "How to use the data". These are some basic ones.
*)

let isObsMain (security, formationMonth) =
    match Map.tryFind (security, formationMonth) msfBySecurityIdAndMonth with
    | None -> false
    | Some row -> row.ObsMain

let isPrimarySecurity (security, formationMonth) =
    match Map.tryFind (security, formationMonth) msfBySecurityIdAndMonth with
    | None -> false
    | Some row -> row.PrimarySec

let isCommonStock (security, formationMonth) =
    match Map.tryFind (security, formationMonth) msfBySecurityIdAndMonth with
    | None -> false
    | Some row -> row.Common

let isExchMain (security, formationMonth) =
    match Map.tryFind (security, formationMonth) msfBySecurityIdAndMonth with
    | None -> false
    | Some row -> row.ExchMain

let hasMarketEquity (security, formationMonth) =
    match Map.tryFind (security, formationMonth) msfBySecurityIdAndMonth with
    | None -> false
    | Some row -> row.MarketEquity.IsSome

let myFilters securityAndFormationMonth =
    isObsMain securityAndFormationMonth &&
    isPrimarySecurity securityAndFormationMonth &&
    isCommonStock securityAndFormationMonth &&
    isExchMain securityAndFormationMonth &&
    isExchMain securityAndFormationMonth &&
    hasMarketEquity securityAndFormationMonth

let doMyFilters (universe:InvestmentUniverse) =
    let filtered = 
        universe.Securities
        // my filters expect security, formationMonth
        |> List.map(fun security -> security, universe.FormationMonth)
        // do the filters
        |> List.filter myFilters
        // now convert back from security, formationMonth -> security
        |> List.map fst
    { universe with Securities = filtered }

(**
Define sample months
*)
let startSample = 
    idAndReturnsRows
    |> List.map(fun row -> DateTime(row.Eom.Year,row.Eom.Month,1))
    |> List.min

let endSample = 
    let lastMonthWithData = 
        idAndReturnsRows
        |> Seq.map(fun row -> DateTime(row.Eom.Year,row.Eom.Month,1))
        |> Seq.max
    // The end of sample is the last month when we have returns.
    // So the last month when we can form portfolios is one month
    // before that.
    lastMonthWithData.AddMonths(-1) 

let sampleMonths = getSampleMonths (startSample, endSample)


(**
Strategy function
*)


let formStrategy ym =
    ym
    |> getInvestmentUniverse
    |> doMyFilters
    |> getMySignals
    |> assignSignalSort strategyName 3
    |> List.map (giveValueWeights getMarketCap)
    |> List.map (getPortfolioReturn getSecurityReturn)  

(**
Your strategy portfolios 
*)
let doParallel = true
let portfolios =
    if doParallel then
        sampleMonths
        |> List.toArray
        |> Array.Parallel.map formStrategy
        |> Array.toList
        |> List.collect id
    else
        sampleMonths
        |> List.collect formStrategy

(** A few of the portfolio return observations.*)
portfolios[..2]

(** These portfolios were value-weighted. Can you do a version that is equal-weighted?.*)
let giveEqualWeights (port: AssignedPortfolio): Portfolio =
    let makePosition securityId weight : Position =
        { SecurityId = securityId; Weight = weight }
    
    { FormationMonth = failwith "unimplemented"
      PortfolioId = failwith "unimplemented"
      Positions = failwith "unimplemented" }

(** Now make the equal-weight strategy.*)
(***do-not-eval***)
let formEqualWeightStrategy ym =
    ym
    |> getInvestmentUniverse
    |> doMyFilters
    |> getMySignals
    |> assignSignalSort strategyName 3
    |> List.map giveEqualWeights
    |> List.map (getPortfolioReturn getSecurityReturn)  

let portfoliosEW = sampleMonths |> List.collect formEqualWeightStrategy

(**
## Plotting returns

Common.fsx has some easy to use code to get Fama-French factors.
We're going to use the French data to get monthly risk-free rates.
*)

let ff3 = French.getFF3 Frequency.Monthly
let monthlyRiskFreeRate =
    [ for obs in ff3 do 
        let key = DateTime(obs.Date.Year,obs.Date.Month,1)
        key, obs.Rf ]
    |> Map

(**
Now I'll convert my portfolios into excess returns.
*)

let portfolioExcessReturns =
    portfolios
    |> List.map(fun x -> 
        match Map.tryFind x.YearMonth monthlyRiskFreeRate with 
        | None -> failwith $"Can't find risk-free rate for {x.YearMonth}"
        | Some rf -> { x with Return = x.Return - rf })

(**
### Single portfolio plot
Let's plot the top portfolio, calling it long.
*)

let long = 
    portfolioExcessReturns 
    |> List.filter(fun x -> 
        x.PortfolioId = Indexed {| Name = strategyName; Index = 3 |})
    
let cumulateSimpleReturn (xs: PortfolioReturn list) =
    let accumulator (priorObs:PortfolioReturn) (thisObs:PortfolioReturn) =
        let asOfNow = (1.0 + priorObs.Return)*(1.0 + thisObs.Return) - 1.0
        { thisObs with Return = asOfNow}
    // remember to make sure that your sort order is correct.
    match xs |> List.sortBy(fun x -> x.YearMonth) with
    | [] -> []      // return empty list if the input list is empty
    | head::tail -> // if there are observations do the calculation
        (head, tail) 
        ||> List.scan accumulator

let longCumulative = long |> cumulateSimpleReturn

let longCumulativeChart =
    longCumulative
    |> List.map(fun x -> x.YearMonth, x.Return)
    |> Chart.Line 
    |> Chart.withTitle "Growth of 1 Euro"

(*** condition: ipynb ***)
#if IPYNB
longCumulativeChart
#endif // IPYNB

(*** hide ***)
#if HTML
longCumulativeChart |> GenericChart.toChartHTML
#endif // HTML
(*** include-it-raw ***)

(** And function to do the plot *)
let portfolioReturnPlot (xs:PortfolioReturn list) =
    xs
    |> List.map(fun x -> x.YearMonth, x.Return)
    |> Chart.Line 
    |> Chart.withTitle "Growth of 1 Euro"

(** Using the function: *)
let longWithFunctionsPlot =
    long
    |> cumulateSimpleReturn
    |> portfolioReturnPlot


(*** condition: ipynb ***)
#if IPYNB
longWithFunctionsPlot
#endif // IPYNB

(*** hide ***)
#if HTML
longWithFunctionsPlot |> GenericChart.toChartHTML
#endif // HTML
(*** include-it-raw ***)

(** 
### Multiple portfolio plot

Now let's use the functions to do a bunch of portfolios at once. 

First, let's add a version of value-weighted market portfolio that
has the same time range and same F# type as our portfolios.
*)

let vwMktRf =
    let portfolioMonths = 
        portfolioExcessReturns 
        |> List.map(fun x -> x.YearMonth)
    let minYm = portfolioMonths |> List.min
    let maxYm = portfolioMonths |> List.max
    
    [ for x in ff3 do
        if x.Date >= minYm && x.Date <= maxYm then
            { PortfolioId = Named("Mkt-Rf")
              YearMonth = x.Date
              Return = x.MktRf } ]


(**
Let's also create a long-short portfolio.
*)

let short = 
    portfolioExcessReturns 
    |> List.filter(fun x -> 
        x.PortfolioId = Indexed {| Name = strategyName; Index = 1 |})

let longShort = 
    // We'll loop through the long portfolio observations,
    // looking for the short portfolio observation for that month.
    // For efficiently looking up the short portfolio by month,
    // put it in a Map collection indexed by month.
    let shortByYearMonthMap = 
        short 
        |> List.map(fun row -> row.YearMonth, row) 
        |> Map
    
    [ for longObs in long do
        match Map.tryFind longObs.YearMonth shortByYearMonthMap with
        | None -> failwith "probably your date variables are not aligned for a weird reason"
        | Some shortObs ->
            { PortfolioId = Named "Long-Short"
              YearMonth = longObs.YearMonth
              Return = longObs.Return - shortObs.Return } ] 
  

let combinedChart =
    List.concat [long; longShort; vwMktRf]
    |> List.groupBy(fun x -> x.PortfolioId)
    |> List.map(fun (portId, xs) ->
        xs
        |> cumulateSimpleReturn
        |> portfolioReturnPlot
        |> Chart.withTraceInfo (Name=portId.ToString()))
    |> Chart.combine


(*** condition: ipynb ***)
#if IPYNB
combinedChart
#endif // IPYNB

(*** hide ***)
#if HTML
combinedChart |> GenericChart.toChartHTML
#endif // HTML
(*** include-it-raw ***)

(**
You might also want to save your results to a csv file.
*)

let [<Literal>] OutputSchema =
    "portfolioName(string),index(int option),yearMonth(date),ret(float)"

(*** do-not-eval ***)
type PortfolioReturnCsv = CsvProvider<OutputSchema>

let makePortfolioReturnCsvRow (row:PortfolioReturn) =
    let name, index =
        match row.PortfolioId with
        | Indexed p -> p.Name, Some p.Index
        | Named name -> name, None
    PortfolioReturnCsv
        .Row(portfolioName=name,
             index = index,
             yearMonth=row.YearMonth,
             ret = row.Return)

let csvRows =
    portfolioExcessReturns
    |> List.map makePortfolioReturnCsvRow

let csv = new PortfolioReturnCsv(csvRows)
csv.Save("myExcessReturnPortfolios.csv")
