(**
---
title: Map collection
category: Practice Quizzes
categoryindex: 2
index: 5
---

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
# Map Collections
If we're doing lookups, then a good data structure 
for that is a Map collection. A Map is an immutable dictionary of elements.
Maps consist of key and value pairs. 
If you look up the key, you get the value.

If you need to do lookups on a key, maps are much more efficient than trying to do the same thing with a list or array. 

- Some references.
    - [Map type documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-fsharpmap-2.html)
    - [Map module documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-mapmodule.html)
    - [F# Wikibook](https://en.wikibooks.org/wiki/F_Sharp_Programming/Sets_and_Maps#Maps)


*)

let exampleMap = Map [("a", "hey"); ("b","ho")]
let exampleMap2 = [(4,"apple"); (10,"pizza")] |> Map

(** Three equivalent ways to find a key given a value.async

Option 1:*)

exampleMap["a"]

(** Option 2:*)
Map.find "a" exampleMap

(** Option 3:*)
exampleMap2 |> Map.find 10

// Comparing performance of list vs. Map lookups.

#time "on"
let isOdd x = if x % 2 = 0 then false else true
let arr = [ for i = 1 to 100_000 do (i, isOdd i) ]
let arrMap = arr |> Map

arr |> List.find (fun (a,b) -> a = 100)
(***include-it***)
arrMap |> Map.find 101
(***include-it***)

(**
Compare performance to find something at the beginning of an list.
*)
(***do-not-eval***)
for i = 1 to 100 do 
    arr |> List.find(fun (a,b) -> a = 1_000) |> ignore

(***do-not-eval***)
for i = 1 to 100 do
    arrMap |> Map.find 1_000 |> ignore

(**
Compare performance to find something that is towards the end of the list.
*)
(***do-not-eval***)
for i = 1 to 100 do 
    arr |> List.find(fun (a,b) -> a = 99_000) |> ignore

(***do-not-eval***)
for i = 1 to 100 do
    arrMap |> Map.find 99_000 |> ignore


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
Use this list
*)

type StockDays3 = 
    {
        Day: int
        Price: decimal
        Dividend: decimal Option 
    }
let stockDays3 = 
    [ for day = 0 to 5 do 
        let dividend = if day % 2 = 0 then None else Some 1m
        { Day = day
          Price = 100m + decimal day
          Dividend = dividend } ]     

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
    |> List.map(fun day ->
        // we just want to create a tuple of the (key,value).
        // The key and value can be anything.
        day.Day, day)
    |> Map
(*** condition:html, include:mapC ***)
(*** condition:html, include-fsi-output:mapC ***)

(*** define: mapD, define-output: mapD ***)    
let mapD =
    stockDays3
    |> List.map(fun day ->
        // we just want to create a tuple of the (key,value).
        // The key and value can be anything.
        day, day.Day)
    |> Map

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
    |> Map
let tslA1 =
    stockA
    |> List.map(fun dayA ->
        let priceB = 
            if stockbByTime.ContainsKey dayA.Time then
                Some stockbByTime[dayA.Time].Price
            else 
                None
        { Time = dayA.Time
          PriceA = Some dayA.Price
          PriceB = priceB } )
// or, just a personal preference if you like the loop or List.map
let tslA2 =
    [ for dayA in stockA do 
        let priceB = 
            if stockbByTime.ContainsKey dayA.Time then
                Some stockbByTime[dayA.Time].Price
            else 
                None
        { Time = dayA.Time
          PriceA = Some dayA.Price
          PriceB = priceB } ]

// or, using Map.tryFind statement
let tslA4 =
    [ for dayA in stockA do
        let priceB =
            match Map.tryFind dayA.Time stockbByTime with
            | Some dayB -> Some dayB.Price
            | None -> None 
        { Time = dayA.Time
          PriceA = Some dayA.Price
          PriceB = priceB } ]

// or, this is the same as tslA4, but using Option.map
let tslA5 =
    [ for dayA in stockA do
        let priceB =
            stockbByTime
            |> Map.tryFind dayA.Time
            |> Option.map (fun x -> x.Price)
        { Time = dayA.Time
          PriceA = Some dayA.Price
          PriceB = priceB } ]



// or, define a function
let tryFindBforA (dayA: StockPriceOb) =
    let priceB =
        match Map.tryFind dayA.Time stockbByTime with
        | Some dayB -> Some dayB.Price
        | None -> None 
    { Time = dayA.Time
      PriceA = Some dayA.Price
      PriceB = priceB } 

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
    |> Map
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
    |> Map
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
            |> Option.map (fun b -> b.Price)
        { Time = time; PriceA = a; PriceB = b }) 

// or, using a function. This is the same thing as in the above
// anonymous function that begins with `(fun time -> `, 
// but I'm going to use different 
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



