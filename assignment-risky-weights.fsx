(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=assignment-risky-weights.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//assignment-risky-weights.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//assignment-risky-weights.ipynb)

Group Name:

Student Name | Student Number
--- | ---
**1:** | &#32;
**2:** | &#32;
**3:** | &#32;
**4:** | &#32;
**5:** | &#32;


This is an assignment. You may work in groups. Please write your group and group member names above. You will find sections labeled **Task** asking you to do each piece of analysis. Please make sure that you complete all of these tasks. I included some tests to help you see if you are calculating the solution correctly, but if you cannot get the test to pass submit your best attempt and you may recieve partial credit.

All work that you submit should be your own. Make use of the course resources and example code on the course website. It should be possible to complete all the requested tasks using information given below or somewhere on the course website.

*)
#r "nuget: FSharp.Data, 5.0.2"
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: Quotes.YahooFinance"
#r "nuget: Plotly.NET, 3.*"
open System
open FSharp.Data
open Plotly.NET
open FSharp.Stats
open Quotes.YahooFinance
(**
for testing.

*)
#r "nuget: FsUnit.Xunit"
#r "nuget: xunit, 2.*"
open Xunit
open FsUnit.Xunit
open FsUnitTyped
fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))
fsi.AddPrinter<YearMonth>(fun ym -> $"{ym.Year}-{ym.Month}")
(**
## Load Data

We get the SPY ETF

*)
type MonthlyReturn = { Date: DateTime; Return: float }
let spy = 
    YahooFinance.History("SPY", 
                         startDate = DateTime(2010,1,1), 
                         endDate = DateTime(2023, 2, 28),
                         interval = Monthly)
    |> List.sortBy (fun month -> month.Date)
    |> List.pairwise
    |> List.map (fun (m0, m1) -> 
        { Date = m1.Date
          Return = (m1.AdjustedClose - m0.AdjustedClose) / m0.AdjustedClose })
(**
Load risk-free rate

*)
// 4-week Treasury Bill: Secondary Market Rate
type DTB4WK = CsvProvider<"https://fred.stlouisfed.org/graph/fredgraph.csv?id=DTB4WK",
                           Schema="Date,RiskFreeRate (float)",
                           MissingValues=".">


// We'll take the 4-week interest rate at the start of the month as the risk-free rate for that month.
// Then we'll put it in a dictionary for efficient lookup.
let rf =
    DTB4WK.GetSample().Rows
    |> Seq.toList
    |> List.filter (fun x -> not (Double.IsNaN x.RiskFreeRate))
    |> List.groupBy (fun day -> day.DATE.Year, day.DATE.Month)
    |> List.map (fun (month, daysInMonth) ->
        let firstDay = 
            daysInMonth 
            |> List.sortBy (fun day -> day.DATE)
            |> List.head
        let date = DateTime(firstDay.DATE.Year, firstDay.DATE.Month, 1)
        // discount basis assumes 30 days in a month, 360 days per year.
        let ret = (30.0 / 360.0) * firstDay.RiskFreeRate / 100.0 
        date, ret)
    |> dict
(**
Look at an example, the index is date

*)
rf[DateTime(2010,1,1)] 
(**
Calculating excess returns of spy returns:

*)
let excessSpy =
    [ for x in spy do 
        { Date = x.Date
          Return = x.Return - rf.[x.Date] } ]
(**
As an example, I'll first calculate the standard deviation of the the excess returns of SPY and assign it to a value named `stdDevExcessSpy`.

*)
let stdDevExcessSpy =
    excessSpy
    |> stDevBy (fun x -> x.Return)
(**
The following test will pass if I calculate it correctly.

*)
// Test.
stdDevExcessSpy
|> should (equalWithin 0.005) 0.04
(**
The test following test will fail if I calculate it incorrectly. In this failing example I report an annualized standard deviation instead of a monthly standard deviation.

*)
let stdDevFAIL =
    let monthlyStDev = 
        excessSpy
        |> stDevBy (fun x -> x.Return)
    monthlyStDev * (sqrt 12.0)

