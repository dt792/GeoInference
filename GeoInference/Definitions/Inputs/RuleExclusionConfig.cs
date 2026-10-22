using System.Reflection;

/// <summary>
/// </summary>
public class RuleExclusionConfig : IInferenceConfig
{
    /// <summary>
    /// </summary>
    public List<string> ExcludedRules { get; set; } = [];

    public bool IsExcluded(MethodInfo rule)
        => IsExcludedName(rule.Name) || IsExcludedName(rule.DeclaringType?.Name);

    /// <summary>
    /// </summary>
    public bool IsExcludedName(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        foreach (var excluded in ExcludedRules)
        {
            if (excluded == name) return true;
            if (ZhEn.RuleAliases.TryGetValue(excluded, out var alias) && alias == name) return true;
            if (ZhEn.RuleAliases.TryGetValue(name, out var nameAlias) && nameAlias == excluded) return true;
        }
        return false;
    }
}
