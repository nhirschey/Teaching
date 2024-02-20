(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=optimization.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//optimization.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//optimization.ipynb)

*)
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: FSharp.Data, 5.0.2"
#r "nuget: DiffSharp-lite"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"
#r "nuget: Quotes.YahooFinance, 0.0.5"
#r "nuget: NovaSBE.Finance, 0.5.0"

open System
open FSharp.Data
open Quotes.YahooFinance
open NovaSBE.Finance.French
open FSharp.Stats
open Plotly.NET
open DiffSharp

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
(**
# Portfolio Optimization

We're now going to see how to do mean-variance portfolio optimization.
The objective is to find the portfolio with the greatest return per
unit of standard deviation.

In particular, we're going to identify the tangency portfolio.
The tangency portfolio is the portfolio fully invested
in risky assets that has the maximum achievable sharpe ratio.
When there is a risk-free rate, the efficient frontier
of optimal portfolios is some combination of
the tangency portfolio and the risk-free asset.
Investors who want safe portfolios hold a lot
of risk-free bonds and very little of the tangency portfolio.
Investors who want riskier portfolios hold little risk-free bonds and
a lot of the tangency portfolio (or even lever the tangency portoflio).

One thing to keep in mind is that often you think
of this as the optimal weight per security.
But one well known problem is that trying to do this naively does not work well.
And by naively I mean taking a stock's average return and covariances in the sample and using that to estimate optimal weights.
In large part, this is because it is hard to estimate a stock's future returns.
I know. Big shock, right?

However, there are ways to do portfolio optimization that work better.
We can do it by creating large groups
of stocks with similar characteristics.
For example, a factor portfolio.
Then you estimate the expected return and covariance matrix using the factor.
That tends to give you better portfolios
because the characteristics that you're using
to form the portfolios help you estimate
the return and covariances of the stocks in it.

A type to hold our data.

*)
type StockData =
    { Symbol : string 
      Date : DateTime
      Return : float }
(**
We get the Fama-French 3-Factor asset pricing model data.

*)
let ff3 = getFF3 Frequency.Monthly |> Array.toList

// Transform to a StockData record type.
let ff3StockData =
    [ 
       ff3 |> List.map(fun x -> {Symbol="HML";Date=x.Date;Return=x.Hml})
       ff3 |> List.map(fun x -> {Symbol="MktRf";Date=x.Date;Return=x.MktRf})
       ff3 |> List.map(fun x -> {Symbol="Smb";Date=x.Date;Return=x.Smb})
    ] |> List.concat
(**
Let's get our factor data.

*)
let tickers = 
    [ 
        "VBR" // Vanguard Small-cap Value ETF
        "VUG" // Vanguard Growth ETF
        "VTI" // Vanguard Total Stock Market ETF
        "BND" // Vanguard Total Bond Market ETF
    ]

let tickPrices = 
    YahooFinance.History(
        tickers,
        startDate = DateTime(2010,1,1),
        interval = Interval.Monthly)

tickPrices[..3]
(**
A function to calculate returns from Price observations

*)
let pricesToReturns (symbol, adjPrices: list<Quote>) =
    adjPrices
    |> List.sortBy (fun x -> x.Date)
    |> List.pairwise
    |> List.map (fun (day0, day1) ->
        let r = day1.AdjustedClose / day0.AdjustedClose - 1.0 
        { Symbol = symbol 
          Date = day1.Date 
          Return = r })

let testPriceObs = 
    tickPrices
    |> List.filter (fun x -> x.Symbol = tickers[0])
    |> List.truncate 4
(**
Looking at the results of grouping test prices by symbol.

*)
testPriceObs
|> List.groupBy (fun x -> x.Symbol)
(**
Same but calculating return observations.

*)
testPriceObs
|> List.groupBy (fun x -> x.Symbol)
|> List.collect pricesToReturns
(**
Now for all of the price data.

*)
let tickReturns =
    tickPrices
    |> List.groupBy (fun x -> x.Symbol)
    |> List.collect pricesToReturns
(**
And let's convert to excess returns

*)
let rf = Map [ for x in ff3 do x.Date, x.Rf ]

let standardInvestmentsExcess =
    let maxff3Date = ff3 |> List.map(fun x -> x.Date) |> List.max
    tickReturns
    |> List.filter(fun x -> x.Date <= maxff3Date)
    |> List.map(fun x -> 
        match Map.tryFind x.Date rf with 
        | None -> failwith $"why isn't there a rf for {x.Date}"
        | Some rf -> { x with Return = x.Return - rf })
(**
If we did it right, the `VTI` return should be pretty similar to the `MktRF`
return from Ken French's website.

*)
standardInvestmentsExcess
|> List.filter(fun x -> x.Symbol = "VTI" && x.Date.Year = 2021)
|> List.map(fun x -> x.Date.Month, round 4 x.Return)
|> List.take 3(* output: 
val it: (int * float) list = [(1, 0.0006); (2, 0.0314); (3, 0.033)]*)
ff3 
|> List.filter(fun x -> x.Date.Year = 2021)
|> List.map(fun x -> x.Date.Month, round 4 x.MktRf)
|> List.take 3(* output: 
val it: (int * float) list = [(1, -0.0003); (2, 0.0278); (3, 0.0308)]*)
(**
Let's create a function that calculates covariances
for two securities using mutually overlapping data.

*)
/// Excess return by symbol
let returnMap =
    standardInvestmentsExcess
    |> List.map (fun x -> (x.Symbol, x.Date), x.Return)
    |> Map

returnMap["VTI", DateTime(2015,12,1)]
(**
Now the cov

*)
let getCov xId yId =
    let xs = 
        standardInvestmentsExcess
        |> List.filter (fun x -> x.Symbol = xId)
    [ for x in xs do
        let yLookup = yId, x.Date
        if returnMap.ContainsKey yLookup then
            x.Return, returnMap[yLookup]]
    |> covOfPairs

getCov "VBR" "VTI"
(**
A covariance matrix.

*)
let covariances =
    [ for rowTick in tickers do 
        [ for colTick in tickers do
            getCov rowTick colTick ]]
    |> dsharp.tensor
(**
Mean/Average returns

*)
let meansByTick =
    standardInvestmentsExcess
    |> List.groupBy (fun x -> x.Symbol)
    |> List.map (fun (sym, xs) ->
        let symAvg = xs |> List.averageBy (fun x -> x.Return)
        sym, symAvg)
    |> Map

let means =
    // Make sure ticker avg returns are ordered by our ticker list
    [ for ticker in tickers do meansByTick[ticker]]
    |> dsharp.tensor
(**
This solution method for finding the tangency portfolio comes from Hilliar, Grinblatt, and Titman 2nd European Edition, Example 5.3.

Since it has the greatest possible Sharpe ratio, that means that you cannot rebalance the portfolio and increase the return per unit of standard deviation.

The solution method relies on the fact that covariance is like marginal variance. At the tangency portfolio, it must be the case that the ratio of each asset's risk premium to it's covariance with the tangency portfolio, $(r_i-r_f)/cov(r_i,r_p)$, is the same. Because that ratio is the return per unit of "marginal variance" and if it was not equal for all assets, then you could rebalance and increase the portfolio's return while holding the portfolio variance constant.

In the below algebra, we solve for the portfolio that has covariances with each asset equal to the asset's risk premium. Then we relever to have a portfolio weight equal to 1.0 (we can relever like this because everything is in excess returns) and then we are left with the tangency portfolio.

solve A * x = b for x

*)
let w' = dsharp.solve(covariances,means)
let w = w' / w'.sum()(* output: 
<null>*)
(**
Portfolio variance

*)
let portVariance = w.matmul(covariances).matmul(w)
(**
Portfolio standard deviation

*)
let portStDev = portVariance.sqrt()
(**
Portfolio mean

*)
let portMean = dsharp.matmul(w, means)

