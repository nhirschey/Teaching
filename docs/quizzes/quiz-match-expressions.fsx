(**
---
title: Match expressions
category: Practice Quizzes
categoryindex: 2
index: 4
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
# Match Expressions
There is a very good discussion of match expressions with examples on [FSharp For Fun and Profit](https://fsharpforfunandprofit.com/posts/match-expression/).The [F# Language reference](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/match-expressions) and [Tour of F#](https://docs.microsoft.com/en-us/dotnet/fsharp/tour#pattern-matching) are also good resources on this.

The idea is that you give the match expression a value to match, the things it can match to, and then what you want to return for each of those matches.

Simple match expressions such as the one below are 
similar to if/then/else statements. However, as shown in the linked
F# for fun and profit article, pattern matching is can do much
more than if/then/else.
*)

let f1 x =
    match x with // x is the thing that I am matching
    | 1.0 -> 1.0 // when x matches 1.0 then return 1.0
    | 2.0 -> 1.0 // when x matches with 2.0 then return 1.0
    | 3.0 -> 7.0 + 7.0 // when x matches with 3.0 return 7.0+7.0
    | z -> z ** 2.0 // everything else matches an arbitrary y, and let's return y**2.0
    
[ 1.0 .. 10.0] |> List.map f1

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
Consider this list of trading days for a stock and it's price and dividends:
*)

type StockDays2 = { Day : int; Price : decimal; Dividend : decimal Option }
let stockDays2 = 
    [ for day = 0 to 5 do 
        let dividend = if day % 2 = 0 then None else Some 1m
        { Day = day
          Price = 100m + decimal day
          Dividend = dividend } ]   

(**

1. create a new list called `daysWithDividends` that is a filtered
  version of `stockDays` that only contains days with dividends. For
  each day with a dividend, you should return a `(int * decimal)` tuple
  where the int is the day  and the decimal is the dividend. 
  Thus the result is an `(int * decimal) list`.
2. Then create a list called `daysWithoutDividends` that is a filtered
  version of `stockDays` that only contains days that do not have dividends.
  For each day without a dividend, you should return the day as an `int`.
  Thus the result is an `int list`.

*)

(*** include-it-raw:preDetails ***)

(**
Days With Dividends: 
*)

(*** define: daysWithAndWithoutDividends, define-output: daysWithAndWithoutDividends ***)           
let daysWithDividends1 =
    // using filter and then a map
    stockDays2
    |> List.filter (fun day -> day.Dividend.IsSome)
    |> List.map(fun day ->
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
    |> List.choose (fun day -> 
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
    |> List.choose(fun day -> 
        match day.Dividend with
        | None -> Some day.Day
        | Some div -> None)
(*** condition:html, include:daysWithAndWithoutDividends2 ***)
(*** condition:html, include-fsi-output:daysWithAndWithoutDividends2 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


