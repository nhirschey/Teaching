(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=loading-scripts.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//loading-scripts.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//loading-scripts.ipynb)

This will introduce using `#load` to load scripts with external code.

When you type `#load "Script.fsx"` in the REPL,
F# interactive compiles the code in `Script.fsx` and puts it into
a code module with the same name as the script.

We are going to use a helper script called `YahooFinance.fsx` that includes
code for requesting price histories from yahoo. To download it,
go to the [YahooFinance](YahooFinance.html) page and click the "download script"
button at the top. Make sure that you have saved it in
the same directory as this file.

If you have downloaded it correctly then the following code will evaluate to `true`.

*)
System.IO.File.Exists("YahooFinance.fsx")(* output: 
true*)
(**
Assuming that the above code evaluated to `true` we can now load it into our session.

*)
#load "YahooFinance.fsx"
(**
Namespaces are a hierarchical way of organizing code.
If we `open` a namespace, then we have access to the code inside the namespace directly.

It is common to open the `System` namespace.

*)
open System
(**
Now we can leave `System` off when accessing code in the `System` namespace,
such as `System.IO.File.Exists` that we used above.

*)
IO.File.Exists("YahooFinance.fsx")(* output: 
true*)
(**
We also want to open the `YahooFinance` module from `YahooFinance.fsx`,
which is similar to a namespace.

*)
open YahooFinance
(**
We are ready to request some data. Let's define our start and end dates.
`DateTime` is a type in the `System` namespace.
We have opened that namespace so we can access the type directly.

*)
let myStart = DateTime(2010,1,1)
let myEnd = DateTime.UtcNow
myEnd(* output: 
2/20/2024 12:49:15 PM*)
(**
Our `YahooFinance` module has code for requesting price histories of stocks.

*)
let bnd = YahooFinance.PriceHistory("BND",startDate=myStart,endDate=myEnd,interval = Interval.Daily)
let vti = YahooFinance.PriceHistory("VTI",startDate=myStart,endDate=myEnd,interval = Interval.Daily)
(**
This returns several data items for each point in time.

*)
vti[0..3](* output: 
[{ Symbol = "VTI"
   Date = 1/4/2010 12:00:00 AM
   Open = 56.860001
   High = 57.380001
   Low = 56.84
   Close = 57.310001
   AdjustedClose = 44.335224
   Volume = 2251500.0 }; { Symbol = "VTI"
                           Date = 1/5/2010 12:00:00 AM
                           Open = 57.34
                           High = 57.540001
                           Low = 57.110001
                           Close = 57.529999
                           AdjustedClose = 44.505424
                           Volume = 1597700.0 }; { Symbol = "VTI"
                                                   Date = 1/6/2010 12:00:00 AM
                                                   Open = 57.5
                                                   High = 57.720001
                                                   Low = 57.41
                                                   Close = 57.610001
                                                   AdjustedClose = 44.567318
                                                   Volume = 2120300.0 };
 { Symbol = "VTI"
   Date = 1/7/2010 12:00:00 AM
   Open = 57.549999
   High = 57.889999
   Low = 57.290001
   Close = 57.849998
   AdjustedClose = 44.752979
   Volume = 1656700.0 }]*)

