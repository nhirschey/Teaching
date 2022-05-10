(**
---
title: Calls NN
category: Lectures
categoryindex: 1
index: 11
---

[![Binder](https://colab.research.google.com/assets/colab-badge.svg)](https://colab.research.google.com/github/nhirschey/teaching/blob/gh-pages/{{fsdocs-source-basename}}.ipynb)&emsp;
[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

*)

(*** condition: ipynb ***)
#if IPYNB
// Google Colab only: uncomment and run the following to install dotnet and the F# kernel
// !bash <(curl -Ls https://raw.githubusercontent.com/gbaydin/scripts/main/colab_dotnet6.sh)
#endif // IPYNB

(***do-not-eval***)

#r "nuget:FSharp.Stats"
// Use lite if you're on Apple Silicon
#r "nuget:DiffSharp-lite,1.0.7"
// Use CPU if you're on Windows/Linux/Intel Mac
//#r "nuget: DiffSharp-cpu,1.0.7"
#r "nuget: Plotly.NET, 2.0.*"
#r "nuget: Plotly.NET.Interactive, 2.0.*"

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
Below is some temp fixes for rnn state saving until the next DiffSharp release comes out. DiffSharp is in active development and a fix is in process.
*)
type ModelBase with
    member m.saveState2(fileName, ?noDiff:bool) =
        let noDiff = defaultArg noDiff true
        let ss =
            if noDiff then 
                let ss = m.state.copy()
                ss.noDiff()
                ss
            else m.state
        saveBinary ss fileName
    member m.loadState2(filename) =
        let s:ParameterDict = loadBinary filename
        m.state.iter(fun (n,p) -> p.value <- s[n])


(**
## Data
We'll use a dataset containing transcripts of quarterly conference calls from NASADAQ100 companies from 2018 to 2021. Let's download that.

*)

let gunzip (inputFile:string) (outputFile:string) =
    Directory.CreateDirectory(Path.GetDirectoryName(outputFile)) |> ignore
    if File.Exists(outputFile) then File.Delete(outputFile)
    use inputStream = File.OpenRead(inputFile)
    use outputStream = File.Create(outputFile)
    use gzipStream = new GZipStream(inputStream, CompressionMode.Decompress)
    gzipStream.CopyTo(outputStream)

let nq100FullUrl = "https://www.dropbox.com/s/izcsjp06lgwbauu/Nasdaq100CallFull.json.gz?dl=1"
let dataFolder = "data"
let nqFullFile = Path.Combine(dataFolder, "Nasdaq100CallFull.json")
let nq100FullFileGz = nqFullFile.Replace(".json", ".json.gz")

download nq100FullUrl nq100FullFileGz
gunzip nq100FullFileGz nqFullFile

(**
Read the downloaded data into a list
*)
(***do-not-eval***)

// Types - Earnings Announcement
type CallId =
    { Ticker: string
      Exchange: string
      FiscalQuarter: int
      Date: DateTime }

type CallFull = 
    { CallId: CallId
      Header: string
      PreparedRemarks: string
      QuestionsAndAnswers: string
      Label: float }

let corpus = 
    File.ReadAllText(nqFullFile)
    |> JsonSerializer.Deserialize<List<CallFull>>
    |> List.map (fun x -> 
        let toTake = min 10_000 x.QuestionsAndAnswers.Length-1
        x.QuestionsAndAnswers[..toTake])
    |> String.concat " "

corpus.Length

corpus[..500]

(**
Set up the NN model.
*)
let seqLen = 64
let batchSize = 16
let hiddenSize = 512
let numLayers = 2

(**
By default, TextDataset uses characters in the corpus you give it.
Just in case our train/valid texts have different characters, we'll
make our own.
*)
let chars = 
    corpus.ToCharArray() 
    |> Array.distinct 
    |> Array.map string 
    |> String.concat ""
let splitIndex = corpus.Length-2_000
let dataset = TextDataset(corpus[..splitIndex], seqLen,chars=chars)
let validDataset = TextDataset(corpus[splitIndex+1..], seqLen,chars=chars)
let loader = dataset.loader(batchSize=batchSize, shuffle=true)
let validLoader = validDataset.loader(batchSize=batchSize, shuffle=false)

(**
Actual model definition
*)
let rnn = LSTM(dataset.numChars, hiddenSize, numLayers=numLayers, batchFirst=true)
let decoder = dsharp.view([-1; hiddenSize]) --> Linear(hiddenSize, dataset.numChars)
let languageModel = rnn --> decoder

printfn "%s" (languageModel.summary())


(** If we want to retrain from a saved model. *)
(***do-not-eval***)
let modelFileName = "data/rnn_language_model_1.07.params"

if false then // keep this false the first time to use a local  model.
    let modelUrl = "https://www.dropbox.com/s/zzm2lrzbwwigzc8/rnn_language_model_1.07.params?dl=1"
    download modelUrl modelFileName // downloads pre-trained model.

if File.Exists(modelFileName) then 
    printfn "Resuming training from existing model params found: %A" modelFileName
    languageModel.loadState2(modelFileName)

(** Prediction function. *)
let predict (text:string) len =
    rnn.reset()
    let mutable prediction = text
    let mutable last = text
    for i in 1..len do
        let lastTensor = last |> dataset.textToTensor
        let nextCharProbs = lastTensor.unsqueeze(0) --> languageModel --> dsharp.slice([-1]) --> dsharp.softmax(-1)
        last <- Categorical(nextCharProbs).sample() |> int |> dataset.indexToChar |> string
        prediction <- prediction + last
    prediction

(** Test a prediction.*)
predict "Analyst Good morning. What do you see for gross margins?" 512


(**
The actual model training.
*)
let optimizer = Adam(languageModel, lr=dsharp.tensor(0.001))

let losses = ResizeArray<float>()

let epochs = 10
let validInterval = 100

let start = System.DateTime.Now
for epoch = 1 to epochs do
    for i, x, t in loader.epoch() do
        let input =  x[*,..seqLen-2]
        let target = t[*,1..]
        rnn.reset()
        languageModel.reverseDiff()
        let output = input --> languageModel
        let loss = dsharp.crossEntropyLoss(output, target.view(-1))
        loss.reverse()
        optimizer.step()
        losses.Add(float loss)
        printfn "%A Epoch: %A/%A minibatch: %A/%A loss: %A" (System.DateTime.Now - start) epoch epochs (i+1) loader.length (float loss)
        GC.Collect()

        if i % validInterval = 0 then
            printfn "\nSample from language model:\n%A\n" (predict "Analyst Yes, I guess the first question," 512)

            languageModel.saveState2(modelFileName)
(*
            let validLoss =
                let lossPerBatch =
                    validLoader.epoch()
                    |> Seq.map (fun (_, x, t) ->
                        let input =  x[*,..seqLen-2]
                        let target = t[*,1..]
                        rnn.reset()
                        let output = input --> languageModel
                        let loss = dsharp.crossEntropyLoss(output, target.view(-1))
                        loss)
                    |> dsharp.stack
                lossPerBatch.mean().toDouble()
            let out = sprintf $"{System.DateTime.Now-start}, {epoch}, {i}, {float loss}\n"
            let lossesFile = modelFileName.Replace(".params","_losses.txt")
            if not (File.Exists(lossesFile)) then File.WriteAllText(lossesFile, "epoch,time,minibatch,loss\n")
            File.AppendAllText(lossesFile,out) 
            GC.Collect()
*)


