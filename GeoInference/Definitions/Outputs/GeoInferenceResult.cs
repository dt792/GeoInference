
public class GeoInferenceResult
{
    // Wall-clock time of the inference phase (from DI/engine init completion to reasoning termination).
    // JsonProperty keeps compatibility with existing result files that use the old "InferenceTime" key.
    [Newtonsoft.Json.JsonProperty("InferenceTime")]
    public TimeSpan InferenceWallClockTime { get; set; }
    public long PeakMemoryBytes { get; set; }
    public bool IsFinished { get; set; } = false;
    public bool HasWarning { get; set; } = false;
    public List<string> Warnings { get; set; } = [];
    public bool IsActivedStop { get; set; }
    public List<string> ActivedStopReasons { get; set; } = [];
    public bool IsCracked { get; set; } = false;
    public List<string> CrackedReasons { get; set; } = [];
    public List<double> KnowledgeTimesSeconds { get; set; }
    public InferenceMetrics InferenceMetrics { get; set; }
    public InferenceLog InferenceLog { get; set; }
    public HumanLikeAnswer HumanLikeAnswer { get; set; }
    public KnowledgeGraph KnowledgeGraph { get; set; }
    public MultiSolutionKnowledgeBase MultiSolutionKnowledgeBase { get; set; }
}
