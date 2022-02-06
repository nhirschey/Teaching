(**

---
title: Basic Portfolio Statistics
category: Lectures
categoryindex: 1
index: 2
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

This page covers important fundamentals for building portfolios.

- [Portfolio Weights](#Portfolio-Weights)
- [Mean and Variance of Portfolio Returns](#Mean-and-Variance-of-Portfolio-Returns) 
- [Leverage](#Leverage)

*)

(**
## Portfolio Weights

An investment portfolio consists of positions in assets. 
It is common to refer to a position's size as its share of
the portfolio's total value. 
This is known as the asset's portfolio weight.

The portfolio weight of asset $i$ in portfolio $p$ is

$$w_i=(\text{positionValue}_i)/(\text{portfolioValue}_p)$$.

### Long positions

Let's calculate weights for an example *Portfolio A* consisting of
- \$100 invested in AAPL
- \$300 invested in GOOG
- \$500 invested in TSLA

These are all long positions, meaning that they have positive costs. 
*)

let aaplPosition = 100.0
let googPosition = 300.0
let tslaPosition = 500.0

// This implies:

let portfolioValueA = aaplPosition + googPosition + tslaPosition

(***include-fsi-output***)

(**
The portfolio weights are then
*)

let aaplWeight = aaplPosition / portfolioValueA
(***include-fsi-output***)

let googWeight = googPosition / portfolioValueA
(***include-fsi-output***)

let tslaWeight = tslaPosition / portfolioValueA
(***include-fsi-output***)

(**
These weights for AAPL, GOOG, and TSLA are all positive.
Long positions always have positive weights.

Another thing to notice is that the portfolio weights add up to one (or 100%):
*)

aaplWeight + googWeight + tslaWeight
(***include-fsi-output***)

(**
This portfolio is a net long portfolio, 
meaning that it costs you money to purchase it. 
Net long portfolios such as this one must 
have portfolio weights that add up to one.
Due to margin requirements, real-world
portfolios are generally net long--you must
put up capital to acquire the portfolio.

The other type of portfolio is a zero-cost portfolio. 
As the name implies, zero-cost portfolios do not require any investment to purchase.
There is no cost because long positions are funded by offsetting short positions.
To see how this works we need to examine how short positions work.

### Short positions

When an investor buys a long position, 
they pay for the position now and hope to sell it later at a higher price.
A short sale reverses this.
The investor sells the position now and hopes to buy it back later at a lower price.
To see how this works, it is helpful to compare the cash flows
from a long purchase to those from a short sale.

First, a long purchase.

- At time 0: investor has \$100 cash and no shares.
- At time 1: investor takes their \$100 of cash and buys 4 shares 
  of stock at a price of \$25 per share. They have no cash and are long 4 shares.
- At time 2: the stock has risen to \$27, the investor sells their 4 shares.
  They have \$108 of cash and no shares.

The investor's cash and stock balances at the end of each period will look something like 

| Time | Cash Balance | Shares | Share Price | Stock Position Value | Account Value |
--------------------------------------------------------------------------------------
| 0   | \$100.       | 0       | \$25.       | \$0                  |\$100.   |
| 1   | \$0.         | 4       | \$25.       | \$100                |\$100.   |
| 2   | \$108.       | 0       | \$27.       | \$0.                 |\$108.   |

Now, a short position.

- At time 0: investor has \$100 cash and no shares.
- At time 1: investor borrows 4 shares of stock and sells them
  for a price of \$25 per share. They have \$200 cash and are short 4 shares.
- At time 2: the stock has risen to \$27, the investor buys back the
  4 shares that they sold short and returns them to the person that
  they borrowed them from. They have $92 of cash and 0 shares.

The investor's cash and stock balances at the end of each period will look something like 

| Time | Cash Balance | Shares | Share Price | Stock Position Value | Account Value |
--------------------------------------------------------------------------------------
| 0   | \$100.       | 0       | \$25.       | \$0                  |\$100.   |
| 1   | \$200        | -4      | \$25.       | -\$100               |\$100.   |
| 2   | \$92         | 0       | \$27.       | \$0.                 |\$92     |

Let's now create a new portfolio, *Portfolio B*, that includes short sales and calculate weights. Assume that you start with *Portfolio A* and short \$150 of AMZN stock. This generates \$150 of cash that you have to put somewhere. For individual investors, often your broker puts it in bonds and gives you none of the interest. Institutional investors can get some of the interest or even reinvest the proceeds in something else. We will assume that we are an institution and can reinvest all of the short proceeds. We will take the \$150 and add \$50 to each of our AAPL, GOOG, and TLSA positions. 

*)

let amznPositionB = -150.0
let aaplPositionB = aaplPosition + 50.0
let googPositionB = googPosition + 50.0
let tslaPositionB = tslaPosition + 50.0

let portfolioValueB = 
    amznPositionB +
    aaplPositionB +
    googPositionB +
    tslaPositionB
(***include-fsi-output***)

(* Compare to *Portfolio A* *)
portfolioValueA = portfolioValueB 
(***include-fsi-output***)

(**
The weights in *Portfolio B*:
*)

let amznWeightB = amznPositionB / portfolioValueB
(***include-fsi-output***)

let aaplWeightB = aaplPositionB / portfolioValueB
(***include-fsi-output***)

let googWeightB = googPositionB / portfolioValueB
(***include-fsi-output***)

let tslaWeightB = tslaPositionB / portfolioValueB
(***include-fsi-output***)

(**
The weights of *Portfolio B* also add up to one.
*)

amznWeightB + aaplWeightB + googWeightB + tslaWeightB
(***include-fsi-output***)

(**
### Zero-cost portfolios

Another type of portfolio that you will see is zero-cost portfolios.
They are called self funding because the short sale proceeds
fund the long investments. The portfolio weights add up to 0.
You can scale weights relative to what they would be per \$ long or short.

An example:
*)

// Portfolio C
let koPosition = -50.0
let hogPosition = 40.0
let yumPosition = 10.0

let scale = 50.0
let koWeight = koPosition / scale
let hogWeight = hogPosition / scale
let yumWeight = yumPosition / scale

koWeight + hogWeight + yumWeight
(***include-fsi-weight***)


(**
Now show how to do stuff with a collection
*)

type Position = { Id: string; PositionValue: float }

let portfolio =
    [ { Id = "AMZN"; PositionValue = amznPositionB }
      { Id = "AAPL"; PositionValue = aaplPositionB }
      { Id = "GOOG"; PositionValue = googPositionB }
      { Id = "TSLA"; PositionValue = tslaPositionB } ]

let positionValues = [ for p in portfolio -> p.PositionValue ]
let portfolioValue = positionValues |> List.sum
let portfolioWeights =
    [ for p in portfolio -> 
        p.Id, p.PositionValue / portfolioValue ]

(**
## Mean and Variance of Portfolio Returns
*)

(**
## Leverage
*)




