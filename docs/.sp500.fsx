#r "nuget: FSharp.Data, 5.0.2"

open System
open FSharp.Data

type WikiPage = HtmlProvider<"https://en.wikipedia.org/wiki/List_of_S%26P_500_companies">

let sp500 = WikiPage.GetSample()

[ for row in sp500.Tables.``S&P 500 component stocksEdit``.Rows do 
    row.Symbol, row.``Date first added`` ]

type IndexMember =
    { Ticker: string
      DateAdded: DateTime }
      
type Index =
    { Date: DateTime
      Tickers: string list }

type IndexChange = 
    { Date: DateTime
      Added: string
      Removed: string }

let changes = 
    [ for row in sp500.Tables.``Selected changes to the list of S&P 500 componentsEdit``.Rows do 
        { Date = row.Date
          Added = row.``Added - Ticker``.Trim()
          Removed = row.``Removed - Ticker``.Trim() } ]

let currentIndex =
    { Date = DateTime.Now 
      Tickers = [ for row in sp500.Tables.``S&P 500 component stocksEdit``.Rows do row.Symbol.Trim() ] }

[ for row in sp500.Tables.``S&P 500 component stocksEdit``.Rows do 
    let dt = 
        match row.``Date first added`` with
        | "" -> None
        | d when d.Length >= 10 -> DateTime.ParseExact(d[0..9], "yyyy-MM-dd", Globalization.CultureInfo.InvariantCulture) |> Some
        | d when d.Length = 4 -> DateTime.ParseExact(d[0..3], "yyyy", Globalization.CultureInfo.InvariantCulture).AddYears(1).AddDays(-1) |> Some
        | d -> failwithf "Unknown date format: %s" d
    dt ]
|> List.choose id
|> List.countBy(fun x -> x < DateTime(2017,1,1))

let constructIndex (changes: IndexChange list) (history: Index list) =
    let rec loop changes history =
        match changes, history with
        | [], _ | _, [] -> history
        | change :: changes, h::hs ->
            let newTickers = 
                if change.Added <> "" then
                    change.Added::h.Tickers
                else
                    h.Tickers
                |> List.filter (fun t -> t <> change.Removed) 
                
            let idx = 
                { Date = change.Date
                  Tickers = newTickers }
            if h.Date > change.Date then
                loop changes (idx::history)
            else
                loop changes (idx::hs)
    loop changes history
DateTime.ParseExact("2008","yyyy", Globalization.CultureInfo.InvariantCulture).AddYears(1).AddDays(-1)
let history = constructIndex changes [ currentIndex ]
