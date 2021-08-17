(**
---
title: Volatility Timing Part 1
category: Practice Quizzes
categoryindex: 1
index: 6
---
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
## Question 1
Given the list below, filter the list so that only numbers greater than `2` remain.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: filter, define-output: filter ***)

[ 1; -4; 7; 2; -10]
|> List.filter(fun x -> x > 2)

(*** condition:html, include:filter ***)
(*** condition:html, include-fsi-output:filter ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 2
Given the list below, take elements until you find one that is greater than `4`.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: takeWhile, define-output: takeWhile ***)

[ 1; -4; 7; 2; -10]
|> List.takeWhile(fun x -> x <= 4)

(*** condition:html, include:takeWhile ***)
(*** condition:html, include-fsi-output:takeWhile ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 3
Given the list below, take elements until you find one that is greater than `4`.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: skipWhile, define-output: skipWhile ***)

[ 1; -4; 7; 2; -10]
|> List.skipWhile(fun x -> x <= 4)

(*** condition:html, include:skipWhile ***)
(*** condition:html, include-fsi-output:skipWhile ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 4
Given the list below, return tuples of all consecutive pairs.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: pairwise, define-output: pairwise ***)

[ 1; -4; 7; 2; -10]
|> List.pairwise

(*** condition:html, include:pairwise ***)
(*** condition:html, include-fsi-output:pairwise ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 5
Given the list below, return sliding windows of 3 consecutive observations.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: windowed, define-output: windowed ***)

[ 1; -4; 7; 2; -10]
|> List.windowed 3

(*** condition:html, include:windowed ***)
(*** condition:html, include-fsi-output:windowed ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 6
Given the list below, use `scan` to return the intermediate and final cumulative sums.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: scan, define-output: scan ***)

(0, [ 1; -4; 7; 2; -10])
||> List.scan (fun acc x -> acc + x) 

(*** condition:html, include:scan ***)
(*** condition:html, include-fsi-output:scan ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 7
Given the list below, use `fold` to return the final sum.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: fold, define-output: fold ***)

(0, [ 1; -4; 7; 2; -10])
||> List.fold (fun acc x -> acc + x) 

(*** condition:html, include:fold ***)
(*** condition:html, include-fsi-output:fold ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 8
Given the list below, use `mapFold` to return the intermediate and final cumulative sums.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: mapFold1, define-output: mapFold1 ***)

(0, [ 1; -4; 7; 2; -10])
||> List.mapFold (fun acc x -> acc + x, acc + x)

(*** condition:html, include:mapFold1 ***)
(*** condition:html, include-fsi-output:mapFold1 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 9
Given the list below, use `mapFold` to return a tuple of

1. A new list in which each element of the original list is transformed by adding `1` to it and then converted into a `string`.
2. The final cumulative sums of the list elements.

```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: mapFold2, define-output: mapFold2 ***)

(0, [ 1; -4; 7; 2; -10])
||> List.mapFold (fun acc x -> string (x + 1), acc + x) 

(*** condition:html, include:mapFold2 ***)
(*** condition:html, include-fsi-output:mapFold2 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 10
Given the list below, use `mapFold` to return a tuple of

1. The list of records with the `Y` field in each record updated to `Y+1`
2. The sum of the `Y` fields.

*)

type R1 = { X : string; Y : int }

let r1xs =
    [ { X = "a"; Y = 1 }
      { X = "b"; Y = -4 }
      { X = "c"; Y = 7 } 
      { X = "d"; Y = 2 }
      { X = "e"; Y = -10 }]

(*** include-it-raw:preDetails ***)
(*** define: mapFold3, define-output: mapFold3 ***)

(0, r1xs)
||> List.mapFold (fun acc x -> { x with Y = x.Y+1}, acc + x.Y) 

(*** condition:html, include:mapFold3 ***)
(*** condition:html, include-fsi-output:mapFold3 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 11
Given the list below, sum all the elements.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: sum, define-output: sum ***)

[ 1; -4; 7; 2; -10]
|> List.sum

(*** condition:html, include:sum ***)
(*** condition:html, include-fsi-output:sum ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 12
Given the list below, add `1` to all the elements and then calculate the sum.
```fsharp
[ 1; -4; 7; 2; -10]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: sumBy, define-output: sumBy ***)

[ 1; -4; 7; 2; -10]
|> List.sumBy(fun x -> x + 1)

(*** condition:html, include:sumBy ***)
(*** condition:html, include-fsi-output:sumBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 13
Given the list below, calculate the `average` of the elements in the list.
```fsharp
[ 1.0; -4.0; 7.0; 2.0; -10.0]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: average, define-output: average ***)

[ 1.0; -4.0; 7.0; 2.0; -10.0]
|> List.average

(*** condition:html, include:average ***)
(*** condition:html, include-fsi-output:average ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.



(**
## Question 14
Given the list below, convert each element to a `decimal` and then calculate the `average` of the elements in the list.

```fsharp
[ 1.0; -4.0; 7.0; 2.0; -10.0]
```
*)

(*** include-it-raw:preDetails ***)
(*** define: averageBy, define-output: averageBy ***)

[ 1.0; -4.0; 7.0; 2.0; -10.0]
|> List.averageBy(fun x -> decimal x)

// Since decimal is a function that converts to
// the decimal type, you could also do.
// The FSharp linter shouLd show you a blue squiggly
// in the above code telling you this.
[ 1.0; -4.0; 7.0; 2.0; -10.0]
|> List.averageBy decimal

(*** condition:html, include:averageBy ***)
(*** condition:html, include-fsi-output:averageBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.

