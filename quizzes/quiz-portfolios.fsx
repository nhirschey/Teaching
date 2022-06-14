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
Real: 00:00:00.021, CPU: 00:00:00.031, GC gen0: 0, gen1: 0, gen2: 0
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

Imagine you have this array

*)
open System
type ArExample = { Date : DateTime; Value: float}
let arr = [|{ Date = DateTime(1990,1,1); Value = 1.25}
            { Date = DateTime(1990,1,2); Value = 2.25}
            { Date = DateTime(1991,1,1); Value = 3.25} |]
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
input.fsx (2,1)-(46,43) interactive warning Accessing the internal type, method or field 'Date@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (2,1)-(46,43) interactive warning Accessing the internal type, method or field 'Value@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
Real: 00:00:00.005, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
val it: {| Group: {| Month: int; Year: int |}; Value: float |}[] =
  [|{ Group = { Month = 1
                Year = 1990 }
      Value = 1.25 }; { Group = { Month = 1
                                  Year = 1991 }
                        Value = 3.25 }|]
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
let bondPos = { Id = "bond"; Weight = 0.75; Return = 0.05}
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
input.fsx (5,1)-(16,37) interactive warning Accessing the internal type, method or field 'Weight@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (5,1)-(16,37) interactive warning Accessing the internal type, method or field 'Return@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
Real: 00:00:00.020, CPU: 00:00:00.015, GC gen0: 0, gen1: 0, gen2: 0
val stockAndBondPort: float = 0.0625
val weightXreturn: float[] = [|0.025; 0.0375|]
val stockAndBondPort2: float = 0.0625
val it: bool = true
```

</details>
</span>
</p>
</div>

## Question 2

Imagine that you have the following positions in your portfolio.
For each position you have a weight and a return.
What is the return of the entire portfolio?

*)
let positions =
    [|{ Id = "stock"; Weight = 0.25; Return = 0.12 }
      { Id = "bond"; Weight = 0.25; Return = 0.22 }
      { Id = "real-estate"; Weight = 0.5; Return = -0.15 } |](* output: 

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>


input.fsx (1,1)-(4,17) interactive warning Accessing the internal type, method or field 'Weight@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(4,17) interactive warning Accessing the internal type, method or field 'Return@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
Real: 00:00:00.048, CPU: 00:00:00.046, GC gen0: 1, gen1: 0, gen2: 0
val threeAssetPortfolioReturn: float = 0.01


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
    [|{ Id = "stock"; Weight = 0.25; Return = 0.12 }
      { Id = "bond"; Weight = -0.25; Return = 0.22 }
      { Id = "real-estate"; Weight = 1.0; Return = -0.15 } |](* output: 

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>


input.fsx (1,1)-(4,17) interactive warning Accessing the internal type, method or field 'Weight@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(4,17) interactive warning Accessing the internal type, method or field 'Return@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
Real: 00:00:00.040, CPU: 00:00:00.046, GC gen0: 0, gen1: 0, gen2: 0
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
let rets = [| 0.1; -0.4; 0.2; 0.15; -0.03 |]
//Note that the units are such that 0.1 is 10%.(* output: 

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>


[Loading C:\Users\runneradmin\AppData\Local\Temp\3116--c8101fe4-dc1c-4738-ae6d-b479b3055840\Project.fsproj.fsx]
Real: 00:00:00.000, CPU: 00:00:00.000, GC gen0: 0, gen1: 0, gen2: 0
namespace FSI_10007.Project

Real: 00:00:00.022, CPU: 00:00:00.015, GC gen0: 0, gen1: 0, gen2: 0
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
let rets = [| 0.1; -0.4; 0.2; 0.15; -0.03 |]
//Note that the units are such that 0.1 is 10%.
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.022, CPU: 00:00:00.015, GC gen0: 0, gen1: 0, gen2: 0
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
let rets = [| 0.1; -0.4; 0.2; 0.15; -0.03 |]
//Note that the units are such that 0.1 is 10%.
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
Real: 00:00:00.021, CPU: 00:00:00.031, GC gen0: 0, gen1: 0, gen2: 0
val annualizedSharpeFromDaily: float = 0.2629143395
val dailyAvgRet: float = 0.004
val dailyStDevRet: float = 0.241516045
val annualizedSharpeFromDaily2: float = 0.2629143395
```

</details>
</span>
</p>
</div>

*)

