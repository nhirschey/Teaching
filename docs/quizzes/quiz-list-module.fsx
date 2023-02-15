(**
---
title: List module functions
category: Practice Quizzes
categoryindex: 2
index: 2
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
let normal = Distributions.ContinuousDistribution.normal 0.0 0.1

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
*)

(*** include-it-raw:preDetails ***)
(*** define: filter, define-output: filter ***)

[ 1; -4; 7; 2; -10]
|> List.filter(fun x -> x > 2)

(*** condition:html, include:filter ***)
(*** condition:html, include-fsi-output:filter ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Given the list below, take elements until you find one that is greater than `4`.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: takeWhile, define-output: takeWhile ***)

[ 1; -4; 7; 2; -10]
|> List.takeWhile(fun x -> x <= 4)

(*** condition:html, include:takeWhile ***)
(*** condition:html, include-fsi-output:takeWhile ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Given the list below, skip elements until you find one that is greater than `4`.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: skipWhile, define-output: skipWhile ***)

[ 1; -4; 7; 2; -10]
|> List.skipWhile(fun x -> x <= 4)

(*** condition:html, include:skipWhile ***)
(*** condition:html, include-fsi-output:skipWhile ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 4
Take a `list` containing floats `1.0 .. 10.0`. Create a new list
that contains each number in the original list divided by `3.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: List1, define-output: List1 ***)

// either of these is correct

// Option 1:
[ for x in [1.0 .. 10.0] do x / 3.0 ]

// Option 2: 
[ 1.0 .. 10.0]
|> List.map (fun x -> x / 3.0)

(*** condition:html, include:List1 ***)
(*** condition:html, include-fsi-output:List1 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 5
Take a `list` containing floats `1.0 .. 10.0`. Group the elements based on whether the elements are greater than or equal to `4.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: listGroupBy, define-output: listGroupBy ***)

[ 1.0 .. 10.0]
|> List.groupBy (fun x -> x >= 4.0)

(*** condition:html, include:listGroupBy ***)
(*** condition:html, include-fsi-output:listGroupBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 6
Take a `list` containing floats `1.0 .. 10.0`. Filter it so that you are left with the elements `> 5.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: listFilter, define-output: listFilter ***)

[ 1.0 .. 10.0]
|> List.filter (fun x -> x > 5.0)

(*** condition:html, include:listFilter ***)
(*** condition:html, include-fsi-output:listFilter ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.



(**
## Question 7
Given the list below, return tuples of all consecutive pairs.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: pairwise, define-output: pairwise ***)

[ 1; -4; 7; 2; -10]
|> List.pairwise

(*** condition:html, include:pairwise ***)
(*** condition:html, include-fsi-output:pairwise ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 8
Given the list below, return sliding windows of 3 consecutive observations.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: windowed, define-output: windowed ***)

[ 1; -4; 7; 2; -10]
|> List.windowed 3

(*** condition:html, include:windowed ***)
(*** condition:html, include-fsi-output:windowed ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.



(**
## Question 9
Given the list below, sum all the elements.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: sum, define-output: sum ***)

[ 1; -4; 7; 2; -10]
|> List.sum

(*** condition:html, include:sum ***)
(*** condition:html, include-fsi-output:sum ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 10
Given the list below, add `1` to all the elements and then calculate the sum.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: sumBy, define-output: sumBy ***)

[ 1; -4; 7; 2; -10]
|> List.sumBy(fun x -> x + 1)

(*** condition:html, include:sumBy ***)
(*** condition:html, include-fsi-output:sumBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 11
Given the list below, calculate the `average` of the elements in the list.
```fsharp
[ 1.0; -4.0; 7.0; 2.0; -10.0]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: average, define-output: average ***)

[ 1.0; -4.0; 7.0; 2.0; -10.0]
|> List.average

