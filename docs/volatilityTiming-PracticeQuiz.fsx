
(*** define: filter ***)
(*
Given a the list below, filter the list so that only numbers great than 2 remain.

[ 1; -4; 7; 2; -10]
*)

(*** define: filter-ans ***)
[ 1; -4; 7; 2; -10]
|> List.filter(fun x -> x > 2)

(*** define: takeWhile ***)
(*
Given a the list below, take elements until you find one that is greater than 4.

[ 1; -4; 7; 2; -10]

*)

(*** define: takeWhile-ans ***)
[ 1; -4; 7; 2; -10]
|> List.takeWhile(fun x -> x <= 4)

(*** define: skipWhile ***)

(*
Given a the list below, skip elements until you find one that is greater than 4.

[ 1; -4; 7; 2; -10]

*)

(*** define: skipWhile-ans ***)
[ 1; -4; 7; 2; -10]
|> List.skipWhile(fun x -> x <= 4)

(*** define: pairwise ***)
(*
Given a the list below, return tuples of all consecutive pairs.

[ 1; -4; 7; 2; -10]

*)

(*** define: pairwise-ans ***)
[ 1; -4; 7; 2; -10]
|> List.pairwise

(*** define: windowed ***)
(*
Given a the list below, return sliding windows of 3 consecutive observations.

[ 1; -4; 7; 2; -10]

*)

(*** define: windowed-ans ***)
[ 1; -4; 7; 2; -10]
|> List.windowed 3

(*** define: scan ***)
(*
Given a the list below, use scan to return the intermediate and final cumulative sums.

[ 1; -4; 7; 2; -10]

*)

(*** define: scan-ans ***)
(0, [ 1; -4; 7; 2; -10])
||> List.scan (fun acc x -> acc + x) 

(*** define: fold ***)
(*
Given a the list below, use fold to return the final sum.

[ 1; -4; 7; 2; -10]

*)

(*** define: fold-ans ***)
(0, [ 1; -4; 7; 2; -10])
||> List.fold (fun acc x -> acc + x)

(*** define: mapFold1 ***)
(*
Given a the list below, use mapFold to return the intermediate and final cumulative sums.

[ 1; -4; 7; 2; -10]

*)

(*** define: mapFold1-ans ***)
(0, [ 1; -4; 7; 2; -10])
||> List.mapFold (fun acc x -> acc + x, acc + x)

(*** define: mapFold2 ***)
(*
Given a the list below, use mapFold to return a tuple of
- A new list in which each element of the original list is transformed by adding 1 to it and then converted into a string.
- The final cumulative sums of the list elements.

[ 1; -4; 7; 2; -10]

*)

(*** define: mapFold2-ans ***)
(0, [ 1; -4; 7; 2; -10])
||> List.mapFold (fun acc x -> string (x + 1), acc + x) 

(*** define: mapFold3 ***)
(*
Given a the list below, use mapFold to return a tuple of
- The list of records with the Y field in each record updated to Y+1
- The sum of the Y fields.
type R1 = { X : string; Y : int }

let r1xs =
    [ { X = "a"; Y = 1 }
      { X = "b"; Y = -4 }
      { X = "c"; Y = 7 } 
      { X = "d"; Y = 2 }
      { X = "e"; Y = -10 }]

*)

(*** define: mapFold3-ans ***)
type R1 = { X : string; Y : int }
let r1xs = 
    [ { X = "a"; Y = 1 }
      { X = "b"; Y = -4 }
      { X = "c"; Y = 7 } 
      { X = "d"; Y = 2 }
      { X = "e"; Y = -10 }]

(0, r1xs)
||> List.mapFold (fun acc x -> { x with Y = x.Y+1}, acc + x.Y)

(*** define: sum ***)
(*
Given a the list below, sum all the elements.

[ 1; -4; 7; 2; -10]

*)

(*** define: sum-ans ***)
[ 1; -4; 7; 2; -10]
|> List.sum

(*** define: sumBy ***)
(*
Given a the list below, add 1 to all the elements and then calculate the sum.

[ 1; -4; 7; 2; -10]

*)

(*** define: sumBy-ans ***)
[ 1; -4; 7; 2; -10]
|> List.sumBy(fun x -> x + 1)

(*** define: average ***)
(*
Given a the list below, calculate the average of the elements in the list.

[ 1.0; -4.0; 7.0; 2.0; -10.0]

*)

(*** define: average-ans ***)
[ 1.0; -4.0; 7.0; 2.0; -10.0]
|> List.average

(*** define:averageBy ***)
(*
Given a the list below, convert each element to a decimal and then calculate the average of the elements in the list.

[ 1.0; -4.0; 7.0; 2.0; -10.0]

*)

(*** define:averageBy-ans ***)
[ 1.0; -4.0; 7.0; 2.0; -10.0]
|> List.averageBy(fun x -> decimal x)

// Since decimal is a function that converts to
// the decimal type, you could also do.
// The FSharp linter shoud show you a blue squiggly
// in the above code telling you this.
[ 1.0; -4.0; 7.0; 2.0; -10.0]
|> List.averageBy decimal

(**
## Questions 
*)

(*** include: filter ***)

(*** include: takeWhile ***)

(*** include: skipWhile ***)

(*** include: pairwise ***)

(*** include: windowed ***)

(*** include: scan ***)

(*** include: fold ***)

(*** include: mapFold1 ***)

(*** include: mapFold2 ***)

(*** include: mapFold3 ***)

(*** include: sum ***)

(*** include: sumBy ***)

(*** include: average ***)

(*** include: averageBy ***)

(**
## Answers 
*)

(*** include: filter***)
(*** include: filter-ans ***)

(*** include: takeWhile ***)
(*** include: takeWhile-ans ***)

(*** include: skipWhile ***)
(*** include: skipWhile-ans ***)

(*** include: pairwise ***)
(*** include: pairwise-ans ***)

(*** include: windowed ***)
(*** include: windowed-ans ***)

(*** include: scan ***)
(*** include: scan-ans ***)

(*** include: fold ***)
(*** include: fold-ans ***)

(*** include: mapFold1 ***)
(*** include: mapFold1-ans ***)

(*** include: mapFold2 ***)
(*** include: mapFold2-ans ***)

(*** include: mapFold3 ***)
(*** include: mapFold3-ans ***)

(*** include: sum ***)
(*** include: sum-ans ***)

(*** include: sumBy ***)
(*** include: sumBy-ans ***)

(*** include: average ***)
(*** include: average-ans ***)

(*** include: averageBy ***)
(*** include: averageBy-ans ***)
