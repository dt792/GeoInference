
using GeoInference.Tools;

using System.Collections.Concurrent;
using System.Diagnostics;

public class IntegratedTester
{
    public const string DefaultResultPath = "..\\..\\..\\..\\Result";
    public string[] Exculed = ["854", "1525", "3468", "5013", "4592"];
    public int MaxThreads = 16;
    public async Task Test()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        GeoInferenceTester tester = new GeoInferenceTester();
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL6, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL6, Exculed, nameof(GeoInferenceConfigs.SolvingNoSimplifyEqSym), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL6, Exculed, nameof(GeoInferenceConfigs.Discovering), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL6, Exculed, nameof(GeoInferenceConfigs.SolvingNoCompassMatrix), maxConcurrency: MaxThreads);

        stopwatch.Stop();
        Console.WriteLine($"✅ Finished. duration :{stopwatch.Elapsed}");
    }
}
public class GeoInferenceTester
{
    public const string DefaultResultPath = "..\\..\\..\\..\\Result";
    public const string DefaultExePath = "GeoInference.exe";
    GeoProblemDataset dataset = GeoProblemDataset.Instance;
    public GeoInferenceTester()
    {
        dataset.RebuildIndex(GeoProblemDataset.DatasetPath);
    }
    string _mode;
    public async Task RunBatchAsync(string datasetName, string[] exculed = null, string mode = nameof(GeoInferenceConfigs.SolvingDebug), string exePath = DefaultExePath, string resultDir = DefaultResultPath, int maxConcurrency = 16)
    {
        _mode = mode;
        _maxConcurrency = maxConcurrency;
        CancellationToken cancellationToken = default;

        var problemList = dataset[datasetName].ToList();
        if (exculed != null)
        {
            problemList = problemList.Where(p => !exculed.Contains(p.Name)).ToList();
        }

        resultDir = $"{resultDir}\\{datasetName}_{mode}";
        resultDir = Path.GetFullPath(resultDir);
        exePath = Path.GetFullPath(exePath);

        bool skipExisting = PrepareResultDirectory(resultDir, mode);
        if (skipExisting)
        {
            var existingNames = Directory.EnumerateFiles(resultDir, "*.json")
                .Select(Path.GetFileNameWithoutExtension)
                .ToHashSet();
            int totalBefore = problemList.Count;
            problemList = problemList.Where(p => !existingNames.Contains(p.Name)).ToList();
            Console.WriteLine($"[{mode}] Skipped {totalBefore - problemList.Count} already-completed problem(s), {problemList.Count} remaining.");
        }

        _totalCount = problemList.Count;
        _completedCount = 0;
        _failedCount = 0;

        _globalStopwatch.Restart();
        Console.WriteLine($"Starting tasks: {_totalCount} total, max concurrency: {_maxConcurrency}");
        PrintProgress(0);

        var semaphore = new SemaphoreSlim(_maxConcurrency);
        var tasks = new List<Task>();

        foreach (var problem in problemList)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            await semaphore.WaitAsync(cancellationToken);
            var pipeName = $"GeoPipe_{problem.Name}_{Guid.NewGuid():N}";
            var savePath = $"{resultDir}\\{problem.Name}.json";
            var task = Task.Run(async () =>
            {
                try
                {
                    bool isCracked = true;
                    while (isCracked && !cancellationToken.IsCancellationRequested)
                    {
                        isCracked = await ExecuteSingleAsync(problem, pipeName, mode, cancellationToken, exePath, savePath);
                        if (isCracked)
                        {
                            DeleteFileIfExists(savePath);
                            Interlocked.Increment(ref _failedCount);
                            Console.WriteLine($"\n[{problem.Name}] Execution failed, retrying immediately...");
                        }
                    }

                    if (!cancellationToken.IsCancellationRequested)
                        Interlocked.Increment(ref _completedCount);
                }
                finally
                {
                    PrintProgress(_completedCount);
                    semaphore.Release();
                }
            }, cancellationToken);

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        _globalStopwatch.Stop();
        Console.WriteLine($"\nAll done! Total time: {_globalStopwatch.Elapsed:mm\\:ss}, completed: {_completedCount}, failed: {_failedCount}");
    }

    private bool PrepareResultDirectory(string resultDir, string mode)
    {
        bool hasFiles = Directory.Exists(resultDir) && Directory.EnumerateFiles(resultDir, "*.json").Any();
        if (!hasFiles)
        {
            EnsureEmptyDirectory(resultDir);
            return false;
        }

        Console.WriteLine($"\n[{mode}] Result directory already contains result files: {resultDir}");
        Console.WriteLine("  1) Full overwrite: clear existing results and re-run all problems");
        Console.WriteLine("  2) Only remaining: keep existing results, run only problems without a result file");
        Console.Write("Please choose [1/2] (default 2): ");
        string answer = Console.ReadLine()?.Trim();

        if (answer == "1")
        {
            EnsureEmptyDirectory(resultDir);
            return false;
        }
        return true;
    }

    private static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    private int _maxConcurrency;
    private readonly Stopwatch _globalStopwatch = new();

    private int _completedCount = 0;
    private int _failedCount = 0;
    private int _totalCount = 0;
    private async Task<bool> ExecuteSingleAsync(GeoProblem problem, string pipeName, string mode, CancellationToken cancellationToken,
        string exePath = DefaultExePath, string savePath = DefaultResultPath)
    {
        var caller = new GeoInferenceExeCaller
        {
            PipeName = pipeName,
            ProblemName = problem.Name,
            ProblemInput = problem.Problem_Input_EN,
            ExePath = exePath,
            Config = mode,
            SavePath = savePath
        };

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromMinutes(10));

        await caller.Run();
        return caller.IsCracked;
    }
    private void PrintProgress(int processed)
    {
        if (_totalCount == 0) return;

        int percent = (int)((double)processed / _totalCount * 100);
        int barLength = 50;
        int filled = (int)((double)processed / _totalCount * barLength);

        string bar = new string('█', filled) + new string('-', barLength - filled);

        Console.Write($"\r{_mode}:[{bar}] {percent}% Completed:{_completedCount} Failed:{_failedCount}");

        if (processed >= _totalCount)
            Console.WriteLine();
    }
    public static void EnsureEmptyDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            foreach (var file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                Directory.Delete(dir, true);
            }
        }
        else
        {
            Directory.CreateDirectory(path);
        }
    }
}