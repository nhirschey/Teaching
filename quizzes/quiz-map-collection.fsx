(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=quizzes/quiz-map-collection.ipynb)&emsp;
[![Script](../img/badge-script.svg)](/Teaching//quizzes/quiz-map-collection.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](/Teaching//quizzes/quiz-map-collection.ipynb)

# Map Collections

If we're doing lookups, then a good data structure
for that is a Map collection. A Map is an immutable dictionary of elements.
Maps consist of key and value pairs.
If you look up the key, you get the value.

If you need to do lookups on a key, maps are much more efficient than trying to do the same thing with a list or array.

* Some references.

  * [Map type documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-fsharpmap-2.html)
  
  * [Map module documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-mapmodule.html)
  
  * [F# Wikibook](https://en.wikibooks.org/wiki/F_Sharp_Programming/Sets_and_Maps#Maps)
  

*)
let exampleMap = Map [("a", "hey"); ("b","ho")]
let exampleMap2 = [(4,"apple"); (10,"pizza")] |> Map
(**
Three equivalent ways to find a key given a value.async

Option 1:

*)
exampleMap["a"]
(**
Option 2:

*)
Map.find "a" exampleMap
(**
Option 3:

*)
exampleMap2 |> Map.find 10

// Comparing performance of list vs. Map lookups.

#time "on"
let isOdd x = if x % 2 = 0 then false else true
let arr = [ for i = 1 to 100_000 do (i, isOdd i) ]
let arrMap = arr |> Map

arr |> List.find (fun (a,b) -> a = 100)(* output: 
(100, false)*)
arrMap |> Map.find 101(* output: 
true*)
(**
Compare performance to find something at the beginning of an list.

*)
for i = 1 to 100 do 
    arr |> List.find(fun (a,b) -> a = 1_000) |> ignore
for i = 1 to 100 do
    arrMap |> Map.find 1_000 |> ignore
(**
Compare performance to find something that is towards the end of the list.

*)
for i = 1 to 100 do 
    arr |> List.find(fun (a,b) -> a = 99_000) |> ignore
for i = 1 to 100 do
    arrMap |> Map.find 99_000 |> ignore
(**
## Question 1

Create a Map collection named `mapA`
from the list `[("a",1);("b",2)]` where the first thing
in the tuple is the key and the second thing is the value.

0 Use `Map.tryFind` to retrieve the value for key `"a"`

1 Use `Map.tryFind` to retrieve the value for key `"c"`

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

Create Map Collection:

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val mapA: Map<string,int> = map [("a", 1); ("b", 2)]
```

or

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val mapA2: Map<string,int> = map [("a", 1); ("b", 2)]
```

or

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val mapA3: Map<string,int> = map [("a", 1); ("b", 2)]
```

Use `Map.tryFind` to retrieve the value for key `"a"`:

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: int option = Some 1
```

or

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: int option = Some 1
```

Use `Map.tryFind` to retrieve the value for key `"c"`:

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: int option = None
```

or

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: int option = None
```

</details>
</span>
</p>
</div>

## Question 2

Create a Map collection named `mapB`
from the list `[(1,"a");(2,"b")]` where the first thing
in the tuple is the key and the second thing is the value.

0 Use `Map.tryFind` to retrieve the value for key `1`

1 Use `Map.tryFind` to retrieve the value for key `3`

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val mapB: Map<int,string> = map [(1, "a"); (2, "b")]
val tryFindMapB1: string option = Some "a"
val tryFindMapB3: string option = None
```

</details>
</span>
</p>
</div>

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
0 Create a Map collection named `mapC`. The key should be the day field, 
and the value should be the full `StockDays3` record.

1 Create a Map collection named `mapD`. The key should be the full
`StockDay3` record. The value should be the day field.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val mapC: Map<int,StockDays3> =
  map
    [(0, { Day = 0
           Price = 100M
           Dividend = None }); (1, { Day = 1
                                     Price = 101M
                                     Dividend = Some 1M });
     (2, { Day = 2
           Price = 102M
           Dividend = None }); (3, { Day = 3
                                     Price = 103M
                                     Dividend = Some 1M });
     (4, { Day = 4
           Price = 104M
           Dividend = None }); (5, { Day = 5
                                     Price = 105M
                                     Dividend = Some 1M })]
```

```
Real: 00:00:00.001, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val mapD: Map<StockDays3,int> =
  map
    [({ Day = 0
        Price = 100M
        Dividend = None }, 0); ({ Day = 1
                                  Price = 101M
                                  Dividend = Some 1M }, 1);
     ({ Day = 2
        Price = 102M
        Dividend = None }, 2); ({ Day = 3
                                  Price = 103M
                                  Dividend = Some 1M }, 3);
     ({ Day = 4
        Price = 104M
        Dividend = None }, 4); ({ Day = 5
                                  Price = 105M
                                  Dividend = Some 1M }, 5)]
```

</details>
</span>
</p>
</div>

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

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
I found 1
I did not find b
I did not find c
I found 7
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val lookFor: x: string -> unit
val it: unit = ()
```

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

```
I found 1
I did not find b
I did not find c
I found 7
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: unit = ()
```

or loop it

```
I found 1

I did not find b

I did not find c

I found 7

Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: unit = ()
```

</details>
</span>
</p>
</div>

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

## Question 1

Create a `TwoStocksPriceOb list` named `tslA` that has prices for
every observation of `stockA`. If there is a price for `stockB`
at the same time as `stockA`, then include the `stockB` price. Otherwise,
the `stockB` price should be `None`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.001, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val stockbByTime: Map<int,StockPriceOb> =
  map [(2, { Stock = "B"
             Time = 2
             Price = 5 }); (3, { Stock = "B"
                                 Time = 3
                                 Price = 4 })]
val tslA1: TwoStocksPriceOb list = [{ Time = 1
                                      PriceA = Some 5
                                      PriceB = None }; { Time = 2
                                                         PriceA = Some 6
                                                         PriceB = Some 5 }]
val tslA2: TwoStocksPriceOb list = [{ Time = 1
                                      PriceA = Some 5
                                      PriceB = None }; { Time = 2
                                                         PriceA = Some 6
                                                         PriceB = Some 5 }]
val tslA4: TwoStocksPriceOb list = [{ Time = 1
                                      PriceA = Some 5
                                      PriceB = None }; { Time = 2
                                                         PriceA = Some 6
                                                         PriceB = Some 5 }]
val tslA5: TwoStocksPriceOb list = [{ Time = 1
                                      PriceA = Some 5
                                      PriceB = None }; { Time = 2
                                                         PriceA = Some 6
                                                         PriceB = Some 5 }]
val tryFindBforA: dayA: StockPriceOb -> TwoStocksPriceOb
val tslA3: TwoStocksPriceOb list = [{ Time = 1
                                      PriceA = Some 5
                                      PriceB = None }; { Time = 2
                                                         PriceA = Some 6
                                                         PriceB = Some 5 }]
```

</details>
</span>
</p>
</div>

## Question 2

Create a `TwoStocksPriceOb list` named `tslB` that has prices for
every observation of stockB. If there is a price for `stockA`
at the same time as `stockB`, then include the `stockA` price. Otherwise,
the `stockA` price should be `None`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val stockaByTime: Map<int,StockPriceOb> =
  map [(1, { Stock = "A"
             Time = 1
             Price = 5 }); (2, { Stock = "A"
                                 Time = 2
                                 Price = 6 })]
val tslB: TwoStocksPriceOb list = [{ Time = 2
                                     PriceA = Some 6
                                     PriceB = Some 5 }; { Time = 3
                                                          PriceA = None
                                                          PriceB = Some 4 }]
```

</details>
</span>
</p>
</div>

## Question 3

Create a `TwoStocksPriceOb list` named `tslC` that only includes times
when there is a price for both `stockA` and `stockB`. The prices for stocks
A and B should always be something.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val stockaByTime2: Map<int,StockPriceOb> =
  map [(1, { Stock = "A"
             Time = 1
             Price = 5 }); (2, { Stock = "A"
                                 Time = 2
                                 Price = 6 })]
val tslC1: TwoStocksPriceOb list = [{ Time = 2
                                      PriceA = Some 6
                                      PriceB = Some 5 }]
val timesA: Set<int> = set [1; 2]
val timesB: Set<int> = set [2; 3]
val timesAandB: Set<int> = set [2]
val tslC2: TwoStocksPriceOb list = [{ Time = 2
                                      PriceA = Some 6
                                      PriceB = Some 5 }]
```

</details>
</span>
</p>
</div>

## Question 4

Create a `TwoStocksPriceOb list` named `tslD` that includes available
stock prices for `stockA` and `stockB` at all possible times. If a price for
one of the stocks is missing for a given time, it should be None.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val stockATimes: int list = [1; 2]
val stockBTimes: int list = [2; 3]
val allTimes: int list = [1; 2; 3]
val tslD: TwoStocksPriceOb list =
  [{ Time = 1
     PriceA = Some 5
     PriceB = None }; { Time = 2
                        PriceA = Some 6
                        PriceB = Some 5 }; { Time = 3
                                             PriceA = None
                                             PriceB = Some 4 }]
val testTime: int = 1
val time1A: StockPriceOb option = Some { Stock = "A"
                                         Time = 1
                                         Price = 5 }
val time1B: StockPriceOb option = None
val time1Aprice: int option = Some 5
val time1Bprice: int option = None
val testOutput: TwoStocksPriceOb = { Time = 1
                                     PriceA = Some 5
                                     PriceB = None }
val getTheMatch: time: int -> TwoStocksPriceOb
val tslD2: TwoStocksPriceOb list =
  [{ Time = 1
     PriceA = Some 5
     PriceB = None }; { Time = 2
                        PriceA = Some 6
                        PriceB = Some 5 }; { Time = 3
                                             PriceA = None
                                             PriceB = Some 4 }]
```

</details>
</span>
</p>
</div>

*)

