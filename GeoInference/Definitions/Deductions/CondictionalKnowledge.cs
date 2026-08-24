using GeoInference.Definitions.Knowledges;

namespace GeoInference.Knowledges;

//ConditionalKnowledgePair
public class CondictionalKnowledge
{
    public Knowledge Knowledge { get; set; }
    public new Dictionary<ulong, Predicate> ConditionDict { get; set; } = [];
    public bool IsEstablish { get; set; }
    public Dictionary<ulong, Predicate> Targets { get; set; } = [];
    public void AddCondiction(params Predicate[] knowledges)
    {
        
        knowledges.Sort((a, b) => a.HashCode.CompareTo(b.HashCode));
        foreach (var knowledge in knowledges)
        {
            if (knowledge.IsAvailable)
            {
                Targets.Add(knowledge.HashCode, knowledge);
                ConditionDict.Add(knowledge.HashCode, null);
            }
           
        }
    }
    public bool Update(Predicate knowledge)
    {
        if (Targets[knowledge.HashCode].Expr is not null)
        {
            if (Targets[knowledge.HashCode].Expr == knowledge.Expr)
            {
                ConditionDict[knowledge.HashCode] = knowledge;
            }
            else
            {
                return false;
            }
        }
        else
        {
            ConditionDict[knowledge.HashCode] = knowledge;
        }
        Check();
        return IsEstablish;
    }
    public void Check()
    {
        if (ConditionDict.ToList().TrueForAll(kv => kv.Value is not null))
        {
            IsEstablish = true;
            foreach (var kv in ConditionDict)
            {
                Knowledge.AddCondition(kv.Value);
            }
        }
    }
    public override string ToString()
    {
        return $"Target：{Knowledge}，Condictions：{StringTool.ComposeList(Targets.Values, ",")},{Knowledge.Reason}";
    }
}
