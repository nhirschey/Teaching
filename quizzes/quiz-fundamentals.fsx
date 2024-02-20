(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=quizzes/quiz-fundamentals.ipynb)&emsp;
[![Script](../img/badge-script.svg)](/Teaching//quizzes/quiz-fundamentals.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](/Teaching//quizzes/quiz-fundamentals.ipynb)

## Question 1

Calculate `3.0` to the power of `4.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: float = 81.0
```

</details>
</span>
</p>
</div>

## Question 2

Assign the integer `1` to a value called `a`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val a: int = 1
```

</details>
</span>
</p>
</div>

## Question 3

Write a function named `add3` that adds `3.0` to any `float` input.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val add3: x: float -> float
```

</details>
</span>
</p>
</div>

## Question 4

Take the following two-parameter function:

*)
let multiply (x:float) (y:float) = x * y
(**
Use the above function and [partial application](https://fsharpforfunandprofit.com/posts/partial-application/)
to define a new function called
`multiply2` that multiplies its input by `2.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val multiply2: (float -> float)
```

</details>
</span>
</p>
</div>

## Question 5

Given a tuple `(1.0,2.0)`, assign the second element to a value named `b`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val b: float = 2.0
```

or

```
val b1: float = 2.0
```

</details>
</span>
</p>
</div>

## Question 6

Create a tuple where the first, second, and third elements are `"a"`, `1`, and `2.0`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: string * int * float = ("a", 1, 2.0)
```

</details>
</span>
</p>
</div>

## Question 7

Define a record type named `Record1` that has a `string` `Id` field and a `float Y` field.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type Record1 =
  {
    Id: string
    Y: float
  }
```

</details>
</span>
</p>
</div>

## Question 8

Given the type signature `val a : float = 2.0`, what is the type of value a?

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

float

</details>
</span>
</p>
</div>

## Question 9

Create a record type named `Record2`. It should have two integer fields `X` and `Y`. Create an instance of the record where `X = 4` and `Y = 2`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
type Record2 =
  {
    X: int
    Y: int
  }
val it: Record2 = { X = 4
                    Y = 2 }
```

</details>
</span>
</p>
</div>

## Question 10

Explain why this expression gives an error when you try to run it: `4 + 7.0`

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

Because 4 is an integer and 7.0 is a float.
Addition is defined on values with the same type.
The two values need to either both be integers or both be floats.

Note: there are some cases where F# will convert (or "automatically widen")
integers into floats when it can tell that the conversion
is intended. But with simple addition like this, it cannot tell
if you are intending to add integers or floats, so it gives an error.

</details>
</span>
</p>
</div>

## Question 11

Create a `list` where the elements are `1`, `2`, and `3`.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int list = [1; 2; 3]
```

or

```
val it: int list = [1; 2; 3]
```

or

```
val it: int list = [1; 2; 3]
```

</details>
</span>
</p>
</div>

## Question 12

Given the below list, use `printfn` to print the whole
list to standard output using the [structured plaintext formatter](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/plaintext-formatting).

The list to print:

*)
[1; 2; 3](* output: 

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>


[1; 2; 3]


</details>
</span>
</p>
</div>
*)
(**
## Question 13

Given the tuple `("hi", false, 20.321, 4)`,
use `printfn` and the tuple to print the following string
to standard output:
`"hi teacher, my False knowledge implies that 4%=0020.1"`

[String formatting](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/plaintext-formatting#format-specifiers-for-printf) documentation will be useful.

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val xString: string = "hi"
val xInt: int = 4
val xFloat: float = 20.321
val xBool: bool = false
```

Using string interpolation

```
hi teacher, my False knowledge implies that 4%=0020.3
```

Using old-style printfn

```
hi teacher, my false knowledge implies that 4%=0020.3
```

</details>
</span>
</p>
</div>

*)

