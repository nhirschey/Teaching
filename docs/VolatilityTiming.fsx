(**

---
title: Volatility Timing
category: Lectures
categoryindex: 1
index: 7
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


# Volatility timing
We're going to look at how to manage portfolio volatility. Managing volatility is a fundamental risk-management task. You probably have some notion of the amount of volatility that you want in your portfolio, or that you feel comfortable bearing. Maybe you're ok with an annualized volatility of 15%, but 30% is too much and makes it hard for you to sleep at night. If that's the case, then when markets get volatile you might want to take action to reduce your portfolio's volatility. We'll discuss some strategies for predicting and managing volatility below.

We will focus on allocating between a risky asset and a risk-free asset as a way to manage volatility (i.e., two-fund seperation). We're taking the risky asset, such as the market portfolio of equities, as given. The essential idea is to put more weight on the risky asset when expected volatility is low and less weight on the risky asset when expected volatility is high. This is related to a portfolio construction strategy known as risk-parity. The managed volatility strategy that we consider below is based off work by [Barroso and Santa-Clara (2015)](https://scholar.google.com/scholar?hl=en&as_sdt=0%2C39&q=barroso+santa-clara+2015+jfe&btnG=), [Daniel and Moskowitz (2016)](https://scholar.google.com/scholar?hl=en&as_sdt=0%2C39&q=daniel+and+moskowitz+momentum+crash+2016+jfe&btnG=), and [Moreira and Muir (2017)](https://scholar.google.com/scholar?hl=en&as_sdt=0%2C39&q=moreira+and+muir+2017+jf+volatility+managed+portfolios&btnG=). The Moreira and Muir (2017) paper is probably the best place to start. Though the observation that a) predictable volatility and b) unpredictable returns implies predictable Sharpe ratios predates the aoove work.
*)

(**
## Acquiring Fama-French data
As a start, let's acquire a long daily time series on aggregate US market returns. We'll use the 3 factors [dataset](http://mba.tuck.dartmouth.edu/pages/faculty/ken.french/Data_Library/f-f_factors.html).

*)

#r "nuget: FSharp.Data"
#load "Common.fsx"
open System
open FSharp.Data
open Common
open Common.French

let ff3 = 
    French.getFF3 Frequency.Daily 
    |> Array.toList
        
(** 
    fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))

*)

ff3 |> List.take 5

(**
## Observing time-varying volatility
One thing that can help us manage volatility is the fact that volatility tends to be somewhat persistent. By this we mean that if our risky asset is volatile today, then it is likely to be volatile tomorrow. We can observe this by plotting monthly volatility as we do below. It also means that we can use recent past volatility to form estimates of future volatility.
*)

#r "nuget: FSharp.Stats"
#r "nuget: Plotly.NET, 2.0.0-preview.17"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.17"

open FSharp.Stats
open Plotly.NET

(** If returns are indepedent and identially distributed, then 
volatitlity (standard deviation) will grow at the square root of t.
So annual volatility is $\sqrt{252}\sigma_{daily}$.

$$ \sigma^2_{[1,T]} = var(T\tilde{r})=Tvar(\tilde{r})=T\sigma^2 \rightarrow$$
$$ \sigma_{[1,T]} = \sqrt{T}\sigma $$ 
*)
let annualizeDaily x = x * sqrt(252.0) * 100. 

let monthlyVol =
    ff3
    |> List.sortBy(fun x -> x.Date)
    |> List.groupBy(fun x -> x.Date.Year, x.Date.Month)
    |> List.map(fun (_ym, xs) -> 
        let dt = xs |> List.last |> fun x -> x.Date
        let annualizedVolPct = xs |> stDevBy(fun x -> x.MktRf) |> annualizeDaily
        dt, annualizedVolPct)

let volChart vols =
    let years = vols |> List.map(fun (dt:DateTime,_vol) -> dt.Year ) 
    let minYear = years |> List.min
    let maxYear = years |> List.max
    vols
    |> Chart.Column
    |> Chart.withMarkerStyle  (Outline = Line.init(Color = Color.fromKeyword ColorKeyword.Blue))  
    |> Chart.withXAxisStyle(TitleText = $"Time-varying Volatility ({minYear}-{maxYear})")
    |> Chart.withYAxisStyle(TitleText = "Annualized Volatility (%)")

