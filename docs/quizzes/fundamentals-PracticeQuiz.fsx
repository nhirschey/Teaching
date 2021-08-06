(**
1. Calculate 3.0 to the power of 4.0.
*)

3.0 ** 4.0

(**
2. Assign the integer `1` to a value called `a`.
*)

let a = 1

(**
3. Write a function named add3 that adds 3.0 to any float input.
*)

let add3 x = x + 3.0

(**
4.Given a tuple `(1.0,2.0)`, assign the second element to a value named `b`.
*)

let b = snd (1.0, 2.0)
// or
// let (_, b) = (1.0, 2.0)

(**
5. Create a tuple where the first, second, and third elements are "a", 1, and 2.0.
*)

("a", 1, 2.0)

(**
6. Define a record type named Record1 that has a string Id field and a float Y field.
*)

type Record1 = { Id : string; Y : float }

(**
7. Given the type signature `val a : float = 2.0`, what is the type of value a? 
*)

// float

(**
8. Create a record type named Record2. It should have two integer fields X and Y. Create an instance of the record where X = 4 and Y = 2.
*)

type Record2 = { X : int; Y : int }
{ X = 4; Y = 2}

(**
9. Explain why this expression gives an error when you try to run it: 4 + 7.0 
*)

// Because 4 is an integer and 7.0 is a float. Addition is defined on values with the same type.
// The two values need to either both be integers or both be floats.

(**
10. Create an array where the elements are 1, 2, and 3.
*)

[| 1; 2; 3 |]
// or [| 1 .. 3 |]
// or [| for i = 1 to 3 do i |]

(**
11. Take a list containing floats 1.0 .. 10.0. Pass it to List.map and use an anonymous function to divide each number by 3.0.
*)

[ 1.0 .. 10.0]
|> List.map (fun x -> x / 3.0)

(**
12. Take a list containing floats 1.0 .. 10.0. Group the elements based on whether the elements are greater than or equal to 4.0.
*)

[ 1.0 .. 10.0]
|> List.groupBy (fun x -> x >= 4.0)

(**
13. Take a list containing floats 1.0 .. 10.0. Filter it so that you are left with the elements > 5.0.
*)

[ 1.0 .. 10.0]
|> List.filter (fun x -> x > 5.0)

(**
14. Take a list containing floats 1.0 .. 10.0. Use List.groupBy to group the elements based on if they're >= 5.0. Then use List.map to get the maxiumum element that is < 5.0 and the minimum value that is >= 5.0.
*)

[ 1.0 .. 10.0]
|> List.groupBy(fun x -> x >= 5.0)
|> List.map(fun (gt5, xs) -> 
    if gt5 then List.min xs else List.max xs)

(**
15. Take a list containing floats 1.0 .. 10.0. Use functions from the List module to sort it in descending order. Then take the 3rd element of the reversed list and add 7.0 to it.
*)

[1.0 .. 10.0]
|> List.sortByDescending id
|> List.item 2
|> fun x -> x + 7.0



