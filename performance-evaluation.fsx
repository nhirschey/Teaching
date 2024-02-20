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
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: FSharp.Data, 5.0.2"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"
#r "nuget: NovaSBE.Finance, 0.5.0"
#r "nuget: Quotes.YahooFinance, 0.0.5"

open System
open FSharp.Data
open Quotes.YahooFinance

open FSharp.Stats
open Plotly.NET
open NovaSBE.Finance
open NovaSBE.Finance.Ols

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))
fsi.AddPrinter<YearMonth>(fun ym -> $"{ym.Year}-{ym.Month}")
(**
## Data

We get the Fama-French 3-Factor asset pricing model data.

*)
let ff3 = French.getFF3 French.Frequency.Monthly
(**
Let's get a portfolio to analyze.

VBR is the Vanguard Small Cap Value ETF. It invests in small-cap value stocks.

*)
type Return = { YearMonth: DateTime; Return: float }

let getReturns (ticker: string) =
    YahooFinance.History(
        ticker,
        startDate = DateTime(2010, 1, 1),
        endDate = DateTime(DateTime.Now.Year - 1, 12, 31),
        interval = Monthly
    )
    |> List.sortBy (fun x -> x.Date)
    |> List.pairwise
    |> List.map (fun (yesterday, today) ->
        { YearMonth = DateTime(today.Date.Year, today.Date.Month, 1)
          Return = today.AdjustedClose / yesterday.AdjustedClose - 1.0 })
    |> List.toArray

let vbr = getReturns "VBR"
(**
We'll also use VTI as a proxy for the market later.

*)
let vti = getReturns "VTI"
(**
A function to accumulate simple returns.

*)
let cumulativeReturn (xs: seq<DateTime * float>) =
    let mutable cr = 1.0

    [ for (dt, r) in xs do
          cr <- cr * (1.0 + r)
          dt, cr - 1.0 ]
(**
Plot of vbr cumulative return.

*)
let vbrChart =
    vbr
    |> Array.map (fun x -> x.YearMonth, x.Return)
    |> cumulativeReturn
    |> Chart.Line
