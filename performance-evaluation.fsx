(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=performance-evaluation.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//performance-evaluation.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//performance-evaluation.ipynb)

# Performance evaluation

We're going to evaluate portfolio performance. The common way to do this is to estimate a portfolio's return adjusted for risk using a factor model with tradeable risk factors.

What's a risk factor? These risk factors are portfolios and the idea is that the expected excess return on these risk factors is compensation to investors for bearing the risk inherent in holding those portfolios. For the return variation in these factors to be "risky", it should be something that investors cannot easily diversify. If it was easy to diversify, then investors could put a small bit of the asset in their portfolio and capture the return without affecting portfolio volatility. That would imply being able to increase return without adding risk. Hence the requirement that a factor constitute return variation that is hard to diversify away.

The greater the riskiness of the factor, the greater the factor's expected return (i.e., the risk-return tradeoff). For example, most people feel that stocks are riskier than bonds and indeed stocks have historically had higher returns than bonds.

The risk adjustment involves estimating a portfolio's $\beta$'s on different risk factors. These $\beta$'s constitute the exposure of the portfolio to the risk factor. If the factor return goes up by 1%, then the portfolio's return goes up by $\beta \times 1\%$.

We can estimate these $\beta$'s by OLS regressions of the portfolio's returns on contemporaneous returns of the risk factors. The slope coefficients on the risk factors are the portfolio's betas on the risk factors. The regression intercept is known as $\alpha$. It represents the average return of the portfolio that is not explained by the portfolio's $\beta$'s on the risk factors. This alpha is the risk-adjusted return.

Intuitively, $\alpha$ is the average return on a portfolio long the investment you are evaluating and short a portfolio with the same factor risk as that portfolio. If the factors and factor betas accurately measure the portfolio's risk, then the alpha is the portfolio's return that is unrelated to the portfolio's risk. Investors like positive alphas because that implies that the portfolio's return is higher than what investors require for bearing the portfolio's risk.

One thing to keep in mind is that throughout this discussion, we have discussed things from the perspective of arbitrage. That is, like a trader. We have not made any assumptions about utility functions or return distributions. This is the Arbitrage Pricing Theory (APT) of Stephen Ross (1976). He was motivated by the observation that

> "... on theoretical grounds it is difficult to justify either the assumption [in mean-variance anlysis and CAPM]() of normality in returns...or of quadratic preferences...and on empirical grounds the conclusions as well as the assumptions of the theory have also come under attack."
> 

The APT way of thinking is less restrictive than economically motivated equilibrium asset pricing models. Which is nice. But it has the cost that it does not tell us as much. With the APT we cannot say precisely what a security's return should be. We can only say that if we go long a portfolio and short the portfolio that replicates its factor exposure, then the alpha shouldn't be **too** big. But if we're thinking like a trader, that's perhaps most of what we care about anyway.

*)
#r "nuget: FSharp.Stats"
#r "nuget: FSharp.Data"
#r "nuget: Plotly.NET, 2.0.0-preview.17"
#r "nuget: Plotly.NET, 2.0.0-preview.17"

#load "Common.fsx"
#load "YahooFinance.fsx"

open System
open FSharp.Data
open Common
open YahooFinance

open FSharp.Stats
open Plotly.NET

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))
fsi.AddPrinter<YearMonth>(fun ym -> $"{ym.Year}-{ym.Month}")
(**
We get the Fama-French 3-Factor asset pricing model data.

*)
let ff3 = French.getFF3 Frequency.Monthly
(**
Let's get a portfolio to analyze.

*)
type Return = { YearMonth : DateTime; Return : float }
        

let vbr = 
    YahooFinance.PriceHistory("VBR",
                              startDate=DateTime(2010,1,1),
                              endDate=DateTime(2021,12,31),
                              interval=Interval.Monthly)
    |> List.sortBy (fun x -> x.Date)
    |> List.pairwise
    |> List.map (fun (yesterday, today) -> 
        { YearMonth = DateTime(today.Date.Year,today.Date.Month,1)
          Return = today.AdjustedClose/yesterday.AdjustedClose - 1.0 })
    |> List.toArray
