(**
---
title: Dicriminated Union
category: Practice Quizzes
categoryindex: 1
index: 5
---
*)

(*** hide ***)
/// example fast binder url: https://mybinder.org/v2/gh/fsprojects/fsharp.formatting/master?urlpath=git-pull?repo=https:/nhirschey.github.com/teaching/gh-pages/fundamentals.ipynb

(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](../img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)
*)

(*** hide,define-output:preDetails ***)
"""
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

"""

(*** hide,define-output:postDetails ***)
"""

</details>
</span>
</p>
</div>
"""

(** 

This practice quiz emphasizes `Discriminated Unions`. They are useful for times when the data that you're representing has multiple mutually exclusive cases. 

Here is some good background reading for before you do these quesitions, particularly the F# for fun and profit link.

- Discriminated Union types

    - The F# language reference for [discriminated unions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions)
    - If you want more a more in depth discussion, see F# for fun and profit's section on [discriminated unions](https://fsharpforfunandprofit.com/posts/discriminated-unions/)
    - The tour of F# section on [discriminated unions](https://docs.microsoft.com/en-us/dotnet/fsharp/tour#record-and-discriminated-union-types)

*)

(**
## Question 1
Create a discriminated union named `Action` with two cases: Buy and Sell.

1. Create a value named 'bAction' and assign `Buy` to it.
2. Create a value named 'sAction' and assign `Sell` to it.
*)

(*** include-it-raw:preDetails ***)
(*** define: createBuySell, define-output: createBuySell ***)

type Action = 
    | Buy 
    | Sell
let bAction = Buy
let sAction = Sell

(*** condition:html, include:createBuySell ***)
(*** condition:html, include-fsi-output:createBuySell ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Create a single case discriminated union to represent a particular kind of string:

1. Create a discriminated union named Ticker with a single case Ticker of string. 
2. Then wrap the string "ABC" in your Ticker type and assign it to a value named 'aTicker'.
3. Then use pattern matching to unwrap the string in `aTicker` and assign it to a value named `aTickerString`.

Discriminated unions like this are usful if you want to make 
sure that you don't accidentally mix up two strings that represent
different things.
A function that takes an input with type `Ticker` will not accept any string,
it will only accept inputs that have time Ticker.
*)

(*** include-it-raw:preDetails ***)
(*** define: singleString, define-output: singleString ***)

type Ticker = Ticker of string
let aTicker = Ticker "ABC"
let (Ticker aTickerString) = aTicker

(*** condition:html, include:singleString ***)
(*** condition:html, include-fsi-output:singleString ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Create a single case discriminated union to represent a particular kind of float:

1. Create a discriminated union named Signal with a single case Signal of float. 
2. Then wrap the string float `1.0` in your Signal type and assign it to a value named 'aSignal'.
3. Then use pattern matching to unwrap the float in `aSignal` and assign it to a value named `aSignalFloat`.
*)

(*** include-it-raw:preDetails ***)
(*** define: singleFloat, define-output: singleFloat ***)

type Signal = Signal of float
let aSignal = Signal 1.2
let (Signal aSignalFloat) = aSignal

(*** condition:html, include:singleFloat ***)
(*** condition:html, include-fsi-output:singleFloat ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 4
Create a discriminated union called called `Funds` with two cases: MutualFund of string and HedgeFund of string.

1. Create a MutualFund case of the Fund union with the string "Fidelity Magellan". Assign it to a value named "magellan".
2. Create a HedgeFund case of the Fund union with the string "Renaissance Medallion". Assign it to a value named "renaissance".

*)  

(*** include-it-raw:preDetails ***)
(*** define: twoCaseString, define-output: twoCaseString ***)

type Funds =
    | MutualFund of string
    | HedgeFund of string

let magellan = MutualFund "Fidelity Magellan"
let renaissance = HedgeFund "Renaissance Medallion"

(*** condition:html, include:twoCaseString ***)
(*** condition:html, include-fsi-output:twoCaseString ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 5
Define two types with the same cases.

```fsharp
type Ambiguous1 = Up | Down
type Ambiguous2 = Up | Down
```
If you try to assign Ambiguous1 to values, 
it can be hard for the compiler (and yourself)
to figure out which of these types you mean. If you write `Up`
the compiler will think that you meant to use whatever was defined
last (Ambigous2).

Use fully qualified names to show how to assign the `Up` case from Ambiguous1 to
a value named `ambiguous1` and the `Up` case from Ambiguous2 to a value named
`ambiguous2`. 
*)

(*** include-it-raw:preDetails ***)
(*** define: clarifyAmbiguity, define-output: clarifyAmbiguity ***)

type Ambiguous1 = Up | Down
type Ambiguous2 = Up | Down
let ambiguous1 = Ambiguous1.Up
let ambiguous2 = Ambiguous2.Up

