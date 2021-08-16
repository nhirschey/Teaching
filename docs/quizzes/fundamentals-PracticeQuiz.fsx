(**
---
title: Fundamentals
category: Practice Quizzes
categoryindex: 1
index: 3
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
1 Calculate `3.0` to the power of `4.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: threePowerFour, define-output: threePowerFour ***)

3.0 ** 4.0

(*** condition:html, include:threePowerFour ***)
(*** condition:html, include-fsi-output:threePowerFour ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
2 Assign the integer `1` to a value called `a`.
*)

(*** include-it-raw:preDetails ***)
(*** define: Assign1toA, define-output: Assign1toA ***)

let a = 1

(*** condition:html, include:Assign1toA ***)
(*** condition:html, include-fsi-output:Assign1toA ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
3 Write a function named `add3` that adds `3.0` to any `float` input.
*)

(*** include-it-raw:preDetails ***)
(*** define: add3Function, define-output: add3Function ***)

let add3 x = x + 3.0

(*** condition:html, include:add3Function ***)
(*** condition:html, include-fsi-output:add3Function ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
4 Given a tuple `(1.0,2.0)`, assign the second element to a value named `b`.
*)

(*** include-it-raw:preDetails ***)
(*** define: assignSndTob, define-output: assignSndTob ***)

let b = snd (1.0, 2.0)
// or
// let (_, b) = (1.0, 2.0)

(*** condition:html, include:assignSndTob ***)
(*** condition:html, include-fsi-output:assignSndTob ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
5  Create a tuple where the first, second, and third elements are `"a"`, `1`, and `2.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: createTuple, define-output: createTuple ***)

("a", 1, 2.0)

(*** condition:html, include:createTuple ***)
(*** condition:html, include-fsi-output:createTuple ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
6 Define a record type named `Record1` that has a `string` `Id` field and a `float Y` field.
*)

(*** include-it-raw:preDetails ***)
(*** define: RecordType1, define-output: RecordType1 ***)

type Record1 = { Id : string; Y : float }

(*** condition:html, include:RecordType1 ***)
(*** condition:html, include-fsi-output:RecordType1 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
7 Given the type signature `val a : float = 2.0`, what is the type of value a? 
*)

(*** include-it-raw:preDetails ***)

(** 
float
*)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your answer here, see website for solution.


(**
8 Create a record type named `Record2`. It should have two integer fields `X` and `Y`. Create an instance of the record where `X = 4` and `Y = 2`.
*)

(*** include-it-raw:preDetails ***)
(*** define: RecordType2, define-output: RecordType2 ***)

type Record2 = { X : int; Y : int }
{ X = 4; Y = 2}

(*** condition:html, include:RecordType2 ***)
(*** condition:html, include-fsi-output:RecordType2 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
9 Explain why this expression gives an error when you try to run it: `4 + 7.0` 
*)

(*** include-it-raw:preDetails ***)

(**

Because 4 is an integer and 7.0 is a float. Addition is defined on values with the same type.
The two values need to either both be integers or both be floats.
*)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your answer here, see website for solution.


(**
10 Create an `array` where the elements are `1`, `2`, and `3`.
*)

(*** include-it-raw:preDetails ***)
(*** define: Array1, define-output: Array1 ***)

[| 1; 2; 3 |]
// or [| 1 .. 3 |]
// or [| for i = 1 to 3 do i |]

(*** condition:html, include:Array1 ***)
(*** condition:html, include-fsi-output:Array1 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
11 Take a `list` containing floats `1.0 .. 10.0`. Pass it to `List.map` and use an anonymous function to divide each number by `3.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: List1, define-output: List1 ***)

[ 1.0 .. 10.0]
|> List.map (fun x -> x / 3.0)

(*** condition:html, include:List1 ***)
(*** condition:html, include-fsi-output:List1 ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
12 Take a `list` containing floats `1.0 .. 10.0`. Group the elements based on whether the elements are greater than or equal to `4.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: listGroupBy, define-output: listGroupBy ***)

[ 1.0 .. 10.0]
|> List.groupBy (fun x -> x >= 4.0)

(*** condition:html, include:listGroupBy ***)
(*** condition:html, include-fsi-output:listGroupBy ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
13 Take a `list` containing floats `1.0 .. 10.0`. Filter it so that you are left with the elements `> 5.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: listFilter, define-output: listFilter ***)

[ 1.0 .. 10.0]
|> List.filter (fun x -> x > 5.0)

(*** condition:html, include:listFilter ***)
(*** condition:html, include-fsi-output:listFilter ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
14 Take a `list` containing floats `1.0 .. 10.0`. Use `List.groupBy` to group the elements based on if they're `>= 5.0`. Then use `List.map` to get the maxiumum element that is `< 5.0` and the minimum value that is `>= 5.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: listGroupMaxAndMin, define-output: listGroupMaxAndMin ***)

[ 1.0 .. 10.0]
|> List.groupBy(fun x -> x >= 5.0)
|> List.map(fun (gt5, xs) -> 
    if gt5 then List.min xs else List.max xs)

(*** condition:html, include:listGroupMaxAndMin ***)
(*** condition:html, include-fsi-output:listGroupMaxAndMin ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
15 Take a `list` containing floats `1.0 .. 10.0`. Use functions from the List module to sort it in descending order. Then take the 3rd element of the reversed list and add `7.0` to it.
*)

(*** include-it-raw:preDetails ***)
(*** define: listSort, define-output: listSort ***)

[1.0 .. 10.0]
|> List.sortByDescending id
|> List.item 2
|> fun x -> x + 7.0


(*** condition:html, include:listSort ***)
(*** condition:html, include-fsi-output:listSort ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.