vbrChart |> Chart.show(* output: 
<div id="412a0095-66f4-4f50-9d34-4fdcbbd34b77"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_412a009566f44f509d344fdcbbd34b77 = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-2.6.3.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","mode":"lines","x":["2010-02-01T00:00:00","2010-03-01T00:00:00","2010-04-01T00:00:00","2010-05-01T00:00:00","2010-06-01T00:00:00","2010-07-01T00:00:00","2010-08-01T00:00:00","2010-09-01T00:00:00","2010-10-01T00:00:00","2010-11-01T00:00:00","2010-12-01T00:00:00","2011-01-01T00:00:00","2011-02-01T00:00:00","2011-03-01T00:00:00","2011-04-01T00:00:00","2011-05-01T00:00:00","2011-06-01T00:00:00","2011-07-01T00:00:00","2011-08-01T00:00:00","2011-09-01T00:00:00","2011-10-01T00:00:00","2011-11-01T00:00:00","2011-12-01T00:00:00","2012-01-01T00:00:00","2012-02-01T00:00:00","2012-03-01T00:00:00","2012-04-01T00:00:00","2012-05-01T00:00:00","2012-06-01T00:00:00","2012-07-01T00:00:00","2012-08-01T00:00:00","2012-09-01T00:00:00","2012-10-01T00:00:00","2012-11-01T00:00:00","2012-12-01T00:00:00","2013-01-01T00:00:00","2013-02-01T00:00:00","2013-03-01T00:00:00","2013-04-01T00:00:00","2013-05-01T00:00:00","2013-06-01T00:00:00","2013-07-01T00:00:00","2013-08-01T00:00:00","2013-09-01T00:00:00","2013-10-01T00:00:00","2013-11-01T00:00:00","2013-12-01T00:00:00","2014-01-01T00:00:00","2014-02-01T00:00:00","2014-03-01T00:00:00","2014-04-01T00:00:00","2014-05-01T00:00:00","2014-06-01T00:00:00","2014-07-01T00:00:00","2014-08-01T00:00:00","2014-09-01T00:00:00","2014-10-01T00:00:00","2014-11-01T00:00:00","2014-12-01T00:00:00","2015-01-01T00:00:00","2015-02-01T00:00:00","2015-03-01T00:00:00","2015-04-01T00:00:00","2015-05-01T00:00:00","2015-06-01T00:00:00","2015-07-01T00:00:00","2015-08-01T00:00:00","2015-09-01T00:00:00","2015-10-01T00:00:00","2015-11-01T00:00:00","2015-12-01T00:00:00","2016-01-01T00:00:00","2016-02-01T00:00:00","2016-03-01T00:00:00","2016-04-01T00:00:00","2016-05-01T00:00:00","2016-06-01T00:00:00","2016-07-01T00:00:00","2016-08-01T00:00:00","2016-09-01T00:00:00","2016-10-01T00:00:00","2016-11-01T00:00:00","2016-12-01T00:00:00","2017-01-01T00:00:00","2017-02-01T00:00:00","2017-03-01T00:00:00","2017-04-01T00:00:00","2017-05-01T00:00:00","2017-06-01T00:00:00","2017-07-01T00:00:00","2017-08-01T00:00:00","2017-09-01T00:00:00","2017-10-01T00:00:00","2017-11-01T00:00:00","2017-12-01T00:00:00","2018-01-01T00:00:00","2018-02-01T00:00:00","2018-03-01T00:00:00","2018-04-01T00:00:00","2018-05-01T00:00:00","2018-06-01T00:00:00","2018-07-01T00:00:00","2018-08-01T00:00:00","2018-09-01T00:00:00","2018-10-01T00:00:00","2018-11-01T00:00:00","2018-12-01T00:00:00","2019-01-01T00:00:00","2019-02-01T00:00:00","2019-03-01T00:00:00","2019-04-01T00:00:00","2019-05-01T00:00:00","2019-06-01T00:00:00","2019-07-01T00:00:00","2019-08-01T00:00:00","2019-09-01T00:00:00","2019-10-01T00:00:00","2019-11-01T00:00:00","2019-12-01T00:00:00","2020-01-01T00:00:00","2020-02-01T00:00:00","2020-03-01T00:00:00","2020-04-01T00:00:00","2020-05-01T00:00:00","2020-06-01T00:00:00","2020-07-01T00:00:00","2020-08-01T00:00:00","2020-09-01T00:00:00","2020-10-01T00:00:00","2020-11-01T00:00:00","2020-12-01T00:00:00","2021-01-01T00:00:00","2021-02-01T00:00:00","2021-03-01T00:00:00","2021-04-01T00:00:00","2021-05-01T00:00:00","2021-06-01T00:00:00","2021-07-01T00:00:00","2021-08-01T00:00:00","2021-09-01T00:00:00","2021-10-01T00:00:00","2021-11-01T00:00:00","2021-12-01T00:00:00","2022-01-01T00:00:00","2022-02-01T00:00:00","2022-03-01T00:00:00","2022-04-01T00:00:00","2022-05-01T00:00:00","2022-06-01T00:00:00","2022-07-01T00:00:00","2022-08-01T00:00:00","2022-09-01T00:00:00","2022-10-01T00:00:00","2022-11-01T00:00:00","2022-12-01T00:00:00","2023-01-01T00:00:00","2023-02-01T00:00:00","2023-03-01T00:00:00","2023-04-01T00:00:00","2023-05-01T00:00:00","2023-06-01T00:00:00","2023-07-01T00:00:00","2023-08-01T00:00:00","2023-09-01T00:00:00","2023-10-01T00:00:00","2023-11-01T00:00:00","2023-12-01T00:00:00"],"y":[0.05014154210289212,0.13566697693540952,0.20701322628581442,0.11103528898699677,0.020357760032583272,0.09645890187919126,0.023386674581881817,0.13015552821092125,0.17085567892660447,0.19754801240690734,0.26569830159082386,0.2974126660730254,0.3583675061213236,0.3782356563144491,0.41061263187622266,0.3807100255227256,0.3519657878280511,0.3070158198635391,0.19994639939367898,0.0722350780426746,0.22695514935645633,0.225218797098651,0.20901354712098463,0.3214627202019531,0.3551690484355441,0.3872990577628179,0.37406777691004955,0.2873010749899376,0.3448825349436566,0.3391636960595865,0.3791948958390363,0.41784554380120476,0.4079853376405931,0.4221835426926903,0.4326347963993451,0.5644138082078083,0.5896987895676691,0.6617094751078971,0.6635545655132924,0.7153506180879945,0.6934992454609981,0.8094338988293348,0.7339653978308076,0.8258231967670564,0.897448015576835,0.9530885188077602,0.9700841375825735,0.9509241301668625,1.047790800099516,1.0741715200190574,1.051167830414597,1.084770223422311,1.1748570890644188,1.0707518397591613,1.1767117853132407,1.0561152240130705,1.1558907354521888,1.1795979570292183,1.180422906862701,1.1456126751025169,1.267074338661693,1.2949752793619393,1.2667544640755843,1.2978232198210793,1.2631860853594437,1.2417736997770459,1.1332432305219888,1.033109997473661,1.193070536668864,1.2219516576323683,1.0975075718060787,0.9787420763799013,1.0084831124469615,1.1798682654632442,1.2339166962046324,1.2652715139673734,1.2616211013483687,1.3738751797493416,1.391108639207399,1.3837839914776922,1.3281544389082733,1.565889457875011,1.6222350300134538,1.6564940784778157,1.7099336908628215,1.679832819900998,1.7011708562552723,1.6363517775132923,1.6915354268308458,1.7282731671330267,1.6884999803080172,1.8080377855456984,1.8460690487237148,1.9361603330537984,1.9319654612293697,2.0079342345535576,1.86547208397812,1.8763626129046767,1.9002750152972991,2.031294300949147,2.0308472449960697,2.1178187434980384,2.19309282222561,2.1220752848372655,1.8555861341571247,1.9227231029179608,1.5696899137683387,1.8775685304884613,1.9924081177061579,1.9215976823020928,2.0498126380239565,1.8017419236469876,1.976395176713893,2.0162242695476458,1.8522492562675676,1.9549918206243166,2.0175483390055784,2.092353844690797,2.154960313457877,2.0712296344112,1.7586572183590672,1.059079779411408,1.3378761288066716,1.450068153459108,1.48870650594169,1.5828896874546676,1.7023977145777272,1.5873325821563733,1.6839297121224739,2.1515596527679666,2.345150826150558,2.4344367565677496,2.7364367892276165,2.9196256026465948,3.0930498841054916,3.184535168433654,3.1265546817439906,3.073877389093326,3.1587888184140107,3.037145028063465,3.237339642451289,3.109239956850544,3.2823537774498908,3.120861874695607,3.1878718542063433,3.2409011049601615,2.9861867971524365,3.0604488042165734,2.6240699382248143,2.999245364940393,2.889384102137005,2.4854234207753194,2.923791464823874,3.141956691094318,2.879572309427263,3.2612854731790364,3.1650925571043302,2.9057885773073404,2.891177320987896,2.7580395626267302,3.0931102088022113,3.3394547531154073,3.1916461822055515,2.9686891770001456,2.79696512982777,3.125700145912755,3.502469758403424],"marker":{},"line":{}}];
            var layout = {"width":600,"height":600,"template":{"layout":{"title":{"x":0.05},"font":{"color":"rgba(42, 63, 95, 1.0)"},"paper_bgcolor":"rgba(255, 255, 255, 1.0)","plot_bgcolor":"rgba(229, 236, 246, 1.0)","autotypenumbers":"strict","colorscale":{"diverging":[[0.0,"#8e0152"],[0.1,"#c51b7d"],[0.2,"#de77ae"],[0.3,"#f1b6da"],[0.4,"#fde0ef"],[0.5,"#f7f7f7"],[0.6,"#e6f5d0"],[0.7,"#b8e186"],[0.8,"#7fbc41"],[0.9,"#4d9221"],[1.0,"#276419"]],"sequential":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]],"sequentialminus":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]},"hovermode":"closest","hoverlabel":{"align":"left"},"coloraxis":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"geo":{"showland":true,"landcolor":"rgba(229, 236, 246, 1.0)","showlakes":true,"lakecolor":"rgba(255, 255, 255, 1.0)","subunitcolor":"rgba(255, 255, 255, 1.0)","bgcolor":"rgba(255, 255, 255, 1.0)"},"mapbox":{"style":"light"},"polar":{"bgcolor":"rgba(229, 236, 246, 1.0)","radialaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""},"angularaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""}},"scene":{"xaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"yaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"zaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true}},"ternary":{"aaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"baxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"caxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"bgcolor":"rgba(229, 236, 246, 1.0)"},"xaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"yaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"annotationdefaults":{"arrowcolor":"#2a3f5f","arrowhead":0,"arrowwidth":1},"shapedefaults":{"line":{"color":"rgba(42, 63, 95, 1.0)"}},"colorway":["rgba(99, 110, 250, 1.0)","rgba(239, 85, 59, 1.0)","rgba(0, 204, 150, 1.0)","rgba(171, 99, 250, 1.0)","rgba(255, 161, 90, 1.0)","rgba(25, 211, 243, 1.0)","rgba(255, 102, 146, 1.0)","rgba(182, 232, 128, 1.0)","rgba(255, 151, 255, 1.0)","rgba(254, 203, 82, 1.0)"]},"data":{"bar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}},"error_x":{"color":"rgba(42, 63, 95, 1.0)"},"error_y":{"color":"rgba(42, 63, 95, 1.0)"}}],"barpolar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"carpet":[{"aaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"},"baxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"}}],"choropleth":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contourcarpet":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"heatmap":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"heatmapgl":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram":[{"marker":{"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"histogram2d":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram2dcontour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"mesh3d":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"parcoords":[{"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"pie":[{"automargin":true}],"scatter":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatter3d":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattercarpet":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergeo":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattermapbox":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolar":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolargl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterternary":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"surface":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"table":[{"cells":{"fill":{"color":"rgba(235, 240, 248, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}},"header":{"fill":{"color":"rgba(200, 212, 227, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}}}]}}};
            var config = {"responsive":true};
            Plotly.newPlot('412a0095-66f4-4f50-9d34-4fdcbbd34b77', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_412a009566f44f509d344fdcbbd34b77();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_412a009566f44f509d344fdcbbd34b77();
            }
</script>
*)
(**
You want to work with excess returns. If you have a zero-cost, long-short portfolio then it is already an excess return. But if you have a long portfolio such as we have with VBR, we need to convert it to an excess return.

*)
type ExcessReturn =
    { YearMonth: DateTime
      ExcessReturn: float }

