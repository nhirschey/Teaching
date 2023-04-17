(**
---
title: Performance Evaluation
category: Lectures
categoryindex: 1
index: 100
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)


# Performance evaluation

We're going to evaluate portfolio performance. The common way to do this is to estimate a portfolio's return adjusted for risk using a factor model with tradeable risk factors. 

What's a risk factor? These risk factors are portfolios and the idea is that the expected excess return on these risk factors is compensation to investors for bearing the risk inherent in holding those portfolios. For the return variation in these factors to be "risky", it should be something that investors cannot easily diversify. If it was easy to diversify, then investors could put a small bit of the asset in their portfolio and capture the return without affecting portfolio volatility. That would imply being able to increase return without adding risk. Hence the requirement that a factor constitute return variation that is hard to diversify away.

The greater the riskiness of the factor, the greater the factor's expected return (i.e., the risk-return tradeoff). For example, most people feel that stocks are riskier than bonds and indeed stocks have historically had higher returns than bonds.

The risk adjustment involves estimating a portfolio's $\beta$'s on different risk factors. These $\beta$'s constitute the exposure of the portfolio to the risk factor. If the factor return goes up by 1%, then the portfolio's return goes up by $\beta \times 1\%$. 

We can estimate these $\beta$'s by OLS regressions of the portfolio's returns on contemporaneous returns of the risk factors. The slope coefficients on the risk factors are the portfolio's betas on the risk factors. The regression intercept is known as $\alpha$. It represents the average return of the portfolio that is not explained by the portfolio's $\beta$'s on the risk factors. This alpha is the risk-adjusted return. 

Intuitively, $\alpha$ is the average return on a portfolio long the investment you are evaluating and short a portfolio with the same factor risk as that portfolio. If the factors and factor betas accurately measure the portfolio's risk, then the alpha is the portfolio's return that is unrelated to the portfolio's risk. Investors like positive alphas because that implies that the portfolio's return is higher than what investors require for bearing the portfolio's risk.

One thing to keep in mind is that throughout this discussion, we have discussed things from the perspective of arbitrage. That is, like a trader. We have not made any assumptions about utility functions or return distributions. This is the Arbitrage Pricing Theory (APT) of Stephen Ross (1976). He was motivated by the observation that

> "... on theoretical grounds it is difficult to justify either the assumption [in mean-variance anlysis and CAPM] of normality in returns...or of quadratic preferences...and on empirical grounds the conclusions as well as the assumptions of the theory have also come under attack."

The APT way of thinking is less restrictive than economically motivated equilibrium asset pricing models. Which is nice. But it has the cost that it does not tell us as much. With the APT we cannot say precisely what a security's return should be. We can only say that if we go long a portfolio and short the portfolio that replicates its factor exposure, then the alpha shouldn't be *too* big. But if we're thinking like a trader, that's perhaps most of what we care about anyway.


*)

#r "nuget: FSharp.Stats"
#r "nuget: FSharp.Data"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"
#r "nuget: NovaSBE.Finance"

(** *)
#r "nuget: Quotes.YahooFinance, 0.0.5"

open System
open FSharp.Data
open Quotes.YahooFinance

open FSharp.Stats
open Plotly.NET
open NovaSBE.Finance
open NovaSBE.Finance.Ols

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__


(*** condition: fsx ***)
#if FSX
fsi.AddPrinter<DateTime>(fun dt -> dt.ToString("s"))
fsi.AddPrinter<YearMonth>(fun ym -> $"{ym.Year}-{ym.Month}")
#endif // FSX

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

(** A function to accumulate simple returns. *)
let cumulativeReturn (xs: seq<DateTime * float>) =
    let mutable cr = 1.0

    [ for (dt, r) in xs do
          cr <- cr * (1.0 + r)
          dt, cr - 1.0 ]

(** Plot of vbr cumulative return. *)
let vbrChart =
    vbr
    |> Array.map (fun x -> x.YearMonth, x.Return)
    |> cumulativeReturn
    |> Chart.Line

