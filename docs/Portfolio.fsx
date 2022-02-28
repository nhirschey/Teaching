(**
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
*)

module Portfolio

#r "nuget: NodaTime"

open System
open NodaTime

/// Misc
/// 

// Type extension until NodaTime puts YearMonth.PlusMonths() in the nuget version.
// taken from here in the NodaTime repo until it is accessible
// https://github.com/carlosschults/nodatime/blob/43e9f24c2ba5a7ed0fd145c082d9e63cd50b1149/src/NodaTime/YearMonth.cs#L156
// See https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/type-extensions
// to understand type extensions.
type YearMonth with
    member this.PlusMonths(months:int) =
                this.OnDayOfMonth(1).PlusMonths(months).ToYearMonth()
///

type SecurityId =
    | Ticker of string
    | Cusip of string
    | Bloomberg of string
    | Permno of int
    | Other of string

type InvestmentUniverse = 
    { FormationMonth : YearMonth 
      Securities : SecurityId list }

type SecuritySignal = 
    { SecurityId : SecurityId
      Signal : float }

type SecuritiesWithSignals =
    { FormationMonth : YearMonth 
      Signals : SecuritySignal list }

type PortfolioId = 
    | Named of string
    | Indexed of name:string * index:int

type AssignedPortfolio =
    { PortfolioId : PortfolioId
      FormationMonth : YearMonth 
      Signals : SecuritySignal list }


let assignSignalSort name n (xs: SecuritiesWithSignals) =
    xs.Signals
    |> List.sortBy(fun x -> x.Signal)
    |> List.splitInto n
    |> List.mapi(fun i ys -> 
        // because lists are 0-indexed and I want the minimum
        // portfolio index to be 1, I'm doing index = i+1.
        { PortfolioId = Indexed(name=name,index=i+1)
          FormationMonth = xs.FormationMonth
          Signals = ys })

type Position =
    { SecurityId : SecurityId 
      Weight : float }

type Portfolio = 
    { PortfolioId: PortfolioId
      FormationMonth : YearMonth
      Positions : Position list }

type PortfolioReturn =
    { PortfolioId: PortfolioId
      YearMonth : YearMonth
      Return : float }

// This type alias defines the type of a function that has
// a tuple input of SecurityId * YearMonth and outputs
// a (SecurityId * float) Option. Think of it like
// using types to write documentation of the functions.
type GetsMarketCaps = SecurityId * YearMonth -> (SecurityId * float) Option

// Defining this type alias makes it easier to read the type of the function that I want for
// marketCapGetter in the function that I have below. Otherwise it might look something like
// let giveValueWeights (marketCapGetter: (SecurityId * YearMonth -> (SecurityId * float) Option) ...
// which is the same thing but not as clear what we're trying to do.
let giveValueWeights (marketCapGetter: GetsMarketCaps) (x: AssignedPortfolio) =
    let mktCaps =
        x.Signals
        // List.choose throws away None results, so this means 
        // that if the market cap getter returns None
        // then we will not have that security in our portfolio.
        |> List.choose(fun signal -> marketCapGetter (signal.SecurityId, x.FormationMonth))

    let totMktCap = mktCaps |> List.sumBy snd

    let pos =
        mktCaps
        |> List.map(fun (id, mktCap) ->
            { SecurityId = id 
              Weight = mktCap / totMktCap })
    { PortfolioId = x.PortfolioId
      FormationMonth = x.FormationMonth 
      Positions = pos }

// This type alias defines the type of a function that has
// a tuple input of SecurityId * YearMonth and outputs
// a (SecurityId * float). Think of it like
// using types to write documentation of the functions.
type GetsReturn = SecurityId * YearMonth -> (SecurityId * float)

// Defining this type alias makes it easier to read the type of the function that I want for
// marketCapGetter in the function that I have below. Otherwise it might look something like
// let giveValueWeights (marketCapGetter: (SecurityId * YearMonth -> (SecurityId * float) Option) ...
// which is the same thing but not as clear what we're trying to do.
let getPortfolioReturn (returnGetter: GetsReturn) (x: Portfolio) =
    let returnMonth = x.FormationMonth.PlusMonths(1)
    let portRet =
        x.Positions
        |> List.sumBy(fun pos -> 
            let (_id, ret) = returnGetter (pos.SecurityId, returnMonth)
            pos.Weight * ret)
    { PortfolioId = x.PortfolioId
      YearMonth = returnMonth
      Return = portRet }


/// This function takes a sample start and sample end
/// and returns a list with all months from start to end.
/// Don't worry about understanding what this function does.
/// The details are beyond the scope of the class, but if you're
/// curious it's a recursive function:
/// https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/functions/recursive-functions-the-rec-keyword
let getSampleMonths (sampleStart:YearMonth, sampleEnd:YearMonth) =
    if sampleEnd <= sampleStart then failwith "sampleEnd should be after sampleStart"
    let rec loop (sampleEnd:YearMonth) window = 
        match window with
        | [] -> failwith "Need a starting point"
        | lastMonth::_monthsBeforeThat ->
            if lastMonth < sampleEnd then 
                loop sampleEnd (lastMonth.PlusMonths(1)::window)
            else window
    loop sampleEnd [sampleStart]
    |> List.rev