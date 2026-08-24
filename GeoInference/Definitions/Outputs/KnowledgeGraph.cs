using System.Text;

using ZTool.Structures;

public class ContinuedEqualityCateInfo
{
    public string Cate { get; set; }
    public List<ContinuedEqualityInfo> ContinuedEqualityInfos { get; set; } = [];
    public override string ToString() => Cate;
}

public class ContinuedEqualityInfo
{
    public string Content { get; set; }
    public override string ToString() => Content;
    public List<string> ReasonGraphNode { get; set; } = [];
    public List<ZPair<string, string>> ReasonGraphEdge { get; set; } = [];
}

public class ConditionalKnowledgeCateInfo
{
    public string Cate { get; set; }
    public List<ConditionalKnowledgeInfo> ConditionalKnowledgeInfos { get; set; } = [];
    public override string ToString() => Cate;
}

public class ConditionalKnowledgeInfo
{
    public string CondictionalKnowledge { get; set; }
    public List<ConditionGroupInfo> CondictionGroupInfos { get; set; } = [];
    public override string ToString() => CondictionalKnowledge + StringTool.ComposeList(CondictionGroupInfos);
}

public class ConditionGroupInfo
{
    public string Reason { get; set; }
    public List<string> Condictions { get; set; } = [];
    public override string ToString() => StringTool.ComposeList(Condictions) + "\t" + Reason;
}

public class KnowledgeCateInfo
{
    public string Name { get; set; }
    public List<KnowledgeInfo> KnowledgeInfos { get; set; } = [];
    public override string ToString() => Name;
}

public class ConditionInfo
{
    public int Index { get; set; }
    public string Type { get; set; }
}

public class KnowledgeInfo
{
    public string Content { get; set; }
    public string Reason { get; set; }
    public bool IsAvailable { get; set; } = true;
    public override string ToString() => Content;
    public List<ConditionInfo> Conditions { get; set; } = new List<ConditionInfo>();
}

[Alias("知识图谱")]
public class KnowledgeGraph
{
    /// <summary>
    
    /// </summary>
    public List<KnowledgeCateInfo> Figures { get; set; } = [];

    /// <summary>
    
    /// </summary>
    public List<KnowledgeCateInfo> Relations { get; set; } = [];

    /// <summary>
    
    /// </summary>
    public List<ConditionalKnowledgeCateInfo> ConditionalKnowledgeInfos { get; set; } = [];

    /// <summary>
    
    /// </summary>
    public List<KnowledgeCateInfo> GeoQuantityRelations { get; set; } = [];

    /// <summary>
    
    /// </summary>
    public List<KnowledgeCateInfo> GeoQuantityValues { get; set; } = [];

    /// <summary>
    
    /// </summary>
    public List<KnowledgeInfo> GeoEquations { get; set; } = [];

    /// <summary>
    
    /// </summary>
    public List<KnowledgeCateInfo> GeoEquationCateInfos { get; set; } = [];

    /// <summary>
    
    /// </summary>
    public List<KnowledgeCateInfo> EqSymCateInfos { get; set; } = [];

    /// <summary>
    
    /// </summary>
    public List<ContinuedEqualityCateInfo> ContinuedEqualityCateInfos { get; set; } = [];

    public List<KnowledgeInfo> FindKnowledgeInfoList(string typeName)
    {
        if (Figures.Exists(x => x.Name == typeName))
            return Figures.First(x => x.Name == typeName).KnowledgeInfos;
        else if (Relations.Exists(x => x.Name == typeName))
            return Relations.First(x => x.Name == typeName).KnowledgeInfos;
        else if (GeoQuantityValues.Exists(x => x.Name == typeName))
            return GeoQuantityValues.First(x => x.Name == typeName).KnowledgeInfos;
        else if (GeoQuantityRelations.Exists(x => x.Name == typeName))
            return GeoQuantityRelations.First(x => x.Name == typeName).KnowledgeInfos;
        else if ("Equations" == typeName)
            return GeoEquations;
        throw new NotImplementedException();
    }

    
    string GetConditionText(ConditionInfo condiction)
    {
        List<KnowledgeInfo> cate;
        try { cate = FindKnowledgeInfoList(condiction.Type); }
        catch (NotImplementedException) { return ""; }
        if (condiction.Index < 0 || condiction.Index >= cate.Count) return "";
        return cate[condiction.Index].ToString();
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var kv in Figures)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(kv.Name);
            stringBuilder.Append($"({kv.KnowledgeInfos.Count})");
            stringBuilder.AppendLine();
            foreach (var knowledge in kv.KnowledgeInfos)
            {
                if (!knowledge.IsAvailable)
                    stringBuilder.Append("~");
                stringBuilder.Append(knowledge.Content + "\n");
                foreach (var condiction in knowledge.Conditions)
                {
                    stringBuilder.Append("\t" + GetConditionText(condiction) + "\n");
                }
            }
        }
        foreach (var kv in Relations)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(kv.Name);
            stringBuilder.Append($"({kv.KnowledgeInfos.Count})");
            stringBuilder.AppendLine();
            foreach (var knowledge in kv.KnowledgeInfos)
            {
                if (!knowledge.IsAvailable)
                    stringBuilder.Append("~");
                stringBuilder.Append(knowledge.Content + "\n");
                foreach (var condiction in knowledge.Conditions)
                {
                    stringBuilder.Append("\t" + GetConditionText(condiction) + "\n");
                }
            }
        }
        foreach (var kv in GeoQuantityRelations)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(kv.Name);
            stringBuilder.Append($"({kv.KnowledgeInfos.Count})");
            stringBuilder.AppendLine();
            foreach (var knowledge in kv.KnowledgeInfos)
            {
                if (!knowledge.IsAvailable)
                    stringBuilder.Append("~");
                stringBuilder.Append(knowledge.Content + "\n");
                foreach (var condiction in knowledge.Conditions)
                {
                    stringBuilder.Append("\t" + GetConditionText(condiction) + "\n");
                }
            }
        }
        foreach (var kv in GeoQuantityValues)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(kv.Name);
            stringBuilder.Append($"({kv.KnowledgeInfos.Count})");
            stringBuilder.AppendLine();
            foreach (var knowledge in kv.KnowledgeInfos)
            {
                if (!knowledge.IsAvailable)
                    stringBuilder.Append("~");
                stringBuilder.Append(knowledge.Content + "\n");
                foreach (var condiction in knowledge.Conditions)
                {
                    stringBuilder.Append("\t" + GetConditionText(condiction) + "\n");
                }
            }
        }
        foreach (var item in ConditionalKnowledgeInfos)
        {
            foreach (ConditionalKnowledgeInfo ck in item.ConditionalKnowledgeInfos)
            {
                stringBuilder.Append($"{ck.CondictionalKnowledge} \t\n{StringTool.ComposeList(ck.CondictionGroupInfos, "\t\n")}");
                stringBuilder.Append("\n\n");
            }
        }
        return stringBuilder.ToString();
    }
}
