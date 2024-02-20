(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=stocksandbonds.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//stocksandbonds.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//stocksandbonds.ipynb)

# Stocks for the long run, this time with both stocks and bonds

*)
#r "nuget: ExcelProvider, 2.0.0"
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"
#r "nuget: NovaSBE.Finance, 0.5.0"


open FSharp.Interop.Excel
open System
open FSharp.Stats
open Plotly.NET
open NovaSBE.Finance.Utils

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
(**
Download the excel file from Robert Shiller's website to your current directory.

*)
download "http://www.econ.yale.edu/~shiller/data/ie_data.xls" "shiller_data.xls"
(**
Now loading the excel data.

*)
let [<Literal>] shillerFile = __SOURCE_DIRECTORY__ + "/shiller_data.xls"
type ShillerXls = ExcelFile<shillerFile,SheetName="Data",Range="A8:V2000",ForceString = true>

let shiller = 
    ShillerXls().Data |> Seq.toList
(**
Let's look at the dates.

*)
shiller[..11] |> List.map (fun x -> x.Date)
(**
Remember that is the same as

*)
[ for x in shiller[..11] do x.Date ]
(**
Function to parse the dates.

*)
let parseDate (x: ShillerXls.Row) =
    let year = int x.Date[..3]
    let month = 
        let m = x.Date[5..]
        if m = "1" then 10 else int m
    DateTime(year, month, 1)
(**
Parse the first few dates.

*)
shiller[..11] 
|> List.map (fun x -> x.Date, parseDate x)
(**
Check that we're getting the right data.

*)
shiller[1823..1825]
(**
Some specific columns.

*)
shiller[1823..1825]
|> List.map (fun x -> 
    x.Date, x.D, x.E)
(**
Take the data until I get to those bad rows.

*)
let shillerClean =
    shiller
    |> List.takeWhile (fun x -> 
        not (isNull x.Date) &&
        not (isNull x.D) &&
        not (isNull x.E))
(**
The S&amp;P500 total (price + dividend) return index is the Price2 column.
To calculate returns from it we want consecutive observations.

*)
let x0 = shillerClean[0]
let x1 = shillerClean[1]

x0.Price2, x1.Price2
(**
We can do this for the whole list using `List.pairwise`

*)
shillerClean
|> List.pairwise
|> List.map (fun (x0, x1) -> x0.Price2, x1.Price2)
|> List.take 3
(**
We'll calculate a log return.

*)
let logReturn (p0, p1) = log (p1 / p0)

logReturn (float x0.Price2, float x1.Price2)
(**
Now for the whole list.

*)
shillerClean[0..5] 
|> List.map (fun x -> float x.Price2)
|> List.pairwise
|> List.map logReturn
(**
A type to hold return data.

*)
type ShillerObs =
    {
        Date: DateTime
        /// S&P 500 log return
        SP500RealReturn: float
        /// 10-year US Treasury log return
        GS10RealReturn: float
        CAPE: float
    }
(**
Function to make the return data.

*)
let makeShillerObs (x0: ShillerXls.Row, x1: ShillerXls.Row) =
    {
        Date = parseDate x0
        SP500RealReturn = logReturn (float x0.Price2, float x1.Price2)
        GS10RealReturn = logReturn (float x0.Returns2, float x1.Returns2)
        CAPE = if x0.CAPE = "NA" then nan else float x0.CAPE
    }
(**
Making our list of records containing stock and bond returns.

*)
let shillerObs =
    shillerClean
    |> List.pairwise
    |> List.map makeShillerObs
(**
Average annualized stock and bond returns

*)
let avgStock = shillerObs |> List.averageBy (fun x -> x.SP500RealReturn * 12.0)
let avgBond = shillerObs |> List.averageBy (fun x -> x.GS10RealReturn * 12.0)
let port6040 = shillerObs |> List.map (fun x -> x.SP500RealReturn * 0.6 + x.GS10RealReturn * 0.4)
let avg6040 = port6040 |> List.averageBy (fun x -> x * 12.0)

let sdStock = shillerObs |> Seq.stDevBy _.SP500RealReturn |> fun x ->  x * sqrt 12.0 
let sdBond = shillerObs |> Seq.stDevBy _.GS10RealReturn |> fun x ->  x * sqrt 12.0
let sd6040 = port6040 |> Seq.stDev |> fun x ->  x * sqrt 12.0 

