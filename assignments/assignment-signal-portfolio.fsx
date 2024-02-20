(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=assignments/assignment-signal-portfolio.ipynb)&emsp;
[![Script](../img/badge-script.svg)](/Teaching//assignments/assignment-signal-portfolio.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](/Teaching//assignments/assignment-signal-portfolio.ipynb)

Group Name:

Student Name | Student Number
--- | ---
**1:** | &#32;
**2:** | &#32;
**3:** | &#32;
**4:** | &#32;
**5:** | &#32;


This is an assignment. You should work in groups. Please write your group and group member names above. You will find sections labeled **Task** asking you to do each piece of analysis. Please make sure that you complete all of these tasks. I included some tests to help you see if you are calculating the solution correctly, but if you cannot get the test to pass submit your best attempt and you may recieve partial credit.

All work that you submit should be your own. Make use of the course resources and example code on the course website. It should be possible to complete all the requested tasks using information given below or somewhere on the course website.

For testing

*)
#r "nuget: FsUnit.Xunit"
#r "nuget: xunit, 2.*"
open Xunit
open FsUnit.Xunit
open FsUnitTyped
(**
For the assignment

*)
#r "nuget: FSharp.Data, 5.0.2"
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: NovaSBE.Finance, 0.5.0"
#r "nuget: MathNet.Numerics"
#r "nuget: MathNet.Numerics.FSharp"
#r "nuget: Plotly.NET, 3.*"
open System
open FSharp.Data
open Plotly.NET
open FSharp.Stats
open MathNet.Numerics.Statistics
fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))
fsi.AddPrinter<YearMonth>(fun ym -> $"{ym.Year}-{ym.Month}")
(**
## Load Data

First, make sure that you're referencing the correct files.

Here I'm assuming that you have a class folder with this notebook and a `data` folder inside of it. The folder hierarchy would look like below where you
have the below files and folders accessible:

```code
/class
    notebook.ipynb
    id_and_return_data.csv
    be_me.csv
    
```
*)
open NovaSBE.Finance.Portfolio
(**
### Data files

We assume the `id_and_return_data.csv` file and the signal csv file  are in the `data` folder. In this example the signal file is `be_me.csv`.

*)
let [<Literal>] IdAndReturnsFilePath = "id_and_return_data.csv"
let [<Literal>] MySignalFilePath = "be_me.csv"
let strategyName = "book-to-market"
(**
If my paths are correct, then this code should read the first few lines of the files.
If it doesn't show the first few lines, fix the above file paths.

*)
IO.File.ReadLines(IdAndReturnsFilePath) 
|> Seq.truncate 5
|> Seq.iter (printfn "%A")
IO.File.ReadLines(MySignalFilePath) 
|> Seq.truncate 5
|> Seq.iter (printfn "%A")
(**
Ok, now assuming those paths were correct the below code will work.
I will put all this prep code in one block so that it is easy to run.

*)
let idAndReturnsCsv = 
    CsvProvider<IdAndReturnsFilePath,ResolutionFolder = __SOURCE_DIRECTORY__>.GetSample().Rows 
    |> Seq.toList
let mySignalCsv = 
    CsvProvider<MySignalFilePath,ResolutionFolder = __SOURCE_DIRECTORY__>.GetSample().Rows 
    |> Seq.toList