// equivalent to
(w * means).sum()
(**
Annualized Sharpe ratio

*)
sqrt(12.0)*(portMean/portStDev)(* output: 
tensor(0.9099)*)
(**
You can use other methods for constrained optimization.

[https://github.com/mathnet/mathnet-numerics/blob/ab1ac92ccab575d51f6967cd785254a46210b4dd/src/Numerics.Tests/OptimizationTests/NonLinearCurveFittingTests.cs#L361](https://github.com/mathnet/mathnet-numerics/blob/ab1ac92ccab575d51f6967cd785254a46210b4dd/src/Numerics.Tests/OptimizationTests/NonLinearCurveFittingTests.cs#L361)

## Comparing mean-variance efficient to 60/40.

Now let's form the mean-variance efficient portfolios
based on the above optimal weights and compare them to
a 60/40 portfolio over our sample. A 60% stock and 40%
bond portfolio is a common investment portfolio.

Our weights are sorted by `symbols`. Let's put them into a
Map collection for easier referencing.

*)
let weights =
    Seq.zip tickers (w.toArray1D<float>())
    |> Map.ofSeq
(**
Next, we'd like to get the symbol data grouped by date.

*)
let stockDataByDate =
    standardInvestmentsExcess
    |> List.groupBy(fun x -> x.Date) // group all symbols on the same date together.
    |> List.sortBy (fun (dt, xs) -> dt) 
(**
Now if we look we do not have all the symbols on all the dates.
This is because our ETFs (VTI, BND) did not start trading until later in our sample
and our strategy ended at the end of the year, while we have stock quotes
for VTI and BND trading recently.

Compare the first month of data

*)
let (firstMonth, firstMonthDta) = stockDataByDate[0]

// look at it
firstMonthDta(* output: 
[{ Symbol = "VBR"
   Date = 2/1/2010 12:00:00 AM
   Return = 0.05014212809 }; { Symbol = "VUG"
                               Date = 2/1/2010 12:00:00 AM
                               Return = 0.03872004654 };
 { Symbol = "VTI"
   Date = 2/1/2010 12:00:00 AM
   Return = 0.03440021749 }; { Symbol = "BND"
                               Date = 2/1/2010 12:00:00 AM
                               Return = 0.000125314496 }]*)
// How many stocks?
firstMonthDta.Length(* output: 
4*)
(**
to the last month of data

*)
// Last item
let lastMonth =
    stockDataByDate 
    |> List.last // last date group
    |> snd // convert (date, StockData list) -> StockData list
// look at it
lastMonth(* output: 
[{ Symbol = "VBR"
   Date = 12/1/2023 12:00:00 AM
   Return = 0.08702258748 }; { Symbol = "VUG"
                               Date = 12/1/2023 12:00:00 AM
                               Return = 0.03692989634 };
 { Symbol = "VTI"
   Date = 12/1/2023 12:00:00 AM
   Return = 0.04413987918 }; { Symbol = "BND"
                               Date = 12/1/2023 12:00:00 AM
                               Return = 0.02843875704 }]*)
// How many stocks?
lastMonth.Length(* output: 
4*)
(**
What's the first month when you have all 3 assets?

*)
let allAssetsStart =
    stockDataByDate
    // find the first array element where there are as many stocks as you have symbols
    |> List.find(fun (month, stocks) -> stocks.Length = tickers.Length)
    |> fst // convert (month, stocks) to month

let allAssetsEnd =
    stockDataByDate
    // find the last array element where there are as many stocks as you have symbols
    |> List.findBack(fun (month, stocks) -> stocks.Length = tickers.Length)
    |> fst // convert (month, stocks) to month
(**
Ok, let's filter our data between those dates.

*)
let stockDataByDateComplete =
    stockDataByDate
    |> List.filter(fun (date, stocks) -> 
        date >= allAssetsStart &&
        date <= allAssetsEnd)
(**
Double check that we have all assets in all months for this data.

*)
let checkOfCompleteData =
    stockDataByDateComplete
    |> List.map snd
    |> List.filter(fun x -> x.Length <> tickers.Length) // discard rows where we have all symbols.

if not (List.isEmpty checkOfCompleteData) then 
        failwith "stockDataByDateComplete has months with missing stocks"
(**
Now let's make my mve and 60/40 ports

To start, let's take a test month so that it is easy to see
what we are doing.

*)
let testMonth =
    stockDataByDateComplete
    |> List.find(fun (date, stocks) -> date = allAssetsStart)
    |> snd

let testBnd = testMonth |> List.find(fun x -> x.Symbol = "BND")
let testVti = testMonth |> List.find(fun x -> x.Symbol = "VTI")
let testVBR = testMonth |> List.find(fun x -> x.Symbol = "VBR")

testBnd.Return*weights.["BND"] +
testVti.Return*weights.["VTI"] +
testVBR.Return*weights.["VBR"](* output: 
0.01717009222*)
(**
Or, same thing but via iterating through the weights.

*)
[ for KeyValue(symbol, weight) in weights do
    let symbolData = testMonth |> List.find(fun x -> x.Symbol = symbol)
    symbolData.Return*weight]
|> List.sum    (* output: 
0.01982146964*)
(**
Now in a function that takes weights and monthData as input

*)
let portfolioMonthReturn weights monthData =
    weights
    |> Map.toList
    |> List.map(fun (symbol, weight) ->
        let symbolData = 
            // we're going to be more safe and use tryFind here so
            // that our function is more reusable
            match monthData |> List.tryFind(fun x -> x.Symbol = symbol) with
            | None -> failwith $"You tried to find {symbol} in the data but it was not there"
            | Some data -> data
        symbolData.Return*weight)
    |> List.sum    
    
portfolioMonthReturn weights testMonth
(**
Here's a thought. We just made a function that takes
weights and a month as input. That means that it should
work if we give it different weights.

Let's try to give it 60/40 weights.

*)
let weights6040 = Map [("VTI",0.6);("BND",0.4)]

weights6040.["VTI"]*testVti.Return +
weights6040.["BND"]*testBnd.Return(* output: 
0.02069025629*)
(**
Now compare to

*)
portfolioMonthReturn weights6040 testMonth(* output: 
0.02069025629*)
(**
Now we're ready to make our mve and 60/40 portfolios.

*)
let portMve =
    stockDataByDateComplete
    |> List.map(fun (date, data) -> 
        { Symbol = "MVE"
          Date = date
          Return = portfolioMonthReturn weights data })

let port6040 = 
    stockDataByDateComplete
    |> List.map(fun (date, data) -> 
        { Symbol = "60/40"
          Date = date 
          Return = portfolioMonthReturn weights6040 data} )
(**
It is nice to plot cumulative returns.

A function to accumulate returns.

*)
let cumulateReturns (xs: list<StockData>) =
    let mutable cr = 1.0
    [ for x in xs do 
        cr <- cr * (1.0 + x.Return)
        { x with Return = cr } ]
    
(**
Ok, cumulative returns.

*)
let portMveCumulative = 
    portMve
    |> cumulateReturns

let port6040Cumulative = 
    port6040
    |> cumulateReturns


let chartMVE = 
    portMveCumulative
    |> List.map(fun x -> x.Date, x.Return)
    |> Chart.Line
    |> Chart.withTraceInfo(Name="MVE")

let chart6040 = 
    port6040Cumulative
    |> List.map(fun x -> x.Date, x.Return)
    |> Chart.Line
    |> Chart.withTraceInfo(Name="60/40")

let chartCombined =
    [ chartMVE; chart6040 ]
    |> Chart.combine
chartCombined |> Chart.show(* output: 
<div id="2c1edf38-b5b5-4e1c-a70c-7956f56d5e3c"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_2c1edf38b5b54e1ca70c7956f56d5e3c = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-2.6.3.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","mode":"lines","x":["2010-02-01T00:00:00","2010-03-01T00:00:00","2010-04-01T00:00:00","2010-05-01T00:00:00","2010-06-01T00:00:00","2010-07-01T00:00:00","2010-08-01T00:00:00","2010-09-01T00:00:00","2010-10-01T00:00:00","2010-11-01T00:00:00","2010-12-01T00:00:00","2011-01-01T00:00:00","2011-02-01T00:00:00","2011-03-01T00:00:00","2011-04-01T00:00:00","2011-05-01T00:00:00","2011-06-01T00:00:00","2011-07-01T00:00:00","2011-08-01T00:00:00","2011-09-01T00:00:00","2011-10-01T00:00:00","2011-11-01T00:00:00","2011-12-01T00:00:00","2012-01-01T00:00:00","2012-02-01T00:00:00","2012-03-01T00:00:00","2012-04-01T00:00:00","2012-05-01T00:00:00","2012-06-01T00:00:00","2012-07-01T00:00:00","2012-08-01T00:00:00","2012-09-01T00:00:00","2012-10-01T00:00:00","2012-11-01T00:00:00","2012-12-01T00:00:00","2013-01-01T00:00:00","2013-02-01T00:00:00","2013-03-01T00:00:00","2013-04-01T00:00:00","2013-05-01T00:00:00","2013-06-01T00:00:00","2013-07-01T00:00:00","2013-08-01T00:00:00","2013-09-01T00:00:00","2013-10-01T00:00:00","2013-11-01T00:00:00","2013-12-01T00:00:00","2014-01-01T00:00:00","2014-02-01T00:00:00","2014-03-01T00:00:00","2014-04-01T00:00:00","2014-05-01T00:00:00","2014-06-01T00:00:00","2014-07-01T00:00:00","2014-08-01T00:00:00","2014-09-01T00:00:00","2014-10-01T00:00:00","2014-11-01T00:00:00","2014-12-01T00:00:00","2015-01-01T00:00:00","2015-02-01T00:00:00","2015-03-01T00:00:00","2015-04-01T00:00:00","2015-05-01T00:00:00","2015-06-01T00:00:00","2015-07-01T00:00:00","2015-08-01T00:00:00","2015-09-01T00:00:00","2015-10-01T00:00:00","2015-11-01T00:00:00","2015-12-01T00:00:00","2016-01-01T00:00:00","2016-02-01T00:00:00","2016-03-01T00:00:00","2016-04-01T00:00:00","2016-05-01T00:00:00","2016-06-01T00:00:00","2016-07-01T00:00:00","2016-08-01T00:00:00","2016-09-01T00:00:00","2016-10-01T00:00:00","2016-11-01T00:00:00","2016-12-01T00:00:00","2017-01-01T00:00:00","2017-02-01T00:00:00","2017-03-01T00:00:00","2017-04-01T00:00:00","2017-05-01T00:00:00","2017-06-01T00:00:00","2017-07-01T00:00:00","2017-08-01T00:00:00","2017-09-01T00:00:00","2017-10-01T00:00:00","2017-11-01T00:00:00","2017-12-01T00:00:00","2018-01-01T00:00:00","2018-02-01T00:00:00","2018-03-01T00:00:00","2018-04-01T00:00:00","2018-05-01T00:00:00","2018-06-01T00:00:00","2018-07-01T00:00:00","2018-08-01T00:00:00","2018-09-01T00:00:00","2018-10-01T00:00:00","2018-11-01T00:00:00","2018-12-01T00:00:00","2019-01-01T00:00:00","2019-02-01T00:00:00","2019-03-01T00:00:00","2019-04-01T00:00:00","2019-05-01T00:00:00","2019-06-01T00:00:00","2019-07-01T00:00:00","2019-08-01T00:00:00","2019-09-01T00:00:00","2019-10-01T00:00:00","2019-11-01T00:00:00","2019-12-01T00:00:00","2020-01-01T00:00:00","2020-02-01T00:00:00","2020-03-01T00:00:00","2020-04-01T00:00:00","2020-05-01T00:00:00","2020-06-01T00:00:00","2020-07-01T00:00:00","2020-08-01T00:00:00","2020-09-01T00:00:00","2020-10-01T00:00:00","2020-11-01T00:00:00","2020-12-01T00:00:00","2021-01-01T00:00:00","2021-02-01T00:00:00","2021-03-01T00:00:00","2021-04-01T00:00:00","2021-05-01T00:00:00","2021-06-01T00:00:00","2021-07-01T00:00:00","2021-08-01T00:00:00","2021-09-01T00:00:00","2021-10-01T00:00:00","2021-11-01T00:00:00","2021-12-01T00:00:00","2022-01-01T00:00:00","2022-02-01T00:00:00","2022-03-01T00:00:00","2022-04-01T00:00:00","2022-05-01T00:00:00","2022-06-01T00:00:00","2022-07-01T00:00:00","2022-08-01T00:00:00","2022-09-01T00:00:00","2022-10-01T00:00:00","2022-11-01T00:00:00","2022-12-01T00:00:00","2023-01-01T00:00:00","2023-02-01T00:00:00","2023-03-01T00:00:00","2023-04-01T00:00:00","2023-05-01T00:00:00","2023-06-01T00:00:00","2023-07-01T00:00:00","2023-08-01T00:00:00","2023-09-01T00:00:00","2023-10-01T00:00:00","2023-11-01T00:00:00","2023-12-01T00:00:00"],"y":[1.019821469644694,1.0584230626669766,1.0532949228910002,0.9775397970779348,0.9360672765194682,1.0026322161159995,0.9752994581665397,1.0447962111250757,1.0976875229400453,1.0873628917845757,1.1552078139808355,1.1842600846716398,1.214645829328605,1.200504388889922,1.2479774097922813,1.247462199996444,1.2206651360497485,1.2165389005522151,1.169755954387133,1.1070654028385951,1.20821088708464,1.201930656586877,1.2220758805664376,1.2537324352643375,1.31933250421309,1.3540724250034186,1.3605012117707562,1.2839260504701788,1.3154029262757785,1.352278909469247,1.3837344828057982,1.4003065431942683,1.3745317593513005,1.3831733841832117,1.3859320509668938,1.4312425104706243,1.444388397766125,1.4790163438068242,1.5307629050582667,1.5546068369694364,1.5171629081905207,1.5977719766401042,1.5674631475088803,1.5958974046961785,1.6777722091768064,1.7159685892842467,1.7684024153553843,1.7042617492891081,1.7808906807922082,1.7625209235805621,1.793665132410016,1.8382979980497078,1.8451874601209997,1.8642083988822298,1.9238826736094143,1.9234924013200565,1.9572640961326888,2.0252994699166615,2.003569163730253,1.95862973860055,2.0624213676767944,1.9825559019953634,2.0406567141689944,2.063539257029678,2.012574841958188,2.1041934137225797,1.9643040181141769,1.923396517719954,2.0841117378431604,2.082749333788085,2.077676847227198,1.9874548034421478,1.9637517074495119,2.0556354113146584,2.053522810384252,2.0926619247299683,2.0908973267197606,2.1695431391034017,2.1631565205134193,2.15833666609233,2.131778449781297,2.1158543496195388,2.126935846534786,2.19809110655552,2.303025564831955,2.3089841373442472,2.356395454270189,2.4440845454476374,2.420933898933034,2.492032799922502,2.5283221245419525,2.521749149574208,2.6061669015289057,2.6731037466611185,2.7044874057166823,2.9129646214088836,2.8296207356359018,2.704819516169088,2.7206152415091895,2.7516216063136927,2.7621914694108174,2.871543945322408,2.984877902649064,3.0178138067271196,2.8487494856584172,2.8876889259046914,2.681443363364608,2.8488275848725797,2.9274235249735474,3.02784664463647,3.1505099420674325,3.001913489528302,3.1950891538412844,3.255255325445857,3.2828223601520863,3.253934315546816,3.339191971277721,3.4867808747382796,3.560922507353667,3.67308168043697,3.4672184260832983,3.307860308036624,3.741675843711477,3.951839126065876,4.036205334768439,4.354089790122058,4.730216388374077,4.566286856431719,4.305899535677418,4.58609818350419,4.696845724612053,4.5997633717686,4.516824870440928,4.589001434641246,4.86029984975638,4.807695986823437,5.053322822065559,5.28942197927983,5.4635745452364946,5.137425405475529,5.559431150303027,5.561369156282936,5.692356175467745,5.283298695204737,4.978978909566273,5.171427867515658,4.6028633505214005,4.510012871862806,4.218816809111821,4.596014766752752,4.379811165543718,3.9915731055976624,4.158786905318862,4.342731816486358,4.065855607322737,4.275604704904568,4.154828586798299,4.528475998514288,4.641798156540858,4.794236945763614,4.958873931721888,5.044504997837248,4.990246917012056,4.725971060989809,4.675776095791138,5.11310696217147,5.158720384615622],"marker":{},"line":{},"name":"MVE"},{"type":"scatter","mode":"lines","x":["2010-02-01T00:00:00","2010-03-01T00:00:00","2010-04-01T00:00:00","2010-05-01T00:00:00","2010-06-01T00:00:00","2010-07-01T00:00:00","2010-08-01T00:00:00","2010-09-01T00:00:00","2010-10-01T00:00:00","2010-11-01T00:00:00","2010-12-01T00:00:00","2011-01-01T00:00:00","2011-02-01T00:00:00","2011-03-01T00:00:00","2011-04-01T00:00:00","2011-05-01T00:00:00","2011-06-01T00:00:00","2011-07-01T00:00:00","2011-08-01T00:00:00","2011-09-01T00:00:00","2011-10-01T00:00:00","2011-11-01T00:00:00","2011-12-01T00:00:00","2012-01-01T00:00:00","2012-02-01T00:00:00","2012-03-01T00:00:00","2012-04-01T00:00:00","2012-05-01T00:00:00","2012-06-01T00:00:00","2012-07-01T00:00:00","2012-08-01T00:00:00","2012-09-01T00:00:00","2012-10-01T00:00:00","2012-11-01T00:00:00","2012-12-01T00:00:00","2013-01-01T00:00:00","2013-02-01T00:00:00","2013-03-01T00:00:00","2013-04-01T00:00:00","2013-05-01T00:00:00","2013-06-01T00:00:00","2013-07-01T00:00:00","2013-08-01T00:00:00","2013-09-01T00:00:00","2013-10-01T00:00:00","2013-11-01T00:00:00","2013-12-01T00:00:00","2014-01-01T00:00:00","2014-02-01T00:00:00","2014-03-01T00:00:00","2014-04-01T00:00:00","2014-05-01T00:00:00","2014-06-01T00:00:00","2014-07-01T00:00:00","2014-08-01T00:00:00","2014-09-01T00:00:00","2014-10-01T00:00:00","2014-11-01T00:00:00","2014-12-01T00:00:00","2015-01-01T00:00:00","2015-02-01T00:00:00","2015-03-01T00:00:00","2015-04-01T00:00:00","2015-05-01T00:00:00","2015-06-01T00:00:00","2015-07-01T00:00:00","2015-08-01T00:00:00","2015-09-01T00:00:00","2015-10-01T00:00:00","2015-11-01T00:00:00","2015-12-01T00:00:00","2016-01-01T00:00:00","2016-02-01T00:00:00","2016-03-01T00:00:00","2016-04-01T00:00:00","2016-05-01T00:00:00","2016-06-01T00:00:00","2016-07-01T00:00:00","2016-08-01T00:00:00","2016-09-01T00:00:00","2016-10-01T00:00:00","2016-11-01T00:00:00","2016-12-01T00:00:00","2017-01-01T00:00:00","2017-02-01T00:00:00","2017-03-01T00:00:00","2017-04-01T00:00:00","2017-05-01T00:00:00","2017-06-01T00:00:00","2017-07-01T00:00:00","2017-08-01T00:00:00","2017-09-01T00:00:00","2017-10-01T00:00:00","2017-11-01T00:00:00","2017-12-01T00:00:00","2018-01-01T00:00:00","2018-02-01T00:00:00","2018-03-01T00:00:00","2018-04-01T00:00:00","2018-05-01T00:00:00","2018-06-01T00:00:00","2018-07-01T00:00:00","2018-08-01T00:00:00","2018-09-01T00:00:00","2018-10-01T00:00:00","2018-11-01T00:00:00","2018-12-01T00:00:00","2019-01-01T00:00:00","2019-02-01T00:00:00","2019-03-01T00:00:00","2019-04-01T00:00:00","2019-05-01T00:00:00","2019-06-01T00:00:00","2019-07-01T00:00:00","2019-08-01T00:00:00","2019-09-01T00:00:00","2019-10-01T00:00:00","2019-11-01T00:00:00","2019-12-01T00:00:00","2020-01-01T00:00:00","2020-02-01T00:00:00","2020-03-01T00:00:00","2020-04-01T00:00:00","2020-05-01T00:00:00","2020-06-01T00:00:00","2020-07-01T00:00:00","2020-08-01T00:00:00","2020-09-01T00:00:00","2020-10-01T00:00:00","2020-11-01T00:00:00","2020-12-01T00:00:00","2021-01-01T00:00:00","2021-02-01T00:00:00","2021-03-01T00:00:00","2021-04-01T00:00:00","2021-05-01T00:00:00","2021-06-01T00:00:00","2021-07-01T00:00:00","2021-08-01T00:00:00","2021-09-01T00:00:00","2021-10-01T00:00:00","2021-11-01T00:00:00","2021-12-01T00:00:00","2022-01-01T00:00:00","2022-02-01T00:00:00","2022-03-01T00:00:00","2022-04-01T00:00:00","2022-05-01T00:00:00","2022-06-01T00:00:00","2022-07-01T00:00:00","2022-08-01T00:00:00","2022-09-01T00:00:00","2022-10-01T00:00:00","2022-11-01T00:00:00","2022-12-01T00:00:00","2023-01-01T00:00:00","2023-02-01T00:00:00","2023-03-01T00:00:00","2023-04-01T00:00:00","2023-05-01T00:00:00","2023-06-01T00:00:00","2023-07-01T00:00:00","2023-08-01T00:00:00","2023-09-01T00:00:00","2023-10-01T00:00:00","2023-11-01T00:00:00","2023-12-01T00:00:00"],"y":[1.0206902562922027,1.0563493438212637,1.07709567172442,1.030954411979217,0.9978793600829948,1.0464372569412232,1.0232679253715957,1.0778280207862792,1.1086308502451914,1.1088682548915305,1.1435942214562724,1.1662655689042287,1.1920808158627383,1.191336965270364,1.2219911733353446,1.2200297082787952,1.2019823277787645,1.19649632859841,1.1606026665228588,1.1076251164034336,1.1875809999619347,1.1846565293840272,1.1897612711719576,1.2378481006882875,1.2681782659672272,1.2856464968370458,1.2891831592795489,1.2462501812168607,1.2729338408528181,1.289790321835338,1.3114159480094774,1.32794503947781,1.3167617063820491,1.3241370971361694,1.3226907161516477,1.3692588386558007,1.3816255750022526,1.4109504627640241,1.4331722183686686,1.444122515488267,1.4181083486327029,1.4733557569041176,1.4415085971451236,1.4772670290270764,1.5246245053017013,1.5474947650024538,1.5621280550104641,1.55017058755815,1.5970353775486734,1.5966358664699294,1.6063929271723256,1.6335229530893343,1.6556310466245712,1.6380500662894293,1.686317537360725,1.6567046760728315,1.693586168870395,1.7243263691017925,1.7157848649828191,1.7140669046225039,1.7627969000078814,1.7495158145580312,1.7584202535007465,1.7688758043000135,1.7387027296010826,1.7671183961110744,1.7007490725984755,1.6714156932788582,1.756498614518475,1.7600215331974074,1.7283380255120466,1.6861199440605363,1.689971117594314,1.762722114244543,1.777271418114521,1.7956175517885093,1.807912539284813,1.8600581022694125,1.859653092457319,1.856916839745657,1.8304989738963002,1.8607598092915034,1.8758519557040694,1.907955840343988,1.9526021373182942,1.9471940265905374,1.9700107538557645,1.9864481585948885,1.991407175360767,2.0213226318790873,2.0299324663259473,2.0503366125511704,2.0804277909988356,2.115646922681191,2.12463401225477,2.1887468597592186,2.125989554580698,2.099495024718086,2.099836765485044,2.137125394751411,2.1372597004805876,2.183723359741646,2.230971172474967,2.2190041310302635,2.114393166017159,2.1413709900803735,2.026276441546409,2.1466886647528516,2.185767541902368,2.210141881514643,2.2646428397536855,2.1888417658182906,2.283620057178646,2.3051791846049454,2.2982281740826176,2.307113845845424,2.3419706471790533,2.3919652426056572,2.4181113544856405,2.445407347708235,2.3392902717043844,2.1221460960260354,2.3197464651490702,2.4010504060582325,2.433750818919043,2.5386441783432234,2.637104669802548,2.573642912470507,2.5437969097730395,2.73601398784356,2.80372859169702,2.8004604673195717,2.8341196672873368,2.8759844532648704,2.978007038156791,2.988860719339487,3.038441729810779,3.090033450638694,3.1406532536204175,3.0381404432150543,3.167086403385734,3.14185750111477,3.198166000639235,3.0679179126470864,3.0061763186006916,3.025880783582385,2.815787723343181,2.8207687740130596,2.6547400554874243,2.833697057844238,2.733200566348914,2.5245036119460087,2.6367472614978436,2.7494036516352702,2.6249041243141913,2.7727216623078204,2.691209221802774,2.7474966750915026,2.768590550013627,2.752875422998392,2.843695148726972,2.8988389066375317,2.8443917133350354,2.7160091309141707,2.649403054516596,2.8353098293377106,2.9426530442678276],"marker":{},"line":{},"name":"60/40"}];
            var layout = {"width":600,"height":600,"template":{"layout":{"title":{"x":0.05},"font":{"color":"rgba(42, 63, 95, 1.0)"},"paper_bgcolor":"rgba(255, 255, 255, 1.0)","plot_bgcolor":"rgba(229, 236, 246, 1.0)","autotypenumbers":"strict","colorscale":{"diverging":[[0.0,"#8e0152"],[0.1,"#c51b7d"],[0.2,"#de77ae"],[0.3,"#f1b6da"],[0.4,"#fde0ef"],[0.5,"#f7f7f7"],[0.6,"#e6f5d0"],[0.7,"#b8e186"],[0.8,"#7fbc41"],[0.9,"#4d9221"],[1.0,"#276419"]],"sequential":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]],"sequentialminus":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]},"hovermode":"closest","hoverlabel":{"align":"left"},"coloraxis":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"geo":{"showland":true,"landcolor":"rgba(229, 236, 246, 1.0)","showlakes":true,"lakecolor":"rgba(255, 255, 255, 1.0)","subunitcolor":"rgba(255, 255, 255, 1.0)","bgcolor":"rgba(255, 255, 255, 1.0)"},"mapbox":{"style":"light"},"polar":{"bgcolor":"rgba(229, 236, 246, 1.0)","radialaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""},"angularaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""}},"scene":{"xaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"yaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"zaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true}},"ternary":{"aaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"baxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"caxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"bgcolor":"rgba(229, 236, 246, 1.0)"},"xaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"yaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"annotationdefaults":{"arrowcolor":"#2a3f5f","arrowhead":0,"arrowwidth":1},"shapedefaults":{"line":{"color":"rgba(42, 63, 95, 1.0)"}},"colorway":["rgba(99, 110, 250, 1.0)","rgba(239, 85, 59, 1.0)","rgba(0, 204, 150, 1.0)","rgba(171, 99, 250, 1.0)","rgba(255, 161, 90, 1.0)","rgba(25, 211, 243, 1.0)","rgba(255, 102, 146, 1.0)","rgba(182, 232, 128, 1.0)","rgba(255, 151, 255, 1.0)","rgba(254, 203, 82, 1.0)"]},"data":{"bar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}},"error_x":{"color":"rgba(42, 63, 95, 1.0)"},"error_y":{"color":"rgba(42, 63, 95, 1.0)"}}],"barpolar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"carpet":[{"aaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"},"baxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"}}],"choropleth":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contourcarpet":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"heatmap":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"heatmapgl":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram":[{"marker":{"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"histogram2d":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram2dcontour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"mesh3d":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"parcoords":[{"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"pie":[{"automargin":true}],"scatter":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatter3d":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattercarpet":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergeo":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattermapbox":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolar":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolargl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterternary":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"surface":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"table":[{"cells":{"fill":{"color":"rgba(235, 240, 248, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}},"header":{"fill":{"color":"rgba(200, 212, 227, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}}}]}}};
            var config = {"responsive":true};
            Plotly.newPlot('2c1edf38-b5b5-4e1c-a70c-7956f56d5e3c', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_2c1edf38b5b54e1ca70c7956f56d5e3c();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_2c1edf38b5b54e1ca70c7956f56d5e3c();
            }
</script>
*)
(**
Those are partly going to differ because they have different volatilities.
It we want to have a sense for which is better per unit of volatility,
then it can make sense to normalize volatilities.

First compare the MVE vol

*)
portMve
|> List.map(fun x -> x.Return)
|> Seq.stDev
|> fun vol -> sqrt(12.0) * vol(* output: 
0.1410377523*)
(**
To the 60/40 vol.

*)
port6040
|> List.map(fun x -> x.Return)
|> Seq.stDev
|> fun vol -> sqrt(12.0)*vol(* output: 
0.09997596746*)
(**
Ok, cumulative returns of the normalized vol returns.

*)
let normalize10pctVol xs =
    let vol = xs |> List.map(fun x -> x.Return) |> Seq.stDev
    let annualizedVol = vol * sqrt(12.0)
    xs 
    |> List.map(fun x -> { x with Return = x.Return * (0.1/annualizedVol)})

