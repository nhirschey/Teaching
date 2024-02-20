(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=quizzes/quiz-list-module.ipynb)&emsp;
[![Script](../img/badge-script.svg)](/Teaching//quizzes/quiz-list-module.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](/Teaching//quizzes/quiz-list-module.ipynb)

These exercises work with functions from the `List` module.
You can find examples of using them [F# core docs](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-listmodule.html).

There are similar functions for [arrays](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-arraymodule.html) and
[sequences](https://fsharp.github.io/fsharp-core-docs/reference/fsharp-collections-seqmodule.html).

We're going to use the following data in the questions

*)
#r "nuget: FSharp.Stats, 0.5.0"

open System
open FSharp.Stats

type ReturnOb = { Symbol : string; Date : DateTime; Return : float }
type ValueOb = { Symbol : string; Date : DateTime; Value : float }

let seed = 1
Random.SetSampleGenerator(Random.RandBasic(seed))   
let normal = Distributions.Continuous.Normal.Init 0.0 0.1

let returns =
    [ 
        for symbol in ["AAPL"; "TSLA"] do
        for month in [1..2] do
        for day in [1..3] do
            { Symbol = symbol 
              Date = DateTime(2021, month, day)
              Return = normal.Sample()}
    ]
(**
## Question 1

Given the list below, filter the list so that only numbers greater than `2` remain.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int list = [7]
```

</details>
</span>
</p>
</div>

## Question 2

Given the list below, take elements until you find one that is greater than `4`.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int list = [1; -4]
```

</details>
</span>
</p>
</div>

## Question 3

Given the list below, skip elements until you find one that is greater than `4`.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int list = [7; 2; -10]
```

</details>
</span>
</p>
</div>

## Question 4

Take a `list` containing floats `1.0 .. 10.0`. Create a new list
that contains each number in the original list divided by `3.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float list =
  [0.3333333333; 0.6666666667; 1.0; 1.333333333; 1.666666667; 2.0; 2.333333333;
   2.666666667; 3.0; 3.333333333]
```

</details>
</span>
</p>
</div>

## Question 5

Take a `list` containing floats `1.0 .. 10.0`. Group the elements based on whether the elements are greater than or equal to `4.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (bool * float list) list =
  [(false, [1.0; 2.0; 3.0]); (true, [4.0; 5.0; 6.0; 7.0; 8.0; 9.0; 10.0])]
```

</details>
</span>
</p>
</div>

## Question 6

Take a `list` containing floats `1.0 .. 10.0`. Filter it so that you are left with the elements `> 5.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float list = [6.0; 7.0; 8.0; 9.0; 10.0]
```

</details>
</span>
</p>
</div>

## Question 7

Given the list below, return tuples of all consecutive pairs.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: (int * int) list = [(1, -4); (-4, 7); (7, 2); (2, -10)]
```

</details>
</span>
</p>
</div>

## Question 8

Given the list below, return sliding windows of 3 consecutive observations.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int list list = [[1; -4; 7]; [-4; 7; 2]; [7; 2; -10]]
```

</details>
</span>
</p>
</div>

## Question 9

Given the list below, sum all the elements.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int = -4
```

</details>
</span>
</p>
</div>

## Question 10

Given the list below, add `1` to all the elements and then calculate the sum.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int = 1
```

</details>
</span>
</p>
</div>

## Question 11

Given the list below, calculate the `average` of the elements in the list.

```fsharp
[ 1.0; -4.0; 7.0; 2.0; -10.0]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float = -0.8
```

</details>
</span>
</p>
</div>

## Question 12

Given the list below, convert each element to a `decimal` and then calculate the `average` of the elements in the list.

```fsharp
[ 1.0; -4.0; 7.0; 2.0; -10.0]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: decimal = -0.8M {Scale = 1uy;}
```

Since `decimal` is a function that converts to
the `decimal` type, you could also do.
The FSharp linter shouLd show you a blue squiggly
in the above code telling you this.

```
val it: decimal = -0.8M {Scale = 1uy;}
```

</details>
</span>
</p>
</div>

## Question 13

Take a `list` containing floats `1.0 .. 10.0`. Use `List.groupBy` to group the elements based on if they're `>= 5.0`. Then use `List.map` to get the maxiumum element that is `< 5.0` and the minimum value that is `>= 5.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val groupedAboveBelow5: (bool * float list) list =
  [(false, [1.0; 2.0; 3.0; 4.0]); (true, [5.0; 6.0; 7.0; 8.0; 9.0; 10.0])]
val it: float list = [4.0; 5.0]
```

</details>
</span>
</p>
</div>

## Question 14

Take a `list` containing floats `1.0 .. 10.0`. Use functions from the List module to sort it in descending order. Then take the 3rd element of the reversed list and add `7.0` to it.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val descendingList: float list =
  [10.0; 9.0; 8.0; 7.0; 6.0; 5.0; 4.0; 3.0; 2.0; 1.0]
val thirdItem: float = 8.0
val it: float = 15.0
```

</details>
</span>
</p>
</div>

## Question 15

Take this list of lists, add `1.0` to each element of the "inner" lists,
and then concatenate all the inner lists together.

```fsharp
[ [ 1.0; 2.0]
  [ 3.0; 4.0] ]  
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val listsToAdd: float list list = [[1.0; 2.0]; [3.0; 4.0]]
val it: float list = [2.0; 3.0; 4.0; 5.0]
```

v2, this is not a correct answer.
it has not concatenated the inner lists
into one big list

```
val it: float list list = [[2.0; 3.0]; [4.0; 5.0]]
```

v3 and v4 below are correct, the same output as v1

```
val it: float list = [2.0; 3.0; 4.0; 5.0]
```

</details>
</span>
</p>
</div>

## Question 16

Given `returns : ReturnOb list`, calculate the arithmetic average return
for every symbol each month.
Give the result as a `ReturnOb list` where the date is the last date for the symbol
each month.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: ReturnOb list =
  [{ Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Sunday;
                                  DayOfYear = 3;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 1;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637452288000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Return = 0.04724914955 };
   { Symbol = "AAPL"
     Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Wednesday;
                                  DayOfYear = 34;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637479072000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Return = 0.01260338828 };
   { Symbol = "TSLA"
     Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Sunday;
                                  DayOfYear = 3;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 1;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637452288000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Return = -0.05050316065 };
   { Symbol = "TSLA"
     Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Wednesday;
                                  DayOfYear = 34;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637479072000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Return = 0.06336462869 }]
```

</details>
</span>
</p>
</div>

## Question 17

Given `returns : ReturnOb list`, calculate the monthly return
for every symbol each month.
Give the result as a `ReturnOb list` where the date is the last date for the symbol
each month.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val groupsForMonthlyReturn: ((string * int * int) * ReturnOb list) list =
  [(("AAPL", 2021, 1),
    [{ Symbol = "AAPL"
       Date = 1/1/2021 12:00:00 AM
       Return = -0.02993466474 }; { Symbol = "AAPL"
                                    Date = 1/2/2021 12:00:00 AM
                                    Return = -0.01872509079 };
     { Symbol = "AAPL"
       Date = 1/3/2021 12:00:00 AM
       Return = 0.1904072042 }]);
   (("AAPL", 2021, 2),
    [{ Symbol = "AAPL"
       Date = 2/1/2021 12:00:00 AM
       Return = -0.01626157984 }; { Symbol = "AAPL"
                                    Date = 2/2/2021 12:00:00 AM
                                    Return = -0.0767937252 };
     { Symbol = "AAPL"
       Date = 2/3/2021 12:00:00 AM
       Return = 0.1308654699 }]);
   (("TSLA", 2021, 1),
    [{ Symbol = "TSLA"
       Date = 1/1/2021 12:00:00 AM
       Return = -0.1487757845 }; { Symbol = "TSLA"
                                   Date = 1/2/2021 12:00:00 AM
                                   Return = 0.1059620976 };
     { Symbol = "TSLA"
       Date = 1/3/2021 12:00:00 AM
       Return = -0.108695795 }]);
   (("TSLA", 2021, 2),
    [{ Symbol = "TSLA"
       Date = 2/1/2021 12:00:00 AM
       Return = 0.04571273462 }; { Symbol = "TSLA"
                                   Date = 2/2/2021 12:00:00 AM
                                   Return = 0.099311576 };
     { Symbol = "TSLA"
       Date = 2/3/2021 12:00:00 AM
       Return = 0.04506957545 }])]
val exampleSimpleReturns: float list = [0.1; -0.2; 0.3]
val exampleLogReturns: float list =
  [0.0953101798; -0.2231435513; 0.2623642645]
val cumulativeLogReturns: float = 0.134530893
val cumulativeSimpleReturns: float = 0.144
val it: ReturnOb list =
  [{ Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM
     Return = 0.1331495388 }; { Symbol = "AAPL"
                                Date = 2/3/2021 12:00:00 AM
                                Return = 0.02704464906 };
   { Symbol = "TSLA"
     Date = 1/3/2021 12:00:00 AM
     Return = -0.1609068633 }; { Symbol = "TSLA"
                                 Date = 2/3/2021 12:00:00 AM
                                 Return = 0.2013744809 }]
```

</details>
</span>
</p>
</div>

## Question 18

Given `returns : ReturnOb list`, calculate the standard deviation of daily returns
for every symbol each month.
Give the result as a `ValueOb list` where the date in each `ValueOb` is the last date for the symbol
each month.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: ValueOb list =
  [{ Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Sunday;
                                  DayOfYear = 3;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 1;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637452288000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1241051372 };
   { Symbol = "AAPL"
     Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Wednesday;
                                  DayOfYear = 34;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637479072000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.106796419 };
   { Symbol = "TSLA"
     Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Sunday;
                                  DayOfYear = 3;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 1;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637452288000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.136976765 };
   { Symbol = "TSLA"
     Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Wednesday;
                                  DayOfYear = 34;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637479072000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.03113263046 }]
```

</details>
</span>
</p>
</div>

## Question 19

Given `returns : ReturnOb list`, calculate the standard deviation of daily returns
for every symbol using rolling 3 day windows.
Give the result as a `ValueOb list` where the date in each `ValueOb` is the last date for the symbol
in the window.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: ValueOb list =
  [{ Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Sunday;
                                  DayOfYear = 3;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 1;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637452288000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1241051372 };
   { Symbol = "AAPL"
     Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                  Day = 1;
                                  DayOfWeek = Monday;
                                  DayOfYear = 32;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637477344000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1200377524 };
   { Symbol = "AAPL"
     Date = 2/2/2021 12:00:00 AM {Date = 2/2/2021 12:00:00 AM;
                                  Day = 2;
                                  DayOfWeek = Tuesday;
                                  DayOfYear = 33;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637478208000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1401026193 };
   { Symbol = "AAPL"
     Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Wednesday;
                                  DayOfYear = 34;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637479072000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.106796419 };
   { Symbol = "TSLA"
     Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Sunday;
                                  DayOfYear = 3;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 1;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637452288000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.136976765 };
   { Symbol = "TSLA"
     Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                  Day = 1;
                                  DayOfWeek = Monday;
                                  DayOfYear = 32;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637477344000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1107173508 };
   { Symbol = "TSLA"
     Date = 2/2/2021 12:00:00 AM {Date = 2/2/2021 12:00:00 AM;
                                  Day = 2;
                                  DayOfWeek = Tuesday;
                                  DayOfYear = 33;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637478208000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1079983767 };
   { Symbol = "TSLA"
     Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Wednesday;
                                  DayOfYear = 34;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637479072000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.03113263046 }]
```

Breaking this answer down,
If you're unsure, it's helpful to work through things step by step.
then build up from there.

```
val groups: (string * ReturnOb list) list =
  [("AAPL",
    [{ Symbol = "AAPL"
       Date = 1/1/2021 12:00:00 AM
       Return = -0.02993466474 }; { Symbol = "AAPL"
                                    Date = 1/2/2021 12:00:00 AM
                                    Return = -0.01872509079 };
     { Symbol = "AAPL"
       Date = 1/3/2021 12:00:00 AM
       Return = 0.1904072042 }; { Symbol = "AAPL"
                                  Date = 2/1/2021 12:00:00 AM
                                  Return = -0.01626157984 };
     { Symbol = "AAPL"
       Date = 2/2/2021 12:00:00 AM
       Return = -0.0767937252 }; { Symbol = "AAPL"
                                   Date = 2/3/2021 12:00:00 AM
                                   Return = 0.1308654699 }]);
   ("TSLA",
    [{ Symbol = "TSLA"
       Date = 1/1/2021 12:00:00 AM
       Return = -0.1487757845 }; { Symbol = "TSLA"
                                   Date = 1/2/2021 12:00:00 AM
                                   Return = 0.1059620976 };
     { Symbol = "TSLA"
       Date = 1/3/2021 12:00:00 AM
       Return = -0.108695795 }; { Symbol = "TSLA"
                                  Date = 2/1/2021 12:00:00 AM
                                  Return = 0.04571273462 };
     { Symbol = "TSLA"
       Date = 2/2/2021 12:00:00 AM
       Return = 0.099311576 }; { Symbol = "TSLA"
                                 Date = 2/3/2021 12:00:00 AM
                                 Return = 0.04506957545 }])]
```

```
val firstGroup: string * ReturnOb list =
  ("AAPL",
   [{ Symbol = "AAPL"
      Date = 1/1/2021 12:00:00 AM
      Return = -0.02993466474 }; { Symbol = "AAPL"
                                   Date = 1/2/2021 12:00:00 AM
                                   Return = -0.01872509079 };
    { Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM
      Return = 0.1904072042 }; { Symbol = "AAPL"
                                 Date = 2/1/2021 12:00:00 AM
                                 Return = -0.01626157984 };
    { Symbol = "AAPL"
      Date = 2/2/2021 12:00:00 AM
      Return = -0.0767937252 }; { Symbol = "AAPL"
                                  Date = 2/3/2021 12:00:00 AM
                                  Return = 0.1308654699 }])
val firstSymbol: string = "AAPL"
val firstObs: ReturnOb list =
  [{ Symbol = "AAPL"
     Date = 1/1/2021 12:00:00 AM
     Return = -0.02993466474 }; { Symbol = "AAPL"
                                  Date = 1/2/2021 12:00:00 AM
                                  Return = -0.01872509079 };
   { Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM
     Return = 0.1904072042 }; { Symbol = "AAPL"
                                Date = 2/1/2021 12:00:00 AM
                                Return = -0.01626157984 };
   { Symbol = "AAPL"
     Date = 2/2/2021 12:00:00 AM
     Return = -0.0767937252 }; { Symbol = "AAPL"
                                 Date = 2/3/2021 12:00:00 AM
                                 Return = 0.1308654699 }]
val windowedFirstObs: ReturnOb list list =
  [[{ Symbol = "AAPL"
      Date = 1/1/2021 12:00:00 AM
      Return = -0.02993466474 }; { Symbol = "AAPL"
                                   Date = 1/2/2021 12:00:00 AM
                                   Return = -0.01872509079 };
    { Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM
      Return = 0.1904072042 }];
   [{ Symbol = "AAPL"
      Date = 1/2/2021 12:00:00 AM
      Return = -0.01872509079 }; { Symbol = "AAPL"
                                   Date = 1/3/2021 12:00:00 AM
                                   Return = 0.1904072042 };
    { Symbol = "AAPL"
      Date = 2/1/2021 12:00:00 AM
      Return = -0.01626157984 }];
   [{ Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM
      Return = 0.1904072042 }; { Symbol = "AAPL"
                                 Date = 2/1/2021 12:00:00 AM
                                 Return = -0.01626157984 };
    { Symbol = "AAPL"
      Date = 2/2/2021 12:00:00 AM
      Return = -0.0767937252 }];
   [{ Symbol = "AAPL"
      Date = 2/1/2021 12:00:00 AM
      Return = -0.01626157984 }; { Symbol = "AAPL"
                                   Date = 2/2/2021 12:00:00 AM
                                   Return = -0.0767937252 };
    { Symbol = "AAPL"
      Date = 2/3/2021 12:00:00 AM
      Return = 0.1308654699 }]]
val firstWindow: ReturnOb list =
  [{ Symbol = "AAPL"
     Date = 1/1/2021 12:00:00 AM
     Return = -0.02993466474 }; { Symbol = "AAPL"
                                  Date = 1/2/2021 12:00:00 AM
                                  Return = -0.01872509079 };
   { Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM
     Return = 0.1904072042 }]
val lastDayOfFirstWindow: ReturnOb = { Symbol = "AAPL"
                                       Date = 1/3/2021 12:00:00 AM
                                       Return = 0.1904072042 }
val firstWindowReturnStdDev: float = 0.1241051372
val firstWindowResult: ValueOb = { Symbol = "AAPL"
                                   Date = 1/3/2021 12:00:00 AM
                                   Value = 0.1241051372 }
```

Now take the inner-most code operating on a single window
and make a function by copying and pasting inside a function.
often using more general variable names

```
val resultForWindow: window: ReturnOb list -> ValueOb
```

test it on your window

```
val firstWindowFunctionResult: ValueOb = { Symbol = "AAPL"
                                           Date = 1/3/2021 12:00:00 AM
                                           Value = 0.1241051372 }
```

check

```
val it: bool = true
```

now a function to create the windows

```
val createWindows: days: ReturnOb list -> ReturnOb list list
```

check

```
val it: bool = true
```

so now we can do

```
val it: ValueOb list =
  [{ Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Sunday;
                                  DayOfYear = 3;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 1;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637452288000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1241051372 };
   { Symbol = "AAPL"
     Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                  Day = 1;
                                  DayOfWeek = Monday;
                                  DayOfYear = 32;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637477344000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1200377524 };
   { Symbol = "AAPL"
     Date = 2/2/2021 12:00:00 AM {Date = 2/2/2021 12:00:00 AM;
                                  Day = 2;
                                  DayOfWeek = Tuesday;
                                  DayOfYear = 33;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637478208000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1401026193 };
   { Symbol = "AAPL"
     Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Wednesday;
                                  DayOfYear = 34;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637479072000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.106796419 }]
```

Cool, now first obs was the obs from the first group.
we could do function to operate on a group.
our group is a tuple of `(string,ReturnObs list)`.
We're not going to use the `string` variable, so we'll preface it
with _ to let the compiler know we're leaving it out o purpose.
the _ is not necessary but it's good practice

```
val resultsForGroup: _symbol: 'a * xs: ReturnOb list -> ValueOb list
```

test it on the first group

```
val it: ValueOb list =
  [{ Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Sunday;
                                  DayOfYear = 3;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 1;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637452288000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1241051372 };
   { Symbol = "AAPL"
     Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                  Day = 1;
                                  DayOfWeek = Monday;
                                  DayOfYear = 32;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637477344000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1200377524 };
   { Symbol = "AAPL"
     Date = 2/2/2021 12:00:00 AM {Date = 2/2/2021 12:00:00 AM;
                                  Day = 2;
                                  DayOfWeek = Tuesday;
                                  DayOfYear = 33;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637478208000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1401026193 };
   { Symbol = "AAPL"
     Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Wednesday;
                                  DayOfYear = 34;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637479072000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.106796419 }]
```

now make the group and apply my
group function to each group

```
val resultsForEachGroup: ValueOb list list =
  [[{ Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM
      Value = 0.1241051372 }; { Symbol = "AAPL"
                                Date = 2/1/2021 12:00:00 AM
                                Value = 0.1200377524 };
    { Symbol = "AAPL"
      Date = 2/2/2021 12:00:00 AM
      Value = 0.1401026193 }; { Symbol = "AAPL"
                                Date = 2/3/2021 12:00:00 AM
                                Value = 0.106796419 }];
   [{ Symbol = "TSLA"
      Date = 1/3/2021 12:00:00 AM
      Value = 0.136976765 }; { Symbol = "TSLA"
                               Date = 2/1/2021 12:00:00 AM
                               Value = 0.1107173508 };
    { Symbol = "TSLA"
      Date = 2/2/2021 12:00:00 AM
      Value = 0.1079983767 }; { Symbol = "TSLA"
                                Date = 2/3/2021 12:00:00 AM
                                Value = 0.03113263046 }]]
```

Okay, but this is an list of `ValueOb list` (that's what `ValueOb list list` means).
What happened is that I had an list of groups, and then I transformed each group.
so it's still one result per group. For instance

```
val it: ValueOb list =
  [{ Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Sunday;
                                  DayOfYear = 3;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 1;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637452288000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1241051372 };
   { Symbol = "AAPL"
     Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                  Day = 1;
                                  DayOfWeek = Monday;
                                  DayOfYear = 32;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637477344000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1200377524 };
   { Symbol = "AAPL"
     Date = 2/2/2021 12:00:00 AM {Date = 2/2/2021 12:00:00 AM;
                                  Day = 2;
                                  DayOfWeek = Tuesday;
                                  DayOfYear = 33;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637478208000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1401026193 };
   { Symbol = "AAPL"
     Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Wednesday;
                                  DayOfYear = 34;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637479072000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.106796419 }]
```

is the first group of results

```
val it: ValueOb list =
  [{ Symbol = "TSLA"
     Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Sunday;
                                  DayOfYear = 3;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 1;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637452288000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.136976765 };
   { Symbol = "TSLA"
     Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                  Day = 1;
                                  DayOfWeek = Monday;
                                  DayOfYear = 32;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637477344000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1107173508 };
   { Symbol = "TSLA"
     Date = 2/2/2021 12:00:00 AM {Date = 2/2/2021 12:00:00 AM;
                                  Day = 2;
                                  DayOfWeek = Tuesday;
                                  DayOfYear = 33;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637478208000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.1079983767 };
   { Symbol = "TSLA"
     Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                  Day = 3;
                                  DayOfWeek = Wednesday;
                                  DayOfYear = 34;
                                  Hour = 0;
                                  Kind = Unspecified;
                                  Microsecond = 0;
                                  Millisecond = 0;
                                  Minute = 0;
                                  Month = 2;
                                  Nanosecond = 0;
                                  Second = 0;
                                  Ticks = 637479072000000000L;
                                  TimeOfDay = 00:00:00;
                                  Year = 2021;}
     Value = 0.03113263046 }]
```

is the second group. I don't want an list of lists.
I just want one list of value obs. So `concat` them.

```
val resultsForEachGroupConcatenated: ValueOb list =
  [{ Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM
     Value = 0.1241051372 }; { Symbol = "AAPL"
                               Date = 2/1/2021 12:00:00 AM
                               Value = 0.1200377524 };
   { Symbol = "AAPL"
     Date = 2/2/2021 12:00:00 AM
     Value = 0.1401026193 }; { Symbol = "AAPL"
                               Date = 2/3/2021 12:00:00 AM
                               Value = 0.106796419 };
   { Symbol = "TSLA"
     Date = 1/3/2021 12:00:00 AM
     Value = 0.136976765 }; { Symbol = "TSLA"
                              Date = 2/1/2021 12:00:00 AM
                              Value = 0.1107173508 };
   { Symbol = "TSLA"
     Date = 2/2/2021 12:00:00 AM
     Value = 0.1079983767 }; { Symbol = "TSLA"
                               Date = 2/3/2021 12:00:00 AM
                               Value = 0.03113263046 }]
```

what's the first thing in the list?

```
val it: ValueOb = { Symbol = "AAPL"
                    Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                                 Day = 3;
                                                 DayOfWeek = Sunday;
                                                 DayOfYear = 3;
                                                 Hour = 0;
                                                 Kind = Unspecified;
                                                 Microsecond = 0;
                                                 Millisecond = 0;
                                                 Minute = 0;
                                                 Month = 1;
                                                 Nanosecond = 0;
                                                 Second = 0;
                                                 Ticks = 637452288000000000L;
                                                 TimeOfDay = 00:00:00;
                                                 Year = 2021;}
                    Value = 0.1241051372 }
```

`Collect` does the `map` and `concat` in one step.

```
val resultsForEachGroupCollected: ValueOb list =
  [{ Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM
     Value = 0.1241051372 }; { Symbol = "AAPL"
                               Date = 2/1/2021 12:00:00 AM
                               Value = 0.1200377524 };
   { Symbol = "AAPL"
     Date = 2/2/2021 12:00:00 AM
     Value = 0.1401026193 }; { Symbol = "AAPL"
                               Date = 2/3/2021 12:00:00 AM
                               Value = 0.106796419 };
   { Symbol = "TSLA"
     Date = 1/3/2021 12:00:00 AM
     Value = 0.136976765 }; { Symbol = "TSLA"
                              Date = 2/1/2021 12:00:00 AM
                              Value = 0.1107173508 };
   { Symbol = "TSLA"
     Date = 2/2/2021 12:00:00 AM
     Value = 0.1079983767 }; { Symbol = "TSLA"
                               Date = 2/3/2021 12:00:00 AM
                               Value = 0.03113263046 }]
```

check, this should evaluate to `true`

```
val it: bool = true
```

why did I write the answer using an anonymous function instead of functions like this?
I use reusable functions for something I'm going to use multiple times.
If it's something I'll do once, and it's not too many lines, then I use
the anonymous lambda function. As you get more experience, you can code using
the type signatures to tell you what everything is. And I don't actually
have to running it step by step.
however, starting out especially, I think you'll find it helpful
to kinda break things down like I did here.
Another way you can do it, similar to the first answer using
an anonymous lambda function, but now we'll do it with fewer
nested lists by concatenating/collecting the windows
into the parent list before doing the standard deviations.

```
val m2Groups: (string * ReturnOb list) list =
  [("AAPL",
    [{ Symbol = "AAPL"
       Date = 1/1/2021 12:00:00 AM
       Return = -0.02993466474 }; { Symbol = "AAPL"
                                    Date = 1/2/2021 12:00:00 AM
                                    Return = -0.01872509079 };
     { Symbol = "AAPL"
       Date = 1/3/2021 12:00:00 AM
       Return = 0.1904072042 }; { Symbol = "AAPL"
                                  Date = 2/1/2021 12:00:00 AM
                                  Return = -0.01626157984 };
     { Symbol = "AAPL"
       Date = 2/2/2021 12:00:00 AM
       Return = -0.0767937252 }; { Symbol = "AAPL"
                                   Date = 2/3/2021 12:00:00 AM
                                   Return = 0.1308654699 }]);
   ("TSLA",
    [{ Symbol = "TSLA"
       Date = 1/1/2021 12:00:00 AM
       Return = -0.1487757845 }; { Symbol = "TSLA"
                                   Date = 1/2/2021 12:00:00 AM
                                   Return = 0.1059620976 };
     { Symbol = "TSLA"
       Date = 1/3/2021 12:00:00 AM
       Return = -0.108695795 }; { Symbol = "TSLA"
                                  Date = 2/1/2021 12:00:00 AM
                                  Return = 0.04571273462 };
     { Symbol = "TSLA"
       Date = 2/2/2021 12:00:00 AM
       Return = 0.099311576 }; { Symbol = "TSLA"
                                 Date = 2/3/2021 12:00:00 AM
                                 Return = 0.04506957545 }])]
val m2GroupsOfWindows: ReturnOb list list list =
  [[[{ Symbol = "AAPL"
       Date = 1/1/2021 12:00:00 AM
       Return = -0.02993466474 }; { Symbol = "AAPL"
                                    Date = 1/2/2021 12:00:00 AM
                                    Return = -0.01872509079 };
     { Symbol = "AAPL"
       Date = 1/3/2021 12:00:00 AM
       Return = 0.1904072042 }];
    [{ Symbol = "AAPL"
       Date = 1/2/2021 12:00:00 AM
       Return = -0.01872509079 }; { Symbol = "AAPL"
                                    Date = 1/3/2021 12:00:00 AM
                                    Return = 0.1904072042 };
     { Symbol = "AAPL"
       Date = 2/1/2021 12:00:00 AM
       Return = -0.01626157984 }];
    [{ Symbol = "AAPL"
       Date = 1/3/2021 12:00:00 AM
       Return = 0.1904072042 }; { Symbol = "AAPL"
                                  Date = 2/1/2021 12:00:00 AM
                                  Return = -0.01626157984 };
     { Symbol = "AAPL"
       Date = 2/2/2021 12:00:00 AM
       Return = -0.0767937252 }];
    [{ Symbol = "AAPL"
       Date = 2/1/2021 12:00:00 AM
       Return = -0.01626157984 }; { Symbol = "AAPL"
                                    Date = 2/2/2021 12:00:00 AM
                                    Return = -0.0767937252 };
     { Symbol = "AAPL"
       Date = 2/3/2021 12:00:00 AM
       Return = 0.1308654699 }]];
   [[{ Symbol = "TSLA"
       Date = 1/1/2021 12:00:00 AM
       Return = -0.1487757845 }; { Symbol = "TSLA"
                                   Date = 1/2/2021 12:00:00 AM
                                   Return = 0.1059620976 };
     { Symbol = "TSLA"
       Date = 1/3/2021 12:00:00 AM
       Return = -0.108695795 }];
    [{ Symbol = "TSLA"
       Date = 1/2/2021 12:00:00 AM
       Return = 0.1059620976 }; { Symbol = "TSLA"
                                  Date = 1/3/2021 12:00:00 AM
                                  Return = -0.108695795 };
     { Symbol = "TSLA"
       Date = 2/1/2021 12:00:00 AM
       Return = 0.04571273462 }];
    [{ Symbol = "TSLA"
       Date = 1/3/2021 12:00:00 AM
       Return = -0.108695795 }; { Symbol = "TSLA"
                                  Date = 2/1/2021 12:00:00 AM
                                  Return = 0.04571273462 };
     { Symbol = "TSLA"
       Date = 2/2/2021 12:00:00 AM
       Return = 0.099311576 }];
    [{ Symbol = "TSLA"
       Date = 2/1/2021 12:00:00 AM
       Return = 0.04571273462 }; { Symbol = "TSLA"
                                   Date = 2/2/2021 12:00:00 AM
                                   Return = 0.099311576 };
     { Symbol = "TSLA"
       Date = 2/3/2021 12:00:00 AM
       Return = 0.04506957545 }]]]
```

first group of windows

```
val it: ReturnOb list list =
  [[{ Symbol = "AAPL"
      Date = 1/1/2021 12:00:00 AM {Date = 1/1/2021 12:00:00 AM;
                                   Day = 1;
                                   DayOfWeek = Friday;
                                   DayOfYear = 1;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637450560000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.02993466474 };
    { Symbol = "AAPL"
      Date = 1/2/2021 12:00:00 AM {Date = 1/2/2021 12:00:00 AM;
                                   Day = 2;
                                   DayOfWeek = Saturday;
                                   DayOfYear = 2;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637451424000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.01872509079 };
    { Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                   Day = 3;
                                   DayOfWeek = Sunday;
                                   DayOfYear = 3;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637452288000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.1904072042 }];
   [{ Symbol = "AAPL"
      Date = 1/2/2021 12:00:00 AM {Date = 1/2/2021 12:00:00 AM;
                                   Day = 2;
                                   DayOfWeek = Saturday;
                                   DayOfYear = 2;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637451424000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.01872509079 };
    { Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                   Day = 3;
                                   DayOfWeek = Sunday;
                                   DayOfYear = 3;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637452288000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.1904072042 };
    { Symbol = "AAPL"
      Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                   Day = 1;
                                   DayOfWeek = Monday;
                                   DayOfYear = 32;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637477344000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.01626157984 }];
   [{ Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                   Day = 3;
                                   DayOfWeek = Sunday;
                                   DayOfYear = 3;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637452288000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.1904072042 };
    { Symbol = "AAPL"
      Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                   Day = 1;
                                   DayOfWeek = Monday;
                                   DayOfYear = 32;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637477344000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.01626157984 };
    { Symbol = "AAPL"
      Date = 2/2/2021 12:00:00 AM {Date = 2/2/2021 12:00:00 AM;
                                   Day = 2;
                                   DayOfWeek = Tuesday;
                                   DayOfYear = 33;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637478208000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.0767937252 }];
   [{ Symbol = "AAPL"
      Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                   Day = 1;
                                   DayOfWeek = Monday;
                                   DayOfYear = 32;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637477344000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.01626157984 };
    { Symbol = "AAPL"
      Date = 2/2/2021 12:00:00 AM {Date = 2/2/2021 12:00:00 AM;
                                   Day = 2;
                                   DayOfWeek = Tuesday;
                                   DayOfYear = 33;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637478208000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.0767937252 };
    { Symbol = "AAPL"
      Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                   Day = 3;
                                   DayOfWeek = Wednesday;
                                   DayOfYear = 34;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637479072000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.1308654699 }]]
```

second group of windows

```
val it: ReturnOb list list =
  [[{ Symbol = "TSLA"
      Date = 1/1/2021 12:00:00 AM {Date = 1/1/2021 12:00:00 AM;
                                   Day = 1;
                                   DayOfWeek = Friday;
                                   DayOfYear = 1;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637450560000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.1487757845 };
    { Symbol = "TSLA"
      Date = 1/2/2021 12:00:00 AM {Date = 1/2/2021 12:00:00 AM;
                                   Day = 2;
                                   DayOfWeek = Saturday;
                                   DayOfYear = 2;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637451424000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.1059620976 };
    { Symbol = "TSLA"
      Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                   Day = 3;
                                   DayOfWeek = Sunday;
                                   DayOfYear = 3;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637452288000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.108695795 }];
   [{ Symbol = "TSLA"
      Date = 1/2/2021 12:00:00 AM {Date = 1/2/2021 12:00:00 AM;
                                   Day = 2;
                                   DayOfWeek = Saturday;
                                   DayOfYear = 2;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637451424000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.1059620976 };
    { Symbol = "TSLA"
      Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                   Day = 3;
                                   DayOfWeek = Sunday;
                                   DayOfYear = 3;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637452288000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.108695795 };
    { Symbol = "TSLA"
      Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                   Day = 1;
                                   DayOfWeek = Monday;
                                   DayOfYear = 32;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637477344000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.04571273462 }];
   [{ Symbol = "TSLA"
      Date = 1/3/2021 12:00:00 AM {Date = 1/3/2021 12:00:00 AM;
                                   Day = 3;
                                   DayOfWeek = Sunday;
                                   DayOfYear = 3;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 1;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637452288000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = -0.108695795 };
    { Symbol = "TSLA"
      Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                   Day = 1;
                                   DayOfWeek = Monday;
                                   DayOfYear = 32;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637477344000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.04571273462 };
    { Symbol = "TSLA"
      Date = 2/2/2021 12:00:00 AM {Date = 2/2/2021 12:00:00 AM;
                                   Day = 2;
                                   DayOfWeek = Tuesday;
                                   DayOfYear = 33;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637478208000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.099311576 }];
   [{ Symbol = "TSLA"
      Date = 2/1/2021 12:00:00 AM {Date = 2/1/2021 12:00:00 AM;
                                   Day = 1;
                                   DayOfWeek = Monday;
                                   DayOfYear = 32;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637477344000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.04571273462 };
    { Symbol = "TSLA"
      Date = 2/2/2021 12:00:00 AM {Date = 2/2/2021 12:00:00 AM;
                                   Day = 2;
                                   DayOfWeek = Tuesday;
                                   DayOfYear = 33;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637478208000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.099311576 };
    { Symbol = "TSLA"
      Date = 2/3/2021 12:00:00 AM {Date = 2/3/2021 12:00:00 AM;
                                   Day = 3;
                                   DayOfWeek = Wednesday;
                                   DayOfYear = 34;
                                   Hour = 0;
                                   Kind = Unspecified;
                                   Microsecond = 0;
                                   Millisecond = 0;
                                   Minute = 0;
                                   Month = 2;
                                   Nanosecond = 0;
                                   Second = 0;
                                   Ticks = 637479072000000000L;
                                   TimeOfDay = 00:00:00;
                                   Year = 2021;}
      Return = 0.04506957545 }]]
```

Now concatenate the windows.

```
val m2GroupsOfWindowsConcatenated: ReturnOb list list =
  [[{ Symbol = "AAPL"
      Date = 1/1/2021 12:00:00 AM
      Return = -0.02993466474 }; { Symbol = "AAPL"
                                   Date = 1/2/2021 12:00:00 AM
                                   Return = -0.01872509079 };
    { Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM
      Return = 0.1904072042 }];
   [{ Symbol = "AAPL"
      Date = 1/2/2021 12:00:00 AM
      Return = -0.01872509079 }; { Symbol = "AAPL"
                                   Date = 1/3/2021 12:00:00 AM
                                   Return = 0.1904072042 };
    { Symbol = "AAPL"
      Date = 2/1/2021 12:00:00 AM
      Return = -0.01626157984 }];
   [{ Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM
      Return = 0.1904072042 }; { Symbol = "AAPL"
                                 Date = 2/1/2021 12:00:00 AM
                                 Return = -0.01626157984 };
    { Symbol = "AAPL"
      Date = 2/2/2021 12:00:00 AM
      Return = -0.0767937252 }];
   [{ Symbol = "AAPL"
      Date = 2/1/2021 12:00:00 AM
      Return = -0.01626157984 }; { Symbol = "AAPL"
                                   Date = 2/2/2021 12:00:00 AM
                                   Return = -0.0767937252 };
    { Symbol = "AAPL"
      Date = 2/3/2021 12:00:00 AM
      Return = 0.1308654699 }];
   [{ Symbol = "TSLA"
      Date = 1/1/2021 12:00:00 AM
      Return = -0.1487757845 }; { Symbol = "TSLA"
                                  Date = 1/2/2021 12:00:00 AM
                                  Return = 0.1059620976 };
    { Symbol = "TSLA"
      Date = 1/3/2021 12:00:00 AM
      Return = -0.108695795 }];
   [{ Symbol = "TSLA"
      Date = 1/2/2021 12:00:00 AM
      Return = 0.1059620976 }; { Symbol = "TSLA"
                                 Date = 1/3/2021 12:00:00 AM
                                 Return = -0.108695795 };
    { Symbol = "TSLA"
      Date = 2/1/2021 12:00:00 AM
      Return = 0.04571273462 }];
   [{ Symbol = "TSLA"
      Date = 1/3/2021 12:00:00 AM
      Return = -0.108695795 }; { Symbol = "TSLA"
                                 Date = 2/1/2021 12:00:00 AM
                                 Return = 0.04571273462 };
    { Symbol = "TSLA"
      Date = 2/2/2021 12:00:00 AM
      Return = 0.099311576 }];
   [{ Symbol = "TSLA"
      Date = 2/1/2021 12:00:00 AM
      Return = 0.04571273462 }; { Symbol = "TSLA"
                                  Date = 2/2/2021 12:00:00 AM
                                  Return = 0.099311576 };
    { Symbol = "TSLA"
      Date = 2/3/2021 12:00:00 AM
      Return = 0.04506957545 }]]
```

same as if I'd used collect instead of map and then concat

```
val m2GroupsOfWindowsCollected: ReturnOb list list =
  [[{ Symbol = "AAPL"
      Date = 1/1/2021 12:00:00 AM
      Return = -0.02993466474 }; { Symbol = "AAPL"
                                   Date = 1/2/2021 12:00:00 AM
                                   Return = -0.01872509079 };
    { Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM
      Return = 0.1904072042 }];
   [{ Symbol = "AAPL"
      Date = 1/2/2021 12:00:00 AM
      Return = -0.01872509079 }; { Symbol = "AAPL"
                                   Date = 1/3/2021 12:00:00 AM
                                   Return = 0.1904072042 };
    { Symbol = "AAPL"
      Date = 2/1/2021 12:00:00 AM
      Return = -0.01626157984 }];
   [{ Symbol = "AAPL"
      Date = 1/3/2021 12:00:00 AM
      Return = 0.1904072042 }; { Symbol = "AAPL"
                                 Date = 2/1/2021 12:00:00 AM
                                 Return = -0.01626157984 };
    { Symbol = "AAPL"
      Date = 2/2/2021 12:00:00 AM
      Return = -0.0767937252 }];
   [{ Symbol = "AAPL"
      Date = 2/1/2021 12:00:00 AM
      Return = -0.01626157984 }; { Symbol = "AAPL"
                                   Date = 2/2/2021 12:00:00 AM
                                   Return = -0.0767937252 };
    { Symbol = "AAPL"
      Date = 2/3/2021 12:00:00 AM
      Return = 0.1308654699 }];
   [{ Symbol = "TSLA"
      Date = 1/1/2021 12:00:00 AM
      Return = -0.1487757845 }; { Symbol = "TSLA"
                                  Date = 1/2/2021 12:00:00 AM
                                  Return = 0.1059620976 };
    { Symbol = "TSLA"
      Date = 1/3/2021 12:00:00 AM
      Return = -0.108695795 }];
   [{ Symbol = "TSLA"
      Date = 1/2/2021 12:00:00 AM
      Return = 0.1059620976 }; { Symbol = "TSLA"
                                 Date = 1/3/2021 12:00:00 AM
                                 Return = -0.108695795 };
    { Symbol = "TSLA"
      Date = 2/1/2021 12:00:00 AM
      Return = 0.04571273462 }];
   [{ Symbol = "TSLA"
      Date = 1/3/2021 12:00:00 AM
      Return = -0.108695795 }; { Symbol = "TSLA"
                                 Date = 2/1/2021 12:00:00 AM
                                 Return = 0.04571273462 };
    { Symbol = "TSLA"
      Date = 2/2/2021 12:00:00 AM
      Return = 0.099311576 }];
   [{ Symbol = "TSLA"
      Date = 2/1/2021 12:00:00 AM
      Return = 0.04571273462 }; { Symbol = "TSLA"
                                  Date = 2/2/2021 12:00:00 AM
                                  Return = 0.099311576 };
    { Symbol = "TSLA"
      Date = 2/3/2021 12:00:00 AM
      Return = 0.04506957545 }]]
```

compare them

```
val m2FirstConcatenated: ReturnOb list =
  [{ Symbol = "AAPL"
     Date = 1/1/2021 12:00:00 AM
     Return = -0.02993466474 }; { Symbol = "AAPL"
                                  Date = 1/2/2021 12:00:00 AM
                                  Return = -0.01872509079 };
   { Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM
     Return = 0.1904072042 }]
val m2FirstCollected: ReturnOb list =
  [{ Symbol = "AAPL"
     Date = 1/1/2021 12:00:00 AM
     Return = -0.02993466474 }; { Symbol = "AAPL"
                                  Date = 1/2/2021 12:00:00 AM
                                  Return = -0.01872509079 };
   { Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM
     Return = 0.1904072042 }]
val it: bool = true
```

If they're not true, make sure they're sorted the same before you take the first obs.

Now, standard deviations of the windows' returns

```
val m2Result: ValueOb list =
  [{ Symbol = "AAPL"
     Date = 1/3/2021 12:00:00 AM
     Value = 0.1241051372 }; { Symbol = "AAPL"
                               Date = 2/1/2021 12:00:00 AM
                               Value = 0.1200377524 };
   { Symbol = "AAPL"
     Date = 2/2/2021 12:00:00 AM
     Value = 0.1401026193 }; { Symbol = "AAPL"
                               Date = 2/3/2021 12:00:00 AM
                               Value = 0.106796419 };
   { Symbol = "TSLA"
     Date = 1/3/2021 12:00:00 AM
     Value = 0.136976765 }; { Symbol = "TSLA"
                              Date = 2/1/2021 12:00:00 AM
                              Value = 0.1107173508 };
   { Symbol = "TSLA"
     Date = 2/2/2021 12:00:00 AM
     Value = 0.1079983767 }; { Symbol = "TSLA"
                               Date = 2/3/2021 12:00:00 AM
                               Value = 0.03113263046 }]
```

</details>
</span>
</p>
</div>

*)

