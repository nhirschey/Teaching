(**
---
title: Factors
category: Practice Quizzes
categoryindex: 1
index: 7
---
*)

(**
[![Binder](../images/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](../images/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](../images/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)
*)

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
On moodle, there is a set of investment lecture notes called "Finance Review". 
Please review the Lecture-08-APT.pdf document, including the appendix. 

In particular,

- What do we mean by a factor?
- The example showing how to hedge Barrick with GLD and SPY.
- Information ratios.

*)

(**
Imagine you have the below returns for IBM and SPY. 
IBM's factor beta on SPY and the (constant)
risk-free rate are given below too.
*)

type ReturnOb = { Time: int; Return : float }
let ibm =
    [| 
        { Time = 0; Return = 0.15 }
        { Time = 1; Return = 0.05 }
        { Time = 2; Return = 0.01 }
    |]

let spy =
    [| 
        { Time = 0; Return = 0.1 }
        { Time = 1; Return = 0.05 }
        { Time = 2; Return = -0.02 }
    |]    

let riskFreeRate = 0.001
let ibmBetaOnSpy = 1.2    


(**
## Question 1
What are the weights on the risk-free asset and
SPY in the portfolio that hedges IBM's exposure to
SPY? 

1. Report the weight on the risk-free asset as a value named `wRf` of type `float`. 
2. Report the weight on the SPY portfolio as a value named `wSpy` of type `float`.
*)

(*** include-it-raw:preDetails ***)
(*** define: WeightsHedgePort, define-output: WeightsHedgePort ***)

let wSpy = ibmBetaOnSpy
let wRf = 1.0-wSpy

(*** condition:html, include:WeightsHedgePort ***)
(*** condition:html, include-fsi-output:WeightsHedgePort ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
What are the returns for Times [0;1;2] on the portfolio 
that hedges IBM's factor exposure to SPY? 

1. Report results as a value named `hedgePortReturns` of type `ReturnOb array`.
*)

(*** include-it-raw:preDetails ***)
(*** define: ReturnsHedgePort, define-output: ReturnsHedgePort ***)

let hedgePortReturns =
    spy
    |> Array.map(fun spy ->
        { Time = spy.Time 
          Return = wSpy*spy.Return + wRf*riskFreeRate })

(*** condition:html, include:ReturnsHedgePort ***)
(*** condition:html, include-fsi-output:ReturnsHedgePort ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Call the portfolio that hedges IBM's factor exposure to SPY
the hedge portfolio. What is the hedge portfolio's factor beta
on SPY? 

1. Report the answer as a value named `hedgePortBetaOnSpy` of type float.
*)

(*** include-it-raw:preDetails ***)
(*** define: BetaSPYHedgePort, define-output: BetaSPYHedgePort ***)

// 3. Hedge portfolio's factor beta on SPY
// See the lecture notes for a fuller explanation:
//
// From the portfolio return equation, the
// hedge portfolio return is
// r_hedgePort = wRf * rf + wSpy * Spy
// =>
// r_hedgePort - rf = (wSpy + wRf - 1) * rf + wSpy*(Spy -rf)
// r_hedgePort - rf = wSpy*(Spy - rf)
// So the beta is
let hedgePortBetaOnSpy = ibmBetaOnSpy 

(*** condition:html, include:BetaSPYHedgePort ***)
(*** condition:html, include-fsi-output:BetaSPYHedgePort ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 4
What are the returns for Times [0;1;2] on the portfolio
that is long IBM and short the portfolio that hedges
IBM's factor exposre to SPY?

1. Report results as a value named `longShortPortReturns` of type `ReturnOb array`.
*)

(*** include-it-raw:preDetails ***)
(*** define: LongShortIBMRet, define-output: LongShortIBMRet ***)

let hedgePortMap =
    // using map because it's efficient for lookups.
    // you could use filter with such a small collection,
    // but we'll use a Map collection so that we
    // practice like it's the real thing.
    hedgePortReturns
    |> Array.map(fun x -> x.Time, x)
    |> Map.ofArray

let longShortPortReturns =
    ibm
    |> Array.choose(fun ibmOb ->
        // We're using Array.choose instead of Array.map
        // because we plan to return Some hedgeOb or None, and we
        // want to throw away all the None cases and return just
        // an array of the valid hedgeObs. 
        let matchingHedgeReturn = Map.tryFind ibmOb.Time hedgePortMap
        // in this example matchingHedgeReturn will always be Some hedgeOb.
        // But, we're using Map.tryFind and dealing with the possibility
        // that None could happen because it's good to practice
        // as if this were real code where bad things could happen.
        match matchingHedgeReturn with
        | None -> None // if there's no hedge return for ibmOb.Time return None
        | Some hedgeOb -> 
            // if there is a matching hedge return for ibmOb.Time return Some ReturnOb
            let longShort =  ibmOb.Return - hedgeOb.Return
            Some { Time = ibmOb.Time; Return = longShort })

(*** condition:html, include:LongShortIBMRet ***)
(*** condition:html, include-fsi-output:LongShortIBMRet ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 5
What is the alpha of IBM from the perspective of 
a factor model that uses SPY as the only risk factor? 

1. Report the result as a value named `alpha` of type float.
*)

(*** include-it-raw:preDetails ***)
(*** define: Alpha, define-output: Alpha ***)

let alpha = 
    longShortPortReturns 
    |> Array.averageBy(fun x -> x.Return)

(*** condition:html, include:Alpha ***)
(*** condition:html, include-fsi-output:Alpha ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 6
What is the information ratio of IBM from the perspective
of a factor model that uses SPY as the only risk factor?

1. Report the result as a value named `io` of type float.
*)

(*** include-it-raw:preDetails ***)
(*** define: IO, define-output: IO ***)

#r "nuget: FSharp.Stats"
open FSharp.Stats

let sdHedgeReturns =
    longShortPortReturns
    |> Array.map(fun x -> x.Return )
    |> Seq.stDev

// Intuitively, you can think of it similar
// to the sharpe ratio of the portfolio after
// hedging out the factor risk.

let io = alpha / sdHedgeReturns

(*** condition:html, include:IO ***)
(*** condition:html, include-fsi-output:IO ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.