let portMveCumulativeNormlizedVol = 
    portMve
    |> normalize10pctVol
    |> cumulateReturns

let port6040CumulativeNormlizedVol = 
    port6040
    |> normalize10pctVol 
    |> cumulateReturns


let chartMVENormlizedVol = 
    portMveCumulativeNormlizedVol
    |> List.map(fun x -> x.Date, x.Return)
    |> Chart.Line
    |> Chart.withTraceInfo(Name="MVE")

let chart6040NormlizedVol = 
    port6040CumulativeNormlizedVol
    |> List.map(fun x -> x.Date, x.Return)
    |> Chart.Line
    |> Chart.withTraceInfo(Name="60/40")

let chartCombinedNormlizedVol =
    [ chartMVENormlizedVol; chart6040NormlizedVol ]
    |> Chart.combine
chartCombinedNormlizedVol |> Chart.show(* output: 
<div id="c58e2865-8444-4d1e-9a67-73d24ce77d7d"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_c58e286584444d1e9a6773d24ce77d7d = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-2.6.3.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","mode":"lines","x":["2010-02-01T00:00:00","2010-03-01T00:00:00","2010-04-01T00:00:00","2010-05-01T00:00:00","2010-06-01T00:00:00","2010-07-01T00:00:00","2010-08-01T00:00:00","2010-09-01T00:00:00","2010-10-01T00:00:00","2010-11-01T00:00:00","2010-12-01T00:00:00","2011-01-01T00:00:00","2011-02-01T00:00:00","2011-03-01T00:00:00","2011-04-01T00:00:00","2011-05-01T00:00:00","2011-06-01T00:00:00","2011-07-01T00:00:00","2011-08-01T00:00:00","2011-09-01T00:00:00","2011-10-01T00:00:00","2011-11-01T00:00:00","2011-12-01T00:00:00","2012-01-01T00:00:00","2012-02-01T00:00:00","2012-03-01T00:00:00","2012-04-01T00:00:00","2012-05-01T00:00:00","2012-06-01T00:00:00","2012-07-01T00:00:00","2012-08-01T00:00:00","2012-09-01T00:00:00","2012-10-01T00:00:00","2012-11-01T00:00:00","2012-12-01T00:00:00","2013-01-01T00:00:00","2013-02-01T00:00:00","2013-03-01T00:00:00","2013-04-01T00:00:00","2013-05-01T00:00:00","2013-06-01T00:00:00","2013-07-01T00:00:00","2013-08-01T00:00:00","2013-09-01T00:00:00","2013-10-01T00:00:00","2013-11-01T00:00:00","2013-12-01T00:00:00","2014-01-01T00:00:00","2014-02-01T00:00:00","2014-03-01T00:00:00","2014-04-01T00:00:00","2014-05-01T00:00:00","2014-06-01T00:00:00","2014-07-01T00:00:00","2014-08-01T00:00:00","2014-09-01T00:00:00","2014-10-01T00:00:00","2014-11-01T00:00:00","2014-12-01T00:00:00","2015-01-01T00:00:00","2015-02-01T00:00:00","2015-03-01T00:00:00","2015-04-01T00:00:00","2015-05-01T00:00:00","2015-06-01T00:00:00","2015-07-01T00:00:00","2015-08-01T00:00:00","2015-09-01T00:00:00","2015-10-01T00:00:00","2015-11-01T00:00:00","2015-12-01T00:00:00","2016-01-01T00:00:00","2016-02-01T00:00:00","2016-03-01T00:00:00","2016-04-01T00:00:00","2016-05-01T00:00:00","2016-06-01T00:00:00","2016-07-01T00:00:00","2016-08-01T00:00:00","2016-09-01T00:00:00","2016-10-01T00:00:00","2016-11-01T00:00:00","2016-12-01T00:00:00","2017-01-01T00:00:00","2017-02-01T00:00:00","2017-03-01T00:00:00","2017-04-01T00:00:00","2017-05-01T00:00:00","2017-06-01T00:00:00","2017-07-01T00:00:00","2017-08-01T00:00:00","2017-09-01T00:00:00","2017-10-01T00:00:00","2017-11-01T00:00:00","2017-12-01T00:00:00","2018-01-01T00:00:00","2018-02-01T00:00:00","2018-03-01T00:00:00","2018-04-01T00:00:00","2018-05-01T00:00:00","2018-06-01T00:00:00","2018-07-01T00:00:00","2018-08-01T00:00:00","2018-09-01T00:00:00","2018-10-01T00:00:00","2018-11-01T00:00:00","2018-12-01T00:00:00","2019-01-01T00:00:00","2019-02-01T00:00:00","2019-03-01T00:00:00","2019-04-01T00:00:00","2019-05-01T00:00:00","2019-06-01T00:00:00","2019-07-01T00:00:00","2019-08-01T00:00:00","2019-09-01T00:00:00","2019-10-01T00:00:00","2019-11-01T00:00:00","2019-12-01T00:00:00","2020-01-01T00:00:00","2020-02-01T00:00:00","2020-03-01T00:00:00","2020-04-01T00:00:00","2020-05-01T00:00:00","2020-06-01T00:00:00","2020-07-01T00:00:00","2020-08-01T00:00:00","2020-09-01T00:00:00","2020-10-01T00:00:00","2020-11-01T00:00:00","2020-12-01T00:00:00","2021-01-01T00:00:00","2021-02-01T00:00:00","2021-03-01T00:00:00","2021-04-01T00:00:00","2021-05-01T00:00:00","2021-06-01T00:00:00","2021-07-01T00:00:00","2021-08-01T00:00:00","2021-09-01T00:00:00","2021-10-01T00:00:00","2021-11-01T00:00:00","2021-12-01T00:00:00","2022-01-01T00:00:00","2022-02-01T00:00:00","2022-03-01T00:00:00","2022-04-01T00:00:00","2022-05-01T00:00:00","2022-06-01T00:00:00","2022-07-01T00:00:00","2022-08-01T00:00:00","2022-09-01T00:00:00","2022-10-01T00:00:00","2022-11-01T00:00:00","2022-12-01T00:00:00","2023-01-01T00:00:00","2023-02-01T00:00:00","2023-03-01T00:00:00","2023-04-01T00:00:00","2023-05-01T00:00:00","2023-06-01T00:00:00","2023-07-01T00:00:00","2023-08-01T00:00:00","2023-09-01T00:00:00","2023-10-01T00:00:00","2023-11-01T00:00:00","2023-12-01T00:00:00"],"y":[1.0140540169733507,1.0412689198489065,1.0376918445090282,0.9847748635695044,0.955151964381794,1.0033107552206006,0.9839178939745954,1.0336286092160183,1.0707292912076865,1.0635886021291052,1.1106409324811897,1.1304451761597913,1.1510105647113744,1.1415091578108176,1.1735148436805167,1.1731713406309079,1.1553029316201182,1.1525339634234395,1.1211086280147524,1.0785076918321115,1.1483729210155142,1.1441405824291258,1.1577373821367585,1.1790011465945278,1.2227410967371148,1.2455694031324325,1.249762353103618,1.199887566842159,1.2207448042769116,1.2450094699159426,1.2655432358677121,1.2762896940334862,1.2596331154528382,1.2652481097639205,1.2670373264571737,1.2964077736665935,1.3048505041598037,1.3270308401476558,1.359950410576614,1.3749699853749402,1.3514888724529341,1.4024018834631689,1.3835397215768588,1.4013348367270921,1.4523092044990913,1.4757521901630508,1.5077249609122845,1.4689510942131976,1.5157815551286526,1.5046957460431591,1.5235476985317904,1.5504279967809063,1.5545478868065126,1.5659100279421063,1.6014505756524917,1.601220236749312,1.6211534848942806,1.661108757841883,1.6484718945677048,1.6222557186465936,1.6832085550470408,1.6369934248802374,1.671008255947595,1.684293745116432,1.654799535222202,1.708211861098108,1.627691541127573,1.6036572694101765,1.6986661850263158,1.697878853591213,1.6949469126210006,1.6427607476420834,1.6288693240862993,1.6829077617316976,1.6816814629078531,1.7044072964716772,1.7033882717519324,1.7488160158050985,1.7451658597295798,1.7424087928167256,1.7272070146348024,1.7180591053742889,1.7244390275902544,1.7653429621156478,1.8250968472923281,1.8284449132631597,1.8550648838933645,1.9040113249272488,1.8912239603287238,1.930605032823768,1.9505385157023913,1.9469431031883764,1.993154572967736,2.029451396231842,2.046345339802782,2.1581904968444103,2.1144087654011745,2.0482870408409446,2.05676822945167,2.0733883321438835,2.079035437932506,2.1373935663059727,2.1972063074811605,2.214396410419554,2.1264375661302433,2.1470463660295462,2.038318653513358,2.128534450331101,2.170171433287516,2.2229559822084837,2.286808238068018,2.210332802854418,2.3111829492510063,2.342040989697021,2.356103540637178,2.341403120160715,2.3849007326383624,2.4596397265865524,2.4967226198208032,2.552480620642756,2.4510485425253123,2.371173790158256,2.591662292600114,2.6948751293483646,2.7356668988179167,2.8884319031714547,3.0653463995806276,2.9900246548134644,2.869133080353912,3.001511580806027,3.0529035654585095,3.0081619453560906,2.9697039601649684,3.0033505251038655,3.1292429814858527,3.1052293145373837,3.217714962575333,3.3243081835333093,3.4019128010710653,3.2579245144645315,3.44767306787025,3.4485252167058844,3.50611488317402,3.327473103469359,3.1915778773904298,3.2790451017953073,3.023433069343236,2.980189548543748,2.8437575545434446,3.0240326999685694,2.9231695300079936,2.739447452885448,2.8208158244269192,2.9092785799436376,2.7777644061104363,2.8793677918815965,2.8216984115533865,3.0016203339934213,3.054878124722562,3.126010571818494,3.2021242968210473,3.2413301664580287,3.216611003266742,3.09583000397645,3.0725163242236957,3.2762745944525173,3.2969975978147423],"marker":{},"line":{},"name":"MVE"},{"type":"scatter","mode":"lines","x":["2010-02-01T00:00:00","2010-03-01T00:00:00","2010-04-01T00:00:00","2010-05-01T00:00:00","2010-06-01T00:00:00","2010-07-01T00:00:00","2010-08-01T00:00:00","2010-09-01T00:00:00","2010-10-01T00:00:00","2010-11-01T00:00:00","2010-12-01T00:00:00","2011-01-01T00:00:00","2011-02-01T00:00:00","2011-03-01T00:00:00","2011-04-01T00:00:00","2011-05-01T00:00:00","2011-06-01T00:00:00","2011-07-01T00:00:00","2011-08-01T00:00:00","2011-09-01T00:00:00","2011-10-01T00:00:00","2011-11-01T00:00:00","2011-12-01T00:00:00","2012-01-01T00:00:00","2012-02-01T00:00:00","2012-03-01T00:00:00","2012-04-01T00:00:00","2012-05-01T00:00:00","2012-06-01T00:00:00","2012-07-01T00:00:00","2012-08-01T00:00:00","2012-09-01T00:00:00","2012-10-01T00:00:00","2012-11-01T00:00:00","2012-12-01T00:00:00","2013-01-01T00:00:00","2013-02-01T00:00:00","2013-03-01T00:00:00","2013-04-01T00:00:00","2013-05-01T00:00:00","2013-06-01T00:00:00","2013-07-01T00:00:00","2013-08-01T00:00:00","2013-09-01T00:00:00","2013-10-01T00:00:00","2013-11-01T00:00:00","2013-12-01T00:00:00","2014-01-01T00:00:00","2014-02-01T00:00:00","2014-03-01T00:00:00","2014-04-01T00:00:00","2014-05-01T00:00:00","2014-06-01T00:00:00","2014-07-01T00:00:00","2014-08-01T00:00:00","2014-09-01T00:00:00","2014-10-01T00:00:00","2014-11-01T00:00:00","2014-12-01T00:00:00","2015-01-01T00:00:00","2015-02-01T00:00:00","2015-03-01T00:00:00","2015-04-01T00:00:00","2015-05-01T00:00:00","2015-06-01T00:00:00","2015-07-01T00:00:00","2015-08-01T00:00:00","2015-09-01T00:00:00","2015-10-01T00:00:00","2015-11-01T00:00:00","2015-12-01T00:00:00","2016-01-01T00:00:00","2016-02-01T00:00:00","2016-03-01T00:00:00","2016-04-01T00:00:00","2016-05-01T00:00:00","2016-06-01T00:00:00","2016-07-01T00:00:00","2016-08-01T00:00:00","2016-09-01T00:00:00","2016-10-01T00:00:00","2016-11-01T00:00:00","2016-12-01T00:00:00","2017-01-01T00:00:00","2017-02-01T00:00:00","2017-03-01T00:00:00","2017-04-01T00:00:00","2017-05-01T00:00:00","2017-06-01T00:00:00","2017-07-01T00:00:00","2017-08-01T00:00:00","2017-09-01T00:00:00","2017-10-01T00:00:00","2017-11-01T00:00:00","2017-12-01T00:00:00","2018-01-01T00:00:00","2018-02-01T00:00:00","2018-03-01T00:00:00","2018-04-01T00:00:00","2018-05-01T00:00:00","2018-06-01T00:00:00","2018-07-01T00:00:00","2018-08-01T00:00:00","2018-09-01T00:00:00","2018-10-01T00:00:00","2018-11-01T00:00:00","2018-12-01T00:00:00","2019-01-01T00:00:00","2019-02-01T00:00:00","2019-03-01T00:00:00","2019-04-01T00:00:00","2019-05-01T00:00:00","2019-06-01T00:00:00","2019-07-01T00:00:00","2019-08-01T00:00:00","2019-09-01T00:00:00","2019-10-01T00:00:00","2019-11-01T00:00:00","2019-12-01T00:00:00","2020-01-01T00:00:00","2020-02-01T00:00:00","2020-03-01T00:00:00","2020-04-01T00:00:00","2020-05-01T00:00:00","2020-06-01T00:00:00","2020-07-01T00:00:00","2020-08-01T00:00:00","2020-09-01T00:00:00","2020-10-01T00:00:00","2020-11-01T00:00:00","2020-12-01T00:00:00","2021-01-01T00:00:00","2021-02-01T00:00:00","2021-03-01T00:00:00","2021-04-01T00:00:00","2021-05-01T00:00:00","2021-06-01T00:00:00","2021-07-01T00:00:00","2021-08-01T00:00:00","2021-09-01T00:00:00","2021-10-01T00:00:00","2021-11-01T00:00:00","2021-12-01T00:00:00","2022-01-01T00:00:00","2022-02-01T00:00:00","2022-03-01T00:00:00","2022-04-01T00:00:00","2022-05-01T00:00:00","2022-06-01T00:00:00","2022-07-01T00:00:00","2022-08-01T00:00:00","2022-09-01T00:00:00","2022-10-01T00:00:00","2022-11-01T00:00:00","2022-12-01T00:00:00","2023-01-01T00:00:00","2023-02-01T00:00:00","2023-03-01T00:00:00","2023-04-01T00:00:00","2023-05-01T00:00:00","2023-06-01T00:00:00","2023-07-01T00:00:00","2023-08-01T00:00:00","2023-09-01T00:00:00","2023-10-01T00:00:00","2023-11-01T00:00:00","2023-12-01T00:00:00"],"y":[1.0206952298811665,1.0563630630542868,1.077114647530736,1.0309614831120195,0.9978782536202391,1.0464477691240035,1.0232726352296515,1.0778460971593524,1.1086568478212566,1.1088943151041886,1.143629445519166,1.1663069412493576,1.192129309753754,1.1913852500850886,1.2220480696169809,1.2200860417089197,1.2020334894059468,1.1965459379195726,1.1606421590260245,1.1076500708649646,1.1876269762856344,1.1847016894683398,1.1898078529939233,1.2379081249335517,1.2682470521606672,1.2857204298062466,1.289258145832605,1.2463123495680368,1.2730037549224955,1.289865213959645,1.3114972945712715,1.3280313848934928,1.3168446361773316,1.324222264465677,1.322775442743228,1.369357743135123,1.38172834572701,1.4110624645220713,1.433291326257333,1.4442451659184554,1.4182225357613418,1.4734876742100997,1.4416300068010548,1.4774000468504935,1.5247731723072842,1.547651160261236,1.5622894471127904,1.5503278695976295,1.5972086811909092,1.5968090307127307,1.6065694953112202,1.633709025567584,1.6558249524255482,1.6382376863577355,1.6865222899363987,1.6568987137594677,1.6937933929439553,1.7245447448197322,1.7160001054761826,1.714281516581937,1.7630293286056886,1.7497432990458037,1.758651036562388,1.7691104732637248,1.7389261415748891,1.7673522908380048,1.7009582265430447,1.6716141877592292,1.756727668231077,1.7602518932719458,1.7285566215186758,1.6863230506275664,1.6901756139345843,1.7629529041086338,1.7775076107567709,1.795860593249934,1.8081602008123179,1.860325443722128,1.85992027832747,1.857182974640842,1.8307549712406304,1.861027313859977,1.8761252583528418,1.9082415387322988,1.9529052548929158,1.9474950044016845,1.9703207440533337,1.986764687190099,1.9917256863995594,2.0216531199985632,2.0302664321588257,2.050678840907558,2.080782276591332,2.1160158768152737,2.1250066943965185,2.1891462022897934,2.1263623583273903,2.0998568125505885,2.100198694369657,2.137502715825365,2.1376370775574807,2.1841201119843188,2.231387868631338,2.2194157147951112,2.1147601950270603,2.1417491882023163,2.0266066405744763,2.1470674357659285,2.1861626037222477,2.210547209061259,2.2650712659830705,2.1892376272158174,2.284055846860208,2.305624271915764,2.298670248053912,2.3075597653901254,2.342431684462254,2.392448141983733,2.418605818697483,2.4459139563426593,2.3397493823006608,2.1225103817231736,2.3201921787012694,2.4015312891133718,2.434246113424679,2.5391860395048136,2.6376912201684983,2.5742000890087886,2.5443404488213046,2.7366448139848782,2.8043913115863224,2.8011216289316057,2.8347968685254283,2.8766817239151306,2.978753574220965,2.9896125859343137,3.0392179902562715,3.090835296674753,3.1414804065115205,3.038915948496255,3.1679258273172923,3.142684172013543,3.1990210263437664,3.0687067988711076,3.006934483099557,3.0266486554080263,2.816451764617973,2.821435187601534,2.6553273244459947,2.8343669425831153,2.7338225303874135,2.525027906493812,2.6373218541429697,2.7500298806474466,2.6254720617843503,2.7733571228579605,2.6918064023829467,2.748119879441499,2.769223610770264,2.7535011118478545,2.8443633160828936,2.8995332895627177,2.845059962749536,2.7166163504054253,2.6499793682967603,2.8359712812330815,2.943365347874907],"marker":{},"line":{},"name":"60/40"}];
            var layout = {"width":600,"height":600,"template":{"layout":{"title":{"x":0.05},"font":{"color":"rgba(42, 63, 95, 1.0)"},"paper_bgcolor":"rgba(255, 255, 255, 1.0)","plot_bgcolor":"rgba(229, 236, 246, 1.0)","autotypenumbers":"strict","colorscale":{"diverging":[[0.0,"#8e0152"],[0.1,"#c51b7d"],[0.2,"#de77ae"],[0.3,"#f1b6da"],[0.4,"#fde0ef"],[0.5,"#f7f7f7"],[0.6,"#e6f5d0"],[0.7,"#b8e186"],[0.8,"#7fbc41"],[0.9,"#4d9221"],[1.0,"#276419"]],"sequential":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]],"sequentialminus":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]},"hovermode":"closest","hoverlabel":{"align":"left"},"coloraxis":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"geo":{"showland":true,"landcolor":"rgba(229, 236, 246, 1.0)","showlakes":true,"lakecolor":"rgba(255, 255, 255, 1.0)","subunitcolor":"rgba(255, 255, 255, 1.0)","bgcolor":"rgba(255, 255, 255, 1.0)"},"mapbox":{"style":"light"},"polar":{"bgcolor":"rgba(229, 236, 246, 1.0)","radialaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""},"angularaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""}},"scene":{"xaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"yaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"zaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true}},"ternary":{"aaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"baxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"caxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"bgcolor":"rgba(229, 236, 246, 1.0)"},"xaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"yaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"annotationdefaults":{"arrowcolor":"#2a3f5f","arrowhead":0,"arrowwidth":1},"shapedefaults":{"line":{"color":"rgba(42, 63, 95, 1.0)"}},"colorway":["rgba(99, 110, 250, 1.0)","rgba(239, 85, 59, 1.0)","rgba(0, 204, 150, 1.0)","rgba(171, 99, 250, 1.0)","rgba(255, 161, 90, 1.0)","rgba(25, 211, 243, 1.0)","rgba(255, 102, 146, 1.0)","rgba(182, 232, 128, 1.0)","rgba(255, 151, 255, 1.0)","rgba(254, 203, 82, 1.0)"]},"data":{"bar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}},"error_x":{"color":"rgba(42, 63, 95, 1.0)"},"error_y":{"color":"rgba(42, 63, 95, 1.0)"}}],"barpolar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"carpet":[{"aaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"},"baxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"}}],"choropleth":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contourcarpet":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"heatmap":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"heatmapgl":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram":[{"marker":{"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"histogram2d":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram2dcontour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"mesh3d":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"parcoords":[{"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"pie":[{"automargin":true}],"scatter":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatter3d":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattercarpet":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergeo":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattermapbox":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolar":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolargl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterternary":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"surface":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"table":[{"cells":{"fill":{"color":"rgba(235, 240, 248, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}},"header":{"fill":{"color":"rgba(200, 212, 227, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}}}]}}};
            var config = {"responsive":true};
            Plotly.newPlot('c58e2865-8444-4d1e-9a67-73d24ce77d7d', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_c58e286584444d1e9a6773d24ce77d7d();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_c58e286584444d1e9a6773d24ce77d7d();
            }
</script>
*)
(**
## Key points to keep in mind.

The mean-variance efficient portfolio will always look like the best portfolio in the sample period in which you estimated the weights. This is because we found it by literally looking for the portfolio with the highest sharpe ratio in that sample period.

A more meaningful comparison would be to estimate mean-variance efficient weights based on past data and see how those weights perform in future data. For instance, estimate weights 2000-2010, and use those weights to determine the portfolio that you're going to hold in 2011. Finally, compare it to 60/40 in 2011. That is an "out of sample" test because your test period (2011) is different from the period when the weights were estimated (2000-2010). Then repeat, always using data **before** the holding period as your training period to estimate the weights for the test holding period.

It is also important to remember that 10-20 years is not long enough to get a good estimate of a portfolio's expected return.

One way to see this is to compare equity returns 2000-2010 and 2010-2020.

*)
ff3
|> Seq.filter(fun x -> 
    x.Date >= DateTime(2000,1,1) &&
    x.Date <= DateTime(2009,12,31))
