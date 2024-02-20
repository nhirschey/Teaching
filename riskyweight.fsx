(**
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath=riskyweight.ipynb)&emsp;
[![Script](img/badge-script.svg)](/Teaching//riskyweight.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)](/Teaching//riskyweight.ipynb)

# Choosing the risky asset weight in your portfolio

*)
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"


open System

open FSharp.Stats

open Plotly.NET
(**
# Portfolios of a risky asset and the risk-free asset

## Portfolio return

Suppose an investor has a portfolio comprised of a risky asset and the risk-free asset. The risky asset has return $r_i$ and the risk-free asset has return $r_f$. If $w$ is the weight that the investor puts on the risky asset, then the portfolio return is

$$r_p = w r_i + (1-w) r_f = w (r_i - r_f) + r_f$$

If we put this in terms of excess returns, meaning returns in excess of the risk-free rate, then the portfolio excess return is

$$r_p - r_f = w (r_i - r_f)$$.

So if we work in excess returns the excess return of the portfolio is the risky weight times the risky asset's excess return.

When $w < 0$ we are short the risky asset and long the risk-free asset, when $0 < w < 1$ we are long both the risky and risk-free asset, and when $w > 1$ we are short the risk-free asset and long the risky asset.

Does this check out? Imagine that you have $1 and you borrow $1 at the risk-free rate for a net stake of $2. Assuming that you invest in an asset with a return of 15% and the risk-free rate is 4%, what are you left with?

*)
let invest = 1.0m
let borrow = 1.0m 
let ret = 0.15m
let rf = 0.04m
let result = (invest + borrow)*(1.0m+ret)-borrow*(1.0m+rf)
printfn $"You are left with {result}"(* output: 
You are left with 1.260*)
(**
If we calculate this using weights ...

*)
let w = (invest + borrow)/invest
let result2 = 1m + rf + w*(ret - rf)
printfn $"You are left with {result2}"(* output: 
You are left with 1.26*)
(**
... we get the same result.

*)
result = result2(* output: 
val it: bool = true*)
(**
## Portfolio variance

Recall the formula for variance of a portfolio consisting of stocks $x$ and $y$:

$$\sigma^2 = w_x^2 \sigma^2_x + w_y^2 \sigma^2_y + 2 w_x w_y cov(r_x,r_y),$$

where $w_x$ and $w_y$ are the weights in stocks $x$ and $y$, $r_x$ and $r_y$ are the stock returns, $\sigma^2$ is variance, and $cov(.,.)$ is covariance.

If one asset is the risk free asset (borrowing a risk-free bond), then this asset does not vary, so the risk free variance and covariance terms are zero. Thus we are left with the result that if we leverage risky asset $x$ by borrowing or lending the risk-free asset, then our leveraged portfolio's standard deviation ($\sigma$) is

$$\sigma^2 = w_x^2 \sigma^2_x \rightarrow \sigma = w_x \sigma_x.$$

# Mean-Variance optimal weight

An investor with mean-variance preferences will try to maximize utility of the form

$$u = \mu - \gamma \frac{\sigma^2}{2}$$

where $\mu$ is the expected portfolio return, $\sigma$ is the standard deviation of the portfolio, and $\gamma$ is the investor's coefficient of relative risk aversion.

If our portfolio is comprised of a risky asset **x** and the risk-free asset, then this objective function can be written

$$u = w_x (\mu_x - r_f) + r_f - \gamma \frac{w_x^2 \sigma_x^2}{2}.$$

To maximize this with respect to $w_x$ we take the derivative and set it equal to zero:

$$\frac{\partial u}{\partial w_x} = (\mu_x - r_f) - \gamma w_x \sigma_x^2 = 0$$

This gives us the optimal weight

$$w_x = \frac{\mu_x - r_f}{\gamma \sigma_x^2}.$$

It is common to define $\mu$ as the expected excess return, so that $\mu_x = r_x - r_f$. Then the optimal weight is

$$w_x = \frac{\mu_x}{\gamma \sigma_x^2}.$$

Typical values for $\gamma$ range from two to five. Higher values for $\gamma$ indicate higher risk aversion.

Examples

*)
let meanVarianceWeight mu sigma gamma =
    mu/(gamma*sigma**2.0)

