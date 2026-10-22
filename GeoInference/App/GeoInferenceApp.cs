global using GeoInference.App;
global using GeoInference.Definitions.Algebras.ZExpr;
global using GeoInference.Knowledges;
global using ZTool.Bases;
global using ZTool.Infrastructures.Alias;
global using ZTool.Infrastructures.DI;
global using ZTool.Infrastructures.Log;
global using ZTool.Structures;
global using GeoInference.Definitions.Knowledges;
global using ZTool.Algorithms;
namespace GeoInference.App;

//GeoReasoningApp GeoInferenceApp

public class GeoInferenceApp
{
    public ZSingletonDI DI { get; set; } = new ZSingletonDI();
    public bool IsFinished { get; set; }
    public bool HasWarnings { get => Warnings.Count > 0; }
    public List<string> Warnings { get; set; } = [];
    public bool IsActivedStop { get; set; }
    public List<string> ActivedStopReasons { get; set; } = [];
    public bool IsCracked { get; set; } = false;
    public List<string> CrackedReasons { get; set; } = [];
    /// <summary>
    /// </summary>
    public bool ThrowOnUnhandledException { get; set; } = false;
    public static bool UseChinese { get; set; }

    List<IOutputMaker> OutputMakers;
    List<Type> plugInTypes = [];
    public void Initialize(GeoInferenceConfig config)
    {
        DI.Set(this);
        config.Components.ForEach(item => DI.Set(item.Item1, item.Item2));
        config.OutputMakers.ForEach(item => DI.Set(typeof(IOutputMaker), item));
        config.Settings.ForEach(DI.Set);
        config.PlugIns.ForEach(DI.Set);
        plugInTypes.AddRange(config.PlugIns);
        plugInTypes.AddRange(config.Settings.OfType<IInferenceConfig>().SelectMany(s => s.PlugInTypes));
    }
    /// <summary>
    /// ZLog 的命名计时器是进程级累计值，每次推理前需要清零，否则耗时会逐次叠加。
    /// </summary>
    static readonly string[] TimerNames = ["Inference", "Rule", "SemiRule", "SemiRule_Check", "EqSym", "Matrix"];

    static void ResetTimers()
    {
        foreach (var name in ZLog.GetTimeSpans().Keys)
            ZLog.ResetStopwatch(name);
        foreach (var name in TimerNames)
            ZLog.ResetStopwatch(name);
    }

    public void Inference(ZScriptInput script)
    {
        ResetTimers();
        DI.Set<TimeoutPlugIn>();
        DI.Set(script);
        DI.Check();
        plugInTypes.ForEach(t => DI.Get(t));
        DI.Get<IInferenceEngine>().Init();
        KnowledgeTimeline.Start();
        ZLog.StartStopwatch("Inference");
        DI.Get<IInferenceEngine>().Run();
        ZLog.StopStopwatch("Inference");
        IsFinished = true;
        try
        {
           
        }
        catch (Exception ex)
        {
            if (ThrowOnUnhandledException)
                throw;
            IsCracked = true;
            CrackedReasons.Add($"Unhandled exception occurred while the inference engine component was running.\n{ex}");
            return;
        }
    }
    public List<object> ExOutputs = [];
    public GeoInferenceResult MakeResult()
    {
        OutputMakers = DI.GetAll<IOutputMaker>();
        GeoInferenceResult result = new GeoInferenceResult();
        result.HasWarning = Warnings.Count > 0;
        result.Warnings = Warnings;

        result.IsActivedStop = IsActivedStop;
        result.ActivedStopReasons = ActivedStopReasons;
        result.IsCracked = IsCracked;
        result.CrackedReasons = CrackedReasons;
        result.IsFinished = !IsActivedStop && !IsCracked;

        result.InferenceWallClockTime = ZLog.GetTimeSpans().GetValueOrDefault("Inference");
        result.PeakMemoryBytes = System.Diagnostics.Process.GetCurrentProcess().PeakWorkingSet64;
#if DEBUG
        KnowledgeTimeline.Flush();
        result.KnowledgeTimesSeconds = KnowledgeTimeline.Snapshot();
#endif
        var outputs = OutputMakers.Select(m => m.Make());
        foreach (var output in outputs)
        {
            if (output is HumanLikeAnswer answer)
                result.HumanLikeAnswer = answer;
            else if (output is KnowledgeGraph kg)
                result.KnowledgeGraph = kg;
            else if (output is InferenceMetrics metrics)
                result.InferenceMetrics = metrics;
            else if (output is InferenceLog log)
                result.InferenceLog = log;
            else if (output is MultiSolutionKnowledgeBase multiSolution)
                result.MultiSolutionKnowledgeBase = multiSolution;
            else
                ExOutputs.Add(output);
        }
        return result;
    }
}
