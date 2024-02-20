(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=fundamentals.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//fundamentals.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//fundamentals.ipynb)

# Fundamentals

A good place to start is to define a one-period return calculation.

Objectives:

* [Interactive programming](#Interactive-programming)

* [How to calculate returns](#Calculating-returns).

* [Working with data.](#Working-with-data)

* [How to calculate return volatility](#Volatility)

## Interactive programming

We are going to focus on interactive programming. This is the most productive (and most common) type of analytic programming. In constrast to compiled programs (e.g, C, C++, Fortran, Java), interactive programs:

* Allow rapid iterative development.

* You can quickly quickly write and rewrite sections of code, evaluating the output, without having to rerun the entire program.

* This is especially useful for financial analysis, because we often evaluate large datasets that take a long time to process.

Interactive programming typically involves a [REPL](https://en.wikipedia.org/wiki/Read%E2%80%93eval%E2%80%93print_loop) (Read, Evaluate, Print, Loop). It is common for scripting langauges such as R, Python, Julia, Ruby, and Perl.

### The terminal

The most basic way that you can run interactive code is at the command line using an interpreter. We can start the F# interactive interpreter by opening a terminal (e.g., terminal.app, cmd, powershell) and running `dotnet fsi`.

Once fsi is open, we can type a code snippet in the prompt followed by ";;" to terminate it and it will run.

![fsi](img/fsi.png)

It is fine to run code this way, but we can do better using an IDE (Integrated development environment) that incorportes syntax highlighting, intellisense tooltips, and execution. We will use two common IDE's: Visual Studio Code with the Ionide extension and Jupyter Notebooks.

## Calculating returns

### Basic calculations in fsi

Let's assume that you have $120.00 today and that you had $100.00 a year ago. Your annual return is then:

*)
(120.0 / 100.0) - 1.0(* output: 
val it: float = 0.2*)
(**
### Basic numerical types: float, int, and decimal

Notice that I included a decimal point "." in the numbers. The decimal point makes it a [floating point](https://en.wikipedia.org/wiki/Floating-point_arithmetic) number. Floating point numbers (floats) are the most commonly used numerical type for mathematical calculations.

If we left the decimal point off and wrote "120" without the ".0" at the end it would be an integer and we would get the wrong answer because integers cannot represent fractions.

*)
(120/100) - 1(* output: 
val it: int = 0*)
(**
The other main numerical data type is [decimal](https://en.wikipedia.org/wiki/Decimal_data_type).

*)
(120m/100m) - 1m(* output: 
val it: decimal = 0.2M {Scale = 1uy;}*)
(**
Decimals are used when you need an exact fractional amount. Floats are insufficient in these circumstances because **"... such representations typically restrict the denominator to a power of two ... 0.3 (3/10) might be represented as 5404319552844595/18014398509481984 (0.299999999999999988897769...)"** ([see wiki](https://en.wikipedia.org/wiki/Decimal_data_type)).

### Static type checking

Finally, since F# is staticly typed, we must do arithmetic using numbers that are all the same type. If we mix floats and integers we will get an error:

```fsharp
(120.0 / 100) - 1.0
```
```output
fundamentals.fsx(18,10): error FS0001: The type 'int' does not match the type 'float'
```
Static tying can slow you down a bit writing simple small programs, but as programs get larger and more complex the benefits become apparent. Specifically, static typing as implemented by F#:

* helps you ensure that the code is correct (i.e., the type of the input data matches what the function expects). In the words of Yaron Minksy at Janestreet, you can ["make illegal states unrepresentable"](https://blog.janestreet.com/effective-ml-revisited/) (see [here](https://fsharpforfunandprofit.com/posts/designing-with-types-making-illegal-states-unrepresentable/) for F# examples).

* it also facilitates editor tooling that can check your code without running it and give tooltip errors (you should have seen a tooltip error in your editor if you type `(120.0 / 100) - 1.0` in your program file). It's like clippy on steriods (you are too young, but your parents might get this reference).

![clippy](img/intellisense-example.png)

[Image source](https://blog.codinghorror.com/it-looks-like-youre-writing-a-for-loop/)

### Assigning values

We could also do the same calculations by assigning $120.00 and $100.0 to named values.

*)
let yearAgo = 100.0
let today = 120.0
(today / yearAgo) - 1.0(* output: 
val yearAgo: float = 100.0
val today: float = 120.0
val it: float = 0.2*)
(**
This works for one-off calculations, but if we want to do this more than once, then it makes more sense to define a function to do this calculation.

### Defining functions

Functions map (or transform) inputs into outputs.

Here is a simple function named `addOne`.
It takes an input x and then it adds 1 to whatever x is.

*)
let addOne x = x + 1

addOne 0 // here x is 0
addOne 1 // here x is 1
addOne 2 // here x is 2
(**
We can also chain them

*)
addOne (addOne (addOne 0)) // = (1 + (1 + (1 + 0)))
(**
We can define a function to calcuate a return.

*)
let calcReturn pv fv = (fv / pv) - 1.0
(**
The type signature tells us that `calcReturn` is a function with two float inputs (pv and fv) and it maps those two inputs into a float output. The program was able to infer that `pv` and `fv` are floats because of the `1.0` float in the calculation.

We can execute it on simple floats:

*)
// here pv = 100., fv = 110.0
calcReturn 100.0 110.0(* output: 
val it: float = 0.1*)
// here pv = 80.0, fv = 60.0
calcReturn 80.0 60.0(* output: 
val it: float = -0.25*)
(**
Or we can execute it on our previously defined `yearAgo` and `today` values:

*)
calcReturn yearAgo today(* output: 
val it: float = 0.2*)
(**
### Handling dividends

Our prior return calculation did not handle cash distributions such as dividends. We can incorporate dividends with a small modificaton:

*)
let simpleReturn beginningPrice endingPrice dividend =
    // This is solving for `r` in FV = PV*(1+r)^t where t=1.
    (endingPrice + dividend) / beginningPrice - 1.0
(**
The examples thus far have used simple (per period) compounding. We can also calculate continuously compounded returns, also known as log returns.

*)
let logReturn beginningPrice endingPrice dividend =
    // This is solving for `r` in FV = PV*e^(rt) where t=1.
    log(endingPrice + dividend) - log(beginningPrice)
(**
These two calculations give slightly different returns.

*)
simpleReturn 100.0 110.0 0.0 (* output: 
val it: float = 0.1*)
logReturn 100.0 110.0 0.0(* output: 
val it: float = 0.0953101798*)
(**
It is typically not important which version of return you use so long as you are consistent and keep track of what type of return it is when you're compounding things.

**Practice:** Can you write a function to compound an initial investment of $100.00 at 6% for 5 years? You can calculate power and exponents using:

*)
2.0**3.0(* output: 
val it: float = 8.0*)
log 2.0(* output: 
val it: float = 0.6931471806*)
exp 0.6931(* output: 
val it: float = 1.999905641*)
exp(log(2.0))(* output: 
val it: float = 2.0*)
(**
### Tuples

Looking at our return functions, we're starting to get several values that we're passing into the functions individaully. It can be useful to group these values together to make it easy to pass them around. Tuples are a simple way to group values.

Further information about tuples can be found in the [F# Language reference](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/tuples) and the [F# for Fun and Profit](https://fsharpforfunandprofit.com/posts/tuples/) websites.

*)
(1,2)(* output: 
val it: int * int = (1, 2)*)
(1,2,3)(* output: 
val it: int * int * int = (1, 2, 3)*)
(**
Tubles can contain mixed types.

*)
(1,"2")(* output: 
val it: int * string = (1, "2")*)
(**
We can also deconstruct tuples. We can use built-in convenience functions for pairs.

*)
fst (1,2)(* output: 
val it: int = 1*)
snd (1,2)(* output: 
val it: int = 2*)
(**
We can also deconstruct tuples using pattern matching.

*)
let (a, b) = (1, 2)(* output: 
val b: int = 2
val a: int = 1*)
let (c, d, e) = (1, "2", 3.0)(* output: 
val e: float = 3.0
val d: string = "2"
val c: int = 1*)
(**
Now redefining our simple return function to take a single tuple as the input parameter.

*)
let simpleReturnTuple (beginningPrice, endingPrice, dividend) =
    // This is solving for `r` in FV = PV*(1+r)^t where t=1.
    (endingPrice + dividend) / beginningPrice - 1.0

simpleReturnTuple (100.0, 110.0, 0.0)(* output: 
val simpleReturnTuple:
  beginningPrice: float * endingPrice: float * dividend: float -> float
val it: float = 0.1*)
let xx = (100.0, 110.0, 0.0)
simpleReturnTuple xx(* output: 
val xx: float * float * float = (100.0, 110.0, 0.0)
val it: float = 0.1*)
(**
### Records

If we want more structure than a tuple, then we can define a record.

For more information on records see the relevant sections of the [F# language reference](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/records) or [F# for Fun and Profit](https://fsharpforfunandprofit.com/posts/records/) websites.

You must first define the record type before you use it:

*)
type RecordExample = 
    { BeginningPrice : float 
      EndingPrice : float 
      Dividend : float }      
(**
And construct a value with that record type.

*)
let x = { BeginningPrice = 100.0; EndingPrice = 110.0; Dividend = 0.0}(* output: 
val x: RecordExample = { BeginningPrice = 100.0
                         EndingPrice = 110.0
                         Dividend = 0.0 }*)
(**
Similar to tuples, we can deconstruct our record value `x` using pattern matching.

*)
let { BeginningPrice = aa; EndingPrice = bb; Dividend = cc} = x(* output: 
val cc: float = 0.0
val bb: float = 110.0
val aa: float = 100.0*)
(**
We can also access individual fields by name.

*)
x.EndingPrice / x.BeginningPrice(* output: 
val it: float = 1.1*)
(**
We can define a return function that operates on the `RecordExample` type explicitly:

*)
let simpleReturnRecord1 { BeginningPrice = beginningPrice; EndingPrice = endingPrice; Dividend = dividend} =
    // This is solving for `r` in FV = PV*(1+r)^t where t=1.
    (endingPrice + dividend) / beginningPrice - 1.0
(**
Or we can let the compiler's type inference figure out the input type.

*)
let simpleReturnRecord2 x =
    // This is solving for `r` in FV = PV*(1+r)^t where t=1.
    (x.EndingPrice + x.Dividend) / x.BeginningPrice - 1.0
(**
Or we can provide a type hint to tell the compiler the type of the input.

*)
let simpleReturnRecord3 (x : RecordExample) =
    // This is solving for `r` in FV = PV*(1+r)^t where t=1.
    (x.EndingPrice + x.Dividend) / x.BeginningPrice - 1.0
(**
All 3 can be used interchangably, but when you have many similar types a type hint may be necessary to make the particular type that you want explicit.

*)
simpleReturnRecord1 x(* output: 
val it: float = 0.1*)
simpleReturnRecord2 x(* output: 
val it: float = 0.1*)
simpleReturnRecord3 x(* output: 
val it: float = 0.1*)
(**
### Pipelines and lambda expressions

Pipelines and lambda functions are two important tools. Pipelines are created using the pipe operator (`|>`) and allow you to pipe the output of one function to the input of another. Lambda expressions allow you to create functions on the fly.

Take this example from before

*)
addOne (addOne (addOne 0)) // = (1 + (1 + (1 + 0)))
(**
We can write the same thing using pipes.

*)
0 |> addOne |> addOne |> addOne // = (1 + (1 + (1 + 0)))
(**
lambda expressions `fun x -> ...` are used to create functions on the fly.

*)
1.0 |> fun x -> x + 1.0 |> fun x -> x ** 2.0(* output: 
val it: float = 4.0*)
(**
### Collections: Arrays, Lists, Sequences

Collections provide a way to store multiple values together.
Maybe you want to store a list of stock prices, or an array of portfolios.

A simple int array.

*)
let ar = [| 0 .. 10 |] 
ar (* output: 
val it: int array = [|0; 1; 2; 3; 4; 5; 6; 7; 8; 9; 10|]*)
(**
When we look at the type signature of the elements in the array `val ar : int []`, it tells us that we have a integer array, meaning an array in which each element of the array is an integer. Arrays are "zero indexed", meaning the 0th item is the first in the array.

We can access the elements individually

*)
ar[0](* output: 
val it: int = 0*)
ar[3](* output: 
val it: int = 3*)
(**
Or we can access a range of elements.

*)
ar[0 .. 2](* output: 
val it: int array = [|0; 1; 2|]*)
(**
Lists and sequences are similar.

*)
// List
[ 1.0 .. 10.0 ](* output: 
val it: float list = [1.0; 2.0; 3.0; 4.0; 5.0; 6.0; 7.0; 8.0; 9.0; 10.0]*)
// Sequence
seq { 1.0 .. 10.0 }(* output: 
val it: float seq = seq [1.0; 2.0; 3.0; 4.0; ...]*)
(**
Arrays, lists, and sequences have different properties that can make one data structure preferable to the others in a given setting. We'll discuss these different properties in due time, but for an overview you can see the F# collection language reference [here](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/fsharp-collection-types). Sequences are the most different as they are "lazy", meaning "Sequences are particularly useful when you have a large, ordered collection of data but don't necessarily expect to use all the elements. Individual sequence elements are computed only as required, so a sequence can perform better than a list if not all the elements are used" (see F# language reference).

These collections have several built-in functions for operating on them such as map, filter, groupBy, etc.

Array.map applies a function to each element of an array.

*)
ar
|> Array.map(fun x -> x + 1)(* output: 
val it: int array = [|1; 2; 3; 4; 5; 6; 7; 8; 9; 10; 11|]*)
(**
The Array.map function is equivalent to

*)
[| for x in ar do x + 1 |]
(**
Array.filter filters elements of a list to those for which a function evaluates to `true`.

*)
ar
|> Array.filter(fun x -> x < 5)(* output: 
val it: int array = [|0; 1; 2; 3; 4|]*)
(**
Array.filter is equivalent to

*)
[| for x in ar do if x < 5 then x |]
(**
While the comprehension syntax `[| for x in ar do ... |]` is common to use,
the Array module functions tend to be more useful when you're chaining multiple operations
together using pipes.

For example, this is what a groupBy function does.

*)
ar
|> Array.groupBy(fun x -> x < 5)(* output: 
val it: (bool * int array) array =
  [|(true, [|0; 1; 2; 3; 4|]); (false, [|5; 6; 7; 8; 9; 10|])|]*)
(**
Now a pipeline of two operations.

*)
ar
|> Array.groupBy(fun x -> x < 5)
|> Array.map(fun (group, xs) -> Array.min xs, Array.max xs)(* output: 
val it: (int * int) array = [|(0, 4); (5, 10)|]*)
(**
## Working with data

With this foundation, let's now try loading some data.
We are going to load some helper code that allows us to download and plot this data.
This will introduce using the `nuget` package manager for loading external libraries,
and how to open namespaces.

We're going to use the **Quotes.YahooFinance** library for this.
We download the code from the [nuget.org](http://www.nuget.org) package manager.

This is equivalent to loading libraries with `pip` or `conda` in python
or `install.packages` in R.

*)
#r "nuget: Quotes.YahooFinance, 0.0.5"
(**
We now have access to the code in the **Quotes.YahooFinance** library.
The library has a function called `YahooFinance.History` that we
can use to download quote histories from yahoo finance.

We can access code using fully qualified names.

*)
Quotes.YahooFinance.YahooFinance.History("AAPL")
(**
However, it's common to open a library's namespace in order to access
the library's functionality directly.

*)
open Quotes.YahooFinance

YahooFinance.History("AAPL")
(**
Namespaces such as `Quotes.YahooFinance` are a hierarchical way of organizing code.

We are ready to request some data. Let's define our start and end dates.
`DateTime` is a type in the `System` namespace.

*)
open System

let myStart = DateTime(2020,1,1)
let myEnd = DateTime.UtcNow
myEnd(* output: 
2/20/2024 12:49:08 PM*)
let aapl = YahooFinance.History("AAPL",startDate=myStart,endDate=myEnd,interval = Interval.Daily)
(**
### Plotting

Now let's plot the stock price using [Plotly.NET](https://plotly.github.io/Plotly.NET/).

*)
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"
open Plotly.NET


let dateAdjClose = [ for x in aapl do x.Date, x.AdjustedClose ]

dateAdjClose[..3](* output: 
input.fsx (6,31)-(6,35) typecheck error The value or constructor 'aapl' is not defined.
input.fsx (6,39)-(6,45) typecheck error Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.
input.fsx (6,47)-(6,62) typecheck error Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.
[Loading C:\Users\runneradmin\.packagemanagement\nuget\Projects\4004--d09c7a98-b352-4ce9-a982-d610913e6be5\Project.fsproj.fsx]
module FSI_0231.Project.fsproj*)
dateAdjClose
|> Chart.Line   (* output: 
No value returned by any evaluator*)
(**
Let's calculate returns for this data. Typically we calculate close-close returns. Looking at the data, we could use the `close`, `divCash`, and `splitFacor` columns to calculate returns accounting for stock splits and dividends (a good at home exercise). But there is also an `adjClose` column that accounts for both those things. So we we can use this

*)
// Returns
let returns = 
    aapl
    |> List.sortBy(fun x -> x.Date)
    |> List.pairwise
    |> List.map(fun (a,b) -> b.Date, calcReturn a.AdjustedClose b.AdjustedClose)

let avgReturnEachMonth = 
    returns
    |> List.groupBy(fun (date, ret) -> DateTime(date.Year, date.Month,1))
    |> List.map(fun (month, xs) -> 
        let avgPrice = [ for (date, price) in xs do price ] |> List.average
        month, xs.Length, avgPrice)
(**
We can look at a few of these

*)
avgReturnEachMonth |> List.take 3 (* output: 
input.fsx (1,1)-(1,19) typecheck error The value or constructor 'avgReturnEachMonth' is not defined.*)
let monthlyReturnChart =
    avgReturnEachMonth
    |> List.map(fun (month, cnt, ret) -> month, ret)
    |> Chart.Bar
monthlyReturnChart (* output: 
No value returned by any evaluator*)
(**
## Volatility

We represent volatility by the standard deviation of returns. We can define a standard deviation function ourself.

*)
let stddev xs =
    let mu = xs |> Seq.average
    let sse = xs |> Seq.sumBy(fun x -> (x - mu)**2.0)
    let n = xs |> Seq.length |> float
    sqrt (sse / (n - 1.0))

[1.0 .. 10.0 ] |> stddev    (* output: 
val stddev: xs: float seq -> float
val it: float = 3.027650354*)
(**
But it is also convenient to use the [FSharp.Stats](https://fslab.org/FSharp.Stats/)

*)
#r "nuget: FSharp.Stats, 0.5.0"

open FSharp.Stats

[1.0 .. 10.0 ] |> stDev(* output: 
[Loading C:\Users\runneradmin\.packagemanagement\nuget\Projects\4004--d09c7a98-b352-4ce9-a982-d610913e6be5\Project.fsproj.fsx]
module FSI_0254.Project.fsproj

val it: float = 3.027650354*)
(**
Now let's look at 5-day rolling volatilities.

*)
let rollingVols =
    returns
    // Sort by date again because you never can be too careful
    // about making sure that you have the right sort order.
    |> List.sortBy fst 
    |> List.windowed 5
    |> List.map(fun xs ->
        let maxWindowDate = xs |> Seq.map fst |> Seq.max
        let dailyVol = 
            xs 
            |> List.map (fun (date, ret) -> ret) 
            |> stDev
        let annualizedVolInPct = dailyVol * sqrt(252.0) * 100.0
        maxWindowDate, annualizedVolInPct)

let volChart = 
    rollingVols
    |> Chart.Line
volChart     (* output: 
No value returned by any evaluator*)

