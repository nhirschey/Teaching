#r "nuget: FSharp.Data"

open System
open System.IO
open FSharp.Data

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

module Secrets =
    let envVars = System.Environment.GetEnvironmentVariables() 
    let localPath = "../secrets.fsx"
    let localPath2 = "secrets.fsx"
    let tiingoKey = 
        let var = "TIINGO_KEY"
        if envVars.Contains var then 
            envVars.[var] :?> string
        elif File.Exists(localPath) then 
            File.ReadAllText(localPath)
                .Replace("let tiingoKey = ","")
                .Replace("\"","")
        elif File.Exists(localPath2) then
            File.ReadAllText(localPath2)
                .Replace("let tiingoKey = ","")
                .Replace("\"","")
        else "you don't have a key"


type Frequency = Daily | Monthly
type ReturnObs = { Symbol : string; Date : DateTime; Return : float }

module Tiingo =

    type TiingoCsv = CsvProvider<"date,close,high,low,open,volume,adjClose,adjHigh,adjLow,adjOpen,adjVolume,divCash,splitFactor
2020-10-01,9.77,10.25,9.69,10.09,4554055,9.77,10.25,9.69,10.09,4554055.0,0.0,1.0">

    type TiingoRequest = { Symbol : string; Start : DateTime; End : DateTime }

    type TiingoObs =
        {
            Date : DateTime
            Close : decimal
            High : decimal
            Low : decimal
            Open : decimal 
            Volume : int
            AdjClose : decimal
            AdjHigh : decimal
            AdjLow : decimal
            AdjOpen : decimal
            AdjVolume : decimal
            DivCash : decimal
            SplitFactor : decimal
        }

    ///<summary>Constructs a Tiingo request. By default is to get the past year of data.</summary>
        /// <param name="symbol">The ticker symbol such as "AAPL","MSFT" etc.</param>
    let request symbol = { Symbol = symbol; Start = DateTime.Now.AddYears(-1); End = DateTime.Now}
    ///<summary>Sets the Tiingo request start date.</summary>
        /// <param name="startOn">Request start date</param>
        /// <param name="request">The Tiingo request to update.</param>
    let startOn startOn request = { request with Start = startOn }
    ///<summary>Sets the Tiingo request end date.</summary>
        /// <param name="endOn">Request start date</param>
        /// <param name="request">The Tiingo request to update.</param>
    let endOn endOn request = { request with End = endOn }

    let private cache = Runtime.Caching.createInMemoryCache (TimeSpan(hours=12,minutes=0,seconds=0))

    ///<summary>Downloads Tiingo data.</summary>
        /// <param name="request">The Tiingo request to download.</param>
    let get request =
        let dtStr (x : DateTime) = x.Date.ToString("yyyy-MM-dd")
        let request = { request with Start = request.Start.Date; End = request.End.Date }
        let key = $"{request.Symbol}-{dtStr request.Start}-{dtStr request.End}.csv"
        match cache.TryRetrieve(key) with
        | Some res -> res
        | None ->
            let result = 
                Http.RequestString
                            ( $"https://api.tiingo.com/tiingo/daily/{request.Symbol}/prices", 
                                httpMethod = "GET",
                                query   = [ "token", Secrets.tiingoKey; 
                                            "startDate", request.Start.ToString("yyyy-MM-dd");
                                            "endDate", request.End.ToString("yyyy-MM-dd");
                                            "format","csv"],
                                headers = [HttpRequestHeaders.Accept HttpContentTypes.Csv])
            cache.Set(key,result)
            result
        |> TiingoCsv.Parse
        |> fun parsed ->
            parsed.Rows
            |> Seq.map(fun row ->
                { Date = row.Date
                  Close = row.Close
                  High = row.High
                  Low = row.Low
                  Open = row.Open
                  Volume = row.Volume
                  AdjClose = row.AdjClose
                  AdjHigh = row.AdjHigh
                  AdjLow = row.AdjLow
                  AdjOpen = row.AdjOpen
                  AdjVolume = row.AdjVolume
                  DivCash = row.DivCash
                  SplitFactor = row.SplitFactor 
                  })
            |> Seq.toArray      
    
    // using a class, keeping private for now.
    type private Download(symbol:string,?startOn:DateTime,?endOn:DateTime) =
        let startOn = defaultArg startOn (DateTime.Now.AddYears(-1))
        let endOn = defaultArg endOn (DateTime.Now)
        let data = get { Symbol = symbol; Start = startOn; End = endOn }
        member this.Rows = data
 
    // Probably deprecated
    let private getFromCacheDirectory cacheDirectory request =
        let dtStr (x : DateTime) = x.Date.ToString("yyyy-MM-dd")
        let request = { request with Start = request.Start.Date; End = request.End.Date }
        let key = $"{request.Symbol}-{dtStr request.Start}-{dtStr request.End}.csv"
        let cacheFile = cacheDirectory + key
        if File.Exists(cacheFile) then
            File.ReadAllText(cacheFile)
        else    
            let result = 
                Http.RequestString
                            ( $"https://api.tiingo.com/tiingo/daily/{request.Symbol}/prices", 
                                httpMethod = "GET",
                                query   = [ "token", Secrets.tiingoKey; 
                                            "startDate", request.Start.ToString("yyyy-MM-dd");
                                            "endDate", request.End.ToString("yyyy-MM-dd");
                                            "format","csv"],
                                headers = [HttpRequestHeaders.Accept HttpContentTypes.Csv])
            File.WriteAllText(cacheFile,result)
            result
        |> TiingoCsv.Parse
    
    let private returnHelper symbol (xs:TiingoObs seq) =
        xs
        |> Seq.sortBy(fun x -> x.Date)
        |> Seq.pairwise
        |> Seq.map(fun (yesterday, today) ->
            { Symbol = symbol 
              Date = today.Date
              Return =  float (today.AdjClose / yesterday.AdjClose) - 1.0})
        |> Seq.toArray      

    let getReturns request =
        get request
        |> (returnHelper request.Symbol)

    // Marking as private so people don't use it by accident
    let private getInternetFileCache request =
        let cache = Runtime.Caching.createInternetFileCache "tiingo" (TimeSpan.FromDays 30.0)
        let request = { request with Start = request.Start.Date; End = request.End.Date }
        let key = request.ToString()
        match cache.TryRetrieve(key) with
        | Some res -> res
        | None ->
            let res =
                Http.RequestString
                        ( $"https://api.tiingo.com/tiingo/daily/{request.Symbol}/prices", 
                            httpMethod = "GET",
                            query   = [ "token", Secrets.tiingoKey; 
                                        "startDate", request.Start.ToString("yyyy-MM-dd");
                                        "endDate", request.End.ToString("yyyy-MM-dd");
                                        "format","csv"],
                            headers = [HttpRequestHeaders.Accept HttpContentTypes.Csv ])
            cache.Set(key, res)
            res
        |> TiingoCsv.Parse

