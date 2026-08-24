
using GeoInference.Tools;

using H.Pipes;

using Newtonsoft.Json;

using System.Text;


public class Program
{
    public static async Task Main(string[] args)
    {
        GeoInferenceApp.IsZhOrEn = false;
        if (args == null || args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
        {
            Console.WriteLine("❌ 缺少任务参数（应为 Base64 编码的任务 JSON）");
            return;
        }

        GeoInferenceExeArgs taskArgs;
        try
        {
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(args[0].Trim()));
            taskArgs = JsonConvert.DeserializeObject<GeoInferenceExeArgs>(json)
                       ?? throw new InvalidDataException("任务 JSON 反序列化结果为空");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ 任务参数解析失败: {ex.Message}");
            return;
        }

        GeoInferenceConfig config = GeoInferenceConfigs.Comfigs.TryGetValue(taskArgs.Config, out var c)
            ? c
            : GeoInferenceConfigs.SolvingDebug;

        var server = new PipeServer<string>(taskArgs.PipeName);
        var clientConnected = new TaskCompletionSource();
        server.ClientConnected += (s, e) => clientConnected.TrySetResult();

        try
        {
            await server.StartAsync();
            
            await clientConnected.Task;

            try
            {
                GeoInferenceApp app = new();
                app.Initialize(config);
                app.Inference(new ZScriptInput(taskArgs.ProblemInput));
                var result = app.MakeResult();
                WriteResult(taskArgs.SavePath, result);
                await server.WriteAsync(GeoInferenceExeCaller.Finished);
            }
            catch (Exception ex)
            {
                GeoInferenceResult inferenceResult = new()
                {
                    IsCracked = true,
                    CrackedReasons = new List<string> { ex.ToString() }
                };
                WriteResult(taskArgs.SavePath, inferenceResult);
                await server.WriteAsync(GeoInferenceExeCaller.Cracked);
            }

            
            await Task.Delay(300);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed: {ex.Message}");
        }
        finally
        {
            await server.StopAsync();
            Console.WriteLine("✅ Server stopped safely.");
        }
    }

    static void WriteResult(string savePath, GeoInferenceResult result)
    {
        var dir = Path.GetDirectoryName(savePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
        var json = JsonConvert.SerializeObject(result, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }
}

//GeoProblemDataset.Instance.RebuildIndex(GeoProblemDataset.DatasetPath);
//var problem = GeoProblemDataset.Instance["FormalGeo7KL6"]["1106"];


//var configName = "default";
//var question = problem.Problem_Input_CN;
////question = @"Points:A(0,0) B(0,1) C(1,1) D(1,2) E(2,2) F(2,3)
////LineParallel AB,CD
////LineParallel CD,EF
////Prove:LineParallel AB,EF
////";
//if (args.Count() == 1)
//{
//    question = args[0];
//}
//else if (args.Count() == 2)
//{
//    configName = args[0];
//    question = args[1];
//}
//GeoInferenceApp app = new();
//var config = GeoInferenceConfigs.Comfigs[configName];
//var script = new ZScriptInput() { Script = question };
//app.Initialize(config);
//app.Inference(script);
//var result = app.MakeResult();
//Console.WriteLine(JsonTool.ToJson(result));

////foreach (var item in GeoProblemDataset.Instance["FormalGeo7KL6"].Take(100))
////{
////    app = new();
////     question = problem.Problem_Input_CN;
////     config = GeoInferenceConfigs.Comfigs[configName];
////     script = new ZScriptInput() { Script = question };
////    app.Initialize(config);
////    app.Inference(script);
////    result = app.MakeResult();
////    Console.WriteLine(JsonTool.ToJson(result));
////}