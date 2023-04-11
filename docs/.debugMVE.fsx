#r "nuget: NovaSBE.Finance"
#r "nuget: FSharp.Stats"

open System
open FSharp.Stats
open NovaSBE.Finance.French

type ReturnObs = { Date: DateTime; Return: float}

type VolatilityPrediction = 
    { /// First date the prediction is valid for
      Date: DateTime
      /// The volatility prediction
      PredictedVol: float }


let ff3 = 
    getFF3 Frequency.Daily 
    |> Array.toList


let meanVarianceUtility gamma returns =
    let mu = returns |> Seq.average
    let sigma = returns |> stDev
    mu - 0.5 * gamma * sigma ** 2.0

let sharpeRatio (returns: list<float>) =
    let mu = returns |> Seq.average
    let sigma = returns |> stDev
    mu / sigma

type ReturnAndVol = { ReturnOb: FF3Obs; Vol: float }
let returnsAndVols =
    ff3
    |> Seq.sortBy (fun x -> x.Date)
    |> Seq.windowed 23
    |> Seq.map (fun window ->
        let dayToPredict = window |> Seq.last
        let trainingDays = window |> Seq.take 22
        { ReturnOb = dayToPredict; Vol = trainingDays |> stDevBy (fun x -> x.MktRf) })
    |> Seq.toList
    |> List.filter (fun x -> x.ReturnOb.Date <= DateTime(2015,12,31))

let sampleAvg = returnsAndVols |> Seq.averageBy (fun x -> x.ReturnOb.MktRf)
let sampleVar = returnsAndVols |> stDevBy (fun x -> x.ReturnOb.MktRf)

let gamma = 3.0
let buyAndHold = 
    let w = sampleAvg / (gamma * sampleVar)
    returnsAndVols
    |> List.map (fun x -> w * x.ReturnOb.MktRf)

let managed =
    returnsAndVols
    |> List.map (fun x -> 
        let w = sampleAvg / (gamma * x.Vol ** 2.0)
        w * x.ReturnOb.MktRf)

(meanVarianceUtility 3.0 buyAndHold)*252.0

(meanVarianceUtility 3.0 managed)*252.0


////
/// 
/// 

let realizedMonthVar =
    ff3
    |> Seq.sortBy (fun x -> x.Date)
    |> Seq.windowed 22
    |> Seq.map (fun window ->
        let lastDay = window |> Seq.last
        lastDay.Date, 22.0 * (window |> varBy (fun x -> x.MktRf)))
    |> Seq.groupBy (fun (dt, vol) -> dt.Year, dt.Month)
    |> Seq.map (fun (month, days) ->
        let (lastDay, lastPrediction) =
            days
            |> Seq.sortBy fst
            |> Seq.last
        let nextMonth = lastDay.AddMonths(1)
        DateTime(nextMonth.Year, nextMonth.Month, 1), lastPrediction)
    |> Map


let ff3Monthly = 
    getFF3 Frequency.Monthly
    |> Array.toList
    |> List.filter (fun x -> 
        x.Date >= DateTime(1926,8,1) && 
        x.Date <= DateTime(2015,12,31))

let monthlyAvg = ff3Monthly |> Seq.averageBy (fun x -> x.MktRf)
let monthlyVar = ff3Monthly |> varBy (fun x -> x.MktRf)

let buyAndHoldMonthly = 
    let w = monthlyAvg / (gamma * monthlyVar)
    ff3Monthly
    |> List.map (fun x -> w * x.MktRf)

let managedMonthly =
    ff3Monthly
    |> List.map (fun x -> 
        let w = monthlyAvg / (gamma * realizedMonthVar[x.Date])
        w * x.MktRf)

(meanVarianceUtility 3.0 buyAndHoldMonthly)*12.0

(meanVarianceUtility 3.0 managedMonthly)*12.0

let sharpeBH = (sqrt 12.0) * (sharpeRatio buyAndHoldMonthly)
let sharpeMV = (sqrt 12.0) * (sharpeRatio managedMonthly)

(sharpeMV ** 2.0 - sharpeBH ** 2.0) / (sharpeBH ** 2.0)

let buyHoldVolMonthly = buyAndHoldMonthly |> stDev
let managedVolMonthly = managedMonthly |> stDev

let managedMonthlyNormalized = 
    managedMonthly
    |> List.map (fun x -> x * (buyHoldVolMonthly / managedVolMonthly))

let bhU = (meanVarianceUtility 3.0 buyAndHoldMonthly)*12.0
let mvU = (meanVarianceUtility 3.0 managedMonthlyNormalized)*12.0

mvU/bhU - 1.0

managedMonthly |> mean
buyAndHoldMonthly |> mean

realizedMonthVol
|> Map.toSeq
|> Chart.Line
|> Chart.show

type ReturnPrediction = { Date: DateTime; PredictedReturn: float }

type VolAndReturnPrediction = { Date: DateTime; PredictedVol: float; PredictedReturn: float }

let combinePredictions (predReturns: seq<ReturnPrediction>) (predVols: seq<VolatilityPrediction>) =
    let predVols = 
        predVols 
        |> Seq.map (fun x -> x.Date, x.PredictedVol) 
        |> Map
    [ for retObs in predReturns do 
        if predVols.ContainsKey retObs.Date then
            { Date = retObs.Date 
              PredictedVol = predVols[retObs.Date]
              PredictedReturn = retObs.PredictedReturn } ]

let managedPortfolio gamma predVols predReturns (xs: list<ReturnObs>) =
    let preds = 
        combinePredictions predReturns predVols
        |> List.map (fun x -> x.Date, x)
        |> Map
    [ for x in xs do 
        if preds.ContainsKey x.Date then
            let pred = preds[x.Date]
            let w = pred.PredictedReturn / (gamma* pred.PredictedVol ** 2.0)
            { Date = x.Date
              Return = w * x.Return } ]

0.05 - 0.5 * 3.0 * (0.1 ** 2.0)

0.04 - 0.5 * 3.0 * (0.1 ** 2.0)
