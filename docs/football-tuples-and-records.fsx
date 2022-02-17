(**
---
title: Tuples and Records (with solutions)
category: Assignments
categoryindex: 2
index: 1
---
*)

(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

# Working with Tuples and Records.

> Developed with [Davide Costa](https://github.com/DavideGCosta)

This set of exercises covers creating and manipulating tuples, records, and anonymous records.
Before you start it is a good idea to review the relevant sections of
the F# language reference (
[tuples](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/tuples),
[records](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/records),
and [anonymous records](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/anonymous-records)
) and
F# for fun and profit (
[tuples](https://fsharpforfunandprofit.com/posts/tuples/) and
[records](https://fsharpforfunandprofit.com/posts/records/))
before you start.
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
## Import the Football Players Data from the Csv File
*)

#r "nuget:FSharp.Data"
open FSharp.Data

(**
In order to import the data correctly we need to create the sample, define the type from the sample and then load the csv file.
We'll use [FSharp.Data CsvProvider](https://fsprojects.github.io/FSharp.Data/library/CsvProvider.html).
 
### Load the Csv file.  

We define the type from the csv sample file.
*)

let [<Literal>] CsvPath = __SOURCE_DIRECTORY__ + "/FootballPlayers.csv"
type FootballPlayersCsv = CsvProvider<CsvPath>

(**
This will load the sample csv file.
*)
let playerStatsTable = 
    FootballPlayersCsv.GetSample().Rows
    |> Seq.toList

(**
Let's see the first 5 rows from the loaded Csv data, stored in `playerStatsTable`.
Again, we do this by using the List `List.truncate` property.
*)
playerStatsTable
|> List.truncate 5
(*** include-fsi-output ***)

(**
## EXERCISES - PART 1

- [Transforming collection elements into new types.](#Transforming-collections)

    1. [Creating tuples.](#Creating-tuples)
    2. [Creating records.](#Creating-records)
    3. [Creating anonymous records.](#Creating-anonymous-records)

- [Simple data transformations.](#Simple-transformations)
    1. [Transformations using tuples.](#Transformations-using-tuples)
    2. [Transformations using records.](#Transformations-using-records)
    3. [Transformations using anonymous records.](#Transformations-using-anonymous-records)

- [Creating and transforming TeamRecord.](#Creating-and-transforming-TeamRecord)
*)

(***hide***)
H2 "Transforming collections"
(*** include-it-raw ***)

(***hide***)
H3 "Creating tuples"
(*** include-it-raw ***)


(**
Example: Transform each element of the `playerStatsTable` List into a tuple with the player and nation ( `Player`, `Nation`)
*)

playerStatsTable
|> List.map(fun x -> x.Player, x.Nation)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
- Transform each element of the `playerStatsTable` List into a tuple with the player and team ( `Player`, `Team`)
*)
(*** include-it-raw:preDetails ***)
(*** define: PlayerAndTeamTuple, define-output: PlayerAndTeamTuple ***)

playerStatsTable
|> List.map(fun x -> x.Player, x.Team)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndTeamTuple ***)
(*** condition:html, include-fsi-output:PlayerAndTeamTuple ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.

(**
- Transform each element of the `playerStatsTable` List into a tuple with the player and league/competiton ( `Player`, `League`)
*)
(*** include-it-raw:preDetails ***)
(*** define: PlayerAndLeagueTuple, define-output: PlayerAndLeagueTuple ***)

playerStatsTable
|> List.map(fun x -> x.Player, x.League)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndLeagueTuple ***)
(*** condition:html, include-fsi-output:PlayerAndLeagueTuple ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Transform each element of the `playerStatsTable` List into a tuple with the player and age ( `Player`, `Age`)
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndAgeTuple, define-output: PlayerAndAgeTuple ***)

playerStatsTable
|> List.map(fun x -> x.Player, x.Age)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndAgeTuple ***)
(*** condition:html, include-fsi-output:PlayerAndAgeTuple ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Transform each element of the `playerStatsTable` List into a tuple with the player and matches played ( `Player`, `MatchesPlayed`)
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndMatchesTuple, define-output: PlayerAndMatchesTuple ***)

playerStatsTable
|> List.map(fun x -> x.Player, x.MatchesPlayed)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndMatchesTuple ***)
(*** condition:html, include-fsi-output:PlayerAndMatchesTuple ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Transform each element of the `playerStatsTable` List into a tuple with the player and goals scored ( `Player`, `GoalsScored`)
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndGoalsTuple, define-output: PlayerAndGoalsTuple ***)

playerStatsTable
|> List.map(fun x -> x.Player, x.GoalsScored)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndGoalsTuple ***)
(*** condition:html, include-fsi-output:PlayerAndGoalsTuple ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.

(**

*)

(*** hide ***)
H3 "Creating records"
(*** include-it-raw ***)


(**
Example: Define a record named `PlayerAndNation` with a field named `Player` that is a `string` and `Nation` that is a `string`. 
Then transform each element of the `playerStatsTable` List into a `PlayerAndNation` record.
*)

type PlayerAndNation =
    { Player : string 
      Nation : string }

(**
The above code creates a record type called `PlayerAndNation`.
This record contains two fields: `Player` of `string` type and `Nation` of `string` type.
Remember, if the types from the csv file are different an error will occur when creating an instance of the record.
 
Common types: 

- `string`, example: `"hello world"`
- `int`, example: `2`
- `float`, example: `2.0`
- `decimal`, example: `2.0m`

Check [basic types documentation](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/basic-types)
to learn about F# basic types.

*)

(**
Now by having the record type created we can `map` the `playerStatsTable` rows to the record `PlayerAndNation`.
*)
playerStatsTable
|> List.map(fun x -> 
    { Player = x.Player
      Nation = x.Nation })
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
Note that you choose the name of the fields in the record. Instead of `Player` it could be anything.
The following code block for example would have also worked, 
but the field name for the player is `PlayerName` instead of `Player` and `Nationality` instead of `Nation`:
*)
type PlayerAndNation2 =
    { PlayerName : string 
      Nationality : string }

playerStatsTable
|> List.map(fun x -> 
    { PlayerName = x.Player
      Nationality = x.Nation })

(**
- Define a record named `PlayerAndTeam` with a field named `Player` that is a `string` and `Team` that is a `string`. 
Then transform each element of the `playerStatsTable` List into a `PlayerAndTeam` record.
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndTeamRecord, define-output: PlayerAndTeamRecord ***)

type PlayerAndTeam =
    { Player : string
      Team : string }
   
playerStatsTable
|> List.map(fun x -> 
    { Player = x.Player 
      Team = x.Team })
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndTeamRecord ***)
(*** condition:html, include-fsi-output:PlayerAndTeamRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Define a record named `PlayerAndLeague` with a field named `Player` that is a `string` and `League` that is a `string`. 
Then transform each element of the `playerStatsTable` List into a `PlayerAndLeague` record.
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndLeagueRecord, define-output: PlayerAndLeagueRecord ***)

type PlayerAndLeague =
    { Player : string
      League : string }

playerStatsTable
|> List.map(fun x -> 
    { Player = x.Player
      League = x.League })
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndLeagueRecord ***)
(*** condition:html, include-fsi-output:PlayerAndLeagueRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Define a record named `PlayerAndAge` with a field named `Player` that is a `string` and `Age` that is a integer(`int`). 
Then transform each element of the `playerStatsTable` List into a `PlayerAndAge` record.
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndAgeRecord, define-output: PlayerAndAgeRecord ***)

type PlayerAndAge =
    { Player : string
      Age : int }

playerStatsTable
|> List.map(fun x -> 
    { Player = x.Player
      Age =  x.Age })
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndAgeRecord ***)
(*** condition:html, include-fsi-output:PlayerAndAgeRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Define a record named `PlayerAndMatchesPlayed` with a field named `Player` that is a `string` and `MatchesPlayed` that is a integer(`int`). 
Then transform each element of the `playerStatsTable` List into a `PlayerAndMatchesPlayed` record.
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndMatchesRecord, define-output: PlayerAndMatchesRecord ***)

type PlayerAndMatchesPlayed =
    { Player : string
      MatchesPlayed : int }

playerStatsTable
|> List.map(fun x -> 
    { Player = x.Player 
      MatchesPlayed = x.MatchesPlayed})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndMatchesRecord ***)
(*** condition:html, include-fsi-output:PlayerAndMatchesRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Define a record named `PlayerAndGoalsScored` with a field named `Player` that is a `string` and `GoalsScored` that is a integer(`int`). 
Then transform each element of the `playerStatsTable` List into a `PlayerAndGoalsScored` record.
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndGoalsRecord, define-output: PlayerAndGoalsRecord ***)

type PlayerAndGoalsScored = 
    { Player : string
      GoalsScored : int }

playerStatsTable
|> List.map(fun x -> 
    { Player = x.Player 
      GoalsScored = x.GoalsScored })
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndGoalsRecord ***)
(*** condition:html, include-fsi-output:PlayerAndGoalsRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(*** hide ***)
H3 "Creating anonymous records"
(*** include-it-raw ***)

(**
Example: Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `Nation` field that is a `string`.

With `Anonymous records` we don't need to define the record type beforehand and we don't need to specify the type of each field.
*)

playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player
       Nation = x.Nation |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
- Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `Team` field that is a `string`.
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndTeam-AnonymousRecord, define-output: PlayerAndTeam-AnonymousRecord ***)

playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player 
       Team = x.Team |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndTeam-AnonymousRecord ***)
