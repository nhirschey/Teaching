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
let private parseYahooPriceHistory symbol result = 
    PriceObsCsv.Parse(result).Rows
    |> Seq.map (fun x -> 
        { Symbol = symbol 
          Date = x.Date
          Open = x.Open
          High = x.High
          Low = x.Low
          Close = x.Close 
          AdjustedClose = x.AdjClose
          Volume = x.Volume })
    |> Seq.toArray


let private cc = System.Net.CookieContainer()
let private retryCount = 5
let private parallelSymbols = 5

type YahooFinance =
    static member PriceHistory(symbols: seq<string>,?startDate: DateTime,?endDate: DateTime,?interval: Interval) =
        let symbols = Seq.toList symbols
        let startDate = defaultArg startDate (DateTime.Now.AddYears(-1))
        let endDate = defaultArg endDate (DateTime.Now)
        let interval = defaultArg interval Interval.Monthly

        let generateYahooUrl (symbol: string) (startDate: DateTime) (endDate: DateTime) (interval: Interval) =
            let time dt = DateTimeOffset(dt).ToUnixTimeSeconds()
            $"https://query1.finance.yahoo.com/v7/finance/download/{symbol}?" +
            $"period1={time startDate}&period2={time endDate}&interval={interval}" +
            $"&events=history&includeAdjustedClose=true"
        
        let rec yahooRequest attempt symbol =
            async {
                let url = generateYahooUrl symbol startDate endDate interval
                try
                    let! result = 
                        Http.AsyncRequestString(
                            url = url, 
                            httpMethod = "GET",
                            query = ["format","csv"],
                            headers = [HttpRequestHeaders.Accept HttpContentTypes.Csv],
                            cookieContainer = cc)
                    return parseYahooPriceHistory symbol result
                with e ->
                    if attempt > 0 then
                        return! yahooRequest (attempt - 1) symbol
                    else return! failwith $"Failed to request {symbol}, Error: {e}"
            }
        let rec getSymbols (symbols: list<string>) output parallelSymbols =
            let download thisDownload =
                [| for symbol in thisDownload do 
                    yahooRequest retryCount symbol |]
                |> Async.Parallel
                |> Async.RunSynchronously
                |> Array.collect id
                |> Array.toList

            if symbols.Length > parallelSymbols then
                let thisDownload, remaining = symbols |> List.splitAt parallelSymbols
                let result = download thisDownload
                System.Threading.Thread.Sleep(1000) // Throttle 1 sec per batch of symbols
                getSymbols remaining (result @ output) parallelSymbols
            else
                let result = download symbols
                result @ output
        getSymbols symbols [] parallelSymbols                
    static member PriceHistory(symbol: string,?startDate: DateTime,?endDate: DateTime,?interval: Interval) =
        YahooFinance.PriceHistory(symbols=[symbol],?startDate=startDate,?endDate=endDate,?interval=interval)

