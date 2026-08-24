using GeoInference.App;
using GeoInference.Tests;

using Newtonsoft.Json;

using ScottPlot;
GeoProblemDataset.Instance.RebuildIndex(GeoProblemDataset.DatasetPath);
////batch test
IntegratedTester tester = new IntegratedTester();
//"Excessive threads during the inference process may cause calculation errors, resulting in the return of a time string. Reducing the number of threads can help mitigate this issue."
tester.MaxThreads = 10;
await tester.Test();

////Run("6892");
IntegratedResultReporter maker = new IntegratedResultReporter();
maker.Total = GeoProblemDataset.Instance["FormalGeo7KL6"].Count();
maker.LoadAllData();
maker.GenerateStatuAndInferenceTimeReportExcel();
maker.GenerateMetricsTables();
maker.GenerateMetricsCharts();


static void Run(string problemName)
{
    //Language Switch: true->CN/false->EN
    GeoInferenceApp.IsZhOrEn = false;
    GeoInferenceApp app = new GeoInferenceApp();
    var config = GeoInferenceConfigs.Discovering;
    GeoProblemDataset.Instance.RebuildIndex(GeoProblemDataset.DatasetPath);
    var problem = GeoProblemDataset.Instance["FormalGeo7KL6"][problemName.ToString()];
    app.Initialize(config);
    app.Inference(new ZScriptInput() { Script= problem.Problem_Input_CN });
    var result= app.MakeResult();
    Console.WriteLine(result.HumanLikeAnswer?.IsAllSuccess);
    Console.WriteLine(result.HumanLikeAnswer?.Answers.First());
    WriteResult($"{problemName}.json",result);
    static void WriteResult(string savePath, GeoInferenceResult result)
    {
        var dir = Path.GetDirectoryName(savePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        var json = JsonConvert.SerializeObject(result, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }
}
