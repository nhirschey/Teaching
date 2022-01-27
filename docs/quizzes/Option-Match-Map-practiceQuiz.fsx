(**
---
title: Option Match Map
category: Practice Quizzes
categoryindex: 2
index: 4
---
*)

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
This practice quiz emphasizes `Optional types`, `Match expressions`, and `Map Collections`. These are some features that we will use in building a portfolio.

Here are some references to these topics. Please read the F# language reference links before proceeding with the questions. The other links (F# tour and F# for fun and profit) provide additional background and examples but are not necessary:

- Option types

    - The F# language reference for [options](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/options)
    - The tour of F# section on [options](https://docs.microsoft.com/en-us/dotnet/fsharp/tour#optional-types)
    - If you want more a more in depth discussion, see F# for fun and profit's section on [options](https://fsharpforfunandprofit.com/posts/the-option-type/)

- Pattern matching using match expressions.

    - [F# Language reference](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/match-expressions)
    - [Tour of F#](https://docs.microsoft.com/en-us/dotnet/fsharp/tour#pattern-matching)
    - [F# for fun and profit](https://fsharpforfunandprofit.com/posts/match-expression/)

- Map collections.

    - [F# Wikibook](https://en.wikibooks.org/wiki/F_Sharp_Programming/Sets_and_Maps#Maps)

*)

(**
# Options
*)

(**
## Question 1
Create a value named `a` and assign `Some 4` to it.
*)

(*** include-it-raw:preDetails ***)
(*** define: aSome4, define-output: aSome4 ***)

let a = Some 4

(*** condition:html, include:aSome4 ***)
(*** condition:html, include-fsi-output:aSome4 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Create a value name `b` and assign `None` to it.
*)

(*** include-it-raw:preDetails ***)
(*** define: bNone, define-output: bNone ***)

let b = None

(*** condition:html, include:bNone ***)
(*** condition:html, include-fsi-output:bNone ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Create a tuple named `c` and assign `(Some 4, None)` to it.
*)

(*** include-it-raw:preDetails ***)
(*** define: cSome4None, define-output: cSome4None ***)

let c = Some 4, None

(*** condition:html, include:cSome4None ***)
(*** condition:html, include-fsi-output:cSome4None ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 4
Write a function named d that takes `x: float` as an input and outputs
`Some x` if x < 0 and `None` if x >= 0. Test it by mapping each element of
`[0.0; 1.4; -7.0]` by function d.
*)

(*** include-it-raw:preDetails ***)

(*** define: dFunction, define-output: dFunction ***)
let d (x: float) = if x < 0.0 then Some x else None
[0.0; 1.4;-7.0] |> List.map d
(*** condition:html, include:dFunction ***)
(*** condition:html, include-fsi-output:dFunction ***)

(**
or, we don't actually have to tell it that `x` is a `float`
because type inference can tell that `x` must be a `float`
because the function does `x < 0.0` and `0.0` is a `float`.
*)

(*** define: dFunction1, define-output: dFunction1 ***)
let d2 x = if x < 0.0 then Some x else None
[0.0; 1.4;-7.0] |> List.map d2
(*** condition:html, include:dFunction1 ***)
(*** condition:html, include-fsi-output:dFunction1 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 5
Consider this array of trading days for a stock and it's price and dividends:
*)

type StockDays = { Day : int; Price : decimal; Dividend : decimal Option }
let stockDays = 
    [| for day = 0 to 5 do 
        let dividend = if day % 2 = 0 then None else Some 1m
        { Day = day
          Price = 100m + decimal day
          Dividend = dividend } |]   

(**
1. create a new array called `stockDaysWithDividends` that is a filtered
  version of `stockDays` that only contains days with dividends. 
2. Then create an array called `stockDaysWithoutDividends` that is a filtered
  version of `stockDays` that only contains days that do not have dividends.
*)

(*** include-it-raw:preDetails ***)

(*** define: stockDayswithDividends, define-output: stockDayswithDividends ***)
let stockDaysWithDivideds =
    stockDays
    |> Array.filter(fun day -> 
        // variable names are arbitrary, but it's helpful to use
        // meaningful names like "day" if the record that our
        // function is operating on represents a day.
        // using a variable named day to represent the day record
        day.Dividend.IsSome)
(*** condition:html, include:stockDayswithDividends ***)
(*** condition:html, include-fsi-output:stockDayswithDividends ***)

(*** define: stockDayswithDividends1, define-output: stockDayswithDividends1 ***)
let stockDaysWithoutDividends =
    stockDays
    |> Array.filter(fun x -> 
        // using a variable named x to represent the day record.
        // less clear by looking at this code that x is a day.
        x.Dividend.IsNone)
(*** condition:html, include:stockDayswithDividends1 ***)
(*** condition:html, include-fsi-output:stockDayswithDividends1 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 6
Consider the value `let nestedOption = (Some (Some 4))`. Pipe
it to `Option.flatten` so that you are left with `Some 4`.
*)

(*** include-it-raw:preDetails ***)
(*** define: nestedOption, define-output: nestedOption ***)

let nestedOption = (Some (Some 4))
nestedOption |> Option.flatten
// this would also work, but doesn't use a pipe
Option.flatten nestedOption

(*** condition:html, include:nestedOption ***)
(*** condition:html, include-fsi-output:nestedOption ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 7
Consider this list `let listOfNestedOptions = [(Some (Some 4)); Some (None); None]`.
Show how to transform it into `[Some 4; None; None]` by mapping a function to each
element of the list. 
*)

(*** include-it-raw:preDetails ***)
(*** define: listOfNestedOptions, define-output: listOfNestedOptions ***)

let listOfNestedOptions = [(Some (Some 4)); Some (None); None]
// map the function Option.flatten to each element of the list
listOfNestedOptions |> List.map Option.flatten


(*** condition:html, include:listOfNestedOptions ***)
(*** condition:html, include-fsi-output:listOfNestedOptions ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
# Match Expressions
*)

(**
## Question 1
Write a function named `ma` that takes `x: float Option` as an input.
Use a match expression to output the `float` if `x` is something and
`0.0` if the `float` is nothing. Provide a test case for both cases to show
that the function works.
*)

(*** include-it-raw:preDetails ***)

(*** define: maFunction, define-output: maFunction ***)
let ma x = 
    match x with
    | None -> 0.0
    | Some y -> y

let ma2Some7 = ma (Some 7.0) // returns 7.0
let ma2None = ma None // returns 0.0
(*** condition:html, include:maFunction ***)
(*** condition:html, include-fsi-output:maFunction ***)

(**
or, see the `x` in the (`Some x`) part of the match expression
is the `float`, not the original (`x: float Option`)
To see this, hover your cursor over the first two xs. it says `x is float Option`.
Then hover over the second two xs. It says `x is float`. Two different xs!
*)

(*** define: maFunction1, define-output: maFunction1 ***)
let ma2 x = 
    match x with
    | None -> 0.0
    | Some x -> x

let ma2Some7Other = ma2 (Some 7.0) // returns 7.0
let ma2NoneOther = ma2 None // returns 0.0
(*** condition:html, include:maFunction1 ***)
(*** condition:html, include-fsi-output:maFunction1 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Write a function named `mb` that takes `x: float` as an input.
Use a match expression to output `1.0` if `x` is `1.0`, `4.0` if `x` is `2.0`,
and `x^3.0` if `x` is anything else. Provide 3 tests for the 3 test cases 
to show that the function works.
*)

(*** include-it-raw:preDetails ***)
(*** define: mbFunction, define-output: mbFunction ***)

let mb x = 
    match x with 
    | 1.0 -> 1.0
    | 2.0 -> 4.0
    | x -> x**3.0

let mb1 = mb 1.0 // evaluates to 1.0
let mb2 = mb 2.0 // evaluates to 4.0
let mb7 = mb 7.0 // evaluates to 343.00

(*** condition:html, include:mbFunction ***)
(*** condition:html, include-fsi-output:mbFunction ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Write a function named `mc` that takes a tuple pair of ints  `x: int * int`
as an input. Handle these cases in the following order:

1. if the first `int` is `7`, return `"a"`.
2. if the second int is `7`, return `"b"`.
3. For everything else, return `"c"`.

Finally, test the function on `(7,6)`, `(6,7)`, `(7, 7)`, and `(6,6)`.
Make sure that you understand how those 4 examples are handled.
*)

(*** include-it-raw:preDetails ***)
(*** define: mcFunction, define-output: mcFunction ***)

let mc x =
    match x with
    | (7, _) -> "a" // the _ in (7, _) indicates wildcard; it matches anything.
    | (_, 7) -> "b" 
    | _ -> "c" // wild card at the end catches anything remaining.

let mc76 = mc (7,6) // evaluates to "a" because it matches the first case and stops checking.
let mc67 = mc (6,7) // evaluates to "b" because it matches the second case and stops checking.
let mc77 = mc (7,7) // evaluates to "a" because it matches the first case and stops checking.
let mc66 = mc (6,6) // evaluates to "c" because it matches the last wildcard.

(*** condition:html, include:mcFunction ***)
(*** condition:html, include-fsi-output:mcFunction ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 4
Consider this array of trading days for a stock and it's price and dividends:
*)

type StockDays2 = { Day : int; Price : decimal; Dividend : decimal Option }
let stockDays2 = 
    [| for day = 0 to 5 do 
        let dividend = if day % 2 = 0 then None else Some 1m
        { Day = day
          Price = 100m + decimal day
          Dividend = dividend } |]   

(**

1. create a new array called `daysWithDividends` that is a filtered
  version of `stockDays` that only contains days with dividends. For
  each day with a dividend, you should return a `(int * decimal)` tuple
  where the int is the day  and the decimal is the dividend. 
  Thus the result is an `(int * decimal) array`.
2. Then create an array called `daysWithoutDividends` that is a filtered
  version of `stockDays` that only contains days that do not have dividends.
  For each day without a dividend, you should return the day as an `int`.
  Thus the result is an `int array`.

*)

(*** include-it-raw:preDetails ***)

(**
Days With Dividends: 
*)

(*** define: daysWithAndWithoutDividends, define-output: daysWithAndWithoutDividends ***)           
let daysWithDividends1 =
    // using filter and then a map
    stockDays2
    |> Array.filter (fun day -> day.Dividend.IsSome)
    |> Array.map(fun day ->
        match day.Dividend with
        | None -> failwith "shouldn't happen because I filtered on IsSome"
        | Some div -> day.Day, div)
(*** condition:html, include:daysWithAndWithoutDividends ***)
(*** condition:html, include-fsi-output:daysWithAndWithoutDividends ***)

(**
or
*)

(*** define: daysWithAndWithoutDividends1, define-output: daysWithAndWithoutDividends1 ***)           
let daysWithDividends2 =
    // using choose, this is better. Think of choose
    // as a filter on IsSome and a map combined. Choose applies
    // a function that returns an option. If the
    // option result is Some x then choose returns x.
    // If the result is None then choose filters it out.
    // Notice that we don't have to worry about 
    // the "this shouldn't happen" exception
    // because it literally cannot happen in this version.
    // This is an example of making illegal states unrepresentable.
    stockDays2
    |> Array.choose (fun day -> 
        // our function takes a day as an input and outputs
        // a `(int * decimal) option`. That is,
        // an optional tuple.
        match day.Dividend with 
        | None -> None
        | Some div -> Some (day.Day, div))
(*** condition:html, include:daysWithAndWithoutDividends1 ***)
(*** condition:html, include-fsi-output:daysWithAndWithoutDividends1 ***)

(**
Days Without Dividends: 
*)

(*** define: daysWithAndWithoutDividends2, define-output: daysWithAndWithoutDividends2 ***)           
let daysWithoutDividends =
    stockDays2
    |> Array.choose(fun day -> 
        match day.Dividend with
        | None -> Some day.Day
        | Some div -> None)
(*** condition:html, include:daysWithAndWithoutDividends2 ***)
(*** condition:html, include-fsi-output:daysWithAndWithoutDividends2 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
# Map Collections
*)

(**
## Question 1
Create a Map collection named `mapA` 
from the list `[("a",1);("b",2)]` where the first thing 
in the tuple is the key and the second thing is the value.

1. Use `Map.tryFind` to retrieve the value for key `"a"`
2. Use `Map.tryFind` to retrieve the value for key `"c"`
*)

(*** include-it-raw:preDetails ***)

(**
Create Map Collection:
*)

(*** define: mapA, define-output: mapA ***)
let mapA = Map [("a",1);("b",2)]
(*** condition:html, include:mapA ***)
(*** condition:html, include-fsi-output:mapA ***)

(**
or
*)

(*** define: mapA2, define-output: mapA2 ***)
let mapA2 = [("a",1);("b",2)] |> Map
(*** condition:html, include:mapA2 ***)
(*** condition:html, include-fsi-output:mapA2 ***)

(**
or
*)

(*** define: mapA3, define-output: mapA3 ***)
let mapA3 = [("a",1);("b",2)] |> Map.ofList
(*** condition:html, include:mapA3 ***)
(*** condition:html, include-fsi-output:mapA3 ***)

(**
Use `Map.tryFind` to retrieve the value for key `"a"`:
*)

(*** define: tryFindA, define-output: tryFindA ***)
Map.tryFind "a" mapA    // evaluates to Some 1
(*** condition:html, include:tryFindA ***)
(*** condition:html, include-fsi-output:tryFindA ***)

(**
or
*)

(*** define: tryFindA1, define-output: tryFindA1 ***)
mapA |> Map.tryFind "a" // evaluates to Some 1
(*** condition:html, include:tryFindA1 ***)
(*** condition:html, include-fsi-output:tryFindA1 ***)

(**
Use `Map.tryFind` to retrieve the value for key `"c"`:
*)

(*** define: tryFindC, define-output: tryFindC ***)
Map.tryFind "c" mapA    // evaluates to None
(*** condition:html, include:tryFindC ***)
(*** condition:html, include-fsi-output:tryFindC ***)

(**
or
*)

(*** define: tryFindC1, define-output: tryFindC1 ***)
mapA |> Map.tryFind "c" // evaluates to None
(*** condition:html, include:tryFindC1 ***)
(*** condition:html, include-fsi-output:tryFindC1 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Create a Map collection named `mapB`
from the list `[(1,"a");(2,"b")]` where the first thing
in the tuple is the key and the second thing is the value.

1. Use `Map.tryFind` to retrieve the value for key `1`
2. Use `Map.tryFind` to retrieve the value for key `3`
*)

(*** include-it-raw:preDetails ***)
(*** define: mapB, define-output: mapB ***)

let mapB = Map [(1,"a");(2,"b")]
let tryFindMapB1 = Map.tryFind 1 mapB
let tryFindMapB3 =Map.tryFind 3 mapB

(*** condition:html, include:mapB ***)
(*** condition:html, include-fsi-output:mapB ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Use this array
*)

type StockDays3 = { Day : int; Price : decimal; Dividend : decimal Option }
let stockDays3 = 
    [| for day = 0 to 5 do 
        let dividend = if day % 2 = 0 then None else Some 1m
        { Day = day
          Price = 100m + decimal day
          Dividend = dividend } |]     

(**
1. Create a Map collection named `mapC`. The key should be the day field, 
  and the value should be the full `StockDays3` record.
2. Create a Map collection named `mapD`. The key should be the full
  `StockDay3` record. The value should be the day field.
*)

(*** include-it-raw:preDetails ***)

(*** define: mapC, define-output: mapC ***)    
let mapC =
    stockDays3
    |> Array.map(fun day ->
        // we just want to create a tuple of the (key,value).
        // The key and value can be anything.
        day.Day, day)
    |> Map.ofArray
(*** condition:html, include:mapC ***)
(*** condition:html, include-fsi-output:mapC ***)

(*** define: mapD, define-output: mapD ***)    
let mapD =
    stockDays3
    |> Array.map(fun day ->
        // we just want to create a tuple of the (key,value).
        // The key and value can be anything.
        day, day.Day)
    |> Map.ofArray

(*** condition:html, include:mapD ***)
(*** condition:html, include-fsi-output:mapD ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 4
Consider a the following Map collection:
*)
let mapp = [("a", 1); ("d",7)] |> Map.ofList

(**
Write a function named `lookFor` that takes `x: string` as an input and
looks up the value in `mapp`. If it finds `Some y`, print
`"I found y"` to standard output where `y` is the actual integer found. 
If it finds `None`, print `"I did not find x"` to standard output
where `x` is the actual key that was looked up. Test it by looking
up `"a"`,`"b"`,"`c"`,`"d"`
*)

(*** include-it-raw:preDetails ***)

(*** define: lookFor, define-output: lookFor ***)    
let lookFor x =
    match Map.tryFind x mapp with
    | Some y -> printfn $"I found {y}"
    | None -> printfn $"I did not find {x}" 

lookFor "a" // I found 1
lookFor "b" // I did not find b
lookFor "c" // I did not find c
lookFor "d" // I found 7
(*** condition:html, include:lookFor ***)
(*** condition:html, include-fsi-merged-output:lookFor ***)

(**
or iterate it
we use iter instead of map
because the result of iter has type `unit`,
and iter is for when your function has type `unit`.
Basically, unit type means the function did something
(in this case, printed to standard output) but
it doesn't actually return any output.  
You could use map, but then we get `unit list` which
isn't really what we want. We just want to iterate
through the list and print to output.
*)

(*** define: lookFor1, define-output: lookFor1 ***)    
["a"; "b"; "c"; "d"] |> List.iter lookFor
(*** condition:html, include:lookFor1 ***)
(*** condition:html, include-fsi-merged-output:lookFor1 ***)

(**
or loop it
*)

(*** define: lookFor2, define-output: lookFor2 ***)    
for letter in ["a"; "b"; "c"; "d"] do
    printfn $"{lookFor letter}"    
(*** condition:html, include:lookFor2 ***)
(*** condition:html, include-fsi-merged-output:lookFor2 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
# Joins

For the following questions use this data:
*)
type StockPriceOb =
    { Stock : string 
      Time : int
      Price : int }
type TwoStocksPriceOb =
    { Time : int 
      PriceA : int option 
      PriceB : int option }
let stockA = 
    [{ Stock = "A"; Time = 1; Price = 5}
     { Stock = "A"; Time = 2; Price = 6}]
let stockB =     
    [{ Stock = "B"; Time = 2; Price = 5}
     { Stock = "B"; Time = 3; Price = 4}]

(**
Hint: remember that Map collections are useful for lookups.
*)


(**
## Question 1
Create a `TwoStocksPriceOb list` named `tslA` that has prices for
every observation of `stockA`. If there is a price for `stockB`
at the same time as `stockA`, then include the `stockB` price. Otherwise,
the `stockB` price should be `None`.
*)

(*** include-it-raw:preDetails ***)
(*** define: TwoStockPriceOb, define-output: TwoStockPriceOb ***)    

let stockbByTime = 
    stockB 
    |> List.map(fun day -> day.Time, day)
    |> Map.ofList
let tslA1 =
    stockA
    |> List.map(fun dayA ->
        let dayB = Map.tryFind dayA.Time stockbByTime
        match dayB with
        | None -> 
            { Time = dayA.Time
              PriceA = Some dayA.Price
              PriceB = None}
        | Some db -> 
            { Time = dayA.Time
              PriceA = Some dayA.Price 
              PriceB = Some db.Price })
// or, just a personal preference if you like the loop or List.map
let tslA2 =
    [ for dayA in stockA do 
        let dayB = Map.tryFind dayA.Time stockbByTime
        match dayB with
        | None -> 
            { Time = dayA.Time
              PriceA = Some dayA.Price
              PriceB = None}
        | Some db -> 
            { Time = dayA.Time
              PriceA = Some dayA.Price 
              PriceB = Some db.Price }]
// or, define a function
let tryFindBforA (dayA: StockPriceOb) =
    let dayB = Map.tryFind dayA.Time stockbByTime
    match dayB with
    | None -> 
        { Time = dayA.Time
          PriceA = Some dayA.Price
          PriceB = None}
    | Some db -> 
        { Time = dayA.Time
          PriceA = Some dayA.Price 
          PriceB = Some db.Price }   
// checkit
tryFindBforA stockA.[0]
// do it
let tslA3 = stockA |> List.map tryFindBforA                         

(*** condition:html, include:TwoStockPriceOb ***)
(*** condition:html, include-fsi-output:TwoStockPriceOb ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Create a `TwoStocksPriceOb list` named `tslB` that has prices for
every observation of stockB. If there is a price for `stockA`
at the same time as `stockB`, then include the `stockA` price. Otherwise,
the `stockA` price should be `None`.
*)

(*** include-it-raw:preDetails ***)
(*** define: tslB, define-output: tslB ***)    

let stockaByTime = 
    stockA 
    |> List.map(fun day -> day.Time, day)
    |> Map.ofList
let tslB =
    stockB
    |> List.map(fun dayB ->
        let dayA = Map.tryFind dayB.Time stockaByTime
        match dayA with
        | None -> 
            { Time = dayB.Time
              PriceA = None
              PriceB = Some dayB.Price }
        | Some da -> 
            { Time = dayB.Time
              PriceA = Some da.Price 
              PriceB = Some dayB.Price })                        

(*** condition:html, include:tslB ***)
(*** condition:html, include-fsi-output:tslB ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Create a `TwoStocksPriceOb list` named `tslC` that only includes times
when there is a price for both `stockA` and `stockB`. The prices for stocks
A and B should always be something.
*)

(*** include-it-raw:preDetails ***)
(*** define: tslC, define-output: tslC ***)    

let stockaByTime2 = 
    stockA 
    |> List.map(fun day -> day.Time, day)
    |> Map.ofList
let tslC1 =
    stockB
    |> List.choose(fun dayB ->
        let dayA = Map.tryFind dayB.Time stockaByTime2
        match dayA with
        | None -> None
        | Some da -> 
            let output =
                { Time = dayB.Time
                  PriceA = Some da.Price 
                  PriceB = Some dayB.Price }
            Some output)
// or, using set which I know you do not know. But #yolo
let timesA = stockA |> List.map(fun x -> x.Time) |> set
let timesB = stockB |> List.map(fun x -> x.Time) |> set
let timesAandB = Set.intersect timesA timesB
let tslC2 =
    timesAandB
    |> Set.toList
    |> List.map(fun time -> 
        { Time = time 
          PriceA = Some stockaByTime.[time].Price
          PriceB = Some stockbByTime.[time].Price})                      

(*** condition:html, include:tslC ***)
(*** condition:html, include-fsi-output:tslC ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 4
Create a `TwoStocksPriceOb list` named `tslD` that includes available
stock prices for `stockA` and `stockB` at all possible times. If a price for
one of the stocks is missing for a given time, it should be None.


*)

(*** include-it-raw:preDetails ***)
(*** define: tslD, define-output: tslD ***)    

let stockATimes = stockA |> List.map(fun x -> x.Time)
let stockBTimes = stockB |> List.map(fun x -> x.Time)
let allTimes = 
    List.append stockATimes stockBTimes
    |> List.distinct
let tslD =
    allTimes
    |> List.map(fun time ->
        let a = 
            match Map.tryFind time stockaByTime with
            | None -> None
            | Some a -> Some a.Price
            
        let b = 
            // same thing as what's done above with match expression,
            // but with Option.map. Personal preference depending
            // on what seems clearest. If you're mapping an option
            // and returning None for the None case,
            // a Option.map can be nice.
            Map.tryFind time stockbByTime
            |> Option.map(fun b -> b.Price)
        { Time = time; PriceA = a; PriceB = b })       
// or, using a function. This is the same thing as in the above
// anonymous lambda function, but I'm going to use different 
// code to achieve the same goal just to show you different options.
// again, it's just personal preference.
// check how to write the function using time = 1 as a test
let testTime = 1
let time1A = Map.tryFind testTime stockaByTime
let time1B = Map.tryFind testTime stockbByTime
let time1Aprice = time1A |> Option.map(fun x -> x.Price )
let time1Bprice = time1B |> Option.map(fun x -> x.Price)
let testOutput = { Time = testTime; PriceA = time1Aprice; PriceB = time1Bprice }
// now turn above code into a function to do the same thing
let getTheMatch time =
    let a = Map.tryFind time stockaByTime
    let b = Map.tryFind time stockbByTime
    let aPrice = a |> Option.map(fun x -> x.Price)
    let bPrice = b |> Option.map(fun x -> x.Price)
    { Time = time; PriceA = aPrice; PriceB = bPrice }
// test it to see that it works
getTheMatch 1
getTheMatch 2
// now do it for the whole list
let tslD2 = allTimes |> List.map getTheMatch                  

(*** condition:html, include:tslD ***)
(*** condition:html, include-fsi-output:tslD ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.



