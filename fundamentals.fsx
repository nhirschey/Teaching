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
val it: decimal = 0.2M*)
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

// We can also chain them
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
input.fsx (1,5)-(1,60) typecheck warning The field labels and expected type of this record expression or pattern do not uniquely determine a corresponding record type
input.fsx (1,1)-(1,64) interactive warning Accessing the internal type, method or field 'Dividend@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(1,64) interactive warning Accessing the internal type, method or field 'EndingPrice@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(1,64) interactive warning Accessing the internal type, method or field 'BeginningPrice@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
val cc: float = 0.0
val bb: float = 110.0
val aa: float = 100.0*)
(**
We can also access individual fields by name.

*)
x.EndingPrice / x.BeginningPrice(* output: 
input.fsx (1,1)-(1,33) interactive warning Accessing the internal type, method or field 'EndingPrice@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
input.fsx (1,1)-(1,33) interactive warning Accessing the internal type, method or field 'BeginningPrice@' from a previous evaluation in F# Interactive is deprecated and may cause subsequent access errors. To enable the legacy generation of a single dynamic assembly that can access internals, use the '--multiemit-' option.
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

This download code used pipelining and lambda functions, which are two important language features. Pipelines are created using the pipe operator (`|>`) and allow you to pipe the output of one function to the input of another. Lambda expressions allow you to create functions on the fly.

*)
1.0 |> fun x -> x + 1.0 |> fun x -> x ** 2.0(* output: 
val it: float = 4.0*)
(**
### Collections: Arrays, Lists, Sequences

A simple int array.

*)
let ar = [| 0 .. 10 |] 
ar |> Array.take 5(* output: 
val it: int[] = [|0; 1; 2; 3; 4|]*)
(**
When we look at the type signature of the elements in the array `val ar : int []`, it tells us that we have a integer array, meaning an array in which each element of the array is an integer. Arrays are "zero indexed", meaning the 0th item is the first in the array. We can access the elements individually or use a range to access multiple together.

*)
ar.[0] // or ar[0] in F# 6(* output: 
val it: int = 0*)
ar.[0 .. 2] // or ar[0 .. 2] in F# 6(* output: 
val it: int[] = [|0; 1; 2|]*)
(**
A simple float array.

*)
let arr = [| 1.0 .. 10.0 |]
arr.[0]
arr.[0 .. 5](* output: 
val arr: float[] = [|1.0; 2.0; 3.0; 4.0; 5.0; 6.0; 7.0; 8.0; 9.0; 10.0|]
val it: float[] = [|1.0; 2.0; 3.0; 4.0; 5.0; 6.0|]*)
(**
Lists and sequences are similar.

*)
// List
[ 1.0 .. 10.0 ]
// Sequence
seq { 1.0 .. 10.0 }
(**
Arrays, lists, and sequences have different properties that can make one data structure preferable to the others in a given setting. We'll discuss these different properties in due time, but for an overview you can see the F# collection language reference [here](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/fsharp-collection-types). Sequences are the most different as they are "lazy", meaning "Sequences are particularly useful when you have a large, ordered collection of data but don't necessarily expect to use all the elements. Individual sequence elements are computed only as required, so a sequence can perform better than a list if not all the elements are used" (see F# language reference).

These collections have several built-in functions for operating on them such as map, filter, groupBy, etc.

*)
arr
|> Array.map(fun x -> x + 1.0)(* output: 
val it: float[] = [|2.0; 3.0; 4.0; 5.0; 6.0; 7.0; 8.0; 9.0; 10.0; 11.0|]*)
arr
|> Array.filter(fun x -> x < 5.0)(* output: 
val it: float[] = [|1.0; 2.0; 3.0; 4.0|]*)
arr
|> Array.groupBy(fun x -> x < 5.0)
|> Array.map(fun (group, xs) -> Array.min xs, Array.max xs)(* output: 
val it: (float * float)[] = [|(1.0, 4.0); (5.0, 10.0)|]*)
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
(**
This illustrates the library namespace hierarchy. If we want to access the function within the hierarchy without typing the full namespace repetitively, we can open it. The following code is equivalent.

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
|> Array.take 5(* output: 
[|{ Date = 6/14/2021 12:00:00 AM
    Close = 130.48M
    High = 130.54M
    Low = 127.07M
    Open = 127.82M
    Volume = 96906490
    AdjClose = 129.739141349036M
    AdjHigh = 129.798800672158M
    AdjLow = 126.348503151609M
    AdjOpen = 127.094244690633M
    AdjVolume = 96906490M
    DivCash = 0.0M
    SplitFactor = 1.0M }; { Date = 6/15/2021 12:00:00 AM
                            Close = 129.64M
                            High = 130.6M
                            Low = 129.39M
                            Open = 129.94M
                            Volume = 62746332
                            AdjClose = 128.90391082533M
                            AdjHigh = 129.85845999528M
                            AdjLow = 128.655330312322M
                            AdjOpen = 129.202207440939M
                            AdjVolume = 62746332M
                            DivCash = 0.0M
                            SplitFactor = 1.0M }; { Date = 6/16/2021 12:00:00 AM
                                                    Close = 130.15M
                                                    High = 130.89M
                                                    Low = 128.461M
                                                    Open = 130.37M
                                                    Volume = 91339351
                                                    AdjClose = 129.411015071866M
                                                    AdjHigh = 130.146813390369M
                                                    AdjLow = 127.731605125985M
                                                    AdjOpen = 129.629765923312M
                                                    AdjVolume = 91339351M
                                                    DivCash = 0.0M
                                                    SplitFactor = 1.0M };
  { Date = 6/17/2021 12:00:00 AM
    Close = 131.79M
    High = 132.55M
    Low = 129.65M
    Open = 129.8M
    Volume = 96721669
    AdjClose = 131.041703237197M
    AdjHigh = 131.797387996741M
    AdjLow = 128.91385404585M
    AdjOpen = 129.063002353655M
    AdjVolume = 96721669M
    DivCash = 0.0M
    SplitFactor = 1.0M }; { Date = 6/18/2021 12:00:00 AM
                            Close = 130.46M
                            High = 131.51M
                            Low = 130.24M
                            Open = 130.71M
                            Volume = 108953309
                            AdjClose = 129.719254907995M
                            AdjHigh = 130.763293062628M
                            AdjLow = 129.500504056548M
                            AdjOpen = 129.967835421003M
                            AdjVolume = 108953309M
                            DivCash = 0.0M
                            SplitFactor = 1.0M }|]*)
(**
### Plotting

Now let's plot the stock price using [Plotly.NET](https://plotly.github.io/Plotly.NET/).

*)
#r "nuget: Plotly.NET, 2.0.0-preview.17"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.17"
open Plotly.NET


let sampleChart =
    aapl
    |> Seq.map(fun x -> x.Date, x.AdjClose)
    |> Chart.Line
sampleChart |> Chart.show   (* output: 
<div id="f253fd77-7ccf-42e6-aa11-c2096e99fa1e"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_f253fd777ccf42e6aa11c2096e99fa1e = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-2.6.3.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","mode":"lines","x":["2021-06-14T00:00:00+00:00","2021-06-15T00:00:00+00:00","2021-06-16T00:00:00+00:00","2021-06-17T00:00:00+00:00","2021-06-18T00:00:00+00:00","2021-06-21T00:00:00+00:00","2021-06-22T00:00:00+00:00","2021-06-23T00:00:00+00:00","2021-06-24T00:00:00+00:00","2021-06-25T00:00:00+00:00","2021-06-28T00:00:00+00:00","2021-06-29T00:00:00+00:00","2021-06-30T00:00:00+00:00","2021-07-01T00:00:00+00:00","2021-07-02T00:00:00+00:00","2021-07-06T00:00:00+00:00","2021-07-07T00:00:00+00:00","2021-07-08T00:00:00+00:00","2021-07-09T00:00:00+00:00","2021-07-12T00:00:00+00:00","2021-07-13T00:00:00+00:00","2021-07-14T00:00:00+00:00","2021-07-15T00:00:00+00:00","2021-07-16T00:00:00+00:00","2021-07-19T00:00:00+00:00","2021-07-20T00:00:00+00:00","2021-07-21T00:00:00+00:00","2021-07-22T00:00:00+00:00","2021-07-23T00:00:00+00:00","2021-07-26T00:00:00+00:00","2021-07-27T00:00:00+00:00","2021-07-28T00:00:00+00:00","2021-07-29T00:00:00+00:00","2021-07-30T00:00:00+00:00","2021-08-02T00:00:00+00:00","2021-08-03T00:00:00+00:00","2021-08-04T00:00:00+00:00","2021-08-05T00:00:00+00:00","2021-08-06T00:00:00+00:00","2021-08-09T00:00:00+00:00","2021-08-10T00:00:00+00:00","2021-08-11T00:00:00+00:00","2021-08-12T00:00:00+00:00","2021-08-13T00:00:00+00:00","2021-08-16T00:00:00+00:00","2021-08-17T00:00:00+00:00","2021-08-18T00:00:00+00:00","2021-08-19T00:00:00+00:00","2021-08-20T00:00:00+00:00","2021-08-23T00:00:00+00:00","2021-08-24T00:00:00+00:00","2021-08-25T00:00:00+00:00","2021-08-26T00:00:00+00:00","2021-08-27T00:00:00+00:00","2021-08-30T00:00:00+00:00","2021-08-31T00:00:00+00:00","2021-09-01T00:00:00+00:00","2021-09-02T00:00:00+00:00","2021-09-03T00:00:00+00:00","2021-09-07T00:00:00+00:00","2021-09-08T00:00:00+00:00","2021-09-09T00:00:00+00:00","2021-09-10T00:00:00+00:00","2021-09-13T00:00:00+00:00","2021-09-14T00:00:00+00:00","2021-09-15T00:00:00+00:00","2021-09-16T00:00:00+00:00","2021-09-17T00:00:00+00:00","2021-09-20T00:00:00+00:00","2021-09-21T00:00:00+00:00","2021-09-22T00:00:00+00:00","2021-09-23T00:00:00+00:00","2021-09-24T00:00:00+00:00","2021-09-27T00:00:00+00:00","2021-09-28T00:00:00+00:00","2021-09-29T00:00:00+00:00","2021-09-30T00:00:00+00:00","2021-10-01T00:00:00+00:00","2021-10-04T00:00:00+00:00","2021-10-05T00:00:00+00:00","2021-10-06T00:00:00+00:00","2021-10-07T00:00:00+00:00","2021-10-08T00:00:00+00:00","2021-10-11T00:00:00+00:00","2021-10-12T00:00:00+00:00","2021-10-13T00:00:00+00:00","2021-10-14T00:00:00+00:00","2021-10-15T00:00:00+00:00","2021-10-18T00:00:00+00:00","2021-10-19T00:00:00+00:00","2021-10-20T00:00:00+00:00","2021-10-21T00:00:00+00:00","2021-10-22T00:00:00+00:00","2021-10-25T00:00:00+00:00","2021-10-26T00:00:00+00:00","2021-10-27T00:00:00+00:00","2021-10-28T00:00:00+00:00","2021-10-29T00:00:00+00:00","2021-11-01T00:00:00+00:00","2021-11-02T00:00:00+00:00","2021-11-03T00:00:00+00:00","2021-11-04T00:00:00+00:00","2021-11-05T00:00:00+00:00","2021-11-08T00:00:00+00:00","2021-11-09T00:00:00+00:00","2021-11-10T00:00:00+00:00","2021-11-11T00:00:00+00:00","2021-11-12T00:00:00+00:00","2021-11-15T00:00:00+00:00","2021-11-16T00:00:00+00:00","2021-11-17T00:00:00+00:00","2021-11-18T00:00:00+00:00","2021-11-19T00:00:00+00:00","2021-11-22T00:00:00+00:00","2021-11-23T00:00:00+00:00","2021-11-24T00:00:00+00:00","2021-11-26T00:00:00+00:00","2021-11-29T00:00:00+00:00","2021-11-30T00:00:00+00:00","2021-12-01T00:00:00+00:00","2021-12-02T00:00:00+00:00","2021-12-03T00:00:00+00:00","2021-12-06T00:00:00+00:00","2021-12-07T00:00:00+00:00","2021-12-08T00:00:00+00:00","2021-12-09T00:00:00+00:00","2021-12-10T00:00:00+00:00","2021-12-13T00:00:00+00:00","2021-12-14T00:00:00+00:00","2021-12-15T00:00:00+00:00","2021-12-16T00:00:00+00:00","2021-12-17T00:00:00+00:00","2021-12-20T00:00:00+00:00","2021-12-21T00:00:00+00:00","2021-12-22T00:00:00+00:00","2021-12-23T00:00:00+00:00","2021-12-27T00:00:00+00:00","2021-12-28T00:00:00+00:00","2021-12-29T00:00:00+00:00","2021-12-30T00:00:00+00:00","2021-12-31T00:00:00+00:00","2022-01-03T00:00:00+00:00","2022-01-04T00:00:00+00:00","2022-01-05T00:00:00+00:00","2022-01-06T00:00:00+00:00","2022-01-07T00:00:00+00:00","2022-01-10T00:00:00+00:00","2022-01-11T00:00:00+00:00","2022-01-12T00:00:00+00:00","2022-01-13T00:00:00+00:00","2022-01-14T00:00:00+00:00","2022-01-18T00:00:00+00:00","2022-01-19T00:00:00+00:00","2022-01-20T00:00:00+00:00","2022-01-21T00:00:00+00:00","2022-01-24T00:00:00+00:00","2022-01-25T00:00:00+00:00","2022-01-26T00:00:00+00:00","2022-01-27T00:00:00+00:00","2022-01-28T00:00:00+00:00","2022-01-31T00:00:00+00:00","2022-02-01T00:00:00+00:00","2022-02-02T00:00:00+00:00","2022-02-03T00:00:00+00:00","2022-02-04T00:00:00+00:00","2022-02-07T00:00:00+00:00","2022-02-08T00:00:00+00:00","2022-02-09T00:00:00+00:00","2022-02-10T00:00:00+00:00","2022-02-11T00:00:00+00:00","2022-02-14T00:00:00+00:00","2022-02-15T00:00:00+00:00","2022-02-16T00:00:00+00:00","2022-02-17T00:00:00+00:00","2022-02-18T00:00:00+00:00","2022-02-22T00:00:00+00:00","2022-02-23T00:00:00+00:00","2022-02-24T00:00:00+00:00","2022-02-25T00:00:00+00:00","2022-02-28T00:00:00+00:00","2022-03-01T00:00:00+00:00","2022-03-02T00:00:00+00:00","2022-03-03T00:00:00+00:00","2022-03-04T00:00:00+00:00","2022-03-07T00:00:00+00:00","2022-03-08T00:00:00+00:00","2022-03-09T00:00:00+00:00","2022-03-10T00:00:00+00:00","2022-03-11T00:00:00+00:00","2022-03-14T00:00:00+00:00","2022-03-15T00:00:00+00:00","2022-03-16T00:00:00+00:00","2022-03-17T00:00:00+00:00","2022-03-18T00:00:00+00:00","2022-03-21T00:00:00+00:00","2022-03-22T00:00:00+00:00","2022-03-23T00:00:00+00:00","2022-03-24T00:00:00+00:00","2022-03-25T00:00:00+00:00","2022-03-28T00:00:00+00:00","2022-03-29T00:00:00+00:00","2022-03-30T00:00:00+00:00","2022-03-31T00:00:00+00:00","2022-04-01T00:00:00+00:00","2022-04-04T00:00:00+00:00","2022-04-05T00:00:00+00:00","2022-04-06T00:00:00+00:00","2022-04-07T00:00:00+00:00","2022-04-08T00:00:00+00:00","2022-04-11T00:00:00+00:00","2022-04-12T00:00:00+00:00","2022-04-13T00:00:00+00:00","2022-04-14T00:00:00+00:00","2022-04-18T00:00:00+00:00","2022-04-19T00:00:00+00:00","2022-04-20T00:00:00+00:00","2022-04-21T00:00:00+00:00","2022-04-22T00:00:00+00:00","2022-04-25T00:00:00+00:00","2022-04-26T00:00:00+00:00","2022-04-27T00:00:00+00:00","2022-04-28T00:00:00+00:00","2022-04-29T00:00:00+00:00","2022-05-02T00:00:00+00:00","2022-05-03T00:00:00+00:00","2022-05-04T00:00:00+00:00","2022-05-05T00:00:00+00:00","2022-05-06T00:00:00+00:00","2022-05-09T00:00:00+00:00","2022-05-10T00:00:00+00:00","2022-05-11T00:00:00+00:00","2022-05-12T00:00:00+00:00","2022-05-13T00:00:00+00:00","2022-05-16T00:00:00+00:00","2022-05-17T00:00:00+00:00","2022-05-18T00:00:00+00:00","2022-05-19T00:00:00+00:00","2022-05-20T00:00:00+00:00","2022-05-23T00:00:00+00:00","2022-05-24T00:00:00+00:00","2022-05-25T00:00:00+00:00","2022-05-26T00:00:00+00:00","2022-05-27T00:00:00+00:00","2022-05-31T00:00:00+00:00","2022-06-01T00:00:00+00:00","2022-06-02T00:00:00+00:00","2022-06-03T00:00:00+00:00","2022-06-06T00:00:00+00:00","2022-06-07T00:00:00+00:00","2022-06-08T00:00:00+00:00","2022-06-09T00:00:00+00:00","2022-06-10T00:00:00+00:00","2022-06-13T00:00:00+00:00"],"y":[129.739141349036,128.90391082533,129.411015071866,131.041703237197,129.719254907995,131.548807483733,133.219268531145,132.940858356576,132.652504961487,132.354208345878,134.01472617277,135.555925353419,136.182348246198,136.490588082328,139.165314402292,141.213617829476,143.749139062156,142.426690732954,144.286072970253,143.679536518514,144.813063657829,148.303134060459,147.636938285598,145.558805196853,141.64117631185,145.320167904365,144.574426365342,145.966477238186,147.716484049761,148.144042532134,145.936647576625,144.156811103489,144.813063657829,145.031814509276,144.693745011586,146.523297587323,146.11562554599,146.225000971714,145.528975535292,145.479184589782,144.991233323789,145.250146240438,148.267477538317,148.476599509457,150.488153708042,149.562042121565,145.748055695534,146.086634124999,147.570404301183,149.084049044673,148.994425342756,147.739693515916,146.923122009559,147.978690054361,152.479791528424,151.195185134278,151.872341993208,153.007575550825,153.654857842449,156.034865037805,154.461471159704,153.425819493105,148.347143051132,148.924718019043,147.50069697747,148.406892185743,148.167895647298,145.449310022477,142.342355022681,142.830306288675,145.240188051337,146.216090583324,146.305714285241,144.762194974445,141.316661545185,142.23281494256,140.908375792006,142.053567538726,138.558243163956,140.520006417032,141.406285247102,142.690891641248,142.302522266274,142.212898564356,140.918333981108,140.320842634994,143.158926529038,144.234410952044,145.93726128847,148.138021079992,148.635930535087,148.855010695329,148.068313756279,148.018522810769,148.695679669699,148.227644781909,151.932091127819,149.17367274659,148.33718486203,149.392752906832,150.856606704813,150.328822682412,150.866564893915,150.028860540987,150.39784936311,147.515747482204,147.465884127864,149.580090351919,149.590063022787,150.587330109606,153.070525155784,157.438554996049,160.111230788723,160.579946319528,160.968880483387,161.497432039401,156.381451884022,159.802077991809,164.848249451111,164.319697895098,163.312458137411,161.397705330719,164.868194792848,170.712179921605,174.601521560197,174.082942675051,178.959578729594,175.259717837497,173.853571245083,178.809988666572,171.789228375369,170.672289238132,169.286087987454,172.517233348746,175.159991128816,175.798242064379,179.837173765995,178.800015995703,178.889770033517,177.712994871071,177.084716606375,181.51258247185,179.208895501299,174.441958826306,171.529938932796,171.699474337555,171.719419679291,174.601521560197,175.050291749265,171.719419679291,172.597014715692,169.335951341795,165.775707841853,164.060408452525,161.966147570206,161.178306571619,159.343335131873,159.253581094059,158.784865563254,169.864502897809,174.302341434152,174.132806029392,175.359444546179,172.427479310933,172.138271855755,171.409337819821,174.574708907371,176.022591581487,171.868666116437,168.393747698559,168.633397244619,172.537687765856,172.298038219796,168.633397244619,167.055704399721,164.080055869469,159.836261824646,162.50236302457,164.609281950352,164.878887689671,162.961691321186,166.316784966034,165.987266840201,162.931735127928,159.067386197702,157.210102215732,162.712056377373,158.288525173005,154.504059424798,150.400060948511,154.863533743889,159.356962732525,160.385458701035,163.740552345883,165.138508031236,168.573484858104,169.961455145705,173.815818678179,174.464869532093,175.343584534315,178.698678179163,177.510415846613,174.355030156815,174.05546822424,178.179437496032,174.804373055679,171.579089581614,171.888636911942,169.841630372675,165.50796774808,167.415178718812,170.151177703003,165.048639451463,164.828960700908,167.155558377246,166.985806615453,166.176989397499,161.55375023808,162.642158593105,156.571036759571,156.341372611263,163.401048822297,157.419795568535,157.729342898864,159.247123357247,165.777573487398,156.541080566313,157.28,152.06,154.51,146.5,142.56,147.11,145.54,149.24,140.82,137.35,137.59,143.11,140.36,140.52,143.78,149.64,148.84,148.71,151.21,145.38,146.14,148.71,147.96,142.64,137.13,131.88],"marker":{},"line":{}}];
            var layout = {"width":600,"height":600,"template":{"layout":{"title":{"x":0.05},"font":{"color":"rgba(42, 63, 95, 1.0)"},"paper_bgcolor":"rgba(255, 255, 255, 1.0)","plot_bgcolor":"rgba(229, 236, 246, 1.0)","autotypenumbers":"strict","colorscale":{"diverging":[[0.0,"#8e0152"],[0.1,"#c51b7d"],[0.2,"#de77ae"],[0.3,"#f1b6da"],[0.4,"#fde0ef"],[0.5,"#f7f7f7"],[0.6,"#e6f5d0"],[0.7,"#b8e186"],[0.8,"#7fbc41"],[0.9,"#4d9221"],[1.0,"#276419"]],"sequential":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]],"sequentialminus":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]},"hovermode":"closest","hoverlabel":{"align":"left"},"coloraxis":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"geo":{"showland":true,"landcolor":"rgba(229, 236, 246, 1.0)","showlakes":true,"lakecolor":"rgba(255, 255, 255, 1.0)","subunitcolor":"rgba(255, 255, 255, 1.0)","bgcolor":"rgba(255, 255, 255, 1.0)"},"mapbox":{"style":"light"},"polar":{"bgcolor":"rgba(229, 236, 246, 1.0)","radialaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""},"angularaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""}},"scene":{"xaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"yaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"zaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true}},"ternary":{"aaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"baxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"caxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"bgcolor":"rgba(229, 236, 246, 1.0)"},"xaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"yaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"annotationdefaults":{"arrowcolor":"#2a3f5f","arrowhead":0,"arrowwidth":1},"shapedefaults":{"line":{"color":"rgba(42, 63, 95, 1.0)"}},"colorway":["rgba(99, 110, 250, 1.0)","rgba(239, 85, 59, 1.0)","rgba(0, 204, 150, 1.0)","rgba(171, 99, 250, 1.0)","rgba(255, 161, 90, 1.0)","rgba(25, 211, 243, 1.0)","rgba(255, 102, 146, 1.0)","rgba(182, 232, 128, 1.0)","rgba(255, 151, 255, 1.0)","rgba(254, 203, 82, 1.0)"]},"data":{"bar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}},"error_x":{"color":"rgba(42, 63, 95, 1.0)"},"error_y":{"color":"rgba(42, 63, 95, 1.0)"}}],"barpolar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"carpet":[{"aaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"},"baxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"}}],"choropleth":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contourcarpet":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"heatmap":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"heatmapgl":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram":[{"marker":{"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"histogram2d":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram2dcontour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"mesh3d":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"parcoords":[{"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"pie":[{"automargin":true}],"scatter":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatter3d":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattercarpet":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergeo":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattermapbox":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolar":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolargl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterternary":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"surface":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"table":[{"cells":{"fill":{"color":"rgba(235, 240, 248, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}},"header":{"fill":{"color":"rgba(200, 212, 227, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}}}]}}};
            var config = {"responsive":true};
            Plotly.newPlot('f253fd77-7ccf-42e6-aa11-c2096e99fa1e', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_f253fd777ccf42e6aa11c2096e99fa1e();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_f253fd777ccf42e6aa11c2096e99fa1e();
            }
</script>
*)
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
(**
We can look at a few of these

*)
avgReturnEachMonth |> Seq.take 3 |> Seq.toList(* output: 
val it: (DateTime * int * float) list =
  [(6/1/2021 12:00:00 AM {Date = 6/1/2021 12:00:00 AM;
                          Day = 1;
                          DayOfWeek = Tuesday;
                          DayOfYear = 152;
                          Hour = 0;
                          Kind = Unspecified;
                          Millisecond = 0;
                          Minute = 0;
                          Month = 6;
                          Second = 0;
                          Ticks = 637581024000000000L;
                          TimeOfDay = 00:00:00;
                          Year = 2021;}, 12, 0.004080613212);
   (7/1/2021 12:00:00 AM {Date = 7/1/2021 12:00:00 AM;
                          Day = 1;
                          DayOfWeek = Thursday;
                          DayOfYear = 182;
                          Hour = 0;
                          Kind = Unspecified;
                          Millisecond = 0;
                          Minute = 0;
                          Month = 7;
                          Second = 0;
                          Ticks = 637606944000000000L;
                          TimeOfDay = 00:00:00;
                          Year = 2021;}, 21, 0.00309588506);
   (8/1/2021 12:00:00 AM {Date = 8/1/2021 12:00:00 AM;
                          Day = 1;
                          DayOfWeek = Sunday;
                          DayOfYear = 213;
                          Hour = 0;
                          Kind = Unspecified;
                          Millisecond = 0;
                          Minute = 0;
                          Month = 8;
                          Second = 0;
                          Ticks = 637633728000000000L;
                          TimeOfDay = 00:00:00;
                          Year = 2021;}, 22, 0.00195681901)]*)