(*** condition:html, include-fsi-output:PlayerAndTeam-AnonymousRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `League` field that is a `string`.
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndLeague-AnonymousRecord, define-output: PlayerAndLeague-AnonymousRecord ***)

playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player
       League = x.League |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndLeague-AnonymousRecord ***)
(*** condition:html, include-fsi-output:PlayerAndLeague-AnonymousRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `Age` field that is a integer(`int`).
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndAge-AnonymousRecord, define-output: PlayerAndAge-AnonymousRecord ***)

playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player
       Age =  x.Age |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndAge-AnonymousRecord ***)
(*** condition:html, include-fsi-output:PlayerAndAge-AnonymousRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `MatchesPlayed` field that is a integer(`int`).
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndMatches-AnonymousRecord, define-output: PlayerAndMatches-AnonymousRecord ***)

playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player 
       MatchesPlayed = x.MatchesPlayed |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndMatches-AnonymousRecord ***)
(*** condition:html, include-fsi-output:PlayerAndMatches-AnonymousRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `GoalsScored` field that is a integer(`int`).
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndGoals-AnonymousRecord, define-output: PlayerAndGoals-AnonymousRecord ***)

playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player 
       GoalsScored = x.GoalsScored |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndGoals-AnonymousRecord ***)