|> Seq.averageBy(fun x ->
    12.0*x.MktRf)(* output: 
-0.01769*)
ff3
|> Seq.filter(fun x -> 
    x.Date >= DateTime(2010,1,1) &&
    x.Date <= DateTime(2019,12,31))
|> Seq.averageBy(fun x ->
    12.0*x.MktRf)(* output: 
0.13093*)
(**
Neither of those 10-year periods is a good estimate of expected market returns.Thus it does not make sense to try forming a mean-variance efficient portfolio using the trailing 10-year period for estimating forward-looking returns.

If we look at US returns 1900-2012, the data indicates that equity excess returns were about 5.5%, and bond excess returns were about 1%. Covariances over shorter periods are more reasonable, so we can use the recent sample to estimate covariances and the long sample for means.

*)
let symStockBond = ["VTI";"BND"]
let covStockBond =
    [ for x in symStockBond do
        [ for y in symStockBond do
            getCov x y ]]
    |> dsharp.tensor

let meansStockBond = dsharp.tensor([ 0.055/12.0; 0.01/12.0])

let wStockBond =
    let w' = dsharp.solve(covStockBond, meansStockBond)
    w' / w'.sum()

wStockBond(* output: 
tensor([0.4083, 0.5917])*)
let stockBondSharpeAndSD (weights:Tensor) =
    let sbVar = weights.matmul(covStockBond).matmul(weights)
    let sbStDev = sqrt(12.0)*sqrt(sbVar)
    let sbMean = 12.0 * (weights.matmul(meansStockBond))
    sbMean/sbStDev, sbStDev

stockBondSharpeAndSD wStockBond(* output: 
(tensor(0.3758), tensor(0.0755))*)
stockBondSharpeAndSD (dsharp.tensor([0.6;0.4]))(* output: 
(tensor(0.3701), tensor(0.1000))*)