// Test
if false then // make this `if true` to run the test.
    stdDevFAIL
    |> should (equalWithin 0.005) 0.04
(**
## Start of the assignment

> **Task:** What is the cumulative return of SPY from the beginning to the end of the sample period? In other words, if you invest
$1 in SPY at the beginning of the sample, how many **additional** dollars would you have by the end? Assign it to a value named `cumulativeSpyReturn`.
> 

Write your solution in the cell below.

*)
// Solution here

// Test
cumulativeReturn 
|> should (equalWithin 0.1) 3.75
(**
> **Task:** What is the cumulative **excess** return of SPY from the beginning to the end of the sample period? Assign it to a value named `cumulativeExcessSpyReturn`.
> 

Write your solution in the cell below.

*)
// Solution here

// Test
cumulativeExcessReturn 
|> should (equalWithin 0.1) 3.3
(**
> **Task:** Plot the cumulative **excess** return of SPY from the beginning to the end of the sample period. The date should be the x-axis and the cumulative excess return should be the y-axis.
> 

<div id="b57c8baf-922d-4bc4-bab4-47b517d28aff"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_b57c8baf922d4bc4bab447b517d28aff = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-2.6.3.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","mode":"lines","x":["2010-02-01T00:00:00","2010-03-01T00:00:00","2010-04-01T00:00:00","2010-05-01T00:00:00","2010-06-01T00:00:00","2010-07-01T00:00:00","2010-08-01T00:00:00","2010-09-01T00:00:00","2010-10-01T00:00:00","2010-11-01T00:00:00","2010-12-01T00:00:00","2011-01-01T00:00:00","2011-02-01T00:00:00","2011-03-01T00:00:00","2011-04-01T00:00:00","2011-05-01T00:00:00","2011-06-01T00:00:00","2011-07-01T00:00:00","2011-08-01T00:00:00","2011-09-01T00:00:00","2011-10-01T00:00:00","2011-11-01T00:00:00","2011-12-01T00:00:00","2012-01-01T00:00:00","2012-02-01T00:00:00","2012-03-01T00:00:00","2012-04-01T00:00:00","2012-05-01T00:00:00","2012-06-01T00:00:00","2012-07-01T00:00:00","2012-08-01T00:00:00","2012-09-01T00:00:00","2012-10-01T00:00:00","2012-11-01T00:00:00","2012-12-01T00:00:00","2013-01-01T00:00:00","2013-02-01T00:00:00","2013-03-01T00:00:00","2013-04-01T00:00:00","2013-05-01T00:00:00","2013-06-01T00:00:00","2013-07-01T00:00:00","2013-08-01T00:00:00","2013-09-01T00:00:00","2013-10-01T00:00:00","2013-11-01T00:00:00","2013-12-01T00:00:00","2014-01-01T00:00:00","2014-02-01T00:00:00","2014-03-01T00:00:00","2014-04-01T00:00:00","2014-05-01T00:00:00","2014-06-01T00:00:00","2014-07-01T00:00:00","2014-08-01T00:00:00","2014-09-01T00:00:00","2014-10-01T00:00:00","2014-11-01T00:00:00","2014-12-01T00:00:00","2015-01-01T00:00:00","2015-02-01T00:00:00","2015-03-01T00:00:00","2015-04-01T00:00:00","2015-05-01T00:00:00","2015-06-01T00:00:00","2015-07-01T00:00:00","2015-08-01T00:00:00","2015-09-01T00:00:00","2015-10-01T00:00:00","2015-11-01T00:00:00","2015-12-01T00:00:00","2016-01-01T00:00:00","2016-02-01T00:00:00","2016-03-01T00:00:00","2016-04-01T00:00:00","2016-05-01T00:00:00","2016-06-01T00:00:00","2016-07-01T00:00:00","2016-08-01T00:00:00","2016-09-01T00:00:00","2016-10-01T00:00:00","2016-11-01T00:00:00","2016-12-01T00:00:00","2017-01-01T00:00:00","2017-02-01T00:00:00","2017-03-01T00:00:00","2017-04-01T00:00:00","2017-05-01T00:00:00","2017-06-01T00:00:00","2017-07-01T00:00:00","2017-08-01T00:00:00","2017-09-01T00:00:00","2017-10-01T00:00:00","2017-11-01T00:00:00","2017-12-01T00:00:00","2018-01-01T00:00:00","2018-02-01T00:00:00","2018-03-01T00:00:00","2018-04-01T00:00:00","2018-05-01T00:00:00","2018-06-01T00:00:00","2018-07-01T00:00:00","2018-08-01T00:00:00","2018-09-01T00:00:00","2018-10-01T00:00:00","2018-11-01T00:00:00","2018-12-01T00:00:00","2019-01-01T00:00:00","2019-02-01T00:00:00","2019-03-01T00:00:00","2019-04-01T00:00:00","2019-05-01T00:00:00","2019-06-01T00:00:00","2019-07-01T00:00:00","2019-08-01T00:00:00","2019-09-01T00:00:00","2019-10-01T00:00:00","2019-11-01T00:00:00","2019-12-01T00:00:00","2020-01-01T00:00:00","2020-02-01T00:00:00","2020-03-01T00:00:00","2020-04-01T00:00:00","2020-05-01T00:00:00","2020-06-01T00:00:00","2020-07-01T00:00:00","2020-08-01T00:00:00","2020-09-01T00:00:00","2020-10-01T00:00:00","2020-11-01T00:00:00","2020-12-01T00:00:00","2021-01-01T00:00:00","2021-02-01T00:00:00","2021-03-01T00:00:00","2021-04-01T00:00:00","2021-05-01T00:00:00","2021-06-01T00:00:00","2021-07-01T00:00:00","2021-08-01T00:00:00","2021-09-01T00:00:00","2021-10-01T00:00:00","2021-11-01T00:00:00","2021-12-01T00:00:00","2022-01-01T00:00:00","2022-02-01T00:00:00","2022-03-01T00:00:00","2022-04-01T00:00:00","2022-05-01T00:00:00","2022-06-01T00:00:00","2022-07-01T00:00:00","2022-08-01T00:00:00","2022-09-01T00:00:00","2022-10-01T00:00:00","2022-11-01T00:00:00","2022-12-01T00:00:00","2023-01-01T00:00:00","2023-02-01T00:00:00"],"y":[0.031153189603870812,0.08937439542260495,0.11063711196221715,0.022262578015163736,-0.03533981966172939,0.03532165826610578,-0.011368626159632633,0.07130032271559328,0.11804643661798542,0.11791599786704676,0.18625350480426373,0.22017872798655325,0.26240162967194514,0.25701833918870753,0.299010894760674,0.2844218999225385,0.2564814252087211,0.23744053124868292,0.16926688519187616,0.08247572349027554,0.2068273935331748,0.20191281444063147,0.2067972637201927,0.27075998083204467,0.32586543455705486,0.36246117828227065,0.359240361257841,0.2775310519397025,0.3224905334897501,0.34501336716988273,0.3786302348756869,0.40595936768988605,0.38768378400927417,0.39546744530251887,0.3978688827284418,0.47976625618565616,0.4986217257745513,0.5485513148749757,0.5852445529600534,0.6226320870492426,0.5925116014804095,0.6836559947962084,0.6331319776998356,0.6766006450126163,0.7626501743392775,0.8148467439350291,0.8518154805757696,0.7962263614093978,0.8779226829066444,0.88511862219256,0.9065692524326714,0.9507824035973231,0.9814949705179437,0.9641868041771913,1.0416840591094285,1.0041142443679054,1.0608846199022883,1.1174501874981986,1.1004686870747622,1.0494472783151778,1.1646183314038727,1.1211170396230337,1.1515020667931966,1.179161678289789,1.124528216534392,1.1830697133056445,1.049974676768294,0.9873189170921632,1.167586541507283,1.1754911178576868,1.1249010281652057,1.0308251639426853,1.028860095882965,1.1537317108238767,1.1730131910822204,1.2097986036990545,1.2055100900154745,1.2974134677759874,1.2998004447190024,1.2878761797379275,1.2591103031022994,1.3418802913934993,1.374749000312793,1.4304823895688865,1.52498722684261,1.516246077426711,1.5507889063097338,1.5854275162552063,1.5875586661229582,1.6515736171399151,1.6571452610453634,1.695217272192839,1.770329423432269,1.8526062879906444,1.8698813346622667,2.0440839745669375,1.9298732164727537,1.834607887671798,1.8567720920510506,1.9222897365506442,1.9218165269757996,2.0391128290447633,2.1313092745087796,2.130590361144698,1.9219935416763332,1.9709101026842588,1.6880259960002255,1.914911946593222,2.0036438716935057,2.038594134280462,2.170632461255964,1.9621485320640617,2.147212254460284,2.2047479540793065,2.1455618003093546,2.1867054078604706,2.267514869252737,2.3815719735349616,2.4583472854780437,2.469624859507325,2.1905247049570304,1.7720760497784669,2.1423848610731495,2.2918431898809977,2.33521395871751,2.546840532210415,2.7941313304307314,2.637220894806405,2.560456116004951,2.94748298289115,3.0761252504219367,3.051480382740759,3.163930699333136,3.3386556920688344,3.583105773731085,3.613122242026396,3.701163789239444,3.831491142579062,3.975074096745498,3.7278956817738367,4.075515796432317,4.034524054701512,4.248498282144967,3.988934229203151,3.8415095699760453,4.007542096527134,3.581600592520201,3.590491519920322,3.1909726311185977,3.5922781624408575,3.3966766415271055,2.9648842603710253,3.2956574710266375,3.5217162766988084,3.2270770290602204,3.4995890250363804,3.369733552859567],"marker":{},"line":{}}];
            var layout = {"width":600,"height":600,"template":{"layout":{"title":{"x":0.05},"font":{"color":"rgba(42, 63, 95, 1.0)"},"paper_bgcolor":"rgba(255, 255, 255, 1.0)","plot_bgcolor":"rgba(229, 236, 246, 1.0)","autotypenumbers":"strict","colorscale":{"diverging":[[0.0,"#8e0152"],[0.1,"#c51b7d"],[0.2,"#de77ae"],[0.3,"#f1b6da"],[0.4,"#fde0ef"],[0.5,"#f7f7f7"],[0.6,"#e6f5d0"],[0.7,"#b8e186"],[0.8,"#7fbc41"],[0.9,"#4d9221"],[1.0,"#276419"]],"sequential":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]],"sequentialminus":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]},"hovermode":"closest","hoverlabel":{"align":"left"},"coloraxis":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"geo":{"showland":true,"landcolor":"rgba(229, 236, 246, 1.0)","showlakes":true,"lakecolor":"rgba(255, 255, 255, 1.0)","subunitcolor":"rgba(255, 255, 255, 1.0)","bgcolor":"rgba(255, 255, 255, 1.0)"},"mapbox":{"style":"light"},"polar":{"bgcolor":"rgba(229, 236, 246, 1.0)","radialaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""},"angularaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""}},"scene":{"xaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"yaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"zaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true}},"ternary":{"aaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"baxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"caxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"bgcolor":"rgba(229, 236, 246, 1.0)"},"xaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"yaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"annotationdefaults":{"arrowcolor":"#2a3f5f","arrowhead":0,"arrowwidth":1},"shapedefaults":{"line":{"color":"rgba(42, 63, 95, 1.0)"}},"colorway":["rgba(99, 110, 250, 1.0)","rgba(239, 85, 59, 1.0)","rgba(0, 204, 150, 1.0)","rgba(171, 99, 250, 1.0)","rgba(255, 161, 90, 1.0)","rgba(25, 211, 243, 1.0)","rgba(255, 102, 146, 1.0)","rgba(182, 232, 128, 1.0)","rgba(255, 151, 255, 1.0)","rgba(254, 203, 82, 1.0)"]},"data":{"bar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}},"error_x":{"color":"rgba(42, 63, 95, 1.0)"},"error_y":{"color":"rgba(42, 63, 95, 1.0)"}}],"barpolar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"carpet":[{"aaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"},"baxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"}}],"choropleth":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contourcarpet":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"heatmap":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"heatmapgl":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram":[{"marker":{"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"histogram2d":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram2dcontour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"mesh3d":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"parcoords":[{"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"pie":[{"automargin":true}],"scatter":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatter3d":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattercarpet":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergeo":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattermapbox":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolar":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolargl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterternary":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"surface":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"table":[{"cells":{"fill":{"color":"rgba(235, 240, 248, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}},"header":{"fill":{"color":"rgba(200, 212, 227, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}}}]}},"title":{"text":"Example Plot"},"xaxis":{"title":{"text":"Date"}},"yaxis":{"title":{"text":"Cumulative Excess Return"}}};
            var config = {"responsive":true};
            Plotly.newPlot('b57c8baf-922d-4bc4-bab4-47b517d28aff', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_b57c8baf922d4bc4bab447b517d28aff();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_b57c8baf922d4bc4bab447b517d28aff();
            }
</script>

Write your solution in the cell below.

*)
// Solution here
(**
> **Task:** Calculate the standard deviation of the `excessSPY` monthly returns from 2010-01 to 2019-12 (inclusive). Assign it to a value named `stdDev2010s`.
> 

Write your solution in the cell below.

*)
// Solution here

