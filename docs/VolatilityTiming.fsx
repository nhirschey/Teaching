(**

---
title: Volatility Timing
category: Lectures
categoryindex: 1
index: 5
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


# Volatility timing
We're going to look at how to manage portfolio volatility. Managing volatility is a fundamental risk-management task. You probably have some notion of the amount of volatility that you want in your portfolio, or that you feel comfortable bearing. Maybe you're ok with an annualized volatility of 15%, but 30% is too much and makes it hard for you to sleep at night. If that's the case, then when markets get volatile you might want to take action to reduce your portfolio's volatility. We'll discuss some strategies for predicting and managing volatility below.

We will focus on allocating between a risky asset and a risk-free asset as a way to manage volatility (i.e., two-fund separation). We're taking the risky asset, such as the market portfolio of equities, as given. The essential idea is to put more weight on the risky asset when expected volatility is low and less weight on the risky asset when expected volatility is high. This is related to a portfolio construction strategy known as risk-parity. The managed volatility strategy that we consider below is based off work by [Barroso and Santa-Clara (2015)](https://scholar.google.com/scholar?hl=en&as_sdt=0%2C39&q=barroso+santa-clara+2015+jfe&btnG=), [Daniel and Moskowitz (2016)](https://scholar.google.com/scholar?hl=en&as_sdt=0%2C39&q=daniel+and+moskowitz+momentum+crash+2016+jfe&btnG=), and [Moreira and Muir (2017)](https://scholar.google.com/scholar?hl=en&as_sdt=0%2C39&q=moreira+and+muir+2017+jf+volatility+managed+portfolios&btnG=). The Moreira and Muir (2017) paper is probably the best place to start. Though the observation that a) predictable volatility and b) unpredictable returns implies predictable Sharpe ratios predates the above work.
*)

(**
## Acquiring Fama-French data
As a start, let's acquire a long daily time series on aggregate US market returns. We'll use the 3 factors [dataset](http://mba.tuck.dartmouth.edu/pages/faculty/ken.french/Data_Library/f-f_factors.html).

*)

#r "nuget: FSharp.Data"
#r "nuget: NovaSBE.Finance"
open System
open FSharp.Data
open NovaSBE.Finance.French

let ff3 = 
    getFF3 Frequency.Daily 
    |> Array.toList
        
        
(***condition:FSX***)
#if FSX
fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))
#endif // FSX

ff3 |> List.take 5

(**
## Observing time-varying volatility
One thing that can help us manage volatility is the fact that volatility tends to be somewhat persistent. By this we mean that if our risky asset is volatile today, then it is likely to be volatile tomorrow. We can observe this by plotting monthly volatility as we do below. It also means that we can use recent past volatility to form estimates of future volatility.
*)

#r "nuget: FSharp.Stats"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"

open FSharp.Stats
open Plotly.NET

(** If returns are indepedent and identially distributed, then 
volatitlity (standard deviation) will grow at the square root of t.
So annual volatility is $\sqrt{252}\sigma_{daily}$.

$$ \sigma^2_{[1,T]} = var(T\tilde{r})=Tvar(\tilde{r})=T\sigma^2 \rightarrow$$
$$ \sigma_{[1,T]} = \sqrt{T}\sigma $$ 
*)
let annualizeDaily x = x * sqrt(252.0) * 100. 

(** First, group days by month.*)

let daysByMonth = ff3 |> List.groupBy (fun x -> x.Date.Year, x.Date.Month)
daysByMonth[0..2]

(** Look at a test month *)
let testMonth = daysByMonth[0]
testMonth

(** Split month from observations that month *)
let testMonthGroup, testMonthObs = testMonth
(** testMonthGroup *)
testMonthGroup

(** testMonthXS *)
testMonthObs

(** Test month return standard deviation. *)
testMonthObs
|> List.map(fun x -> x.MktRf)
|> stDev
|> annualizeDaily

(** Then, compute the standard deviation of returns for each month.*)
[ for ((year, month), xs) in daysByMonth do 
    let lastDayOb = xs |> List.sortBy (fun x -> x.Date) |> List.last 
    let lastDate = lastDayOb.Date
    let annualizedVolPct = 
        xs 
        |> List.map(fun x -> x.MktRf)
        |> stDev
        |> annualizeDaily
    lastDate, annualizedVolPct]

