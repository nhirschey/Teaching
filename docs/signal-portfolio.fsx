(**
---
title: Signal Portfolio
category: Lectures
categoryindex: 1
index: 10
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

*)

(***do-not-eval-file***)
#r "nuget: FSharp.Data, 5.0.2"
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"

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

(**
## Load Data

First, make sure that you're referencing the correct files.

Here I'm assuming that you have a class folder with this `signal-exploration.ipynb` notebook and a `data` folder inside of it. The folder hierarchy would look like below where you
have the below files and folders accessible:

```code
/class
    signal-portfolio.ipynb
    id_and_return_data.csv
    zero_trades_252d.csv
    
```

First, make sure that our working directory is the source file directory.
*)

(***condition:ipynb***)
#if IPYNB
let [<Literal>] ResolutionFolder = __SOURCE_DIRECTORY__
Environment.CurrentDirectory <- ResolutionFolder
#endif // ipynb

(***condition:fsx***)
// These conditions are so that I can store files in the /data folder during developement
#if FSX
let [<Literal>] ResolutionFolder = __SOURCE_DIRECTORY__ + "/data"
Environment.CurrentDirectory <- ResolutionFolder
#endif // fsx

(**
### We will use the portfolio module
We will use the [Portfolio module](https://github.com/nhirschey/NovaSBE.Finance/blob/main/src/NovaSBE.Finance/Portfolio.fs). Make sure that you load this code
*)

#r "nuget: NovaSBE.Finance, 0.4.0"
open NovaSBE.Finance
open NovaSBE.Finance.Portfolio

(**
### Data files
We assume the `id_and_return_data.csv` file and the signal csv file are in the same folder as the notebook. In this example the signal file is `be_me.csv`. You should replace that file name with your signal file name.
*)

let [<Literal>] IdAndReturnsFilePath = "id_and_return_data.csv"
let [<Literal>] MySignalFilePath = "be_me.csv"
let strategyName = "book to market"

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
The data is already filtered to valid securities based on the data documentation
section 1.2, "How to use the data", from the paper the data came from. *)
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
    |> getMySignals
    |> assignSignalSort strategyName 3
    |> Seq.toList
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
      Name = failwith "unimplemented"
      Index = failwith "unimplemented"
      Positions = failwith "unimplemented" }

(** Now make the equal-weight strategy.*)
(***do-not-eval***)
let formEqualWeightStrategy ym =
    ym
    |> getInvestmentUniverse
    |> getMySignals
    |> assignSignalSort strategyName 3
    |> Seq.toList
    |> List.map giveEqualWeights
    |> List.map (getPortfolioReturn getSecurityReturn)  

(**
If you have defined `giveEqualWeights` above then 
you can calculate equal weight portfolios with

```fsharp
let portfoliosEW = sampleMonths |> List.collect formEqualWeightStrategy
```
*)

(***hide***)
let portfoliosEW =
    try  
        sampleMonths |> List.collect formEqualWeightStrategy
    with 
        | _ -> printfn "Error: you probably need to define giveEqualWeights"; []

(**
## Plotting returns

Common.fsx has some easy to use code to get Fama-French factors.
We're going to use the French data to get monthly risk-free rates.
*)
open NovaSBE.Finance.French

let ff3 = getFF3 Frequency.Monthly
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
        match Map.tryFind x.Month monthlyRiskFreeRate with 
        | None -> failwith $"Can't find risk-free rate for {x.Month}"
        | Some rf -> { x with Return = x.Return - rf })

(**
### Single portfolio plot
Let's plot the top portfolio, calling it long.
*)

let long = 
    portfolioExcessReturns 
    |> List.filter(fun x -> x.Index = 3)

let cumulateSimpleReturn (xs: PortfolioReturn list) =
    let xs = xs |> List.sortBy (fun x -> x.Month)
    let mutable cr = 1.0
    [ for x in xs do 
        cr <- cr * (1.0 + x.Return)
        { x with Return = cr - 1.0 } ]

let longCumulative = long |> cumulateSimpleReturn

let longCumulativeChart =
    longCumulative
    |> List.map(fun x -> x.Month, x.Return)
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
    |> List.map(fun x -> x.Month, x.Return)
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
        |> List.map(fun x -> x.Month)
    let minYm = portfolioMonths |> List.min
    let maxYm = portfolioMonths |> List.max
    
    [ for x in ff3 do
        if x.Date >= minYm && x.Date <= maxYm then
            { Name = "Mkt-Rf"
              Index = 1
              Month = x.Date
              Return = x.MktRf } ]


(**
Let's also create a long-short portfolio.
*)

let short = 
    portfolioExcessReturns 
    |> List.filter(fun x -> 
        x.Name = strategyName && x.Index = 1)

let longShort = 
    // We'll loop through the long portfolio observations,
    // looking for the short portfolio observation for that month.
    // For efficiently looking up the short portfolio by month,
    // put it in a Map collection indexed by month.
    let shortByYearMonthMap = 
        short 
        |> List.map(fun row -> row.Month, row) 
        |> Map
    
    [ for longObs in long do
        match Map.tryFind longObs.Month shortByYearMonthMap with
        | None -> failwith "probably your date variables are not aligned for a weird reason"
        | Some shortObs ->
            { Name = "Long-Short"
              Index = 1
              Month = longObs.Month
              Return = longObs.Return - shortObs.Return } ] 
  

let combinedChart =
    List.concat [long; longShort; vwMktRf]
    |> List.groupBy(fun x -> x.Name, x.Index)
    |> List.map(fun ((name, index), xs) ->
        xs
        |> cumulateSimpleReturn
        |> portfolioReturnPlot
        |> Chart.withTraceInfo (Name=($"{name}: {index}")))
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
    "Name(string),Index(int),Month(date),Ret(float)"

(*** do-not-eval ***)
type PortfolioReturnCsv = CsvProvider<OutputSchema>

let makePortfolioReturnCsvRow (row:PortfolioReturn) =
    PortfolioReturnCsv
        .Row(name=row.Name,
             index = row.Index,
             month=row.Month,
             ret = row.Return)

let csvRows =
    portfolioExcessReturns
    |> List.map makePortfolioReturnCsvRow

let csv = new PortfolioReturnCsv(csvRows)
csv.Save("myExcessReturnPortfolios.csv")
