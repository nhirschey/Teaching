(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=quizzes/quiz-match-expressions.ipynb)&emsp;
[![Script](../img/badge-script.svg)](/Teaching//quizzes/quiz-match-expressions.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](/Teaching//quizzes/quiz-match-expressions.ipynb)

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

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val ma: x: float option -> float
val ma2Some7: float = 7.0
val ma2None: float = 0.0
```

or, see the `x` in the (`Some x`) part of the match expression
is the `float`, not the original (`x: float Option`)
To see this, hover your cursor over the first two xs. it says `x is float Option`.
Then hover over the second two xs. It says `x is float`. Two different xs!

```
val ma2: x: float option -> float
val ma2Some7Other: float = 7.0
val ma2NoneOther: float = 0.0
```

</details>
</span>
</p>
</div>

## Question 2

Write a function named `mb` that takes `x: float` as an input.
Use a match expression to output `1.0` if `x` is `1.0`, `4.0` if `x` is `2.0`,
and `x^3.0` if `x` is anything else. Provide 3 tests for the 3 test cases
to show that the function works.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val mb: x: float -> float
val mb1: float = 1.0
val mb2: float = 4.0
val mb7: float = 343.0
```

</details>
</span>
</p>
</div>

## Question 3

Write a function named `mc` that takes a tuple pair of ints  `x: int * int`
as an input. Handle these cases in the following order:

0 if the first `int` is `7`, return `"a"`.

1 if the second int is `7`, return `"b"`.

2 For everything else, return `"c"`.

Finally, test the function on `(7,6)`, `(6,7)`, `(7, 7)`, and `(6,6)`.
Make sure that you understand how those 4 examples are handled.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val mc: int * int -> string
val mc76: string = "a"
val mc67: string = "b"
val mc77: string = "a"
val mc66: string = "c"
```

</details>
</span>
</p>
</div>

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
0 create a new list called `daysWithDividends` that is a filtered
version of `stockDays` that only contains days with dividends. For
each day with a dividend, you should return a `(int * decimal)` tuple
where the int is the day  and the decimal is the dividend.
Thus the result is an `(int * decimal) list`.

1 Then create a list called `daysWithoutDividends` that is a filtered
version of `stockDays` that only contains days that do not have dividends.
For each day without a dividend, you should return the day as an `int`.
Thus the result is an `int list`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

Days With Dividends:

```
val daysWithDividends1: (int * decimal) list = [(1, 1M); (3, 1M); (5, 1M)]
```

or

```
val daysWithDividends2: (int * decimal) list = [(1, 1M); (3, 1M); (5, 1M)]
```

Days Without Dividends:

```
val daysWithoutDividends: int list = [0; 2; 4]
```

</details>
</span>
</p>
</div>

*)