(**
The default DateTime printing is too verbose if we don't care about time. We can simplify the printing:

    fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))
 
*)
avgReturnEachMonth |> Seq.take 3 |> Seq.toList(* output: 
val it: (DateTime * int * float) list =
  [(6/1/2021 12:00:00 AM {Date = 6/1/2021 12:00:00 AM;
                          Day = 1;
                          DayOfWeek = Tuesday;
                          DayOfYear = 152;
                          Hour = 0;
                          Kind = Unspecified;
                          Millisecond = 0;
                          Minute = 0;
                          Month = 6;
                          Second = 0;
                          Ticks = 637581024000000000L;
                          TimeOfDay = 00:00:00;
                          Year = 2021;}, 12, 0.004080613212);
   (7/1/2021 12:00:00 AM {Date = 7/1/2021 12:00:00 AM;
                          Day = 1;
                          DayOfWeek = Thursday;
                          DayOfYear = 182;
                          Hour = 0;
                          Kind = Unspecified;
                          Millisecond = 0;
                          Minute = 0;
                          Month = 7;
                          Second = 0;
                          Ticks = 637606944000000000L;
                          TimeOfDay = 00:00:00;
                          Year = 2021;}, 21, 0.00309588506);
   (8/1/2021 12:00:00 AM {Date = 8/1/2021 12:00:00 AM;
                          Day = 1;
                          DayOfWeek = Sunday;
                          DayOfYear = 213;
                          Hour = 0;
                          Kind = Unspecified;
                          Millisecond = 0;
                          Minute = 0;
                          Month = 8;
                          Second = 0;
                          Ticks = 637633728000000000L;
                          TimeOfDay = 00:00:00;
                          Year = 2021;}, 22, 0.00195681901)]*)