(**
A function to accumulate simple returns.

*)
let cumulativeReturn (xs: seq<DateTime * float>) =
    /// cr0 is a cumulative return through dt0.
    /// r1 is the return only for period dt1.
    let accumulate (dt0, cr0) (dt1, r1) =
        let cr1 = (1.0 + cr0) * (1.0 + r1) - 1.0
        (dt1, cr1)
    let l = xs |> Seq.sortBy fst |> Seq.toList
    match l with
    | [] -> []
    | h::t ->
        (h, t) ||> List.scan accumulate    
(**
Plot of vbr cumulative return.

*)
let vbrChart =
    vbr
    |> Array.map (fun x -> x.YearMonth, x.Return)
    |> cumulativeReturn
    |> Chart.Line
vbrChart |> Chart.show(* output: 
<div id="9991ab2e-a3a6-4ec5-8d87-3e372d39bf34"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_9991ab2ea3a64ec58d873e372d39bf34 = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-2.6.3.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","mode":"lines","x":["2010-02-01T00:00:00","2010-03-01T00:00:00","2010-04-01T00:00:00","2010-05-01T00:00:00","2010-06-01T00:00:00","2010-07-01T00:00:00","2010-08-01T00:00:00","2010-09-01T00:00:00","2010-10-01T00:00:00","2010-11-01T00:00:00","2010-12-01T00:00:00","2011-01-01T00:00:00","2011-02-01T00:00:00","2011-03-01T00:00:00","2011-04-01T00:00:00","2011-05-01T00:00:00","2011-06-01T00:00:00","2011-07-01T00:00:00","2011-08-01T00:00:00","2011-09-01T00:00:00","2011-10-01T00:00:00","2011-11-01T00:00:00","2011-12-01T00:00:00","2012-01-01T00:00:00","2012-02-01T00:00:00","2012-03-01T00:00:00","2012-04-01T00:00:00","2012-05-01T00:00:00","2012-06-01T00:00:00","2012-07-01T00:00:00","2012-08-01T00:00:00","2012-09-01T00:00:00","2012-10-01T00:00:00","2012-11-01T00:00:00","2012-12-01T00:00:00","2013-01-01T00:00:00","2013-02-01T00:00:00","2013-03-01T00:00:00","2013-04-01T00:00:00","2013-05-01T00:00:00","2013-06-01T00:00:00","2013-07-01T00:00:00","2013-08-01T00:00:00","2013-09-01T00:00:00","2013-10-01T00:00:00","2013-11-01T00:00:00","2013-12-01T00:00:00","2014-01-01T00:00:00","2014-02-01T00:00:00","2014-03-01T00:00:00","2014-04-01T00:00:00","2014-05-01T00:00:00","2014-06-01T00:00:00","2014-07-01T00:00:00","2014-08-01T00:00:00","2014-09-01T00:00:00","2014-10-01T00:00:00","2014-11-01T00:00:00","2014-12-01T00:00:00","2015-01-01T00:00:00","2015-02-01T00:00:00","2015-03-01T00:00:00","2015-04-01T00:00:00","2015-05-01T00:00:00","2015-06-01T00:00:00","2015-07-01T00:00:00","2015-08-01T00:00:00","2015-09-01T00:00:00","2015-10-01T00:00:00","2015-11-01T00:00:00","2015-12-01T00:00:00","2016-01-01T00:00:00","2016-02-01T00:00:00","2016-03-01T00:00:00","2016-04-01T00:00:00","2016-05-01T00:00:00","2016-06-01T00:00:00","2016-07-01T00:00:00","2016-08-01T00:00:00","2016-09-01T00:00:00","2016-10-01T00:00:00","2016-11-01T00:00:00","2016-12-01T00:00:00","2017-01-01T00:00:00","2017-02-01T00:00:00","2017-03-01T00:00:00","2017-04-01T00:00:00","2017-05-01T00:00:00","2017-06-01T00:00:00","2017-07-01T00:00:00","2017-08-01T00:00:00","2017-09-01T00:00:00","2017-10-01T00:00:00","2017-11-01T00:00:00","2017-12-01T00:00:00","2018-01-01T00:00:00","2018-02-01T00:00:00","2018-03-01T00:00:00","2018-04-01T00:00:00","2018-05-01T00:00:00","2018-06-01T00:00:00","2018-07-01T00:00:00","2018-08-01T00:00:00","2018-09-01T00:00:00","2018-10-01T00:00:00","2018-11-01T00:00:00","2018-12-01T00:00:00","2019-01-01T00:00:00","2019-02-01T00:00:00","2019-03-01T00:00:00","2019-04-01T00:00:00","2019-05-01T00:00:00","2019-06-01T00:00:00","2019-07-01T00:00:00","2019-08-01T00:00:00","2019-09-01T00:00:00","2019-10-01T00:00:00","2019-11-01T00:00:00","2019-12-01T00:00:00","2020-01-01T00:00:00","2020-02-01T00:00:00","2020-03-01T00:00:00","2020-04-01T00:00:00","2020-05-01T00:00:00","2020-06-01T00:00:00","2020-07-01T00:00:00","2020-08-01T00:00:00","2020-09-01T00:00:00","2020-10-01T00:00:00","2020-11-01T00:00:00","2020-12-01T00:00:00","2021-01-01T00:00:00","2021-02-01T00:00:00","2021-03-01T00:00:00","2021-04-01T00:00:00","2021-05-01T00:00:00","2021-06-01T00:00:00","2021-07-01T00:00:00","2021-08-01T00:00:00","2021-09-01T00:00:00","2021-10-01T00:00:00","2021-11-01T00:00:00","2021-12-01T00:00:00"],"y":[0.05014181936868667,0.13566662602025525,0.2070131000376072,0.1110351426646703,0.020357949827266353,0.09645891706107412,0.023386982322778138,0.1301550984327311,0.17085583127519044,0.19754765469198876,0.2656983580389898,0.29741264587503946,0.3583674104830923,0.3782352569938898,0.41061219179955377,0.3807098179322177,0.35196561279738336,0.3070152940246169,0.19994672022284887,0.0722349057140732,0.2269549663968029,0.2252186204040354,0.20901364367227138,0.32146271057830433,0.3551695111373623,0.38729913256720194,0.3740674557392145,0.28730117174145664,0.3448825180430031,0.33916366231148554,0.3791948299893497,0.41784521303118005,0.4079856482770805,0.4221836469690581,0.43263505604693453,0.56441418865671,0.5896991753535774,0.661709384867726,0.6635545351997401,0.7153508433170832,0.6934996277116092,0.8094341344659566,0.7339650451667425,0.8258228303966044,0.8974473142563226,0.9530880342972183,0.9700839106901673,0.9509245260486132,1.0477902861914554,1.0741709601605591,1.0511679376592267,1.0847702879877668,1.1748565557021258,1.070752138175803,1.176711285073344,1.0561150276184752,1.155890396730181,1.1795978414554358,1.1804222435643439,1.1456123779377032,1.2670747401873097,1.2949749981029406,1.2667543503533119,1.297822633590891,1.263185698649282,1.2417731353087924,1.133243395185949,1.0331098038732862,1.1930704456331127,1.2219519745193708,1.0975074590115717,0.9787413785925863,1.0084827230054172,1.1798682074180347,1.233916605707131,1.2652720424154071,1.2616205175085629,1.3738750767018608,1.39110827621005,1.3837847378958403,1.3281545402843737,1.5658895058311582,1.6222353737807644,1.6564943478951775,1.7099339208364528,1.6798328786635959,1.7011710593132934,1.6363515152286316,1.6915354650743306,1.7282727736879697,1.68849978046826,1.8080380136914016,1.8460686619240945,1.9361606383587873,1.9319652368926667,2.0079344439584275,1.865472166928405,1.876362542734674,1.9002752111180365,2.031293995396211,2.0308475783039928,2.1178187956859427,2.193092433882197,2.122076025496019,1.85558558251069,1.922722721913988,1.5696892155717568,1.8775684131472539,1.992408514672491,1.9215978863027585,2.049813156728107,1.801741851521364,1.9763949977773518,2.0162239171047402,1.8522489080439608,1.9549911184648283,2.0175478806218567,2.092354226193485,2.154960093021183,2.071228840513186,1.758657099012654,1.0590801998526258,1.3378757653283464,1.4500679398193506,1.4887064941991328,1.582889059027035,1.7023979503952398,1.587332499553768,1.6839299256514204,2.151559171261203,2.345150416677325,2.434435915417379,2.736436582418458,2.9196257895117954,3.0930500127684306,3.1845346025012056,3.1265549036299136,3.0738773480952526,3.158788975290271,3.0371450225171595,3.2373398785585934,3.109239943635586,3.2823541301612025],"marker":{},"line":{}}];
            var layout = {"width":600,"height":600,"template":{"layout":{"title":{"x":0.05},"font":{"color":"rgba(42, 63, 95, 1.0)"},"paper_bgcolor":"rgba(255, 255, 255, 1.0)","plot_bgcolor":"rgba(229, 236, 246, 1.0)","autotypenumbers":"strict","colorscale":{"diverging":[[0.0,"#8e0152"],[0.1,"#c51b7d"],[0.2,"#de77ae"],[0.3,"#f1b6da"],[0.4,"#fde0ef"],[0.5,"#f7f7f7"],[0.6,"#e6f5d0"],[0.7,"#b8e186"],[0.8,"#7fbc41"],[0.9,"#4d9221"],[1.0,"#276419"]],"sequential":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]],"sequentialminus":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]},"hovermode":"closest","hoverlabel":{"align":"left"},"coloraxis":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"geo":{"showland":true,"landcolor":"rgba(229, 236, 246, 1.0)","showlakes":true,"lakecolor":"rgba(255, 255, 255, 1.0)","subunitcolor":"rgba(255, 255, 255, 1.0)","bgcolor":"rgba(255, 255, 255, 1.0)"},"mapbox":{"style":"light"},"polar":{"bgcolor":"rgba(229, 236, 246, 1.0)","radialaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""},"angularaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""}},"scene":{"xaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"yaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"zaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true}},"ternary":{"aaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"baxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"caxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"bgcolor":"rgba(229, 236, 246, 1.0)"},"xaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"yaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"annotationdefaults":{"arrowcolor":"#2a3f5f","arrowhead":0,"arrowwidth":1},"shapedefaults":{"line":{"color":"rgba(42, 63, 95, 1.0)"}},"colorway":["rgba(99, 110, 250, 1.0)","rgba(239, 85, 59, 1.0)","rgba(0, 204, 150, 1.0)","rgba(171, 99, 250, 1.0)","rgba(255, 161, 90, 1.0)","rgba(25, 211, 243, 1.0)","rgba(255, 102, 146, 1.0)","rgba(182, 232, 128, 1.0)","rgba(255, 151, 255, 1.0)","rgba(254, 203, 82, 1.0)"]},"data":{"bar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}},"error_x":{"color":"rgba(42, 63, 95, 1.0)"},"error_y":{"color":"rgba(42, 63, 95, 1.0)"}}],"barpolar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"carpet":[{"aaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"},"baxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"}}],"choropleth":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contourcarpet":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"heatmap":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"heatmapgl":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram":[{"marker":{"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"histogram2d":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram2dcontour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"mesh3d":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"parcoords":[{"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"pie":[{"automargin":true}],"scatter":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatter3d":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattercarpet":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergeo":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattermapbox":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolar":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolargl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterternary":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"surface":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"table":[{"cells":{"fill":{"color":"rgba(235, 240, 248, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}},"header":{"fill":{"color":"rgba(200, 212, 227, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}}}]}}};
            var config = {"responsive":true};
            Plotly.newPlot('9991ab2e-a3a6-4ec5-8d87-3e372d39bf34', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_9991ab2ea3a64ec58d873e372d39bf34();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_9991ab2ea3a64ec58d873e372d39bf34();
            }
</script>
*)
(**
You want to work with excess returns. If you have a zero-cost, long-short portfolio then it is already an excess return. But if you have a long portfolio such as we have with VBR, we need to convert it to an excess return.

*)
type ExcessReturn = { YearMonth: DateTime; ExcessReturn: float }

