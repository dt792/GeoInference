using System.Reflection;

namespace ZGeoReasoning.Definitions.Deductions;

public class SemiConditionRuleAttribute : Attribute
{
}

public class RuleInfo
{
    public string Name { get; init; }
    public string Description { get; init; }
    public MethodInfo RuleMethod { get; set; }
    public Type DefineType { get; init; }
    public RuleClass RuleClass { get; set; }
    public List<Type> LeftTypes { get; init; } = [];
    public List<int> LeftIndexs { get; init; } = [];
    public bool IsSemiRule { get; set; }
    public RuleInfo(MethodInfo ruleMethod)
    {
        RuleMethod = ruleMethod;
        Name = ruleMethod.Name;
        DefineType = RuleMethod.DeclaringType;
        LeftTypes = RuleMethod.GetParameters().Select(p => p.ParameterType).ToList();
        LeftTypes.ForEach(s => LeftIndexs.Add(0));
        IsSemiRule = ruleMethod.GetCustomAttribute<SemiConditionRuleAttribute>() != null;
    }
    public override string ToString()
    {
        return Name;
    }
}