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

Let's calculate weights for an example portfolio "A" consisting of
- \$100 invested in AAPL
- \$300 invested in GOOG
- \$500 invested in TSLA

These are all long positions, meaning that they have positive costs. 
*)

let aaplPosition = 100
let googPosition = 300
let tslaPosition = 500

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

Imagine that an investor holding PortfolioA decides that 
they want to short \$250 worth of Meta.
*)

(**
 
- A long portfolio has portfolio weights that sum to 1.0 (or 100%). 
- A zero-cost portfolio has portfolio weights that sum to 0.0.

*)

type Position = { Id: string; Position: int; Price : decimal }

let portfolio =
    [| { Id = "AAPL"; Position = 100; Price = 22.20m }
       { Id = "AMZN"; Position = 20; Price = 40.75m }
       { Id = "TSLA"; Position = 50; Price = 30.6m } |]

// We can do it on the fly
portfolio
|> Array.map(fun pos -> // getting position value
       pos.Id,
       (float pos.Position)*(float pos.Price))
|> fun ps -> // calculating weights
    let portfolioValue = 
        ps 
        |> Array.sumBy(fun (id, value) -> value)
    ps
    |> Array.map(fun (id, value) -> id, value / portfolioValue)


// We can also define a bit more structure to get the same thing.
// Proper functions with defined types are good for reusable code.
type PositionValue = { Id : string; Value : float }
type PositionWeight = { Id : string; Weight : float }

let calcValue (x:Position) = 
    { Id = x.Id
      Value = (float x.Position) * (float x.Price)}

let calcWeights xs =
    let portfolioValue = xs |> Array.sumBy(fun x -> x.Value)
    xs
    |> Array.map(fun pos -> 
        { Id = pos.Id
          Weight = pos.Value / portfolioValue })

portfolio
|> Array.map calcValue
|> calcWeights

let portfolioWithShorts =
    [| { Id = "AAPL"; Position = 100; Price = 22.20m }
       { Id = "AMZN"; Position = -20; Price = 40.75m }
       { Id = "TSLA"; Position = 50; Price = 30.6m } |]

portfolioWithShorts
|> Array.map calcValue
|> calcWeights


(**
## Mean and Variance of Portfolio Returns
*)

(**
## Leverage
*)