[ for gamma in [1.0 .. 5.0 ] do 
    {| Gamma = gamma
       RiskyWeight = meanVarianceWeight 0.075 0.14 gamma |} ](* output: 
val meanVarianceWeight: mu: float -> sigma: float -> gamma: float -> float
val it: {| Gamma: float; RiskyWeight: float |} list =
  [{ Gamma = 1.0
     RiskyWeight = 3.826530612 }; { Gamma = 2.0
                                    RiskyWeight = 1.913265306 };
   { Gamma = 3.0
     RiskyWeight = 1.275510204 }; { Gamma = 4.0
                                    RiskyWeight = 0.9566326531 };
   { Gamma = 5.0
     RiskyWeight = 0.7653061224 }]*)
(**
# Kelly Criterion

The mean-variance optimal weight has a similar form to the optimal weight from the Kelly Criterion (Kelly, 1956). The Kelly Criterion is the weight that maximizes the expected geometric growth rate of an investor's wealth or, equivalently, the expected value of log wealth. It is often seen in industry as a formula for determining the optimal fraction of your wealth to bet on a risky asset. Originally developed for gambling, it can also be used for asset management.

The objective is to maximize wealth, which grows as

$$(1+w r_1)(1+w r_2)...(1+w r_T)=\prod_{i=1}^T (1 + w_i r_i)$$

It can be shown that the expected long-term growth rate is

$$\approx w \mu - \frac{1}{2} w^2 \sigma^2$$

This is maximized when

$$w = \frac{\mu}{\sigma^2}$$

where $\mu$ is the expected excess return of the risky asset and $\sigma^2$ is the variance of the risky asset.

*)
let kellyWeight mu sigma =
    mu/(sigma**2.0)

kellyWeight 0.075 0.14(* output: 
val kellyWeight: mu: float -> sigma: float -> float
val it: float = 3.826530612*)
(**
> Practice: Use a loop to calculate the Kelly Criterion weight for a range of $\mu$ and $\sigma$ values.
> 

> First, hold $\sigma$ fixed and vary $\mu$ from 0.05 to 0.10 by 0.01. What happens to the optimal weight as $\mu$ increases?
> 

    // Answer here

> Second, hold $\mu$ fixed and vary $\sigma$ from 0.1 to 0.2 by 0.01. What happens to the optimal weight as $\sigma$ increases?
> 

    // Answer here

# Simulating results from different portfolio rules

We can simulate the results of different portfolio rules, using the mean-variance weight and varying $\gamma$. The Kelly Criterion corresponds to $\gamma = 1$.

*)
let seed = 99
Random.SetSampleGenerator(Random.RandThreadSafe(seed))

// Let's start with this sample of returns
let rnorm1 = Distributions.Continuous.Normal.Init 0.01 1.0

let careerLength = 30
let draws =
    [ for draw in [1 .. 100] do 
        [ for year in [1 .. 10_000] do
            rnorm1.Sample() ]]
(**
Those are our returns. Let's calculate it for a particular gamma.

*)
let gamma = 2.0
let ww = meanVarianceWeight 0.01 1.0 gamma
let investmentResult (riskyWeight: float) (returns: float list)  =
    let mutable wealth = 1.0
    for yearReturn in returns do 
        let newWealth = wealth*(1.0 + riskyWeight*yearReturn)
        // If we go bankrupt, we are done
        wealth <- max 0.0 newWealth
    // This is the last wealth value
    wealth

investmentResult  1.0 [0.1; 0.1; 0.1](* output: 
1.331*)
investmentResult  ww [0.1; 0.1; 0.1](* output: 
1.00150075*)
(**
Now do it for all our draws.

*)
let myResult =
    [ for draw in draws do investmentResult  ww draw]