(**
A list of `Signal` records. The signal type is defined in the `NovaSBE.Finance.Portfolio` module [here](https://github.com/nhirschey/NovaSBE.Finance/blob/6d1398625e5a9279af00bb6e1c1802af3596c3f6/src/NovaSBE.Finance/Portfolio.fs#L178-L181).

*)
let mySignals =
    mySignalCsv
    |> List.choose (fun row -> 
        match row.Signal with
        | None -> None
        | Some signal ->
            let signalRecord: Signal =
                { SecurityId = Other row.Id
                  FormationDate = DateTime(row.Eom.Year, row.Eom.Month, 1)
                  Signal = signal }
            Some signalRecord)

// look at a few signals
mySignals[..3]
(**
A list of Security return records. The `SecurityReturn` type is defined in the `NovaSBE.Finance.Portfolio` module [here](https://github.com/nhirschey/NovaSBE.Finance/blob/6d1398625e5a9279af00bb6e1c1802af3596c3f6/src/NovaSBE.Finance/Portfolio.fs#L173-L176)

*)
let myReturns =
    idAndReturnsCsv
    |> List.choose (fun row -> 
        match row.Ret with
        | None -> None
        | Some ret ->
            let ret: SecurityReturn =
                { SecurityId = Other row.Id
                  Date = DateTime(row.Eom.Year, row.Eom.Month, 1)
                  Return= ret }
            Some ret)

// look at a few returns
myReturns[..3]
(**
A list of security market caps. We'll need this for value-weight portfolios. The `WeightVariable` type is defined in the `NovaSBE.Finance.Portfolio` module [here](https://github.com/nhirschey/NovaSBE.Finance/blob/6d1398625e5a9279af00bb6e1c1802af3596c3f6/src/NovaSBE.Finance/Portfolio.fs#L183-L186).

*)
let myMktCaps =
    idAndReturnsCsv
    |> List.choose (fun row -> 
        match row.MarketEquity with
        | None -> None
        | Some mktCap ->
            let mktCap: WeightVariable =
                { SecurityId = Other row.Id
                  FormationDate = DateTime(row.Eom.Year, row.Eom.Month, 1)
                  Value = mktCap }
            Some mktCap)

// look at a few market caps
myMktCaps[..3]
(**
## Forming our strategy

We're now going to use the `Backtest` code to generate portfolios. It is defined in the `NovaSBE.Finance.Portfolio` module [here](https://github.com/nhirschey/NovaSBE.Finance/blob/6d1398625e5a9279af00bb6e1c1802af3596c3f6/src/NovaSBE.Finance/Portfolio.fs#L199).
The `Backtest` class automates some of the code we did earlier to make portfolio construction simpler.

*)
let backtest = Backtest(returns=myReturns, signals=mySignals, nPortfolios=3, name = strategyName)
(**
### Equal Weighted Portfolios

*)
let ew = backtest.strategyEqualWeighted()
(**
Some portfolios with their positions.

*)
ew.Portfolios[..3]
(**
Some portfolio returns.

*)
ew.Returns[..3]
(**
### Value Weighted Portfolios

*)
let vw = backtest.strategyValueWeighted(myMktCaps)

vw.Portfolios[..3]
vw.Returns[..3]
(**
### Plot of value-weight returns

*)
let cumulativeReturn xs =
    let sorted = xs |> List.sortBy (fun (dt, r) -> dt)
    let mutable cr = 1.0
    [ for (dt, r) in sorted do 
        cr <- cr * (1.0 + r)
        (dt, cr - 1.0) ]

let makeCumulativeChart (returns:List<PortfolioReturn>) =
    let firstObs = returns[0]
    returns
    |> List.map (fun x -> x.Month, x.Return)
    |> cumulativeReturn
    |> Chart.Line
    |> Chart.withTraceInfo(Name = $"{firstObs.Name}: {firstObs.Index}")

vw.Returns
|> List.filter (fun x -> x.Index = 1)
|> makeCumulativeChart
(**
All the ports.

*)
vw.Returns
|> List.groupBy (fun x -> x.Index)
|> List.map (fun (idx, xs) -> makeCumulativeChart xs)
|> Chart.combine
(**
## Start of assignment

> **Task:** How many stocks are in the tercile 3 portfolio held during September 2017? Assign the result to a value named `nStocksSept2017`. Remember that this portfolio
was formed at the end of August 2017.
> 

*)
// Solution here
(**
Tests.

*)
nStocksSept2017 |> should equal 1282
(**
> **Task:** What is the minimum and maximum weight of a stock in the tercile 3 portfolio held during September 2017? Do it for both the value and equal weight portfolios.
Assign the results to values named `vwMinSept2017`, `vwMaxSept2017`, `ewMinSept2017`, `ewMaxSept2017`.
> 

*)
// Solution here
(**
Tests

*)
let tol = 1e-6
vwMinSept2017 |> should (equalWithin tol)  1.134675008e-07
vwMaxSept2017 |> should (equalWithin tol)  0.06467652288
ewMinSept2017 |> should (equalWithin tol)  0.0007800312012
ewMaxSept2017 |> should (equalWithin tol)  0.0007800312012
(**
> **Task:** Plot a histogram of the Sept 2017 (formed August 2017) position weights for the stocks in the value weight tercile 3.
> 

> **Task:** Calculate the total weight put in quintile 3's top 10 positions in Sept 2017 (formed August 2017) when using value weights. Assign it to a value named `topWeightsSept2017`.
> 

*)
// Solution here
(**
Tests

*)
topWeightsSept2017 |> should (equalWithin tol) 0.3619460225
(**
> **Task:** Write a function that takes a `Portfolio` as it's input and outputs a tuple of the formaiton date and the sum of the top 10 position weights. I have type hints to constrain the function type.
> 

*)
// Solution here
let calcTop10Weights (p:Portfolio) : DateTime * float =
    failwith "unimplimented"
(**
tests

*)
// Portfolio with 10 test positions
let testPortfolio =
    { FormationMonth = DateTime(1999,1,1)
      Name = "test"
      Index = 1
      Positions = [ for i in 1..20 do { SecurityId = Other "test"; Weight = 1./20.} ] }

let testPortfolioDate, testPortfolioWeight = testPortfolio |> calcTop10Weights

testPortfolioDate |> should equal (DateTime(1999,1,1))
testPortfolioWeight |> should (equalWithin tol) 0.5
(**
> **Task:** Using the value-weight strategy, calculate the total weight put in quintile 3's top 10 positions every month. Assign it to a value named `topWeights` that has type `list<DateTime * float>` where the first thing in the tuple is the formation month and the second thing is the sum of the top 10 position weights.
> 

*)
// Solution here
(**
tests

*)
topWeights |> shouldHaveLength 252
topWeights |> should be ofExactType<list<DateTime * float>>
topWeights
|> List.averageBy (fun (dt, w) -> w)
|> should (equalWithin tol) 0.3174428516
(**
> **Task:** Plot a line chart of `topWeights` that shows how the top 10 weights evolves over the sample period.
> 

*)

