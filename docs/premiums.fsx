(**
---
title: Premiums
category: Lectures
categoryindex: 1
index: 14
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

# Equity Premium usings CAPE

*)

#r "nuget: NovaSBE.Finance,0.3.*-*"
#r "nuget: FSharp.Stats,0.4.*"
#r "nuget: FSharp.Data,5.*-*"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"
#r "nuget: ExcelProvider,2.*"

open System
open NovaSBE.Finance
open NovaSBE.Finance.Utils
open FSharp.Stats
open Plotly.NET
open Plotly.NET.Interactive
open FSharp.Data
open NovaSBE.Finance.Ols
open FSharp.Interop.Excel

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let goyal2022Id = "1g4LOaRj4TvwJr9RIaA_nwrXXWTOy46bP"
let sheetToCsv id sheet = $"https://docs.google.com/spreadsheets/d/{id}/gviz/tq?tqx=out:csv&sheet={sheet}"

download (sheetToCsv goyal2022Id "Monthly") "goyalMonthly.csv"

(**
Shiller data
*)
(** Download the excel file from Robert Shiller's website to your current directory. *)
download "http://www.econ.yale.edu/~shiller/data/ie_data.xls" "shiller_data.xls"

(** Now loading the excel data.*)

let [<Literal>] shillerFile = __SOURCE_DIRECTORY__ + "/shiller_data.xls"
type ShillerXls = ExcelFile<shillerFile,SheetName="Data",Range="A8:V2000",ForceString = true>

let shillerDate (x: ShillerXls.Row) =
    let year = int x.Date[..3]
    let month = 
        let m = x.Date[5..]
        if m = "1" then 10 else int m
    DateTime(year, month, 1)

type ShillerData = 
    { Month: DateTime
      CAPE: float
      Ret: float }
let shiller = 
    ShillerXls().Data 
    |> Seq.toList
    |> Seq.takeWhile (fun x -> 
        not (isNull x.Date) &&
        not (isNull x.D) && 
        not (isNull x.CAPE)
        )
    |> Seq.pairwise
    |> Seq.map (fun (x0, x1) ->
        let cape = 
            match x1.CAPE with
            | "NA" -> nan
            | cape -> cape |> float
        shillerDate x1,
        { Month = shillerDate x1
          CAPE = cape 
          Ret = (float x1.P + float x1.D / 12.0) / float x0.P - 1.0})
    |> Map     


/// Goyal and Welch Monthly Excel worksheet
let gwXlMd = CsvProvider<"goyalMonthly.csv",ResolutionFolder = __SOURCE_DIRECTORY__>.GetSample().Rows
let parseMonth (yyyyMM) = DateTime.ParseExact(string yyyyMM,"yyyyMM",Globalization.CultureInfo.InvariantCulture)

type Predictors =
    { Month: DateTime
      CAPE: float
      R: float }
let capePredictor=
    gwXlMd
    |> Seq.filter (fun x ->
        x.Yyyymm >= 188102 &&
        x.Yyyymm <= 202303)
    |> Seq.map (fun x ->
        let month = parseMonth x.Yyyymm
        let r =
            if month < DateTime(1927,1,1) then
                shiller[month].Ret - x.Rfree
            else
                x.CRSP_SPvw - x.Rfree
        { Month = month
          CAPE = shiller[month.AddMonths(-1)].CAPE 
          R = r })
    |> Seq.toArray

let crspRet = gwXlMd |> Seq.map (fun x -> parseMonth x.Yyyymm, x) |> Map

type ReturnPrediction = { Month: DateTime; R: float }
let muHistorical =
    let mutable acc = 0.0
    let mutable n = 0.0
    let mutable dt = shiller.Keys |> Seq.min
    [| while dt <= DateTime(2022,12,1) do 
        if dt < DateTime(1927,1,1) then 
          acc <- acc + shiller[dt].Ret - crspRet[dt].Rfree
        else
          acc <- acc + crspRet[dt].CRSP_SPvw - crspRet[dt].Rfree
        n <- n + 1.0
        dt <- dt.AddMonths(1)
        { Month = dt.AddMonths(1)
          R = acc / n}
    |]
let muHistoricalMap = muHistorical |> Seq.map (fun x -> x.Month, x.R) |> Map

(** Might have seen a chart like this before. Looks like CAPE is a good predictor. *)
capePredictor
|> Seq.filter (fun x -> x.Month <= DateTime(2005,12,1))
|> Seq.sortBy (fun x -> x.CAPE)
|> Seq.splitInto 5
|> Seq.map (fun xs -> 
    let cape = xs |> Seq.map (fun x -> x.CAPE) |> Seq.average
    let r = xs |> Seq.map (fun x -> x.R * 12.0) |> Seq.average
    $"%.2f{cape}", r)
|> Chart.Column
|> Chart.withXAxisStyle ("CAPE")
|> Chart.withYAxisStyle ("Annualized Excess Return")
|> Chart.withYAxis(LayoutObjects.LinearAxis.init(TickFormat = ".1%"))


(**
Though simple OLS results are not too special.
*)

Ols("R~CAPE", capePredictor).fit().summary()

(** Until 2005 *)
Ols("R~CAPE", 
    capePredictor 
    |> Array.filter (fun x -> x.Month <= DateTime(2005,12,1))).fit().summary()

(**
We can do rolling regressions.
*)

let capePrediction (xs: array<Predictors>) =
    let mdl = Ols("R~CAPE",xs[..xs.Length-2]).fit()
    let lastObs = xs |> Array.last
    { Month = lastObs.Month 
      R = mdl.predict([lastObs])[0] }

capePrediction capePredictor[..100]

(** Restricted predictor*)
let capePredictionRestricted (xs: array<Predictors>) =
    let mdl = Ols("R~CAPE",xs[..xs.Length-2]).fit()
    let lastObs = xs |> Array.last
    let slope = min 0.0 mdl.coefs["CAPE"]
    let prediction = max 0.0 (mdl.coefs["Intercept"] + slope * lastObs.CAPE)
    { Month = lastObs.Month 
      R = prediction }

(** Restricted vs. unrestricted predictions*)
let index1999 = capePredictor |> Array.findIndex (fun x -> x.Month = DateTime(1999,1,1))
printfn $"Unrestricted:\n{capePrediction capePredictor[..index1999]}"
printfn $"Restricted:\n{capePredictionRestricted capePredictor[..index1999]}"

(** the CAPE predictons. *)
let cape1927Index =
    capePredictor
    |> Array.findIndex (fun x -> x.Month = DateTime(1927,1,1))

let muCAPE =
    [| for i = cape1927Index to capePredictor.Length-1 do 
        capePrediction capePredictor[..i] |]

(** Historical sample average. *)


(** 
Compare predictions
*)

[ Chart.Line (muCAPE |> Array.map (fun x -> x.Month, 12.0*x.R),
              Name = "CAPE")
  Chart.Line (muHistorical |> Array.map (fun x -> x.Month, 12.0*x.R),
              Name = "Historical")]
|> Chart.combine

(**
Accuracy
*)

let muCAPEMap = muCAPE |> Array.map (fun x -> x.Month, x.R) |> Map

type PredictionCombo =
    { Month: DateTime 
      Actual: float 
      PredRegression: float 
      PredHistAvg: float }
let accuracyComps =
    [| for i = cape1927Index to capePredictor.Length-1 do
        let month = capePredictor[i].Month 
        {  Month = month
           Actual = 12.0*capePredictor[i].R 
           PredRegression = 12.0*muCAPEMap[month]
           PredHistAvg = 12.0*muHistoricalMap[month] }|]

accuracyComps
|> Array.filter (fun x ->
    x.Month = DateTime(1929,1,1) ||
    x.Month = DateTime(2000,1,1) ||
    x.Month = DateTime(2022,12,1))

(** root mean squared errors *)

let rmse xy = 
    xy 
    |> Array.averageBy (fun (x,y) -> (x - y) ** 2.0)
    |> sqrt

let rmses =
    [| for i = 1 to accuracyComps.Length-1 do 
        let mseCAPE = 
            accuracyComps[..i]
            |> Array.map (fun x -> x.Actual, x.PredRegression)
            |> rmse 
        let mseHist =
            accuracyComps[..i]
            |> Array.map (fun x -> x.Actual, x.PredHistAvg)
            |> rmse
        {| Month = accuracyComps[i].Month 
           mseCAPE = mseCAPE
           mseHist = mseHist |}|]

[ Chart.Line(rmses |> Array.map (fun x -> x.Month, x.mseCAPE),
             Name ="CAPE")
  Chart.Line(rmses |> Array.map (fun x -> x.Month, x.mseHist),
             Name = "Hist")]
|> Chart.combine

(** The difference, much easier to interpret. *)
rmses
|> Array.map (fun x ->
    x.Month, (x.mseHist - x.mseCAPE))
|> Chart.Line
|> Chart.withTitle("Hist Avg. RMSE - CAPE RMSE")
|> Chart.withYAxisStyle(TitleText="Positive means that CAPE has lower error")

(** OOS R2*)
let oosR2 (xs: seq<PredictionCombo>) =
    let sseHist = xs |> Seq.sumBy (fun x -> (x.Actual - x.PredHistAvg) ** 2.0)
    let sseRegression = xs |> Seq.sumBy (fun x -> (x.Actual - x.PredRegression) ** 2.0)
    1.0 - sseRegression / sseHist

(** Pre 2005 *)
accuracyComps
|> Array.filter (fun x -> x.Month <= DateTime(2005,12,1))
|> oosR2

(** Plot of OOS R2*)
[| for i = 1 to accuracyComps.Length-1 do 
    accuracyComps[i].Month,
    accuracyComps[..i] |> oosR2 |]
|> Array.filter (fun (dt, xs) -> dt <= DateTime(2005,12,1))
|> Chart.Line
|> Chart.withYAxis(LayoutObjects.LinearAxis.init(TickFormat = ".1%",TickVals = [| -0.01;0.0;0.01;0.02|]))
|> Chart.withYAxisStyle(MinMax = (-0.01,0.02))
|> Chart.withSize(Width=1000,Height=500)