myResult(* output: 
[2.837887967; 1.023004546; 1.643739485; 2.12179225; 1.436309156; 2.346815953;
 0.8786401356; 3.792752596; 1.815664246; 2.972106488; 1.517589968; 2.72606676;
 1.155107973; 1.410694065; 1.632517331; 1.947730141; 2.279910946; 1.481477892;
 2.086684258; 0.538281238; 2.424849205; 1.468674191; 1.143783327; 1.833370522;
 1.6039682; 1.368936275; 0.616193267; 1.263363465; 1.947484509; 1.297083759;
 0.8644086517; 1.754023531; 1.036148661; 3.989536962; 0.9810447439; 3.340688473;
 1.148285313; 1.288333618; 1.734002934; 1.23840977; 2.793957289; 1.001289703;
 1.461894733; 3.625290137; 2.028797723; 2.550341752; 1.306375484; 1.828023863;
 1.044705207; 1.150132042; 1.927413813; 1.461602833; 1.065465273; 1.045489968;
 1.014470884; 1.671204029; 1.284125979; 2.096228342; 0.9096693827; 3.574390319;
 0.9957710939; 1.800322085; 2.254395349; 0.4030964431; 2.036395802; 1.689207898;
 1.345681325; 0.665578875; 0.440331489; 0.918066828; 1.896234599; 1.664944745;
 5.056735953; 0.8511369538; 3.299352623; 0.9708549067; 4.549881402; 2.913110159;
 0.5309154183; 1.944486473; 4.581272808; 3.034970632; 0.9535913349; 1.787086779;
 0.734172244; 1.464532329; 1.17019237; 0.8295362389; 1.195536513; 0.6561338853;
 1.353833407; 1.084669355; 0.666350047; 1.181805047; 1.374376343; 0.7532359753;
 1.322688859; 2.582040177; 2.217090877; 0.9919548743]*)
(**
Now let's calculate some statistics.

*)
type SimulationSummary =
    { Gamma: float 
      AvgLogWealth: float 
      AvgWealth: float
      AvgGeometricGrowth: float 
      MinWealth: float
      FractionBankrupt: float
      FractionLoseMoney: float }

let calcSummary (gamma: float) (nPeriods: int) (wealths: list<float>) =
    { Gamma = gamma
      AvgLogWealth = wealths |> List.map log |> List.average
      AvgWealth = wealths |> List.average
      AvgGeometricGrowth = 
        wealths 
        |> List.map (fun w -> w**(1.0/float nPeriods) - 1.0) 
        |> List.average
      MinWealth = wealths |> List.min
      FractionBankrupt = 
        let bankrupts = wealths |> List.filter (fun w -> w = 0.0)
        float bankrupts.Length / float wealths.Length
      FractionLoseMoney = 
        let lostMoney = wealths |> List.filter (fun w -> w < 1.0)
        float lostMoney.Length / float wealths.Length  
      }
calcSummary gamma careerLength myResult(* output: 
val it: SimulationSummary = { Gamma = 2.0
                              AvgLogWealth = 0.3996341337
                              AvgWealth = 1.70989802
                              AvgGeometricGrowth = 0.01356404241
                              MinWealth = 0.4030964431
                              FractionBankrupt = 0.0
                              FractionLoseMoney = 0.21 }*)
(**
Now a function to do it for a particular gamma

*)
let gammaRecord (mu: float) (sigma: float) (draws: list<list<float>>) (gamma: float) =
    let w = meanVarianceWeight mu sigma gamma
    let myResult = draws |> List.map (investmentResult w)
    let careerLength = draws[0] |> List.length
    calcSummary gamma careerLength myResult

gammaRecord 0.01 1.0 draws 3.0(* output: 
{ Gamma = 3.0
  AvgLogWealth = 0.2942149837
  AvgWealth = 1.426273684
  AvgGeometricGrowth = 2.942253804e-05
  MinWealth = 0.5613435247
  FractionBankrupt = 0.0
  FractionLoseMoney = 0.17 }*)
(**
Now do it for many gammas.

*)
let ruleResults =
    let gammas = [0.5 .. 0.1 .. 1.0] @ [1.25 .. 0.25 .. 2.0]
    [ for gamma in gammas do
        gammaRecord 0.01 1.0 draws gamma ]

