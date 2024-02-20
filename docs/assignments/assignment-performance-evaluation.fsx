(**
---
title: Performance Evaluation
category: Assignments
categoryindex: 2
index: 7
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
(*** condition: ipynb ***)
#r "nuget: Plotly.NET.Interactive, 3.*"

(** *)

open System
open FSharp.Data
open Plotly.NET
open FSharp.Stats
open MathNet.Numerics.Statistics
open NovaSBE.Finance.Ols

(*** condition: fsx ***)
#if FSX
fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))
fsi.AddPrinter<YearMonth>(fun ym -> $"{ym.Year}-{ym.Month}")
#endif // FSX

(**
## Load Data

First, make sure that you're referencing the correct files.

Here I'm assuming that you have a class folder with this notebook and a `data` folder inside of it. The folder hierarchy would look like below where you
have the below files and folders accessible:

```code
/class
    notebook.ipynb
    id_and_return_data.csv
    rd_sale.csv
    
```
*)

open NovaSBE.Finance.Portfolio

(**
### Data files
We assume the `id_and_return_data.csv` file and the signal csv file  are in the notebook folder. 
*)

let [<Literal>] IdAndReturnsFilePath = "id_and_return_data.csv"
let [<Literal>] MySignalFilePath = "rd_sale.csv"
let strategyName = "R&D to sales"

(**
If my paths are correct, then this code should read the first few lines of the files.
If it doesn't show the first few lines, fix the above file paths.
*)

IO.File.ReadLines(IdAndReturnsFilePath) 
|> Seq.truncate 5
|> Seq.iter (printfn "%A")
(** *)
IO.File.ReadLines(MySignalFilePath) 
|> Seq.truncate 5
|> Seq.iter (printfn "%A")
(** *)

(** Ok, now assuming those paths were correct the below code will work. 
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
### Value Weighted Portfolios
*)

let vw = backtest.strategyValueWeighted(myMktCaps)

vw.Portfolios[..3]

(** *)
vw.Returns[..3]

(**
### Long-short portfolios

We get the Fama-French 3-Factor asset pricing model data.
*)

open NovaSBE.Finance

let ff3Lookup = 
    French.getFF3 French.Frequency.Monthly
    |> Array.map (fun x -> DateTime(x.Date.Year, x.Date.Month, 1), x)
    |> Map

(** Isolate some notable portfolios.*)
type SignalPortfolioObs = 
    { Month: DateTime
      Name: string
      ExcessReturn: float }
let long =
    vw.Returns
    |> List.filter (fun row -> row.Index = 3)
    |> List.map (fun row ->
        let retx = row.Return - ff3Lookup[row.Month].Rf 
        { Month = row.Month
          Name = "Long"
          ExcessReturn = retx })

let short =
    vw.Returns
    |> List.filter (fun row -> row.Index = 1)
    |> List.map (fun row -> 
        let retx = row.Return - ff3Lookup[row.Month].Rf 
        { Month = row.Month
          Name = "Short"
          ExcessReturn = retx })

let longShort =
    vw.Returns
    |> List.groupBy (fun x -> x.Month)
    |> List.map (fun (month, xs) ->
        let long = xs |> List.find (fun x -> x.Index = 3)
        let short = xs |> List.find (fun x -> x.Index = 1)
        { Month = long.Month
          Name = "Long-short"
          ExcessReturn = long.Return - short.Return })

(**
## Start of assignment

> **Task:** Calculate the annualized Sharpe ratios of your long, short, and long-short portfolios. Assign them to values `longSharpe`, `shortSharpe`, and `longShortSharpe`, respectively.
*)
// Solution here.

(***hide***)
let sharpe (xs: float list) =
    let mn = xs |> List.average
    let sd = xs |> stDev
    (sqrt 12.0) * (mn / sd)

let longSharpe = long |> List.map (fun x -> x.ExcessReturn) |> sharpe
let shortSharpe = short |> List.map (fun x -> x.ExcessReturn) |> sharpe
let longShortSharpe = longShort |> List.map (fun x -> x.ExcessReturn) |> sharpe

