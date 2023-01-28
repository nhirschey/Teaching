
(**
---
title: Loading external scripts
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

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

System.IO.File.Exists("YahooFinance.fsx")
(***include-it***)

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

IO.File.Exists("YahooFinance.fsx")
(***include-it***)

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
myEnd
(***include-it***)

(**
Our `YahooFinance` module has code for requesting price histories of stocks.
*)

let bnd = YahooFinance.PriceHistory("BND",startDate=myStart,endDate=myEnd,interval = Interval.Daily)
let vti = YahooFinance.PriceHistory("VTI",startDate=myStart,endDate=myEnd,interval = Interval.Daily)

(**
This returns several data items for each point in time.
*)

vti[0..3]
(***include-it***)