let veryLongRunChart =
    ruleResults
    |> List.map (fun x -> 
        let kellyFraction = 1.0 / x.Gamma
        kellyFraction, x.AvgGeometricGrowth)
    |> Chart.Line
    |> Chart.withXAxisStyle("Kelly fraction")
    |> Chart.withYAxisStyle("Average geometric growth")
veryLongRunChart(* output: 
<div id="1771c712-1dd7-41f1-a243-9a6f20bfb7a2"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_1771c7121dd741f1a2439a6f20bfb7a2 = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-2.6.3.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"scatter","mode":"lines","x":[2.0,1.6666666666666667,1.4285714285714286,1.25,1.1111111111111112,1.0,0.8,0.6666666666666666,0.5714285714285714,0.5],"y":[9.740686217507566E-06,3.5931432269324934E-05,4.782216803905226E-05,5.301416684176496E-05,5.4845080090880624E-05,5.491950822580027E-05,5.1939800136249214E-05,4.7729621971690504E-05,4.363332349391569E-05,3.996557729414984E-05],"marker":{},"line":{}}];
            var layout = {"width":600,"height":600,"template":{"layout":{"title":{"x":0.05},"font":{"color":"rgba(42, 63, 95, 1.0)"},"paper_bgcolor":"rgba(255, 255, 255, 1.0)","plot_bgcolor":"rgba(229, 236, 246, 1.0)","autotypenumbers":"strict","colorscale":{"diverging":[[0.0,"#8e0152"],[0.1,"#c51b7d"],[0.2,"#de77ae"],[0.3,"#f1b6da"],[0.4,"#fde0ef"],[0.5,"#f7f7f7"],[0.6,"#e6f5d0"],[0.7,"#b8e186"],[0.8,"#7fbc41"],[0.9,"#4d9221"],[1.0,"#276419"]],"sequential":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]],"sequentialminus":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]},"hovermode":"closest","hoverlabel":{"align":"left"},"coloraxis":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"geo":{"showland":true,"landcolor":"rgba(229, 236, 246, 1.0)","showlakes":true,"lakecolor":"rgba(255, 255, 255, 1.0)","subunitcolor":"rgba(255, 255, 255, 1.0)","bgcolor":"rgba(255, 255, 255, 1.0)"},"mapbox":{"style":"light"},"polar":{"bgcolor":"rgba(229, 236, 246, 1.0)","radialaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""},"angularaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""}},"scene":{"xaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"yaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"zaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true}},"ternary":{"aaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"baxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"caxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"bgcolor":"rgba(229, 236, 246, 1.0)"},"xaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"yaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"annotationdefaults":{"arrowcolor":"#2a3f5f","arrowhead":0,"arrowwidth":1},"shapedefaults":{"line":{"color":"rgba(42, 63, 95, 1.0)"}},"colorway":["rgba(99, 110, 250, 1.0)","rgba(239, 85, 59, 1.0)","rgba(0, 204, 150, 1.0)","rgba(171, 99, 250, 1.0)","rgba(255, 161, 90, 1.0)","rgba(25, 211, 243, 1.0)","rgba(255, 102, 146, 1.0)","rgba(182, 232, 128, 1.0)","rgba(255, 151, 255, 1.0)","rgba(254, 203, 82, 1.0)"]},"data":{"bar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}},"error_x":{"color":"rgba(42, 63, 95, 1.0)"},"error_y":{"color":"rgba(42, 63, 95, 1.0)"}}],"barpolar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"carpet":[{"aaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"},"baxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"}}],"choropleth":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contourcarpet":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"heatmap":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"heatmapgl":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram":[{"marker":{"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"histogram2d":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram2dcontour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"mesh3d":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"parcoords":[{"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"pie":[{"automargin":true}],"scatter":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatter3d":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattercarpet":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergeo":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattermapbox":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolar":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolargl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterternary":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"surface":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"table":[{"cells":{"fill":{"color":"rgba(235, 240, 248, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}},"header":{"fill":{"color":"rgba(200, 212, 227, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}}}]}},"xaxis":{"title":{"text":"Kelly fraction"}},"yaxis":{"title":{"text":"Average geometric growth"}}};
            var config = {"responsive":true};
            Plotly.newPlot('1771c712-1dd7-41f1-a243-9a6f20bfb7a2', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_1771c7121dd741f1a2439a6f20bfb7a2();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_1771c7121dd741f1a2439a6f20bfb7a2();
            }
</script>
*)
(**
A couple takeaways:

0 Even an investor who is not very risk averse should not bet more than 1x the Kelly bet. This is particularly true if we take into account uncertainty about expected returns and variances.
  

1 It takes a very long time to converge to the kelly result.
  

Now let's try with representative returns, monthly rebalancing.

*)
let monthlyMu = 0.075/12.0
let monthlySigma = 0.14/sqrt 12.0
let rnorm2 = Distributions.Continuous.Normal.Init monthlyMu monthlySigma
let investmentCareer = 30
let drawsRealistic =
    [ for draw in [1 .. 1000] do 
        [ for year in [1 .. investmentCareer*12] do
            rnorm2.Sample() ]]

