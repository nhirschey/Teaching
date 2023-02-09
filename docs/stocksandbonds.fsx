(**
---
title: Stocks and bonds
category: Lectures
categoryindex: 1
index: 3
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

# Stocks for the long run, this time with both stocks and bonds

*)

#r "nuget: ExcelProvider, 2.0.0"
#r "nuget: FSharp.Stats"
#r "nuget: Plotly.NET"
#r "nuget: Plotly.NET.Interactive"


open FSharp.Interop.Excel
open System
open System.Net

open FSharp.Stats
open Plotly.NET

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

(** 
A function to download a file. Don't worry about the specifics of the code in this function. 
*)

let download (inputUrl: string) (outputFile: string) =
    if IO.File.Exists(outputFile) then
        printfn $"The file {outputFile} already exists. Skipping download" 
    else
        use fileStream = IO.File.Create(outputFile)
        use http = new Http.HttpClient()
        use urlStream = http.GetStreamAsync(inputUrl).Result
        urlStream.CopyTo(fileStream)

(** Download the excel file from Robert Shiller's website to your current directory. *)
download "http://www.econ.yale.edu/~shiller/data/ie_data.xls" "shiller_data.xls"


let [<Literal>] shillerFile = __SOURCE_DIRECTORY__ + "/shiller_data.xls"
type ShillerXls = ExcelFile<shillerFile,SheetName="Data",Range="A8:V2000",ForceString = true>

let shiller = 
    ShillerXls().Data |> Seq.toList

(** Let's look at the dates. *)
shiller[..7] |> List.map (fun x -> x.Date)

(** Remember that is the same as *)
[ for x in shiller[..7] do x.Date ]

(** Function to parse the dates. *)
let parseDate (x: ShillerXls.Row) =
    let year = int x.Date[..3]
    let month = 
        let m = x.Date[5..]
        if m = "1" then 10 else int m
    DateTime(year, month, 1)

(** Parse the first few dates. *)
shiller[..7] 
|> List.map (fun x -> x.Date, parseDate x)

(** Check that we're getting the right data. *)

shiller[1823..1825]

(** Some specific columns. *)
shiller[1823..1825]
|> List.map (fun x -> 
    x.Date, x.D, x.E)

(** Take the data until I get to those bad rows.*)

let shillerClean =
    shiller
    |> List.takeWhile (fun x -> 
        not (isNull x.Date) &&
        not (isNull x.D) &&
        not (isNull x.E))


(** 
The S&P500 total (price + dividend) return index is the Price2 column.
To calculate returns from it, we want to do $p1/p0$. We can do this using
`List.pairwise`.
*)

[ 1..10] |> List.pairwise

(**
With the price index ...
*)
shillerClean[0..5] 
|> List.map (fun x -> x.Price2)
|> List.pairwise

(** We'll calculate a log return. *)
shillerClean[0..5] 
|> List.map (fun x -> x.Price2)
|> List.pairwise
|> List.map (fun (x0, x1) -> log (float x1 / float x0))

(** A type to hold return data.*)
type ShillerObs =
    {
        Date: DateTime
        /// S&P 500 log return
        SP500RealReturn: float
        /// 10-year US Treasury log return
        GS10RealReturn: float
        CAPE: float
    }

(** Making our list of records containing stock and bond returns.*)
let shillerObs =
    shillerClean
    |> List.pairwise
    |> List.map (fun (x0, x1) -> 
        {
            Date = parseDate x1
            SP500RealReturn = log ((float x1.Price2)/(float x0.Price2))
            GS10RealReturn = log ((float x1.Returns2)/(float x0.Returns2))
            CAPE = if x1.CAPE = "NA" then nan else float x1.CAPE
        })

(** Let's look at returns by decade. *)
let dateToDecade (date: DateTime) = floor (float date.Year / 10.0) * 10.0

[ for i in [1..10] do 
    let y = DateTime(2005,1,1).AddYears(i) 
    y, dateToDecade y ]


(**
Starting with stocks, remember how group by works
*)

[ ("a", 1); ("a", 2); ("b", 3)]
|> List.groupBy (fun (x, y) -> x)

(**
Now with the stock data.
*)
shillerObs
|> List.groupBy (fun x -> dateToDecade x.Date)

(**
Return by decade
*)
let stockByDecade =
    shillerObs
    |> List.groupBy (fun x -> dateToDecade x.Date)
    |> List.map (fun (decade, obs) ->
        let decadeReturn = obs |> List.map (fun x -> x.SP500RealReturn) |> List.sum
        decade, decadeReturn)

(** Plot of stock return by decade*)
stockByDecade
|> Chart.Column

(** Now the same thing for bonds.*)
let bondByDecade =
    shillerObs
    |> List.groupBy (fun x -> dateToDecade x.Date)
    |> List.map (fun (decade, obs) ->
        let decadeReturn = obs |> List.map (fun x -> x.GS10RealReturn) |> List.sum
        decade, decadeReturn)

bondByDecade
|> Chart.Column

(** Combine them.*)
(***do-not-eval***)
[ Chart.Column(stockByDecade, Name = "Stocks")
  Chart.Column(bondByDecade, Name = "Bonds")]
|> Chart.combine
(***hide***)
[ Chart.Column(stockByDecade, Name = "Stocks")
  Chart.Column(bondByDecade, Name = "Bonds")]
|> Chart.combine
|> GenericChart.toChartHTML


(**
Let's make a cumulative chart of stock returns.
*)
let accStockRet =
    let mutable accRet = 0.0
    [ for x in shillerObs do 
        accRet <- accRet + x.SP500RealReturn 
        x.Date, accRet ]

accStockRet[..5]

(** A line chart of it*)
accStockRet
|> Chart.Line

(** If we wanted to see how 1 EUR would grow, remember that we have to
plot $e^r$.
*)
[ for (date, ret) in accStockRet do date, exp ret]
|> Chart.Line

(**
> Practice: Plot a line chart of cumulative *log* returns for bonds.
*)

(** Let's revisit our simulations, this time with stock and bond returns.

Grabbing stock and bond returns.
*)
let stockReturns = shillerObs |> List.map (fun x -> x.SP500RealReturn)
let bondReturns = shillerObs |> List.map (fun x -> x.GS10RealReturn)

(** We need a covariance matrix of stock/bond returns to sample both from a multivariate normal distribution.*)
let covMatrix = 
    [[ var stockReturns            ; cov stockReturns bondReturns ]
     [ cov stockReturns bondReturns; var bondReturns              ]]
    |> matrix

(** Create a vector of average returns. *)
let avgStockReturn = stockReturns |> List.average
let avgBondReturn = bondReturns |> List.average
let avgReturns = [ avgStockReturn; avgBondReturn] |> vector

(** Annualize returns and covariances so that we sample annual values.*)
let annualizedCov = covMatrix * 12.0
let annualizedRet = avgReturns * 12.0

(** Our sampler.*)
let rmultinorm = 
    Distributions.ContinuousDistribution.multivariateNormal annualizedRet annualizedCov

(** Try a sample. *)
let s = rmultinorm.Sample()
(** Stock return*)
s[0]
(** Bond return *)
s[1]

(** 1k draws of 30 year investment returns. *)
type MarketDraw = { StockReturn: float; BondReturn: float}

let stockBondDraws =
    [ for i in [1..1000] do
        [ for  y in [1..30] do
            let s = rmultinorm.Sample()
            { StockReturn = s[0]; BondReturn = s[1]} ]]

let firstDraw = stockBondDraws[0]

(** Our wealth evolution setup. *)
let expenses = 50_000.0
let initialWealth = 1_000_000.0

(** We accumulate wealth from log returns this time. *)
let stockWealthEvolution =
    [ for life in stockBondDraws do
        let mutable wealth = initialWealth
        [ for r in life do
            // We'll take expenses out at the start of the year.
            if wealth > expenses then
                wealth <- (wealth - expenses) * exp r.StockReturn
            else
                wealth <- 0.0
            wealth ] ]

let stockTerminalWealth = [ for x in stockWealthEvolution do x[x.Length-1] ]

(** Chance of going broke with stocks? *)
let nBrokeStock =
    stockTerminalWealth
    |> List.filter (fun x -> x <= 0.0)
    |> List.length
    |> float

let chanceBrokeStock = nBrokeStock / (float stockTerminalWealth.Length)    

printfn $"chance broke=%.3f{chanceBrokeStock}"

(** Same thing for bonds.*)
let bondWealthEvolution =
    [ for life in stockBondDraws do
        let mutable wealth = initialWealth
        [ for r in life do
            // We'll take expenses out at the start of the year.
            if wealth > expenses then
                wealth <- (wealth - expenses) * exp r.BondReturn
            else
                wealth <- 0.0
            wealth ] ]

let bondTerminalWealth = [ for x in bondWealthEvolution do x[x.Length-1] ]

(** Chance of going broke with stocks? *)
let nBrokeBond =
    bondTerminalWealth
    |> List.filter (fun x -> x <= 0.0)
    |> List.length
    |> float

let chanceBrokeBond = nBrokeBond / (float bondTerminalWealth.Length)    

printfn $"chance broke=%.3f{chanceBrokeBond}"

(** Rather than repeating code, a function to do all that. *)

let calcChanceBroke expenses initialWealth lives =
    let wealthEvolution =
        [ for life in lives do
            let mutable wealth = initialWealth
            [ for r in life do
                // We'll take expenses out at the start of the year.
                if wealth > expenses then
                    wealth <- (wealth - expenses) * exp r
                else
                    wealth <- 0.0
                wealth ] ]

    let terminalWealth = [ for x in wealthEvolution do x[x.Length-1] ]

    (** Chance of going broke with stocks? *)
    let nBroke =
        terminalWealth
        |> List.filter (fun x -> x <= 0.0)
        |> List.length
        |> float

    let chanceBroke = nBroke / (float terminalWealth.Length)    
    chanceBroke

(** Try it for stocks. *)
let stockOnlyLives = 
    [ for life in stockBondDraws do
        [ for r in life do r.StockReturn ]]

calcChanceBroke 50_000 1_000_000 stockOnlyLives

(** Some different bond/stock ratios.*)
let bondStockRatios = [0.0 .. 0.2 .. 1.0]
let bondStockRatioLives =
    [ for ratio in bondStockRatios do 
        [ for life in stockBondDraws do
            [ for r in life do ratio * r.BondReturn + (1.0 - ratio) * r.StockReturn ]]]

[ for ratioLives in bondStockRatioLives do 
    calcChanceBroke 50_000 1_000_000 ratioLives ]

(**
What are some things to consider?
*)