let lev6040 = sdStock / sd6040
/// this is a cost above the risk-free rate
let costOfLeverage = 0.01
let lev6040Return = lev6040 * avg6040 - max 0.0 ((lev6040 - 1.0) * costOfLeverage)
(**
Standard deviation of stock and bond returns

Let's look at returns by decade.

*)
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
(**
Plot of stock return by decade

*)
stockByDecade
|> Chart.Column(* output: 
<null>*)
(**
Now the same thing for bonds.

*)
let bondByDecade =
    shillerObs
    |> List.groupBy (fun x -> dateToDecade x.Date)
    |> List.map (fun (decade, obs) ->
        let decadeReturn = obs |> List.map (fun x -> x.GS10RealReturn) |> List.sum
        decade, decadeReturn)

bondByDecade
|> Chart.Column
(**
Combine them.

*)
[ Chart.Column(stockByDecade, Name = "Stocks")
  Chart.Column(bondByDecade, Name = "Bonds")]
|> Chart.combine
(**
Let's make a cumulative chart of stock returns.

*)
let accStockRet =
    let mutable accRet = 0.0
    [ for x in shillerObs do 
        accRet <- accRet + x.SP500RealReturn 
        x.Date, accRet ]

accStockRet[..5]
(**
A line chart of it

*)
accStockRet
|> Chart.Line
(**
If we wanted to see how 1 EUR would grow, remember that we have to
plot $e^r$.

*)
[ for (date, ret) in accStockRet do date, exp ret]
|> Chart.Line
(**
> Practice: Plot a line chart of cumulative **log** returns for bonds.
> 

Let's revisit our simulations, this time with stock and bond returns.

Grabbing stock and bond returns.

*)
let stockReturns = shillerObs |> List.map (fun x -> x.SP500RealReturn)
let bondReturns = shillerObs |> List.map (fun x -> x.GS10RealReturn)
(**
We need a covariance matrix of stock/bond returns to sample both from a multivariate normal distribution.

*)
let covMatrix = 
    [[ var stockReturns            ; cov stockReturns bondReturns ]
     [ cov stockReturns bondReturns; var bondReturns              ]]
    |> matrix
(**
Create a vector of average returns.

*)
let avgStockReturn = stockReturns |> List.average
let avgBondReturn = bondReturns |> List.average
let avgReturns = [ avgStockReturn; avgBondReturn] |> vector
(**
Annualize returns and covariances so that we sample annual values.

*)
let annualizedCov = covMatrix * 12.0
let annualizedRet = avgReturns * 12.0
(**
Our sampler.

*)
let rmultinorm = 
    Distributions.Continuous.MultivariateNormal.Init annualizedRet annualizedCov
(**
Try a sample.

*)
let s = rmultinorm.Sample()
(**
Stock return

*)
s[0]
(**
Bond return

*)
s[1]
(**
1k draws of 30 year investment returns.

*)
type MarketDraw = { StockReturn: float; BondReturn: float}

let stockBondDraws =
    [ for i in [1..1000] do
        [ for  y in [1..30] do
            let s = rmultinorm.Sample()
            { StockReturn = s[0]; BondReturn = s[1]} ]]

let firstDraw = stockBondDraws[0]
(**
Our wealth evolution setup.

*)
let expenses = 50_000.0
let initialWealth = 1_000_000.0
(**
We accumulate wealth from log returns this time.

*)
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
(**
Chance of going broke with stocks?

*)
let nBrokeStock =
    stockTerminalWealth
    |> List.filter (fun x -> x <= 0.0)
    |> List.length
    |> float

let chanceBrokeStock = nBrokeStock / (float stockTerminalWealth.Length)    

printfn $"chance broke=%.3f{chanceBrokeStock}"
(**
Same thing for bonds.

*)
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
(**
Chance of going broke with stocks?

*)
let nBrokeBond =
    bondTerminalWealth
    |> List.filter (fun x -> x <= 0.0)
    |> List.length
    |> float

let chanceBrokeBond = nBrokeBond / (float bondTerminalWealth.Length)    

printfn $"chance broke=%.3f{chanceBrokeBond}"
(**
Rather than repeating code, a function to do all that.

*)
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
(**
Try it for stocks.

*)
let stockOnlyLives = 
    [ for life in stockBondDraws do
        [ for r in life do r.StockReturn ]]

calcChanceBroke 50_000 1_000_000 stockOnlyLives
(**
Some different bond/stock ratios.

*)
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

