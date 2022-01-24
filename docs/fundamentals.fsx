(**
---
title: Fundamentals
category: Lectures
categoryindex: 0
index: 1
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

# Fundamentals
A good place to start is to define a one-period return calculation.

Objectives:

- [Interactive programming](#Interactive-programming)
- [How to calculate returns](#Calculating-returns).
- [Working with data.](#Working-with-data)
- [How to calculate return volatility](#Volatility)
*)

(**
## Interactive programming

We are going to focus on interactive programming. This is the most productive (and most common) type of analytic programming. In constrast to compiled programs (e.g, C, C++, Fortran, Java), interactive programs:

- Allow rapid iterative development.
- You can quickly quickly write and rewrite sections of code, evaluating the output, without having to rerun the entire program.
- This is especially useful for financial analysis, because we often evaluate large datasets that take a long time to process.

Interactive programming typically involves a [REPL](https://en.wikipedia.org/wiki/Read%E2%80%93eval%E2%80%93print_loop) (Read, Evaluate, Print, Loop). It is common for scripting langauges such as R, Python, Julia, Ruby, and Perl.

### The terminal

The most basic way that you can run interactive code is at the command line using an interpreter. We can start the F# interactive interpreter by opening a terminal (e.g., terminal.app, cmd, powershell) and running `dotnet fsi`.

Once fsi is open, we can type a code snippet in the prompt followed by ";;" to terminate it and it will run.

![fsi](img/fsi.png)


It is fine to run code this way, but we can do better using an IDE (Integrated development environment) that incorportes syntax highlighting, intellisense tooltips, and execution.
*)


(**
## Calculating returns

### Basic calculations in fsi

Let's assume that you have \$120.00 today and that you had \$100.00 a year ago. Your annual return is then:

*)

(120.0 / 100.0) - 1.0

(*** include-fsi-output ***)

(**
### Basic numerical types: float, int, and decimal

Notice that I included zeros after the decimal point. This is important. The decimal point makes it a [floating point](https://en.wikipedia.org/wiki/Floating-point_arithmetic) number. Floating point numbers (floats) are the most commonly used numerical type for mathematical calculations. 

If we left the decimal off it would be an integer and we would get the wrong answer because integers cannot represent fractions.
*)

(120/100) - 1

(*** include-fsi-output ***)

(** The other numerical data type is [decimal](https://en.wikipedia.org/wiki/Decimal_data_type). *)

(120m/100m) - 1m

(*** include-fsi-output ***)

(** Decimals are used when you need an exact fractional amount. Floats are insufficient in these circumstances because *"... such representations typically restrict the denominator to a power of two ... 0.3 (3/10) might be represented as 5404319552844595/18014398509481984 (0.299999999999999988897769...)"* ([see wiki](https://en.wikipedia.org/wiki/Decimal_data_type)).

### Static type checking

Finally, since F# is staticly typed, we must do arithmetic using numbers that are all the same type. If we mix floats and integers we will get an error:

```fsharp
(120.0 / 100) - 1.0
```
```output
fundamentals.fsx(18,10): error FS0001: The type 'int' does not match the type 'float'
```
Static tying can slow you down a bit writing simple small programs, but as programs get larger and more complex the benefits become apparent. Specifically, static typing as implemented by F#:

- helps you ensure that the code is correct (i.e., the type of the input data matches what the function expects). In the words of Yaron Minksy at Janestreet, you can ["make illegal states unrepresentable"](https://blog.janestreet.com/effective-ml-revisited/) (see [here](https://fsharpforfunandprofit.com/posts/designing-with-types-making-illegal-states-unrepresentable/) for F# examples).
- it also facilitates editor tooling that can check your code without running it and give tooltip errors (you should have seen a tooltip error in your editor if you type `(120.0 / 100) - 1.0` in your program file). It's like clippy on steriods (you are too young, but your parents might get this reference).

![clippy](img/intellisense-example.png)

[Image source](https://blog.codinghorror.com/it-looks-like-youre-writing-a-for-loop/)

 *)

(**
### Assigning values
We could also do the same calculations by assigning \$120.00 and \$100.0 to named values.
*)

let yearAgo = 100.0
let today = 120.0
(today / yearAgo) - 1.0

(*** include-fsi-output ***)
(** This works for one-off calculations, but if we want to do this more than once, then it makes more sense to define a function to do this calculation. *)

(**
### Defining functions 
Functions map (or transform) inputs into outputs.
*)

// Here is a simple function.
// It takes an input as an input x and then it adds
// 1 to whatever x is.
let f(x) = x + 1

f(1) // here x is 1
f(2) // here x is 2
f(3) // here x is 3

// We can also chain them
f(f(f(1))) // = (1 + (1 + (1 + 1)))

// The parentheses are optional, and it's more standard leave them off.
let f2 x = x + 1

// We can easily define a function to calcuate this return. 
let calcReturn pv fv = (fv / pv) - 1.0

(** The type signature tells us that `calcReturn` is a function with two float inputs (pv and fv) and it maps those two inputs into a float output. The program was able to infer that `pv` and `fv` are floats because of the `1.0` float in the calculation.*)

(** Se can execute it on simple floats:*)
// here pv = 100., fv = 110.0
calcReturn 100.0 110.0
(*** include-fsi-output ***)

// here pv = 80.0, fv = 60.0
calcReturn 80.0 60.0

(*** include-fsi-output ***)

(** Or we can execute it on our previously defined `yearAgo` and `today` values: *)
calcReturn yearAgo today
(*** include-fsi-output ***)

(** However, if we try to get it to execute on decimals, we will get an error because we defined the function to only operate on floats. This is another (simple) example of the compiler using type checking.

```
calcReturn 100.0m 120.0m
```
```output
fundamentals.fsx(23,14): error FS0001: This expression was expected to have type
    'float'    
but here has type
    'decimal'    
```

*)

(**
### Handling dividends
Our prior return calculation did not handle cash distributions such as dividends. We can incorporate dividends with a small modificaton:

*)

let simpleReturn beginningPrice endingPrice dividend =
    // This is solving for `r` in FV = PV*(1+r)^t where t=1.
    (endingPrice + dividend) / beginningPrice - 1.0

(**
The examples thus far have used simple (per period) compounding. We can also calculate continuously compounded returns, sldo known as log returns.
*)

let logReturn beginningPrice endingPrice dividend =
    // This is solving for `r` in FV = PV*e^(rt) where t=1.
    log(endingPrice + dividend) - log(beginningPrice)

(** These two calculations give slightly different returns.*)

simpleReturn 100.0 110.0 0.0 
(*** include-fsi-output ***)
logReturn 100.0 110.0 0.0
(*** include-fsi-output ***)

(** It is typically not important which version of return you use so long as you are consistent and keep track of what type of return it is when you're compounding things.

**Practice:** Can you write a function to compound an initial investment of \$100.00 at 6% for 5 years? You can calculate power and exponents using:*)

2.0**3.0
(*** include-fsi-output ***)
log 2.0
(*** include-fsi-output ***)
exp 0.6931
(*** include-fsi-output ***)
exp(log(2.0))
(*** include-fsi-output ***)

(**
### Tuples 
Looking at our return functions, we're starting to get several values that we're passing into the functions individaully. It can be useful to group these values together to make it easy to pass them around. Tuples are a simple way to group values.

*)

(1,2)
(*** include-fsi-output ***)
(1,2,3)
(*** include-fsi-output ***)
(** Tubles can contain mixed types.*)
(1,"2")
(*** include-fsi-output ***)
(** We can also deconstruct tuples. We can use built-in convenience functions for pairs.*)
fst (1,2)
(*** include-fsi-output ***)
snd (1,2)
(*** include-fsi-output ***)
(** We can also deconstruct tuples using pattern matching.*)
let (a, b) = (1, 2)
(*** include-fsi-output ***)
let (c, d, e) = (1, "2", 3.0)
(*** include-fsi-output ***)

(** Now redefining our simple return function to take a single tuple as the input parameter.*)

let simpleReturnTuple (beginningPrice, endingPrice, dividend) =
    // This is solving for `r` in FV = PV*(1+r)^t where t=1.
    (endingPrice + dividend) / beginningPrice - 1.0

simpleReturnTuple (100.0, 110.0, 0.0)
(*** include-fsi-output ***)
let xx = (100.0, 110.0, 0.0)
simpleReturnTuple xx
(*** include-fsi-output ***)

(**
### Records
If we want more structure than a tuple, then we can define a record.

*)

type RecordExample = 
    { BeginningPrice : float 
      EndingPrice : float 
      Dividend : float }      

(** And construct a value with that record type.*)
let x = { BeginningPrice = 100.0; EndingPrice = 110.0; Dividend = 0.0}
(*** include-fsi-output ***)
(** Similar to tuples, we can deconstruct our record value `x` using pattern matching.*)
let { BeginningPrice = aa; EndingPrice = bb; Dividend = cc} = x
(*** include-fsi-output ***)
(** We can also access individual fields by name.*)
x.EndingPrice / x.BeginningPrice
(*** include-fsi-output ***)

(** We can define a return function that operates on the `RecordExample` type explicitly:*)

let simpleReturnRecord1 { BeginningPrice = beginningPrice; EndingPrice = endingPrice; Dividend = dividend} =
    // This is solving for `r` in FV = PV*(1+r)^t where t=1.
    (endingPrice + dividend) / beginningPrice - 1.0

(** Or we can let the compiler's type inference figure out the input type.*)
let simpleReturnRecord2 x =
    // This is solving for `r` in FV = PV*(1+r)^t where t=1.
    (x.EndingPrice + x.Dividend) / x.BeginningPrice - 1.0

(** Or we can provide a type hint to tell the compiler the type of the input.*)
let simpleReturnRecord3 (x : RecordExample) =
    // This is solving for `r` in FV = PV*(1+r)^t where t=1.
    (x.EndingPrice + x.Dividend) / x.BeginningPrice - 1.0

(** All 3 can be used interchangably, but when you have many similar types a type hint may be necessary to make the particular type that you want explicit.*)

simpleReturnRecord1 x
(*** include-fsi-output ***)
simpleReturnRecord2 x
(*** include-fsi-output ***)
simpleReturnRecord3 x
(*** include-fsi-output ***)



(**
### Pipelines and lambda expressions
This download code used pipelining and lambda functions, which are two important language features. Pipelines are created using the pipe operator (`|>`) and allow you to pipe the output of one function to the input of another. Lambda expressions allow you to create functions on the fly. 

*)

1.0 |> fun x -> x + 1.0 |> fun x -> x ** 2.0
(*** include-fsi-output ***)

(**
### Collections: Arrays, Lists, Sequences

*)

(** A simple int array.*)

let ar = [| 0 .. 10 |] 
(** *)
ar |> Array.take 5
(*** include-fsi-output ***)

(**
When we look at the type signature of the elements in the array `val ar : int []`, it tells us that we have a integer array, meaning an array in which each element of the array is an integer. Arrays are "zero indexed", meaning the 0th item is the first in the array. We can access the elements individually or use a range to access multiple together.
*)

ar.[0]
(*** include-fsi-output ***)
ar.[0 .. 2]
(*** include-fsi-output ***)

(** A simple float array.*)
let arr = [| 1.0 .. 10.0 |]
arr.[0]
arr.[0 .. 5]
(*** include-fsi-output ***)

(** Lists and sequences are similar. *)
// List
[ 1.0 .. 10.0 ]
// Sequence
seq { 1.0 .. 10.0 }

(** Arrays, lists, and sequences have different properties that can make one data structure preferable to the others in a given setting. We'll discuss these different properties in due time, but for an overview you can see the F# collection language reference [here](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/fsharp-collection-types). Sequences are the most different as they are "lazy", meaning "Sequences are particularly useful when you have a large, ordered collection of data but don't necessarily expect to use all the elements. Individual sequence elements are computed only as required, so a sequence can perform better than a list if not all the elements are used" (see F# language reference).

These collections have several built-in functions for operating on them such as map, filter, groupBy, etc.*)

arr
|> Array.map(fun x -> x + 1.0)
(*** include-fsi-output ***)

arr
|> Array.filter(fun x -> x < 5.0)
(*** include-fsi-output ***)
arr
|> Array.groupBy(fun x -> x < 5.0)
|> Array.map(fun (group, xs) -> Array.min xs, Array.max xs)
(*** include-fsi-output ***)

(**
## Working with data

With this foundation, let's now try loading some data. We are going to obtain and process the data using an external F# library called [FSharp.Data](https://github.com/fsprojects/FSharp.Data) that makes the processing easier. 

### Namespaces
First, let's create a file directory to hold data. We are going to use built-in dotnet IO (input-output) libraries to do so.

*)
// Set working directory to the this code file's directory
System.IO.Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)
// Now create cache directory one level above the working directory
System.IO.File.WriteAllLines("test.txt",["first";"second"]) 

(** This illustrates the library namespace hierarchy. If we want to access the function within the hierarchy without typing the full namespace repetitively, we can open it. The following code is equivalent.

```
open System.IO
Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)
File.WriteAllLines("test.txt",["first";"second"]) 
```

It is common to open the System namespace
*)

open System

(**
### API keys
We are going to request the data from the provider [tiingo](https://api.tiingo.com/). Make sure that you are signed up and have your [API token](https://api.tiingo.com/documentation/general/connecting). An [API](https://en.wikipedia.org/wiki/API) (application programming interface) allows you to write code to communicate with another program. In this case we are goig to write code that requests stock price data from tiingo's web servers.

Once you have your api key, create a file called `secrets.fsx` and save it at the root/top level of your project folder. In `secrets.fsx`, assign your key to a value named `tiingoKey`. If you are using git, make sure to add `secrets.fsx` to your `.gitignore` file.

```fsharp
let tiingoKey = "yourSuperSecretApiKey"
```

We can load this in our interactive session as follows, assuming that `secrets.fsx` is located one folder above the current one in the file system.


    #load "secrets.fsx"

and we can access the value by typing 

    Secrets.tiingoKey

*)

(**
### FSharp.Data Csv Type Provider
We're now going to process our downloaded data using the **FSharp.Data** [Csv Type Provider](http://fsprojects.github.io/FSharp.Data/library/CsvProvider.html). This is code that automatically defines the types of input data based on a sample. We have already reference the nuget packaged and opened the namespace, so we can just use it now.
*)

#load "Common.fsx"
open Common


let aapl = 
    "AAPL"
    |> Tiingo.request
    |> Tiingo.get  

aapl
|> Array.take 5
(***include-it***)


(**
### Plotting
Now let's plot the stock price using [Plotly.NET](https://plotly.github.io/Plotly.NET/).
*)

#r "nuget: Plotly.NET, 2.0.0-beta5"
open Plotly.NET


let sampleChart =
    aapl
    |> Seq.map(fun x -> x.Date, x.AdjClose)
    |> Chart.Line

(***do-not-eval***)
sampleChart |> Chart.Show   

(***hide***)
sampleChart |> GenericChart.toChartHTML
(*** include-it-raw ***) 

(**
Let's calculate returns for this data. Typically we calculate close-close returns. Looking at the data, we could use the `close`, `divCash`, and `splitFacor` columns to calculate returns accounting for stock splits and dividends (a good at home exercise). But there is also an `adjClose` column that accounts for both those things. So we we can use this
*)

// Returns
let returns = 
    aapl
    |> Seq.sortBy(fun x -> x.Date)
    |> Seq.pairwise
    |> Seq.map(fun (a,b) -> b.Date, calcReturn (float a.AdjClose) (float b.AdjClose))

let avgReturnEachMonth = 
    returns
    |> Seq.groupBy(fun (date, ret) -> DateTime(date.Year, date.Month,1))
    |> Seq.map(fun (month, xs) -> month, Seq.length xs, xs |> Seq.averageBy snd)

(** We can look at a few of these*)
avgReturnEachMonth |> Seq.take 3 |> Seq.toList
(***include-fsi-output***)

(** The default DateTime printing is too verbose if we don't care about time. We can simplify the printing:

    fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))
 *)

avgReturnEachMonth |> Seq.take 3 |> Seq.toList
(***include-fsi-output***)

let monthlyReturnChart =
    avgReturnEachMonth
    |> Seq.map(fun (month, cnt, ret) -> month, ret)
    |> Chart.Bar

(***do-not-eval***)
monthlyReturnChart |> Chart.Show
(***hide***)
monthlyReturnChart |> GenericChart.toChartHTML
(*** include-it-raw ***) 

(**
## Volatility
We represent volatility by the standard deviation of returns. We can define a standard deviation function ourself.
*)

let stddev xs =
    let mu = xs |> Seq.average
    let sse = xs |> Seq.sumBy(fun x -> (x - mu)**2.0)
    let n = xs |> Seq.length |> float
    sqrt (sse / (n - 1.0))

[1.0 .. 10.0 ] |> stddev    
(***include-fsi-output***)

(**
But it is also convenient to use the [FSharp.Stats](https://fslab.org/FSharp.Stats/)
*)

#r "nuget: FSharp.Stats, 0.4.0"

open FSharp.Stats
[1.0 .. 10.0 ] |> Seq.stDev
(***include-fsi-output***)

(** Now let's look at 5-day rolling volatilities.*)

let rollingVols =
    returns
    // Sort by date again because you never can be too careful
    // about making sure that you have the right sort order.
    |> Seq.sortBy fst 
    |> Seq.windowed 5
    |> Seq.map(fun xs ->
        let maxWindowDate = xs |> Seq.map fst |> Seq.max
        let dailyVol = xs |> Seq.stDevBy snd
        let annualizedVolInPct = dailyVol * sqrt(252.0) * 100.0
        maxWindowDate, annualizedVolInPct)

let volChart = 
    rollingVols
    |> Chart.Line

(***do-not-eval***)
volChart |> Chart.Show    
(***hide***)
volChart |> GenericChart.toChartHTML
(*** include-it-raw ***) 