(** Tests *)
longSharpe |> should (equalWithin 1e-6) 0.2613490337
shortSharpe |> should (equalWithin 1e-6) 0.490118588
longShortSharpe |> should (equalWithin 1e-6) -0.05676492997

(**
> **Task:** Create a `list<RegData>` for your  long-short portfolio. Assign it to a value named `longShortRd`.
*)
type RegData =
    { Month: DateTime
      ExcessReturn: float 
      MktRf: float
      Hml: float
      Smb: float }

// Solution here.

(***hide***)
let longShortRd =
    longShort
    |> List.map (fun x -> 
        { Month = x.Month
          ExcessReturn = x.ExcessReturn
          MktRf = ff3Lookup[x.Month].MktRf
          Hml = ff3Lookup[x.Month].Hml
          Smb = ff3Lookup[x.Month].Smb })

(** Tests *)
longShortRd |> shouldHaveLength 252
longShortRd |> should be ofExactType<RegData list>

(**
> **Task:** Fit CAPM and Fama-French 3-factor models for your long-short portfolio. Assign them to a values named `capmModel` and `ff3Model`, respectively.
*)
// Solution here.

(***hide***)
let capmModel = Ols("ExcessReturn ~ MktRf", longShortRd).fit()
let ff3Model = Ols("ExcessReturn ~ MktRf + Hml + Smb", longShortRd).fit()

(** Tests.*)
capmModel |> should be ofExactType<RegressionResults>
ff3Model |> should be ofExactType<RegressionResults>

(**
### CAPM model evaluation.
*)

(**
> **Task:** What is the CAPM alpha for your long-short portfolio? Use code to assign the alpha to the value `capmAlpha`. Is it significantly different from zero? Use code to assign the t-statistic to the value `capmAlphaT`. 
*)
// Solution here.

(***hide***)
let capmAlpha = capmModel.coefs["Intercept"]
let capmAlphT = capmModel.tvalues["Intercept"]

(** Tests *)
capmAlpha |> should (equalWithin 1e-6) -0.003015844638
capmAlphT |> should (equalWithin 1e-6) -1.011538156

(**
> **Task:** What is the CAPM beta for your long-short portfolio? Use code to assign the beta to the value `capmBeta`. Is it significantly different from zero? Use code to assign the t-statistic to the value `capmBetaT`.
*)
// Solution here.

(***hide***)
let capmBeta = capmModel.coefs["MktRf"]
let capmBetaT = capmModel.tvalues["MktRf"]

(** Tests *)
capmBeta |> should (equalWithin 1e-6) 0.3874415756
capmBetaT |> should (equalWithin 1e-6) 5.957967542

(**
> **Task:** What is the information ratio for your long-short portfolio when using the CAPM model? Assign it to a value named `capmIR`.
*)
// Solution here.

(***hide***)
let capmIR = capmAlpha / (capmModel.resid |> stDev)

(** Tests *)
capmIR |> should (equalWithin 1e-6) -0.06434136117

(**
### Fama-French 3-factor model evaluation.
*)

(**
> **Task:** What is the Fama-French 3-factor model alpha for your long-short portfolio. Is it significantly different from zero? 
*)
// Solution here.

(**
> **Task:** What are the betas on the Market, HML, and SMB factors for your long-short portfolio. Are they significantly different from zero? 
*)
// Solution here.

(**
> **Task:** Based on the Market, HML, and SMB factor betas for your long-short portfolio, would you say your portfolio is more like a value portfolio, more like a growth portfolio, or neither? Explain.
*)
// Solution here.

(**
> **Task:** Based on the Market, HML, and SMB factor betas for your long-short portfolio, would you say your portfolio is more like a small-cap portfolio, more like a large-cap portfolio, or neither? Explain.
*)
// Solution here.


(**
> **Task:** What is the information ratio for your long-short portfolio when using the Fama and French 3-factor model? 
*)
// Solution here.

