(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=football-collection-functions.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//football-collection-functions.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//football-collection-functions.ipynb)

# Using List collection functions and calculating summary statistics.

> Developed with [Davide Costa](https://github.com/DavideGCosta)
> 

You should now feel comfortable with the footballer dataset and how to work with
tuples, records, anonymous records. You should also know how to perform simple transformations.
With a large and heterogeneous dataset, it's useful to understand how to sort, group,
and filter the data, and also many other interesting List functions.

It is a good idea to browse the documentation for lists at the [F# language reference](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/Lists)
and the [F# core library](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-listmodule.html) documentation sites before you start.
For further discussion of collection functions, the related [F# for fun and profit](https://fsharpforfunandprofit.com/posts/list-module-functions/)
page is also useful.

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

* [List Functions.](#List-Functions)
  

  0 [List.take](#1-List-take)
    
  
  1 [List.truncate](#2-List-truncate)
    
  
  2 [List.distinct](#3-List-distinct)
    
  
  3 [List.countBy](#4-List-countBy)
    
  
  4 [List.filter](#5-List-filter)
    
  
  5 [List.sort and List.sortDescending](#6-List-sort-and-List-sortDescending)
    
  
  6 [List.sortBy and List.sortByDescending](#7-List-sortBy-and-List-sortByDescending)
    
  
  7 [List.splitInto](#8-List-splitInto)
    
  
  8 [List.groupBy](#9-List-groupBy)
    
  

* [Statistics List Functions.](#Statistics-List-Functions)
  

  0 [List.max](#1-List-max)
    
  
  1 [List.min](#2-List-min)
    
  
  2 [List.maxBy](#3-List-maxBy)
    
  
  3 [List.minBy](#4-List-minBy)
    
  
  4 [List.sum](#5-List-sum)
    
  
  5 [List.sumBy](#6-List-sumBy)
    
  
  6 [List.average](#7-List-average)
    
  
  7 [List.averageBy](#8-List-averageBy)
    
  
  8 [Seq.stDev](#9-Seq-stDev)
    
  
  9 [Seq.pearsonOfPairs](#10-Seq-pearsonOfPairs)
    
  

* [Further Statistics practice.](#Further-Statistics-practice)
  

  0 [List.countBy, List.filter and List.averageBy](#1-List-countBy-List-filter-and-List-averageBy)
    
  
  1 [List.groupBy, List.map and transformations](#2-List-groupBy-List-map-and-transformations)
    
  
  2 [List.sortDescending, List.splitInto, List.map and Seq.stDev](#3-List-sortDescending-List-splitInto-List-map-and-Seq-stDev)
    
  

## List Functions.

### 1 List.take

`List.take 5` takes the first 5 rows.
`List.take 2` takes the first 2 rows

Example: Take the first 4 rows from `playerStatsTable` with `List.take`.

*)
playerStatsTable
|> List.take 4(* output: 
val it: CsvProvider<...>.Row list =
  [("Robert Lewandowski", "pl POL", "FW", "Bayern Munich", "deBundesliga", 32,
    34, 35);
   ("Kylian Mbappé", "fr FRA", "FW", "Paris S-G", "frLigue 1", 22, 35, 28);
   ("Karim Benzema", "fr FRA", "FW", "Real Madrid", "esLa Liga", 33, 32, 27);
   ("Ciro Immobile", "it ITA", "FW", "Lazio", "itSerie A", 31, 31, 27)]*)
(**
* Take the first 7 rows from `playerStatsTable` with `List.take`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: CsvProvider<...>.Row list =
  [("Robert Lewandowski", "pl POL", "FW", "Bayern Munich", "deBundesliga", 32,
    34, 35);
   ("Kylian Mbappé", "fr FRA", "FW", "Paris S-G", "frLigue 1", 22, 35, 28);
   ("Karim Benzema", "fr FRA", "FW", "Real Madrid", "esLa Liga", 33, 32, 27);
   ("Ciro Immobile", "it ITA", "FW", "Lazio", "itSerie A", 31, 31, 27);
   ("Wissam Ben Yedder", "fr FRA", "FW", "Monaco", "frLigue 1", 30, 37, 25);
   ("Patrik Schick", "cz CZE", "FW", "Leverkusen", "deBundesliga", 25, 27, 24);
   ("Son Heung-min", "kr KOR", "MF,FW", "Tottenham", "engPremier League", 29,
    35, 23)]
```

</details>
</span>
</p>
</div>

### 2 List.truncate

`List.truncate 5` takes the first 5 rows.
`List.truncate 2` takes the first 2 rows

You must have noted that `List.take` and `List.truncate` return similar outputs, but these are not exactly the same.
`List.take` gives you the exact number of items that you specify in the parameters,
while `List.truncate` takes at maximum the number of items you specified in the parameters.
Thus, in most cases both give you the exact same output, except if you ask for more items then the ones available in the List (List length).
In this particular scenario `List.truncate` returns the maximum number of elements (all the elements in the List),
while `List.take` returns an error, since it is supposed to take the exact number of elements you asked for, which is impossible in this particular case.

Example: Take the first 4 rows from `playerStatsTable` with `List.truncate`.

*)
playerStatsTable
|> List.truncate 4(* output: 
val it: CsvProvider<...>.Row list =
  [("Robert Lewandowski", "pl POL", "FW", "Bayern Munich", "deBundesliga", 32,
    34, 35);
   ("Kylian Mbappé", "fr FRA", "FW", "Paris S-G", "frLigue 1", 22, 35, 28);
   ("Karim Benzema", "fr FRA", "FW", "Real Madrid", "esLa Liga", 33, 32, 27);
   ("Ciro Immobile", "it ITA", "FW", "Lazio", "itSerie A", 31, 31, 27)]*)
(**
* Take the first 7 rows from `playerStatsTable` with `List.truncate`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: CsvProvider<...>.Row list =
  [("Robert Lewandowski", "pl POL", "FW", "Bayern Munich", "deBundesliga", 32,
    34, 35);
   ("Kylian Mbappé", "fr FRA", "FW", "Paris S-G", "frLigue 1", 22, 35, 28);
   ("Karim Benzema", "fr FRA", "FW", "Real Madrid", "esLa Liga", 33, 32, 27);
   ("Ciro Immobile", "it ITA", "FW", "Lazio", "itSerie A", 31, 31, 27);
   ("Wissam Ben Yedder", "fr FRA", "FW", "Monaco", "frLigue 1", 30, 37, 25);
   ("Patrik Schick", "cz CZE", "FW", "Leverkusen", "deBundesliga", 25, 27, 24);
   ("Son Heung-min", "kr KOR", "MF,FW", "Tottenham", "engPremier League", 29,
    35, 23)]
```

</details>
</span>
</p>
</div>

### 3 List.distinct

`List.distinct` returns the unique elements from the List.
`["hello"; "world"; "hello"; "hi"] |> List.distinct` returns `["hello"; "world"; "hi"]`

Example: From `playerStatsTable` `Nation` field find the unique elements with `List.distinct`.

*)
playerStatsTable
|> List.map(fun x -> x.Nation)
|> List.distinct(* output: 
val it: string list =
  ["pl POL"; "fr FRA"; "it ITA"; "cz CZE"; "kr KOR"; "eg EGY"; "no NOR";
   "ar ARG"; "es ESP"; "pt POR"; "br BRA"; "eng ENG"; "rs SRB"; "sn SEN";
   "tr TUR"; "dz ALG"; "be BEL"; "ca CAN"; "hr CRO"; "de GER"; "tn TUN";
   "ng NGA"; "co COL"; "ci CIV"; "jp JPN"; "at AUT"; "zw ZIM"; "nl NED";
   "sct SCO"; "uy URU"; "xk KVX"; "cm CMR"; "dk DEN"; "ml MLI"; "ch SUI";
   "ir IRN"; "pe PER"; ""; "se SWE"; "gq EQG"; "ro ROU"; "me MNE"]*)
(**
* From `playerStatsTable` `League` field find the unique elements with `List.distinct`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: string list =
  ["deBundesliga"; "frLigue 1"; "esLa Liga"; "itSerie A"; "engPremier League"]
```

</details>
</span>
</p>
</div>

### 4 List.countBy

`List.countBy` returns a list of paired tuples with the unique elements and their counts.

Example: From `playerStatsTable` `Team` field find the unique elements and their counts with `List.countBy`.

*)
playerStatsTable
|> List.countBy(fun x -> x.Team)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: (string * int) list =
  [("Bayern Munich", 3); ("Paris S-G", 3); ("Real Madrid", 3); ("Lazio", 2);
   ("Monaco", 3)]*)
(**
* From `playerStatsTable` `League` field find the unique elements and their counts with `List.countBy`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (string * int) list =
  [("deBundesliga", 36); ("frLigue 1", 46); ("esLa Liga", 30);
   ("itSerie A", 52); ("engPremier League", 36)]
```

</details>
</span>
</p>
</div>

### 5 List.filter

`List.filter` allows you to extract a subset of the dataset based on one or multiple conditions.

Example: `Filter` the `playerStatsTable` to get only portuguese players. (`Nation = "pt POR"`).
Remember that we have to look to the dataset to find the string correspondent to portuguese players,
which in this case is `"pt POR"`

*)
playerStatsTable
|> List.filter(fun x -> x.Nation = "pt POR")
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: CsvProvider<...>.Row list =
  [("Cristiano Ronaldo", "pt POR", "FW", "Manchester Utd", "engPremier League",
    36, 30, 18);
   ("Gonçalo Guedes", "pt POR", "FW,MF", "Valencia", "esLa Liga", 24, 36, 11);
   ("Bruno Fernandes", "pt POR", "MF", "Manchester Utd", "engPremier League",
    26, 36, 10);
   ("Bernardo Silva", "pt POR", "MF,FW", "Manchester City",
    "engPremier League", 26, 35, 8);
   ("Raphaël Guerreiro", "pt POR", "DF", "Dortmund", "deBundesliga", 27, 23, 4)]*)
(**
* `Filter` the `playerStatsTable` to get only 16 year-old players. (`Age = 16`).

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: CsvProvider<...>.Row list = []
```

</details>
</span>
</p>
</div>

### 6 List.sort and List.sortDescending

* `[1; 4; 5; 3; 6] |> List.sort` returns `[1; 3; 4; 5; 6]` (ascending sort).

* `[1; 4; 5; 3; 6] |> List.sortDescending` returns `[6; 5; 4; 3; 1]` (descending sort).

Example: map `playerStatsTable` to get a list of `Age` and sort it (ascending).

Since we want to sort the age List we first use `List.map` to get only that List.
Then we use `List.sort` to sort it.

*)
playerStatsTable
|> List.map(fun x -> x.Age)
|> List.sort
|> List.truncate 60 //just to observe the first 60 values, not a part of the exercise.(* output: 
val it: int list =
  [17; 17; 18; 18; 19; 19; 19; 19; 19; 20; 20; 20; 20; 21; 21; 21; 21; 21; 21;
   21; 21; 21; 21; 21; 21; 21; 21; 21; 21; 22; 22; 22; 22; 22; 22; 22; 22; 22;
   22; 22; 23; 23; 23; 23; 23; 23; 23; 23; 23; 23; 23; 23; 23; 23; 24; 24; 24;
   24; 24; 24]*)
(**
* map `playerStatsTable` to get a list of `GoalsScored` and sort it (ascending).
Hint:
To sort the GoalsScored List you first need to use `List.map` to get only that List.
Then use `List.sort` to sort it.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int list =
  [0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0;
   1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 1; 2; 2; 2;
   2; 2; 2; 2; 2; 2; 2; 2; 2; 2]
```

</details>
</span>
</p>
</div>

Example: Map `playerStatsTable` to get a list of `Age` and sort it (descending).

Since we want to sort the age List we first use `List.map` to get only that List.
Then we use `List.sortDescending` to sort it.

*)
playerStatsTable
|> List.map(fun x -> x.Age)
|> List.sortDescending
|> List.truncate 60 //just to observe the first 60 values, not a part of the exercise.(* output: 
val it: int list =
  [40; 36; 36; 36; 35; 34; 34; 34; 34; 34; 34; 34; 34; 33; 33; 33; 33; 33; 33;
   33; 33; 33; 33; 32; 32; 32; 32; 32; 32; 31; 31; 31; 31; 31; 31; 30; 30; 30;
   30; 30; 30; 30; 30; 30; 29; 29; 29; 29; 29; 29; 29; 29; 29; 29; 29; 29; 29;
   29; 28; 28]*)
(**
* Map `playerStatsTable` to get a list of `GoalsScored` and sort it (descending).
Hint:
To sort the GoalsScored List you first need to use `List.map` to get only that List.
Then use `List.sortDescending` to sort it.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int list =
  [35; 28; 27; 27; 25; 24; 23; 23; 22; 21; 21; 21; 20; 20; 18; 18; 17; 17; 17;
   17; 17; 17; 16; 16; 16; 16; 16; 15; 15; 13; 13; 13; 13; 13; 12; 12; 12; 12;
   12; 12; 11; 11; 11; 11; 11; 11; 11; 11; 11; 11; 11; 10; 10; 10; 10; 10; 10;
   10; 10; 10]
```

</details>
</span>
</p>
</div>

### 7 List.sortBy and List.sortByDescending

`List.sortBy` is very usefull to sort the dataset accordingly to a certain dataset field.

Example: sort (ascending) `playerStatsTable` by `Age` (`List.sortBy`).

*)
playerStatsTable
|> List.sortBy(fun x -> x.Age)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: CsvProvider<...>.Row list =
  [("Giorgio Scalvini", "it ITA", "DF,MF", "Atalanta", "itSerie A", 17, 18, 1);
   ("Alejandro Primo", "es ESP", "GK", "Levante", "esLa Liga", 17, 1, 0);
   ("Florian Wirtz", "de GER", "MF,FW", "Leverkusen", "deBundesliga", 18, 24,
    7); ("Destiny Udogie", "it ITA", "DF", "Udinese", "itSerie A", 18, 35, 5);
   ("Bukayo Saka", "eng ENG", "FW,MF", "Arsenal", "engPremier League", 19, 38,
    11)]*)
(**
* sort (ascending) `playerStatsTable` by `GoalsScored` (`List.sortBy`).

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: CsvProvider<...>.Row list =
  [("Stefan Ortega", "de GER", "GK", "Arminia", "deBundesliga", 28, 33, 0);
   ("Rui Patrício", "pt POR", "GK", "Roma", "itSerie A", 33, 38, 0);
   ("Philipp Pentke", "de GER", "GK", "Hoffenheim", "deBundesliga", 36, 1, 0);
   ("Pavao Pervan", "at AUT", "GK", "Wolfsburg", "deBundesliga", 33, 6, 0);
   ("Nick Pope", "eng ENG", "GK", "Burnley", "engPremier League", 29, 36, 0)]
```

</details>
</span>
</p>
</div>

Example: sort (descending) `playerStatsTable` by `Age` (`List.sortByDescending`).

*)
playerStatsTable
|> List.sortByDescending(fun x -> x.Age)
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: CsvProvider<...>.Row list =
  [("Gianluca Pegolo", "it ITA", "GK", "Sassuolo", "itSerie A", 40, 1, 0);
   ("Cristiano Ronaldo", "pt POR", "FW", "Manchester Utd", "engPremier League",
    36, 30, 18);
   ("Fernandinho", "br BRA", "MF,DF", "Manchester City", "engPremier League",
    36, 19, 2);
   ("Philipp Pentke", "de GER", "GK", "Hoffenheim", "deBundesliga", 36, 1, 0);
   ("Daniele Padelli", "it ITA", "GK", "Udinese", "itSerie A", 35, 3, 0)]*)
(**
* sort (descending) `playerStatsTable` by `GoalsScored` (`List.sortByDescending`).

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: CsvProvider<...>.Row list =
  [("Robert Lewandowski", "pl POL", "FW", "Bayern Munich", "deBundesliga", 32,
    34, 35);
   ("Kylian Mbappé", "fr FRA", "FW", "Paris S-G", "frLigue 1", 22, 35, 28);
   ("Karim Benzema", "fr FRA", "FW", "Real Madrid", "esLa Liga", 33, 32, 27);
   ("Ciro Immobile", "it ITA", "FW", "Lazio", "itSerie A", 31, 31, 27);
   ("Wissam Ben Yedder", "fr FRA", "FW", "Monaco", "frLigue 1", 30, 37, 25)]
```

</details>
</span>
</p>
</div>

### 8 List.splitInto

`List.splitInto` is very usefull to split your dataset into multiple subsets.
This function is commonly used to generate quantiles by splitting a sorted List.
For instance, for investment strategies financial assets are usually sorted by a certain signal
and then splitted into quantiles. If the signal has a positive sign, it means that the long strategy consists of going long
on the first quantile stocks, and the long-short strategy consists of going long on the first quantile stocks and short on the last quantile stocks.

Note: `List.splitInto` receives one parameter which refers to the number of groups you want to create out of the dataset.

Example: Sort the `playerStatsTable` by `GoalsScored` and then split the dataset into 4 groups using `List.sortBy` and `List.splitInto`.

*)
playerStatsTable
|> List.sortBy(fun x -> x.GoalsScored)
|> List.splitInto 4
|> List.truncate 2 //just to observe the first 2 groups Lists, not a part of the exercise.
|> List.map(fun x -> x |> List.truncate 5) //just to observe the first 5 rows of each group List, not a part of the exercise.(* output: 
val it: CsvProvider<...>.Row list list =
  [[("Stefan Ortega", "de GER", "GK", "Arminia", "deBundesliga", 28, 33, 0);
    ("Rui Patrício", "pt POR", "GK", "Roma", "itSerie A", 33, 38, 0);
    ("Philipp Pentke", "de GER", "GK", "Hoffenheim", "deBundesliga", 36, 1, 0);
    ("Pavao Pervan", "at AUT", "GK", "Wolfsburg", "deBundesliga", 33, 6, 0);
    ("Nick Pope", "eng ENG", "GK", "Burnley", "engPremier League", 29, 36, 0)];
   [("Quentin Merlin", "fr FRA", "DF,MF", "Nantes", "frLigue 1", 19, 28, 2);
    ("Pascal Groß", "de GER", "MF,DF", "Brighton", "engPremier League", 30, 29,
     2);
    ("Mads Pedersen", "dk DEN", "MF,DF", "Augsburg", "deBundesliga", 24, 29, 2);
    ("Lukas Kübler", "de GER", "DF,MF", "Freiburg", "deBundesliga", 28, 29, 2);
    ("Josan", "es ESP", "DF,MF", "Elche", "esLa Liga", 31, 31, 2)]]*)
(**
* Sort the `playerStatsTable` by `Age` and then split the dataset into 5 groups using `List.sortBy` and `List.splitInto`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: CsvProvider<...>.Row list list =
  [[("Giorgio Scalvini", "it ITA", "DF,MF", "Atalanta", "itSerie A", 17, 18, 1);
    ("Alejandro Primo", "es ESP", "GK", "Levante", "esLa Liga", 17, 1, 0);
    ("Florian Wirtz", "de GER", "MF,FW", "Leverkusen", "deBundesliga", 18, 24,
     7); ("Destiny Udogie", "it ITA", "DF", "Udinese", "itSerie A", 18, 35, 5);
    ("Bukayo Saka", "eng ENG", "FW,MF", "Arsenal", "engPremier League", 19, 38,
     11)];
   [("Lautaro Martínez", "ar ARG", "FW", "Inter", "itSerie A", 23, 35, 21);
    ("Christopher Nkunku", "fr FRA", "FW,MF", "RB Leipzig", "deBundesliga", 23,
     34, 20);
    ("Tammy Abraham", "eng ENG", "FW", "Roma", "itSerie A", 23, 37, 17);
    ("Ludovic Blas", "fr FRA", "MF,FW", "Nantes", "frLigue 1", 23, 35, 10);
    ("Emmanuel Dennis", "ng NGA", "FW,MF", "Watford", "engPremier League", 23,
     33, 10)]]
```

</details>
</span>
</p>
</div>

### 9 List.groupBy

`List.groupBy` allows you to group elements of a list.
It takes a key-generating function and a list as inputs.
The function is executed on each element of the List, returning a list of tuples
where the first element of each tuple is the key and the second is a list of the elements for which the function produced that key.

Example: Group the `playerStatsTable` by `Nation` using `List.groupBy`.

*)
playerStatsTable
|> List.groupBy(fun x -> x.Nation)
|> List.truncate 2 //just to observe the first 2 groups Lists, not a part of the exercise.
|> List.map(fun (x, xs) -> x, xs |> List.truncate 5) //just to observe the first 5 rows of each group List, not a part of the exercise.(* output: 
val it: (string * CsvProvider<...>.Row list) list =
  [("pl POL",
    [("Robert Lewandowski", "pl POL", "FW", "Bayern Munich", "deBundesliga",
      32, 34, 35);
     ("Przemysław Frankowski", "pl POL", "DF", "Lens", "frLigue 1", 26, 37, 6);
     ("Matty Cash", "pl POL", "DF", "Aston Villa", "engPremier League", 23, 38,
      4)]);
   ("fr FRA",
    [("Kylian Mbappé", "fr FRA", "FW", "Paris S-G", "frLigue 1", 22, 35, 28);
     ("Karim Benzema", "fr FRA", "FW", "Real Madrid", "esLa Liga", 33, 32, 27);
     ("Wissam Ben Yedder", "fr FRA", "FW", "Monaco", "frLigue 1", 30, 37, 25);
     ("Moussa Dembélé", "fr FRA", "FW", "Lyon", "frLigue 1", 25, 30, 21);
     ("Martin Terrier", "fr FRA", "FW,MF", "Rennes", "frLigue 1", 24, 37, 21)])]*)
(**
* Group the `playerStatsTable` by `Age` using `List.groupBy`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (int * CsvProvider<...>.Row list) list =
  [(32,
    [("Robert Lewandowski", "pl POL", "FW", "Bayern Munich", "deBundesliga",
      32, 34, 35);
     ("Marco Reus", "de GER", "MF,FW", "Dortmund", "deBundesliga", 32, 29, 9);
     ("Ivan Perišić", "hr CRO", "DF", "Inter", "itSerie A", 32, 35, 8);
     ("Axel Witsel", "be BEL", "MF,DF", "Dortmund", "deBundesliga", 32, 29, 2);
     ("Ivan Radovanović", "rs SRB", "DF,MF", "Salernitana", "itSerie A", 32,
      14, 1)]);
   (22,
    [("Kylian Mbappé", "fr FRA", "FW", "Paris S-G", "frLigue 1", 22, 35, 28);
     ("Gianluca Scamacca", "it ITA", "FW", "Sassuolo", "itSerie A", 22, 36, 16);
     ("Moussa Diaby", "fr FRA", "FW,MF", "Leverkusen", "deBundesliga", 22, 32,
      13);
     ("Randal Kolo Muani", "fr FRA", "FW,MF", "Nantes", "frLigue 1", 22, 36,
      12);
     ("Mason Mount", "eng ENG", "MF", "Chelsea", "engPremier League", 22, 32,
      11)])]
```

</details>
</span>
</p>
</div>

## Statistics List Functions

### 1 List.max

`[1; 4; 5; 3; 6] |> List.max` returns `6` (the highest value in the List).

Example: Map `playerStatsTable` to get the `Age` List, and find the maximum (`List.max`).

*)
playerStatsTable
|> List.map(fun x -> x.Age)
|> List.max(* output: 
val it: int = 40*)
(**
* Map `playerStatsTable` to get the `GoalsScored` List, and find the maximum (`List.max`).

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int = 35
```

</details>
</span>
</p>
</div>

### 2 List.min

`[1; 4; 5; 3; 6] |> List.min` returns `1` (the lowest value in the List).

Example: Map `playerStatsTable` to get the `Age` List, and find the minimum (`List.min`).

*)
playerStatsTable
|> List.map(fun x -> x.Age)
|> List.min(* output: 
val it: int = 17*)
(**
* Map `playerStatsTable` to get the `GoalsScored` List, and find the minimum (`List.min`).

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int = 0
```

</details>
</span>
</p>
</div>

### 3 List.maxBy

Sometimes you want the element with the "maximum y" where "y" is the result of applying a particular function to a list element. This is what `List.maxBy` is for. This function is best understood by seeing an example.

Example: Find the player in `playerStatsTable` with the maximum `Age` using `maxBy`. What we need to do then is write a function that takes a player as input and outputs the players age. `List.maxBy` will then find the player that is the maxiumum after transforming it using this function.

*)
playerStatsTable
|> List.maxBy(fun x -> x.Age)(* output: 
val it: CsvProvider<...>.Row =
  ("Gianluca Pegolo", "it ITA", "GK", "Sassuolo", "itSerie A", 40, 1, 0)*)
(**
* Find the maximum `playerStatsTable` row by `GoalsScored` using `maxBy`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: CsvProvider<...>.Row =
  ("Robert Lewandowski", "pl POL", "FW", "Bayern Munich", "deBundesliga", 32,
   34, 35)
```

</details>
</span>
</p>
</div>

### 4 List.minBy

Sometimes you want the element with the "minimum y" where "y" is the result of applying a particular function to a list element. This is what `List.minBy` is for.

Example: Find the player in `playerStatsTable` with the minimum `Age` using `minBy`.

*)
playerStatsTable
|> List.minBy(fun x -> x.Age)(* output: 
val it: CsvProvider<...>.Row =
  ("Giorgio Scalvini", "it ITA", "DF,MF", "Atalanta", "itSerie A", 17, 18, 1)*)
(**
* Find the minimum `playerStatsTable` row by `GoalsScored` using `minBy`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: CsvProvider<...>.Row =
  ("Stefan Ortega", "de GER", "GK", "Arminia", "deBundesliga", 28, 33, 0)
```

</details>
</span>
</p>
</div>

### 5 List.sum

`[1; 4; 5; 3; 6] |> List.sum` returns `19` (sum of the List elements).

Example: Calculate the total number of years lived by all players. Hint: transform (`List.map`) each element of `playerStatsTable` into an integer representing the player's `Age` and then get the sum (`List.sum`) of all the players' ages (the result should be an `int`).

*)
playerStatsTable
|> List.map(fun x -> x.Age)
|> List.sum(* output: 
val it: int = 5270*)
(**
* Calculate the total goals scored (`GoalsScored`) by all players in `playerStatsTable`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int = 1470
```

</details>
</span>
</p>
</div>

### 6 List.sumBy

We are using a dataset that has multiple fields per List element. If you want to get the sum for particular fields it convenient to use `List.sumBy`.
It takes a function and transforms each element using that function and afterward sums all the transformed elements. It is like an `List.map` and `List.sum` combined into one function.

Example: Use `List.sumBy` to calculate the total number of years lived by all players in `playerStatsTable`. Remember that each player has lived `Age` years.

*)
playerStatsTable
|> List.sumBy(fun x -> x.Age)(* output: 
val it: int = 5270*)
(**
* Find the sum of the `GoalsScored` by all players in `playerStatsTable` using `List.sumBy`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int = 1470
```

</details>
</span>
</p>
</div>

### 7 List.average

`[1.0; 2.0; 5.0; 2.0] |> List.average` returns `2.5` (the average of all the List elements).

Example: Transform `playerStatsTable` into a list of the players' ages (`Age`) and find the average `Age` (`List.average`).
The field `x.Age` needs to be transformed from `int` to `float` because `List.average` only works with `floats` or `decimals`.

*)
playerStatsTable
|> List.map(fun x -> float x.Age)
|> List.average(* output: 
val it: float = 26.35*)
(**
* Use `List.map` to transform `playerStatsTable` into a list of the players' `GoalsScored` and find the average `GoalsScored` (`List.average`).
Hint: The variable `x.GoalsScored` needs to be transformed from `int` to `float` since `List.average` only works with `floats` or `decimals`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float = 7.35
```

</details>
</span>
</p>
</div>

### 8 List.averageBy

We are using a dataset that has multiple fields per List element. If you want to get the average for particular fields it convenient to use `List.averageBy`.
It takes a function and transforms each element using that function and afterward averages all the transformed elements. It is like an `List.map` and `List.average` combined into one function.

Example: Find the average `Age` using `List.averageBy`.
The `Age` needs to be transformed from `int` to `float` since `List.averageBy` only works with `floats` or `decimals`.

*)
playerStatsTable
|> List.averageBy(fun x -> float x.Age)(* output: 
val it: float = 26.35*)
(**
* Find the average `GoalsScored` using `List.averageBy`.
Hint: The `GoalsScored` needs to be transformed from `int` to `float` since `List.averageBy` only works with `floats` or `decimals`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float = 7.35
```

</details>
</span>
</p>
</div>

### 9 Seq.stDev

For `Seq.stDev` to work, we loaded the `FSharp.Stats nuget` (`#r "nuget: FSharp.Stats, 0.5.0"`).
This nuget contains the standard deviation function.
Besides this we also opened the module `FSharp.Stats` (`open FSharp.Stats`).
[FSharp.Stats documentation](https://fslab.org/FSharp.Stats/)

Example: Use `List.map` to transform `playerStatsTable` by `GoalsScored` and find the standard deviation. (`Seq.stDev`).
Note that for `Seq.stDev` to work the values need to be `floats` or `decimals`, so we need to transform the `GoalsScored` from `int` to `float`.

*)
playerStatsTable
|> List.map(fun x -> float x.GoalsScored)
|> Seq.stDev(* output: 
val it: float = 6.733811781*)
(**
* Transform `playerStatsTable` into a list of the players' `Age`'s and find the standard deviation. (`Seq.stDev`).
Hint: You need to transform `Age` values from `int` to `floats`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float = 4.343018426
```

</details>
</span>
</p>
</div>

### 10 Seq.pearsonOfPairs

In order to perform correlations we have to load and open the namespace `FSharp.Stats`.
Also, we `open FSharpe.Stats.Correlation` to allow a easier access to the correlation functions.

It will be helpfull to check the [FSharp.Stats.Correlation Documentation](https://fslab.org/FSharp.Stats/reference/fsharp-stats-correlation-seq.html#pearson) before starting the exercises.

Example: Test the correlation between `MatchesPlayed` and `GoalsScored` using `pearsonOfPairs`.

`Seq.pearsonOfPairs` expects a list of tuples (x1 * x2), computing the correlation between x1 and x2.
So we use `List.map` to get a list of tuples with (`MatchesPlayed`, `GoalsScored`).
Then we only need to pipe (`|>`) to `Seq.pearsonOfPairs`.

*)
playerStatsTable
|> List.map(fun x -> x.MatchesPlayed, x.GoalsScored)
|> Seq.pearsonOfPairs(* output: 
val it: float = 0.4641226145*)
(**
* Test the correlation between `MatchesPlayed` and `Age` using `pearsonOfPairs`.
Hints:
`Seq.pearsonOfPairs` expects a list of tuples (x1 * x2). Use `List.map` to get a list of tuples with (`MatchesPlayed`, `Age`).
Then you only need to pipe (`|>`) to `Seq.pearsonOfPairs`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float = -0.07750635099
```

</details>
</span>
</p>
</div>

* Test the correlation between `GoalsScored` and `Age` using `pearsonOfPairs`.
Hints:
`Seq.pearsonOfPairs` expects a list of tuples (x1 * x2). Use `List.map` to get a list of tuples with (`GoalsScored`, `Age`).
Then you only need to pipe (`|>`) to `Seq.pearsonOfPairs`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float = 0.01881518088
```

</details>
</span>
</p>
</div>

## Further Statistics practice

Now that you should feel confortable with `List.filter`, `List.groupBy`, `List.splitInto`
and also some f# statistics functions, let's combine those concepts together.

### 1 List.countBy, List.filter and List.averageBy

Example: Find the average goals scored by portuguese players.

In order to find the average goals for portuguese players we know that we need to use `List.filter`.
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
|> List.averageBy(fun x -> float x.Age)(* output: 
val it: float = 28.66666667*)
(**
* Find the average age for players playing on the Premier League  .
Hint:
You'll first need to use `List.filter` to get only players from the Premier League (`x.League = "engPremier League"`).
Then use averageBy to compute the average by age, don't forget to use `float x.Age` to transform age values to float type.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float = 25.58333333
```

</details>
</span>
</p>
</div>

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
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
type TeamAndAvgGls =
  {
    Team: string
    AvgGoalsScored: float
  }
val it: TeamAndAvgGls list =
  [{ Team = "Bayern Munich"
     AvgGoalsScored = 14.66666667 }; { Team = "Paris S-G"
                                       AvgGoalsScored = 15.33333333 };
   { Team = "Real Madrid"
     AvgGoalsScored = 18.0 }; { Team = "Lazio"
                                AvgGoalsScored = 19.0 };
   { Team = "Monaco"
     AvgGoalsScored = 11.66666667 }]*)
(**
or

*)
//example using tuple:
playerStatsTable
|> List.groupBy(fun x -> x.Team)
|> List.map(fun (team, playerStats) -> team, playerStats |> List.averageBy(fun playerStats -> float playerStats.GoalsScored))
|> List.truncate 5 //just to observe the first 5 rows, not a part of the exercise.(* output: 
val it: (string * float) list =
  [("Bayern Munich", 14.66666667); ("Paris S-G", 15.33333333);
   ("Real Madrid", 18.0); ("Lazio", 19.0); ("Monaco", 11.66666667)]*)
(**
* Group `playerStatsTable` by `League` and then compute the Average `Age` by group.
Hint: Use `groupBy` to group by league (`League`).
Then use `averageBy` to compute the average by age (`Age`) and pipe it
(`|>`) to `List.map` to organize the data in a record or tuple with League (`League`) and Average Age.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type LeagueAndAvgAge =
  {
    League: string
    AverageAge: float
  }
val it: (string * float) list =
  [("deBundesliga", 27.11111111); ("frLigue 1", 25.7173913);
   ("esLa Liga", 26.53333333); ("itSerie A", 26.80769231);
   ("engPremier League", 25.58333333)]
```

</details>
</span>
</p>
</div>

### 3 List.sortDescending, List.splitInto, List.map and Seq.stDev

* From `playerStatsTable` sort the players' `Age` (descending), split the dataset into quartiles (4-quantiles) and compute the standard deviation for each quantile.
Hint: You only need the `Age` field from the dataset, so you can use `map` straight away to get the `Age` List.
Sort that List with `List.sortDescending`, and then split it into 4 parts using `List.splitInto`.
Finally use `List.map` to iterate through each quantile and apply the function `Seq.stDev`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float list = [2.294714424; 0.9082389329; 0.9171829097; 1.59604102]
```

</details>
</span>
</p>
</div>

*)

