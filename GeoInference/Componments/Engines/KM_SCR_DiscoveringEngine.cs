
using System.Reflection;

public class KM_SCR_DiscoveringEngine : IInferenceEngine
{
    [DI] ZScriptInput Input { get; set; }
    [DI] TargetBase TargetBase { get; set; }
    [DI] KnowledgeBase KnowledgeBase;
    [DI] ZScriptBuilder ZScriptBuilder;
    [DI] KnowledgeBaseUpdater Updater { get; set; }
    [DI] EquationSolver eqSolver { get; set; }
    [DI] GeoFigureMerger Merger { get; set; }
    [DI] SolvingStopJudge Juder { get; set; }
    [DI] ConditionalKnowledgeSolver ConditionalKnowledgeSolver { get; set; }
    [DI] RuleSolver ProductionRuleSolver { get; set; }

    Dictionary<Type, List<Predicate>> InitCates = new(){ { typeof(Point),[] },
    { typeof(Segment),[] },
    { typeof(Line),[] },
    { typeof(Angle),[] },
    { typeof(Circle),[] },
    { typeof(Triangle),[] } };
    public override void Init()
    {
        Updater.PredicateAdded += eqSolver.Update;
        Updater.EquationAdded += eqSolver.Update;

        ZhEn.KnowledgeAliases = ZhEn.ExtractKnowledgeAliases(typeof(ZhEn).Assembly);
        ZhEn.RuleAliases = ZhEn.ExtractRuleAliases(typeof(ZhEn).Assembly);
        ZExprParser.ParseQuantity = ZScriptBuilder.ParseQuantity;
        Quantity.Parse = s => ZScriptBuilder.ParseQuantity(s);
        Knowledge.InitClassIndex(FindPredicateTypes().Union(
            [typeof(QuantityValue), typeof(QuantityRatio), typeof(LinearEquation), typeof(ProductionEquation), typeof(Equation)]));
        KnowledgeBase.Categories = InitCates;
        ZScriptBuilder.LoadPredicates(FindPredicateTypes());

        ZScriptBuilder.Build(Input.Script);

        foreach (var item in KnowledgeBase.Predicates)
        {
            if (GeoInferenceApp.IsZhOrEn)
                item.Value.AddReason("已知");
            else
                item.Value.AddReason("Given");
        }
        foreach (var item in KnowledgeBase.Equations)
        {
            if (GeoInferenceApp.IsZhOrEn)
                item.Value.AddReason("已知");
            else
                item.Value.AddReason("Given");
        }
    }
    int round = 1;
    public override void Run()
    {

        Merger.Merge();

        ZLog.LogKeyPoint("Starting main process reasoning.");
        Starting?.Invoke();
        Merger.Merge();
        ProductionRuleSolver.LoadRules(findRules());
        ZLog.LogKeyPoint("Rule set loaded.");
        do
        {
            ZLog.Info($"Starting round {round} of reasoning");
            KnowledgeBase.LastRoundKnowledges.Clear();
            KnowledgeBase.LastRoundKnowledges.AddRange(KnowledgeBase.NewKnowledges);
            KnowledgeBase.NewKnowledges.Clear();
            ZLog.StartStopwatch("Rule");
            ProductionRuleSolver.MakeNew();
            ZLog.StopStopwatch("Rule");
            while (ProductionRuleSolver.HasNext && !TargetBase.AllSolved)
            {
                var pair = ProductionRuleSolver.Next();
                if (pair.RuleInfo.IsSemiRule)
                {
                    ZLog.StartStopwatch("SemiRule");
                    ProductionRuleSolver.Solve(pair);
                    ZLog.StopStopwatch("SemiRule");
                }
                else
                {
                    ZLog.StartStopwatch("Rule");
                    ProductionRuleSolver.Solve(pair);
                    ZLog.StopStopwatch("Rule");
                }
            }


            ZLog.StartStopwatch("EqSym");
            eqSolver.SolveEqSym();
            ZLog.StopStopwatch("EqSym");

            ZLog.StartStopwatch("Matrix");
            eqSolver.SolveMatrix();
            ZLog.StopStopwatch("Matrix");

            ZLog.StartStopwatch("SemiRule");
            ZLog.StartStopwatch("SemiRule_Check");
            ConditionalKnowledgeSolver.MakeNew();
            while (ConditionalKnowledgeSolver.HasNext)
            {
                var obj = ConditionalKnowledgeSolver.Next();
                ConditionalKnowledgeSolver.Solve(obj);
            }
            ZLog.StopStopwatch("SemiRule_Check");
            ZLog.StopStopwatch("SemiRule");

            round++;
            ZLog.Info($"Round {round} of reasoning yielded a total of {KnowledgeBase.NewKnowledges}");
        }
        while (!TargetBase.AllSolved && KnowledgeBase.NewKnowledges.Count > 0);
        Finished?.Invoke();
        ZLog.LogKeyPoint($"Inference complete, {round} rounds of reasoning in total.");
    }

    public static List<Type> FindPredicateTypes()
    {
        List<Type> result = [];
        foreach (var type in typeof(KM_SCR_DiscoveringEngine).Assembly.GetTypes())
        {
            if (type.IsSubclassOf(typeof(Predicate)))
            {
                result.Add(type);
            }
        }
        return result;
    }

    IEnumerable<MethodInfo> findRules()
    {
        List<MethodInfo> Rules = [];
        List<RuleType> ruleTypes = [RuleType.Tradition, RuleType.Internal];
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
