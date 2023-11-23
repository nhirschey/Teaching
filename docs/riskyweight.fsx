(**
---
title: Risky Weights
category: Lectures
categoryindex: 1
index: 4
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

# Choosing the risky asset weight in your portfolio

*)

#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"


open System

open FSharp.Stats

open Plotly.NET

(**
# Portfolios of a risky asset and the risk-free asset

## Portfolio return
Suppose an investor has a portfolio comprised of a risky asset and the risk-free asset. The risky asset has return $r_i$ and the risk-free asset has return $r_f$. If $w$ is the weight that the investor puts on the risky asset, then the portfolio return is

$$ 
r_p = w r_i + (1-w) r_f = w (r_i - r_f) + r_f
$$

If we put this in terms of excess returns, meaning returns in excess of the risk-free rate, then the portfolio excess return is

$$ 
r_p - r_f = w (r_i - r_f) 
$$.

So if we work in excess returns the excess return of the portfolio is the risky weight times the risky asset's excess return.

When $w < 0$ we are short the risky asset and long the risk-free asset, when $0 < w < 1$ we are long both the risky and risk-free asset, and when $w > 1$ we are short the risk-free asset and long the risky asset.

Does this check out? Imagine that you have \$1 and you borrow \$1 at the risk-free rate for a net stake of \$2. Assuming that you invest in an asset with a return of 15% and the risk-free rate is 4%, what are you left with?
*)

let invest = 1.0m
let borrow = 1.0m 
let ret = 0.15m
let rf = 0.04m
let result = (invest + borrow)*(1.0m+ret)-borrow*(1.0m+rf)
printfn $"You are left with {result}"
(***include-output***)

(** If we calculate this using weights ... *)
let w = (invest + borrow)/invest
let result2 = 1m + rf + w*(ret - rf)
printfn $"You are left with {result2}"
(***include-output***)

(** ... we get the same result. *)
result = result2
(***include-fsi-output***)

(**
## Portfolio variance
Recall the formula for variance of a portfolio consisting of stocks $x$ and $y$:

$$ 
\sigma^2 = w_x^2 \sigma^2_x + w_y^2 \sigma^2_y + 2 w_x w_y cov(r_x,r_y), 
$$

where $w_x$ and $w_y$ are the weights in stocks $x$ and $y$, $r_x$ and $r_y$ are the stock returns, $\sigma^2$ is variance, and $cov(.,.)$ is covariance. 

If one asset is the risk free asset (borrowing a risk-free bond), then this asset does not vary, so the risk free variance and covariance terms are zero. Thus we are left with the result that if we leverage risky asset $x$ by borrowing or lending the risk-free asset, then our leveraged portfolio's standard deviation ($\sigma$) is

$$ 
\sigma^2 = w_x^2 \sigma^2_x \rightarrow \sigma = w_x \sigma_x. 
$$ 

*)

(**
# Mean-Variance optimal weight

An investor with mean-variance preferences will try to maximize utility of the form

$$ 
u = \mu - \gamma \frac{\sigma^2}{2} 
$$

where $\mu$ is the expected portfolio return, $\sigma$ is the standard deviation of the portfolio, and $\gamma$ is the investor's coefficient of relative risk aversion. 

If our portfolio is comprised of a risky asset *x* and the risk-free asset, then this objective function can be written

$$ 
u = w_x (\mu_x - r_f) + r_f - \gamma \frac{w_x^2 \sigma_x^2}{2}. 
$$

To maximize this with respect to $w_x$ we take the derivative and set it equal to zero:

$$ 
\frac{\partial u}{\partial w_x} = (\mu_x - r_f) - \gamma w_x \sigma_x^2 = 0 
$$

This gives us the optimal weight

$$ 
w_x = \frac{\mu_x - r_f}{\gamma \sigma_x^2}. 
$$

It is common to define $\mu$ as the expected excess return, so that $\mu_x = r_x - r_f$. Then the optimal weight is

$$ 
w_x = \frac{\mu_x}{\gamma \sigma_x^2}. 
$$

Typical values for $\gamma$ range from two to five. Higher values for $\gamma$ indicate higher risk aversion.

Examples
*)
let meanVarianceWeight mu sigma gamma =
    mu/(gamma*sigma**2.0)

[ for gamma in [1.0 .. 5.0 ] do 
    {| Gamma = gamma
       RiskyWeight = meanVarianceWeight 0.075 0.14 gamma |} ]

(***include-fsi-output***)

(**
# Kelly Criterion

The mean-variance optimal weight has a similar form to the optimal weight from the Kelly Criterion (Kelly, 1956). The Kelly Criterion is the weight that maximizes the expected geometric growth rate of an investor's wealth or, equivalently, the expected value of log wealth. It is often seen in industry as a formula for determining the optimal fraction of your wealth to bet on a risky asset. Originally developed for gambling, it can also be used for asset management.

The objective is to maximize wealth, which grows as

$$
(1+w r_1)(1+w r_2)...(1+w r_T)=\prod_{i=1}^T (1 + w_i r_i)
$$

It can be shown that the expected long-term growth rate is

$$
 \approx w \mu - \frac{1}{2} w^2 \sigma^2
$$

This is maximized when

$$ 
w = \frac{\mu}{\sigma^2} 
$$

where $\mu$ is the expected excess return of the risky asset and $\sigma^2$ is the variance of the risky asset.
*)

