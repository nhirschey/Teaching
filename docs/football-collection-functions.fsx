(**
---
title: Collection functions (with solutions)
category: Assignments
categoryindex: 2
index: 2
---
*)

(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


# Using List collection functions and calculating summary statistics.

> Developed with [Davide Costa](https://github.com/DavideGCosta)


You should now feel comfortable with the footballer dataset and how to work with
tuples, records, anonymous records. You should also know how to perform simple transformations.
With a large and heterogeneous dataset, it's useful to understand how to sort, group, 
and filter the data, and also many other interesting List functions.  

It is a good idea to browse the documentation for lists at the [F# language reference](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/Lists)
and the [F# core library](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-listmodule.html) documentation sites before you start. 
For further discussion of collection functions, the related [F# for fun and profit](https://fsharpforfunandprofit.com/posts/list-module-functions/)
page is also useful.

*)

(*** condition:prepare ***)
let makeNumberedHeading (htmlTag:string) (text:string) =
    let name = text.Replace(" ", "-")
    let snippet = sprintf $"<{htmlTag} class=numbered><a name={name} class=anchor href=#{name}>{text}</a></{htmlTag}>"
    snippet 

let H2 = makeNumberedHeading "h2"
let H3 = makeNumberedHeading "h3"

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
### Reference needed nuget packages and open namespaces
*)

#r "nuget: FSharp.Data, 5.0.2"
#r "nuget: FSharp.Stats, 0.5.0"

open FSharp.Data
open FSharp.Stats
open FSharp.Stats.Correlation

(**
### Load the Csv file.  

*)

let [<Literal>] CsvPath = __SOURCE_DIRECTORY__ + "/FootballPlayers.csv"
type FootballPlayersCsv = CsvProvider<CsvPath>

let playerStatsTable = 
    FootballPlayersCsv.GetSample().Rows
    |> Seq.toList

(**
## EXERCISES - PART 2

- [List Functions.](#List-Functions)

    1. [List.take](#1-List-take)
    1. [List.truncate](#2-List-truncate)
    1. [List.distinct](#3-List-distinct)
    1. [List.countBy](#4-List-countBy)
    1. [List.filter](#5-List-filter)
    1. [List.sort and List.sortDescending](#6-List-sort-and-List-sortDescending)
    1. [List.sortBy and List.sortByDescending](#7-List-sortBy-and-List-sortByDescending)
    1. [List.splitInto](#8-List-splitInto)
    1. [List.groupBy](#9-List-groupBy)

- [Statistics List Functions.](#Statistics-List-Functions)
    1. [List.max](#1-List-max)
    1. [List.min](#2-List-min)
    1. [List.maxBy](#3-List-maxBy)
    1. [List.minBy](#4-List-minBy)
    1. [List.sum](#5-List-sum)
    1. [List.sumBy](#6-List-sumBy)
    1. [List.average](#7-List-average)
    1. [List.averageBy](#8-List-averageBy)
    1. [Seq.stDev](#9-Seq-stDev)
    1. [Seq.pearsonOfPairs](#10-Seq-pearsonOfPairs)

- [Further Statistics practice.](#Further-Statistics-practice)
    1. [List.countBy, List.filter and List.averageBy](#1-List-countBy-List-filter-and-List-averageBy)
    1. [List.groupBy, List.map and transformations](#2-List-groupBy-List-map-and-transformations)
    1. [List.sortDescending, List.splitInto, List.map and Seq.stDev](#3-List-sortDescending-List-splitInto-List-map-and-Seq-stDev)
*)


(**
## List Functions.

*)


(**
### 1 List.take

`List.take 5` takes the first 5 rows.  
`List.take 2` takes the first 2 rows  

*)

(**
Example: Take the first 4 rows from `playerStatsTable` with `List.take`.
*)
playerStatsTable
|> List.take 4
(*** include-fsi-output ***)

(**
- Take the first 7 rows from `playerStatsTable` with `List.take`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListTake, define-output: ListTake ***)

playerStatsTable
|> List.take 7

(*** condition:html, include:ListTake ***)
(*** condition:html, include-fsi-output:ListTake ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 2 List.truncate
`List.truncate 5` takes the first 5 rows.  
`List.truncate 2` takes the first 2 rows  

You must have noted that `List.take` and `List.truncate` return similar outputs, but these are not exactly the same.
`List.take` gives you the exact number of items that you specify in the parameters, 
while `List.truncate` takes at maximum the number of items you specified in the parameters.
Thus, in most cases both give you the exact same output, except if you ask for more items then the ones available in the List (List length). 
In this particular scenario `List.truncate` returns the maximum number of elements (all the elements in the List), 
while `List.take` returns an error, since it is supposed to take the exact number of elements you asked for, which is impossible in this particular case. 
*)

(**
Example: Take the first 4 rows from `playerStatsTable` with `List.truncate`.
*)

playerStatsTable
|> List.truncate 4
(*** include-fsi-output ***)

(**
- Take the first 7 rows from `playerStatsTable` with `List.truncate`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListTruncate, define-output: ListTruncate ***)

playerStatsTable
|> List.truncate 7

(*** condition:html, include:ListTruncate ***)
(*** condition:html, include-fsi-output:ListTruncate ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 3 List.distinct

`List.distinct` returns the unique elements from the List.  
`["hello"; "world"; "hello"; "hi"] |> List.distinct` returns `["hello"; "world"; "hi"]`
*)

(**
Example: From `playerStatsTable` `Nation` field find the unique elements with `List.distinct`.  
*)

playerStatsTable
|> List.map(fun x -> x.Nation)
|> List.distinct
(*** include-fsi-output ***)

(**
- From `playerStatsTable` `League` field find the unique elements with `List.distinct`.  
*)

(*** include-it-raw:preDetails ***)
(*** define: ListDistinct, define-output: ListDistinct ***)

playerStatsTable
|> List.map(fun x -> x.League)
|> List.distinct

(*** condition:html, include:ListDistinct ***)
(*** condition:html, include-fsi-output:ListDistinct ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 4 List.countBy
 
`List.countBy` returns a list of paired tuples with the unique elements and their counts.
*)

(**
Example: From `playerStatsTable` `Team` field find the unique elements and their counts with `List.countBy`.
*)

playerStatsTable
|> List.countBy(fun x -> x.Team)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
- From `playerStatsTable` `League` field find the unique elements and their counts with `List.countBy`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListCountBy, define-output: ListCountBy ***)

playerStatsTable
|> List.countBy(fun x -> x.League)

(*** condition:html, include:ListCountBy ***)
(*** condition:html, include-fsi-output:ListCountBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 5 List.filter

`List.filter` allows you to extract a subset of the dataset based on one or multiple conditions.
*)

(**
Example: `Filter` the `playerStatsTable` to get only portuguese players. (`Nation = "pt POR"`).  
Remember that we have to look to the dataset to find the string correspondent to portuguese players,
which in this case is `"pt POR"`
*)

playerStatsTable
|> List.filter(fun x -> x.Nation = "pt POR")
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
- `Filter` the `playerStatsTable` to get only 16 year-old players. (`Age = 16`).
*)

(*** include-it-raw:preDetails ***)
(*** define: ListFilter, define-output: ListFilter ***)

playerStatsTable
|> List.filter(fun x -> x.Age = 16)

(*** condition:html, include:ListFilter ***)
(*** condition:html, include-fsi-output:ListFilter ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 6 List.sort and List.sortDescending

- `[1; 4; 5; 3; 6] |> List.sort` returns `[1; 3; 4; 5; 6]` (ascending sort).
- `[1; 4; 5; 3; 6] |> List.sortDescending` returns `[6; 5; 4; 3; 1]` (descending sort).
*)

(**
Example: map `playerStatsTable` to get a list of `Age` and sort it (ascending).  

Since we want to sort the age List we first use `List.map` to get only that List.
Then we use `List.sort` to sort it.
*)

playerStatsTable
|> List.map(fun x -> x.Age)
|> List.sort
|> List.truncate 60 //just to observe the first 60 values, not a part of the exercise.
(*** include-fsi-output ***)

(**
- map `playerStatsTable` to get a list of `GoalsScored` and sort it (ascending).  
Hint:
To sort the GoalsScored List you first need to use `List.map` to get only that List.
Then use `List.sort` to sort it.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListSort, define-output: ListSort ***)

playerStatsTable
|> List.map(fun x -> x.GoalsScored)
|> List.sort
|> List.truncate 60 //just to observe the first 60 values, not a part of the exercise.

(*** condition:html, include:ListSort ***)
(*** condition:html, include-fsi-output:ListSort ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
Example: Map `playerStatsTable` to get a list of `Age` and sort it (descending).  
  
Since we want to sort the age List we first use `List.map` to get only that List.
Then we use `List.sortDescending` to sort it.
*)

playerStatsTable
|> List.map(fun x -> x.Age)
|> List.sortDescending
|> List.truncate 60 //just to observe the first 60 values, not a part of the exercise.
(*** include-fsi-output ***)

(**
- Map `playerStatsTable` to get a list of `GoalsScored` and sort it (descending).  
Hint:
To sort the GoalsScored List you first need to use `List.map` to get only that List.
Then use `List.sortDescending` to sort it.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListSortDescending, define-output: ListSortDescending ***)

playerStatsTable
|> List.map(fun x -> x.GoalsScored)
|> List.sortDescending
|> List.truncate 60 //just to observe the first 60 values, not a part of the exercise.

(*** condition:html, include:ListSortDescending ***)
(*** condition:html, include-fsi-output:ListSortDescending ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 7 List.sortBy and List.sortByDescending

`List.sortBy` is very usefull to sort the dataset accordingly to a certain dataset field.  
*)

(**
Example: sort (ascending) `playerStatsTable` by `Age` (`List.sortBy`).
*)

playerStatsTable
|> List.sortBy(fun x -> x.Age)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
- sort (ascending) `playerStatsTable` by `GoalsScored` (`List.sortBy`).
*)

(*** include-it-raw:preDetails ***)
(*** define: ListSortBy, define-output: ListSortBy ***)

playerStatsTable
|> List.sortBy(fun x -> x.GoalsScored)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:ListSortBy ***)
(*** condition:html, include-fsi-output:ListSortBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.

(**
Example: sort (descending) `playerStatsTable` by `Age` (`List.sortByDescending`).
*)

playerStatsTable
|> List.sortByDescending(fun x -> x.Age)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
- sort (descending) `playerStatsTable` by `GoalsScored` (`List.sortByDescending`).
*)

(*** include-it-raw:preDetails ***)
(*** define: ListSortByDescending, define-output: ListSortByDescending ***)

playerStatsTable
|> List.sortByDescending(fun x -> x.GoalsScored)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:ListSortByDescending ***)
(*** condition:html, include-fsi-output:ListSortByDescending ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 8 List.splitInto

`List.splitInto` is very usefull to split your dataset into multiple subsets.
This function is commonly used to generate quantiles by splitting a sorted List.
For instance, for investment strategies financial assets are usually sorted by a certain signal 
and then splitted into quantiles. If the signal has a positive sign, it means that the long strategy consists of going long 
on the first quantile stocks, and the long-short strategy consists of going long on the first quantile stocks and short on the last quantile stocks.

Note: `List.splitInto` receives one parameter which refers to the number of groups you want to create out of the dataset.  
*)

(**
Example: Sort the `playerStatsTable` by `GoalsScored` and then split the dataset into 4 groups using `List.sortBy` and `List.splitInto`.
*)

playerStatsTable
|> List.sortBy(fun x -> x.GoalsScored)
|> List.splitInto 4
|> List.truncate 2 //just to observe the first 2 groups Lists, not a part of the exercise.
|> List.map(fun x -> x |> List.truncate 5) //just to observe the first 5 rows of each group List, not a part of the exercise.
(*** include-fsi-output ***)

(**
- Sort the `playerStatsTable` by `Age` and then split the dataset into 5 groups using `List.sortBy` and `List.splitInto`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListSplitInto, define-output: ListSplitInto ***)

playerStatsTable
|> List.sortBy(fun x -> x.Age)
|> List.splitInto 5
|> List.truncate 2 //just to observe the first 2 groups Lists, not a part of the exercise.
|> List.map(fun x -> x |> List.truncate 5) //just to observe the first 5 rows of each group List, not a part of the exercise.


(*** condition:html, include:ListSplitInto ***)
(*** condition:html, include-fsi-output:ListSplitInto ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 9 List.groupBy

`List.groupBy` allows you to group elements of a list.
It takes a key-generating function and a list as inputs.
The function is executed on each element of the List, returning a list of tuples
where the first element of each tuple is the key and the second is a list of the elements for which the function produced that key.

*)

(**
Example: Group the `playerStatsTable` by `Nation` using `List.groupBy`.
*)

playerStatsTable
|> List.groupBy(fun x -> x.Nation)
|> List.truncate 2 //just to observe the first 2 groups Lists, not a part of the exercise.
|> List.map(fun (x, xs) -> x, xs |> List.truncate 5) //just to observe the first 5 rows of each group List, not a part of the exercise.
(*** include-fsi-output ***)

(**
- Group the `playerStatsTable` by `Age` using `List.groupBy`.  
*)

(*** include-it-raw:preDetails ***)
(*** define: ListGroupBy, define-output: ListGroupBy ***)

playerStatsTable
|> List.groupBy(fun x -> x.Age)
|> List.map(fun (x, xs) -> x, xs |> List.truncate 5) //just to observe the first 5 rows of each group List, not a part of the exercise.
|> List.truncate 2 //just to observe the first 2 groups Lists, not a part of the exercise.

(*** condition:html, include:ListGroupBy ***)
(*** condition:html, include-fsi-output:ListGroupBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Statistics List Functions 
*)

(**
### 1 List.max

`[1; 4; 5; 3; 6] |> List.max` returns `6` (the highest value in the List).
*)

(**
Example: Map `playerStatsTable` to get the `Age` List, and find the maximum (`List.max`).
*)

playerStatsTable
|> List.map(fun x -> x.Age)
|> List.max
(*** include-fsi-output ***)

(**
- Map `playerStatsTable` to get the `GoalsScored` List, and find the maximum (`List.max`).
*)

(*** include-it-raw:preDetails ***)
(*** define: ListMax, define-output: ListMax ***)

playerStatsTable
|> List.map(fun x -> x.GoalsScored)
|> List.max

(*** condition:html, include:ListMax ***)
(*** condition:html, include-fsi-output:ListMax ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 2 List.min

`[1; 4; 5; 3; 6] |> List.min` returns `1` (the lowest value in the List).
*)

(**
Example: Map `playerStatsTable` to get the `Age` List, and find the minimum (`List.min`).
*)

playerStatsTable
|> List.map(fun x -> x.Age)
|> List.min
(*** include-fsi-output ***)

(**
- Map `playerStatsTable` to get the `GoalsScored` List, and find the minimum (`List.min`).
*)

(*** include-it-raw:preDetails ***)
(*** define: ListMin, define-output: ListMin ***)

playerStatsTable
|> List.map(fun x -> x.GoalsScored)
|> List.min

(*** condition:html, include:ListMin ***)
(*** condition:html, include-fsi-output:ListMin ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 3 List.maxBy

Sometimes you want the element with the "maximum y" where "y" is the result of applying a particular function to a list element. This is what `List.maxBy` is for. This function is best understood by seeing an example.
*)

(**
Example: Find the player in `playerStatsTable` with the maximum `Age` using `maxBy`. What we need to do then is write a function that takes a player as input and outputs the players age. `List.maxBy` will then find the player that is the maxiumum after transforming it using this function.
*)

playerStatsTable
|> List.maxBy(fun x -> x.Age)
(*** include-fsi-output ***)

(**
- Find the maximum `playerStatsTable` row by `GoalsScored` using `maxBy`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListMaxBy, define-output: ListMaxBy ***)

playerStatsTable
|> List.maxBy(fun x -> x.GoalsScored)

(*** condition:html, include:ListMaxBy ***)
(*** condition:html, include-fsi-output:ListMaxBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 4 List.minBy

Sometimes you want the element with the "minimum y" where "y" is the result of applying a particular function to a list element. This is what `List.minBy` is for.  
*)

(**
Example: Find the player in `playerStatsTable` with the minimum `Age` using `minBy`.
*)

playerStatsTable
|> List.minBy(fun x -> x.Age)
(*** include-fsi-output ***)

(**
- Find the minimum `playerStatsTable` row by `GoalsScored` using `minBy`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListMinBy, define-output: ListMinBy ***)

playerStatsTable
|> List.minBy(fun x -> x.GoalsScored)

(*** condition:html, include:ListMinBy ***)
(*** condition:html, include-fsi-output:ListMinBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 5 List.sum

`[1; 4; 5; 3; 6] |> List.sum` returns `19` (sum of the List elements).
*)

(**
Example: Calculate the total number of years lived by all players. Hint: transform (`List.map`) each element of `playerStatsTable` into an integer representing the player's `Age` and then get the sum (`List.sum`) of all the players' ages (the result should be an `int`).
*)

playerStatsTable
|> List.map(fun x -> x.Age)
|> List.sum
(*** include-fsi-output ***)

(**
- Calculate the total goals scored (`GoalsScored`) by all players in `playerStatsTable`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListSum, define-output: ListSum ***)

playerStatsTable
|> List.map(fun x -> x.GoalsScored)
|> List.sum

(*** condition:html, include:ListSum ***)
(*** condition:html, include-fsi-output:ListSum ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 6 List.sumBy

We are using a dataset that has multiple fields per List element. If you want to get the sum for particular fields it convenient to use `List.sumBy`.
It takes a function and transforms each element using that function and afterward sums all the transformed elements. It is like an `List.map` and `List.sum` combined into one function.
*)

(**
Example: Use `List.sumBy` to calculate the total number of years lived by all players in `playerStatsTable`. Remember that each player has lived `Age` years.
*)

playerStatsTable
|> List.sumBy(fun x -> x.Age)
(*** include-fsi-output ***)

(**
- Find the sum of the `GoalsScored` by all players in `playerStatsTable` using `List.sumBy`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListSumBy, define-output: ListSumBy ***)

playerStatsTable
|> List.sumBy(fun x -> x.GoalsScored)

(*** condition:html, include:ListSumBy ***)
(*** condition:html, include-fsi-output:ListSumBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**

### 7 List.average

`[1.0; 2.0; 5.0; 2.0] |> List.average` returns `2.5` (the average of all the List elements).
*)

(**
Example: Transform `playerStatsTable` into a list of the players' ages (`Age`) and find the average `Age` (`List.average`).  
The field `x.Age` needs to be transformed from `int` to `float` because `List.average` only works with `floats` or `decimals`.
*)

playerStatsTable
|> List.map(fun x -> float x.Age)
|> List.average
(*** include-fsi-output ***)

(**
- Use `List.map` to transform `playerStatsTable` into a list of the players' `GoalsScored` and find the average `GoalsScored` (`List.average`).  
Hint: The variable `x.GoalsScored` needs to be transformed from `int` to `float` since `List.average` only works with `floats` or `decimals`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListAverage, define-output: ListAverage ***)

playerStatsTable
|> List.map(fun x -> float x.GoalsScored)
|> List.average

(*** condition:html, include:ListAverage ***)
(*** condition:html, include-fsi-output:ListAverage ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 8 List.averageBy

We are using a dataset that has multiple fields per List element. If you want to get the average for particular fields it convenient to use `List.averageBy`.
It takes a function and transforms each element using that function and afterward averages all the transformed elements. It is like an `List.map` and `List.average` combined into one function.
*)

(**
Example: Find the average `Age` using `List.averageBy`.  
The `Age` needs to be transformed from `int` to `float` since `List.averageBy` only works with `floats` or `decimals`.
*)

playerStatsTable
|> List.averageBy(fun x -> float x.Age)
(*** include-fsi-output ***)

(**
- Find the average `GoalsScored` using `List.averageBy`.  
Hint: The `GoalsScored` needs to be transformed from `int` to `float` since `List.averageBy` only works with `floats` or `decimals`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListAverageBy, define-output: ListAverageBy ***)

playerStatsTable
|> List.averageBy(fun x -> float x.GoalsScored)

(*** condition:html, include:ListAverageBy ***)
(*** condition:html, include-fsi-output:ListAverageBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 9 Seq.stDev

For `Seq.stDev` to work, we loaded the `FSharp.Stats nuget` (`#r "nuget: FSharp.Stats, 0.5.0"`).
This nuget contains the standard deviation function.
Besides this we also opened the module `FSharp.Stats` (`open FSharp.Stats`).
[FSharp.Stats documentation](https://fslab.org/FSharp.Stats/)
*)

(**
Example: Use `List.map` to transform `playerStatsTable` by `GoalsScored` and find the standard deviation. (`Seq.stDev`).  
Note that for `Seq.stDev` to work the values need to be `floats` or `decimals`, so we need to transform the `GoalsScored` from `int` to `float`.
*)

playerStatsTable
|> List.map(fun x -> float x.GoalsScored)
|> Seq.stDev
(*** include-fsi-output ***)

(**
- Transform `playerStatsTable` into a list of the players' `Age`'s and find the standard deviation. (`Seq.stDev`).  
Hint: You need to transform `Age` values from `int` to `floats`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ListStDev, define-output: ListStDev ***)

playerStatsTable
|> List.map(fun x -> float x.Age)
|> Seq.stDev

(*** condition:html, include:ListStDev ***)
(*** condition:html, include-fsi-output:ListStDev ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 10 Seq.pearsonOfPairs

In order to perform correlations we have to load and open the namespace `FSharp.Stats`.  
Also, we `open FSharpe.Stats.Correlation` to allow a easier access to the correlation functions.  

It will be helpfull to check the [FSharp.Stats.Correlation Documentation](https://fslab.org/FSharp.Stats/reference/fsharp-stats-correlation-seq.html#pearson) before starting the exercises.  
*)

(**
Example: Test the correlation between `MatchesPlayed` and `GoalsScored` using `pearsonOfPairs`.  

`Seq.pearsonOfPairs` expects a list of tuples (x1 * x2), computing the correlation between x1 and x2. 
So we use `List.map` to get a list of tuples with (`MatchesPlayed`, `GoalsScored`).
Then we only need to pipe (`|>`) to `Seq.pearsonOfPairs`. 
*)

playerStatsTable
|> List.map(fun x -> x.MatchesPlayed, x.GoalsScored)
|> Seq.pearsonOfPairs
(*** include-fsi-output ***)

(**
- Test the correlation between `MatchesPlayed` and `Age` using `pearsonOfPairs`.  
Hints:
`Seq.pearsonOfPairs` expects a list of tuples (x1 * x2). Use `List.map` to get a list of tuples with (`MatchesPlayed`, `Age`).
Then you only need to pipe (`|>`) to `Seq.pearsonOfPairs`.  
*)

(*** include-it-raw:preDetails ***)
(*** define: pearsonMatchesAndAge, define-output: pearsonMatchesAndAge ***)

playerStatsTable
|> List.map(fun x -> x.MatchesPlayed, x.Age)
|> Seq.pearsonOfPairs

(*** condition:html, include:pearsonMatchesAndAge ***)
(*** condition:html, include-fsi-output:pearsonMatchesAndAge ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.

(**
- Test the correlation between `GoalsScored` and `Age` using `pearsonOfPairs`.  
Hints:
`Seq.pearsonOfPairs` expects a list of tuples (x1 * x2). Use `List.map` to get a list of tuples with (`GoalsScored`, `Age`).
Then you only need to pipe (`|>`) to `Seq.pearsonOfPairs`.

*)

(*** include-it-raw:preDetails ***)
(*** define: pearsonGoalsAndAge, define-output: pearsonGoalsAndAge ***)

playerStatsTable
|> List.map(fun x -> x.GoalsScored, x.Age)
|> Seq.pearsonOfPairs

(*** condition:html, include:pearsonGoalsAndAge ***)
(*** condition:html, include-fsi-output:pearsonGoalsAndAge ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Further Statistics practice

Now that you should feel confortable with `List.filter`, `List.groupBy`, `List.splitInto`  
and also some f# statistics functions, let's combine those concepts together. 
*)


(**
### 1 List.countBy, List.filter and List.averageBy

Example: Find the average goals scored by portuguese players.
*) 

(**
In order to find the average goals for portuguese players we know that we need to use ``List.filter``.
But we need to know what is the string correspondent to portuguese players!
Using `List.distinct` or `List.countBy` we can observe all the `Nation` strings, which allow us to see that portuguese Nation string is `"pt POR"`.
*)

playerStatsTable
|> List.countBy(fun x -> x.Nation)

(**
Now that we know what is the Portuguese string we can filter `x.Nation = "pt POR"` in order to only get portuguese players' rows!
Then we can easily pipe it (`|>`) to `List.averageBy (fun x -> float x.Age)` to get the average age of portuguese players.
*)

playerStatsTable
|> List.filter(fun x -> x.Nation = "pt POR")
|> List.averageBy(fun x -> float x.Age)
(*** include-fsi-output ***)

(**
- Find the average age for players playing on the Premier League  .
Hint:
You'll first need to use `List.filter` to get only players from the Premier League (`x.League = "engPremier League"`).  
Then use averageBy to compute the average by age, don't forget to use `float x.Age` to transform age values to float type.  
*)


(*** include-it-raw:preDetails ***)
(*** define: filterAndAverageBy, define-output: filterAndAverageBy ***)

playerStatsTable
|> List.filter(fun x -> x.League = "engPremier League")
|> List.averageBy(fun x -> float x.Age)

(*** condition:html, include:filterAndAverageBy ***)
(*** condition:html, include-fsi-output:filterAndAverageBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 2. List.groupBy, List.map and transformations.


Example: Group `playerStatsTable` by `Team` and compute the average number of `GoalsScored`.
*)

//example using record:
type TeamAndAvgGls =
    { Team : string
      AvgGoalsScored : float }

playerStatsTable
|> List.groupBy(fun x -> x.Team)
|> List.map(fun (team, playerStats) -> 
    { Team = team
      AvgGoalsScored = playerStats |> List.averageBy(fun playerStats -> float playerStats.GoalsScored)})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
or
*)

//example using tuple:
playerStatsTable
|> List.groupBy(fun x -> x.Team)
|> List.map(fun (team, playerStats) -> team, playerStats |> List.averageBy(fun playerStats -> float playerStats.GoalsScored))
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
- Group `playerStatsTable` by `League` and then compute the Average `Age` by group.  
Hint: Use `groupBy` to group by league (`League`).  
Then use `averageBy` to compute the average by age (`Age`) and pipe it 
(`|>`) to `List.map` to organize the data in a record or tuple with League (`League`) and Average Age.   
*)

(*** include-it-raw:preDetails ***)
(*** define: AvgAgeByGroup, define-output: AvgAgeByGroup ***)

//solution using record:
type LeagueAndAvgAge =
    { League : string 
      AverageAge : float }

playerStatsTable
|> List.groupBy(fun x -> x.League)
|> List.map(fun (leagues, playerStats) ->
    { League = leagues
      AverageAge = playerStats |> List.averageBy(fun playerStats -> float playerStats.Age) })

//solution using tuples:
playerStatsTable
|> List.groupBy(fun x -> x.League)
|> List.map(fun (leagues, playerStats) -> 
    leagues, 
    playerStats |> List.averageBy(fun playerStats -> float playerStats.Age) )


(*** condition:html, include:AvgAgeByGroup ***)
(*** condition:html, include-fsi-output:AvgAgeByGroup ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 3 List.sortDescending, List.splitInto, List.map and Seq.stDev

- From `playerStatsTable` sort the players' `Age` (descending), split the dataset into quartiles (4-quantiles) and compute the standard deviation for each quantile.  
Hint: You only need the `Age` field from the dataset, so you can use `map` straight away to get the `Age` List.
Sort that List with `List.sortDescending`, and then split it into 4 parts using `List.splitInto`.
Finally use `List.map` to iterate through each quantile and apply the function `Seq.stDev`.
*)

(*** include-it-raw:preDetails ***)
(*** define: stDevByAgeGroup, define-output: stDevByAgeGroup ***)

playerStatsTable
|> List.map(fun x -> float x.Age)
|> List.sortDescending
|> List.splitInto 4
|> List.map(fun x -> x |> Seq.stDev)

(*** condition:html, include:stDevByAgeGroup ***)
(*** condition:html, include-fsi-output:stDevByAgeGroup ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.
