(**
---
title: Plotting (with solutions)
category: Assignments
categoryindex: 2
index: 4
---
*)

(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

# Transformations and Plotting

> Developed with [Davide Costa](https://github.com/DavideGCosta)

*)

(*** condition:prepare ***)
#r "nuget: FSharp.Formatting"

type H2 = H2 of string 
type H3 = H3 of string 

let makeNumberedHeading (htmlTag:string) (text:string) =
    let name = text.Replace(" ", "-")
    let snippet = sprintf $"<{htmlTag} class=numbered><a name={name} class=anchor href=#{name}>{text}</a></{htmlTag}>"
    snippet 
fsi.AddPrinter(fun (H2 text) -> makeNumberedHeading "h2" text)
fsi.AddPrinter(fun (H3 text) -> makeNumberedHeading "h3" text)

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

#r "nuget:FSharp.Data"
#r "nuget: FSharp.Stats"
#r "nuget: Plotly.NET, 2.0.0-preview.16"

open FSharp.Data
open FSharp.Stats
open FSharp.Stats.Correlation
open Plotly.NET

(**
### Load the Csv file.  

*)

let [<Literal>] CsvPath = __SOURCE_DIRECTORY__ + "/FootballPlayers.csv"
type FootballPlayersCsv = CsvProvider<CsvPath>

let playerStatsTable = 
    FootballPlayersCsv.GetSample().Rows
    |> Seq.toList

(**
## EXERCISES - PART 3

- [List Functions and Plotting.](#List-Functions-and-Plotting)

    1. [Example: From playerStatsTable create a bar chart with the sum of GoalsScored (y-axis) by League (x-axis)](#1-Example-From-playerStatsTable-create-a-bars-chart-with-the-sum-of-GoalsScored-y-axis-by-League-x-axis)
    1. [From playerStatsTable create a bar chart with the sum of GoalsScored (y-axis) by Age(x-axis)](#2-From-playerStatsTable-create-a-bars-chart-with-the-sum-of-GoalsScored-y-axis-by-Age-x-axis)
    1. [Example: From playerStatsTable create a bars chart with the Average of GoalsScored (y-axis) by League (x-axis)](#3-Example-From-playerStatsTable-create-a-bars-chart-with-the-Average-of-GoalsScored-y-axis-by-League-x-axis)
    1. [From playerStatsTable create a bars chart with the average GoalsScored (y-axis) by Age(x-axis)](#4-From-playerStatsTable-create-a-bars-chart-with-the-average-GoalsScored-per-Match-Played-y-axis-by-Age-x-axis-only-for-Fowrward-Players)

*)

(**
## List Functions and Plotting

Now that you know how to work with the dataset using the most common List functions, we still need to practice how to create usefull plots with the data.  

For plotting in F# we load `Plotly.NET 2.0.0-beta9` nugget and open the `Plotly.NET` namespace.  
Documentation links:  

- [Plotly.NET General Documentation](https://plotly.net/)  
- [Plotly.NET bar and column charts Documentation](https://plotly.net/2_1_bar-and-column-charts.html)  
- [Plotly.NET line and scatter plots Documentation](https://plotly.net/2_0_line-scatter-plots.html)   

It will be helpfull to check the documentation before starting the exercises.  
*)

(**
### 1 Example: From playerStatsTable create a bar chart with the sum of GoalsScored (y-axis) by League (x-axis).

*)

let sumGoalsScoredByLeague = 
    playerStatsTable
    |> List.groupBy(fun x -> x.League)
    |> List.map(fun (league, playerStats) -> 
        (league,
         playerStats |> List.sumBy(fun x -> x.GoalsScored)))

let sumGoalsScoredByLeagueChart =
    sumGoalsScoredByLeague
    |> Chart.Column
    |> Chart.withYAxisStyle("Sum of Goals Scored")
    |> Chart.withXAxisStyle("Top 5 Football Leagues")
    |> Chart.withTitle("Sum of Goals Scored by League")
    //not a part of the exercise, just for formatting purposes.
    //|> GenericChart.mapLayout(fun layout -> 
    //    layout?width <- 1000.0
    //    layout)

(*** do-not-eval ***) 
sumGoalsScoredByLeagueChart

(*** hide ***)
sumGoalsScoredByLeagueChart |> GenericChart.toChartHTML
(*** include-it-raw ***)

(**
### 2 From playerStatsTable create a bar chart with the sum of GoalsScored (y-axis) by Age(x-axis).
Hints:
*)  
(**

- Group `playerStatsTable` by `Age` using `List.groupBy`.  
- Map the returned List of tuples, into an List of tuples with two elements.  
    - First element: age. The first element of the `List.groupBy` output. Which corresponds to the Age groups.
    - Second element: sum of goals scored by `Age`, use `List.sumBy` applied to the second element of the `List.groupBy` output. Which corresponds to all the rows per `Age` group.  
- Then, store this new List of tuples to a variable. (In this case call it `sumGoalsScoredByAge`).  
- After that in order to create a bar chart we need to follow the syntax: `Chart.Column`.
- Use the syntax `Chart.withYAxisStyle("Sum of Goals Score")` to label the y-axis.  
- Use the syntax `Chart.withXAxisStyle("Age")` to label the x-axis.  
- Use the syntax `Chart.withTitle("Sum of Goals Score by Age")` to give a title to the chart.
- Then Pipe (`|>`) to `Chart.Show` if you want to observe the chart.

*)

(*** define: SumGoalsByAge***)
let sumGoalsByAge =
    playerStatsTable
    |> List.groupBy(fun x -> x.Age)
    |> List.map(fun (ageGroups, playerStats) ->
        (ageGroups,
         playerStats |> List.sumBy(fun x -> x.GoalsScored)))

(*** define: SumGoalsByAgeChart***)
let sumGoalsByAgeChart =
    sumGoalsByAge
    |> Chart.Column
    |> Chart.withYAxisStyle("Sum of Goals Scored")
    |> Chart.withXAxisStyle("Age")
    |> Chart.withTitle("Sum of Goals Scored by Age")
    //not a part of the exercise, just for formatting purposes.
    //|> GenericChart.mapLayout(fun layout -> 
    //    layout?width <- 1000.0
    //    layout)

(*** do-not-eval, define: SumGoalsByAgeChartShow ***) 
sumGoalsByAgeChart

(*** hide, define-output:SumGoalsByAgeChartHTML ***)
sumGoalsByAgeChart |> GenericChart.toChartHTML

(*** condition:html, include-it-raw:preDetails ***)
(*** condition:html, include:SumGoalsByAge ***)
(*** condition:html, include:SumGoalsByAgeChart ***)
(*** condition:html, include:SumGoalsByAgeChartShow ***)
(*** condition:html, include-it-raw:SumGoalsByAgeChartHTML ***)
(*** condition:html, include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
### 3 Example: From playerStatsTable create a bar chart with the Average of GoalsScored (y-axis) by League (x-axis).
*)

let averageGoalsScoredByLeague = 
    playerStatsTable
    |> List.groupBy(fun x -> x.League)
    |> List.map(fun (league, playerStats) -> 
        (league,
         playerStats |> List.averageBy(fun x -> float x.GoalsScored)))

let averageGoalsScoredByLeagueChart = 
    averageGoalsScoredByLeague
    |> Chart.Column
    |> Chart.withYAxisStyle("Average of Goals Scored")
    |> Chart.withXAxisStyle("Top 5 Football Leagues")
    |> Chart.withTitle("Average of Goals Scored by League")
    //not a part of the exercise, just for formatting purposes.
    //|> GenericChart.mapLayout(fun layout -> 
    //    layout?width <- 1000.0
    //    layout)
(*** do-not-eval ***) 
averageGoalsScoredByLeagueChart

(*** hide ***)
averageGoalsScoredByLeagueChart |> GenericChart.toChartHTML
(*** include-it-raw ***)

(**
### 4 From playerStatsTable create a bars chart with the average GoalsScored per Match Played(y-axis) by Age(x-axis) only for Forward Players.

Hints: 
*)
(**

- Filter `playerStatsTable` by `Position` to get only forward position (`Position = "FW"`), use `List.Filter`.
- Group `playerStatsTable` by `Age` using `List.groupBy`.
- Map the returned List of tuples, into an List of tuples with two elements.

    - First element: age --> The first element of the `List.groupBy` output. Which corresponds to the Age groups.
    - Second element: sum of total GoalsScored divided by the total games played by age. 
                      Note that you can get the total games played by using `List.sumBy(fun x -> x.MatchesPlayed)`.
                      Also remember to transform the variables to `floats` before dividing to get the exact results.
                      
- Then, store this new List of tuples to a variable. (Name it `averageGoalsPerMatchByAgeFW`).
- After that, in order to create a bars chart we need to follow the syntax: `Chart.Column`, you just need to pipe (`|>`) the variable `averageGoalsPerMatchByAgeFW` to `Chart.Column`.
- Use the syntax `Chart.withYAxisStyle("Average of Goals Scored Per Match")` to label the y-axis.
- Use the syntax `Chart.withXAxisStyle("Age")` to label the x-axis.
- Use the syntax `Chart.withTitle("Average of Goals Scored Per Match by Age")` to give a title to the chart.
- Then Pipe (`|>`) to `Chart.Show` if you want to observe the chart.

*)


(*** define: averageGoalsPerMatchByAgeFW***)
let averageGoalsPerMatchByAgeFW =
    // Some players don't play many games in a season
    // so we need to normalize their skill by dividing by the number of games played.
    playerStatsTable
    |> List.filter(fun x -> x.Position = "FW" )
    |> List.groupBy(fun x -> x.Age)
    |> List.map(fun (age, playerStats) ->
        // weighted average, where the weight is games played.
        let totalGamesPlayed = playerStats |> List.sumBy(fun x -> x.MatchesPlayed)
        (   age,
            playerStats 
            |> List.sumBy(fun x -> 
                float x.GoalsScored/ float totalGamesPlayed )))

(*** define: averageGoalsPerMatchByAgeFWChart***)
let averageGoalsPerMatchByAgeFWChart = 
    averageGoalsPerMatchByAgeFW
    |> Chart.Column
    |> Chart.withYAxisStyle("Average of Goals Scored Per Match")
    |> Chart.withXAxisStyle("Age")
    |> Chart.withTitle("Average of Forward Position (FW) Goals Scored per Match By Age")
    //not a part of the exercise, just for formatting purposes.
    //|> GenericChart.mapLayout(fun layout -> 
    //    layout?width <- 1000.0
    //    layout)

(*** do-not-eval, define: averageGoalsPerMatchByAgeFWChartShow ***) 
averageGoalsPerMatchByAgeFWChart

(*** hide, define-output:averageGoalsPerMatchByAgeFWChartHTML ***)
averageGoalsPerMatchByAgeFWChart |> GenericChart.toChartHTML

(*** condition:html, include-it-raw:preDetails ***)
(*** condition:html, include:averageGoalsPerMatchByAgeFW ***)
(*** condition:html, include:averageGoalsPerMatchByAgeFWChart ***)
(*** condition:html, include:averageGoalsPerMatchByAgeFWChartShow ***)
(*** condition:html, include-it-raw:averageGoalsPerMatchByAgeFWChartHTML ***)
(*** condition:html, include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.




