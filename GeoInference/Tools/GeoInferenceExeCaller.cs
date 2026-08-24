using System.Diagnostics;
using System.Text;

using H.Pipes;

using Newtonsoft.Json;

namespace GeoInference.Tools;


public class GeoInferenceExeArgs
{
    public string PipeName { get; set; } = GeoInferenceExeCaller.DefaultPipeName;
    public string SavePath { get; set; } = GeoInferenceExeCaller.DefaultResultPath;
    public string Config { get; set; } = nameof(GeoInferenceConfigs.SolvingDebug);
    public string ProblemInput { get; set; } = "";
}

public class GeoInferenceExeCaller
{
    public const string DefaultPipeName = "DefaultPipeName";
    public const string DefaultProblemName = "DefaultProblemName";
    public const string DefaultResultPath = "D:\\Results";
    public const string Finished = "Finished";
    public const string Cracked = "Cracked";
    public string ExePath { get; set; } = Path.GetFullPath("GeoInference.exe");
    public string ProblemName { get; set; } = GeoInferenceExeCaller.DefaultProblemName;
    public string PipeName { get; set; } = GeoInferenceExeCaller.DefaultPipeName;
    public string SavePath = DefaultResultPath;
    public string Config = nameof(GeoInferenceConfigs.SolvingDebug);
    public string ProblemInput { get; set; } = "Points:S(2,20,0) Q(31,0,0) P(0,0,0) A(2,0,0) Z(21,0,0) R(21,20,0)\nSegs:PAZQ QR RZ SR SP SA\nTrapezoid PQRS\nRQ=12\nRS=10\nAngle SPA=45\nAngle ZQR=30\nLinesParallel SR,AZ\nLinesPerpendicular PA,SA\nLinesPerpendicular RZ,QZ\nSolve:Perimeter of quadrilateral PQRS\n";
    public bool IsCracked { get; set; }

    
    public string BuildArguments()
    {
        var taskArgs = new GeoInferenceExeArgs
        {
            PipeName = PipeName,
            SavePath = SavePath,
            Config = Config,
            ProblemInput = ProblemInput
        };
        string json = JsonConvert.SerializeObject(taskArgs);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public async Task Run()
    {
        Process? serverProcess = null;
        try
        {
            var cts = new CancellationTokenSource();
            var startInfo = new ProcessStartInfo
            {
                FileName = ExePath,
                Arguments = BuildArguments(),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };
            serverProcess = Process.Start(startInfo);
            if (serverProcess == null)
            {
                Console.WriteLine("Failed to start server, please check file permissions or path.");
                return;
            }
            //Console.WriteLine($"Problem Name: {ProblemName},Server process started (PID: {serverProcess.Id})");
            var client = new PipeClient<string>(PipeName);
            client.MessageReceived += (s, e) =>
            {
                if (e.Message == GeoInferenceExeCaller.Finished)
                {
                    //Console.WriteLine("Server finished computation, exiting...");
                    cts.Cancel();
                }
                else if (e.Message == GeoInferenceExeCaller.Cracked)
                {
                    IsCracked = true;
                    //Console.WriteLine($"Server reported crash for {ProblemName}, exiting...");
                    cts.Cancel();
                }
                else
                {
                    Console.WriteLine(e.Message);
                }
            };
            client.ExceptionOccurred += (s, e) => Console.WriteLine($"Communication exception: {e.Exception.Message}");

            await client.ConnectAsync();
            //Console.WriteLine("Connected! Start Inference!");

            await Task.Delay(Timeout.InfiniteTimeSpan, cts.Token);
        }
        catch (OperationCanceledException)
        {
            //Console.WriteLine("\nReceived cancel signal...");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"Timeout: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Run failed: {ex.Message}");
        }
        finally
        {
            if (serverProcess != null && !serverProcess.HasExited)
            {
                serverProcess.Kill();
                serverProcess.WaitForExit();
                //Console.WriteLine("Server process exited.");
            }
        }
    }
}
