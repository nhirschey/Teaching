(**
---
title: Momentum Signal
category: Lectures
categoryindex: 1
index: 5
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)
*)

#time "on"

#r "nuget: FSharp.Data"
#r "nuget: FSharp.Stats"
#r "nuget: Plotly.NET,2.0.0-preview.17"
#r "nuget: Plotly.NET.Interactive,2.0.0-preview.17"

#load "Portfolio.fsx"

open System
open FSharp.Data
open FSharp.Stats
open Plotly.NET

open Portfolio

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

let samplePeriod x = 
    x >= DateTime(2010, 1, 1) &&
    x <= DateTime(2020, 2, 1)

(*** condition: ipynb ***)
#if IPYNB
// Set dotnet interactive formatter to plaintext
Formatter.Register(fun (x:obj) (writer: TextWriter) -> fprintfn writer "%120A" x )
Formatter.SetPreferredMimeTypesFor(typeof<obj>, "text/plain")
Formatter.SetPreferredMimeTypesFor(typeof<GenericChart.GenericChart>,"text/html")
#endif // IPYNB

(**
# Price momentum
Price momentum is one of the most common quant signals. It is (fairly)
straight forward to calculate, and you only need returns to do it,
so it is a good starting point and reference 'strategy'.

## Input data
Create a type that represents the file.
This code figures out what the columns of the csv file are.

- `Sample` is the path to our file. We assume in our source
directory there is a folder called `data`. Inside this data
folder we have our csv file.  
- `ResolutionFolder` to indicate what folder relative paths
  are relative to.

First, let's verify that the csv file exists where we think it will be.
*)
let [<Literal>] ResolutionFolder = __SOURCE_DIRECTORY__

let [<Literal>] CsvFile = "data/msf-momentum.csv"

if IO.File.Exists(ResolutionFolder + "/" + CsvFile) then 
    printfn "Success!!"
else
    let filesThere = IO.Directory.EnumerateFiles(
        ResolutionFolder,
        searchPattern = "*",
        searchOption=IO.SearchOption.AllDirectories)
    printfn "We did not find the file. Here are the files in your source directory.\n"
    filesThere |> Seq.iteri (printfn "%i. %A")

(**
Assuming that you got "Success!!" above this code below will work.
*)
type MsfCsv = CsvProvider<Sample=CsvFile,
                          ResolutionFolder=ResolutionFolder>

// assign the content of the file to a value
let msfCsv = MsfCsv.GetSample()

(** look at the file attributes *)
msfCsv

(** look at the headers *)
msfCsv.Headers

(** look at the first few rows *)
msfCsv.Rows |> Seq.truncate 3

(** Read them into a list. *)
let msfRows = msfCsv.Rows |> Seq.toList

msfRows[..3]

(**
## Signal construction
We want to create a momentum signal and see how it relates to future returns.
The signal is some measure of past returns. A common measure is the past year return,
skipping the most recent month. We skip the most recent month because stocks tend
to reverse following very recent returns (known as "reversals"). 
The reversal is very likely a liquidity effect and it is less important this century.
So returns are positively correlated with returns from 12 months to 1 months ago, 
but negatively correlated with returns last month. This is illustrated very nicely
in Jegadeesh (1990).

If we're forming a portfolio at the end of month $t-1$ to hold in month $t$, 
then we're saying returns in month $t$ are positively correlated
with returns in months $t-12$ through month $t-2$.
For example, if we want to hold a momenum portfolio in January 2021, 
then we will form it on December 31, 2020.
We will want to go long stocks that had high returns from the beginning 
of January 2020 to the end of November 2020.

Let's create a record to hold some info about past returns for a stock.
We will use this as a trading signal.
*)

type PriorReturnOb = 
    { SecurityId : SecurityId
      FormationMonth : DateTime 
      Retm12m2 : float
      N : int }

(**
We're dealing with monthly data.
When we use `DateTime`, then we have to give the month a 
day value. We will give it the first day of the month. Why?

If we use the last day of the month,
then what happens when we add months. For example,
we have to start doing things like.
*)

