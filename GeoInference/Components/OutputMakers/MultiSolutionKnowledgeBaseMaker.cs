/// <summary>
/// </summary>
[Alias("多解知识库生成器")]
public class MultiSolutionKnowledgeBaseMaker : IOutputMaker
{
    [DI] KnowledgeBase knowledgeBase;
    [DI] KnowledgeBaseUpdater updater;
    [DI] TargetBase targetBase;

    static string GetTypeName(Type type)
    {
        var alias = type.GetAlias();
        if (alias.Length == 0)
            return type.Name;
        return alias[0];
    }

    static string KnowledgeToString(Knowledge knowledge)
    {
        if (knowledge is QuantityValue qv)
            return $"{qv.Quantity}={qv.Expr}";
        return knowledge.ToString();
    }

    static string GetConditionTypeName(Knowledge knowledge)
    {
        if (knowledge is Equation)
            return "Equations";
        return GetTypeName(knowledge.GetType());
    }

    static InferencePathInfo MakePath(string reason, List<Knowledge> conditions)
    {
        var path = new InferencePathInfo { Reason = reason };
        foreach (var condition in conditions)
        {
            path.Conditions.Add(new ConditionInfo
            {
                Type = GetConditionTypeName(condition),
                Index = (int)condition.PosIndex
            });
        }
        return path;
    }

    static bool SamePath(InferencePathInfo path, string reason, List<Knowledge> conditions)
    {
        if (path.Reason != reason || path.Conditions.Count != conditions.Count)
            return false;
        for (int i = 0; i < conditions.Count; i++)
        {
            if (path.Conditions[i].Type != GetConditionTypeName(conditions[i]) ||
                path.Conditions[i].Index != (int)conditions[i].PosIndex)
                return false;
        }
        return true;
    }

    public object Make()
    {
        MultiSolutionKnowledgeBase output = new();
        foreach (var target in targetBase.Targets)
        {
            if (!target.IsSuccess || target.Conclusion is null)
                continue;
            var reference = new ConditionInfo
            {
                Type = GetConditionTypeName(target.Conclusion),
                Index = (int)target.Conclusion.PosIndex
            };
            if (!output.Targets.Any(t => t.Type == reference.Type && t.Index == reference.Index))
                output.Targets.Add(reference);
        }

        return output;
    }
}