// ff3 indexed by month
let ff3ByMonth =
    ff3 |> Array.map (fun x -> DateTime(x.Date.Year, x.Date.Month, 1), x) |> Map

let vbrExcess =
    vbr
    |> Array.map (fun x ->
        { YearMonth = x.YearMonth
          ExcessReturn = x.Return - ff3ByMonth[x.YearMonth].Rf })

let vtiExcess =
    vti
    |> Array.map (fun x ->
        { YearMonth = x.YearMonth
          ExcessReturn = x.Return - ff3ByMonth[x.YearMonth].Rf })
(**
## Factor Models

For regression, it is helpful to have the portfolio
return data merged into our factor model data.

*)
type RegData =
    {
        Date: DateTime
        /// Make sure Portfolio is an Excess Return
        Portfolio: float
        MktRf: float
        Hml: float
        Smb: float
    }

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
let sharpe (xs: float seq) = (Seq.mean xs) / (Seq.stDev xs)

let annualizeMonthlySharpe monthlySharpe = sqrt (12.0) * monthlySharpe
(**
Our portfolio.

*)
regData |> Array.map (fun x -> x.Portfolio) |> sharpe |> annualizeMonthlySharpe(* output: 
*)
(**
The market.

*)
regData |> Array.map (fun x -> x.MktRf) |> sharpe |> annualizeMonthlySharpe(* output: 
*)
(**
The HML factor.

*)
regData |> Array.map (fun x -> x.Hml) |> sharpe |> annualizeMonthlySharpe(* output: 
*)
(**
Now we can estimate our factor models using OLS.

*)
let capmEstimate = Ols("Portfolio ~ MktRf", regData).fit()
let ff3Estimate = Ols("Portfolio ~ MktRf + Hml + Smb", regData).fit()
(**
CAPM results.

*)
capmEstimate.summary ()
(**
* What's the interpretation of the alpha?

* What's the interpretation of the beta?

* If you want to replicate VBR with the MKT factor and risk-free bonds, what are the weights that you would use?

Fama-French 3-Factor model results

*)
ff3Estimate.summary ()
(**
* What's the interpretation of the alpha?

* What's the interpretation of the factor betas?

* If you want to replicate VBR with risk free bonds and the MKT, HML, and SMB factors, what are the weights that you would use?

> **Practice:** What is the expected annual return of VBR? When answering this,
> 

> * Assume that your alpha and beta estimates for the 3 factor model explaining VBR returns are accurate.
> 
> * Use the average annual premia on the Fama and French factors from 1926 until end of 2022 as your estimate of the factors' expected returns.
> 

*)
// Answer here
(**
You will probably see that the CAPM $R^2$ is lower than the
Fama-French $R^2$. This means that you can explain more of the
portfolio's returns with the Fama-French model. Or in trader terms,
you can hedge the portfolio better with the multi-factor model.

Let's turn things around and see if we can explain the HML factor.

*)
let hmlRegData =
    let vbrByMonth = vbrExcess |> Array.map (fun x -> x.YearMonth, x) |> Map

    [| for vti in vtiExcess do
           {| YearMonth = vti.YearMonth
              Hml = ff3ByMonth[vti.YearMonth].Hml
              Vti = vti.ExcessReturn
              Vbr = vbrByMonth[vti.YearMonth].ExcessReturn |} |]