let realisticResults =
    let gammas = [0.75 .. 0.25 .. 3.0]
    [ for gamma in gammas do
        gammaRecord monthlyMu monthlySigma drawsRealistic gamma ]
(**
The results:

*)
realisticResults(* output: 
val it: SimulationSummary list =
  [{ Gamma = 0.75
     AvgLogWealth = 3.450913017
     AvgWealth = 404528.9387
     AvgGeometricGrowth = 0.00969663175
     MinWealth = 0.0003612225864
     FractionBankrupt = 0.0
     FractionLoseMoney = 0.19 }; { Gamma = 1.0
                                   AvgLogWealth = 4.129875279
                                   AvgWealth = 17346.91124
                                   AvgGeometricGrowth = 0.01157315897
                                   MinWealth = 0.01479285394
                                   FractionBankrupt = 0.0
                                   FractionLoseMoney = 0.087 };
   { Gamma = 1.25
     AvgLogWealth = 4.011820663
     AvgWealth = 1924.313345
     AvgGeometricGrowth = 0.011228559
     MinWealth = 0.07365774793
     FractionBankrupt = 0.0
     FractionLoseMoney = 0.05 }; { Gamma = 1.5
                                   AvgLogWealth = 3.730401519
                                   AvgWealth = 446.1184822
                                   AvgGeometricGrowth = 0.01043149726
                                   MinWealth = 0.1711251698
                                   FractionBankrupt = 0.0
                                   FractionLoseMoney = 0.036 };
   { Gamma = 1.75
     AvgLogWealth = 3.433054336
     AvgWealth = 165.4387707
     AvgGeometricGrowth = 0.009593159598
     MinWealth = 0.2813529385
     FractionBankrupt = 0.0
     FractionLoseMoney = 0.024 }; { Gamma = 2.0
                                    AvgLogWealth = 3.158107278
                                    AvgWealth = 81.67908898
                                    AvgGeometricGrowth = 0.008819737755
                                    MinWealth = 0.3864483271
                                    FractionBankrupt = 0.0
                                    FractionLoseMoney = 0.016 };
   { Gamma = 2.25
     AvgLogWealth = 2.913705061
     AvgWealth = 48.21241209
     AvgGeometricGrowth = 0.008133277799
     MinWealth = 0.4789578792
     FractionBankrupt = 0.0
     FractionLoseMoney = 0.014 }; { Gamma = 2.5
                                    AvgLogWealth = 2.699006896
                                    AvgWealth = 31.99927166
                                    AvgGeometricGrowth = 0.00753093058
                                    MinWealth = 0.5573887274
                                    FractionBankrupt = 0.0
                                    FractionLoseMoney = 0.011 };
   { Gamma = 2.75
     AvgLogWealth = 2.510690679
     AvgWealth = 23.03190791
     AvgGeometricGrowth = 0.007003073346
     MinWealth = 0.6227931384
     FractionBankrupt = 0.0
     FractionLoseMoney = 0.008 }; { Gamma = 3.0
                                    AvgLogWealth = 2.345068506
                                    AvgWealth = 17.57720025
                                    AvgGeometricGrowth = 0.006539170532
                                    MinWealth = 0.6770275267
                                    FractionBankrupt = 0.0
                                    FractionLoseMoney = 0.006 }]*)

