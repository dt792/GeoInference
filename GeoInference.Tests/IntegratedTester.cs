
using System.Diagnostics;

using GeoInference.Tools;

public class IntegratedTester
{
    public const string DefaultResultPath = "..\\..\\..\\..\\Result";
    public string[] Exculed = ["1935", "787", "665", "854", "1525", "3468", "5013", "4592"];
    public int MaxThreads = 8;
    public async Task Test()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        GeoInferenceTester tester = new GeoInferenceTester();
        //await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL1, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        //await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL2, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        //await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL3, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        //await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL4, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        //await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL5, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        //await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL6, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        //-----
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL1, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL1, Exculed, nameof(GeoInferenceConfigs.SolvingNoSimplifyEqSym), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL1, Exculed, nameof(GeoInferenceConfigs.Discovering), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL1, Exculed, nameof(GeoInferenceConfigs.SolvingNoCompassMatrix), maxConcurrency: MaxThreads);

        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL2, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL2, Exculed, nameof(GeoInferenceConfigs.SolvingNoSimplifyEqSym), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL2, Exculed, nameof(GeoInferenceConfigs.Discovering), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL2, Exculed, nameof(GeoInferenceConfigs.SolvingNoCompassMatrix), maxConcurrency: MaxThreads);

        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL3, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL3, Exculed, nameof(GeoInferenceConfigs.SolvingNoSimplifyEqSym), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL3, Exculed, nameof(GeoInferenceConfigs.Discovering), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL3, Exculed, nameof(GeoInferenceConfigs.SolvingNoCompassMatrix), maxConcurrency: MaxThreads);

        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL4, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL4, Exculed, nameof(GeoInferenceConfigs.SolvingNoSimplifyEqSym), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL4, Exculed, nameof(GeoInferenceConfigs.Discovering), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL4, Exculed, nameof(GeoInferenceConfigs.SolvingNoCompassMatrix), maxConcurrency: MaxThreads);

        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL5, Exculed, nameof(GeoInferenceConfigs.Solving), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL5, Exculed, nameof(GeoInferenceConfigs.SolvingNoSimplifyEqSym), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL5, Exculed, nameof(GeoInferenceConfigs.Discovering), maxConcurrency: MaxThreads);
        await tester.RunBatchAsync(GeoProblemDataset.FormalGeo7KL5, Exculed, nameof(GeoInferenceConfigs.SolvingNoCompassMatrix), maxConcurrency: MaxThreads);

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
    string _datasetName;
    public async Task RunBatchAsync(string datasetName, string[] exculed = null, string mode = nameof(GeoInferenceConfigs.SolvingDebug), string exePath = DefaultExePath, string resultDir = DefaultResultPath, int maxConcurrency = 16)
    {
        _mode = mode;
        _datasetName = datasetName;
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
            int totalBefore = problemList.Count;
            int crashedCount = 0;
            var completedNames = new HashSet<string>();
            foreach (var file in Directory.EnumerateFiles(resultDir, "*.json"))
            {
                if (IsCrackedResult(file))
                {
                    crashedCount++;
                    continue;
                }
                completedNames.Add(Path.GetFileNameWithoutExtension(file));
            }
            problemList = problemList.Where(p => !completedNames.Contains(p.Name)).ToList();
            Console.WriteLine($"[{mode}] Skipped {totalBefore - problemList.Count} already-completed problem(s), re-running {crashedCount} crashed problem(s), {problemList.Count} remaining.");
        }

        _totalCount = problemList.Count;
        _completedCount = 0;
        _crackedCount = 0;
        _finishedCount = 0;

        _globalStopwatch.Restart();
        Console.WriteLine($"Starting tasks: {_totalCount} total, max concurrency: {_maxConcurrency}");
        _lastRenderedProcessed = -1;
        _lastRenderedLineWidth = 0;
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
                    bool isCracked = await ExecuteSingleAsync(problem, pipeName, mode, cancellationToken, exePath, savePath);
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        // Completed and cracked are mutually exclusive: a cracked problem only counts as cracked.
                        if (isCracked)
                        {
                            Interlocked.Increment(ref _crackedCount);
                            WriteLogLine($"[{problem.Name}] Cracked, result recorded; continuing with remaining tasks.");
                        }
                        else
                        {
                            Interlocked.Increment(ref _completedCount);
                        }
                    }
                }
                finally
                {
                    int finished = Interlocked.Increment(ref _finishedCount);
                    PrintProgress(finished);
                    semaphore.Release();
                }
            }, cancellationToken);

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        _globalStopwatch.Stop();
        WriteLogLine($"All done! Total time: {_globalStopwatch.Elapsed:mm\\:ss}, completed: {_completedCount}, cracked: {_crackedCount}, processed: {_finishedCount}/{_totalCount}");
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
        Console.WriteLine("  2) Only remaining: keep existing results, run problems without a result file or with a crashed result");
        Console.Write("Please choose [1/2] (default 2): ");
        string answer = Console.ReadLine()?.Trim();

        if (answer == "1")
        {
            EnsureEmptyDirectory(resultDir);
            return false;
        }
        return true;
    }

    private static bool IsCrackedResult(string path)
    {
        try
        {
            var probe = Newtonsoft.Json.JsonConvert.DeserializeObject<CrackedProbe>(File.ReadAllText(path));
            return probe?.IsCracked ?? true;
        }
        catch
        {
            return true;
        }
    }

    private class CrackedProbe
    {
        public bool IsCracked { get; set; }
    }

    private int _maxConcurrency;
    private readonly Stopwatch _globalStopwatch = new();

    private int _completedCount = 0;
    private int _crackedCount = 0;
    private int _finishedCount = 0;
    private int _totalCount = 0;

    // All console rendering goes through this lock; otherwise the N concurrent tasks
    // interleave their writes and the single-line progress bar gets shredded.
    private readonly object _consoleLock = new();
    private int _lastRenderedProcessed = -1;
    private int _lastRenderedLineWidth = 0;
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

        lock (_consoleLock)
        {
            if (processed > _totalCount) processed = _totalCount;

            // Tasks finish in arbitrary order, so a thread that incremented later can get here
            // first. Never let the bar move backwards, otherwise the shorter line leaves garbage.
            if (processed < _lastRenderedProcessed) return;
            _lastRenderedProcessed = processed;

            int percent = (int)((double)processed / _totalCount * 100);
            int completed = Volatile.Read(ref _completedCount);
            int cracked = Volatile.Read(ref _crackedCount);

            string prefix = $"{_datasetName}_{_mode}:[";
            string suffix = $"] {percent}% Completed:{completed} Cracked:{cracked} Processed:{processed}/{_totalCount}";

            int barLength = 50;
            try
            {
                int width = Console.WindowWidth;
                if (width > 0)
                    barLength = Math.Clamp(width - prefix.Length - suffix.Length - 1, 10, 50);
            }
            catch (IOException) { }
            catch (ArgumentOutOfRangeException) { }

            int filled = (int)((double)processed / _totalCount * barLength);
            string bar = new string('█', filled) + new string('-', barLength - filled);

            string line = $"{prefix}{bar}{suffix}";
            if (line.Length < _lastRenderedLineWidth)
                line = line.PadRight(_lastRenderedLineWidth);

            Console.Write("\r" + line);
            _lastRenderedLineWidth = line.Length;

            if (processed >= _totalCount)
            {
                Console.WriteLine();
                _lastRenderedLineWidth = 0;
            }
        }
    }

    // Log output must erase the in-place progress line first, otherwise the next bar redraw
    // overwrites the beginning of the message.
    private void WriteLogLine(string message)
    {
        lock (_consoleLock)
        {
            if (_lastRenderedLineWidth > 0)
            {
                Console.Write("\r" + new string(' ', _lastRenderedLineWidth) + "\r");
                _lastRenderedLineWidth = 0;
            }
            Console.WriteLine(message);
        }
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