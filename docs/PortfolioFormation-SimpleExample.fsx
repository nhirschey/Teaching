(**

---
title: Building a strategy
category: Lectures
categoryindex: 1
index: 7
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

# Building a strategy

Our objective is to write code that will allow us to do something like

```fsharp
samplePeriod
|> filterInvestmentUniverse
|> constructSignals
|> assignPortfolioWeights
|> getPortfolioReturns
```

Now let's dig in. This is a *relatively* basic version of a strategy to build intuition with what we're doing. It is simpler than working with real data. But we will next move to real data.
*)

#r "nuget: FSharp.Stats"
open FSharp.Stats

(**
## Workflow for using quantitative signals

How do we decide on the position weights for the different securities? An example workflow:

1. **Define your investment universe.** Your investment universe is the set of securities that you are considering for investment. An investment universe is typically a subset of securities grouped by some common characteristics: small-capitalization stocks, utility stocks, corporate bonds, firms listed on Euronext, etc.
2. **Get your signal for each security.** The signal is some information that you believe can be used to predict returns or risk. It could be a stock tip from a friend, a stock analyst recommendation, something you read on reddit, revenue growth, ... 
3. **Define a mapping from signals to portfolio weights.** 

Let's go through this step by step.

*)

(**

## 1. Investment universe: modelling a security

We're going to be forming portfolios consisting of weights on different securities. We need a way to identify those securities. Securities can have different identifiers. Stock exchanges use ticker symbols, but CUSIPs are commonly used by other entities (e.g., ratings agencies, regulators, etc.). Some data providers might also use their own custom identifier. For example, Bloomberg has their own "Bloomberg ticker" and CRSP uses PERMNOs. Thus, if we're trying to identify a security we can use tickers OR CUSIPs OR Bloomberg Tickers OR PERMNOs. How do we model this?

A good type for modelling "OR" relationships is a Discriminated Union. The pipe (`|`) symbol indicates an "or" relationship. 
*)
type SecurityId =
    | Ticker of string
    | Cusip of string
    | Bloomberg of string
    | Permno of int


// Defining examples of these ids.
let tickerExample = Ticker "KO"
let permnoExample = Permno 1001


// Deconstructing using pattern matching.
// Think "match what's on the left side of = to what's on the right side"
let (Ticker deconTickerExample) = tickerExample
printfn "%s" deconTickerExample

let (Permno deconPermnoExample) = permnoExample
printfn "%i" deconPermnoExample

// Now we can define our investment universe.
let investmentUniverse =
    [for tick in [ "AAPL"; "KO"; "GOOG";"DIS";"GME"] do 
        Ticker tick ]

(**
## 2. Signals
*)

let signals =
    [ Ticker "AAPL", 2.0
      Ticker "KO", -1.4
      Ticker "GOOG", 0.4 
      Ticker "DIS", 1.1 ]
    |> Map

(***do-not-eval***)
signals[Ticker "AAPL"]
(** *)

Map.find (Ticker "AAPL") signals
(***include-it***)
Map.tryFind (Ticker "AAPL") signals
(***include-it***)
Map.tryFind (Ticker "GME") signals
(***include-fsi-output***)

(** Now a function that gets the signal given a security id.*)
let getSignal id = Map.tryFind id signals

getSignal (Ticker "AAPL")
getSignal (Ticker "GOOG")

(**
We can call this function on all signals in our investment universe.
*)
// using a loop.
[ for security in investmentUniverse do
    security, getSignal security ]

// same thing with List.map
investmentUniverse
|> List.map(fun security -> security, getSignal security)

