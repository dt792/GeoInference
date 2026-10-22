/// <summary>
/// </summary>
[Alias("多解知识库")]
public class MultiSolutionKnowledgeBase
{
    public List<KnowledgeCateInfo> Nodes { get; set; } = [];
    /// <summary>
    /// </summary>
    public List<ConditionInfo> Targets { get; set; } = [];
}

/// <summary>
/// </summary>
public class MultiPathKnowledgeInfo : KnowledgeInfo
{
    public string Type { get; set; }
    public int Index { get; set; }
    public List<InferencePathInfo> Paths { get; set; } = [];
}

/// <summary>
/// </summary>
public class InferencePathInfo
{
    public string Reason { get; set; }
    public List<ConditionInfo> Conditions { get; set; } = [];
}