module French =
    //open System.Net
    open System.IO.Compression

    type private FF3Csv = CsvProvider<"Date (string),Mkt-RF,SMB,HML,RF
        19260701,    0.10,   -0.24,   -0.28,   0.009">
    type FF3Obs = 
        { Date : DateTime 
          MktRf : float
          Smb : float 
          Hml : float
          Rf : float 
          Frequency : Frequency } 

    type private FF5Csv = CsvProvider<"Date (string),Mkt-RF,SMB,HML,RMW,CMA,RF
        19260701,    0.10,   -0.24,   -0.28,0.0,1.2,  0.009">

    type FF5Obs = 
        { Date : DateTime 
          MktRf : float
          Smb : float 
          Hml : float
          Rmw : float
          Cma : float
          Rf : float 
          Frequency : Frequency } 

    let private frenchDay x = 
        DateTime.ParseExact(x,
            "yyyyMMdd",
            Globalization.CultureInfo.InvariantCulture)
    let private frenchMonth x = 
        DateTime.ParseExact(x,
            "yyyyMM",
            Globalization.CultureInfo.InvariantCulture)

    let private cache = 
        let today = DateTime.Now
        let nextMonth = today.AddMonths(1)
        let eom = DateTime(nextMonth.Year, nextMonth.Month, 1).AddSeconds(-1.0) 
        Runtime.Caching.createInternetFileCache "French" (eom - today)

    let private getData (dataset:string) =
        match cache.TryRetrieve(dataset) with
        | Some data -> data
        | None ->
            //let dataset = "F-F_Research_Data_Factors_CSV"
            let urlString = $"http://mba.tuck.dartmouth.edu/pages/faculty/ken.french/ftp/{dataset}.zip"
            let request = Http.RequestStream(urlString, httpMethod = "GET",headers = [HttpRequestHeaders.Accept HttpContentTypes.Any])
            use archive = new ZipArchive(request.ResponseStream,ZipArchiveMode.Read)
            let file = archive.GetEntry($"{dataset}".Replace("_CSV",".CSV"))
            use reader = new StreamReader(file.Open())
            let data  = reader.ReadToEnd()
            cache.Set(dataset,data)
            data
    let getFF3 frequency =
            let (dataset, dateParser) =
                match frequency with
                | Monthly -> "F-F_Research_Data_Factors_CSV", frenchMonth
                | Daily -> "F-F_Research_Data_Factors_daily_CSV", frenchDay
            let data = new StringReader(getData dataset)
            [| while data.Peek() <> -1 do
                    data.ReadLine() |]
            |> Array.skipWhile(fun line -> not (line.Contains("Mkt-RF")))
            |> Array.skip 1
            |> Array.takeWhile(fun line -> line <> "")
            |> Array.map(fun line -> 
                let parsedLine = FF3Csv.ParseRows(line).[0] 
                { Date = dateParser parsedLine.Date
                  MktRf = float parsedLine.``Mkt-RF`` / 100.0
                  Smb = float parsedLine.SMB / 100.0
                  Hml = float parsedLine.HML / 100.0
                  Rf = float parsedLine.RF / 100.0 
                  Frequency = frequency })

    let getFF5 frequency =
        let (dataset, dateParser) =
            match frequency with
            | Monthly -> "F-F_Research_Data_5_Factors_2x3_CSV", frenchMonth
            | Daily -> "F-F_Research_Data_5_Factors_2x3_daily_CSV", frenchDay
        let data = new StringReader(getData dataset)
        [| while data.Peek() <> -1 do
                data.ReadLine() |]
        |> Array.skipWhile(fun line -> not (line.Contains("Mkt-RF")))
        |> Array.skip 1
        |> Array.takeWhile(fun line -> line <> "")
        |> Array.map(fun line -> 
            let parsedLine = FF5Csv.ParseRows(line).[0] 
            { Date = dateParser parsedLine.Date
              MktRf = float parsedLine.``Mkt-RF`` / 100.0
              Smb = float parsedLine.SMB / 100.0
              Hml = float parsedLine.HML / 100.0
              Rmw = float parsedLine.RMW / 100.0
              Cma = float parsedLine.CMA / 100.0
              Rf = float parsedLine.RF / 100.0 
              Frequency = frequency })

module Fred =
    type Series = CsvProvider<"https://fred.stlouisfed.org/graph/fredgraph.csv?id=GS10",
                              Schema="Date,Value (float)",
                              MissingValues=".">
    let private fredUrl series = $"https://fred.stlouisfed.org/graph/fredgraph.csv?id={series}"
    
    ///<summary>Gets a FRED data series as a CsvProvider</summary>
        /// <param name="series">The series name such as GS10, EXUSEU, etc.</param>
    let get (series:string) =  Series.Load(fredUrl series)
    