(*** condition:html, include-fsi-output:PlayerAndGoals-AnonymousRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(*** hide ***)
H2 "Simple transformations"
(*** include-it-raw ***)

(**
Now that you are used to work with `List.map` to organize the data into tuples, records and anonymous records.
Let's try to do it while applying some simple transformations as sum, multiplication, type transformations and so on.
*)

(*** hide ***)
H3 "Transformations using tuples"
(*** include-it-raw ***)

(**
Example: map the `playerStatsTable` to a tuple of player and age, but add 1 to age. ( `Player`, `Age + 1`)
*)

playerStatsTable
|> List.map(fun x -> x.Age + 1)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**

When to use integers or floats/decimals:

  1. Use integers if the results of the calculations should be integers (1, 2, 3, 4, ...).
  2. Use floats or decimals if the results of the calculations may be floats or decimals (1.1, 2.1324, ...).

*)

(**
- map the `playerStatsTable` to a tuple of player and goals scored, but multiply goals scored by 10. ( `Player`, `GoalsScored * 10`)
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndGoals-Times10-Tuple, define-output: PlayerAndGoals-Times10-Tuple ***)

playerStatsTable
|> List.map(fun  x-> x.Player, x.GoalsScored * 10)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndGoals-Times10-Tuple ***)
(*** condition:html, include-fsi-output:PlayerAndGoals-Times10-Tuple ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- map the `playerStatsTable` to a tuple of player and goals scored, but divide GoalsScored by 2. ( `Player`, `GoalsScored / 2`)
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndGoals-DividedBy2-Tuple, define-output: PlayerAndGoals-DividedBy2-Tuple ***)

playerStatsTable
|> List.map(fun x -> x.Player,  x.GoalsScored / 2)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndGoals-DividedBy2-Tuple ***)
(*** condition:html, include-fsi-output:PlayerAndGoals-DividedBy2-Tuple ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
In this case, look how dividing using integers rounds the results to the nearest integers.
If the results are decimals you might prefer to get exact results.
For that you can use floats or decimals types.
In order to convert a variable to float you have to use the syntax: `float variable`.
*)

(**
Example: map the `playerStatsTable` to a tuple of player and age, but convert age to float. ( `Player`, `float Age`)
*)
playerStatsTable
|> List.map(fun x -> x.Player, float x.Age) 
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
- map the `playerStatsTable` to a tuple of player and goals scored, but convert goalsScored to float. ( `Player`, `float GoalsScored`)
*)

(*** include-it-raw:preDetails ***)
(*** define: playerAndGoals-Float-Tuple, define-output: playerAndGoals-Float-Tuple ***)

playerStatsTable
|> List.map(fun x -> x.Player, float x.GoalsScored) 
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:playerAndGoals-Float-Tuple ***)
(*** condition:html, include-fsi-output:playerAndGoals-Float-Tuple ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- map the `playerStatsTable` to a tuple of player and goals scored, but divide goalsScored by 2.0. ( `Player`, `GoalsScored / 2.0`)
Hint: convert goals scored to float and divide by 2.0 (you can't divide by 2 because if you perform math operations with different types, you'll get an error).
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndGoals-DividedBy2-Tuple, define-output: PlayerAndGoals-DividedBy2-Tuple ***)

