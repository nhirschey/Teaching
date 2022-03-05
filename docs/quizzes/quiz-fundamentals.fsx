(**
---
title: Fundamentals
category: Practice Quizzes
categoryindex: 2
index: 1
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
Calculate `3.0` to the power of `4.0`.
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
## Question 2
Assign the integer `1` to a value called `a`.
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
## Question 3
Write a function named `add3` that adds `3.0` to any `float` input.
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
## Question 4
Take the following two-parameter function:
*)

let multiply (x:float) (y:float) = x * y

(**
Use the above function and [partial application](https://fsharpforfunandprofit.com/posts/partial-application/)
to define a new function called 
`multiply2` that multiplies its input by `2.0`.
*)

(*** include-it-raw:preDetails ***)
(*** define: twoParaFunction, define-output: twoParaFunction ***)

let multiply2 = multiply 2.0

(*** condition:html, include:twoParaFunction ***)
(*** condition:html, include-fsi-output:twoParaFunction ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.




(**
## Question 5
Given a tuple `(1.0,2.0)`, assign the second element to a value named `b`.
*)

(*** include-it-raw:preDetails ***)

(*** define: assignSndTob, define-output: assignSndTob ***)
let b = snd (1.0, 2.0)
(*** condition:html, include: assignSndTob ***)
(*** condition:html, include-fsi-output: assignSndTob ***)

(**
or
*)

(*** define: assignSndTob1, define-output: assignSndTob1 ***)
let (_, b1) = (1.0, 2.0)
(*** condition:html, include: assignSndTob1 ***)
(*** condition:html, include-fsi-output: assignSndTob1 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 6
Create a tuple where the first, second, and third elements are `"a"`, `1`, and `2.0`.
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
## Question 7
Define a record type named `Record1` that has a `string` `Id` field and a `float Y` field.
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
## Question 8
Given the type signature `val a : float = 2.0`, what is the type of value a? 
*)

(*** include-it-raw:preDetails ***)

(** 
float
*)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your answer here, see website for solution.


(**
## Question 9
Create a record type named `Record2`. It should have two integer fields `X` and `Y`. Create an instance of the record where `X = 4` and `Y = 2`.
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
## Question 10
Explain why this expression gives an error when you try to run it: `4 + 7.0` 
*)

(*** include-it-raw:preDetails ***)

(**

Because 4 is an integer and 7.0 is a float. 
Addition is defined on values with the same type.
The two values need to either both be integers or both be floats.

Note: there are some cases where F# will convert (or "automatically widen")
integers into floats when it can tell that the conversion
is intended. But with simple addition like this, it cannot tell
if you are intending to add integers or floats, so it gives an error.
*)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your answer here, see website for solution.


(**
## Question 11
Create a `list` where the elements are `1`, `2`, and `3`.
*)

(*** include-it-raw:preDetails ***)

(*** define: list, define-output: list ***)
[ 1; 2; 3 ]
(*** condition:html, include: list ***)
(*** condition:html, include-fsi-output: list ***)

(**
or
*)

(*** define: list1, define-output: list1 ***)
[ 1 .. 3 ]
(*** condition:html, include: list1 ***)
(*** condition:html, include-fsi-output: list1 ***)

(**
or
*)

(*** define: list2, define-output: list2 ***)
[ for i = 1 to 3 do i ]
(*** condition:html, include: list2 ***)
(*** condition:html, include-fsi-output: list2 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 12
Given the below list, use `printfn` to print the whole
list to standard output using the [structured plaintext formatter](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/plaintext-formatting). 

The list to print:
*)

[1; 2; 3]

(*** include-it-raw:preDetails ***)
(*** define: printfnStructuredObject, define-output: printfnStructuredObject ***)

[1; 2; 3] |> (printfn "%A")

(*** condition:html, include:printfnStructuredObject ***)
(*** condition:html, include-output:printfnStructuredObject ***)
(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.


(**
## Question 13
Given the tuple `("hi", false, 20.321, 4)`,
use `printfn` and the tuple to print the following string
to standard output:
`"hi teacher, my False knowledge implies that 4%=0020.1"`

[String formatting](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/plaintext-formatting#format-specifiers-for-printf) documentation will be useful. 
*)

(*** include-it-raw:preDetails ***)

(*** define: printfnStringInterpolation, define-output: printfnStringInterpolation ***)
let (xString, xBool, xFloat, xInt) = ("hi", false, 20.321, 4)
(*** condition:html, include:printfnStringInterpolation ***)
(*** condition:html, include-fsi-output:printfnStringInterpolation ***)

(**
Using string interpolation
*)

(*** define: printfnStringInterpolation1, define-output: printfnStringInterpolation1 ***)
printfn $"{xString} teacher, my {xBool} knowledge implies that {xInt}%%=%06.1f{xFloat}"
(*** condition:html, include:printfnStringInterpolation1 ***)
(*** condition:html, include-output:printfnStringInterpolation1 ***)

(**
Using old-style printfn
*)

(*** define: printfnStringInterpolation2, define-output: printfnStringInterpolation2 ***)
printfn "%s teacher, my %b knowledge implies that %i%%=%06.1f" xString xBool xInt xFloat
(*** condition:html, include:printfnStringInterpolation2 ***)
(*** condition:html, include-output:printfnStringInterpolation2 ***)

(*** include-it-raw:postDetails ***)

(*** condition:ipynb ***)
// write your code here, see website for solution.