// Test
stdDev2010s 
|> should (equalWithin 0.005) 0.035
(**
> **Task:** Calculate the average monthly excess returns of SPY from 2010-01 to 2019-12 (inclusive). Assign it to a value named `mu2010s`.
> 

Write your solution in the cell below.

*)
// Solution here

// Test
mu2010s
|> should (equalWithin 0.005) 0.01
(**
> **Task:** If you are a mean-variance investor with a risk aversion parameter $\gamma=3$, what is the optimal weight of SPY in your portfolio over the period 2010-1 to 2019-12? Use your $\mu$ and $\sigma$ from the same period. Assign it to a value named `optimalWeight2010s`.
> 

Write your solution in the cell below.

*)
// Solution here

// Test
optimalWeight2010s
|> should (equalWithin 0.1) 2.75
(**
> **Task:** Given that optimal weight in SPY for the 2010s, do you think the 2010s were a good decade to be invested in SPY? Why or why not? Explain using your estimate of the `optimalWeight2010s` as part of your justification.
> 

Write your solution in the cell below.

*)
// Solution here
(**
> **Task:** The `optimalWeight2010s` is close to 2.75. Use a weight of 2.75 to invest in SPY excess returns from 2020-01 to the end of the sample (inclusive). What is the cumulative excess return of this portfolio? Assign it to a value named `cumulativeExcessReturn2020s`.
> 

Write your solution in the cell below.

*)
// Solution here

// Test
cumulativeExcessReturn2020s
|> should (equalWithin 0.05) 0.35
(**
> **Task:** Plot the cumulative **excess** return of an investment in SPY levered to 2.75 from the 2020-01 to the end of the sample. The date should be the x-axis and the cumulative excess return should be the y-axis.
> 

Write your solution in the cell below.

*)
// Solution here
(**
> **Task:** If you are a mean-variance investor with a risk aversion parameter $\gamma=3$, what is the optimal weight of SPY in your portfolio over the period 2020-1 to the end of the sample? Use $\mu$ and $\sigma$ estimated from 2020-01 to the end of the sample to form your estimate. Assign it to a value named `optimalWeight2020s`.
> 

Write your solution in the cell below.

*)
// Solution here

// Test
optimalWeight2020s
|> should (equalWithin 0.1) 0.7
(**
> **Task:** Why is the optimal weight from the 2010s so different from the 2020s? Be specific and justify your answer using the data. What do we learn from this?
> 

Write your solution in the cell below.

*)
// Solution here