let endOfFebruary = DateTime(2020,2,28)
let endOfMarch = DateTime(2020, 3, 31)

(** What if we add one month to `endOfFebruary`? *)
let endOfFebruaryPlus1Month = endOfFebruary.AddMonths(1)
endOfFebruaryPlus1Month

(** Hmm. *)

(endOfMarch - endOfFebruaryPlus1Month).Days

(** That's a problem. Instead, if we do it with the first day of the 
month we're ok. 
*)

let beginningOfFebruary = DateTime(2020,2,1)
let beginningOfMarch = DateTime(2020,3,1) 

DateTime(2020,3,1) = (beginningOfFebruary.AddMonths(1)) 

(**
We just have to remember that 2000-01-01 is the
return for the full month of January, from December 31, 1999, until
January 31, 2000. 

Note: the NodaTime library has a `YearMonth` to handle monthly data
without having to specify a day. But we're using `DateTime` to 
keep things slightly simpler.
*)

(**
Let's focus on a single stock.

First few rows
*)
msfRows
|> List.filter(fun x -> x.Ticker = "AMZN")
|> List.truncate 3

(**
Key by security and month
*)
msfRows
|> List.filter(fun x -> x.Ticker = "AMZN")
|> List.map(fun x ->
    let ym = DateTime(x.Month.Year,x.Month.Month, 1) 
    let key = Permno x.Permno, ym
    key, x)
|> List.truncate 3

(**
Assign those to a value and index with a Map collection.
*)
let amznReturns = 
    // we're filtering and then storing as a Map collection
    // that allows us to look up by a key of (permno, yearMonth)
    msfRows
    |> List.filter(fun x -> x.Ticker = "AMZN")
    |> List.map(fun x ->
        let ym = DateTime(x.Month.Year,x.Month.Month, 1) 
        let key = Permno x.Permno, ym
        key, x)
    |> Map

(** Amazon is Permno 84788 *)
let amznPermno = Permno 84788
amznReturns[amznPermno, DateTime(2019,1,1)]

(** A function to get past year of returns. *)
let getPastYearObs returns (security, formationMonth: DateTime) =
        [ -11 .. -1 ]
        |> List.choose(fun i -> 
            let returnMonth = formationMonth.AddMonths(i)
            Map.tryFind (security, returnMonth) returns)    

(** check Permno Amzn *)
getPastYearObs amznReturns (amznPermno, DateTime(2019,1,1))  

(** Check bad data, nothing is returned *)
getPastYearObs amznReturns (Permno -400, DateTime(2019,1,1))  

// making cumulative returns 
let cumulativeReturn rets =
    // using Seq so that it will work with any collection
    let cumulativeLogReturn =
        rets |> Seq.sumBy (fun r -> log (1.0 + r))
    exp(cumulativeLogReturn) - 1.0

(** check result with no data. *)
cumulativeReturn []

(** check up 100\% and then down 50\%.*)
cumulativeReturn [1.0;-0.5]

(** check *)
cumulativeReturn [0.1; 0.1; 0.1; 0.1]

(** compared to *)
1.1 ** 4.0 - 1.0

(**
We're now ready to create our Momentum signal function.
*)

// If you don't want to write the typessecurity, month all the time.
// 

let getMomentumSignal returns (security, formationMonth) =
    let priorObs = getPastYearObs  returns (security, formationMonth)
    let priorRets = priorObs |> List.choose(fun (x:MsfCsv.Row) -> x.Ret)
    // We should probably return None if there are no observations.
    // If they are all missing, List.choose will return an empty
    // array. See:
    // ([ None; None ]: int option list) |> List.choose id
    //
    // So we'll check for an empty array and return None in that case.
    if List.isEmpty priorRets then
        None 
    else
        Some { SecurityId = security 
               FormationMonth = formationMonth
               Retm12m2 = cumulativeReturn priorRets
               N = priorRets.Length }