hmlRegData[..3]
(**
Explain the HML factor with VTI and VBR

*)
let hmlModel = Ols("Hml ~ Vti + Vbr", hmlRegData).fit()
hmlModel.summary()
(**
* What's the interpretation of the alpha?

* What's the interpretation of the betas?

* If you want to replicate HML using VTI, VBR, and risk-free bonds, what are the weights you would use?

Note:

HML average return.

*)
hmlRegData |> Array.averageBy (fun x -> 12.0 * x.Hml)
(**
VTI average return

*)
hmlRegData |> Array.averageBy (fun x -> 12.0 * x.Vti)
(**
VBR average return.

*)
hmlRegData |> Array.averageBy (fun x -> 12.0 * x.Vbr)
(**
## Information Ratios

We want residuals so that we can estimate information ratios.

*)
let capmResiduals = capmEstimate.resid
let ff3Residuals = ff3Estimate.resid
(**
In general I would write a function to do this. Function makes it a bit
simpler to follow. It's hard for me to read the next few lines and understand
what everything is. Too much going on.

*)
let capmAlpha = 12.0 * capmEstimate.coefs["Intercept"]
let capmStDevResiduals = sqrt (12.0) * (Seq.stDev capmResiduals)
let capmInformationRatio = capmAlpha / capmStDevResiduals(* output: 
val capmAlpha: float = -0.03043835147
val capmStDevResiduals: float = 0.07553319814
val capmInformationRatio: float = -0.4029797787*)
let ff3Alpha = 12.0 * ff3Estimate.coefs["Intercept"]
let ff3StDevResiduals = sqrt (12.0) * (Seq.stDev ff3Residuals)
let ff3InformationRatio = ff3Alpha / ff3StDevResiduals(* output: 
val ff3Alpha: float = -0.008830759293
val ff3StDevResiduals: float = 0.03868130811
val ff3InformationRatio: float = -0.22829526*)
(**
Here is the function version.

*)
let informationRatio monthlyAlpha (monthlyResiduals: float array) =
    let annualAlpha = 12.0 * monthlyAlpha
    let annualStDev = sqrt (12.0) * (Seq.stDev monthlyResiduals)
    annualAlpha / annualStDev

informationRatio capmEstimate.coefs["Intercept"] capmResiduals(* output: 
val informationRatio:
  monthlyAlpha: float -> monthlyResiduals: float array -> float
val it: float = -0.4029797787*)
informationRatio ff3Estimate.coefs["Intercept"] ff3Residuals(* output: 
val it: float = -0.22829526*)