// ff3 indexed by month
let ff3ByMonth = 
    ff3
    |> Array.map(fun x -> DateTime(x.Date.Year, x.Date.Month,1), x)
    |> Map

let vbrExcess =
    vbr
    |> Array.map (fun x ->
        { YearMonth = x.YearMonth
          ExcessReturn = x.Return - ff3ByMonth[x.YearMonth].Rf } )
(**
For regression, it is helpful to have the portfolio
return data merged into our factor model data.

*)
type RegData =
    { Date : DateTime
      /// Make sure Portfolio is an Excess Return
      Portfolio : float
      MktRf : float 
      Hml : float 
      Smb : float }

let regData =
    vbrExcess
    |> Array.map (fun x ->
        let xff3 = ff3ByMonth[x.YearMonth]
        { Date = x.YearMonth
          Portfolio = x.ExcessReturn
          MktRf = xff3.MktRf 
          Hml = xff3.Hml 
          Smb = xff3.Smb })
(**
One way to evaluate performance is Sharpe ratios.

*)
/// Calculates sharpe ratio of a sequence of excess returns
let sharpe (xs: float seq) =
    (Seq.mean xs) / (Seq.stDev xs)

let annualizeMonthlySharpe monthlySharpe = sqrt(12.0) * monthlySharpe
    
