using System.Runtime.CompilerServices;


namespace GeoInference.Definitions.Knowledges;

public abstract class Knowledge
{
    public static void InitClassIndex(IEnumerable<Type> types)
    {
        ClassIndexDict.Clear();
        ulong classIndex = 0;
        foreach (var type in types)
        {
            if (!type.IsAbstract)
            {
                ClassIndexDict.Add(type.FullName, classIndex);
                classIndex++;
            }
        }
    }
    public static Dictionary<string, ulong> ClassIndexDict { get; set; } = new();
    public uint PosIndex { get; set; }
    public ulong HashCode { get; set; } = 0;
    public int Level { get; set; } = 0;
    public List<Knowledge> Conditions { get; set; } = new();
    public string Reason { get; set; }
    public bool IsAvailable { get; set; } = true;
    public abstract void SetHashCode();
    public void AddReason([CallerMemberName] string reason = "error")
    {
        if (reason is null) return;
        if (GeoInferenceApp.IsZhOrEn)
            if (ZhEn.RuleAliases.ContainsKey(reason))
                Reason = ZhEn.RuleAliases[reason];
            else
                Reason = reason;
        else
            Reason = reason;
    }
    public void AddCondition(IEnumerable<Knowledge> conditionPreds)
    {
        foreach (var condition in conditionPreds)
        {
            if (condition.Level >= Level)
            {
                Level = condition.Level + 1;
            }
            Conditions.Add(condition);
        }
    }
    public void AddCondition(params Knowledge[] conditionPreds)
    {
        foreach (var condition in conditionPreds)
        {
            if (condition.Level >= Level)
            {
                Level = condition.Level + 1;
            }
            Conditions.Add(condition);
        }
    }
    public bool StrContains(string content)
    {
        return ToString().Contains(content);
    }
}
