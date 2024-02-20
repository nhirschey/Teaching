(**
[![Binder](../img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=quizzes/quiz-folding.ipynb)&emsp;
[![Script](../img/badge-script.svg)](/Teaching//quizzes/quiz-folding.fsx)&emsp;
[![Notebook](../img/badge-notebook.svg)](/Teaching//quizzes/quiz-folding.ipynb)

## Question 1

Given the list below, use `scan` to return the intermediate and final cumulative sums.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int list = [0; 1; -3; 4; 6; -4]
```

</details>
</span>
</p>
</div>

## Question 2

Given the list below, use `fold` to return the final sum.

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

## Question 3

Given the list below, use `mapFold` to return the intermediate and final cumulative sums.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: int list * int = ([1; -3; 4; 6; -4], -4)
```

</details>
</span>
</p>
</div>

## Question 4

Given the list below, use `mapFold` to return a tuple of

0 A new list in which each element of the original list is transformed by adding `1` to it and then converted into a `string`.

1 The final cumulative sums of the list elements.

```fsharp
[ 1; -4; 7; 2; -10]
```
<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>

```
val it: string list * int = (["2"; "-3"; "8"; "3"; "-9"], -4)
```

</details>
</span>
</p>
</div>

## Question 5

Given the list below, use `mapFold` to return a tuple of

0 The list of records with the `Y` field in each record updated to `Y+1`

1 The sum of the `Y` fields.

*)
type R1 = { X : string; Y : int }

let r1xs =
    [ { X = "a"; Y = 1 }
      { X = "b"; Y = -4 }
      { X = "c"; Y = 7 } 
      { X = "d"; Y = 2 }
      { X = "e"; Y = -10 }](* output: 

<div style="padding-left: 40px;">
<p> 
<span>
<details>
<summary><p style="display:inline">answer</p></summary>


val it: R1 list * int =
  ([{ X = "a"
      Y = 2 }; { X = "b"
                 Y = -3 }; { X = "c"
                             Y = 8 }; { X = "d"
                                        Y = 3 }; { X = "e"
                                                   Y = -9 }], -4)


</details>
</span>
</p>
</div>
*)

