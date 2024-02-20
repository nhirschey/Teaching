(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=quizzes/quiz-options.ipynb)&emsp;
[![Script](../img/badge-script.svg)](/Teaching//quizzes/quiz-options.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](/Teaching//quizzes/quiz-options.ipynb)

# Options

Sometimes something exists or doesn't exist. This can be useful to model explicitly. [FSharp For Fun And Profit](https://fsharpforfunandprofit.com/posts/the-option-type/) has a nice discussion of option types and how to use them. You can also find information on the [tour of F#](https://docs.microsoft.com/en-us/dotnet/fsharp/tour#options), [F# language reference](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/options) and the [F# core documentation](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-optionmodule.html).
Finally, Jane Street's tech [blog](https://blog.janestreet.com/making-something-out-of-nothing-or-why-none-is-better-than-nan-and-null/) has a good discussion as well.

The main purpose is to model situations where you could have "something" or "nothing" explicitly.

For example, this `testMap` does not contain a "c" key.

*)
let testMap = Map [("a", 1); ("b", 1)]
(**
If we try to find the value indexed by "c" we will get an exception.

*)
Map.find "c" testMap
(**
The preferred way to do this is to try to get the key, and then if there is no value for that key return nothing. Options are either `Some x` or `None`, where `x` is the data that you want. This is what the "..try" functions are about.

*)
Map.tryFind "a" testMap
Map.tryFind "c" testMap
(**
Other option examples

*)
let xx = Some 4.0
let yy = None

xx |> Option.map(fun x -> x + 1.0)(* output: 
Some 5.0*)
yy |> Option.map (fun x -> x + 1.0)(* output: 
<null>*)
let add100ToOption x = x |> Option.map(fun x -> x + 100.0)
let xxyy = [xx; yy; xx; yy; add100ToOption xx ] 
xxyy(* output: 
[Some 4.0; None; Some 4.0; None; Some 104.0]*)
(**
now add another 100 to every element

*)
xxyy |> List.map add100ToOption(* output: 
[Some 104.0; None; Some 104.0; None; Some 204.0]*)
let divideBy2 x = x / 2.0
xxyy 
|> List.map(fun x -> 
    x |> Option.map divideBy2
)(* output: 
[Some 2.0; None; Some 2.0; None; Some 52.0]*)
(**
Choose is like *.map but it discards
the `None` results and unwraps the `Some` results.

*)
xxyy 
|> List.choose(fun x -> 
    x |> Option.map divideBy2
)(* output: 
[2.0; 2.0; 52.0]*)
(**
## Question 1

Create a value named `a` and assign `Some 4` to it.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val a: int option = Some 4
```

</details>
</span>
</p>
</div>

## Question 2

Create a value name `b` and assign `None` to it.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val b: 'a option
```

</details>
</span>
</p>
</div>

## Question 3

Create a tuple named `c` and assign `(Some 4, None)` to it.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val c: int option * 'a option
```

</details>
</span>
</p>
</div>

## Question 4

Write a function named d that takes `x: float` as an input and outputs
`Some x` if x < 0 and `None` if x >= 0. Test it by mapping each element of
`[0.0; 1.4; -7.0]` by function d.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val d: x: float -> float option
val it: float option list = [None; None; Some -7.0]
```

or, we don't actually have to tell it that `x` is a `float`
because type inference can tell that `x` must be a `float`
because the function does `x < 0.0` and `0.0` is a `float`.

```
val d2: x: float -> float option
val it: float option list = [None; None; Some -7.0]
```

</details>
</span>
</p>
</div>

## Question 5

Consider this list of trading days for a stock and it's price and dividends:

*)
type StockDays = 
    { 
        Day : int 
        Price : decimal
        Dividend : decimal Option 
    }

let stockDays = 
    [ for day = 0 to 5 do 
        let dividend = if day % 2 = 0 then None else Some 1m
        { Day = day
          Price = 100m + decimal day
          Dividend = dividend } ]   
(**
0 create a new list called `stockDaysWithDividends` that is a filtered
version of `stockDays` that only contains days with dividends.

1 Then create an list called `stockDaysWithoutDividends` that is a filtered
version of `stockDays` that only contains days that do not have dividends.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val stockDaysWithDivideds: StockDays list =
  [{ Day = 1
     Price = 101M
     Dividend = Some 1M }; { Day = 3
                             Price = 103M
                             Dividend = Some 1M }; { Day = 5
                                                     Price = 105M
                                                     Dividend = Some 1M }]
```

```
val stockDaysWithoutDividends: StockDays list =
  [{ Day = 0
     Price = 100M
     Dividend = None }; { Day = 2
                          Price = 102M
                          Dividend = None }; { Day = 4
                                               Price = 104M
                                               Dividend = None }]
```

</details>
</span>
</p>
</div>

## Question 6

Consider the value `let nestedOption = (Some (Some 4))`. Pipe
it to `Option.flatten` so that you are left with `Some 4`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val nestedOption: int option option = Some (Some 4)
val it: int option = Some 4
```

</details>
</span>
</p>
</div>

## Question 7

Consider this list `let listOfNestedOptions = [(Some (Some 4)); Some (None); None]`.
Show how to transform it into `[Some 4; None; None]` by mapping a function to each
element of the list.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val listOfNestedOptions: int option option list =
  [Some (Some 4); Some None; None]
val it: int option list = [Some 4; None; None]
```

</details>
</span>
</p>
</div>

*)

