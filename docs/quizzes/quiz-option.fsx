(**
---
title: Options
category: Practice Quizzes
categoryindex: 2
index: 3
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
# Options
Sometimes something exists or doesn't exist. This can be useful to model explicitly. [FSharp For Fun And Profit](https://fsharpforfunandprofit.com/posts/the-option-type/) has a nice discussion of option types and how to use them. You can also find information on the [tour of F#](https://docs.microsoft.com/en-us/dotnet/fsharp/tour#optional-types) and the [F# language reference](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-optionmodule.html).

The main purpose is to model situations where you could have "something" or "nothing" explicitly.

For example, this `testMap` does not contain a "c" key.
*)

let testMap = Map [("a", 1); ("b", 1)]

(**
 If we try to find the value indexed by "c" we will get an exception.
 *)

(***do-not-eval***)
Map.find "c" testMap

(**
 The preferred way to do this is to try to get the key, and then if there is no value for that key return nothing. Options are either `Some x` or `None`, where `x` is the data that you want. This is what the "..try" functions are about.
*)

Map.tryFind "a" testMap

(***do-not-eval***)
Map.tryFind "c" testMap

(** Other option examples *)
let xx = Some 4.0
let yy = None

xx |> Option.map(fun x -> x + 1.0)
(***include-it***)

yy |> Option.map (fun x -> x + 1.0)
(***include-it***)

let add100ToOption x = x |> Option.map(fun x -> x + 100.0)
let xxyy = [xx; yy; xx; yy; add100ToOption xx ] 
xxyy
(***include-it***)

(**
now add another 100 to every element
*)
xxyy |> List.map add100ToOption
(***include-it***)

let divideBy2 x = x / 2.0
xxyy 
|> List.map(fun x -> 
    x |> Option.map divideBy2
)
(***include-it***)

(**
Choose is like *.map but it discards
the `None` results and unwraps the `Some` results.
*)
xxyy 
|> List.choose(fun x -> 
    x |> Option.map divideBy2
)
(***include-it***)

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
1. create a new list called `stockDaysWithDividends` that is a filtered
  version of `stockDays` that only contains days with dividends. 
2. Then create an list called `stockDaysWithoutDividends` that is a filtered
  version of `stockDays` that only contains days that do not have dividends.
*)

(*** include-it-raw:preDetails ***)

(*** define: stockDayswithDividends, define-output: stockDayswithDividends ***)
let stockDaysWithDivideds =
    stockDays
    |> List.filter(fun day -> 
        // variable names are arbitrary.
        // We could have written `fun x` or `fun y` or ...
        // But it's helpful to use
        // meaningful names like "day" if the record that our
        // function is operating on represents a day.
        day.Dividend.IsSome)
(*** condition:html, include:stockDayswithDividends ***)
(*** condition:html, include-fsi-output:stockDayswithDividends ***)

(*** define: stockDayswithDividends1, define-output: stockDayswithDividends1 ***)
let stockDaysWithoutDividends =
    stockDays
    |> List.filter(fun day -> 
        day.Dividend.IsNone)
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


