#r "nuget: FSharp.Stats"
#r "nuget: Plotly.NET"

open FSharp.Stats
open FSharp.Stats.Distributions.ContinuousDistribution

open Plotly.NET

(**
From 1/1871-1/2023, the US market return was annualized
7.5% with a 14% standard deviation (Robert Schiller data). 
*)

let rnorm = normal 0.075 0.14

for i = 0 to 5 do
    printfn $"{rnorm.Sample()}"

(***include-output***)

(** Put sampled values in a list. *)

let returns = [ for i = 1 to 1000 do rnorm.Sample() ]

(** Plot the distribution of returns. *)

let hist = Chart.Histogram(returns)

(***do-not-eval***)
hist

(***hide***)
hist |> GenericChart.toChartHTML
(***include-it-raw***)

let percentiles =
    [ for p in [0.0; 0.01; 0.05; 0.5; 0.95; 0.99; 1.0] do 
        let pctl = Quantile.compute p returns
        p, pctl ]
(***include-fsi-output***)
for (p, pctl) in percentiles do
    printfn $"percentile %.2f{p} is {pctl}"
(***include-output***)

(** A draw of 10-years of returns. *)

let draw10y = [ for i = 1 to 10 do rnorm.Sample() ]
(***include-fsi-output***)

(** Compute the cumulative product of the returns. *)
let mutable cumprod = 1.0
for r in draw10y do
    cumprod <- cumprod * (1.0 + r)
    printfn $"r={round 3 r}"
    printfn $"    cumprod={round 3 cumprod}"
(***include-output***)

(** Let's plot the cumulative return.*)
let draw10yCR = 
    let mutable year = 0
    let mutable cumprod = 1.0
    [ for r in draw10y do
        cumprod <- cumprod * (1.0 + r)
        year <- year + 1
        year, cumprod ]

(***include-fsi-output***)

(** Functional version of the cumulative return calculation. *)

((0, 1.0), draw10y)
||> List.scan (fun (year, cumprod) r -> 
    (year + 1, cumprod * (1.0 + r)))
|> List.tail
(***include-fsi-output***)

(** Plot the cumulative return. *)
let draw10yCrPlot = Chart.Line(draw10yCR)

(***do-not-eval***)
draw10yCrPlot

(***hide***)
draw10yCrPlot |> GenericChart.toChartHTML
(***include-it-raw***)


(**
> *Practice*: Sample 20 years of returns and plot the cumulative return.
*)

// Answer here

(** Let's simulate 1000 draws of 30 years of returns. *)

let draw1k = 
    [ for i = 1 to 1_000 do
        [ for y = 1 to 30 do rnorm.Sample() ] ]

let draw1kCR =
    [ for life in draw1k do
        let mutable accRet = 1.0
        let mutable year = 0
        [ for r in life do
            accRet <- accRet * (1.0 + r)
            year <- year+1
            year, accRet ] ]

(** Look at a few observtions from the first draw.*)
draw1kCR[0][0..5]

(***include-fsi-output***)

(** Plot the first draw *)
(***do-not-eval***)
Chart.Line(draw1kCR[0])
(***hide***)
Chart.Line(draw1kCR[0]) |> GenericChart.toChartHTML
(***include-it-raw***)

(** Plot the second draw *)
(***do-not-eval***)
Chart.Line(draw1kCR[1])
(***hide***)
Chart.Line(draw1kCR[1]) |> GenericChart.toChartHTML
(***include-it-raw***)

(** Plot the first and second together *)
(***do-not-eval***)
[ Chart.Line(draw1kCR[0])
  Chart.Line(draw1kCR[1]) ]
|> Chart.combine  

(***hide***)
[ Chart.Line(draw1kCR[0],Name = "first")
  Chart.Line(draw1kCR[1], Name = "second") ]
|> Chart.combine
|> GenericChart.toChartHTML
(***include-it-raw***)

(** Plot all 1000 draws together *)
(***do-not-eval***)
[ for x in draw1kCR do Chart.Line(x) ]
|> Chart.combine
|> Chart.withLayoutStyle(ShowLegend=false)
(***hide***)
[ for x in draw1kCR do Chart.Line(x) ]
|> Chart.combine
|> Chart.withLayoutStyle(ShowLegend=false)
|> GenericChart.toChartHTML
(***include-it-raw***)

(** A more useful version may be to plot the values at the end of the last year.*)
let terminalValues = 
    [ for x in draw1kCR do 
        let (yr, ret) = x[x.Length-1]
        ret ]

(***do-not-eval***)
terminalValues |> Chart.Histogram
(***hide***)
terminalValues
|> Chart.Histogram
|> GenericChart.toChartHTML
(***include-it-raw***)

(** What is the chance we lose money? *)
let nLoseMoney =
    terminalValues
    |> List.filter (fun x -> x < 1.0)
    |> List.length
    |> float

let chanceLose = nLoseMoney / (float terminalValues.Length)    
printfn $"chance lose money=%.3f{chanceLose}"
(***include-output***)

(** What is the chance we double our money? *)
let nDoubleMoney =
    terminalValues
    |> List.filter (fun x -> x >= 2.0)
    |> List.length
    |> float

let chanceDouble = nDoubleMoney / (float terminalValues.Length)    
printfn $"chance double money=%.3f{chanceDouble}"
(***include-output***)

(**
> **Practice**: What is the chance we earn at least a 10% compound rate of return per year?
*)

// Answer here

(**
Now let's say we have 1m EUR and we want to live off of 50k EUR per year.
What is the chance that we can do that?
*)

let expenses = 50_000.0
let initialWealth = 1_000_000.0
let wealthEvolution =
    [ for life in draw1k do
        let mutable wealth = initialWealth
        [ for r in life do
            // We'll take expenses out at the start of the year.
            if wealth > expenses then
                wealth <- (wealth - expenses) * (1.0 + r)
            else
                wealth <- 0.0
            wealth ] ]

let terminalWealth = [ for x in wealthEvolution do x[x.Length-1] ]

(** What's the chance that we don't have enough money?*)
let nBroke =
    terminalWealth
    |> List.filter (fun x -> x <= 0.0)
    |> List.length
    |> float

let chanceBroke = nBroke / (float terminalWealth.Length)    

printfn $"chance broke=%.3f{chanceBroke}"
(***include-output***)

(**
> **Practice**: What is our chance of going broke if the market's standard deviation is 20% per year?
*)

// Answer here

(**
> **Practice**: What is our chance of going broke if the market's
return is 5% per year?
*)

// Answer here