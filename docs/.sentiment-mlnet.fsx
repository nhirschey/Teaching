#r "nuget: Microsoft.ML, 1.7.*"
#r "nuget: Microsoft.ML.FastTree"
#r "nuget: DiffSharp-lite"

open System
open System.IO
open Microsoft.ML
open Microsoft.ML.Data
open DiffSharp
open DiffSharp.Data

dsharp.config(device=Device.CPU)

/// Type representing the text to run sentiment analysis on.
[<CLIMutable>]
type Sentiment =
    {
        [<LoadColumn(0)>]
        Label: bool

        [<LoadColumn(1)>]
        Text: string
    }

/// Result of sentiment prediction.
[<CLIMutable>]
type  SentimentPrediction = 
    { 
        // ColumnName attribute is used to change the column name from
        // its default value, which is the name of the field.
        [<ColumnName("PredictedLabel")>]
        Prediction : bool; 

        // No need to specify ColumnName attribute, because the field
        // name "Probability" is the column name we want.
        Probability : float32; 

        Score : float32 
    }

/// <summary>
/// IMDB movie reviews dataset from http://ai.stanford.edu/~amaas/data/sentiment/:
/// "This is a dataset for binary sentiment classification containing
///  substantially more data than previous benchmark datasets. 
/// We provide a set of 25,000 highly polar movie reviews for training, 
/// and 25,000 for testing. There is additional unlabeled data for use as well.
/// Raw text and already processed bag of words formats are provided. 
/// See the README file contained in the release for more details."
/// </summary>
/// <param name="root">Directory where the datasets are saved. Defaults to ".data". If they do not exist they will be downloaded to this location.</param>
/// <param name="split">The train or test dataset. Defaults to train.</param>
/// <param name="url">URL to download the dataset file from. If not provided, the dataset will be downloaded from http://ai.stanford.edu/~amaas/data/sentiment/</param>
type IMDB(?root:string, ?split:string, ?url:string) =
    // Pytorch uses a split parameter that can be 'train', 'test', or ('train','test').
    let split = 
        let split = defaultArg split "train"
        if not (List.contains split ["train"; "test"]) 
        then failwithf $"""Expected split to be one of "train" or "test" but was {split}."""
        else split
    let path = Path.Combine(defaultArg root ".data", "imdb") |> Path.GetFullPath
    let pathExtracted = Path.Combine(path, "aclImdb")
    let url = defaultArg url "http://ai.stanford.edu/~amaas/data/sentiment/aclImdb_v1.tar.gz"
    let file = Path.Combine(path, Path.GetFileName(url))


    let loadIMDB directory =
        let reviews sentiment = 
            Path.Combine(pathExtracted, directory, sentiment)
            |> Directory.EnumerateFiles
            |> Seq.map(fun file -> File.ReadAllText(file), sentiment)
        seq { yield! reviews "pos"
              yield! reviews "neg" }
        |> Seq.toArray

    let data, target =
        if not (File.Exists(file)) then download url file
        if not (Directory.Exists(pathExtracted)) then extractTarGz file path
        loadIMDB split
        |> Array.unzip
    member d.classes = 2
    member d.classNames = [|"pos"; "neg" |]
    member d.length = data.Length
    member d.description = "IMDB"
    member d.item(i) = target.[i], data.[i]
    member d.Item
        with get(i:int) =
            d.item(i)

let imdbTrainDataset = IMDB(root="data",split="train")
let imdbTestDataset = IMDB(root="data",split="test")

let labelData (dataset: IMDB) =
    [ for i = 0 to dataset.length-1 do
        let (label, text) = dataset[i]
        { Label = if label = "pos" then true else false 
          Text = text }]

let imdbTrainLabelled = labelData imdbTrainDataset
let imdbTestLabelled = labelData imdbTestDataset

let ctx = new MLContext(seed = 1)

let trainingDataView = ctx.Data.LoadFromEnumerable(imdbTrainLabelled)
let testDataView = ctx.Data.LoadFromEnumerable(imdbTestLabelled)

// inspect
trainingDataView.Preview(3).RowView

let dataProcessPipeline = ctx.Transforms.Text.FeaturizeText("Features","Text")

dataProcessPipeline.Preview(trainingDataView,5).RowView
|> Seq.iter
    (fun row ->
        row.Values
        |> Array.map (function KeyValue(k,v) -> sprintf "| %s:%O" k v)
        |> Array.fold (+) "Row--> "
        |> printfn "%s\n"
    )

let sampleFeaturized = dataProcessPipeline.Fit(trainingDataView)

[<CLIMutable>]
type TransformedText = { Features: single [] }
let samplePredictionEngine = ctx.Model.CreatePredictionEngine<Sentiment,TransformedText>(sampleFeaturized)

samplePredictionEngine.Predict(imdbTrainLabelled[0])

dataProcessPipeline.Preview(trainingDataView,5).RowView
|> Seq.collect (fun row ->
    row.Values
    |> Array.filter (function KeyValue(k,v) -> k = "Features")
    |> Array.map (function KeyValue(k,v) -> v))


let trainer = 
    ctx.BinaryClassification.Trainers
        .FastTree(labelColumnName="Label",featureColumnName="Features")
let trainingPipeline = dataProcessPipeline.Append(trainer)

let trainedModel = trainingPipeline.Fit(trainingDataView)

let trainPredictions = trainedModel.Transform trainingDataView


let trainMetrics = ctx.BinaryClassification.Evaluate(trainPredictions,"Label","Score")

let predictions = trainedModel.Transform testDataView
let metrics = ctx.BinaryClassification.Evaluate(predictions, "Label", "Score")

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
    printfn"************************************************************"

printBinaryClassificationMetrics "IMDB-Train" trainMetrics
printBinaryClassificationMetrics "IMDB-Test" metrics

let options = Trainers.FastTree.FastTreeBinaryTrainer.Options()
options.NumberOfTrees
options.