// Check
getMomentumSignal amznReturns (amznPermno, DateTime(2019,1,1)) 
getMomentumSignal amznReturns (Permno -400, DateTime(2019,1,1))  

(**
One thing you may notice is that our momentum signal function gets everything from
it's inputs. That means that if we give it different intputs then
we could get momentum signals for other stocks. 

For example we can create a map collection like we had for amzn, but for all stocks.
*)

let msfByPermnoMonth =
    msfRows
    |> List.map(fun x ->
        let ym = DateTime(x.Month.Year,x.Month.Month,1) 
        let key = Permno x.Permno, ym
        key, x)
    |> Map

(**
finding some permnos for notable tickers

don't use tickers. companies change tickers, so you might look up the wrong company
That's why I'm picking some tickers that I know haven't changed, but my function
is using PERMNO.
*)

let testTicker = "HOG"
let firstTestTickerRow = 
    msfRows
    |> List.find (fun row -> row.Ticker = testTicker)

firstTestTickerRow.Ticker, firstTestTickerRow.Permno

(**
Now do for a list of tickers
*)

let notableTicks =
    [ for ticker in ["MSFT";"AAPL";"HOG"] do
        let firstTickerRow =
            msfRows
            |> List.find (fun row -> row.Ticker = ticker)
        firstTickerRow.Ticker, firstTickerRow.Permno ]
    |> Map

notableTicks

(** 
wrap integer ticker in `Permno` tag 
to make it have type `SecurityId`.
*)

let msftPermno = Permno notableTicks["MSFT"]
let aaplPermno = Permno notableTicks["AAPL"]
let hogPermno = Permno notableTicks["HOG"]

msftPermno, aaplPermno, hogPermno

(**
Creating a tuple of (permno, yearMonth) that
we can use for looking up things in that month
for that ticker.
*)
let msftTestIndex = (msftPermno, DateTime(2019,1,1))
let aaplTestIndex = (aaplPermno, DateTime(2019,1,1))  

msftTestIndex, aaplTestIndex

(**
Microsoft momentum signal:
*)
getMomentumSignal msfByPermnoMonth msftTestIndex 

(**
Apple momentum signal:
*)
getMomentumSignal msfByPermnoMonth aaplTestIndex  

