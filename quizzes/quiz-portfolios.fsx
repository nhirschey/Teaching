(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=quizzes/quiz-portfolios.ipynb)&emsp;
[![Script](../img/badge-script.svg)](/Teaching//quizzes/quiz-portfolios.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](/Teaching//quizzes/quiz-portfolios.ipynb)

# Some good things to reference

[Anonymous Records](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/anonymous-records#syntax). You can read the above link for details, but the point of these is quite simple.

Records have been our main type for holding data for an observation. We've typically defined these ahead of time with a name before using them. This is good for important types that you will use frequently.

If you're using a particular record in only a few lines of code, then it can feel cumbersome to define the type beforehand. Anonymous records are a good solution in these circumstances. They are records that you can essentially use like regular records that we've been using, but you don't have to define the name of the record ahead of time.

I rarely use anonymous records, but you might find them useful for exploratory data manipulation. They're also kind of nice for these short problems because I don't need to define a record for each problem.

# Anonymous records

## Question 1

0 Create a **record** named `ExampleRec` that has an `X` field of type int and a `Y` field of type int. Create an example `ExampleRec` and assign it to a value named `r`.

1 Create an **anonymous record** that has an `X` field of type int and a `Y` field of type int. Create an example of the anonymous record and assign it to a value named `ar`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type ExampleRec =
  {
    X: int
    Y: int
  }
val r: ExampleRec = { X = 1
                      Y = 2 }
val ar: {| X: int; Y: int |} = { X = 1
                                 Y = 2 }
```

</details>
</span>
</p>
</div>

## Question 2

Imagine you have this list

*)
open System
type ArExample = { Date : DateTime; Value: float}
let arr = [ { Date = DateTime(1990,1,1); Value = 1.25}
            { Date = DateTime(1990,1,2); Value = 2.25}
            { Date = DateTime(1991,1,1); Value = 3.25} ]
(**
0 Group the observations by a tuple of `(year,month)` and find the 
minimum value for each group. Report the result as a tuple of the group
and the minimum value [so it will be `((year, month), minValue)`]().

1 Now, the same thing with anonymous records.
Group the observations by an Anonymous Record `{| Year = year; Month= month|}` and find the
minimum value for each group. Report the result as an Anonymous record with a Group
field for the group and a value field for the minimum value [so it will be
`{| Group = {| Year = year; Month= month|}; Value = minValue |}`]().

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: {| Group: {| Month: int; Year: int |}; Value: float |} list =
  [{ Group = { Month = 1
               Year = 1990 }
     Value = 1.25 }; { Group = { Month = 1
                                 Year = 1991 }
                       Value = 3.25 }]
```

</details>
</span>
</p>
</div>

# Portfolio Returns

## Question 1

Imagine that you have the following positions in your portfolio.
For each position you have a weight and a return.
What is the return of the entire portfolio?

*)
type PortReturnPos = { Id: string;  Weight: float; Return: float}
let stockPos = { Id = "stock"; Weight = 0.25; Return = 0.1 }
let bondPos = { Id = "bond"; Weight = 0.75; Return = 0.05}(* output: 

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>


val stockAndBondPort: float = 0.0625
val weightXreturn: float list = [0.025; 0.0375]
val stockAndBondPort2: float = 0.0625
val it: bool = true


</details>
</span>
</p>
</div>
*)
(**
## Question 2

Imagine that you have the following positions in your portfolio.
For each position you have a weight and a return.
What is the return of the entire portfolio?

*)
let positions =
    [ { Id = "stock"; Weight = 0.25; Return = 0.12 }
      { Id = "bond"; Weight = 0.25; Return = 0.22 }
      { Id = "real-estate"; Weight = 0.5; Return = -0.15 } ](* output: 

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>


val threeAssetPortfolioReturn: float = 0.01
val threeAssetPortfolioReturn2: float = 0.01
val threeAssetPortfolioReturn3: float = 0.01


</details>
</span>
</p>
</div>
*)
(**
## Question 3

Imagine that you have the following positions in your portfolio.
For each position you have a weight and a return.
What is the return of the entire portfolio?

*)
let positionsWithShort =
    [ { Id = "stock"; Weight = 0.25; Return = 0.12 }
      { Id = "bond"; Weight = -0.25; Return = 0.22 }
      { Id = "real-estate"; Weight = 1.0; Return = -0.15 } ](* output: 

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>


val positionsWithShortReturn: float = -0.175


</details>
</span>
</p>
</div>
*)
(**
# Sharpe Ratios

## Question 1

Imagine that you have the following array of **annual** returns in
excess of the risk-free rate. What is the **annualized** Sharpe ratio?

*)
let rets = [ 0.1; -0.4; 0.2; 0.15; -0.03 ]
//Note that the units are such that 0.1 is 10%.(* output: 

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>


[Loading C:\Users\runneradmin\.packagemanagement\nuget\Cache\ec63001b4149383af67dc5083312da4ecf34b844b655d861ca5bad281c016ede.fsx]
module FSI_0051.
       Ec63001b4149383af67dc5083312da4ecf34b844b655d861ca5bad281c016ede

val retsAvg: float = 0.004
val retsStdDev: float = 0.241516045
val retsSharpeRatio: float = 0.01656204663


</details>
</span>
</p>
</div>
*)
(**
## Question 2

Imagine that you have the following array of **monthly** returns in
excess of the risk-free rate. What is the **annualized** Sharpe ratio?

```fsharp
let rets = [ 0.1; -0.4; 0.2; 0.15; -0.03 ]
//Note that the units are such that 0.1 is 10%.
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val monthlyRetsAnnualizedAvg: float = 0.048
val monthlyRetsAnnualizedAvg2: float = 0.048
val monthlyRetsAvg: float = 0.004
val monthlyRetsAnnualizedAvg3: float = 0.048
val monthlyRetsAnnualizedSd: float = 0.8366361216
val monthlyRetsSd: float = 0.241516045
val monthlyRetsAnnualizedSd2: float = 0.8366361216
val annualizedSharpeFromMonthly: float = 0.05737261249
val annualizedSharpeFromMonthly2: float = 0.05737261249
val it: bool = true
```

</details>
</span>
</p>
</div>

## Question 3

Imagine that you have the following array of **daily** returns in
excess of the risk-free rate. What is the **annualized** Sharpe ratio?

```fsharp
let rets = [ 0.1; -0.4; 0.2; 0.15; -0.03 ]
//Note that the units are such that 0.1 is 10%.
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val annualizedSharpeFromDaily: float = 0.2629143395
val dailyAvgRet: float = 0.004
val dailyStDevRet: float = 0.241516045
val annualizedSharpeFromDaily2: float = 0.2629143395
```

</details>
</span>
</p>
</div>

# Risky weights

## Question 1

Assume that you are a mean-variance investor where your
utility function has the form

$$U = \mu - \frac{\gamma}{2}\sigma^2$$

You plan to allocate only between a risky asset and the risk-free asset.
Your risk aversion parameter $\gamma=3$. For the risky asset, $\mu=0.1$ and $\sigma=0.2$. What is your optimal weight in the risky asset?

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val mu: float = 0.1
val sigma: float = 0.2
val gamma: float = 3.0
val riskyWeight: float = 0.8333333333
```

</details>
</span>
</p>
</div>

*)