(** Or, all in one go *)
let monthlyVol =
    ff3
    |> List.groupBy(fun x -> x.Date.Year, x.Date.Month)
    |> List.map(fun (_ym, xs) -> 
        let lastOb = xs |> List.sortBy (fun x -> x.Date) |> List.last 
        let annualizedVolPct = xs |> stDevBy(fun x -> x.MktRf) |> annualizeDaily
        lastOb.Date, annualizedVolPct)

(** A function to plot volatilities.*)
let volChart (vols: list<DateTime * float>) =
    let years = vols |> List.map(fun (dt:DateTime,_vol) -> dt.Year ) 
    let minYear = years |> List.min
    let maxYear = years |> List.max
    vols
    |> Chart.Column
    |> Chart.withMarkerStyle  (Outline = Line.init(Color = Color.fromKeyword ColorKeyword.Blue))  
    |> Chart.withXAxisStyle(TitleText = $"Time-varying Volatility ({minYear}-{maxYear})")
    |> Chart.withYAxisStyle(TitleText = "Annualized Volatility (%)")

(** the full time series *)
let allVolsChart = volChart monthlyVol
(***do-not-eval***)
allVolsChart
(***hide***)
allVolsChart |> GenericChart.toChartHTML
(*** include-it-raw ***)

(** since 2019 *)
let since2019VolChart = 
    monthlyVol 
    |> List.filter(fun (dt,_) -> dt >= DateTime(2019,1,1))
    |> volChart
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

(***condition:fsx,do-not-eval***)
#if FSX
exampleLeveragesChart |> Chart.show
#endif // FSX
(***hide***)
exampleLeveragesChart |> GenericChart.toChartHTML
(***include-it-raw***)

(**
## Effect of leverage on returns
The effect of leverage on returns can be seen from the portfolio return equation,
$$ r_p = \Sigma^N_{i=1} w_i r_i,$$
where $r$ is return, $i$ indexes stocks, and $w$ is portfolio weights.

So if we borrow 50% of our starting equity by getting a risk-free loan, then we have
$$ r_{\text{levered}} = 150\% \times r_{\text{unlevered}} - 50\% \times r_f$$

If we put in terms of excess returns,
$$ r_{\text{levered}} - r_f = 150\% \times (r_{\text{unlevered}}-r_f) - 50\% \times (r_f-r_f)=150\% \times (r_{\text{unlevered}}-r_f)$$

So, if we work in excess returns we can just multiply unlevered excess returns by the weight. 

## Managed Portfolios.
*)

let since2020 = 
    ff3 |> List.filter(fun x -> x.Date >= DateTime(2020,1,1))

let cumulativeReturn (xs: list<DateTime * float>) =
    let xs = xs |> List.sortBy (fun (dt, r) -> dt)
    let mutable cr = 0.0
    [ for (dt, r) in xs do
        cr <- (1.0 + cr) * (1.0 + r) - 1.0
        dt, cr ]
        
let cumulativeReturnEx =
    since2020
    |> List.map (fun x -> x.Date, x.MktRf)
    |> cumulativeReturn

let cumulativeReturnExPlot =
    cumulativeReturnEx
    |> Chart.Line

(***condition:fsx,do-not-eval***)
#if FSX
cumulativeReturnExPlot |> Chart.show
#endif // FSX

(***condition:ipynb***)
#if IPYNB
cumulativeReturnExPlot
#endif // IPYNB

(***hide***)
cumulativeReturnExPlot |> GenericChart.toChartHTML
(***include-it-raw***)

