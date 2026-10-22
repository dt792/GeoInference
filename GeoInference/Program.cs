
using GeoInference.Tools;

using H.Pipes;

using Newtonsoft.Json;

using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        GeoInferenceApp.UseChinese = true;
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

        GeoInferenceConfig config = GeoInferenceConfigs.Configs.TryGetValue(taskArgs.Config, out var c)
            ? c
            : GeoInferenceConfigs.Solving;

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