(***condition:ipynb***)
#if IPYNB
vbrChart
#endif // IPYNB

(***condition:fsx***)
#if FSX
vbrChart |> Chart.show
#endif // FSX

(***hide***)
vbrChart |> GenericChart.toChartHTML
(*** include-it-raw ***)

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

(** One way to evaluate performance is Sharpe ratios. *)

/// Calculates sharpe ratio of a sequence of excess returns
let sharpe (xs: float seq) = (Seq.mean xs) / (Seq.stDev xs)

let annualizeMonthlySharpe monthlySharpe = sqrt (12.0) * monthlySharpe


(** Our portfolio. *)
regData |> Array.map (fun x -> x.Portfolio) |> sharpe |> annualizeMonthlySharpe
(***include-output***)

(** The market. *)
regData |> Array.map (fun x -> x.MktRf) |> sharpe |> annualizeMonthlySharpe
(***include-output***)

(** The HML factor. *)
regData |> Array.map (fun x -> x.Hml) |> sharpe |> annualizeMonthlySharpe
(***include-output***)


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
- What's the interpretation of the alpha?
- What's the interpretation of the beta?
- If you want to replicate VBR with the MKT factor and risk-free bonds, what are the weights that you would use?
*)

(**
Fama-French 3-Factor model results
*)
ff3Estimate.summary ()

(**
- What's the interpretation of the alpha?
- What's the interpretation of the factor betas?
- If you want to replicate VBR with risk free bonds and the MKT, HML, and SMB factors, what are the weights that you would use?
*)

(**
> **Practice:** What is the expected annual return of VBR? When answering this, 
>
> - Assume that your alpha and beta estimates for the 3 factor model explaining VBR returns are accurate. 
> - Use the average annual premia on the Fama and French factors from 1926 until end of 2022 as your estimate of the factors' expected returns.

*)

// Answer here

(**
You will probably see that the CAPM $R^2$ is lower than the
Fama-French $R^2$. This means that you can explain more of the
portfolio's returns with the Fama-French model. Or in trader terms,
you can hedge the portfolio better with the multi-factor model.
*)

(**
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
- What's the interpretation of the alpha?
- What's the interpretation of the betas?
- If you want to replicate HML using VTI, VBR, and risk-free bonds, what are the weights you would use?

Note:

HML average return.
*)
hmlRegData |> Array.averageBy (fun x -> 12.0 * x.Hml)

(**
VTI average return
*)
hmlRegData |> Array.averageBy (fun x -> 12.0 * x.Vti)

(** VBR average return. *)
hmlRegData |> Array.averageBy (fun x -> 12.0 * x.Vbr)

(** 
## Information Ratios
We want residuals so that we can estimate information ratios.*)
let capmResiduals = capmEstimate.resid
let ff3Residuals = ff3Estimate.resid

(**
In general I would write a function to do this. Function makes it a bit
simpler to follow. It's hard for me to read the next few lines and understand
what everything is. Too much going on.
*)

let capmAlpha = 12.0 * capmEstimate.coefs["Intercept"]
let capmStDevResiduals = sqrt (12.0) * (Seq.stDev capmResiduals)
let capmInformationRatio = capmAlpha / capmStDevResiduals
(*** include-fsi-output***)

let ff3Alpha = 12.0 * ff3Estimate.coefs["Intercept"]
let ff3StDevResiduals = sqrt (12.0) * (Seq.stDev ff3Residuals)
let ff3InformationRatio = ff3Alpha / ff3StDevResiduals
(*** include-fsi-output***)

(** Here is the function version.*)

let informationRatio monthlyAlpha (monthlyResiduals: float array) =
    let annualAlpha = 12.0 * monthlyAlpha
    let annualStDev = sqrt (12.0) * (Seq.stDev monthlyResiduals)
    annualAlpha / annualStDev

informationRatio capmEstimate.coefs["Intercept"] capmResiduals
(*** include-fsi-output***)

informationRatio ff3Estimate.coefs["Intercept"] ff3Residuals
(*** include-fsi-output***)
