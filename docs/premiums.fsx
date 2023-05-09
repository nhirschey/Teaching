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
        not (isNull x.CAPE))
    |> Seq.pairwise
    |> Seq.choose (fun (x0, x1) ->
        match x1.CAPE with
        | "NA" -> None
        | cape -> 
            let result = 
                { Month = shillerDate x1
                  CAPE = float cape 
                  Ret = (float x1.P + float x1.D / 12.0) / float x0.P - 1.0}
            Some (result.Month, result))
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
type ReturnPrediction = { Month: DateTime; R: float }
let capePrediction (xs: array<Predictors>) =
    let mdl = Ols("R~CAPE",xs[..xs.Length-2]).fit()
    let lastObs = xs |> Array.last
    { Month = lastObs.Month 
      R = mdl.predict([lastObs])[0] }

capePrediction capePredictor[..100]

(** the CAPE predictons. *)
let cape1927Index =
    capePredictor
    |> Array.findIndex (fun x -> x.Month = DateTime(1927,1,1))

let muCAPE =
    [| for i = cape1927Index to capePredictor.Length-1 do 
        capePrediction capePredictor[..i] |]

(** Historical sample average. *)

let muHistorical =
    let mutable acc = 0.0
    [| for i = 0 to capePredictor.Length-1 do 
        acc <- acc + capePredictor[i].R 
        { Month = capePredictor[i].Month.AddMonths(1)
          R = acc / (float i + 1.0)} |]


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
let muHistoricalMap = muHistorical |> Array.map (fun x -> x.Month, x.R) |> Map

type PredictionCombo =
    { Month: DateTime 
      Actual: float 
      PredCAPE: float 
      PredHist: float }
let accuracyComps =
    [| for i = cape1927Index to capePredictor.Length-1 do
        let month = capePredictor[i].Month 
        {  Month = month
           Actual = capePredictor[i].R 
           PredCAPE = muCAPEMap[month]
           PredHist = muHistoricalMap[month] }|]

accuracyComps[..3]


(** root mean squared errors *)

let rmse xy = 
    xy 
    |> Array.averageBy (fun (x,y) -> (x - y) ** 2.0)
    |> sqrt

let rmses =
    [| for i = 1 to accuracyComps.Length-1 do 
        let mseCAPE = 
            accuracyComps[..i]
            |> Array.map (fun x -> x.Actual, x.PredCAPE)
            |> rmse 
        let mseHist =
            accuracyComps[..i]
            |> Array.map (fun x -> x.Actual, x.PredHist)
            |> rmse
        {| Month = accuracyComps[i].Month 
           mseCAPE = mseCAPE
           mseHist = mseHist |}|]

[ Chart.Line(rmses |> Array.map (fun x -> x.Month, x.mseCAPE),
             Name ="CAPE")
  Chart.Line(rmses |> Array.map (fun x -> x.Month, x.mseHist),
             Name = "Hist")]
|> Chart.combine

(** The difference. *)
rmses
|> Array.map (fun x ->
    x.Month, 12.0*(x.mseCAPE - x.mseHist))
|> Chart.Line
|> Chart.withTitle("CAPE RMSE - Hist Avg. RMSE")