let allVolsChart = volChart monthlyVol
let since2019VolChart = 
    monthlyVol 
    |> List.filter(fun (dt,_) -> dt >= DateTime(2019,1,1))
    |> volChart

(** the full time series *)
(***do-not-eval***)
allVolsChart
(***hide***)
allVolsChart |> GenericChart.toChartHTML
(*** include-it-raw ***)

(** since 2019 *)
(***do-not-eval***)
since2019VolChart
(***hide***)
since2019VolChart |> GenericChart.toChartHTML
(*** include-it-raw ***)


(**
## Effect of leverage on volatility
Recall the formula for variance of a portfolio consisting of stocks $x$ and $y$:

$$ \sigma^2 = w_x^2 \sigma^2_x + w_y^2 \sigma^2_y + 2 w_x w_y cov(r_x,r_y), $$

where $w_x$ and $w_y$ are the weights in stocks $x$ and $y$, $r_x$ and $r_y$ are the stock returns, $\sigma^2$ is variance, and $cov(.,.)$ is covariance. 

If one asset is the risk free asset (borrowing a risk-free bond), then this asset has no variance and the covariance term is 0.0. Thus we are left with the result that if we leverage asset $x$ by borrowing or lending the risk-free asset, then our leveraged portfolio's standard deviation ($\sigma$) is

$$ \sigma^2 = w_x^2 \sigma^2_x \rightarrow \sigma = w_x \sigma_x$$ 
*)

let leveragedVol (weight: float, vol: float) = weight * vol

/// We're doing leverage in terms of weight on the risky asset.
/// 1 = 100%, 1.25 = 125%, etc.
let exampleLeverages =
    [ 1.0; 1.5; 2.0 ]

let rollingVolSubset =
    monthlyVol
    |> List.filter(fun (dt, vol) -> dt > DateTime(2020,1,1))

// Let's take each of the leverages and map a function to them.
// The function takes a leverage as input, and the output is
// a tuple of (leverage, leveraged volatilities)
let exampleLeveragedVols =
    exampleLeverages
    |> List.map(fun leverage ->
        let leveragedVols =
            rollingVolSubset
            |> List.map(fun (dt, vol) -> 
                dt, leveragedVol(leverage, vol))
        leverage, leveragedVols)

let exampleLeveragesChart =
    exampleLeveragedVols
    |> List.map(fun (leverage, leveragedVols) ->
        Chart.Line(leveragedVols,Name= $"Levarage of {leverage}"))
    |> Chart.combine

(***do-not-eval***)
exampleLeveragesChart |> Chart.show

(***hide***)
exampleLeveragesChart |> GenericChart.toChartHTML
(***include-it-raw***)

(**
## Effect of leverage on returns
The effect of leverage on returns can be seen from the portfolio return equation,
$$ r_p = \Sigma^N_{i=1} w_i r_i,$$
where $r$ is return, $i$ indexes stocks, and $w$ is portfolio weights.

So if we borrow 50% of our starting equity by getting a risk-free loan, then we have
$$ r_{\text{levered}} = 150\% \times r_{\text{unlevevered}} - 50\% \times r_f$$

If we put in terms of excess returns,
$$ r_{\text{levered}} - r_f = 150\% \times (r_{\text{unlevevered}}-r_f) - 50\% \times (r_f-r_f)=150\% \times (r_{\text{unlevevered}}-r_f)$$

So, if we work in excess returns we can just multiply unlevered excess returns by the weight. 

Does this check out? Imagine that you have \$1 and you borrow \$1 for a net stake of \$2. Then you invest at an exess return of 15%. What are you left with?
*)

let invest = 1.0m
let borrow = 1.0m 
let ret = 0.15m
let result = (invest + borrow)*(1.0m+ret)-borrow
result = 1.0m + ret * (invest + borrow)/invest

(**
## Calculating cumulative returns
We can illustrate this with cumulative return plots. Let's first show a simple example for how we can calculate cumulative returns.

Imagine that you invest \$1 at a 10% return. What do you have after n years?
*)

[ 1.0 .. 5.0 ]
|> List.map(fun years -> 1.0*(1.1**years))

(** But what if we have the data like this?*)
[ for i = 1 to 5 do 0.1 ]

