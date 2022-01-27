(**
---
title: Volatility Timing Part 2
category: Practice Quizzes
categoryindex: 2
index: 3
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
We're going to use the following in the questions
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
    [| 
        for symbol in ["AAPL"; "TSLA"] do
        for month in [1..2] do
        for day in [1..3] do
            { Symbol = symbol 
              Date = DateTime(2021, month, day)
              Return = normal.Sample()}
    |]


(**
## Question 1
Take this array of arrays, add `1.0` to each element of the "inner" arrays,
and then concatenate all the inner arrays together.
*)
[| [| 1.0; 2.0|]
   [| 3.0; 4.0|] |]

(*** include-it-raw:preDetails ***)

(*** define: arraysAdd1, define-output: arraysAdd1 ***)
[| [| 1.0; 2.0|]
   [| 3.0; 4.0|] |]
|> Array.collect(fun xs -> xs |> Array.map(fun x -> x + 1.0))
(*** condition:html, include:arraysAdd1 ***)
(*** condition:html, include-fsi-output:arraysAdd1 ***)

(**
or
*)

(*** define: arraysAdd11, define-output: arraysAdd11 ***)
[| [| 1.0; 2.0|]
   [| 3.0; 4.0|] |]
|> Array.map(fun xs -> xs |> Array.map(fun x -> x + 1.0))
|> Array.concat

(*** condition:html, include:arraysAdd11 ***)
(*** condition:html, include-fsi-output:arraysAdd11 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Take the following two-parameter function:
*)

let add x y = x + y

(**
Use the above function and [partial application](https://fsharpforfunandprofit.com/posts/partial-application/)
to define a new function called 
`add2` that adds 2 
to it's input.
*)

(*** include-it-raw:preDetails ***)
(*** define: twoParaFunction, define-output: twoParaFunction ***)

let add2 = add 2

(*** condition:html, include:twoParaFunction ***)
(*** condition:html, include-fsi-output:twoParaFunction ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Given `returns : ReturnOb []`, use `printfn` to print the whole
array to standard output using the [structured plaintext formatter](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/plaintext-formatting). 
*)

(*** include-it-raw:preDetails ***)
(*** define: printfnStructuredObject, define-output: printfnStructuredObject ***)

returns |> (printfn "%A")

(*** condition:html, include:printfnStructuredObject ***)
(*** condition:html, include-output:printfnStructuredObject ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 4
Given the tuple `("hi", false, 20.321, 4)`,
use `printfn` and the tuple to print the following string
to standard output:
`"hi teacher, my False knowledge implies that 4%=0020.1"`

[String formatting](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/plaintext-formatting#format-specifiers-for-printf) documentation will be useful. 
*)

(*** include-it-raw:preDetails ***)

(*** define: printfnStringInterpolation, define-output: printfnStringInterpolation ***)
let (xString, xBool, xFloat, xInt) = ("hi", false, 20.321, 4)
(*** condition:html, include:printfnStringInterpolation ***)
(*** condition:html, include-fsi-output:printfnStringInterpolation ***)

(**
Using string interpolation
*)

(*** define: printfnStringInterpolation1, define-output: printfnStringInterpolation1 ***)
printfn $"{xString} teacher, my {xBool} knowledge implies that {xInt}%%=%06.1f{xFloat}"
(*** condition:html, include:printfnStringInterpolation1 ***)
(*** condition:html, include-output:printfnStringInterpolation1 ***)

(**
Using old-style printfn
*)

(*** define: printfnStringInterpolation2, define-output: printfnStringInterpolation2 ***)
printfn "%s teacher, my %b knowledge implies that %i%%=%06.1f" xString xBool xInt xFloat
(*** condition:html, include:printfnStringInterpolation2 ***)
(*** condition:html, include-output:printfnStringInterpolation2 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 5
Given `returns : ReturnOb []`, calculate the arithmetic average return 
for every symbol each month.
Give the result as a `ReturnOb []` where the date is the last date for the symbol
each month.
*)

(*** include-it-raw:preDetails ***)
(*** define: arithmeticReturn, define-output: arithmeticReturn ***)

returns
|> Array.groupBy(fun x -> x.Symbol, x.Date.Year, x.Date.Month)
|> Array.map(fun ((symbol, _year, _month), xs) ->
    { Symbol = symbol 
      Date = xs |> Array.map(fun x -> x.Date) |> Array.max 
      Return = xs|> Array.averageBy(fun x -> x.Return) })

(*** condition:html, include:arithmeticReturn ***)
(*** condition:html, include-fsi-output:arithmeticReturn ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 6
Given `returns : ReturnOb []`, calculate the monthly return 
for every symbol each month.
Give the result as a `ReturnOb []` where the date is the last date for the symbol
each month. 
*)

(*** include-it-raw:preDetails ***)
(*** define: monthlyReturn, define-output: monthlyReturn ***)

returns
|> Array.groupBy(fun x -> x.Symbol, x.Date.Year, x.Date.Month)
|> Array.map(fun ((symbol, _year, _month), xs) ->
    let monthReturnPlus1 = (1.0, xs) ||> Array.fold(fun acc x -> acc * (1.0 + x.Return))
    { Symbol = symbol 
      Date = xs |> Array.map(fun x -> x.Date) |> Array.max 
      Return =  monthReturnPlus1 - 1.0 })

(*** condition:html, include:monthlyReturn ***)
(*** condition:html, include-fsi-output:monthlyReturn ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 7
Given `returns : ReturnOb []`, calculate the standard deviation of daily returns
for every symbol each month.
Give the result as a `ValueOb []` where the date in each `ValueOb` is the last date for the symbol
each month. 
*)

(*** include-it-raw:preDetails ***)
(*** define: monthlyStdDev, define-output: monthlyStdDev ***)

returns
|> Array.groupBy(fun x -> x.Symbol, x.Date.Year, x.Date.Month)
|> Array.map(fun ((symbol, _year, _month), xs) ->
    let sd = xs |> stDevBy(fun x -> x.Return)
    { Symbol = symbol 
      Date = xs |> Array.map(fun x -> x.Date) |> Array.max 
      Value =  sd })

(*** condition:html, include:monthlyStdDev ***)
(*** condition:html, include-fsi-output:monthlyStdDev ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 8
Given `returns : ReturnOb []`, calculate the standard deviation of daily returns
for every symbol using rolling 3 day windows.
Give the result as a `ValueOb []` where the date in each `ValueOb` is the last date for the symbol
in the window. 
*)

(*** include-it-raw:preDetails ***)

(***define:rollingStdDev, define-output:rollingStdDev ***)
returns
|> Array.groupBy(fun x -> x.Symbol)
|> Array.collect(fun (_symbol, xs) ->
    xs
    |> Array.sortBy(fun x -> x.Date)
    |> Array.windowed 3
    |> Array.map(fun ys -> 
        let last = ys |> Array.last 
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
    |> Array.groupBy(fun x -> x.Symbol)
(***condition:html,include:rollingStdDev1 ***)
(***condition:html,include-fsi-output:rollingStdDev1 ***)

(*** define: rollingStdDev2, define-output: rollingStdDev2 ***)
let firstGroup = groups |> Array.item 0 // or groups |> Array.head
let firstSymbol, firstObs = firstGroup // like let a,b = (1,2)
let windowedFirstObs = 
    firstObs
    |> Array.sortBy(fun x -> x.Date)
    |> Array.windowed 3
let firstWindow = windowedFirstObs.[0]
let lastDayOfFirstWindow = firstWindow |> Array.last
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
    let lastDay = window |> Array.last
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
let createWindows (days: ReturnOb array) =
    days
    |> Array.sortBy(fun day -> day.Date)
    |> Array.windowed 3
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
|> Array.map resultForWindow
(***condition:html,include:rollingStdDev8 ***)
(***condition:html,include-fsi-output:rollingStdDev8 ***)

(**
Cool, now first obs was the obs from the first group.
we could do function to operate on a group.
our group is a tuple of `(string,ReturnObs array)`.
We're not going to use the `string` variable, so we'll preface it
with _ to let the compiler know we're leaving it out o purpose.
the _ is not necessary but it's good practice
*)

(*** define: rollingStdDev9, define-output: rollingStdDev9 ***)
let resultsForGroup (_symbol, xs) =
    xs
    |> createWindows
    |> Array.map resultForWindow
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
    |> Array.groupBy(fun x -> x.Symbol)
    |> Array.map resultsForGroup
(***condition:html,include:rollingStdDev11 ***)
(***condition:html,include-fsi-output:rollingStdDev11 ***)

(**
Okay, but this is an array of `ValueOb arrays` (that's what `ValueOb [ ][ ]` means).
What happened is that I had an array of groups, and then I transformed each group.
so it's still one result per group. For instance
*)

(*** define: rollingStdDev12, define-output: rollingStdDev12 ***)
resultsForEachGroup.[0]
(***condition:html,include:rollingStdDev12 ***)
(***condition:html,include-fsi-output:rollingStdDev12 ***)

(**
is the first group of results
*)

(*** define: rollingStdDev13, define-output: rollingStdDev13 ***)
resultsForEachGroup.[1]
(***condition:html,include:rollingStdDev13 ***)
(***condition:html,include-fsi-output:rollingStdDev13 ***)

(**
is the second group. I don't want an array of arrays.
I just want one array of value obs. So `concat` them.
*)

(*** define: rollingStdDev14, define-output: rollingStdDev14 ***)
let resultsForEachGroupConcatenated =
    resultsForEachGroup |> Array.concat
(***condition:html,include:rollingStdDev14 ***)
(***condition:html,include-fsi-output:rollingStdDev14 ***)

(**
what's the first thing in the array?
*)

(*** define: rollingStdDev15, define-output: rollingStdDev15 ***)
resultsForEachGroupConcatenated.[0]  
(***condition:html,include:rollingStdDev15 ***)
(***condition:html,include-fsi-output:rollingStdDev15 ***)

(**
`Collect` does the `map` and `concat` in one step.
*)

(*** define: rollingStdDev16, define-output: rollingStdDev16 ***)
let resultsForEachGroupCollected =
    returns
    |> Array.groupBy(fun x -> x.Symbol)
    |> Array.collect resultsForGroup 
(***condition:html,include:rollingStdDev16 ***)
(***condition:html,include-fsi-output:rollingStdDev16 ***)

(**
check, this should evaluate to `true`
*)

(*** define: rollingStdDev17, define-output: rollingStdDev17 ***)
resultsForEachGroupConcatenated.[0] = resultsForEachGroupCollected.[0]
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
nested arrays by concatenating/collecting the windows
into the parent array before doing the standard deviations.
*)

(*** define: rollingStdDev18, define-output: rollingStdDev18 ***)
let m2Groups =
    returns
    |> Array.groupBy(fun x -> x.Symbol)

let m2GroupsOfWindows =
    m2Groups
    |> Array.map(fun (symbol, xs) -> 
        xs
        |> Array.sortBy(fun x -> x.Date)
        |> Array.windowed 3
    )
(***condition:html,include:rollingStdDev18 ***)
(***condition:html,include-fsi-output:rollingStdDev18 ***)

(**
first group of windows
*)

(*** define: rollingStdDev19, define-output: rollingStdDev19 ***)
m2GroupsOfWindows.[0]    
(***condition:html,include:rollingStdDev19 ***)
(***condition:html,include-fsi-output:rollingStdDev19 ***)

(**
second group of windows
*)

(*** define: rollingStdDev20, define-output: rollingStdDev20 ***)
m2GroupsOfWindows.[1]    
(***condition:html,include:rollingStdDev20 ***)
(***condition:html,include-fsi-output:rollingStdDev20 ***)

(**
 Now concatenate the windows.
*)

(*** define: rollingStdDev21, define-output: rollingStdDev21 ***)
let m2GroupsOfWindowsConcatenated = m2GroupsOfWindows |> Array.concat  
(***condition:html,include:rollingStdDev21 ***)
(***condition:html,include-fsi-output:rollingStdDev21 ***)

(**
same as if I'd used collect instead of map and then concat
*)

(*** define: rollingStdDev22, define-output: rollingStdDev22 ***)
let m2GroupsOfWindowsCollected =
    m2Groups
    |> Array.collect(fun (symbol, xs) -> 
        xs
        |> Array.sortBy(fun x -> x.Date)
        |> Array.windowed 3 
    )
(***condition:html,include:rollingStdDev22 ***)
(***condition:html,include-fsi-output:rollingStdDev22 ***)

(**
compare them
*)

(*** define: rollingStdDev23, define-output: rollingStdDev23 ***)
let m2FirstConcatenated = m2GroupsOfWindowsConcatenated.[0]    
let m2FirstCollected = m2GroupsOfWindowsCollected.[0]
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
    |> Array.map(fun window -> 
        let lastDay = window |> Array.last 
        { Symbol = lastDay.Symbol
          Date = lastDay.Date
          Value = window |> stDevBy(fun x -> x.Return )})

(***condition:html,include:rollingStdDev24 ***)
(***condition:html,include-fsi-output:rollingStdDev24 ***) 

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.