(*** condition:html, include:clarifyAmbiguity ***)
(*** condition:html, include-fsi-output:clarifyAmbiguity ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 6
Imagine that analyst recommendations have the form
```fsharp
type AnalystRec = Buy | Sell | Hold
```
You have recommendations from two analysts
```fsharp
let goldmanRec = Buy
let barclaysRec = Sell
```
You want to act on goldman recommendations as follows:
```fsharp
let actionOnGoldman (x: AnalystRec) =
    match x with
    | Buy | Hold -> "I am buying this!"
    | Sell -> "I am selling this!"
```

The problem is that this `actionOnGoldman` function will
work for both `goldmanRec` and `barclaysRec`.
```fsharp
actionOnGoldman goldmanRec // evaluates to "I am buying this!"
actionOnGoldman barclaysRec // evaluates to "I am selling this!"
```

1. Create a single case union called `GoldmanRec` where the single case
is GoldmanRec of AnalystRec. 
2. Create a modified `actionOnGoldman` function called `actionOnGoldmanOnly` so that it will only work on recommendations with the type `GoldmanRec`.

If `wrappedGoldmanRec` is buy `GoldmanRec`, the result should be
```
actionOnGoldmanOnly wrappedGoldmanRec // evaluates to "I am buying this!"
actionOnGoldmanOnly barclaysRec // compiler error.
```
*)

(*** include-it-raw:preDetails ***)
(*** define: funForSingleCase, define-output: funForSingleCase ***)

type AnalystRec = Buy | Sell | Hold
type GoldmanRec = GoldmanRec of AnalystRec
let goldmanRec = Buy
let barclaysRec = Sell
let actionOnGoldman (x: AnalystRec) =
    match x with
    | Buy | Hold -> "I am buying this!"
    | Sell -> "I am selling this!"
actionOnGoldman goldmanRec // what we want.
actionOnGoldman barclaysRec // oops.
// constructing it from scratch.
let wrappedGoldmanRec = GoldmanRec Buy
// or, wrapping our previously created value
let wrappedGoldmanRec2 = GoldmanRec goldmanRec
wrappedGoldmanRec = wrappedGoldmanRec2 // true
// constructing it from scratch
let actionOnGoldmanOnly (x: GoldmanRec) =
    match x with
    | GoldmanRec Buy | GoldmanRec Hold -> "I am buying this!"
    | GoldmanRec Sell -> "I am selling this!"
// or, unwrapping the GoldmanRec with pattern matching
// in the input definition:
let actionOnGoldmanOnly2 (GoldmanRec x) =
    // Since we unwrapped the goldman recommendation,
    // now it is just the inner analyst recommendation.
    // We can leave off the GoldmanRec that was wrapping the
    // recomendation.
    match x with
    | Buy | Hold -> "I am buying this!"
    | Sell -> "I am selling this!"
// or, since you see above that once we unwrap the goldman rec,
// it is the same as our orignal function.
let actionOnGoldmanOnly3 (GoldmanRec x) = actionOnGoldman x

// check
actionOnGoldmanOnly wrappedGoldmanRec
actionOnGoldmanOnly2 wrappedGoldmanRec
actionOnGoldmanOnly3 wrappedGoldmanRec
// These would all give compiler errors. 
// Uncomment them (delete the // at the start) to test them yourself.
//actionOnGoldmanOnly barclaysRec
//actionOnGoldmanOnly2 barclaysRec
//actionOnGoldmanOnly3 barclaysRec

(*** condition:html, include:funForSingleCase ***)
(*** condition:html, include-fsi-output:funForSingleCase ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 7
Imagine that stock tips have the form
```fsharp
type StockTip = Buy | Sell | Hold
```
You have recommendations from two people
```fsharp
let friendRec = Buy
let professorRec = Sell
```
You want to actions as follows:
```fsharp
let actionOnFriend (x: StockTip) = x
let actionOnProfessor (x: StockTip) =
    match x with
    | Buy -> StockTip.Sell
    | Hold -> StockTip.Sell
    | Sell -> StockTip.Buy
```

1. Create a two case union called `FriendOrFoe` where the two cases are Friend of StockTip and Professor of StockTip.
2. Create a function called `actionFriendOrFoe` that will properly handle tips from friends and tips from professors.

Show that `friendRec` and `professorRec` wrapped in the `FriendOrFoe` type are handled properly by `actionFriendOrFoe`.
*)

(*** include-it-raw:preDetails ***)
(*** define: funForTwoCases, define-output: funForTwoCases ***)

type StockTip = Buy | Sell | Hold
let friendRec = Buy
let professorRec = Sell
let actionOnFriend (x: StockTip) = x
let actionOnProfessor (x: StockTip) =
    match x with
    | Buy -> StockTip.Sell
    | Hold -> StockTip.Sell
    | Sell -> StockTip.Buy
// or, since we're doing the same thing with a professor's
// Buy or Hold recommendation, this could also be written
let actionOnProfessor2 (x: StockTip) =
    match x with
    | Buy | Hold -> StockTip.Sell
    | Sell -> StockTip.Buy
type FriendOrFoe = 
    | Friend of StockTip
    | Professor of StockTip
let wrappedFriendRec = Friend friendRec
let wrappedProfessorRec = Professor professorRec
let actionOnFriendOrFoe (x: FriendOrFoe) =
    match x with
    | Friend tip -> actionOnFriend tip
    | Professor tip -> actionOnProfessor tip 
actionOnFriendOrFoe wrappedFriendRec // evaluates to Buy
actionOnFriendOrFoe wrappedProfessorRec // evaluates to Buy
actionOnFriendOrFoe (Professor Hold) // evaluates to Sell

(*** condition:html, include:funForTwoCases ***)
(*** condition:html, include-fsi-output:funForTwoCases ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.