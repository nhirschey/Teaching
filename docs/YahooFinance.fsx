(**
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)

Based on code from https://github.com/aexsalomao/YahooFinance/
*) 

#r "nuget: FSharp.Data"

open System
open FSharp.Data

type Interval = 
    | Daily
    | Weekly
    | Monthly
    override this.ToString() = 
        match this with
        | Daily -> "1d"
        | Weekly -> "1wk"
        | Monthly -> "1mo"

type PriceObs = 
    { Symbol : string
      Date : DateTime
      Open : float
      High : float
      Low : float
      Close : float
      AdjustedClose : float
      Volume : float }

type private PriceObsCsv = CsvProvider<Sample="Date (date),Open (float),High (float),Low (float), Close (float),AdjClose (float),Volume (float)">
let private cc = System.Net.CookieContainer()

type YahooFinance =
    static member PriceHistory(symbol: string,?startDate: DateTime,?endDate: DateTime,?interval: Interval) =
        let startDate = defaultArg startDate (DateTime.Now.AddYears(-1))
        let endDate = defaultArg endDate (DateTime.Now)
        let interval = defaultArg interval Interval.Monthly

        let generateYahooUrl (symbol: string) (startDate: DateTime) (endDate: DateTime) (interval: Interval) =
            let time dt = DateTimeOffset(dt).ToUnixTimeSeconds()
            $"https://query1.finance.yahoo.com/v7/finance/download/{symbol}?" +
            $"period1={time startDate}&period2={time endDate}&interval={interval}" +
            $"&events=history&includeAdjustedClose=true"
        
        let url = generateYahooUrl symbol startDate endDate interval
        let req = Http.RequestString(
                        url = url, 
                        httpMethod = "GET",
                        query = ["format","csv"],
                        headers = [HttpRequestHeaders.Accept HttpContentTypes.Csv],
                        silentHttpErrors = false,
                        cookieContainer = cc)
        PriceObsCsv.Parse(req).Rows
        |> Seq.map (fun x -> 
            { Symbol = symbol 
              Date = x.Date
              Open = x.Open
              High = x.High
              Low = x.Low
              Close = x.Close 
              AdjustedClose = x.AdjClose
              Volume = x.Volume })
        |> Seq.toList