(**
and we can use [partial function application](https://fsharpforfunandprofit.com/posts/partial-application/)
to "bake in" the msfByPermnoMonth parameter so that we don't keep having to pass it around.
*)

let getMomentumSignalAny = getMomentumSignal msfByPermnoMonth

(**
Microsoft again:
*)
getMomentumSignalAny msftTestIndex 

(**
Apple again:
*)
getMomentumSignalAny aaplTestIndex  

(**
## Defining the investment universe

Let's say we have a portfolio formation month. Can we look up securities available to invest in?

*)

let securitiesByFormationMonth =
    let byYearMonth =
        msfRows
        |> List.groupBy (fun x -> DateTime(x.Month.Year, x.Month.Month,1))
    [ for (yearMonth, stocksThatMonth) in byYearMonth do 
        let permnos = [ for stock in stocksThatMonth do Permno stock.Permno ]
        yearMonth, permnos ]
    |> Map

let getInvestmentUniverse formationMonth =
    match Map.tryFind formationMonth securitiesByFormationMonth with
    | Some securities -> 
        { FormationMonth = formationMonth 
          Securities = securities }
    | None -> failwith $"{formationMonth} is not in the date range"      

getInvestmentUniverse (DateTime(2011,10,1))
// getInvestmentUniverse (YearMonth(1990,10))

(**
You might also want to filter the investment universe by some criteria.
*)


let isCommonStock securityFormationMonth =
    match Map.tryFind securityFormationMonth msfByPermnoMonth with
    | None -> false
    | Some x -> List.contains x.Shrcd [10; 11]

let onNyseNasdaqAmex securityFormationMonth =
    match Map.tryFind securityFormationMonth msfByPermnoMonth with
    | None -> false
    | Some x -> List.contains x.Exchcd [ 1; 2; 3]

let hasPrice13mAgo (security, formationMonth:DateTime) =
    //13m before the holding month, 12m before the formation month
    match Map.tryFind (security, formationMonth.AddMonths(-12)) msfByPermnoMonth with
    | None -> false
    | Some m13 -> m13.Prc.IsSome

let hasReturn2mAgo (security, formationMonth:DateTime) =
    //2m before the holding month, 1m before the formation month
    match Map.tryFind (security, formationMonth.AddMonths(-1)) msfByPermnoMonth with
    | None -> false
    | Some m2 -> m2.Ret.IsSome

let hasMe1mAgo (security, formationMonth) =
    //1m before the holding month, so the formation month
    match Map.tryFind (security, formationMonth) msfByPermnoMonth with
    | None -> false
    | Some m1 -> m1.Prc.IsSome && m1.Shrout.IsSome

let has8ReturnsPastYear securityFormationMonth =
    match getMomentumSignalAny securityFormationMonth with 
    | None -> false 
    | Some x -> x.N >= 8

let danielMoskowitzRestrictions securityFormationMonth =
    isCommonStock securityFormationMonth &&
    onNyseNasdaqAmex securityFormationMonth &&
    hasPrice13mAgo securityFormationMonth &&
    hasReturn2mAgo securityFormationMonth &&
    hasMe1mAgo securityFormationMonth &&
    has8ReturnsPastYear securityFormationMonth 

let restrictUniverse (investmentUniverse: InvestmentUniverse) =
    let filtered =
        investmentUniverse.Securities
        |> List.filter(fun security -> 
            danielMoskowitzRestrictions (security, investmentUniverse.FormationMonth))
    { FormationMonth = investmentUniverse.FormationMonth
      Securities = filtered }        

(**
Now we can see where we are.
*)

DateTime(2011,10,1)
|> getInvestmentUniverse
|> restrictUniverse

let investmentUniverse =
    DateTime(2011,10,1)
    |> getInvestmentUniverse
let restrictedInvestmentUniverse =
    investmentUniverse |> restrictUniverse

// See if we're excluding some securities.
investmentUniverse.Securities.Length
restrictedInvestmentUniverse.Securities.Length

(**
## Momentum signals for our investment universe
Let's look at how to transform our array of securities in our investment
universe into an array with the signals.

Recall that our momentum function returns a type of observation specific to momentum.
*)

getMomentumSignalAny (Permno 84788, DateTime(2019,1,1)) 

(**
This is fine, but if we want our code to work with any type of signal,
then we need to transform it into something more generic.

This is the purpose of the `SecuritySignal` record in the `Portfolio` module.
It's the same thing that we had in the simpler portfolio formation example.
It can represent any signal that is a float.
And to hold an array of security signals for a particular month,
we now have the type `SecuritiesWithSignals` also from the `Portfolio` module.

Let's write a function that transforms our momentum signal into a more generic
security signal.
*)

let getMomentumSecuritySignal (security, formationMonth ) =
    match getMomentumSignalAny (security, formationMonth) with
    | None -> None
    | Some signalOb ->
        let signal = { SecurityId = security; Signal = signalOb.Retm12m2 }
        Some signal

(** Now compare *)
getMomentumSignalAny (Permno 84788, DateTime(2019,1,1)) 

(** to *)
getMomentumSecuritySignal (Permno 84788, DateTime(2019,1,1))

(**
Now a function that takes our investment universe and returns
our securities with their (now more generic) signal.
*)

let getMomentumSignals (investmentUniverse: InvestmentUniverse) =
    let arrayOfSecuritySignals =
        investmentUniverse.Securities
        |> List.choose(fun security -> 
            getMomentumSecuritySignal (security, investmentUniverse.FormationMonth))    
    { FormationMonth = investmentUniverse.FormationMonth 
      Signals = arrayOfSecuritySignals }

restrictedInvestmentUniverse
|> getMomentumSignals


(** or, if we want to look at the full pipeline. *)
DateTime(2015,7,1)
|> getInvestmentUniverse
|> restrictUniverse
|> getMomentumSignals


(**
## Assigning portfolios
Now that we have the signals for our portfolio,
we can assign portfolios. For many strategies it
is common to use decile sorts. This means that
you sort securities into 10 portfolios based on the signal.
But other numbers of portfolios (tercile = 3, quintile = 5, etc)
are also common.

There's a tradeoff between signal strength and diversification.
More portfolios means that top/bottom portfolios are stronger bets
on the signal. But there are fewer securities,
so they are also less diversified. Often, the trade-off between
a stronger signal vs. less diversificaton balances out.
Specifically, long-short tercile sorts may have a lower return spread
than decile sorts. But since terciles are better diversified,
the tercile and decile shorts are not as different when looking 
at sharpe ratios.

The `Portfolio` module has a function named `assignSignalSorts`
to form portfolios by sorting securities into `n` groups.
*)

let mom201507 =
    DateTime(2015,7,1)
    |> getInvestmentUniverse
    |> restrictUniverse
    |> getMomentumSignals
    |> assignSignalSort "Momentum" 10

[ for port in mom201507 do 
    $"My name is {port.PortfolioId} and I have {port.Signals.Length} stocks" ]
(**
## Calculating Portfolio weights

We'll use a value-weight scheme. So we need a function that gets market capitalizations.

*)

let getMarketCap (security, formationMonth) =
    match Map.tryFind (security, formationMonth) msfByPermnoMonth with
    | None -> None
    | Some x -> 
        match x.Prc, x.Shrout with 
        // we need valid price and valid shrout to get market caps
        | Some prc, Some shrout -> Some (security, prc*shrout)
        // anything else and we can't calculate it
        | _ -> None

(**
Some examples.
*)

(**
Amazon market cap:
*)
getMarketCap (amznPermno, DateTime(2019,1,1))

(**
Amazon future market cap, returns none:
*)
getMarketCap (amznPermno, DateTime(2030,1,1))

(** 
Now for a list of securities:
*)
[ getMarketCap (amznPermno, DateTime(2019,1,1))
  getMarketCap (hogPermno, DateTime(2019,1,1)) ]

(**
Using List.choose will unwrap the option type.

Compare this
*)
[ getMarketCap (amznPermno, DateTime(2019,1,1))
  None
  getMarketCap (hogPermno, DateTime(2019,1,1)) ]

(** to this *)

[ getMarketCap (amznPermno, DateTime(2019,1,1))
  None
  getMarketCap (hogPermno, DateTime(2019,1,1)) ]
|> List.choose id // id is a special build-in function: `let id x = x`

(**
Assign our example capitalizations to a value
*)
let exampleCapitalizations =
    [ getMarketCap (amznPermno, DateTime(2019,1,1))
      getMarketCap (hogPermno, DateTime(2019,1,1)) ]
    |> List.choose id 

exampleCapitalizations

(** Now the value weights *)
let exampleValueWeights =
    let tot = exampleCapitalizations |> List.sumBy snd
    exampleCapitalizations
    |> List.map(fun (_id, cap) -> cap / tot )
    |> List.sortDescending

exampleValueWeights

(**
Now imagining we have the same example in terms of an assigned portfolio
with made up signals.
*)

let mktCapExPort: AssignedPortfolio =
    let signals =
        [ { SecurityId = amznPermno; Signal = 1.0 }
          { SecurityId = hogPermno; Signal = 1.0 } ]  
    { PortfolioId = Named "Mkt Cap Example"
      FormationMonth = DateTime(2019,1,1)
      Signals = signals }

(**
The portfolio module has a function that can help us.
It has two inputs.

- A function that gets market capitalizations
- An assigned portfolio

We should see that it gives the same value weights.
*)

let exampleValueWeights2 =
    giveValueWeights getMarketCap mktCapExPort

exampleValueWeights2

(**
It can be more convenient to "bake in" my function to get market caps:
*)

let myValueWeights = giveValueWeights getMarketCap

myValueWeights mktCapExPort

(**
So now we can construct our portfolios with value weights.
*)

let portfoliosWithWeights =
    let assignedPortfolios =
        DateTime(2015,7,1)
        |> getInvestmentUniverse
        |> restrictUniverse
        |> getMomentumSignals
        |> assignSignalSort "Momentum" 10

    [ for portfolio in assignedPortfolios do
        myValueWeights portfolio ]

(**
Note that because of the size distribution,
some of these portfolios are not very diversified.
This is illustrated by inspecting maximum portfolio
weights.
*)

[ for port in portfoliosWithWeights do
    let maxWeight = 
        [ for position in port.Positions do 
            position.Weight ] 
        |> List.max
    let totalWeights =
        [ for position in port.Positions do 
            position.Weight ]
        |> List.sum    
    port.PortfolioId.ToString() , maxWeight ]
|> Chart.Bar

(**
## Calculating Portfolio returns

We need our function to get returns given weights.

We can start with a function that gets returns.
It looks a lot like our function to get market capitalizations.

*)

let getSecurityReturn (security, formationMonth) =
    // If the security has a missing return, assume that we got 0.0.
    // Note: If we were doing excess returns, we'd need 0.0 - rf
    let missingReturn = 0.0
    match Map.tryFind (security, formationMonth) msfByPermnoMonth with
    | None -> security, missingReturn
    | Some x ->  
        match x.Ret with 
        | None -> security, missingReturn
        | Some r -> security, r

getSecurityReturn (amznPermno, DateTime(2019,1,1))        

let portReturn =
    getPortfolioReturn getSecurityReturn exampleValueWeights2

portfoliosWithWeights
|> List.map (getPortfolioReturn getSecurityReturn)    

(** baking in the getSecurityReturn function *)

let myPortfolioReturns = getPortfolioReturn getSecurityReturn

portfoliosWithWeights
|> List.map myPortfolioReturns


// Put it all together.
let sampleMonths = getSampleMonths (DateTime(2010,5,1), DateTime(2020,2,1)) 

sampleMonths |> List.rev |> List.take 3

(**
These two functions below produce the same result. The first assigns each function
result to an intermediate value. The second uses a pipeline.
Either style is fine, though while I may start with something like the first
version while writing test code, I typically use the pipeline 
style shown in in the second version.
*)
let formMomtenumPortWithAssignments ym =
    let universe = getInvestmentUniverse ym
    let restricted = restrictUniverse universe
    let signals = getMomentumSignals restricted
    let assignedPortfolios = assignSignalSort "Momentum" 10 signals
    let portfoliosWithWeights =
        [ for portfolio in assignedPortfolios do
            myValueWeights portfolio ]
    let portfolioReturns =
        [ for portfolio in portfoliosWithWeights do
            myPortfolioReturns portfolio ]
    portfolioReturns


let formMomentumPort ym =
    ym
    |> getInvestmentUniverse
    |> restrictUniverse
    |> getMomentumSignals
    |> assignSignalSort "Momentum" 10
    |> List.map myValueWeights
    |> List.map myPortfolioReturns


(**
We can process months sequentially.
*)
(*** do-not-eval ***)
let momentumPortsSequential =
    sampleMonths
    |> List.collect formMomentumPort

(**
Or we can speed things up and process months in Parallel using
all available CPU cores. Note that the only changes are to 

1. Use `Array.Parallel.collect` intead of 
`List.collect`. The array collection is the only parallel 
collection in base F# and the module functions are somewhat limited, 
but if you google F# parallel processing
you can find other options and also asynchronous coding and PSeq 
(I mostly use PSeq for my parallel code).
2. `Array.Parallel.collect` expects to operate on arrays, so 
we need to have our portfolio returned as arrays instead of lists.
This is what we are doing with `formMomentumPortArray`.
*)

(***do-not-eval***)
let formMomentumPortArray ym =
    ym 
    |> formMomentumPort 
    |> List.toArray

let momentumPortsParallel =
    sampleMonths
    |> List.toArray
    |> Array.Parallel.collect formMomentumPortArray 

(**
## Plotting returns

Let's look at the cumulative returns of the portfolios.

To start, get the top momentum portfolio.
*)

let mom10 = 
    momentumPortsSequential
    |> List.filter (fun p -> 
        p.PortfolioId = Indexed {| Index = 10; Name = "Momentum" |})
    |> List.sortBy (fun p -> p.YearMonth )

// first few months
mom10[0..3]

(** for easier accumulation make log returns. *)
let mom10LogReturns = 
    [ for monthObs in mom10 do 
        {  monthObs with Return = log(1.0 + monthObs.Return) }]

mom10LogReturns[..3]

(**
Now calculate cumulative log returns using a sum.

We're going to use [scan](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-listmodule.html#scan). The idea with `List.scan` is that you write a function that takes two inputs: 1) the prior state, 2) the current observation. With those two inputs the function returns a new state.

For example, for a running total of observations, the prior state is the prior total, the current observation is what you're going to add to the prior total to get the new total. For example: 
*)      

let itemsToAdd = [0; 1; -7; 4; 10]

(** The first item, or "head" of the list. *)
itemsToAdd[0]

(** The items after the first, the "tail" of the list. *)

itemsToAdd[1..]

(** The running total, aka cumulative sum: *)

(itemsToAdd[0],itemsToAdd[1..])
||> List.scan (fun acc x -> acc + x)

(** One nice things bout lists is that they are efficient to add/remove things from the front of the list, and there's a special [`::` (cons) operator](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/lists#operators-for-working-with-lists) for doing this. It works for construction and deconstruction. 

Construction:
*)
0::itemsToAdd[1..]

(**
Deconstruction using pattern matching:
*)

let headOfList::tailOfList = itemsToAdd

(** the head *)
headOfList

(** the tail*)
tailOfList

(** So this would also work: *)
(headOfList, tailOfList)
||> List.scan (fun acc x -> acc + x)

(**
We can use this to calculate cumulative returns.
*)

let mom10CumulativeLogReturns =
    let h::t = mom10LogReturns
    (h, t)
    ||> List.scan (fun priorMonth thisMonth -> 
        { thisMonth with Return = thisMonth.Return + priorMonth.Return })

(**
The first cumulative return is just the first return:
*)

printfn $"The first return is:            %.4f{mom10LogReturns[0].Return}"
printfn $"The first cumulative return is: %.4f{mom10CumulativeLogReturns[0].Return}"


(**
But subsequently the cumulative returns reflect the running total:
*)

[ for month in mom10CumulativeLogReturns do
    month.YearMonth, month.Return ]
|> Chart.Line
|> Chart.withTitle "Momentum Portfolio 10"

(**
We can also plot all 10 portfolios.
*)

let makeCumulativeLogReturn portfolioObs =
    let headLogReturn::tailLogReturns = 
        portfolioObs
        |> List.sortBy (fun p -> p.YearMonth )
        |> List.map (fun p -> 
            { p with Return = log(1.0 + p.Return) })
    
    let cumulativeLogReturn =
        (headLogReturn, tailLogReturns)
        ||> List.scan (fun acc thisMonth -> 
            { thisMonth with Return = thisMonth.Return + acc.Return })
    cumulativeLogReturn

// test it
let testMom10 = makeCumulativeLogReturn mom10

testMom10 = mom10CumulativeLogReturns // should evaluate to true

(** Now let's use that function to do it for all 10 portfolios.*)

let byPortfolioId =
    momentumPortsSequential
    |> List.groupBy (fun p -> p.PortfolioId)

let listOfCharts =
    [ for (portId, portObs) in byPortfolioId do
        let cumulativeLogReturn = makeCumulativeLogReturn portObs
        let plotData =
            [ for month in cumulativeLogReturn do
                month.YearMonth, month.Return ]
        Chart.Line(plotData, Name = portId.ToString()) ]

(** Plot one of them.*)

listOfCharts[0]

(** Plot all of them. *)
listOfCharts
|> Chart.combine