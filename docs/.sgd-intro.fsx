#r "nuget:FSharp.Stats"
// Use lite if you're on Apple Silicon
//#r "nuget:DiffSharp-lite,1.0.7"
// Use CPU if you're on Windows/Linux/Intel Mac
#r "nuget: DiffSharp-cpu,1.0.7"
#r "nuget: Plotly.NET, 3.*"
#r "nuget: Plotly.NET.Interactive, 3.*"

open DiffSharp
open DiffSharp.Compose
open DiffSharp.Model
open DiffSharp.Data
open DiffSharp.Optim
open DiffSharp.Util
open DiffSharp.Distributions

open System
open System.IO
open System.IO.Compression
open System.Text.Json

open Plotly.NET

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

//dsharp.config(backend=Backend.Reference,device=Device.CPU)
dsharp.config(backend=Backend.Torch, device=Device.CPU)
dsharp.seed(1)

(*** condition: ipynb ***)
#if IPYNB
// Set dotnet interactive formatter to plaintext
Formatter.Register(fun (x:obj) (writer: TextWriter) -> fprintfn writer "%120A" x )
Formatter.SetPreferredMimeTypesFor(typeof<obj>, "text/plain")
// Make plotly graphs work with interactive plaintext formatter
Formatter.SetPreferredMimeTypesFor(typeof<GenericChart.GenericChart>,"text/html")
#endif // IPYNB


(**
A neural network is a way to make predictions from data. You give the model data as input and the model multiplies the input data by some weights and then produces an output. We see this pattern in other forms.

Imagine you have input data $x$ and weights $\alpha$ and $\beta$. In ordinary least squares we have $f(x)= \alpha + \beta x$. The function $f(x)$ multiplies the input $x$ by some weights and produces the output. Neural networks are the same, except the function $f(x)$ is more complicated.

One of the things about neural networks is that they are a bit more involved to train. We have a procedure:

- Define the model with random weights.
- Form predictions from data using that model.
- Calculate the "loss", or error, of the model.
- Estimate the best way to change the weights to reduce the loss. This is the gradient, how changing the weights affects the loss.
- Update the weights based on that estimate and repeat this training process until the model is sufficiently accurate or until you start to overfit and model accuracy is deteriorating.

Let's look at how that works.
*)

let f (x: Tensor) = x ** 2.0

(** That's a tensor function. 

Evaluate it at 2.0. 
*)

f (dsharp.tensor(2.0))

(** Evaluate it at 3.0. *)
f (dsharp.tensor(3.0))


(** We can calculate the gradient, which is the derivative evaluated at a particular point. 

If $f(x) = x^2$ then the derivative is $2x$.

Gradient evaluate at 3 should be $2*3 = 6$.
*)

dsharp.grad f (dsharp.tensor(3.0))

(** Gradient evaluated at $-2$ should be $2*-2=-4$. *)
dsharp.grad f (dsharp.tensor(-2.0))


(**
We can plot the function evaluated at different points.
*)

[-3.0 .. 0.5 .. 3.0]
|> List.map (fun x -> x, x**2.0)
|> Chart.Line

(** 
If we wanted to minimize the function, we'd be searching for the minimum which occurs when $x=0$.

Let's see how we could do that with gradient descent. We create a tensor named `x`. By adding `.reverseDiff()` to the end we are telling diffsharp to add reverse mode auto-differentiation support to the tensor.

*)
let x = dsharp.tensor(3.0).reverseDiff()

x
(**
That's what the `:rev` suffix means.

Now let's have a function that takes such an input and returns a scalar output.
*)

let fx = f x

(**
`fx` is the result of `f()` evaluated at the point `x`.

If we now call `fx.reverse`, we will calculate the reverse mode gradient of `f(x)` at the point `x`. Basically, work backwards from fx to x, calculating how changing x will affect fx. That is, calculate $\delta fx/ delta x$.
*)
fx.reverse()

(**
If we then look at the x, we can see that we have the derivatives of fx with respect to x.
*)
x.derivative

(**
That tells us the slope of `f` with respect to x. Also known as the gradient in this context.

We can use that for optimization. Imagine that we wanted to see how to change the weight in order to minimize `f(x)`. We can move opposite the gradient.

You typically don't move all the way towards the gradient. You move by a small amount, controlled by a parameter called the learning rate.

*)

let lr = 0.1
let newX = x - x.derivative * lr
newX

(**
We can look at `f` evaluated at those new points compared to `f` evaluated at the old points.
*)
f newX, f x

(**
We should have gotten smaller. This is gradient descent.

We can see the same thing in a loop.
*)

let mutable lastValue = dsharp.tensor(-3.0)
for i = 1 to 10 do
    let y = lastValue.reverseDiff()
    let fy = f y
    fy.reverse()
    lastValue <- lastValue - y.derivative * lr
    printfn $"y=%.4f{float lastValue}, f(y)=%.4f{float fy}"
 
    
    

(***hide***)
//Let's try another.

let xs = dsharp.tensor([1..20])
let ys = (xs + dsharp.randn(20)) ** 2.0

Seq.zip (xs.toArray1D<float>()) (ys.toArray1D<float>())
|> Chart.Line