(*** condition:html, include:average ***)
(*** condition:html, include-fsi-output:average ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.



(**
## Question 12
Given the list below, convert each element to a `decimal` and then calculate the `average` of the elements in the list.

```fsharp
[ 1.0; -4.0; 7.0; 2.0; -10.0]
```
*)

(*** include-it-raw:preDetails ***)

(*** define: averageBy, define-output: averageBy ***)
[ 1.0; -4.0; 7.0; 2.0; -10.0]
|> List.averageBy(fun x -> decimal x)
(*** condition:html, include: averageBy ***)
(*** condition:html, include-fsi-output: averageBy ***)

(**
Since `decimal` is a function that converts to
the `decimal` type, you could also do.
The FSharp linter shouLd show you a blue squiggly
in the above code telling you this.
*)

(*** define: averageBy1, define-output: averageBy1 ***)
[ 1.0; -4.0; 7.0; 2.0; -10.0]
|> List.averageBy decimal
(*** condition:html, include: averageBy1 ***)
(*** condition:html, include-fsi-output: averageBy1 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.

(**
## Question 13
Take a `list` containing floats `1.0 .. 10.0`. Use `List.groupBy` to group the elements based on if they're `>= 5.0`. Then use `List.map` to get the maxiumum element that is `< 5.0` and the minimum value that is `>= 5.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: listGroupMaxAndMin, define-output: listGroupMaxAndMin ***)

let groupedAboveBelow5 =
    [ 1.0 .. 10.0]
    |> List.groupBy(fun x -> x >= 5.0)

// to see groups and observations
[ for (gt5, xs) in groupedAboveBelow5 do (gt5, xs) ]

// to see just groups
[ for (gt5, xs) in groupedAboveBelow5 do gt5 ]
// to see just observations in each group
[ for (gt5, xs) in groupedAboveBelow5 do xs ]
// to see first group
groupedAboveBelow5[0]
// to see second group
groupedAboveBelow5[1]

[ for (gt5, xs) in groupedAboveBelow5 do
    if gt5 then 
        xs |> List.min 
    else
        xs |> List.max ]

// equivalently
[ 1.0 .. 10.0]
|> List.groupBy(fun x -> x >= 5.0)
|> List.map (fun (gt5, xs) ->
    if gt5 then 
        xs |> List.min 
    else 
        xs |> List.max )

(*** condition:html, include:listGroupMaxAndMin ***)
(*** condition:html, include-fsi-output:listGroupMaxAndMin ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 14
Take a `list` containing floats `1.0 .. 10.0`. Use functions from the List module to sort it in descending order. Then take the 3rd element of the reversed list and add `7.0` to it.
*)

(*** include-it-raw:preDetails ***)
(*** define: listSort, define-output: listSort ***)

let descendingList =
    [1.0 .. 10.0]
    |> List.sortByDescending id

// index 2 = 3rd item because it is 0-indexed
let thirdItem = descendingList[2]

thirdItem + 7.0


(*** condition:html, include:listSort ***)
(*** condition:html, include-fsi-output:listSort ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.

(**
## Question 15
Take this list of lists, add `1.0` to each element of the "inner" lists,
and then concatenate all the inner lists together.

```fsharp
[ [ 1.0; 2.0]
  [ 3.0; 4.0] ]  
```
*)

(*** include-it-raw:preDetails ***)

(*** define: listsAdd1, define-output: listsAdd1 ***)
let listsToAdd = 
    [ [ 1.0; 2.0]
      [ 3.0; 4.0] ]

// Compare the output of these different versions. 

//v1
[ for list in listsToAdd do
    for x in list do x + 1.0 ]
(*** condition:html, include:listsAdd1 ***)
(*** condition:html, include-fsi-output:listsAdd1 ***)

(** 
v2, this is not a correct answer.
it has not concatenated the inner lists
into one big list
*)

(*** define: listsAdd2, define-output: listsAdd2 ***)
[ for xs in listsToAdd do
    [ for x in xs do x + 1.0] ]
(*** condition:html, include:listsAdd2 ***)
(*** condition:html, include-fsi-output:listsAdd2 ***)

(**
v3 and v4 below are correct, the same output as v1
*)

(*** define: listsAdd3, define-output: listsAdd3 ***)
//v3
[ for xs in listsToAdd do
    [ for x in xs do x + 1.0] ]
|> List.concat    

// v4
listsToAdd
|> List.collect(fun xs -> 
    [ for x in xs do x + 1.0 ])
(*** condition:html, include:listsAdd3 ***)
(*** condition:html, include-fsi-output:listsAdd3 ***)


(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 16
Given `returns : ReturnOb list`, calculate the arithmetic average return 
for every symbol each month.
Give the result as a `ReturnOb list` where the date is the last date for the symbol
each month.
*)

(*** include-it-raw:preDetails ***)
(*** define: arithmeticReturn, define-output: arithmeticReturn ***)

returns
|> List.groupBy(fun x -> x.Symbol, x.Date.Year, x.Date.Month)
|> List.map(fun ((symbol, _year, _month), xs) ->
    { Symbol = symbol 
      Date = xs |> List.map(fun x -> x.Date) |> List.max 
      Return = xs |> List.averageBy(fun x -> x.Return) })

(*** condition:html, include:arithmeticReturn ***)
(*** condition:html, include-fsi-output:arithmeticReturn ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 17
Given `returns : ReturnOb list`, calculate the monthly return 
for every symbol each month.
Give the result as a `ReturnOb list` where the date is the last date for the symbol
each month. 
*)

(*** include-it-raw:preDetails ***)
(*** define: monthlyReturn, define-output: monthlyReturn ***)

let groupsForMonthlyReturn =
    returns
    |> List.groupBy(fun x -> x.Symbol, x.Date.Year, x.Date.Month)

// look at the groups
[ for (group, obs) in groupsForMonthlyReturn do group ]

// look at the first observation for each group
// This works:
[ for (group, obs) in groupsForMonthlyReturn do obs ]

// but some custom printing makes it more clear
for (group, obs) in groupsForMonthlyReturn do 
    printfn $"-------"
    printfn $"group: {group}"
    printfn $"Observations:"
    obs |> List.iter (printfn "%A") 
    printfn $"-------\n"


// remember how we can calculate cumulative returns
let exampleSimpleReturns = 
    [ 0.1; -0.2; 0.3 ]
let exampleLogReturns =
    [ for ret in exampleSimpleReturns do log(1.0 + ret) ]
let cumulativeLogReturns = exampleLogReturns |> List.sum
let cumulativeSimpleReturns = exp(cumulativeLogReturns) - 1.0 
// compare cumulativeSimpleReturns to
(1.0 + 0.1)*(1.0+ -0.2)*(1.0 + 0.3)-1.0   

// now the returns
[ for ((symbol, year, month), obs) in groupsForMonthlyReturn do
    let cumulativeLogReturn =
        obs
        |> List.sumBy (fun ob -> log(1.0 + ob.Return))
    let cumulativeSimpleReturn = exp(cumulativeLogReturn) - 1.0
    let maxDate = 
        obs 
        |> List.map (fun ob -> ob.Date)
        |> List.max
    { Symbol = symbol
      Date = maxDate 
      Return = cumulativeSimpleReturn } ]

// The above code assigned intermediate values
// to make each of the steps easy to see.
// It is good to see intermediate values when you are learning.
// But when you have more experience, you can tell from the types
// what is going on and it becomes less necessary to assign
// intermediate values.
//
// Thus it would also be possible to do the same thing
// without assigning intermediate values using the code below.
//
// use whichever style is easiest for you.
returns
|> List.groupBy(fun x -> x.Symbol, x.Date.Year, x.Date.Month)
|> List.map(fun ((symbol, year, month), obs) ->
    let cumulativeLogReturn = obs |> List.sumBy (fun ob -> log (1.0+ ob.Return))
    { Symbol = symbol 
      Date = obs |> List.map(fun ob -> ob.Date) |> List.max 
      Return =  exp(cumulativeLogReturn) - 1.0 })

(*** condition:html, include:monthlyReturn ***)
(*** condition:html, include-fsi-output:monthlyReturn ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 18
Given `returns : ReturnOb list`, calculate the standard deviation of daily returns
for every symbol each month.
Give the result as a `ValueOb list` where the date in each `ValueOb` is the last date for the symbol
each month. 
*)

(*** include-it-raw:preDetails ***)
(*** define: monthlyStdDev, define-output: monthlyStdDev ***)

returns
|> List.groupBy(fun x -> x.Symbol, x.Date.Year, x.Date.Month)
|> List.map(fun ((symbol, _year, _month), xs) ->
    let sd = xs |> stDevBy(fun x -> x.Return)
    { Symbol = symbol 
      Date = xs |> List.map(fun x -> x.Date) |> List.max 
      Value =  sd })

(*** condition:html, include:monthlyStdDev ***)
(*** condition:html, include-fsi-output:monthlyStdDev ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 19
Given `returns : ReturnOb list`, calculate the standard deviation of daily returns
for every symbol using rolling 3 day windows.
Give the result as a `ValueOb list` where the date in each `ValueOb` is the last date for the symbol
in the window. 
*)

(*** include-it-raw:preDetails ***)

(***define:rollingStdDev, define-output:rollingStdDev ***)
returns
|> List.groupBy(fun x -> x.Symbol)
|> List.collect(fun (_symbol, xs) ->
    xs
    |> List.sortBy(fun x -> x.Date)
    |> List.windowed 3
    |> List.map(fun ys -> 
        let last = ys |> List.last 
        { Symbol = last.Symbol
          Date = last.Date
          Value = ys |> stDevBy(fun x -> x.Return)}))
(***condition:html,include:rollingStdDev ***)
(***condition:html,include-fsi-output:rollingStdDev ***)

(**
Breaking this answer down,
If you're unsure, it's helpful to work through things step by step.
then build up from there.
*)

(*** define: rollingStdDev1, define-output: rollingStdDev1 ***)
let groups = 
    returns
    |> List.groupBy(fun x -> x.Symbol)
(***condition:html,include:rollingStdDev1 ***)
(***condition:html,include-fsi-output:rollingStdDev1 ***)

(*** define: rollingStdDev2, define-output: rollingStdDev2 ***)
let firstGroup = groups[0] // or groups |> List.head
let firstSymbol, firstObs = firstGroup // like let a,b = (1,2)
let windowedFirstObs = 
    firstObs
    |> List.sortBy(fun x -> x.Date)
    |> List.windowed 3
let firstWindow = windowedFirstObs[0]
let lastDayOfFirstWindow = firstWindow |> List.last
let firstWindowReturnStdDev = firstWindow |> stDevBy(fun x -> x.Return)
let firstWindowResult =
    { Symbol = lastDayOfFirstWindow.Symbol 
      Date = lastDayOfFirstWindow.Date
      Value = firstWindowReturnStdDev }
(***condition:html,include:rollingStdDev2 ***)
(***condition:html,include-fsi-output:rollingStdDev2 ***)

(**
Now take the inner-most code operating on a single window
and make a function by copying and pasting inside a function.
often using more general variable names
*)

(*** define: rollingStdDev3, define-output: rollingStdDev3 ***)
let resultForWindow window =
    let lastDay = window |> List.last
    let stddev = window |> stDevBy(fun x -> x.Return)
    { Symbol = lastDay.Symbol 
      Date = lastDay.Date
      Value = stddev }
(***condition:html,include:rollingStdDev3 ***)
(***condition:html,include-fsi-output:rollingStdDev3 ***)

(**
test it on your window
*)

(*** define: rollingStdDev4, define-output: rollingStdDev4 ***)
let firstWindowFunctionResult = resultForWindow firstWindow
(***condition:html,include:rollingStdDev4 ***)
(***condition:html,include-fsi-output:rollingStdDev4 ***)

(**
check
*)

(*** define: rollingStdDev5, define-output: rollingStdDev5 ***)
firstWindowResult = firstWindowFunctionResult // evaluates to true
(***condition:html,include:rollingStdDev5 ***)
(***condition:html,include-fsi-output:rollingStdDev5 ***)

(**
now a function to create the windows
*)

(*** define: rollingStdDev6, define-output: rollingStdDev6 ***)
let createWindows (days: ReturnOb list) =
    days
    |> List.sortBy(fun day -> day.Date)
    |> List.windowed 3
(***condition:html,include:rollingStdDev6 ***)
(***condition:html,include-fsi-output:rollingStdDev6 ***)

(**
check
*)

(*** define: rollingStdDev7, define-output: rollingStdDev7 ***)
(createWindows firstObs) = windowedFirstObs // evaluates to true
(***condition:html,include:rollingStdDev7 ***)
(***condition:html,include-fsi-output:rollingStdDev7 ***)

(**
so now we can do
*)

(*** define: rollingStdDev8, define-output: rollingStdDev8 ***)
firstObs
|> createWindows
|> List.map resultForWindow
(***condition:html,include:rollingStdDev8 ***)
(***condition:html,include-fsi-output:rollingStdDev8 ***)

(**
Cool, now first obs was the obs from the first group.
we could do function to operate on a group.
our group is a tuple of `(string,ReturnObs list)`.
We're not going to use the `string` variable, so we'll preface it
with _ to let the compiler know we're leaving it out o purpose.
the _ is not necessary but it's good practice
*)

(*** define: rollingStdDev9, define-output: rollingStdDev9 ***)
let resultsForGroup (_symbol, xs) =
    xs
    |> createWindows
    |> List.map resultForWindow
(***condition:html,include:rollingStdDev9 ***)
(***condition:html,include-fsi-output:rollingStdDev9 ***)

(**
test it on the first group
*)

(*** define: rollingStdDev10, define-output: rollingStdDev10 ***)
resultsForGroup firstGroup
(***condition:html,include:rollingStdDev10 ***)
(***condition:html,include-fsi-output:rollingStdDev10 ***)

(**
now make the group and apply my 
group function to each group
*)

(*** define: rollingStdDev11, define-output: rollingStdDev11 ***)
let resultsForEachGroup =
    returns
    |> List.groupBy(fun x -> x.Symbol)
    |> List.map resultsForGroup
(***condition:html,include:rollingStdDev11 ***)
(***condition:html,include-fsi-output:rollingStdDev11 ***)

(**
Okay, but this is an list of `ValueOb list` (that's what `ValueOb list list` means).
What happened is that I had an list of groups, and then I transformed each group.
so it's still one result per group. For instance
*)

(*** define: rollingStdDev12, define-output: rollingStdDev12 ***)
resultsForEachGroup[0]
(***condition:html,include:rollingStdDev12 ***)
(***condition:html,include-fsi-output:rollingStdDev12 ***)

(**
is the first group of results
*)

(*** define: rollingStdDev13, define-output: rollingStdDev13 ***)
resultsForEachGroup[1]
(***condition:html,include:rollingStdDev13 ***)
(***condition:html,include-fsi-output:rollingStdDev13 ***)

(**
is the second group. I don't want an list of lists.
I just want one list of value obs. So `concat` them.
*)

(*** define: rollingStdDev14, define-output: rollingStdDev14 ***)
let resultsForEachGroupConcatenated =
    resultsForEachGroup |> List.concat
(***condition:html,include:rollingStdDev14 ***)
(***condition:html,include-fsi-output:rollingStdDev14 ***)

(**
what's the first thing in the list?
*)

(*** define: rollingStdDev15, define-output: rollingStdDev15 ***)
resultsForEachGroupConcatenated[0]  
(***condition:html,include:rollingStdDev15 ***)
(***condition:html,include-fsi-output:rollingStdDev15 ***)

(**
`Collect` does the `map` and `concat` in one step.
*)

(*** define: rollingStdDev16, define-output: rollingStdDev16 ***)
let resultsForEachGroupCollected =
    returns
    |> List.groupBy(fun x -> x.Symbol)
    |> List.collect resultsForGroup 
(***condition:html,include:rollingStdDev16 ***)
(***condition:html,include-fsi-output:rollingStdDev16 ***)

(**
check, this should evaluate to `true`
*)

(*** define: rollingStdDev17, define-output: rollingStdDev17 ***)
resultsForEachGroupConcatenated[0] = resultsForEachGroupCollected[0]
(***condition:html,include:rollingStdDev17 ***)
(***condition:html,include-fsi-output:rollingStdDev17 ***)

(**
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
*)

(*** define: rollingStdDev18, define-output: rollingStdDev18 ***)
let m2Groups =
    returns
    |> List.groupBy(fun x -> x.Symbol)

let m2GroupsOfWindows =
    m2Groups
    |> List.map(fun (symbol, xs) -> 
        xs
        |> List.sortBy(fun x -> x.Date)
        |> List.windowed 3
    )
(***condition:html,include:rollingStdDev18 ***)
(***condition:html,include-fsi-output:rollingStdDev18 ***)

(**
first group of windows
*)

(*** define: rollingStdDev19, define-output: rollingStdDev19 ***)
m2GroupsOfWindows[0]    
(***condition:html,include:rollingStdDev19 ***)
(***condition:html,include-fsi-output:rollingStdDev19 ***)

(**
second group of windows
*)

(*** define: rollingStdDev20, define-output: rollingStdDev20 ***)
m2GroupsOfWindows[1]    
(***condition:html,include:rollingStdDev20 ***)
(***condition:html,include-fsi-output:rollingStdDev20 ***)

(**
 Now concatenate the windows.
*)

(*** define: rollingStdDev21, define-output: rollingStdDev21 ***)
let m2GroupsOfWindowsConcatenated = m2GroupsOfWindows |> List.concat  
(***condition:html,include:rollingStdDev21 ***)
(***condition:html,include-fsi-output:rollingStdDev21 ***)

(**
same as if I'd used collect instead of map and then concat
*)

(*** define: rollingStdDev22, define-output: rollingStdDev22 ***)
let m2GroupsOfWindowsCollected =
    m2Groups
    |> List.collect(fun (symbol, xs) -> 
        xs
        |> List.sortBy(fun x -> x.Date)
        |> List.windowed 3 
    )
(***condition:html,include:rollingStdDev22 ***)
(***condition:html,include-fsi-output:rollingStdDev22 ***)

(**
compare them
*)

(*** define: rollingStdDev23, define-output: rollingStdDev23 ***)
let m2FirstConcatenated = m2GroupsOfWindowsConcatenated[0]    
let m2FirstCollected = m2GroupsOfWindowsCollected[0]
m2FirstCollected = m2FirstConcatenated // true. 

(***condition:html,include:rollingStdDev23 ***)
(***condition:html,include-fsi-output:rollingStdDev23 ***) 

(**
If they're not true, make sure they're sorted the same before you take the first obs.
*)

(**
Now, standard deviations of the windows' returns
*)

(*** define: rollingStdDev24, define-output: rollingStdDev24 ***)
let m2Result =
    m2GroupsOfWindowsCollected
    |> List.map(fun window -> 
        let lastDay = window |> List.last 
        { Symbol = lastDay.Symbol
          Date = lastDay.Date
          Value = window |> stDevBy(fun x -> x.Return )})

(***condition:html,include:rollingStdDev24 ***)
(***condition:html,include-fsi-output:rollingStdDev24 ***) 

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.