(** Let's create a type to hold a security identifier and its signal.*)
type SecuritySignal = 
    { SecurityId : SecurityId
      Signal : float }

(** 
Now a function that gets the signal and puts it in the 
`SecuritySignal` record type. It's possible that the security signal does not exist, so we'll use an option type to handle the fact that we might
find the signal and we might not.
*)
let getSecuritySignal security =
    match Map.tryFind security signals with
    | None -> None
    | Some signal ->
        let result = { SecurityId = security; Signal = signal }
        Some result


(** 
Here we can see that we retured something when there
was a signal and nothing if there was none.
*)
getSecuritySignal (Ticker "GME")
(*** include-fsi-output ***)
getSecuritySignal (Ticker "GOOG")
(*** include-fsi-output ***)

for security in investmentUniverse do
    let securitySignal = getSecuritySignal security
    printfn $"{securitySignal}"
(***include-output***)

(** This is equivalent but using a pipeline. *)
investmentUniverse
|> List.map getSecuritySignal
|> List.iter (printfn "%A")
(*** include-output ***)

(**
If we do choose instead of map, then we will
end up with only the results when there was something.
*)
investmentUniverse
|> List.choose getSecuritySignal
|> List.iter (printfn "%A") 
(*** include-output***)

let securitySignals = 
    investmentUniverse
    |> List.choose getSecuritySignal

securitySignals
|> List.iter (printfn "%A")
(*** include-output***)

(**
## 3. Defining a mapping between signals and portfolio weights.

Now let's look at turning signals into weights.

Often, you only want stocks with signals above or below a given threshold in a portfolio. For instance, if you have a "value" portfolio you might only want stocks with low price to earnings (P/E) ratios in your portfolio. Or maybe you want to go long value stocks and short growth stocks.

A typical procedure is to assign securities to portfolios based on signals, and then weight securities within those sub portfolios.

*)

(**

First, let's represent portfolios Ids.
A first step is to define portfolio IDs. A simple ID is a string name, but often we will do things like create 10 size portfolios infexed from 1 to 10, like ("Size", 1), ("Size", 2), ... We can model this as a discriminated union. 

Here, `Indexed` is a tuple where the first element is a string and the second is an integer. I could just say `Indexed of string * int`, but I am going to name them to give them meaning. Though the names are optional when constructing and deconstructing
*)

type PortfolioId = 
    | Named of string
    | Indexed of {| Name: string; Index: int |}// name:string * index:int

// Example portfolio IDs
let portfolioId1 = Named "Market"
let portfolioId2 = Indexed {| Name = "Size"; Index = 1 |} 
let portfolioId3 = Indexed {| Name = "Size" ; Index = 2 |}    

let getPortfolioIdString port =
    match port with
    | Named name -> name
    | Indexed p -> $"{p.Name}: {p.Index}"


getPortfolioIdString portfolioId1
(***include-it***)
getPortfolioIdString portfolioId3
(***include-it***)

(**
Let's assign securities to portolios based on whether their signal is above or below the median.
*)

// Model for an assigned portfolio
type AssignedPortfolio =
    { PortfolioId : PortfolioId 
      Signals : list<SecuritySignal> }

let medianSignal = 
    securitySignals 
    |> List.map(fun x -> x.Signal)
    |> Seq.median
(** The median signal is *)
(***include-value:medianSignal***)


let aboveMedian =
    securitySignals
    |> List.filter(fun x -> x.Signal >= medianSignal)
(** the above-median securities: *)
(***include-value:aboveMedian***)

let belowMedian =
    securitySignals
    |> List.filter(fun x -> x.Signal < medianSignal)
(** the below-median securities: *)
(***include-value:belowMedian***)


let assigned =
    [ { PortfolioId = Named("Above Median")
        Signals = aboveMedian }
      { PortfolioId = Named("Below Median")
        Signals = belowMedian} ]
(** assigned to portfolios: *)
(***include-value:assigned***)

(**
Or create a reusable function to do the same thing
*)

let assignAboveBelowMedian securitySignals =
    let medianSignal = 
        securitySignals 
        |> List.map(fun x -> x.Signal)
        |> Seq.median

    let aboveMedian =
        securitySignals
        |> List.filter(fun x -> x.Signal >= medianSignal)

    let belowMedian =
        securitySignals
        |> List.filter(fun x -> x.Signal < medianSignal)

    [ { PortfolioId = Named("Above Median")
        Signals = aboveMedian }
      { PortfolioId = Named("Below Median")
        Signals = belowMedian} ]


(**
## Modelling a position
Now we have assigned securities to portfolios based on the trading signal. Now we can form weights. We can think of a portfolio as consisting of positions where positions are symbols and weights.
*)

type Position =
    { SecurityId : SecurityId 
      Weight : float }

// Defining example positions

let koPosition = { SecurityId = Ticker "KO"; Weight = 0.25 }
let permnoPosition = { SecurityId = Permno 1001; Weight = 0.75 }

(**
## Modelling a portfolio
And once we have multiple positions, then we can group them into a portfolio.
*)

(** And a portfolio can consist of a Portfolio Id and an List of positions*)

type Portfolio = 
    { PortfolioId: PortfolioId
      Positions : list<Position> }

(**
An example constructing a portfolio
*)

let portfolioExample1 =
    { PortfolioId = Named "Example 1"
      Positions = [ koPosition; permnoPosition ] }

(**
## Defining portfolio position weights

Once you have a portfolio of securities that have met some signal criteria, it is common to weight those securities using either of two simple weighting schemes: equal weight or value weight.

Equal weight means that every security has the same weight.

Value-weight means that you weight securities proportional to their market value. This means that your portfolio put more weight on more valuable securities. Or it "tilts" toward more valuable securities. This weighting scheme is utilized when you want to make sure that you are not putting too much weight on small illiquid securities that are hard to purchase.


Equal-weight is easy:
*)
let weightTestPort = 
    assigned |> List.find (fun x -> x.PortfolioId = Named("Above Median"))

let nSecurities = weightTestPort.Signals.Length

let ewTestWeights =
    [ for signal in weightTestPort.Signals do 
        { SecurityId = signal.SecurityId
          Weight = 1.0 / (float nSecurities) } ]

let giveEqualWeights x =
    let n = x.Signals.Length
    let pos =
        [ for signal in x.Signals do 
            { Position.SecurityId = signal.SecurityId
              Weight = 1.0 / (float n) } ]
    { PortfolioId = x.PortfolioId 
      Positions = pos }

(** For one portfolio: *)
giveEqualWeights weightTestPort
(***include-fsi-output***)

(** For all portfolios:*)
[ for portfolio in assigned do giveEqualWeights portfolio ]
(***include-fsi-output***)

(** or equivalently:*)
assigned |> List.map giveEqualWeights
(***include-fsi-output***)

(** 
For value weights, we need the securities' market values
split into above/below median and for portfolios with those.
*)
let marketCapitalizations =
    [ Ticker "AAPL", 10.0
      Ticker "KO", 4.0
      Ticker "GOOG", 7.0 
      Ticker "DIS", 5.0 ]
    |> Map

let mktCaps =
    [ for signal in weightTestPort.Signals do 
        let mktcap = Map.find signal.SecurityId marketCapitalizations
        signal.SecurityId, mktcap ]

(***include-fsi-output***)

let vwTestWeights =
    let totMktCap = mktCaps |> List.sumBy snd
    [ for (id, mktCap) in mktCaps do 
        { SecurityId = id 
          Weight = mktCap / totMktCap } ]
(***include-fsi-output***)

(**
Now a function to do the same as above.
*)
let giveValueWeights x =
    let mktCaps =
        [ for signal in x.Signals do 
            let mktcap = Map.find signal.SecurityId marketCapitalizations
            signal.SecurityId, mktcap ]
    let totMktCap = mktCaps |> List.sumBy snd
    let pos =
        [ for (id, mktCap) in mktCaps do 
            { SecurityId = id 
              Weight = mktCap / totMktCap } ]
    { PortfolioId = x.PortfolioId; Positions = pos }

(**
We can map our function to both of our assigned portfolios.
*)

[ for x in assigned do giveValueWeights x ]
(***include-it***)

(** or equivalently *)
assigned |> List.map giveValueWeights  
(***include-it***)

(**
All together now. This is our workflow.
*)
let strategyWeights =
    investmentUniverse
    |> List.choose getSecuritySignal
    |> assignAboveBelowMedian
    |> List.map giveValueWeights

(*** include-fsi-output***)

(**
# How do we calculate returns?


Take these returns:
*)
let returnMap =
    [ Ticker "AAPL", -0.4
      Ticker "KO", -0.1
      Ticker "GOOG", 0.15 
      Ticker "DIS", 0.1 ]
    |> Map

(**
What is the return of the two portfolios?

Hint: the value-weight assignment code is a good reference for figuring out how to look up the stock returns.
*)