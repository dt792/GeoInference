namespace ZGeoReasoning.Definitions.Deductions;

public class RulePredPair
{
    public RuleInfo RuleInfo;
    public Predicate[] Args;
    public RulePredPair(RuleInfo ruleInfo, Predicate[] args)
    {
        RuleInfo = ruleInfo;
        Args = args;
    }
    public override string ToString()
    {
        return $"{RuleInfo.Name}-{StringTool.ComposeList(Args)}";
    }
}