playerStatsTable
|> List.map(fun x -> x.Player, float x.GoalsScored / 2.0)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndGoals-DividedBy2-Tuple ***)
(*** condition:html, include-fsi-output:PlayerAndGoals-DividedBy2-Tuple ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(*** hide ***)
H3 "Transformations using records"
(*** include-it-raw ***)

(**
Example: map the `playerStatsTable` to a record of player and age, but add 1 to age. ( `Player`, `Age + 1`)
*)
type PlayerAndAgePlus1Int =
    { Player : string
      AgePlus1Int : int }

playerStatsTable
|> List.map(fun x ->
    { Player = x.Player 
      AgePlus1Int = x.Age + 1})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

(**
- map the `playerStatsTable` to a record of player and goals scored, but multiply goals scored by 10. ( `Player`, `GoalsScored * 10`)
Hint: You have to create a new type record.
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndGoals-Times10-Record, define-output: PlayerAndGoals-Times10-Record ***)

type PlayerAndGls = 
    { Player : string 
      GoalsScored : int}

playerStatsTable
|> List.map(fun x -> 
    { Player = x.Player 
      GoalsScored = x.GoalsScored * 10 })
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndGoals-Times10-Record ***)
(*** condition:html, include-fsi-output:PlayerAndGoals-Times10-Record ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- map the `playerStatsTable` to a record of player and goals scored, but divide goals scored by 2.0. ( `Player`, `float GoalsScored  / 2.0`)  
Hint: You have to create a new type record, because previous type has goals scored as integers but you want floats.  
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndGoals-DividedBy2-Record, define-output: PlayerAndGoals-DividedBy2-Record ***)

type PlayerAndGlsFloat = 
    { Player : string 
      GoalsScoredFloat : float}

playerStatsTable
|> List.map(fun x -> 
    { Player = x.Player 
      GoalsScoredFloat = (float x.GoalsScored) / 2.0 })
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndGoals-DividedBy2-Record ***)
(*** condition:html, include-fsi-output:PlayerAndGoals-DividedBy2-Record ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(*** hide ***)
H3 "Transformations using anonymous records"
(*** include-it-raw ***)

(**
Example: map the `playerStatsTable` to an anonymoys record of player and age, but add 1 to age. ( `Player`, `Age + 1`)
*)

playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player
       AgePlus1 = x.Age + 1 |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)

// or 

playerStatsTable
|> List.map(fun x ->
    {| Player = x.Player 
       AgePlus1Float = (float x.Age) + 1.0 |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.
(*** include-fsi-output ***)  

(**
- map the `playerStatsTable` to an anonymous record of player and goals scored, but multiply goals scored by 10. ( `Player`, `GoalsScored * 10`)
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndGoals-Times10-AnonymousRecord, define-output: PlayerAndGoals-Times10-AnonymousRecord ***)

playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player 
       GoalsScoredFloat = x.GoalsScored * 10 |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

// or 

playerStatsTable
|> List.map(fun x ->
    {| Player = x.Player 
       AgePlus1Float = (float x.GoalsScored) * 10.0 |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndGoals-Times10-AnonymousRecord ***)
(*** condition:html, include-fsi-output:PlayerAndGoals-Times10-AnonymousRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
- map the `playerStatsTable` to an anonymous record of player and goals scored, but divide goals scored by 2.0. ( `Player`, `float GoalsScored  / 2.0`)  
Hint: Remember that you have to transform GoalsScored to float.
*)

(*** include-it-raw:preDetails ***)
(*** define: PlayerAndGoals-DividedBy2-AnonymousRecord, define-output: PlayerAndGoals-DividedBy2-AnonymousRecord ***)

playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player 
       GoalsScoredFloat = (float x.GoalsScored) / 2.0 |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:PlayerAndGoals-DividedBy2-AnonymousRecord ***)
(*** condition:html, include-fsi-output:PlayerAndGoals-DividedBy2-AnonymousRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(*** hide ***)
H2 "Creating and transforming TeamRecord"
(*** include-it-raw ***)

(**

Now that you are used to work with records and perform simple Transformations, map `playerStatsTable` to a record type that includes:  

- Player (`Player`) - type `string`  
- Nation (`Nation`) - type `string`  
- League (`League`) - type `string`  
- AgeNextYear (`Age + 1`) - type `int`  
- HalfGoalsScored (`GoalsScored / 2.0`) - type `float`  
  
Hint: Create a new type.
*)

(*** include-it-raw:preDetails ***)
(*** define: TeamRecord, define-output: TeamRecord ***)

type TeamRecord = 
    { Player : string
      Nation : string
      League : string
      AgeNextYear : int
      HalfGoalsScored : float }

playerStatsTable
|> List.map(fun x ->
    { Player = x.Player
      Nation = x.Nation
      League = x.League
      AgeNextYear = x.Age + 1
      HalfGoalsScored = float x.GoalsScored / 2.0})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.

(*** condition:html, include:TeamRecord ***)
(*** condition:html, include-fsi-output:TeamRecord ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.