(** Let's try leverage with daily rebalancing (each day we take leverage of X).*)

let getLeveragedReturn leverage =
    since2020 
    |> List.map (fun x -> x.Date, leverage * x.MktRf)
    |> cumulativeReturn

let exampleLeveragedReturnChart = 
    exampleLeverages
    |> List.map(fun lev ->
        let levReturn = getLeveragedReturn lev
        Chart.Line(levReturn, Name= $"Leverage of {lev}")) 
    |> Chart.combine

(***condition:fsx,do-not-eval***)
#if FSX
exampleLeveragedReturnChart |> Chart.show
#endif //FSX

(***condition:ipynb***)
#if IPYNB
exampleLeveragedReturnChart
#endif // IPYNB

(***hide***)
exampleLeveragedReturnChart |> GenericChart.toChartHTML
(***include-it-raw***)

(**
## Predicting volatility
To manage or target volatility, we need to be able to predict volatility. A simple model would be to use past sample volatility to predict future volatility. How well does this work?

Let's start by creating a dataset that has the past 22 days as a training period and the 23rd day as a test period. We'll look at how volatility the past 22 days predicts volatility on the 23rd day.
*)

type DaysWithTrailing = 
    { Train: list<FF3Obs> 
      Test: FF3Obs }
let dayWithTrailing =
    ff3 
    |> List.sortBy(fun x -> x.Date)
    |> List.windowed 23
    |> List.map(fun xs ->
        let train = xs |> List.take (xs.Length-1)
        let test = xs |> List.last
        { Train = train; Test = test })

(**
This is a list of records where `Train` is the 22-day training dataset and the `Test` is the 23rd day that we use as the test date.

*)
(** Look at the training data.*)
dayWithTrailing[0].Train

(** Look at the test data. *)
dayWithTrailing[0].Test

(**
One way to do this is to look at the correlation between volatilities. What is the correlation between volatility the past 22 days (training observations) and volatility on the 23rd day (test observation)? 

How do we measure volatility that last (23rd) day? You can't calculate a standard deviation of returns when there is one day. You need multiple days. But we can create a pseudo 1-day standard deviation by using the absolute value of the return that last day. 
*)

open Correlation

let trainVsTest =
    [ for x in dayWithTrailing do 
        let trainSd = x.Train |> stDevBy(fun x -> x.MktRf)
        let testSd = abs(x.Test.MktRf)
        annualizeDaily trainSd, annualizeDaily testSd ]


trainVsTest 
|> Seq.pearsonOfPairs

(*** include-it ***)

(**
Another way is to try sorting days into 20 groups based on trailing 22-day volatility. 
Then we'll see if this sorts actual realized volatility. Think of this as splitting days into 20 groups along the x-axis and comparing the typical x-axis value to the typical y-axis value.
*)

let twentyChunksByTrainVol =
    trainVsTest
    |> List.sortBy fst
    |> List.splitInto 20

(** A bin-scatterplot. *)

let binScatterData =
    [ for chunk in twentyChunksByTrainVol do 
        let avgTrain = chunk |> List.averageBy fst
        let avgTest = chunk |> List.averageBy snd
        avgTrain, avgTest ]

let binScatterPlot = 
    binScatterData
    |> Chart.Point

(***condition:fsx,do-not-eval***)
#if FSX
binScatterPlot |> Chart.show
#endif //FSX

(***condition:ipynb***)
#if IPYNB
binScatterPlot
#endif // IPYNB

(***hide***)
binScatterPlot |> GenericChart.toChartHTML
(***include-it-raw***)

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
    |> List.map(fun x -> 
        let predicted = x.Train |> stDevBy(fun x -> x.MktRf) |> annualizeDaily
        let w = (15.0/predicted)
        { Date = x.Test.Date
          Return = x.Test.MktRf * w 
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
    |> List.filter(fun x -> x.Date >= DateTime(2019,1,1) )
    |> List.groupBy(fun x -> x.Date.Year, x.Date.Month)
    |> List.map(fun (_, xs) -> 
        xs |> List.map(fun x -> x.Date) |> Seq.max,
        xs |> List.averageBy(fun x -> x.Weight))
    |> Chart.Line 
    |> Chart.withTraceInfo(Name="weight on the Market")

let volComparison = 
    [ targettedSince2019; rawSince2019]
    |> Chart.combine

let volComparisonWithWeights =
    [volComparison; weightsSince2019 ]
    |> Chart.SingleStack()


(***condition:fsx,do-not-eval***)
#if FSX
volComparisonWithWeights |> Chart.show
#endif //FSX

(***condition:ipynb***)
#if IPYNB
volComparisonWithWeights
#endif // IPYNB

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
    |> stDevBy(fun x -> x.Test.MktRf)
    |> annualizeDaily

let buyHoldWeight predictedStdDev = 1.0

let inverseStdDevWeight predictedStdDev = 
    min (sampleStdDev / predictedStdDev) leverageLimit

let inverseStdDevNoLeverageLimit predictedStdDev = 
    sampleStdDev / predictedStdDev

(** Some examples: *)
let weightExampleLineChart weightFun name =
    [ for sd in [ 5.0 .. 15.0 .. 65.0] do sd, weightFun sd ]
    |> Chart.Line
    |> Chart.withTraceInfo(Name=name)
    |> Chart.withXAxisStyle(TitleText="Predicted volatility")
    |> Chart.withYAxisStyle(TitleText="Portfolio weight")



weightExampleLineChart buyHoldWeight "Buy-Hold"

(** Combining: *)
let combinedWeightChart =
    [ weightExampleLineChart buyHoldWeight "Buy-Hold"
      weightExampleLineChart inverseStdDevWeight "Inverse StdDev"
      weightExampleLineChart inverseStdDevNoLeverageLimit "Inverse StdDev No Limit" ]
    |> Chart.combine

(***condition:fsx,do-not-eval***)
#if FSX
combinedWeightChart |> Chart.show
#endif //FSX

(***condition:ipynb***)
#if IPYNB
combinedWeightChart
#endif // IPYNB

(***hide***)
combinedWeightChart |> GenericChart.toChartHTML
(***include-it-raw***) 

(** A function to construct the backtest given a weighting function. *)

let getManaged weightFun =
    let managedReturn =
        dayWithTrailing
        |> List.map(fun x -> 
            let predicted = x.Train |> stDevBy(fun x -> x.MktRf) |> annualizeDaily
            let w = weightFun predicted
            { Date = x.Test.Date
              Return = x.Test.MktRf * w 
              Weight = w })
    
    // Rescale to have same realized SD for
    // more interpretable graphs. 
    // Does not affect sharpe ratio
    let sd = managedReturn |> stDevBy(fun x -> x.Return) |> annualizeDaily
    [ for x in managedReturn do 
        { x with Return = x.Return * (sampleStdDev/sd)}]

(** Function to accumulate the returns. *)
let accVolPortReturn (port: List<VolPosition>) =
    port
    |> List.map (fun x -> x.Date, x.Return)
    |> cumulativeReturn
    |> List.map (fun (dt, ret) ->
        // because we will do log plot later,
        // so return must be > 0. So make
        // 1.0 = 0% cumulative return
        dt, 1.0 + ret)

(** A function to make a chart. *)
let portChart name port  = 
    port 
    |> Chart.Line
    |> Chart.withTraceInfo(Name=name)
    |> Chart.withYAxis (LayoutObjects.LinearAxis.init(AxisType = StyleParam.AxisType.Log))

(** Now making the 3 strategies. *)    
let buyHoldMktPort = getManaged buyHoldWeight 
let managedMktPort = getManaged inverseStdDevWeight
let managedMktPortNoLimit = getManaged inverseStdDevNoLeverageLimit 

let bhVsManagedChart =
    Chart.combine(
        [ buyHoldMktPort |> accVolPortReturn |> (portChart "Buy-Hold")
          managedMktPort |> accVolPortReturn |> (portChart "Managed Vol")
          managedMktPortNoLimit |> accVolPortReturn |> (portChart "Managed Vol No Limit")
          ])

(***condition:fsx,do-not-eval***)
#if FSX
bhVsManagedChart |> Chart.show
#endif //FSX

(***condition:ipynb***)
#if IPYNB
bhVsManagedChart
#endif // IPYNB

(***hide***)
bhVsManagedChart |> GenericChart.toChartHTML
(***include-it-raw***)

let summarisePortfolio (portfolio, name) =
    let mu = 
        let mu = portfolio |> List.averageBy(fun x -> x.Return) 
        round 2 (100.0*252.0*mu)
    let sd = portfolio |> stDevBy(fun x -> x.Return) |> annualizeDaily
    printfn $"Name: %25s{name} Mean: %.2f{mu} SD: %.2f{sd} Sharpe: %.3f{round 3 (mu/sd)}"

summarisePortfolio (buyHoldMktPort, "Buy-Hold Mkt")

(** Now let's compare the performance of the managed portfolio to the buy and hold portfolio. *)
let listOfAllPortfolios =
    [ buyHoldMktPort, "Buy-Hold Mkt"
      managedMktPort, "Managed Vol Mkt"
      managedMktPortNoLimit, "Manage Vol No Limit"]

for port in listOfAllPortfolios do 
    summarisePortfolio port

(***include-output***)

(**
## Things to consider

- What's a reasonable level of volatility to target?
- What's a reasonable level of leverage, and what should we think about when leveraging a portfolio that has low recent volatility?
- What happens if expected returns go up when volatility is high?
- How do we decide on what risky portfolio to invest in?
*)

              