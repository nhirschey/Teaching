(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=quizzes/quiz-list-module.ipynb)&emsp;
[![Script](../img/badge-script.svg)](/Teaching//quizzes/quiz-list-module.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](/Teaching//quizzes/quiz-list-module.ipynb)

These exercises work with functions from the `List` module.
You can find examples of using them [F# core docs](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-listmodule.html).

There are similar functions for [arrays](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-arraymodule.html) and
[sequences](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-seqmodule.html).

We're going to use the following data in the questions

*)
#r "nuget: FSharp.Stats"

open System
open FSharp.Stats

type ReturnOb = { Symbol : string; Date : DateTime; Return : float }
type ValueOb = { Symbol : string; Date : DateTime; Value : float }

let seed = 1
Random.SetSampleGenerator(Random.RandBasic(seed))   
let normal = Distributions.Continuous.normal 0.0 0.1

let returns =
    [ 
        for symbol in ["AAPL"; "TSLA"] do
        for month in [1..2] do
        for day in [1..3] do
            { Symbol = symbol 
              Date = DateTime(2021, month, day)
              Return = normal.Sample()}
    ]
(**
## Question 1

Given the list below, filter the list so that only numbers greater than `2` remain.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: int list = [7]
```

</details>
</span>
</p>
</div>

## Question 2

Given the list below, take elements until you find one that is greater than `4`.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: int list = [1; -4]
```

</details>
</span>
</p>
</div>

## Question 3

Given the list below, take elements until you find one that is greater than `4`.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: int list = [7; 2; -10]
```

</details>
</span>
</p>
</div>

## Question 4

Take a `list` containing floats `1.0 .. 10.0`. Create a new list
that contains each number in the original list divided by `3.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.001, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: float list =
  [0.3333333333; 0.6666666667; 1.0; 1.333333333; 1.666666667; 2.0; 2.333333333;
   2.666666667; 3.0; 3.333333333]
```

</details>
</span>
</p>
</div>

## Question 5

Take a `list` containing floats `1.0 .. 10.0`. Group the elements based on whether the elements are greater than or equal to `4.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: (bool * float list) list =
  [(false, [1.0; 2.0; 3.0]); (true, [4.0; 5.0; 6.0; 7.0; 8.0; 9.0; 10.0])]
```

</details>
</span>
</p>
</div>

## Question 6

Take a `list` containing floats `1.0 .. 10.0`. Filter it so that you are left with the elements `> 5.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: float list = [6.0; 7.0; 8.0; 9.0; 10.0]
```

</details>
</span>
</p>
</div>

## Question 7

Given the list below, return tuples of all consecutive pairs.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: (int * int) list = [(1, -4); (-4, 7); (7, 2); (2, -10)]
```

</details>
</span>
</p>
</div>

## Question 8

Given the list below, return sliding windows of 3 consecutive observations.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: int list list = [[1; -4; 7]; [-4; 7; 2]; [7; 2; -10]]
```

</details>
</span>
</p>
</div>

## Question 9

Given the list below, sum all the elements.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: int = -4
```

</details>
</span>
</p>
</div>

## Question 10

Given the list below, add `1` to all the elements and then calculate the sum.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: int = 1
```

</details>
</span>
</p>
</div>

## Question 11

Given the list below, calculate the `average` of the elements in the list.

```fsharp
[ 1.0; -4.0; 7.0; 2.0; -10.0]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: float = -0.8
```

</details>
</span>
</p>
</div>

## Question 12

Given the list below, convert each element to a `decimal` and then calculate the `average` of the elements in the list.

```fsharp
[ 1.0; -4.0; 7.0; 2.0; -10.0]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.001, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: decimal = -0.8M
```

Since `decimal` is a function that converts to
the `decimal` type, you could also do.
The FSharp linter shouLd show you a blue squiggly
in the above code telling you this.

```
Real: 00:00:00.001, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: decimal = -0.8M
```

</details>
</span>
</p>
</div>

## Question 13

Take a `list` containing floats `1.0 .. 10.0`. Use `List.groupBy` to group the elements based on if they're `>= 5.0`. Then use `List.map` to get the maxiumum element that is `< 5.0` and the minimum value that is `>= 5.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.004, CPU: 00:00:00.015, GC gen0: 0, gen1: 0, gen2: 0
val groupedAboveBelow5: (bool * float list) list =
  [(false, [1.0; 2.0; 3.0; 4.0]); (true, [5.0; 6.0; 7.0; 8.0; 9.0; 10.0])]
val it: float list = [4.0; 5.0]
```

</details>
</span>
</p>
</div>

## Question 14

Take a `list` containing floats `1.0 .. 10.0`. Use functions from the List module to sort it in descending order. Then take the 3rd element of the reversed list and add `7.0` to it.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val descendingList: float list =
  [10.0; 9.0; 8.0; 7.0; 6.0; 5.0; 4.0; 3.0; 2.0; 1.0]
val thirdItem: float = 8.0
val it: float = 15.0
```

</details>
</span>
</p>
</div>

## Question 15

Take this list of lists, add `1.0` to each element of the "inner" lists,
and then concatenate all the inner lists together.

```fsharp
[ [ 1.0; 2.0]
  [ 3.0; 4.0] ]  
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.001, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val listsToAdd: float list list = [[1.0; 2.0]; [3.0; 4.0]]
val it: float list = [2.0; 3.0; 4.0; 5.0]
```

v2, this is not a correct answer.
it has not concatenated the inner lists
into one big list

```
Real: 00:00:00.019, CPU: 00:00:00.015, GC gen0: 0, gen1: 0, gen2: 0
val it: float list list = [[2.0; 3.0]; [4.0; 5.0]]
```

v3 and v4 below are correct, the same output as v1

```
Real: 00:00:00.020, CPU: 00:00:00.015, GC gen0: 0, gen1: 0, gen2: 0
val it: float list = [2.0; 3.0; 4.0; 5.0]
```

</details>
</span>
</p>
</div>

## Question 16

Given `returns : ReturnOb list`, calculate the arithmetic average return
for every symbol each month.
Give the result as a `ReturnOb list` where the date is the last date for the symbol
each month.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
input.fsx (1,1)-(6,58) interactive warning Accessing the internal type, method or field 'Date@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(6,58) interactive warning Accessing the internal type, method or field 'Return@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(6,58) interactive warning Accessing the internal type, method or field 'Symbol@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
```

</details>
</span>
</p>
</div>

## Question 17

Given `returns : ReturnOb list`, calculate the monthly return
for every symbol each month.
Give the result as a `ReturnOb list` where the date is the last date for the symbol
each month.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
input.fsx (1,1)-(62,50) interactive warning Accessing the internal type, method or field 'Symbol@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(62,50) interactive warning Accessing the internal type, method or field 'Date@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(62,50) interactive warning Accessing the internal type, method or field 'Return@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
```

</details>
</span>
</p>
</div>

## Question 18

Given `returns : ReturnOb list`, calculate the standard deviation of daily returns
for every symbol each month.
Give the result as a `ValueOb list` where the date in each `ValueOb` is the last date for the symbol
each month.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
input.fsx (1,1)-(7,21) interactive warning Accessing the internal type, method or field 'Date@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(7,21) interactive warning Accessing the internal type, method or field 'Symbol@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(7,21) interactive warning Accessing the internal type, method or field 'Return@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
```

</details>
</span>
</p>
</div>

## Question 19

Given `returns : ReturnOb list`, calculate the standard deviation of daily returns
for every symbol using rolling 3 day windows.
Give the result as a `ValueOb list` where the date in each `ValueOb` is the last date for the symbol
in the window.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
input.fsx (1,1)-(11,54) interactive warning Accessing the internal type, method or field 'Symbol@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(11,54) interactive warning Accessing the internal type, method or field 'Date@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(11,54) interactive warning Accessing the internal type, method or field 'Return@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
```

Breaking this answer down,
If you're unsure, it's helpful to work through things step by step.
then build up from there.

```
input.fsx (1,1)-(3,39) interactive warning Accessing the internal type, method or field 'Symbol@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
```

```
input.fsx (1,18)-(1,24) typecheck error The value or constructor 'groups' is not defined. Maybe you want one of the following:
   groupedAboveBelow5
input.fsx (9,65)-(9,71) typecheck error The type 'ValueOb' does not define the field, constructor or member 'Return'.
```

Now take the inner-most code operating on a single window
and make a function by copying and pasting inside a function.
often using more general variable names

```
input.fsx (1,1)-(6,23) interactive warning Accessing the internal type, method or field 'Return@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(6,23) interactive warning Accessing the internal type, method or field 'Symbol@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(6,23) interactive warning Accessing the internal type, method or field 'Date@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val resultForWindow: window: ReturnOb list -> ValueOb
```

test it on your window

```
input.fsx (1,49)-(1,60) typecheck error The value or constructor 'firstWindow' is not defined.
```

check

```
input.fsx (1,1)-(1,18) typecheck error The value or constructor 'firstWindowResult' is not defined.
input.fsx (1,21)-(1,46) typecheck error The value or constructor 'firstWindowFunctionResult' is not defined.
```

now a function to create the windows

```
input.fsx (1,1)-(4,23) interactive warning Accessing the internal type, method or field 'Date@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val createWindows: days: ReturnOb list -> ReturnOb list list
```

check

```
input.fsx (1,16)-(1,24) typecheck error The value or constructor 'firstObs' is not defined.
input.fsx (1,28)-(1,44) typecheck error The value or constructor 'windowedFirstObs' is not defined. Maybe you want one of the following:
   Windows
```

so now we can do

```
input.fsx (1,1)-(1,9) typecheck error The value or constructor 'firstObs' is not defined.
```

Cool, now first obs was the obs from the first group.
we could do function to operate on a group.
our group is a tuple of `(string,ReturnObs list)`.
We're not going to use the `string` variable, so we'll preface it
with ** to let the compiler know we're leaving it out o purpose.
the ** is not necessary but it's good practice

```
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val resultsForGroup: _symbol: 'a * xs: ReturnOb list -> ValueOb list
```

test it on the first group

```
input.fsx (1,17)-(1,27) typecheck error The value or constructor 'firstGroup' is not defined.
```

now make the group and apply my
group function to each group

```
input.fsx (1,1)-(4,32) interactive warning Accessing the internal type, method or field 'Symbol@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
```

Okay, but this is an list of `ValueOb list` (that's what `ValueOb list list` means).
What happened is that I had an list of groups, and then I transformed each group.
so it's still one result per group. For instance

```
input.fsx (1,1)-(1,20) typecheck error The value or constructor 'resultsForEachGroup' is not defined. Maybe you want one of the following:
   resultsForGroup
   Result
   result
   Result
```

is the first group of results

```
input.fsx (1,1)-(1,20) typecheck error The value or constructor 'resultsForEachGroup' is not defined. Maybe you want one of the following:
   resultsForGroup
   Result
   result
   Result
```

is the second group. I don't want an list of lists.
I just want one list of value obs. So `concat` them.

```
input.fsx (2,5)-(2,24) typecheck error The value or constructor 'resultsForEachGroup' is not defined. Maybe you want one of the following:
   resultsForGroup
   Result
   result
   Result
```

what's the first thing in the list?

```
input.fsx (1,1)-(1,32) typecheck error The value or constructor 'resultsForEachGroupConcatenated' is not defined. Maybe you want one of the following:
   resultsForGroup
```

`Collect` does the `map` and `concat` in one step.

```
input.fsx (1,1)-(4,36) interactive warning Accessing the internal type, method or field 'Symbol@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
```

check, this should evaluate to `true`

```
input.fsx (1,1)-(1,32) typecheck error The value or constructor 'resultsForEachGroupConcatenated' is not defined. Maybe you want one of the following:
   resultsForGroup
input.fsx (1,38)-(1,66) typecheck error The value or constructor 'resultsForEachGroupCollected' is not defined. Maybe you want one of the following:
   resultsForGroup
```

why did I write the answer using an anonymous function instead of functions like this?
I use reusable functions for something I'm going to use multiple times.
If it's something I'll do once, and it's not too many lines, then I use
the anonymous lambda function. As you get more experience, you can code using
the type signatures to tell you what everything is. And I don't actually
have to running it step by step.
however, starting out especially, I think you'll find it helpful
to kinda break things down like I did here.
Another way you can do it, similar to the first answer using
an anonymous lambda function, but now we'll do it with fewer
nested lists by concatenating/collecting the windows
into the parent list before doing the standard deviations.

```
input.fsx (1,1)-(11,6) interactive warning Accessing the internal type, method or field 'Symbol@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(11,6) interactive warning Accessing the internal type, method or field 'Date@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
```

first group of windows

```
input.fsx (1,1)-(1,18) typecheck error The value or constructor 'm2GroupsOfWindows' is not defined.
```

second group of windows

```
input.fsx (1,1)-(1,18) typecheck error The value or constructor 'm2GroupsOfWindows' is not defined.
```

Now concatenate the windows.

```
input.fsx (1,37)-(1,54) typecheck error The value or constructor 'm2GroupsOfWindows' is not defined.
```

same as if I'd used collect instead of map and then concat

```
input.fsx (2,5)-(2,13) typecheck error The value or constructor 'm2Groups' is not defined.
```

compare them

```
input.fsx (1,27)-(1,56) typecheck error The value or constructor 'm2GroupsOfWindowsConcatenated' is not defined.
input.fsx (2,24)-(2,50) typecheck error The value or constructor 'm2GroupsOfWindowsCollected' is not defined.
```

If they're not true, make sure they're sorted the same before you take the first obs.

Now, standard deviations of the windows' returns

```
input.fsx (2,5)-(2,31) typecheck error The value or constructor 'm2GroupsOfWindowsCollected' is not defined.
input.fsx (7,48)-(7,54) typecheck error The type 'ValueOb' does not define the field, constructor or member 'Return'.
```

</details>
</span>
</p>
</div>

*)

