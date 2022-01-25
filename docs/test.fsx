#r "nuget: Plotly.NET, 2.0.0-preview.16"

open Plotly.NET

let x  = [1.; 2.; 3.; 4.; 5.; 6.; 7.; 8.; 9.; 10.; ]
let y = [2.; 1.5; 5.; 1.5; 3.; 2.5; 2.5; 1.5; 3.5; 1.]
let line1 = Chart.Line(x,y)

(** try 1*)
line1 |> GenericChart.toChartHTML
(***include-it-raw***)


(** try 2 *)
line1 |> GenericChart.toChartHTML
(***include-fsi-output***)