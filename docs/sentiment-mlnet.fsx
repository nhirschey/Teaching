(**
---
title: Sentiment classification
category: Lectures
categoryindex: 1
index: 10
---

[![Binder](img/badge-binder.svg)](https://mybinder.org/v2/gh/nhirschey/teaching/gh-pages?filepath={{fsdocs-source-basename}}.ipynb)&emsp;
[![Script](img/badge-script.svg)]({{root}}/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook](img/badge-notebook.svg)]({{root}}/{{fsdocs-source-basename}}.ipynb)

*)

(**
> Developed with [Antonio Salomao](https://github.com/aexsalomao), building off his independent work [here](https://aexsalomao.github.io/ConferenceCalls/).

A lot of the growth in machine learning involves learning from loosely structured data such as text and images. The following analysis provides a light introduction to learning from textual data. We will see if it is possible to identify whether a firm is reporting good or bad results from the text of their quarterly earnings conference call. This is something that can be learned from looking at financial statements, but we want to see if we can train a model to learn similar information from the words spoken during the call. This is known as sentiment analysis and has been explored in finance contexts by [Tetlock (2007)](https://scholar.google.com/citations?view_op=view_citation&hl=en&citation_for_view=MwKqqlAAAAAJ:3fE2CSJIrl8C), [Loughran and McDonald (2011)](https://scholar.google.com/citations?view_op=view_citation&hl=en&citation_for_view=FnFYSIQAAAAJ:blknAaTinKkC), and others.

We're going to use [ML.NET](https://dotnet.microsoft.com/en-us/apps/machinelearning-ai/ml-dotnet), which provides a production-ready API for training and deploying machine learning models.

To start we'll load some libaries.
*)

#r "nuget:FSharp.Stats"
#r "nuget: Microsoft.ML, 1.7.*"
#r "nuget: Microsoft.ML.FastTree"
#r "nuget: FSharp.Data"
#r "nuget: Plotly.NET, 2.0.*"

#time "on"

open System
open System.IO
open System.IO.Compression
open System.Text.Json
open System.Net
open System
open FSharp.Data
open FSharp.Stats
open Plotly.NET
open Microsoft.ML
open Microsoft.ML.Data
open Microsoft.ML.Transforms.Text

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

(*** condition: ipynb ***)
#if IPYNB
// Set dotnet interactive formatter to plaintext
Formatter.Register(fun (x:obj) (writer: TextWriter) -> fprintfn writer "%120A" x )
Formatter.SetPreferredMimeTypesFor(typeof<obj>, "text/plain")
// Make plotly graphs work with interactive plaintext formatter
Formatter.SetPreferredMimeTypesFor(typeof<GenericChart.GenericChart>,"text/html")
#endif // IPYNB


(**
## Data
We'll use a dataset containing transcripts of quarterly conference calls from NASADAQ100 companies from 2018 to 2021. Let's download that.

*)

(***do-not-eval***)

let download (inputUrl:string) (outputFile:string) =
    Directory.CreateDirectory(Path.GetDirectoryName(outputFile)) |> ignore
    if IO.File.Exists(outputFile) then
        printfn $"The file {outputFile} already exists. Skipping download" 
    else
        let web = Http.RequestStream(inputUrl)
        use fileStream = IO.File.Create(outputFile)
        web.ResponseStream.CopyTo(fileStream)
        fileStream.Close()

// Decompress a gzip file
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
You should now have a file called `Nasdaq100CallFull.json` in the `data` folder.

Let's read it into a list.
*)

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

type CallOnlyQA =
    { CallId: CallId
      Header: string
      QuestionsAndAnswers: string
      Label: float }

let nq100Full = 
    File.ReadAllText(nqFullFile)
    |> JsonSerializer.Deserialize<List<CallFull>>

(**
Let's look at a call.
*)

let tsla2021q4 =
    nq100Full
    |> List.find (fun x -> 
        x.CallId.Ticker = "TSLA" &&
        x.CallId.Date.Year = 2021 && 
        x.CallId.FiscalQuarter = 4)

(**
The opening of the prepared remarks section.
*)
let elonStarts = tsla2021q4.PreparedRemarks.IndexOf("Elon has some opening remarks. Elon?")
tsla2021q4.PreparedRemarks[elonStarts..elonStarts+1_000]

(**
Opening of the Q&A section.
*)

let firstAnalystQuestion = tsla2021q4.QuestionsAndAnswers.IndexOf("Please go ahead.")
tsla2021q4.QuestionsAndAnswers[firstAnalystQuestion..firstAnalystQuestion+1_000]

(**
And the market-adjusted stock return from the day before to the day after the call.
*)
tsla2021q4.Label

(**
Typical word lengths of the prepared remarks, Q&A, and market return.
*)

let preparedLengthChart =
    nq100Full
    |> Seq.map (fun x -> x.PreparedRemarks.Split([|' '|]).Length)
    |> Chart.Histogram
    |> Chart.withTraceInfo(Name = "Prepared Remarks Length")


let qaLengthChart =
    nq100Full
    |> Seq.map (fun x -> x.QuestionsAndAnswers.Split([|' '|]).Length)
    |> Chart.Histogram
    |> Chart.withTraceInfo(Name = "Q&A Length")

let returnChart =
    nq100Full
    |> Seq.map (fun x -> x.Label)
    |> Chart.Histogram
    |> Chart.withTraceInfo(Name = "Return")

[ preparedLengthChart; qaLengthChart; returnChart ]
|> Chart.SingleStack()

(**
## Binary sentiment model
Is the market's reaction to the call correlated with the text of the call? 

We need some types that work with ML.NET.
*)

[<CLIMutable>]
type BinarySentimentInput =
    { Label: bool
      Text: string }

[<CLIMutable>]
type BinarySentimentOutput =
    { PredictedLabel: bool
      Probability: single
      Score: single }

// ML.NET context
let ctx = new MLContext(seed = 1)

(**
A train and test split of the data.
*)

let nq100FullSentiment =
    nq100Full
    |> Seq.map (fun x ->
        { Label = x.Label > 0.0
          Text =  x.QuestionsAndAnswers })
    |> ctx.Data.LoadFromEnumerable    
    

let nq100FullSplits =
    ctx.Data.TrainTestSplit(nq100FullSentiment,
                            testFraction = 0.2, 
                            seed = 1)

(**
ML.NET has some built-in featurization [transforms](https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/prepare-data-ml-net) that we can use to prepare the data.

[FeaturizeText](https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.textcatalog.featurizetext?view=ml-dotnet) converts the text into vectors of normalized word and character n-grams.
*)

let featurizePipeline = 
    ctx.Transforms.Text.FeaturizeText(
        outputColumnName = "Features", 
        inputColumnName = "Text")

(**
There are many different trainers.
*)

let treeTrainer = 
    ctx.BinaryClassification.Trainers.FastTree(
        labelColumnName = "Label",
        featureColumnName = "Features")


(**
We can put the featurization and the trainer together into a pipeline.
*)

let treePipeline = featurizePipeline.Append(treeTrainer)

(**
Trained model.
*)
let binaryTreeModel = treePipeline.Fit(nq100FullSplits.TrainSet)

(**
Model performance. First some functions to compute metrics.
*)

let computeMetrics (model:TransformerChain<_>) iDataView =
    let predictions = model.Transform iDataView
    ctx.BinaryClassification
        .Evaluate(predictions, 
                  labelColumnName = "Label",
                  scoreColumnName = "Score")

let printBinaryClassificationMetrics name (metrics : CalibratedBinaryClassificationMetrics) =
    printfn"************************************************************"
    printfn"*       Metrics for %s binary classification model      " name
    printfn"*-----------------------------------------------------------"
    printfn"*       Accuracy:                             %.2f%%" (metrics.Accuracy * 100.)
    printfn"*       Area Under Curve:                     %.2f%%" (metrics.AreaUnderRocCurve * 100.)
    printfn"*       Area under Precision recall Curve:    %.2f%%" (metrics.AreaUnderPrecisionRecallCurve * 100.)
    printfn"*       F1Score:                              %.2f%%" (metrics.F1Score * 100.)
    printfn"*       LogLogg:                              %.2f%%" (metrics.LogLoss)
    printfn"*       LogLossreduction:                     %.2f%%" (metrics.LogLossReduction)
    printfn"*       PositivePrecision:                    %.2f" (metrics.PositivePrecision)
    printfn"*       PositiveRecall:                       %.2f" (metrics.PositiveRecall)
    printfn"*       NegativePrecision:                    %.2f" (metrics.NegativePrecision)
    printfn"*       NegativeRecall:                       %.2f" (metrics.NegativeRecall)
    printfn"*\n-----------------------------------------------------------"
    printfn"*      Confusion matrix for %s binary classification model      " name
    printfn"*-----------------------------------------------------------"
    printfn $"{(metrics.ConfusionMatrix.GetFormattedConfusionTable())}"
    printfn"************************************************************"

(**
Now let's actually look at the model performance.

We should be good in the training set.
*)

nq100FullSplits.TrainSet 
|> computeMetrics binaryTreeModel
|> printBinaryClassificationMetrics "Train set"

(**
The test is how well we do in the test set.
*)

nq100FullSplits.TestSet 
|> computeMetrics binaryTreeModel
|> printBinaryClassificationMetrics "Test set"

(**
That looks pretty good. But maybe there's something special about our train/test sample.

Let's try k-fold cross validation. If we do 5 folds,
that means that we split the data into 5 random groups. 
Then we train the model on 4/5 of the data and test on the remaining 1/5.
We do this 5 times, cycling through the data. 
*)

let downcastPipeline (pipeline : IEstimator<'a>) =
    match pipeline with
    | :? IEstimator<ITransformer> as p -> p
    | _ -> failwith "The pipeline has to be an instance of IEstimator<ITransformer>."

//https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/train-machine-learning-model-cross-validation-ml-net
let cvResults = 
    ctx.BinaryClassification
        .CrossValidate(data = nq100FullSentiment, 
                       estimator = downcastPipeline treePipeline,
                       numberOfFolds=5,
                       seed = 1) 

(**
Results.
*)
cvResults
|> Seq.iteri (fun i x -> printfn $"Fold {i+1}: {x.Metrics.Accuracy}")

cvResults
|> Seq.averageBy (fun  x -> x.Metrics.Accuracy)
|> printfn "Average accuracy: %.2f%%"

(**
That's actually pretty good.

Let's try a model on prepared remarks.
*)

let nq100PreparedSentiment =
    nq100Full
    |> Seq.map (fun x ->
        { Label = x.Label > 0.0
          Text =  x.PreparedRemarks })
    |> ctx.Data.LoadFromEnumerable    

let cvPreparedRemarks = 
    ctx.BinaryClassification
        .CrossValidate(data = nq100PreparedSentiment, 
                       estimator = downcastPipeline treePipeline,
                       numberOfFolds=5, 
                       seed = 1)

(** Look at the cross validation performance. *)
cvPreparedRemarks
|> Seq.iteri (fun i x -> printfn $"Fold {i+1}: {x.Metrics.Accuracy}")

cvPreparedRemarks
|> Seq.averageBy (fun  x -> x.Metrics.Accuracy)
|> printfn "Average accuracy: %.2f%%"

(**
It's not a huge difference, but prepared remarks is not as informative as the Q&A.
That can make sense if management tries to put a positive spin on things.
*)

(**
Is the model improved by training it with extreme events?
*)

let nq100ExtremeSentiment =
    nq100Full
    |> Seq.filter (fun x -> (abs x.Label) > 0.075)
    |> Seq.map (fun x ->
        { Label = x.Label > 0.0
          Text =  x.QuestionsAndAnswers })
    |> ctx.Data.LoadFromEnumerable    

let cvExtreme = 
    ctx.BinaryClassification
        .CrossValidate(data = nq100ExtremeSentiment, 
                       estimator = downcastPipeline treePipeline,
                       numberOfFolds=5, 
                       seed = 1)

(** Look at the cross validation performance. *)
cvExtreme
|> Seq.iteri (fun i x -> printfn $"Fold {i+1}: {x.Metrics.Accuracy}")

cvExtreme
|> Seq.averageBy (fun  x -> x.Metrics.Accuracy)
|> printfn "Average accuracy: %.2f%%"

(**
## Multiclass model

The multiple classes will be "positive", "negative", and "neutral".
*)

[<CLIMutable>]
type MulticlassSentimentInput =
    { Label: string
      Text: string }

[<CLIMutable>]
type MulticlassSentimentOutput =
    { PredictedLabel: string
      Probability: single
      Score: single }

let nq100MulticlassSentiment =
    nq100Full
    |> Seq.map (fun x ->
        { Label = 
            if x.Label < -0.05 then "neg"
            elif x.Label > 0.05 then "pos"
            else "neutral"
          Text =  x.QuestionsAndAnswers })
    |> ctx.Data.LoadFromEnumerable    

let nq100MulticlassSplits =
    ctx.Data.TrainTestSplit(nq100MulticlassSentiment,
                            testFraction = 0.2, 
                            seed = 1)

(**
Picking a multi-class trainer.
*)
let multiTrainer = 
    ctx.MulticlassClassification.Trainers.SdcaMaximumEntropy(
        labelColumnName = "Label",
        featureColumnName = "Features")

(**
The finished pipeline.
*)
let multiPipeline =
    // Estimator chain seems to speed this up.
    EstimatorChain()
        .Append(featurizePipeline)
        // for multiclass, you have to put the label in a keyvalue store
        .Append(ctx.Transforms.Conversion.MapValueToKey("Label"))
        .AppendCacheCheckpoint(ctx)
        .Append(multiTrainer)

(**
The model (this can be slow to train).
*)
let multiModel = multiPipeline.Fit(nq100MulticlassSplits.TrainSet)

(**
Evaluating the model.
*)

let computeMultiClassMetrics (model:TransformerChain<_>) iDataView =
    let predictions = model.Transform iDataView
    ctx.MulticlassClassification
        .Evaluate(predictions, 
                  labelColumnName = "Label",
                  scoreColumnName = "Score")

let printMultiClassClassificationMetrics name (metrics : MulticlassClassificationMetrics) =
    printfn "************************************************************"
    printfn "*    Metrics for %s multi-class classification model   " name
    printfn "*-----------------------------------------------------------"
    printfn "    AccuracyMacro = %.4f, a value between 0 and 1, the closer to 1, the better" metrics.MacroAccuracy
    printfn "    AccuracyMicro = %.4f, a value between 0 and 1, the closer to 1, the better" metrics.MacroAccuracy
    printfn "    LogLoss = %.4f, the closer to 0, the better" metrics.LogLoss
    printfn "    LogLoss for class 1 = %.4f, the closer to 0, the better" metrics.PerClassLogLoss.[0]
    printfn "    LogLoss for class 2 = %.4f, the closer to 0, the better" metrics.PerClassLogLoss.[1]
    printfn "    LogLoss for class 3 = %.4f, the closer to 0, the better" metrics.PerClassLogLoss.[2]
    printfn "************************************************************"
    printfn $"{metrics.ConfusionMatrix.GetFormattedConfusionTable()}"

nq100MulticlassSplits.TrainSet
|> computeMultiClassMetrics multiModel
|> printMultiClassClassificationMetrics "Multi-class: TrainSet"

nq100MulticlassSplits.TestSet
|> computeMultiClassMetrics multiModel
|> printMultiClassClassificationMetrics "Multi-class: TestSet"

(**
A function to make predictions.
*)
let binaryTreePredictions = 
    ctx.Model.CreatePredictionEngine<BinarySentimentInput, BinarySentimentOutput>(binaryTreeModel)

(**
Look at test output for a negative call.
*)
let sampleNegCall :BinarySentimentInput = { 
    Label = false
    Text = "Our earnings are terrible. All our customers are leaving 
    and are profits and free cash flow is falling. "}

binaryTreePredictions.Predict(sampleNegCall)

(**
Look at test output for a positive call.
*)
let samplePosCall: BinarySentimentInput = { 
    Label = true
    Text = "We had very high free cash flow. Sales were up, profits were up, 
    we paid down debt. We expect to beat expectations."}

binaryTreePredictions.Predict(samplePosCall)


(**
## Other areas for exploration

Setting options on the text featurizer.
*)

let textFeatureOptions =
    // Set up word n-gram options
    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.transforms.text.wordbagestimator.options?view=ml-dotnet
    let wordOptions = new WordBagEstimator.Options()
    wordOptions.NgramLength <- 2
    wordOptions.MaximumNgramsCount <- [| for i = 0 to wordOptions.NgramLength-1 do 1_000 |]

    // Set up stop word options
    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.transforms.text.textfeaturizingestimator.options.stopwordsremoveroptions?view=ml-dotnet
    let stopOptions = new StopWordsRemovingEstimator.Options()

    // Set up char n-gram options
    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.transforms.text.charbagestimator.options?view=ml-dotnet
    let charOptions = null                                        
    
    // Set the text options
    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.transforms.text.textfeaturizingestimator.options?view=ml-dotnet
    let textOptions = new Transforms.Text.TextFeaturizingEstimator.Options()
    textOptions.CharFeatureExtractor <- charOptions
    textOptions.WordFeatureExtractor <- wordOptions
    textOptions.StopWordsRemoverOptions <- stopOptions
    
    // return the text options
    textOptions

let featurizePipelineCustom = 
    ctx.Transforms.Text
        .FeaturizeText("Features",
                       "Text", 
                       options=textFeatureOptions)