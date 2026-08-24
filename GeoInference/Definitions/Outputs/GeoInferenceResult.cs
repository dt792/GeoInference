
public class GeoInferenceResult
{
    public TimeSpan InferenceTime { get; set; }
    public bool IsFinished { get; set; } = false;
    public bool HasWarning { get; set; } = false;
    public List<string> Warnings { get; set; } = [];
    public bool IsActivedStop { get; set; }
    public List<string> ActivedStopReasons { get; set; } = [];
    public bool IsCracked { get; set; } = false;
    public List<string> CrackedReasons { get; set; } = [];
    public InferenceMetrics InferenceMetrics { get; set; }
    public InferenceLog InferenceLog { get; set; }
    public HumanLikeAnswer HumanLikeAnswer { get; set; }
    public KnowledgeGraph KnowledgeGraph { get; set; }
}
