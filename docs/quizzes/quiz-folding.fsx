(**
---
title: Fold and Friends
category: Practice Quizzes
categoryindex: 2
index: 99
---
*)

(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](../img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)
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
## Question 2
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
## Question 3
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
## Question 4
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
## Question 5
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