let kellyWeight mu sigma =
    mu/(sigma**2.0)

kellyWeight 0.075 0.14
(***include-fsi-output***)

(**
> Practice: Use a loop to calculate the Kelly Criterion weight for a range of $\mu$ and $\sigma$ values. 
>
> First, hold $\sigma$ fixed and vary $\mu$ from 0.05 to 0.10 by 0.01. What happens to the optimal weight as $\mu$ increases? 

    // Answer here

> Second, hold $\mu$ fixed and vary $\sigma$ from 0.1 to 0.2 by 0.01. What happens to the optimal weight as $\sigma$ increases?

    // Answer here

*)

(**
# Simulating results from different portfolio rules

We can simulate the results of different portfolio rules, using the mean-variance weight and varying $\gamma$. The Kelly Criterion corresponds to $\gamma = 1$.
*)

let seed = 99
Random.SetSampleGenerator(Random.RandThreadSafe(seed))

// Let's start with this sample of returns
let rnorm1 = Distributions.Continuous.Normal.Init 0.01 1.0

let careerLength = 30
let draws =
    [ for draw in [1 .. 100] do 
        [ for year in [1 .. 10_000] do
            rnorm1.Sample() ]]

(** Those are our returns. Let's calculate it for a particular gamma. *)


let gamma = 2.0
let ww = meanVarianceWeight 0.01 1.0 gamma
(***include-fsi-result***)

let investmentResult (riskyWeight: float) (returns: float list)  =
    let mutable wealth = 1.0
    for yearReturn in returns do 
        let newWealth = wealth*(1.0 + riskyWeight*yearReturn)
        // If we go bankrupt, we are done
        wealth <- max 0.0 newWealth
    // This is the last wealth value
    wealth

investmentResult  1.0 [0.1; 0.1; 0.1]
(***include-it***)

investmentResult  ww [0.1; 0.1; 0.1]
(***include-it***)

(** Now do it for all our draws.*)
let myResult =
    [ for draw in draws do investmentResult  ww draw]

myResult
(***include-it***)

(** Now let's calculate some statistics. *)

type SimulationSummary =
    { Gamma: float 
      AvgLogWealth: float 
      AvgWealth: float
      AvgGeometricGrowth: float 
      MinWealth: float
      FractionBankrupt: float
      FractionLoseMoney: float }

let calcSummary (gamma: float) (nPeriods: int) (wealths: list<float>) =
    { Gamma = gamma
      AvgLogWealth = wealths |> List.map log |> List.average
      AvgWealth = wealths |> List.average
      AvgGeometricGrowth = 
        wealths 
        |> List.map (fun w -> w**(1.0/float nPeriods) - 1.0) 
        |> List.average
      MinWealth = wealths |> List.min
      FractionBankrupt = 
        let bankrupts = wealths |> List.filter (fun w -> w = 0.0)
        float bankrupts.Length / float wealths.Length
      FractionLoseMoney = 
        let lostMoney = wealths |> List.filter (fun w -> w < 1.0)
        float lostMoney.Length / float wealths.Length  
      }
(** *)
calcSummary gamma careerLength myResult
(***include-fsi-output***)

(** Now a function to do it for a particular gamma *)

let gammaRecord (mu: float) (sigma: float) (draws: list<list<float>>) (gamma: float) =
    let w = meanVarianceWeight mu sigma gamma
    let myResult = draws |> List.map (investmentResult w)
    let careerLength = draws[0] |> List.length
    calcSummary gamma careerLength myResult

gammaRecord 0.01 1.0 draws 3.0
(***include-it***)

(** Now do it for many gammas.*)
let ruleResults =
    let gammas = [0.5 .. 0.1 .. 1.0] @ [1.25 .. 0.25 .. 2.0]
    [ for gamma in gammas do
        gammaRecord 0.01 1.0 draws gamma ]

let veryLongRunChart =
    ruleResults
    |> List.map (fun x -> 
        let kellyFraction = 1.0 / x.Gamma
        kellyFraction, x.AvgGeometricGrowth)
    |> Chart.Line
    |> Chart.withXAxisStyle("Kelly fraction")
    |> Chart.withYAxisStyle("Average geometric growth")
(***do-not-eval***)
veryLongRunChart
(***hide***)
veryLongRunChart |> GenericChart.toChartHTML
(***include-it-raw***)

(**
A couple takeaways:

1. Even an investor who is not very risk averse should not bet more than 1x the Kelly bet. This is particularly true if we take into account uncertainty about expected returns and variances.

2. It takes a very long time to converge to the kelly result.

*)

(** Now let's try with representative returns, monthly rebalancing. *)

let monthlyMu = 0.075/12.0
let monthlySigma = 0.14/sqrt 12.0
let rnorm2 = Distributions.Continuous.Normal.Init monthlyMu monthlySigma
let investmentCareer = 30
let drawsRealistic =
    [ for draw in [1 .. 1000] do 
        [ for year in [1 .. investmentCareer*12] do
            rnorm2.Sample() ]]

let realisticResults =
    let gammas = [0.75 .. 0.25 .. 3.0]
    [ for gamma in gammas do
        gammaRecord monthlyMu monthlySigma drawsRealistic gamma ]

(** The results:*)
realisticResults
(***include-fsi-output***)
