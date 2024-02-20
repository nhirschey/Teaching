(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=football-tuples-and-records.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//football-tuples-and-records.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//football-tuples-and-records.ipynb)

# Working with Tuples and Records.

> Developed with [Davide Costa](https://github.com/DavideGCosta)
> 

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

## Import the Football Players Data from the Csv File

*)
#r "nuget: FSharp.Data, 5.0.2"
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
|> List.truncate 5(* output: 
val it: CsvProvider<...>.Row list =
  [("Robert Lewandowski", "pl POL", "FW", "Bayern Munich", "deBundesliga", 32,
    34, 35);
   ("Kylian Mbappé", "fr FRA", "FW", "Paris S-G", "frLigue 1", 22, 35, 28);
   ("Karim Benzema", "fr FRA", "FW", "Real Madrid", "esLa Liga", 33, 32, 27);
   ("Ciro Immobile", "it ITA", "FW", "Lazio", "itSerie A", 31, 31, 27);
   ("Wissam Ben Yedder", "fr FRA", "FW", "Monaco", "frLigue 1", 30, 37, 25)]*)
(**
## EXERCISES - PART 1

* [Transforming collection elements into new types.](#Transforming-collections)
  

  0 [Creating tuples.](#Creating-tuples)
    
  
  1 [Creating records.](#Creating-records)
    
  
  2 [Creating anonymous records.](#Creating-anonymous-records)
    
  

* [Simple data transformations.](#Simple-transformations)
  

  0 [Transformations using tuples.](#Transformations-using-tuples)
    
  
  1 [Transformations using records.](#Transformations-using-records)
    
  
  2 [Transformations using anonymous records.](#Transformations-using-anonymous-records)
    
  

* [Creating and transforming TeamRecord.](#Creating-and-transforming-TeamRecord)
  

<h2 class=numbered><a name=Transforming-collections class=anchor href=#Transforming-collections>Transforming collections</a></h2>

<h3 class=numbered><a name=Creating-tuples class=anchor href=#Creating-tuples>Creating tuples</a></h3>

Example: Transform each element of the `playerStatsTable` List into a tuple with the player and nation ( `Player`, `Nation`)

*)
playerStatsTable
|> List.map(fun x -> x.Player, x.Nation)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: (string * string) list =
  [("Robert Lewandowski", "pl POL"); ("Kylian Mbappé", "fr FRA");
   ("Karim Benzema", "fr FRA"); ("Ciro Immobile", "it ITA");
   ("Wissam Ben Yedder", "fr FRA")]*)
(**
* Transform each element of the `playerStatsTable` List into a tuple with the player and team ( `Player`, `Team`)

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (string * string) list =
  [("Robert Lewandowski", "Bayern Munich"); ("Kylian Mbappé", "Paris S-G");
   ("Karim Benzema", "Real Madrid"); ("Ciro Immobile", "Lazio");
   ("Wissam Ben Yedder", "Monaco")]
```

</details>
</span>
</p>
</div>

* Transform each element of the `playerStatsTable` List into a tuple with the player and league/competiton ( `Player`, `League`)

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (string * string) list =
  [("Robert Lewandowski", "deBundesliga"); ("Kylian Mbappé", "frLigue 1");
   ("Karim Benzema", "esLa Liga"); ("Ciro Immobile", "itSerie A");
   ("Wissam Ben Yedder", "frLigue 1")]
```

</details>
</span>
</p>
</div>

* Transform each element of the `playerStatsTable` List into a tuple with the player and age ( `Player`, `Age`)

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (string * int) list =
  [("Robert Lewandowski", 32); ("Kylian Mbappé", 22); ("Karim Benzema", 33);
   ("Ciro Immobile", 31); ("Wissam Ben Yedder", 30)]
```

</details>
</span>
</p>
</div>

* Transform each element of the `playerStatsTable` List into a tuple with the player and matches played ( `Player`, `MatchesPlayed`)

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (string * int) list =
  [("Robert Lewandowski", 34); ("Kylian Mbappé", 35); ("Karim Benzema", 32);
   ("Ciro Immobile", 31); ("Wissam Ben Yedder", 37)]
```

</details>
</span>
</p>
</div>

* Transform each element of the `playerStatsTable` List into a tuple with the player and goals scored ( `Player`, `GoalsScored`)

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (string * int) list =
  [("Robert Lewandowski", 35); ("Kylian Mbappé", 28); ("Karim Benzema", 27);
   ("Ciro Immobile", 27); ("Wissam Ben Yedder", 25)]
```

</details>
</span>
</p>
</div>

<h3 class=numbered><a name=Creating-records class=anchor href=#Creating-records>Creating records</a></h3>

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

* `string`, example: `"hello world"`

* `int`, example: `2`

* `float`, example: `2.0`

* `decimal`, example: `2.0m`

Check [basic types documentation](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/basic-types)
to learn about F# basic types.

Now by having the record type created we can `map` the `playerStatsTable` rows to the record `PlayerAndNation`.

*)
playerStatsTable
|> List.map(fun x -> 
    { Player = x.Player
      Nation = x.Nation })
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: PlayerAndNation list =
  [{ Player = "Robert Lewandowski"
     Nation = "pl POL" }; { Player = "Kylian Mbappé"
                            Nation = "fr FRA" }; { Player = "Karim Benzema"
                                                   Nation = "fr FRA" };
   { Player = "Ciro Immobile"
     Nation = "it ITA" }; { Player = "Wissam Ben Yedder"
                            Nation = "fr FRA" }]*)
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
* Define a record named `PlayerAndTeam` with a field named `Player` that is a `string` and `Team` that is a `string`. 
Then transform each element of the `playerStatsTable` List into a `PlayerAndTeam` record.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type PlayerAndTeam =
  {
    Player: string
    Team: string
  }
val it: PlayerAndTeam list =
  [{ Player = "Robert Lewandowski"
     Team = "Bayern Munich" }; { Player = "Kylian Mbappé"
                                 Team = "Paris S-G" };
   { Player = "Karim Benzema"
     Team = "Real Madrid" }; { Player = "Ciro Immobile"
                               Team = "Lazio" };
   { Player = "Wissam Ben Yedder"
     Team = "Monaco" }]
```

</details>
</span>
</p>
</div>

* Define a record named `PlayerAndLeague` with a field named `Player` that is a `string` and `League` that is a `string`. 
Then transform each element of the `playerStatsTable` List into a `PlayerAndLeague` record.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type PlayerAndLeague =
  {
    Player: string
    League: string
  }
val it: PlayerAndLeague list =
  [{ Player = "Robert Lewandowski"
     League = "deBundesliga" }; { Player = "Kylian Mbappé"
                                  League = "frLigue 1" };
   { Player = "Karim Benzema"
     League = "esLa Liga" }; { Player = "Ciro Immobile"
                               League = "itSerie A" };
   { Player = "Wissam Ben Yedder"
     League = "frLigue 1" }]
```

</details>
</span>
</p>
</div>

* Define a record named `PlayerAndAge` with a field named `Player` that is a `string` and `Age` that is a integer(`int`). 
Then transform each element of the `playerStatsTable` List into a `PlayerAndAge` record.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type PlayerAndAge =
  {
    Player: string
    Age: int
  }
val it: PlayerAndAge list =
  [{ Player = "Robert Lewandowski"
     Age = 32 }; { Player = "Kylian Mbappé"
                   Age = 22 }; { Player = "Karim Benzema"
                                 Age = 33 }; { Player = "Ciro Immobile"
                                               Age = 31 };
   { Player = "Wissam Ben Yedder"
     Age = 30 }]
```

</details>
</span>
</p>
</div>

* Define a record named `PlayerAndMatchesPlayed` with a field named `Player` that is a `string` and `MatchesPlayed` that is a integer(`int`). 
Then transform each element of the `playerStatsTable` List into a `PlayerAndMatchesPlayed` record.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type PlayerAndMatchesPlayed =
  {
    Player: string
    MatchesPlayed: int
  }
val it: PlayerAndMatchesPlayed list =
  [{ Player = "Robert Lewandowski"
     MatchesPlayed = 34 }; { Player = "Kylian Mbappé"
                             MatchesPlayed = 35 }; { Player = "Karim Benzema"
                                                     MatchesPlayed = 32 };
   { Player = "Ciro Immobile"
     MatchesPlayed = 31 }; { Player = "Wissam Ben Yedder"
                             MatchesPlayed = 37 }]
```

</details>
</span>
</p>
</div>

* Define a record named `PlayerAndGoalsScored` with a field named `Player` that is a `string` and `GoalsScored` that is a integer(`int`). 
Then transform each element of the `playerStatsTable` List into a `PlayerAndGoalsScored` record.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type PlayerAndGoalsScored =
  {
    Player: string
    GoalsScored: int
  }
val it: PlayerAndGoalsScored list =
  [{ Player = "Robert Lewandowski"
     GoalsScored = 35 }; { Player = "Kylian Mbappé"
                           GoalsScored = 28 }; { Player = "Karim Benzema"
                                                 GoalsScored = 27 };
   { Player = "Ciro Immobile"
     GoalsScored = 27 }; { Player = "Wissam Ben Yedder"
                           GoalsScored = 25 }]
```

</details>
</span>
</p>
</div>

<h3 class=numbered><a name=Creating-anonymous-records class=anchor href=#Creating-anonymous-records>Creating anonymous records</a></h3>

Example: Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `Nation` field that is a `string`.

With `Anonymous records` we don't need to define the record type beforehand and we don't need to specify the type of each field.

*)
playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player
       Nation = x.Nation |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: {| Nation: string; Player: string |} list =
  [{ Nation = "pl POL"
     Player = "Robert Lewandowski" }; { Nation = "fr FRA"
                                        Player = "Kylian Mbappé" };
   { Nation = "fr FRA"
     Player = "Karim Benzema" }; { Nation = "it ITA"
                                   Player = "Ciro Immobile" };
   { Nation = "fr FRA"
     Player = "Wissam Ben Yedder" }]*)
(**
* Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `Team` field that is a `string`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: {| Player: string; Team: string |} list =
  [{ Player = "Robert Lewandowski"
     Team = "Bayern Munich" }; { Player = "Kylian Mbappé"
                                 Team = "Paris S-G" };
   { Player = "Karim Benzema"
     Team = "Real Madrid" }; { Player = "Ciro Immobile"
                               Team = "Lazio" };
   { Player = "Wissam Ben Yedder"
     Team = "Monaco" }]
```

</details>
</span>
</p>
</div>

* Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `League` field that is a `string`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: {| League: string; Player: string |} list =
  [{ League = "deBundesliga"
     Player = "Robert Lewandowski" }; { League = "frLigue 1"
                                        Player = "Kylian Mbappé" };
   { League = "esLa Liga"
     Player = "Karim Benzema" }; { League = "itSerie A"
                                   Player = "Ciro Immobile" };
   { League = "frLigue 1"
     Player = "Wissam Ben Yedder" }]
```

</details>
</span>
</p>
</div>

* Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `Age` field that is a integer(`int`).

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: {| Age: int; Player: string |} list =
  [{ Age = 32
     Player = "Robert Lewandowski" }; { Age = 22
                                        Player = "Kylian Mbappé" };
   { Age = 33
     Player = "Karim Benzema" }; { Age = 31
                                   Player = "Ciro Immobile" };
   { Age = 30
     Player = "Wissam Ben Yedder" }]
```

</details>
</span>
</p>
</div>

* Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `MatchesPlayed` field that is a integer(`int`).

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: {| MatchesPlayed: int; Player: string |} list =
  [{ MatchesPlayed = 34
     Player = "Robert Lewandowski" }; { MatchesPlayed = 35
                                        Player = "Kylian Mbappé" };
   { MatchesPlayed = 32
     Player = "Karim Benzema" }; { MatchesPlayed = 31
                                   Player = "Ciro Immobile" };
   { MatchesPlayed = 37
     Player = "Wissam Ben Yedder" }]
```

</details>
</span>
</p>
</div>

* Transform each element of the `playerStatsTable` List into an anonymous record with a `Player` field that is a `string` and a `GoalsScored` field that is a integer(`int`).

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: {| GoalsScored: int; Player: string |} list =
  [{ GoalsScored = 35
     Player = "Robert Lewandowski" }; { GoalsScored = 28
                                        Player = "Kylian Mbappé" };
   { GoalsScored = 27
     Player = "Karim Benzema" }; { GoalsScored = 27
                                   Player = "Ciro Immobile" };
   { GoalsScored = 25
     Player = "Wissam Ben Yedder" }]
```

</details>
</span>
</p>
</div>

<h2 class=numbered><a name=Simple-transformations class=anchor href=#Simple-transformations>Simple transformations</a></h2>

Now that you are used to work with `List.map` to organize the data into tuples, records and anonymous records.
Let's try to do it while applying some simple transformations as sum, multiplication, type transformations and so on.

<h3 class=numbered><a name=Transformations-using-tuples class=anchor href=#Transformations-using-tuples>Transformations using tuples</a></h3>

Example: map the `playerStatsTable` to a tuple of player and age, but add 1 to age. ( `Player`, `Age + 1`)

*)
playerStatsTable
|> List.map(fun x -> x.Age + 1)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: int list = [33; 23; 34; 32; 31]*)
(**
When to use integers or floats/decimals:

0 Use integers if the results of the calculations should be integers (1, 2, 3, 4, ...).

1 Use floats or decimals if the results of the calculations may be floats or decimals (1.1, 2.1324, ...).

* map the `playerStatsTable` to a tuple of player and goals scored, but multiply goals scored by 10. ( `Player`, `GoalsScored * 10`)

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (string * int) list =
  [("Robert Lewandowski", 350); ("Kylian Mbappé", 280); ("Karim Benzema", 270);
   ("Ciro Immobile", 270); ("Wissam Ben Yedder", 250)]
```

</details>
</span>
</p>
</div>

* map the `playerStatsTable` to a tuple of player and goals scored, but divide GoalsScored by 2. ( `Player`, `GoalsScored / 2`)

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (string * int) list =
  [("Robert Lewandowski", 17); ("Kylian Mbappé", 14); ("Karim Benzema", 13);
   ("Ciro Immobile", 13); ("Wissam Ben Yedder", 12)]
```

</details>
</span>
</p>
</div>

In this case, look how dividing using integers rounds the results to the nearest integers.
If the results are decimals you might prefer to get exact results.
For that you can use floats or decimals types.
In order to convert a variable to float you have to use the syntax: `float variable`.

Example: map the `playerStatsTable` to a tuple of player and age, but convert age to float. ( `Player`, `float Age`)

*)
playerStatsTable
|> List.map(fun x -> x.Player, float x.Age) 
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: (string * float) list =
  [("Robert Lewandowski", 32.0); ("Kylian Mbappé", 22.0);
   ("Karim Benzema", 33.0); ("Ciro Immobile", 31.0);
   ("Wissam Ben Yedder", 30.0)]*)
(**
* map the `playerStatsTable` to a tuple of player and goals scored, but convert goalsScored to float. ( `Player`, `float GoalsScored`)

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (string * float) list =
  [("Robert Lewandowski", 35.0); ("Kylian Mbappé", 28.0);
   ("Karim Benzema", 27.0); ("Ciro Immobile", 27.0);
   ("Wissam Ben Yedder", 25.0)]
```

</details>
</span>
</p>
</div>

* map the `playerStatsTable` to a tuple of player and goals scored, but divide goalsScored by 2.0. ( `Player`, `GoalsScored / 2.0`)
Hint: convert goals scored to float and divide by 2.0 (you can't divide by 2 because if you perform math operations with different types, you'll get an error).

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (string * int) list =
  [("Robert Lewandowski", 17); ("Kylian Mbappé", 14); ("Karim Benzema", 13);
   ("Ciro Immobile", 13); ("Wissam Ben Yedder", 12)]
```

</details>
</span>
</p>
</div>

<h3 class=numbered><a name=Transformations-using-records class=anchor href=#Transformations-using-records>Transformations using records</a></h3>

Example: map the `playerStatsTable` to a record of player and age, but add 1 to age. ( `Player`, `Age + 1`)

*)
type PlayerAndAgePlus1Int =
    { Player : string
      AgePlus1Int : int }

playerStatsTable
|> List.map(fun x ->
    { Player = x.Player 
      AgePlus1Int = x.Age + 1})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
type PlayerAndAgePlus1Int =
  {
    Player: string
    AgePlus1Int: int
  }
val it: PlayerAndAgePlus1Int list =
  [{ Player = "Robert Lewandowski"
     AgePlus1Int = 33 }; { Player = "Kylian Mbappé"
                           AgePlus1Int = 23 }; { Player = "Karim Benzema"
                                                 AgePlus1Int = 34 };
   { Player = "Ciro Immobile"
     AgePlus1Int = 32 }; { Player = "Wissam Ben Yedder"
                           AgePlus1Int = 31 }]*)
(**
* map the `playerStatsTable` to a record of player and goals scored, but multiply goals scored by 10. ( `Player`, `GoalsScored * 10`)
Hint: You have to create a new type record.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type PlayerAndGls =
  {
    Player: string
    GoalsScored: int
  }
val it: PlayerAndGls list =
  [{ Player = "Robert Lewandowski"
     GoalsScored = 350 }; { Player = "Kylian Mbappé"
                            GoalsScored = 280 }; { Player = "Karim Benzema"
                                                   GoalsScored = 270 };
   { Player = "Ciro Immobile"
     GoalsScored = 270 }; { Player = "Wissam Ben Yedder"
                            GoalsScored = 250 }]
```

</details>
</span>
</p>
</div>

* map the `playerStatsTable` to a record of player and goals scored, but divide goals scored by 2.0. ( `Player`, `float GoalsScored  / 2.0`)
Hint: You have to create a new type record, because previous type has goals scored as integers but you want floats.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type PlayerAndGlsFloat =
  {
    Player: string
    GoalsScoredFloat: float
  }
val it: PlayerAndGlsFloat list =
  [{ Player = "Robert Lewandowski"
     GoalsScoredFloat = 17.5 }; { Player = "Kylian Mbappé"
                                  GoalsScoredFloat = 14.0 };
   { Player = "Karim Benzema"
     GoalsScoredFloat = 13.5 }; { Player = "Ciro Immobile"
                                  GoalsScoredFloat = 13.5 };
   { Player = "Wissam Ben Yedder"
     GoalsScoredFloat = 12.5 }]
```

</details>
</span>
</p>
</div>

<h3 class=numbered><a name=Transformations-using-anonymous-records class=anchor href=#Transformations-using-anonymous-records>Transformations using anonymous records</a></h3>

Example: map the `playerStatsTable` to an anonymoys record of player and age, but add 1 to age. ( `Player`, `Age + 1`)

*)
playerStatsTable
|> List.map(fun x -> 
    {| Player = x.Player
       AgePlus1 = x.Age + 1 |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: {| AgePlus1: int; Player: string |} list =
  [{ AgePlus1 = 33
     Player = "Robert Lewandowski" }; { AgePlus1 = 23
                                        Player = "Kylian Mbappé" };
   { AgePlus1 = 34
     Player = "Karim Benzema" }; { AgePlus1 = 32
                                   Player = "Ciro Immobile" };
   { AgePlus1 = 31
     Player = "Wissam Ben Yedder" }]*)
// or 

playerStatsTable
|> List.map(fun x ->
    {| Player = x.Player 
       AgePlus1Float = (float x.Age) + 1.0 |})
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: {| AgePlus1Float: float; Player: string |} list =
  [{ AgePlus1Float = 33.0
     Player = "Robert Lewandowski" }; { AgePlus1Float = 23.0
                                        Player = "Kylian Mbappé" };
   { AgePlus1Float = 34.0
     Player = "Karim Benzema" }; { AgePlus1Float = 32.0
                                   Player = "Ciro Immobile" };
   { AgePlus1Float = 31.0
     Player = "Wissam Ben Yedder" }]*)
(**
* map the `playerStatsTable` to an anonymous record of player and goals scored, but multiply goals scored by 10. ( `Player`, `GoalsScored * 10`)

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: {| AgePlus1Float: float; Player: string |} list =
  [{ AgePlus1Float = 350.0
     Player = "Robert Lewandowski" }; { AgePlus1Float = 280.0
                                        Player = "Kylian Mbappé" };
   { AgePlus1Float = 270.0
     Player = "Karim Benzema" }; { AgePlus1Float = 270.0
                                   Player = "Ciro Immobile" };
   { AgePlus1Float = 250.0
     Player = "Wissam Ben Yedder" }]
```

</details>
</span>
</p>
</div>

* map the `playerStatsTable` to an anonymous record of player and goals scored, but divide goals scored by 2.0. ( `Player`, `float GoalsScored  / 2.0`)
Hint: Remember that you have to transform GoalsScored to float.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: {| GoalsScoredFloat: float; Player: string |} list =
  [{ GoalsScoredFloat = 17.5
     Player = "Robert Lewandowski" }; { GoalsScoredFloat = 14.0
                                        Player = "Kylian Mbappé" };
   { GoalsScoredFloat = 13.5
     Player = "Karim Benzema" }; { GoalsScoredFloat = 13.5
                                   Player = "Ciro Immobile" };
   { GoalsScoredFloat = 12.5
     Player = "Wissam Ben Yedder" }]
```

</details>
</span>
</p>
</div>

<h2 class=numbered><a name=Creating-and-transforming-TeamRecord class=anchor href=#Creating-and-transforming-TeamRecord>Creating and transforming TeamRecord</a></h2>

Now that you are used to work with records and perform simple Transformations, map `playerStatsTable` to a record type that includes:

* Player (`Player`) - type `string`

* Nation (`Nation`) - type `string`

* League (`League`) - type `string`

* AgeNextYear (`Age + 1`) - type `int`

* HalfGoalsScored (`GoalsScored / 2.0`) - type `float`

Hint: Create a new type.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type TeamRecord =
  {
    Player: string
    Nation: string
    League: string
    AgeNextYear: int
    HalfGoalsScored: float
  }
val it: TeamRecord list =
  [{ Player = "Robert Lewandowski"
     Nation = "pl POL"
     League = "deBundesliga"
     AgeNextYear = 33
     HalfGoalsScored = 17.5 }; { Player = "Kylian Mbappé"
                                 Nation = "fr FRA"
                                 League = "frLigue 1"
                                 AgeNextYear = 23
                                 HalfGoalsScored = 14.0 };
   { Player = "Karim Benzema"
     Nation = "fr FRA"
     League = "esLa Liga"
     AgeNextYear = 34
     HalfGoalsScored = 13.5 }; { Player = "Ciro Immobile"
                                 Nation = "it ITA"
                                 League = "itSerie A"
                                 AgeNextYear = 32
                                 HalfGoalsScored = 13.5 };
   { Player = "Wissam Ben Yedder"
     Nation = "fr FRA"
     League = "frLigue 1"
     AgeNextYear = 31
     HalfGoalsScored = 12.5 }]
```

</details>
</span>
</p>
</div>

*)