(**
Our portfolio.

*)
regData
|> Array.map (fun x -> x.Portfolio)
|> sharpe
|> annualizeMonthlySharpe(* output: 
*)
(**
The market.

*)
regData
|> Array.map (fun x -> x.MktRf)
|> sharpe
|> annualizeMonthlySharpe(* output: 
*)
(**
The HML factor.

*)
regData
|> Array.map (fun x -> x.Hml)
|> sharpe
|> annualizeMonthlySharpe(* output: 
*)
(**
[Accord.NET](http://accord-framework.net/) is a .NET (C#/F#/VB.NET) machine learning library.

*)
#r "nuget: Accord"
#r "nuget: Accord.Statistics"

open Accord
open Accord.Statistics.Models.Regression.Linear
(**
The OLS trainer is documented [here](https://github.com/accord-net/framework/wiki/Regression) with an example in C#.

We'll use it in a a more F# way

*)
type RegressionOutput =
    { Model : MultipleLinearRegression 
      TValuesWeights : float array
      TValuesIntercept : float 
      R2: float }

/// Type alias for x, y regression data 
type XY = (float array) array * float array

let fitModel (x: (float array) array, y: float array) =
    let ols = new OrdinaryLeastSquares(UseIntercept=true)
    let estimate = ols.Learn(x,y)
    let mse = estimate.GetStandardError(x,y)
    let se = estimate.GetStandardErrors(mse, ols.GetInformationMatrix())
    let tvaluesWeights = 
        estimate.Weights
        |> Array.mapi(fun i w -> w / se.[i])
    let tvalueIntercept = estimate.Intercept / (se |> Array.last)
    let r2 = estimate.CoefficientOfDetermination(x,y)
    { Model = estimate
      TValuesWeights = tvaluesWeights
      TValuesIntercept = tvalueIntercept  
      R2 = r2 }

let capmModelData = 
    regData
    |> Array.map(fun obs -> [|obs.MktRf|], obs.Portfolio)
    |> Array.unzip 

let ff3ModelData = 
    regData
    |> Array.map(fun obs -> [|obs.MktRf; obs.Hml; obs.Smb |], obs.Portfolio)
    |> Array.unzip
(**
Now we can estimate our models.

*)
let capmEstimate = capmModelData |> fitModel
let ff3Estimate = ff3ModelData |> fitModel
(**
CAPM results.

*)
capmEstimate.Model(* output: 
input.fsx (1,1)-(1,13) typecheck error The value, namespace, type or module 'capmEstimate' is not defined.*)
capmEstimate.TValuesIntercept(* output: 
input.fsx (1,1)-(1,13) typecheck error The value, namespace, type or module 'capmEstimate' is not defined.*)
capmEstimate.R2 (* output: 
input.fsx (1,1)-(1,13) typecheck error The value, namespace, type or module 'capmEstimate' is not defined.*)
(**
Fama-French 3-Factor model results

*)
ff3Estimate.Model(* output: 
input.fsx (1,1)-(1,12) typecheck error The value, namespace, type or module 'ff3Estimate' is not defined.*)
ff3Estimate.TValuesIntercept(* output: 
input.fsx (1,1)-(1,12) typecheck error The value, namespace, type or module 'ff3Estimate' is not defined.*)
ff3Estimate.R2(* output: 
input.fsx (1,1)-(1,12) typecheck error The value, namespace, type or module 'ff3Estimate' is not defined.*)
(**
You will probably see that the CAPM $R^2$ is lower than the
Fama-French $R^2$. This means that you can explain more of the
portfolio's returns with the Fama-French model. Or in trader terms,
you can hedge the portfolio better with the multi-factor model.

We also want predicted values so that we can get regression residuals for calculating
the information ratio.

*)
type Prediction = { Label : float; Score : float}

let makePredictions 
    (estimate:MultipleLinearRegression) 
    (x: (float array) array, y: float array) =
    (estimate.Transform(x), y)
    ||> Array.zip
    |> Array.map(fun (score, label) -> { Score = score; Label = label })

let residuals (xs: Prediction array) = xs |> Array.map(fun x -> x.Label - x.Score)

let capmPredictions = makePredictions capmEstimate.Model capmModelData
let ff3Predictions = makePredictions ff3Estimate.Model ff3ModelData

capmPredictions |> Array.take 3(* output: 
*)
capmPredictions |> residuals |> Array.take 3(* output: 
input.fsx (1,1)-(1,16) typecheck error The value or constructor 'capmPredictions' is not defined.
input.fsx (1,20)-(1,29) typecheck error The value or constructor 'residuals' is not defined. Maybe you want one of the following:
   Result*)
let capmResiduals = residuals capmPredictions
let ff3Residuals = residuals ff3Predictions
(**
In general I would write a function to do this. Function makes it a bit
simpler to follow. It's hard for me to read the next few lines and understand
what everything is. Too much going on.

*)
let capmAlpha = 12.0 * capmEstimate.Model.Intercept 
let capmStDevResiduals = sqrt(12.0) * (Seq.stDev capmResiduals)
let capmInformationRatio = capmAlpha / capmStDevResiduals(* output: 
input.fsx (1,24)-(1,36) typecheck error The value, namespace, type or module 'capmEstimate' is not defined.
input.fsx (2,50)-(2,63) typecheck error The value or constructor 'capmResiduals' is not defined.*)
let ff3Alpha = 12.0 * ff3Estimate.Model.Intercept 
let ff3StDevResiduals = sqrt(12.0) * (Seq.stDev ff3Residuals)
let ff3InformationRatio = ff3Alpha / ff3StDevResiduals(* output: 
input.fsx (1,23)-(1,34) typecheck error The value, namespace, type or module 'ff3Estimate' is not defined.
input.fsx (2,49)-(2,61) typecheck error The value or constructor 'ff3Residuals' is not defined.*)
// Function version

let informationRatio monthlyAlpha (monthlyResiduals: float array) =
    let annualAlpha = 12.0 * monthlyAlpha
    let annualStDev = sqrt(12.0) * (Seq.stDev monthlyResiduals)
    annualAlpha / annualStDev 

informationRatio capmEstimate.Model.Intercept capmResiduals(* output: 
input.fsx (8,18)-(8,30) typecheck error The value, namespace, type or module 'capmEstimate' is not defined.
input.fsx (8,47)-(8,60) typecheck error The value or constructor 'capmResiduals' is not defined.*)
informationRatio ff3Estimate.Model.Intercept ff3Residuals(* output: 
input.fsx (1,1)-(1,17) typecheck error The value or constructor 'informationRatio' is not defined. Maybe you want one of the following:
   Integration*)