let monthlyReturnChart =
    avgReturnEachMonth
    |> Seq.map(fun (month, cnt, ret) -> month, ret)
    |> Chart.Bar
monthlyReturnChart |> Chart.show(* output: 
<div id="28ea0bb6-12d8-4aeb-ac31-91c6e0614f5b"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_28ea0bb612d84aebac3191c6e0614f5b = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-2.6.3.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"bar","x":[0.00408061321173217,0.003095885059913576,0.0019568190098954064,-0.0032670973797820145,0.0027846923099908833,0.004869983333998906,0.0034220197547167167,-0.0005661315187366455,-0.00281304962063521,0.0026222857134160843,-0.004878178710156972,-0.002200215503949427,-0.01307965162833127],"y":["2021-06-01T00:00:00","2021-07-01T00:00:00","2021-08-01T00:00:00","2021-09-01T00:00:00","2021-10-01T00:00:00","2021-11-01T00:00:00","2021-12-01T00:00:00","2022-01-01T00:00:00","2022-02-01T00:00:00","2022-03-01T00:00:00","2022-04-01T00:00:00","2022-05-01T00:00:00","2022-06-01T00:00:00"],"orientation":"h","marker":{"pattern":{}}}];
            var layout = {"width":600,"height":600,"template":{"layout":{"title":{"x":0.05},"font":{"color":"rgba(42, 63, 95, 1.0)"},"paper_bgcolor":"rgba(255, 255, 255, 1.0)","plot_bgcolor":"rgba(229, 236, 246, 1.0)","autotypenumbers":"strict","colorscale":{"diverging":[[0.0,"#8e0152"],[0.1,"#c51b7d"],[0.2,"#de77ae"],[0.3,"#f1b6da"],[0.4,"#fde0ef"],[0.5,"#f7f7f7"],[0.6,"#e6f5d0"],[0.7,"#b8e186"],[0.8,"#7fbc41"],[0.9,"#4d9221"],[1.0,"#276419"]],"sequential":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]],"sequentialminus":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]},"hovermode":"closest","hoverlabel":{"align":"left"},"coloraxis":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"geo":{"showland":true,"landcolor":"rgba(229, 236, 246, 1.0)","showlakes":true,"lakecolor":"rgba(255, 255, 255, 1.0)","subunitcolor":"rgba(255, 255, 255, 1.0)","bgcolor":"rgba(255, 255, 255, 1.0)"},"mapbox":{"style":"light"},"polar":{"bgcolor":"rgba(229, 236, 246, 1.0)","radialaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""},"angularaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""}},"scene":{"xaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"yaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"zaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true}},"ternary":{"aaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"baxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"caxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"bgcolor":"rgba(229, 236, 246, 1.0)"},"xaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"yaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"annotationdefaults":{"arrowcolor":"#2a3f5f","arrowhead":0,"arrowwidth":1},"shapedefaults":{"line":{"color":"rgba(42, 63, 95, 1.0)"}},"colorway":["rgba(99, 110, 250, 1.0)","rgba(239, 85, 59, 1.0)","rgba(0, 204, 150, 1.0)","rgba(171, 99, 250, 1.0)","rgba(255, 161, 90, 1.0)","rgba(25, 211, 243, 1.0)","rgba(255, 102, 146, 1.0)","rgba(182, 232, 128, 1.0)","rgba(255, 151, 255, 1.0)","rgba(254, 203, 82, 1.0)"]},"data":{"bar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}},"error_x":{"color":"rgba(42, 63, 95, 1.0)"},"error_y":{"color":"rgba(42, 63, 95, 1.0)"}}],"barpolar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"carpet":[{"aaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"},"baxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"}}],"choropleth":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contourcarpet":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"heatmap":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"heatmapgl":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram":[{"marker":{"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"histogram2d":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram2dcontour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"mesh3d":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"parcoords":[{"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"pie":[{"automargin":true}],"scatter":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatter3d":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattercarpet":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergeo":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattermapbox":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolar":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolargl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterternary":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"surface":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"table":[{"cells":{"fill":{"color":"rgba(235, 240, 248, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}},"header":{"fill":{"color":"rgba(200, 212, 227, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}}}]}}};
            var config = {"responsive":true};
            Plotly.newPlot('28ea0bb6-12d8-4aeb-ac31-91c6e0614f5b', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_28ea0bb612d84aebac3191c6e0614f5b();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_28ea0bb612d84aebac3191c6e0614f5b();
            }
</script>
*)
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
val stddev: xs: seq<float> -> float
val it: float = 3.027650354*)
(**
But it is also convenient to use the [FSharp.Stats](https://fslab.org/FSharp.Stats/)

*)
#r "nuget: FSharp.Stats"

open FSharp.Stats
[1.0 .. 10.0 ] |> Seq.stDev(* output: 
[Loading C:\Users\runneradmin\AppData\Local\Temp\3116--c8101fe4-dc1c-4738-ae6d-b479b3055840\Project.fsproj.fsx]
namespace FSI_3188.Project

val it: float = 3.027650354*)
(**
Now let's look at 5-day rolling volatilities.

*)
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
volChart |> Chart.show    (* output: 
<div id="6f03996d-ba23-4f41-b497-4a3dccee81a4"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_6f03996dba234f41b4974a3dccee81a4 = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-2.6.3.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","mode":"lines","x":["2021-06-21T00:00:00+00:00","2021-06-22T00:00:00+00:00","2021-06-23T00:00:00+00:00","2021-06-24T00:00:00+00:00","2021-06-25T00:00:00+00:00","2021-06-28T00:00:00+00:00","2021-06-29T00:00:00+00:00","2021-06-30T00:00:00+00:00","2021-07-01T00:00:00+00:00","2021-07-02T00:00:00+00:00","2021-07-06T00:00:00+00:00","2021-07-07T00:00:00+00:00","2021-07-08T00:00:00+00:00","2021-07-09T00:00:00+00:00","2021-07-12T00:00:00+00:00","2021-07-13T00:00:00+00:00","2021-07-14T00:00:00+00:00","2021-07-15T00:00:00+00:00","2021-07-16T00:00:00+00:00","2021-07-19T00:00:00+00:00","2021-07-20T00:00:00+00:00","2021-07-21T00:00:00+00:00","2021-07-22T00:00:00+00:00","2021-07-23T00:00:00+00:00","2021-07-26T00:00:00+00:00","2021-07-27T00:00:00+00:00","2021-07-28T00:00:00+00:00","2021-07-29T00:00:00+00:00","2021-07-30T00:00:00+00:00","2021-08-02T00:00:00+00:00","2021-08-03T00:00:00+00:00","2021-08-04T00:00:00+00:00","2021-08-05T00:00:00+00:00","2021-08-06T00:00:00+00:00","2021-08-09T00:00:00+00:00","2021-08-10T00:00:00+00:00","2021-08-11T00:00:00+00:00","2021-08-12T00:00:00+00:00","2021-08-13T00:00:00+00:00","2021-08-16T00:00:00+00:00","2021-08-17T00:00:00+00:00","2021-08-18T00:00:00+00:00","2021-08-19T00:00:00+00:00","2021-08-20T00:00:00+00:00","2021-08-23T00:00:00+00:00","2021-08-24T00:00:00+00:00","2021-08-25T00:00:00+00:00","2021-08-26T00:00:00+00:00","2021-08-27T00:00:00+00:00","2021-08-30T00:00:00+00:00","2021-08-31T00:00:00+00:00","2021-09-01T00:00:00+00:00","2021-09-02T00:00:00+00:00","2021-09-03T00:00:00+00:00","2021-09-07T00:00:00+00:00","2021-09-08T00:00:00+00:00","2021-09-09T00:00:00+00:00","2021-09-10T00:00:00+00:00","2021-09-13T00:00:00+00:00","2021-09-14T00:00:00+00:00","2021-09-15T00:00:00+00:00","2021-09-16T00:00:00+00:00","2021-09-17T00:00:00+00:00","2021-09-20T00:00:00+00:00","2021-09-21T00:00:00+00:00","2021-09-22T00:00:00+00:00","2021-09-23T00:00:00+00:00","2021-09-24T00:00:00+00:00","2021-09-27T00:00:00+00:00","2021-09-28T00:00:00+00:00","2021-09-29T00:00:00+00:00","2021-09-30T00:00:00+00:00","2021-10-01T00:00:00+00:00","2021-10-04T00:00:00+00:00","2021-10-05T00:00:00+00:00","2021-10-06T00:00:00+00:00","2021-10-07T00:00:00+00:00","2021-10-08T00:00:00+00:00","2021-10-11T00:00:00+00:00","2021-10-12T00:00:00+00:00","2021-10-13T00:00:00+00:00","2021-10-14T00:00:00+00:00","2021-10-15T00:00:00+00:00","2021-10-18T00:00:00+00:00","2021-10-19T00:00:00+00:00","2021-10-20T00:00:00+00:00","2021-10-21T00:00:00+00:00","2021-10-22T00:00:00+00:00","2021-10-25T00:00:00+00:00","2021-10-26T00:00:00+00:00","2021-10-27T00:00:00+00:00","2021-10-28T00:00:00+00:00","2021-10-29T00:00:00+00:00","2021-11-01T00:00:00+00:00","2021-11-02T00:00:00+00:00","2021-11-03T00:00:00+00:00","2021-11-04T00:00:00+00:00","2021-11-05T00:00:00+00:00","2021-11-08T00:00:00+00:00","2021-11-09T00:00:00+00:00","2021-11-10T00:00:00+00:00","2021-11-11T00:00:00+00:00","2021-11-12T00:00:00+00:00","2021-11-15T00:00:00+00:00","2021-11-16T00:00:00+00:00","2021-11-17T00:00:00+00:00","2021-11-18T00:00:00+00:00","2021-11-19T00:00:00+00:00","2021-11-22T00:00:00+00:00","2021-11-23T00:00:00+00:00","2021-11-24T00:00:00+00:00","2021-11-26T00:00:00+00:00","2021-11-29T00:00:00+00:00","2021-11-30T00:00:00+00:00","2021-12-01T00:00:00+00:00","2021-12-02T00:00:00+00:00","2021-12-03T00:00:00+00:00","2021-12-06T00:00:00+00:00","2021-12-07T00:00:00+00:00","2021-12-08T00:00:00+00:00","2021-12-09T00:00:00+00:00","2021-12-10T00:00:00+00:00","2021-12-13T00:00:00+00:00","2021-12-14T00:00:00+00:00","2021-12-15T00:00:00+00:00","2021-12-16T00:00:00+00:00","2021-12-17T00:00:00+00:00","2021-12-20T00:00:00+00:00","2021-12-21T00:00:00+00:00","2021-12-22T00:00:00+00:00","2021-12-23T00:00:00+00:00","2021-12-27T00:00:00+00:00","2021-12-28T00:00:00+00:00","2021-12-29T00:00:00+00:00","2021-12-30T00:00:00+00:00","2021-12-31T00:00:00+00:00","2022-01-03T00:00:00+00:00","2022-01-04T00:00:00+00:00","2022-01-05T00:00:00+00:00","2022-01-06T00:00:00+00:00","2022-01-07T00:00:00+00:00","2022-01-10T00:00:00+00:00","2022-01-11T00:00:00+00:00","2022-01-12T00:00:00+00:00","2022-01-13T00:00:00+00:00","2022-01-14T00:00:00+00:00","2022-01-18T00:00:00+00:00","2022-01-19T00:00:00+00:00","2022-01-20T00:00:00+00:00","2022-01-21T00:00:00+00:00","2022-01-24T00:00:00+00:00","2022-01-25T00:00:00+00:00","2022-01-26T00:00:00+00:00","2022-01-27T00:00:00+00:00","2022-01-28T00:00:00+00:00","2022-01-31T00:00:00+00:00","2022-02-01T00:00:00+00:00","2022-02-02T00:00:00+00:00","2022-02-03T00:00:00+00:00","2022-02-04T00:00:00+00:00","2022-02-07T00:00:00+00:00","2022-02-08T00:00:00+00:00","2022-02-09T00:00:00+00:00","2022-02-10T00:00:00+00:00","2022-02-11T00:00:00+00:00","2022-02-14T00:00:00+00:00","2022-02-15T00:00:00+00:00","2022-02-16T00:00:00+00:00","2022-02-17T00:00:00+00:00","2022-02-18T00:00:00+00:00","2022-02-22T00:00:00+00:00","2022-02-23T00:00:00+00:00","2022-02-24T00:00:00+00:00","2022-02-25T00:00:00+00:00","2022-02-28T00:00:00+00:00","2022-03-01T00:00:00+00:00","2022-03-02T00:00:00+00:00","2022-03-03T00:00:00+00:00","2022-03-04T00:00:00+00:00","2022-03-07T00:00:00+00:00","2022-03-08T00:00:00+00:00","2022-03-09T00:00:00+00:00","2022-03-10T00:00:00+00:00","2022-03-11T00:00:00+00:00","2022-03-14T00:00:00+00:00","2022-03-15T00:00:00+00:00","2022-03-16T00:00:00+00:00","2022-03-17T00:00:00+00:00","2022-03-18T00:00:00+00:00","2022-03-21T00:00:00+00:00","2022-03-22T00:00:00+00:00","2022-03-23T00:00:00+00:00","2022-03-24T00:00:00+00:00","2022-03-25T00:00:00+00:00","2022-03-28T00:00:00+00:00","2022-03-29T00:00:00+00:00","2022-03-30T00:00:00+00:00","2022-03-31T00:00:00+00:00","2022-04-01T00:00:00+00:00","2022-04-04T00:00:00+00:00","2022-04-05T00:00:00+00:00","2022-04-06T00:00:00+00:00","2022-04-07T00:00:00+00:00","2022-04-08T00:00:00+00:00","2022-04-11T00:00:00+00:00","2022-04-12T00:00:00+00:00","2022-04-13T00:00:00+00:00","2022-04-14T00:00:00+00:00","2022-04-18T00:00:00+00:00","2022-04-19T00:00:00+00:00","2022-04-20T00:00:00+00:00","2022-04-21T00:00:00+00:00","2022-04-22T00:00:00+00:00","2022-04-25T00:00:00+00:00","2022-04-26T00:00:00+00:00","2022-04-27T00:00:00+00:00","2022-04-28T00:00:00+00:00","2022-04-29T00:00:00+00:00","2022-05-02T00:00:00+00:00","2022-05-03T00:00:00+00:00","2022-05-04T00:00:00+00:00","2022-05-05T00:00:00+00:00","2022-05-06T00:00:00+00:00","2022-05-09T00:00:00+00:00","2022-05-10T00:00:00+00:00","2022-05-11T00:00:00+00:00","2022-05-12T00:00:00+00:00","2022-05-13T00:00:00+00:00","2022-05-16T00:00:00+00:00","2022-05-17T00:00:00+00:00","2022-05-18T00:00:00+00:00","2022-05-19T00:00:00+00:00","2022-05-20T00:00:00+00:00","2022-05-23T00:00:00+00:00","2022-05-24T00:00:00+00:00","2022-05-25T00:00:00+00:00","2022-05-26T00:00:00+00:00","2022-05-27T00:00:00+00:00","2022-05-31T00:00:00+00:00","2022-06-01T00:00:00+00:00","2022-06-02T00:00:00+00:00","2022-06-03T00:00:00+00:00","2022-06-06T00:00:00+00:00","2022-06-07T00:00:00+00:00","2022-06-08T00:00:00+00:00","2022-06-09T00:00:00+00:00","2022-06-10T00:00:00+00:00","2022-06-13T00:00:00+00:00"],"y":[17.328941870832846,16.173743282643038,17.33453522374084,16.650820134213763,13.561429321779823,12.861472088162081,12.354248816860988,11.313440373468156,9.934567394824331,10.927931011981869,11.33485778504985,12.541507206364845,19.470862591510173,18.58229724668966,19.487170607756713,18.227349461027917,21.232399067364273,19.23224331158765,23.304522888238736,31.24257366126545,37.17792433178676,30.968049702879195,32.66911520331664,31.882529772919472,18.34871562731078,17.554911462425064,19.67177697606153,18.30505437952548,14.556601619582981,13.517162946543285,14.496394415355374,10.011646311403085,9.936674006432387,11.049427479734392,10.79249923018535,3.5777572663722093,4.402101697887257,16.439252935475103,15.182287129969355,15.836363860460919,17.049338979112814,28.63929480435851,22.97403161532327,24.770660859634912,23.62627525265611,23.30586332175193,12.485178048032777,13.823576045874683,12.723925574578429,24.750663969552953,26.359420595785625,24.337304766017656,22.243999649412018,22.42924122016833,13.670191899781194,14.68293172947461,16.624083831141572,28.81701695146352,28.742290750342125,21.468729755160144,24.79202693482446,25.20279080907173,16.025743870599324,18.17649194446125,20.133885373335726,25.14500209959275,26.35258978081117,22.31456537300509,15.77277765530735,24.941341350985976,20.71321459486335,18.43576566119119,21.056637242965973,25.037757964245564,25.04184391994771,25.008897658679828,24.593786894064714,24.265522539725477,11.035383542362618,11.536416674326539,10.636881391260458,18.016533846101325,18.25322501038643,18.92399729512204,14.690779785548328,10.38364598024342,9.009552403039507,13.004041371469697,11.986619713451505,6.126201442992895,6.126262946483618,19.39279249818061,24.75339407850573,25.288289511187944,25.60663329413498,25.920529420120786,17.722480256101328,10.565925567977255,10.54017534807888,9.679738872309267,14.435609194017266,14.736101848687087,19.379460129564986,19.07503132145673,19.713263488126493,12.42206101333373,17.06931875349418,17.27970717308177,15.918575391838106,17.418639803768013,18.467902128008372,28.72316602972555,30.811669797722512,38.434323615693074,38.92249303667917,39.65484152080994,30.235493192815987,30.117433834749345,32.207395533534616,32.213352407786815,31.12970075443763,22.947739992720564,37.337122352880925,33.025544556554095,35.27019865352667,47.655200977282746,39.41656665969611,38.09795780697655,42.14263987719515,36.99946396607153,19.615462057398965,20.178905397147414,18.827952693436455,18.45010599925531,19.020345168754147,19.532521950142307,20.939397596439953,23.016020162261928,30.020596266985194,31.21610333625402,31.685254462408807,18.54533163898319,26.797205758701992,18.868714469465043,20.250901919125404,20.550210903641446,25.065315416178258,20.51711677806253,17.21440133398468,16.347675000131208,10.371525963238538,9.257415277195529,8.125866439399571,8.462923721310469,53.426213365631334,52.51484062592511,49.5027844510979,47.94815571968412,52.81646060747366,24.814501965434932,13.65613749831081,20.838560316534593,21.100144851416363,24.922769602692313,28.648588863676828,28.93170423044894,31.234592094704496,29.91576781812632,28.99189063376551,25.949332621907494,28.09450691435171,15.518587897717564,26.80313785840407,29.811864951473733,29.746510912100334,28.011525970608904,20.761422746467424,20.08980179488134,23.617573185009466,27.693365632112954,27.69842611847904,37.05989225532589,40.305339176238476,41.27468450861671,41.902022693922646,50.749627087309825,48.064542045527936,43.46255164783337,37.22332414078732,17.47687378148456,14.994816448974282,11.475819844132468,11.43532081565244,13.347600213873257,14.252161163763327,13.647349529332578,19.080935379687496,21.92764846071946,21.792946586313736,27.89327567409779,27.40133503923014,29.262320479981856,27.834196818377382,28.533930834083215,16.443241278560595,23.908008652609343,27.311304234804084,33.4802886301246,33.52637084623787,30.508809007358984,29.354044997667007,25.329876156052023,23.97249743982227,25.227820280574292,29.81625318620041,29.70487468824073,51.50421643744131,54.46910871020192,53.940673136201745,46.33484682215578,52.705106326332675,61.00206366342093,55.58459760827652,60.32937864100035,61.5987915441615,52.04398168428579,44.43911501918167,55.97902932815567,53.01064071624264,56.03497134305962,58.416714880575405,57.96149398234669,48.410131753003,61.53958387991971,56.62681045060669,40.395942362402934,36.19738336049432,41.245112188289355,37.81551327131506,31.114455020438907,29.696494356209552,46.46432984645255,32.941431307328415,36.4293412336462,36.61496221036008,39.666136191755264,39.697730455289744,40.2562390714666],"marker":{},"line":{}}];
            var layout = {"width":600,"height":600,"template":{"layout":{"title":{"x":0.05},"font":{"color":"rgba(42, 63, 95, 1.0)"},"paper_bgcolor":"rgba(255, 255, 255, 1.0)","plot_bgcolor":"rgba(229, 236, 246, 1.0)","autotypenumbers":"strict","colorscale":{"diverging":[[0.0,"#8e0152"],[0.1,"#c51b7d"],[0.2,"#de77ae"],[0.3,"#f1b6da"],[0.4,"#fde0ef"],[0.5,"#f7f7f7"],[0.6,"#e6f5d0"],[0.7,"#b8e186"],[0.8,"#7fbc41"],[0.9,"#4d9221"],[1.0,"#276419"]],"sequential":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]],"sequentialminus":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]},"hovermode":"closest","hoverlabel":{"align":"left"},"coloraxis":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"geo":{"showland":true,"landcolor":"rgba(229, 236, 246, 1.0)","showlakes":true,"lakecolor":"rgba(255, 255, 255, 1.0)","subunitcolor":"rgba(255, 255, 255, 1.0)","bgcolor":"rgba(255, 255, 255, 1.0)"},"mapbox":{"style":"light"},"polar":{"bgcolor":"rgba(229, 236, 246, 1.0)","radialaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""},"angularaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""}},"scene":{"xaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"yaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"zaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true}},"ternary":{"aaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"baxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"caxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"bgcolor":"rgba(229, 236, 246, 1.0)"},"xaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"yaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"annotationdefaults":{"arrowcolor":"#2a3f5f","arrowhead":0,"arrowwidth":1},"shapedefaults":{"line":{"color":"rgba(42, 63, 95, 1.0)"}},"colorway":["rgba(99, 110, 250, 1.0)","rgba(239, 85, 59, 1.0)","rgba(0, 204, 150, 1.0)","rgba(171, 99, 250, 1.0)","rgba(255, 161, 90, 1.0)","rgba(25, 211, 243, 1.0)","rgba(255, 102, 146, 1.0)","rgba(182, 232, 128, 1.0)","rgba(255, 151, 255, 1.0)","rgba(254, 203, 82, 1.0)"]},"data":{"bar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}},"error_x":{"color":"rgba(42, 63, 95, 1.0)"},"error_y":{"color":"rgba(42, 63, 95, 1.0)"}}],"barpolar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"carpet":[{"aaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"},"baxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"}}],"choropleth":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contourcarpet":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"heatmap":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"heatmapgl":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram":[{"marker":{"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"histogram2d":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram2dcontour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"mesh3d":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"parcoords":[{"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"pie":[{"automargin":true}],"scatter":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatter3d":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattercarpet":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergeo":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattermapbox":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolar":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolargl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterternary":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"surface":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"table":[{"cells":{"fill":{"color":"rgba(235, 240, 248, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}},"header":{"fill":{"color":"rgba(200, 212, 227, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}}}]}}};
            var config = {"responsive":true};
            Plotly.newPlot('6f03996d-ba23-4f41-b497-4a3dccee81a4', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_6f03996dba234f41b4974a3dccee81a4();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_6f03996dba234f41b4974a3dccee81a4();
            }
</script>
*)

