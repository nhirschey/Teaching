(**
---
title: Webscraping HTML to CSV
category: Webscraping Assignment
categoryindex: 1
index: 1
---
*)

(***hide***)
// original link:
// https://fbref.com/en/comps/Big5/2019-2020/stats/players/2019-2020-Big-5-European-Leagues-Stats

(**
# Webscraping HTML to CSV

> Originally developed by [Davide Costa](https://github.com/DavideGCosta)

For the **Football Players' Exercises** we are trying to scrape data from [Sports Reference's](https://www.sports-reference.com/) [Football Reference](https://fbref.com/en/) website. The particular data that we want to scrap is player data from the [Big 5 European Leauges table](https://fbref.com/en/comps/Big5/stats/players/Big-5-European-Leagues-Stats). This data consists on the stats of the players that played in the big 5 football european leagues in the most recent season. Since we need to scrape the data from a webpage and store it in a csv file we will need to use 2 type providers from [FSharp.Data](https://fsharp.github.io/FSharp.Data/):

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
    HtmlProvider<"https://fbref.com/en/comps/Big5/stats/players/Big-5-European-Leagues-Stats">

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
From the tables available in the webpage, let's assign the `Player Standard Stats 2019-2020 Big 5 European Leagues` to a variable called `footballers`.
*)

let footballers = big5.Tables.``Player Standard Stats 2019-2020 Big 5 European Leagues``

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
 The table data is not exactly what we need. To work with it, we need to clean it up.

- **Repeated Headers.**
    - If we run `footballers.Rows[25]` we can see that it corresponds to the headers and not to a player row. This means that the header is in line 0 and then repeats after every 25 players.  
    - We remove those lines by using `Array.filter(fun x -> x.Player <> "Player")`. This code filters the data set to include only rows where the player's name is not equal to the string `"Player"`.  
- **Age and Goals Scored with empty strings.**    
    - In order to make some arithmetic operations with age and goals scored, we need to convert the data into integers or floats/decimals.  
    - But some players' age and goals scored is missing, and converting empty strings ("") into these types returns an error.  
    - We remove those lines by using `Array.filter(fun x -> x.Age <> "" and x.Performance - Gls <> "")`.  
*)

let footballersParsed =
    footballers.Rows
    // Array.filter 
    |> Array.filter (fun x -> 
        //
        x.Player <> "Player" && 
        x.Age <> "" && 
        x.``Performance - Gls`` <> "")
    |> Array.sortByDescending(fun x -> 
        x.``Performance - Gls``,
        x.Player,
        x.Squad)

(**
## Create a Csv and store the Data using Csv Provider.  
Now that the data is scraped from the webpage and stored to `FootballPlayersParsedTable` variable.
We need to save the data to a Csv file in order to use it in the Exercises.
*)

(**
First we need to construct a sample of the data that will be stored in the Csv File.
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

For the sample provider we provide the header and 2 example rows. The header gives the field names. The example rows give example data that lets the type provider figure out the type of the fields.

- The only point of `player1` and `aardvark` is to point out that the player field contains `strings`. You could use any string in place of `player1` or `aardvark`. The same is true for the `Nation`, `Position`, `Team` and `League` fields.  
- For `Age`, `MatchesPlayed` and `GoalsScored`, we use `00` to represent that the field contains `integers`.  You could use any other `integer` for the sample.  
*)

[<Literal>]
let FootballPlayersCsvSample = 
    " Player, Nation, Position, Team, League, Age, MatchesPlayed, GoalsScored
      player1, nation1, pos1, team1, league1, 00, 00, 00
      aardvark, nation2, pos2, team2, league2, 00, 00, 00"

(**
With the sample created, now we define the type from the sample.
*)

type FootballPlayersCsv = CsvProvider<FootballPlayersCsvSample>

(**
Now that we have the data and the Csv sample let's create an "array of CSV rows". First we'll show the code and then explain it.  
*)

let FootballPlayersCsvRows = 
    footballersParsed
    |> Array.map(fun htmlRow -> 
        FootballPlayersCsv.Row
            ( player = htmlRow.Player,
              nation = htmlRow.Nation,
              position = htmlRow.Pos,
              team = htmlRow.Squad,
              league = htmlRow.Comp,
              age = int htmlRow.Age,
              matchesPlayed = int htmlRow.``Playing Time - MP``,
              goalsScored = int htmlRow.``Performance - Gls`` ))

(**
The steps are:
1. We start with the array that contains the data (`footballersParsed`).  
2. Then we use `Array.map` to iterate through the rows of the html table and convert each row of the html table into a `FootballPlayersCsv.Row`.  
    - We don't want all the fields from the html table. We only want the ones specified in the `FootballPlayersCsv` type.  
    - So for each field in a `FootballPlayersCsv` row we assign the corresponding field from the html table row.  
3. Note that we use `int` to convert `age`, `matchesPlayed` and `goalsScored` because those fields' values are `strings` in the html table and we want `integers` instead.  
*)

(**
Let's look at the first 5 rows of `FootballPlayersCsvRows` using the `Array.truncate` property. We don't need to use `FootballPlayersCsvRows.Rows` because the variable is already an array of csv rows.  
*)
FootballPlayersCsvRows[0..4]
(*** include-it ***)
(**
Now we have the data we need, but
using `FootballPlayersCsvRows.Length`
you can observe that there are more than 2500 rows! 
*)

FootballPlayersCsvRows.Length

(**
For our exercises we don't need such a heavy dataset,
so let's transform it to get the best players by position!
Steps: 
1. `Array.filter`, we filter the dataset by position, in order to remove odd positions
with few players as "GK,FW", "FW,DF" and "DF,FW".
2. `Array.sortByDescending`, we want the best players so we want to sort the data from highest goals scored
and highest matches played, to lowest goals scored and lowest matches played.
3. `Array.groupBy`, we want to get the top players by position we need to group the data by 
positions.
4. `Array.truncate`, we only want to keep the 25 best players by position (groups).
5. `Array.collect`, concatenate the top players by position and return the combined array.
*)

let top200PlayersByPosition =
    FootballPlayersCsvRows
    |> Array.filter(fun x -> x.Position <> "GK,FW" && x.Position <> "FW,DF" && x.Position <> "DF,FW" )
    |> Array.sortByDescending(fun x -> x.GoalsScored, x.MatchesPlayed)
    |> Array.groupBy(fun x -> x.Position)
    |> Array.collect(fun (_, y) ->  y |> Array.truncate 25)
    |> Array.sortByDescending(fun x -> x.GoalsScored, x.Player,x.Team)

(**
Rather than an "array of Csv rows", we want a "Csv file".  Here's how we do that.
*)
let FootballPlayersCsvFile = new FootballPlayersCsv(top200PlayersByPosition)

(**
Ok, let's write the file. Remember that `__SOURCE_DIRECTORY__` is a magic variable that points 
to whatever folder this script file (aka the source code file) is contained in.
So this will write a csv file in the current directory.
*)

let filePath = __SOURCE_DIRECTORY__ + "/" + "FootballPlayers.csv"
FootballPlayersCsvFile.Save(filePath)

(**
and if you want to read the data back in from the file to see that it works:
*)
let backIn = FootballPlayersCsv.Load(filePath)

backIn.Rows
|> Seq.take 5
|> Seq.iter (printfn "%A")
(*** include-output ***)

