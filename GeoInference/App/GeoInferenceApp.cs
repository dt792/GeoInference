global using GeoInference.App;
global using GeoInference.Definitions.Algebras.ZExpr;
global using GeoInference.Knowledges;
global using ZTool.Bases;
global using ZTool.Infrastructures.Alias;
global using ZTool.Infrastructures.DI;
global using ZTool.Infrastructures.Log;
global using ZTool.Structures;
global using GeoInference.Definitions.Knowledges;
global using ZGeoReasoning.Definitions.Deductions;
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
    public static bool IsZhOrEn { get; set; }

    List<IOutputMaker> OutputMakers;
    public void Initialize(GeoInferenceConfig config)
    {
        config.Components.ForEach(item => DI.Set(item.Item1, item.Item2));
        config.PlugIns.ForEach(DI.Set);
        config.OutputMakers.ForEach(item => DI.Set(typeof(IOutputMaker), item));
        config.Settings.ForEach(DI.Set);
        
    }
    public void Inference(ZScriptInput script)
    {
        DI.Set(script);
        DI.Check();
        DI.Get<IInferenceEngine>().Init();
        try
        {
            ZLog.StartStopwatch("Inference");
            DI.Get<IInferenceEngine>().Run();
            ZLog.StopStopwatch("Inference");
            IsFinished = true;
        }
        catch (Exception ex)
        {
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

        result.InferenceTime = ZLog.GetTimeSpans().GetValueOrDefault("Inference");
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
            else
                ExOutputs.Add(output);
        }
        return result;
    }
}
