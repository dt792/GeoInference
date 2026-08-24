
using System.Reflection;
internal class GeoFigureMerger
{
    [DI] RuleSolver ProductionRuleSolver { get; set; }
    public void Merge()
    {
        ZLog.LogKeyPoint("Implicit Knowledge Reasoning and Preliminary Knowledge Merging");
        Run([RuleType.BaseFigureSearching, RuleType.Internal]);
        Run([RuleType.AutoGeneration]);
    }
    void Run(List<RuleType> ruleTypes)
    {
        ProductionRuleSolver.LoadRules(findRules(ruleTypes));
        ProductionRuleSolver.MakeNew();
        do
        {
            if (ProductionRuleSolver.HasNext)
            {
                var pair = ProductionRuleSolver.Next();
                ProductionRuleSolver.Solve(pair);
            }
        }
        while (ProductionRuleSolver.HasNext);
    }
    IEnumerable<MethodInfo> findRules(List<RuleType> ruleTypes)
    {
        List<MethodInfo> Rules = [];
        foreach (var type in typeof(RuleClass).Assembly.GetTypes())
        {
            if (type.IsSubclassOf(typeof(RuleClass)) && !type.IsAbstract)
            {
                var t = type.GetCustomAttribute<RuleTypeAttribute>();
                foreach (var ruleType in ruleTypes)
                {
                    if (t is not null && t.RuleType == ruleType)
                    {
                        foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                        {
                            Rules.Add(methodInfo);
                        }
                    }
                }
            }
        }
        return Rules;
    }
}