(** For this, we could use functions that operate by threading an accumulator through a collection: [fold](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-arraymodule.html#fold) and [scan](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-arraymodule.html#scan). The website [www.fsharpforfunandprofit.com](https://fsharpforfunandprofit.com/posts/list-module-functions/#14) has a good discussion of the various module functions. We could use `fold` to return a final cumulative return or `scan` to return intermediate and final cumulative returns.

Let's start with `scan` to see how it works. First, let's define a function to calculate returns:
*)

/// <summary>Function to calculate cumulative returns.
/// We are using the FV = PV*(1+r) formula.</summary>
/// <param name="pv">cumulative return thus far, like the PV in our FV formula</param>
/// <param name="ret">this period's return</param> 
let fv pv ret = pv*(1.0+ret)

// Now let's imagine we have a series of returns like
[0.1; -0.05; 0.3; -0.15]

// Our starting place is 1.0
1.0
(*** include-it ***)
// After the first period we're at
(fv 1.0 0.1)
(*** include-it ***)
// After two periods we're at
(fv (fv 1.0 0.1) -0.05)
(*** include-it ***)
// And so on
(fv (fv (fv 1.0 0.1) -0.05) 0.3)
(*** include-it ***)
(fv (fv (fv (fv 1.0 0.1) -0.05) 0.3) -0.15)
(*** include-it ***)

// now put these all in a list
[(fv 1.0 0.1)
 (fv (fv 1.0 0.1) -0.05)
 (fv (fv (fv 1.0 0.1) -0.05) 0.3)
 (fv (fv (fv (fv 1.0 0.1) -0.05) 0.3) -0.15)]
(*** include-it ***)

(*
We can do this all in one step using scan. Scan expects: to be given:

- An initial state (`1.0`).
- A collection to accumulate ('[...]`).
- A function to apply to do the accumulation (`fv`). The function nee
*)
(1.0, [0.1; -0.05; 0.3; -0.15])
||> List.scan fv
(*** include-it ***)

// Or if all we cared about was the final cumulative sum,
// we could just use fold
(1.0, [0.1; -0.05; 0.3; -0.15])
||> List.fold fv
(*** include-it ***)

(** One thing about `scan` and `fold` is that they expect the output of our function `fv` to have the same F# type as
the accumulator (in this case our pv term, but often called acc in functions). This is fine if we're
just working with floats. But sometimes we want to thread a float accumulator through our function but return a different `type` from our function. This is where `mapFold` is useful. It combines a `map` function and a `fold` function.

If we look at the definition of `mapFold` for arrays, we can see that we call it with `Array.mapFold mapping state array`, where 


```output
mapping : 'State -> 'T -> 'Result * 'State
The function to transform elements from the input array and accumulate the final value.

state : 'State
The initial state.

array : 'T[]
The input array.

Returns: 'Result[] * 'State
The array of transformed elements, and the final accumulated value.
```

But maybe it's easier to just think of it as a combination of a `map` transformation and a `fold` transformation.

In this example, we want to thread a float accumulator through a collection, but we're interested in returning a record with a stock ticker symbol and the cumulative return of the stock at that point.
*)


type MapFoldInputRecord = { Symbol: string; Return : float }
type MapFoldOutputRecord = { Symbol : string; CumulativeReturn : float }

// `map` transformation
let mapfun pv (input:MapFoldInputRecord) =
  let cumulativeReturn = pv*(1.0 + input.Return)
  { Symbol = input.Symbol; CumulativeReturn = cumulativeReturn }

mapfun 10.0 { Symbol = "AAPL"; Return = 0.3 }
(*** include-it ***)

// `fold` transformation
let foldfun pv (input:MapFoldInputRecord) =
  pv*(1.0 + input.Return)

foldfun 10.0 { Symbol = "AAPL"; Return = 0.3 }
(*** include-it ***)

(** 
Now we'll combine the `map` and `fold` transformations.

mapFold expects us to give it a function that operates on an accumulator
and the collection element. It expect us to return a tuple of the `map` result
and the `fold` result.

So let's create `mapfoldfun` that is a tuple of the `mapfun` and `foldfun` transformations.
*)
let mapfoldfun acc input = mapfun acc input, foldfun acc input

mapfoldfun 10.0 { Symbol = "AAPL"; Return = 0.3 }
(*** include-it ***)

// Now create a list of records that we want to get cumulative returns for
let mapFoldExampleRecords =
    [{ Symbol = "AAPL"; Return = 0.1}
     { Symbol = "AAPL"; Return = -0.05}
     { Symbol = "AAPL"; Return = 0.3}
     { Symbol = "AAPL"; Return = -0.15}]

// Now accumulate them with mapFold and our mapfoldfun.
// This will give us a tuple of cumulative returns as records of type `MapFoldOutputRecord` and the final cumulative return.
(1.0, mapFoldExampleRecords)
||> List.mapFold mapfoldfun
(*** include-it ***)

// We can discard the final cumulative return part by calling fst at the end.
(1.0, mapFoldExampleRecords)
||> List.mapFold mapfoldfun
|> fst
(*** include-it ***)
List.scan (fun acc (x: MapFoldInputRecord) -> {x with Return = (1.0+acc.Return) * (1.0+x.Return)-1.0}) mapFoldExampleRecords.Head mapFoldExampleRecords.Tail
// Same thing, but using an anonymous function instead of mapfoldfun
(1.0, mapFoldExampleRecords)
||> List.mapFold(fun acc input -> 
    let cumret = acc*(1.0+input.Return)
    { Symbol = input.Symbol; CumulativeReturn = cumret}, cumret)
|> fst

(** Now we know how to calculate cumulative returns on our real data.*)

let since2020 = 
    ff3 |> List.filter(fun x -> x.Date >= DateTime(2020,1,1))
    
let cumulativeReturnEx =
    let accReturn (priorObs: FF3Obs) (currentObs: FF3Obs) = 
        let outCumRet = (1.0 + priorObs.MktRf) * (1.0 + currentObs.MktRf) - 1.0
        { currentObs with MktRf = outCumRet }
    match since2020 with
    | [] -> []
    | h::t ->
        (h, t) ||> List.scan accReturn

let cumulativeReturnExPlot =
    cumulativeReturnEx
    |> List.map(fun x -> x.Date.Date, x.MktRf)
    |> Chart.Line

(***do-not-eval***)
cumulativeReturnExPlot |> Chart.show

(***hide***)
cumulativeReturnExPlot |> GenericChart.toChartHTML
(***include-it-raw***)

(** Let's try leverage with daily rebalancing (each day we take leverage of X).*)

let getLeveragedReturn leverage =
    match since2020 |> List.map (fun x -> x.Date, x.MktRf) with
    | [] -> []
    | h::t ->
        (h, t)
        ||> List.scan (fun (dt0, cr0) (dt1, r1) ->
            let lr = leverage * r1
            let cr1 = (1.0 + cr0)*(1.0 + lr) - 1.0
            dt1, cr1)

let exampleLeveragedReturnChart = 
    exampleLeverages
    |> Seq.map(fun lev ->
        let levReturn = getLeveragedReturn lev
        Chart.Line(levReturn, Name= $"Leverage of {lev}")) 
    |> Chart.combine

(***do-not-eval***)
exampleLeveragedReturnChart |> Chart.show

(***hide***)
exampleLeveragedReturnChart |> GenericChart.toChartHTML
(***include-it-raw***)

(**
## Predicting volatility
To manage or target volatility, we need to be able to predict volatility. A simple model would be to use past volatility to predict future volatility. How well does this work?

Let's start by creating a dataset that has the past 22 days as a training period and the 23rd day as a test period. We'll look at how volatility the past 22 days predicts volatility on the 23rd day.
*)

let dayWithTrailing =
    ff3 
    |> List.sortBy(fun x -> x.Date)
    |> List.windowed 23
    |> List.map(fun xs ->
        let train = xs |> List.take (xs.Length-1)
        let test = xs |> List.last
        train, test)

(*
One way to do this is to look at the correlation between volatilities. What is the correlation between volatility the past 22 days (training observations) and volatility on the 23rd day (test observation)? 

How do we measure volatility that last (23rd) day? You can't calculate a standard deviation of returns when there is one day. You need multiple days. But we can create a pseudo 1-day standard deviation by using the absolute value of the return that last day. 
*)

open Correlation

let trainVsTest =
    dayWithTrailing
    |> List.map(fun (train, test) ->
        let trainSd = train |> stDevBy(fun x -> x.MktRf)
        let testSd = abs(test.MktRf)
        annualizeDaily trainSd, annualizeDaily testSd)


// Plot a sample of 1_000 points
let trainVsTestChart =
    trainVsTest
    |> List.splitInto 1_000 // 1000 groups of train, test
    |> List.map Seq.head // get the observation at the start of each group
    |> Chart.Point
   

(***do-not-eval***)
trainVsTestChart |> Chart.show

(***hide***)
trainVsTestChart |> GenericChart.toChartHTML
(***include-it-raw***)

let trainPdSd, testPdSd = 
    trainVsTest 
    |> List.unzip
Seq.pearson trainPdSd testPdSd

(*** include-it ***)

(*
Another way is to try sorting days into 5 groups based on trailing 22-day volatility. 
Then we'll see if this sorts actual realized volatility. Think of this as splitting the points in the above chart into 5 groups along the x-axis and comparing the typical x-axis value to the typical y-axis value.
*)

dayWithTrailing
|> List.sortBy(fun (train, _test) -> train |> stDevBy(fun x -> x.MktRf))
|> List.splitInto 5
|> List.iter(fun xs ->
    
    let predicted = 
        xs 
        |> List.collect fst
        |> stDevBy (fun x -> x.MktRf)
        |> annualizeDaily
    let actual = 
        xs 
        |> List.map(fun (_train, test) -> test.MktRf)
        |> stDev 
        |> annualizeDaily
    printfn $"N: {xs.Length}, Predicted: %.1f{predicted}, Actual: %.1f{actual}")

(***include-output***)

(** Even with this very simple volatility model, we seem to do a reasonable job predicting volatility. We get a decent spread.*)

(**
## Targetting volatility
How can we target volatility? Recall our formula for a portfolio's standard deviation if we're allocating between the risk-free asset and a risky asset:
$$\sigma = w_x \sigma_x$$ 
If we choose 
$$w_x=\frac{\text{target volatility}}{\text{predicted volatility}}=\frac{\text{target}}{\hat{\sigma}_x}$$ 
then we get
$$\sigma = \frac{\text{target}\times \sigma_x}{\hat{\sigma}_x}$$ 

We're not going to predict volatility perfectly, but *if* we did then we'd have a portfolio that had a constant volatility equal to our target.

Let's see what happens if we try to target 15% annualized volatility.
*)

type VolPosition =
    { Date : DateTime 
      Return : float 
      Weight : float }

let targetted =
    dayWithTrailing
    |> List.map(fun (train,test) -> 
        let predicted = train |> stDevBy(fun x -> x.MktRf) |> annualizeDaily
        let w = (15.0/predicted)
        { Date = test.Date
          Return = test.MktRf * w 
          Weight = w })

let targettedSince2019 = 
    targetted
    |> List.filter(fun x -> x.Date >= DateTime(2019,1,1) )
    |> List.groupBy(fun x -> x.Date.Year, x.Date.Month)
    |> List.map(fun (_, xs) -> 
        xs |> List.map(fun x -> x.Date) |> List.max,
        xs |> stDevBy(fun x -> x.Return) |> annualizeDaily) 
    |> volChart 
    |> Chart.withTraceInfo(Name="Managed")
let rawSince2019 =
    monthlyVol
    |> List.filter(fun (dt,_) -> dt >= DateTime(2019,1,1))
    |> volChart
    |> Chart.withTraceInfo(Name="Unmangaged")

let weightsSince2019 = 
    targetted
    |> Seq.filter(fun x -> x.Date >= DateTime(2019,1,1) )
    |> Seq.groupBy(fun x -> x.Date.Year, x.Date.Month)
    |> Seq.map(fun (_, xs) -> 
        xs |> Seq.map(fun x -> x.Date) |> Seq.max,
        xs |> Seq.averageBy(fun x -> x.Weight))
    |> Chart.Line 
    |> Chart.withTraceInfo(Name="weight on the Market")

let volComparison = 
    [ targettedSince2019; rawSince2019]
    |> Chart.combine

let volComparisonWithWeights =
    [volComparison; weightsSince2019 ]
    |> Chart.SingleStack()


(***do-not-eval***)
volComparisonWithWeights |> Chart.show

(***hide***)
volComparisonWithWeights |> GenericChart.toChartHTML
(***include-it-raw***) 

(** We can see that in this example, 

- Volatility still moves around with our managed portfolio. We haven't targetted a 15\% volatility perfectly. 
- We do avoid some of the extreme volatilities from 2020, particularly in March and April.
- The weight we put on the market varies quite (implies lots of trading) and goes as high as 2 (lots of leverage). *)

(**
## Evaluating performance.

Let's now compare buy and hold to our managed volatility strategy. 

For the managed portfolio, we'll also impose the constraint that the investor cannot borrow more than 30% of their equity (i.e, max weight = 1.3).

We start by defining functions to calculate the weights. In the managed weights, we set the numerator so that we're targetting the full-sample standard deviation of the market.
*)

let leverageLimit = 1.3

let sampleStdDev = 
    dayWithTrailing 
    |> stDevBy(fun (train, test) -> test.MktRf)
    |> annualizeDaily

let buyHoldWeight predictedStdDev = 1.0

let inverseStdDevWeight predictedStdDev = 
    min (sampleStdDev / predictedStdDev) leverageLimit

let inverseStdDevNoLeverageLimit predictedStdDev = 
    sampleStdDev / predictedStdDev


let getManaged weightFun =
    dayWithTrailing
    |> List.map(fun (train,test) -> 
        let predicted = train |> stDevBy(fun x -> x.MktRf) |> annualizeDaily
        let w = weightFun predicted
        { Date = test.Date
          Return = test.MktRf * w 
          Weight = w })
    |> fun xs -> // Rescale to have same realized SD for
                 // more interpretable graphs. 
                 // Does not affect sharpe ratio
        let sd = xs |> stDevBy(fun x -> x.Return) |> annualizeDaily
        xs |> Seq.map(fun x -> { x with Return = x.Return * (sampleStdDev/sd)})

let accVolPortReturn (port: seq<VolPosition>) =
    let mapper acc (x : VolPosition) =
        let outAcc = acc * (1.0+x.Return)
        { x with Return = outAcc }, outAcc
    (1.0, port)
    ||> Seq.mapFold mapper 
    |> fst

let portChart name port  = 
    port 
    |> Seq.map(fun x -> x.Date, x.Return)
    |> Chart.Line
    |> Chart.withTraceInfo(Name=name)
    |> Chart.withYAxis (LayoutObjects.LinearAxis.init(AxisType = StyleParam.AxisType.Log))

    
let buyHoldMktPort = getManaged buyHoldWeight 
let managedMktPort = getManaged inverseStdDevWeight
let managedMktPortNoLimit = getManaged inverseStdDevNoLeverageLimit 

let bhVsManagedChart =
    Chart.combine(
        [ buyHoldMktPort |> accVolPortReturn |> (portChart "Buy-Hold")
          managedMktPort |> accVolPortReturn |> (portChart "Managed Vol")
          managedMktPortNoLimit |> accVolPortReturn |> (portChart "Managed Vol No Limit")
          ])

(***do-not-eval***)
bhVsManagedChart |> Chart.show

(***hide***)
bhVsManagedChart |> GenericChart.toChartHTML
(***include-it-raw***)

[ buyHoldMktPort, "Buy-Hold Mkt"
  managedMktPort, "Managed Vol Mkt"
  managedMktPortNoLimit, "Manage Vol No Limit"]
|> Seq.iter(fun (x, name) -> 
    let mu = 
        x 
        |> Seq.averageBy(fun x -> x.Return) 
        |> fun x -> round 2 (100.0*252.0*x)
    let sd = x |> Seq.stDevBy(fun x -> x.Return) |> annualizeDaily
    printfn $"Name: %25s{name} Mean: %.2f{mu} SD: %.2f{sd} Sharpe: %.3f{round 3 (mu/sd)}")
(***include-output***)

(**
## Things to consider

- What's a reasonable level of volatility to target?
- What's a reasonable level of leverage, and what should we think about when leveraging a portfolio that has low recent volatility?
- What happens if expected returns go up when volatility is high?
- How do we decide on what risky portfolio to invest in?
*)

              