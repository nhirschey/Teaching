(**
---
title: Webscraping HTML to CSV
category: Assignments
categoryindex: 2
index: 0
---
*)

(***hide***)
// specific link:
// https://fbref.com/en/comps/Big5/2019-2020/stats/players/2019-2020-Big-5-European-Leagues-Stats

// most recent link:
// "https://fbref.com/en/comps/Big5/stats/players/Big-5-European-Leagues-Stats"

(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

# Webscraping HTML to CSV

> Developed with [Davide Costa](https://github.com/DavideGCosta)

For the **Football Players' Exercises** we are trying to scrape data from [Sports Reference's](https://www.sports-reference.com/) [Football Reference](https://fbref.com/en/) website. The particular data that we want to scrape is player data from the [Big 5 European Leauges table](https://fbref.com/en/comps/Big5/stats/players/Big-5-European-Leagues-Stats). This data consists on the stats of the players that played in the big 5 football european leagues in the most recent season. Since we need to scrape the data from a webpage and store it in a csv file we will need to use 2 type providers from [FSharp.Data](https://fsharp.github.io/FSharp.Data/):

- We'll use the [HTML Type Provider](https://fsprojects.github.io/FSharp.Data/library/HtmlProvider.html) to scrape the html from the webpage and access the players' table.  
- We'll use the [CSV Type Provider](https://fsprojects.github.io/FSharp.Data/library/CsvProvider.html) to save the data to a csv file.  

Both type providers are located in `FSharp.Data` assembly. To use it we need to load and open `FSharp.Data`.
*)

#r "nuget: FSharp.Data"
open FSharp.Data

(**
## Get the players' table using HTML type provider.
We need to define the type of data that we're going to access.
We do this by providing a sample of the data.
The type provider uses the sample to generate code to read data with that format. 
*)

type Big5EuropeanLeagues = 
    HtmlProvider<"https://fbref.com/en/comps/Big5/2021-2022/stats/players/2021-2022-Big-5-European-Leagues-Stats">

(**
The type `Big5EuropeanLeagues` contains information about the structure of the web page. It knows what the html is, it knows what the html tables are, etc.
*)

(**
Now that we have the `Big5EuropeanLeagues` HTML type defined we can use `GetSample()` to load the sample webpage
that we used to define the type.  

*)
let big5 = Big5EuropeanLeagues.GetSample()

(**
Now, we have the HTML of the webpage stored in the variable `big5`. Let's observe the first 200 characters of the HTML.
*)
(***do-not-eval***)
big5.Html.ToString()[..200]
(***hide***)
big5.Html.ToString().[..200]
(*** include-it ***)
(**
From the tables available in the webpage, let's assign the player stats table to a variable called `footballers`.
*)

let footballers = big5.Tables.``Player Standard Stats 2021-2022 Big 5 European Leagues``

(**
Now let's observe the first 3 table rows. The `Rows` property gives us an array of rows.
We can index into the array using `[..2]` to limit it to the first 3 rows.
*)
(***do-not-eval***)
footballers.Rows[..2]
(***hide***)
footballers.Rows.[..2]
(*** include-it ***)
(**
Let's look at the header fields in the table using the property `Headers`.
*)
footballers.Headers
(*** include-it ***)
(**
Let´s look at the first 5 rows of fields `Player` and `Age`.
*)
(***do-not-eval***)
[ for row in footballers.Rows[..4] do row.Player, row.Age]
(***hide***)
[ for row in footballers.Rows.[..4] do row.Player, row.Age]
(*** include-it ***)

(**
## Clean the data

The table data is not exactly what we need. To work with it, we need to clean it up.

### Repeated headers

The table header line repeats after every 25 players.
We can see this if we look at the 26th row of the table.

*)

(***do-not-eval***)
footballers.Rows[25]
(***hide***)
footballers.Rows.[25]
(***include-it***)

(**
We can remove these lines by using [Array.filter](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-arraymodule.html#filter). `Array.filter` applies a function to each element of the array and
returns only those elements where the function evaluates to `true`.
We'll create a function that evaluates to `true` if the player's name *does not* equal `"Player"`
*)

let footballersNoHeaders = 
    footballers.Rows
    // 1. Rows where row.Player does not equal "Player"
    |> Array.filter (fun row -> row.Player <> "Player") 

(**
### Removing missing values

In order to make some arithmetic operations with age and goals scored, we need to convert the data into integers or floats/decimals. But some players' age and goals scored is missing, and converting empty strings ("") into these types returns an error.  
We remove those lines by removing filtering to rows where age and goals are not "".
*)

let footballersClean =
    footballersNoHeaders
    |> Array.filter (fun row -> 
        row.Age <> "" && 
        row.``Performance - Gls`` <> "")

(**
Now we have the data we need, but
there are more than 2500 rows.  
*)

footballersClean.Length

(**
For our exercises we don't need so many observations,
so let's transform it to get the best players by position!
*)

let playersByPosition =
    footballersClean
    // 1. filter out odd positions with few players
    |> Array.filter(fun x -> x.Pos <> "GK,FW" && x.Pos <> "FW,DF" && x.Pos <> "DF,FW" )
    // 2. group players by position
    |> Array.groupBy(fun x -> x.Pos)
    // 3. sort positions.
    |> Array.sortBy (fun (position, _players) -> position)


let bestPlayersByPosition =
    // This will get top 25 players for each position
    // We create a new list by looping through each position group.
    [| for (position, playersInPosition) in playersByPosition do
        // 4. Sort best to worst (and convert to list at the end)
        let bestToWorst =
            playersInPosition
            |> Array.sortByDescending (fun player ->
                int player.``Performance - Gls``, player.Matches)
        // 5. Truncate to top 25
        let top25 = bestToWorst |> Array.truncate 25         
        top25 |]
    // 6. concatenate to a single big array
    |> Array.concat
    
let bestPlayers = 
    // sort best by position to overall top goal scorers,
    // with ties broken by alphabetical sorting on 
    // the players name and squad.
    bestPlayersByPosition
    |> Array.sortByDescending (fun player -> 
        int player.``Performance - Gls``,
        player.Player,
        player.Squad)

(**
## Create a Csv and store the Data using Csv Provider.  
Now that the data is scraped from the webpage and stored to `FootballPlayersParsedTable` variable.
We need to save the data to a Csv file in order to use it in the Exercises.
*)

(**
First we need to construct a sample of the data that will be stored in the comma separated (csv) File.
By running `footballers.Headers` as done previously, we can easily observe that the table has fields that will not be used.  
The csv file doesn't need to contain all those fields, we only need: 

- `Player` (Players' Name)  
- `Nation` (Players' Nationality)  
- `Pos` (Players' Position)  
- `Squad` (Players' Team)  
- `Comp` (Players' League)  
- `Age` (Players' Age)   
- `Playing Time - MP` (Players' Matches Played)  
- `Performance - Gls` (Players' Goals Scored)  

The csv type provider will infer csv field types from a sample that you give it.
It can infer the types from rows of the sample or from explicitly defined types
added in parentheses after column names.
We'll use explicit column type definitions in our sample.

*)

[<Literal>]
let FootballPlayersCsvSample = 
    "Player (string)," +
    "Nation (string)," +
    "Position (string)," +
    "Team (string)," +
    "League (string)," +
    "Age (int)," +
    "MatchesPlayed (int)," +
    "GoalsScored (int)"

// the sample csv file that we've created:
FootballPlayersCsvSample
(***include-it***)


(**
With the sample created, now we define the type from the sample.
*)

type FootballPlayersCsv = CsvProvider<FootballPlayersCsvSample,ResolutionFolder = __SOURCE_DIRECTORY__>

(**
Now that we have the data and the Csv sample let's create a "list of CSV rows".
*)

let bestPlayersCsvRows = 
    [ for player in bestPlayers do
        FootballPlayersCsv.Row( 
            player = player.Player,
            nation = player.Nation,
            position = player.Pos,
            team = player.Squad,
            league = player.Comp,
            // For the age variable just take the year (first two digits).
            // Sometimes 29 and 185 days old is given as "29-185" and we
            // want to throw away the days part.
            age = int player.Age.[0..1], 
            matchesPlayed = int player.``Playing Time - MP``,
            goalsScored = int player.``Performance - Gls`` ) ]

(**
Note that we use `int` to convert `age`, `matchesPlayed` and `goalsScored` because those fields' values are `strings` in the html table and we want `integers` instead.  
*)

(**
Let's look at the first 5 csv rows. We don't need to use `bestPlayersCsvRows.Rows` because the variable is already a list of csv rows.  
*)

(***do-not-eval***)
bestPlayersCsvRows[0..4]
(***hide***)
bestPlayersCsvRows.[0..4]
(*** include-it ***)
(**
Rather than a "list of Csv rows", we want a "Csv file".  Here's how we do that.
*)
let bestPlayersCsvFile = new FootballPlayersCsv(bestPlayersCsvRows)

(**
Ok, let's write the file. Remember that `__SOURCE_DIRECTORY__` is a magic variable that points 
to whatever folder this code file (aka the source code file) is contained in.
So this will write the data to a csv file named "FootballPlayers.csv" in the current directory.
*)

let filePath = System.IO.Path.Combine(__SOURCE_DIRECTORY__,"FootballPlayers.csv")
bestPlayersCsvFile.Save(filePath)

(**
And if you want to read the data back in from the file to see that it works:
*)
let backIn = FootballPlayersCsv.Load(filePath)

backIn.Rows
|> Seq.truncate 5
|> Seq.iter (printfn "%A")
(*** include-output ***)

