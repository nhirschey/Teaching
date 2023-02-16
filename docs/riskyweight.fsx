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

#r "nuget: FSharp.Stats"
#r "nuget: Plotly.NET"
#r "nuget: Plotly.NET.Interactive"


open System

open FSharp.Stats
open FSharp.Stats.Distributions.ContinuousDistribution

open Plotly.NET

(**
From 1/1871-1/2023, the US market return was annualized
7.5% with a 14% standard deviation (Robert Schiller data). 
*)

let rnorm = normal 0.075 0.14


(**
# Portfolios of a risky asset and the risk-free asset

## Portfolio return
Suppose an investor has a portfolio comprised of a risky asset and the risk-free asset. The risky asset has return $r_i$ and the risk-free asset has return $r_f$. If $w$ is the weight that the investor puts on the risky asset, then the portfolio return is

$$ r_p = w r_i + (1-w) r_f = w (r_i - r_f) + r_f$$

If we put this in terms of excess returns, meaning returns in excess of the risk-free rate, then the portfolio excess return is

$$ r_p - r_f = w (r_i - r_f) $$.

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

$$ \sigma^2 = w_x^2 \sigma^2_x + w_y^2 \sigma^2_y + 2 w_x w_y cov(r_x,r_y), $$

where $w_x$ and $w_y$ are the weights in stocks $x$ and $y$, $r_x$ and $r_y$ are the stock returns, $\sigma^2$ is variance, and $cov(.,.)$ is covariance. 

If one asset is the risk free asset (borrowing a risk-free bond), then this asset does not vary, so the risk free variance and covariance terms are zero. Thus we are left with the result that if we leverage risky asset $x$ by borrowing or lending the risk-free asset, then our leveraged portfolio's standard deviation ($\sigma$) is

$$ \sigma^2 = w_x^2 \sigma^2_x \rightarrow \sigma = w_x \sigma_x$$ 

*)

(**
# Mean-Variance optimal weight

An investor with mean-variance preferences will try to maximize utility of the form

$$ u = \mu - \gamma \frac{\sigma^2}{2} $$

where $\mu$ is the expected portfolio return, $\sigma$ is the standard deviation of the portfolio, and $\gamma$ is the investor's coefficient of relative risk aversion. 

If our portfolio is comprised of a risky asset *x* and the risk-free asset, then this objective function can be written

$$ u = w_x (\mu_x ) + r_f - \gamma \frac{w_x^2 \sigma_x^2}{2} $$.

To maximize this with respect to $w_x$ we take the derivative and set it equal to zero:

$$ \frac{\partial u}{\partial w_x} = (\mu_x - r_f) - \gamma w_x \sigma_x^2 = 0 $$

This gives us the optimal weight

$$ w_x = \frac{\mu_x - r_f}{\gamma \sigma_x^2} $$.

It is common to define $\mu$ as the expected excess return, so that $\mu_x = r_x - r_f$. Then the optimal weight is

$$ w_x = \frac{\mu_x}{\gamma \sigma_x^2} $$.

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

The mean-variance optimal weight has a similar form to the optimal weight from the Kelly Criterion. The Kelly Criterion is often seen in industry as a formula for determining the optimal fraction of your wealth to bet on a risky asset. Originally developed for gambling, it can also be used for asset management.

The Kelly weight is the weight that maximizes the expected geometric growth rate of an investor's wealth. This weight is

$$ w = \frac{\mu}{\sigma^2} $$

where $\mu$ is the expected excess return of the risky asset and $\sigma^2$ is the variance of the risky asset.
*)

let kellyWeight mu sigma =
    mu/(sigma**2.0)

kellyWeight 0.075 0.14
(***include-fsi-output***)

(**
# Simulating results from different portfolio rules

We can simulate the results of different portfolio rules, using the mean-variance weight and varying $\gamma$. The Kelly Criterion corresponds to $\gamma = 1$.
*)

let seed = 99
Random.SetSampleGenerator(Random.RandThreadSafe(seed))

let careerLength = 30.0
let draws =
    [ for draw in [1 .. 10000] do 
        [ for year in [1.0 .. careerLength] do
            rnorm.Sample() ]]

(** Those are our returns. Let's calculate it for a particular gamma. *)


let gamma = 2.0
let ww = meanVarianceWeight 0.075 0.14 gamma

let myResult =
    [for draw in draws do
        let mutable wealth = 1.0
        for yearReturn in draw do 
            let newWealth = wealth*(1.0 + ww*yearReturn)
            wealth <- max 0.0 newWealth
        // This is the last wealth value per draw
        wealth ]

let myAvgWealth = myResult |> List.average
let myAvgGrowth =
    [ for wealth in myResult do
        let growthRate = wealth**(1.0/careerLength) - 1.0 
        growthRate ] 
    |> List.average

(** Same thing as a function. *)
type GammaResult = 
    { 
        Gamma : float 
        AvgWealth : float
        AvgGrowth : float 
    }
let gammaRecord gamma =
    let w = meanVarianceWeight 0.075 0.14 gamma
    let myResult =
        [for draw in draws do
            let mutable wealth = 1.0
            for yearReturn in draw do 
                let newWealth = wealth*(1.0 + w*yearReturn)
                wealth <- max 0.0 newWealth
            // This is the last wealth value per draw
            wealth ]
    let myAvgWealth = myResult |> List.average
    let myAvgGrowth =
        [ for wealth in myResult do
            let growthRate = wealth**(1.0/careerLength) - 1.0 
            growthRate ] 
        |> List.average
    { Gamma = gamma
      AvgWealth = myAvgWealth
      AvgGrowth = myAvgGrowth }

gammaRecord 3.0

(** Now do it for many gammas.*)
let ruleResults =
    [ for gamma in [0.25 .. 0.25 .. 3.0] do
        gammaRecord gamma ]

(** Now with daily compounding. *)

let gammaRecord2 gamma =
    let w = meanVarianceWeight (0.075/252.0) (0.14/sqrt(252.0)) gamma
    let myResult =
        [for draw in draws do
            let mutable wealth = 1.0
            for yearReturn in draw do 
                let newWealth = wealth*(1.0 + w*yearReturn/252.0)
                wealth <- max 0.0 newWealth
            // This is the last wealth value per draw
            wealth ]
    let myAvgWealth = myResult |> List.average
    let myAvgGrowth =
        [ for wealth in myResult do
            let growthRate = wealth**(1.0/careerLength) - 1.0 
            growthRate ] 
        |> List.average
    { Gamma = gamma
      AvgWealth = myAvgWealth
      AvgGrowth = myAvgGrowth }

gammaRecord 3.0

(** Now do it for many gammas.*)
let ruleResults2 =
    [ for gamma in [0.25 .. 0.25 .. 3.0] do
        gammaRecord2 gamma ]
