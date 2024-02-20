(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=quizzes/quiz-factors.ipynb)&emsp;
[![Script](../img/badge-script.svg)](/Teaching//quizzes/quiz-factors.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](/Teaching//quizzes/quiz-factors.ipynb)

On moodle, there is a set of investment lecture notes called "Finance Review".
Please review the Lecture-08-APT.pdf document, including the appendix.

In particular,

* What do we mean by a factor?

* The example showing how to hedge Barrick with GLD and SPY.

* Information ratios.

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

0 Report the weight on the risk-free asset as a value named `wRf` of type `float`.

1 Report the weight on the SPY portfolio as a value named `wSpy` of type `float`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val wSpy: float = 1.2
val wRf: float = -0.2
```

</details>
</span>
</p>
</div>

## Question 2

What are the returns for Times [0;1;2]() on the portfolio
that hedges IBM's factor exposure to SPY?

0 Report results as a value named `hedgePortReturns` of type `ReturnOb array`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val hedgePortReturns: ReturnOb array =
  [|{ Time = 0
      Return = 0.1198 }; { Time = 1
                           Return = 0.0598 }; { Time = 2
                                                Return = -0.0242 }|]
```

</details>
</span>
</p>
</div>

## Question 3

Call the portfolio that hedges IBM's factor exposure to SPY
the hedge portfolio. What is the hedge portfolio's factor beta
on SPY?

0 Report the answer as a value named `hedgePortBetaOnSpy` of type float.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val hedgePortBetaOnSpy: float = 1.2
```

</details>
</span>
</p>
</div>

## Question 4

What are the returns for Times [0;1;2]() on the portfolio
that is long IBM and short the portfolio that hedges
IBM's factor exposre to SPY?

0 Report results as a value named `longShortPortReturns` of type `ReturnOb array`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val hedgePortMap: Map<int,ReturnOb> =
  map
    [(0, { Time = 0
           Return = 0.1198 }); (1, { Time = 1
                                     Return = 0.0598 });
     (2, { Time = 2
           Return = -0.0242 })]
val longShortPortReturns: ReturnOb array =
  [|{ Time = 0
      Return = 0.0302 }; { Time = 1
                           Return = -0.0098 }; { Time = 2
                                                 Return = 0.0342 }|]
```

</details>
</span>
</p>
</div>

## Question 5

What is the alpha of IBM from the perspective of
a factor model that uses SPY as the only risk factor?

0 Report the result as a value named `alpha` of type float.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val alpha: float = 0.0182
```

</details>
</span>
</p>
</div>

## Question 6

What is the information ratio of IBM from the perspective
of a factor model that uses SPY as the only risk factor?

0 Report the result as a value named `io` of type float.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
[Loading C:\Users\runneradmin\.packagemanagement\nuget\Cache\ec63001b4149383af67dc5083312da4ecf34b844b655d861ca5bad281c016ede.fsx]
module FSI_0035.
       Ec63001b4149383af67dc5083312da4ecf34b844b655d861ca5bad281c016ede

val sdHedgeReturns: float = 0.02433105012
val io: float = 0.7480153922
```

</details>
</span>
</p>
</div>

*)

