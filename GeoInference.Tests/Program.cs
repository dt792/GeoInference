using GeoInference.App;
using GeoInference.Tests;

using Newtonsoft.Json;

using ScottPlot;
GeoProblemDataset.Instance.RebuildIndex(GeoProblemDataset.DatasetPath);
////batch test
IntegratedTester tester = new IntegratedTester();
//"Excessive threads during the inference process may cause calculation errors, resulting in the return of a time string. Reducing the number of threads can help mitigate this issue.";
tester.MaxThreads = 5;
await tester.Test();

//Run("3026", "FormalGeo7KL2");
IntegratedResultReporter maker = new IntegratedResultReporter();
maker.InitTotals();
maker.LoadAllData();
maker.GenerateStatuAndInferenceTimeReportExcel();
maker.GenerateAllLevelReports();

static void Run(string problemName, string dataset = "FormalGeo7KL6")
{
    //Language Switch: true->CN/false->EN
    GeoInferenceApp.UseChinese = true;
    GeoInferenceApp app = new GeoInferenceApp();

   var config=  GeoInferenceConfigs.Solving;
    GeoProblemDataset.Instance.RebuildIndex(GeoProblemDataset.DatasetPath);
    var problem = GeoProblemDataset.Instance[dataset][problemName.ToString()];
    app.Initialize(config);
    try
    {
        app.Inference(new ZScriptInput() { Script = problem.Problem_Input_EN });
    }
    catch (Exception ex)
    {
        Console.WriteLine("INFERENCE_CRASH: " + ex.Message);
    }
    var result = app.MakeResult();
    Console.WriteLine(result.HumanLikeAnswer?.IsAllSuccess);
    Console.WriteLine(result.HumanLikeAnswer?.Answers.First());
    WriteResult($"{problemName}.json", result);
    static void WriteResult(string savePath, GeoInferenceResult result)
    {
        var dir = Path.GetDirectoryName(savePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        var json = JsonConvert.SerializeObject(result, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }
}
