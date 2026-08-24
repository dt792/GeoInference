
using System.Reflection;

public class InferenceMetricsMaker : IOutputMaker
{
    [DI] private EquationSolver eqSolver = null!;
    [DI] private RuleSolver ruleSolver = null!;
    [DI] private KnowledgeBase knowledgeBase = null!;
    [DI] private ZScriptBuilder zscriptBuilder = null;
    private InferenceMetrics output = new();
    public object Make()
    {
        output = new();
        CollectTimeMetrics();
        CollectKnowledgeMetrics();
        CollectRuleMetrics();
        CollectSemiMetrics();
        CollectEqualityChainMetrics();
        return output;
    }
    private void CollectTimeMetrics()
    {
        var loggedDurations = ZLog.GetTimeSpans();
        output.TotalDuration = loggedDurations.GetValueOrDefault("Inference").TotalSeconds;
        output.RuleDuration = loggedDurations.GetValueOrDefault("Rule").TotalSeconds;
        output.SemiRuleDuration = loggedDurations.GetValueOrDefault("SemiRule").TotalSeconds;
        output.EqSymDuration = loggedDurations.GetValueOrDefault("EqSym").TotalSeconds;

        output.MatrixDuration = loggedDurations.GetValueOrDefault("Matrix").TotalSeconds;

        output.SemiRule_Check = loggedDurations.GetValueOrDefault("SemiRule_Check").TotalSeconds;
    }
    void CollectKnowledgeMetrics()
    {
        {
            var mergedCount = (ulong)knowledgeBase.Categories[typeof(Angle)].Count;
            output.MergedPredicateDistribution.Add(GetFriendlyTypeName(typeof(Angle)), mergedCount);
            foreach (Angle pred in knowledgeBase.Categories[typeof(Angle)])
            {
                var count = CalClassicAngle(pred);
                AccumulateClassicCount(typeof(Angle), count);
                output.ClassicFigureCount += count;
                output.MergedFigureCount += 1;
            }
            mergedCount = (ulong)knowledgeBase.Categories[typeof(Line)].Count;
            output.MergedPredicateDistribution.Add(GetFriendlyTypeName(typeof(Line)), mergedCount);
            foreach (Line pred in knowledgeBase.Categories[typeof(Line)])
            {
                var count = CalClassicLine(pred);
                AccumulateClassicCount(typeof(Line), count);
                output.ClassicFigureCount += count;
                output.MergedFigureCount += 1;
            }
            mergedCount = (ulong)knowledgeBase.Categories[typeof(Circle)].Count;
            output.MergedPredicateDistribution.Add(GetFriendlyTypeName(typeof(Circle)), mergedCount);
            foreach (Circle pred in knowledgeBase.Categories[typeof(Circle)])
            {
                var count = CalClassicCircle(pred);
                AccumulateClassicCount(typeof(Circle), count);
                output.ClassicFigureCount += count;
                output.MergedFigureCount += 1;
            }
        }
        {
            foreach (var category in knowledgeBase.Categories)
            {
                var predicateType = category.Key;
                var instances = category.Value;
                var mergedCount = (ulong)instances.Count;
                if (mergedCount == 0) continue;
                if (predicateType == typeof(QuantityValue) || predicateType == typeof(QuantityRatio) ||
                    predicateType == typeof(Line) || predicateType == typeof(Angle) || predicateType == typeof(Circle))
                    continue;
                if (predicateType.IsAssignableTo(typeof(Figure)))
                {
                    output.UnrelatedFigureCount += mergedCount;
                }
                else
                {
                    var ctor = predicateType.GetConstructors()[0];
                    var composeType = ctor.GetParameters();
                    bool isMerged = composeType.Any(p => p.ParameterType == typeof(Line) || p.ParameterType == typeof(Angle) || p.ParameterType == typeof(Circle));
                    if (isMerged)
                    {
                        output.MergedPredicateDistribution.Add(GetFriendlyTypeName(predicateType), mergedCount);
                        ulong i = 1;
                        foreach (Predicate pred in instances)
                        {
                            i = 1;
                            foreach (Predicate pred2 in pred.Properties)
                            {
                                if (pred2 is Angle angle)
                                    i *= CalClassicAngle(angle);
                                else if (pred2 is Line line)
                                    i *= CalClassicLine(line);
                                else if (pred2 is Circle circle)
                                    i *= CalClassicCircle(circle);
                            }
                            AccumulateClassicCount(predicateType, i);
                            output.ClassicRelationCount += i;
                            output.MergedRelationCount += 1;
                        }
                    }
                    else
                    {
                        output.UnrelatedPredicateDistribution.Add(GetFriendlyTypeName(predicateType), mergedCount);
                        output.UnrelatedRelationCount += mergedCount;
                    }
                }
            }
        }
        {
            foreach (QuantityValue item in knowledgeBase.QuantityValues.Values)
            {
                if (item.Quantity is GeoQuantity g)
                {
                    ulong count = 1;
                    foreach (var figure in g.Figures)
                    {
                        if (figure is Angle angle)
                            count *= CalClassicAngle(angle);
                        else if (figure is Line line)
                            count *= CalClassicLine(line);
                        else if (figure is Circle circle)
                            count *= CalClassicCircle(circle);
                    }
                    if (count != 1)
                    {
                        output.ClassicQvCount += count;
                        output.MergedQvCount += 1;
                    }
                    else
                        output.UnrelatedQvCount += 1;
                }
            }
            foreach (QuantityRatio item in knowledgeBase.QuantityRatios.Values)
            {
                ulong count = 1;
                if (item.Quantity1 is GeoQuantity g1)
                {
                    foreach (var figure in g1.Figures)
                    {
                        if (figure is Angle angle)
                            count *= CalClassicAngle(angle);
                        else if (figure is Line line)
                            count *= CalClassicLine(line);
                        else if (figure is Circle circle)
                            count *= CalClassicCircle(circle);
                    }
                }
                if (item.Quantity2 is GeoQuantity g2)
                {
                    foreach (var figure in g2.Figures)
                    {
                        if (figure is Angle angle)
                            count *= CalClassicAngle(angle);
                        else if (figure is Line line)
                            count *= CalClassicLine(line);
                        else if (figure is Circle circle)
                            count *= CalClassicCircle(circle);
                    }
                }

                if (count != 1)
                {
                    output.ClassicQrCount += count;
                    output.MergedQrCount += 1;
                }
                else
                    output.UnrelatedQrCount += 1;
            }
        }
        {
            foreach (Equation item in knowledgeBase.Equations.Values)
            {
                if (item is LinearEquation linear)
                {
                    ulong count = 1;
                    foreach (var quantity in linear.Coff.Keys.Where(q => q is GeoQuantity).Select(q => (GeoQuantity)q))
                    {
                        foreach (var figure in quantity.Figures)
                        {
                            if (figure is Angle angle)
                                count *= CalClassicAngle(angle);
                            else if (figure is Line line)
                                count *= CalClassicLine(line);
                            else if (figure is Circle circle)
                                count *= CalClassicCircle(circle);
                        }
                    }

                    if (count != 1)
                    {
                        output.ClassicLinearEquationCount += count;
                        output.MergedLinearEquationCount += 1;
                    }
                    else
                        output.UnrelatedLinearEquationCount += 1;
                }
                else if (item is ProductionEquation production)
                {
                    ulong count = 1;
                    foreach (var quantity in production.Coff.Keys.Where(q => q is GeoQuantity).Select(q => (GeoQuantity)q))
                    {
                        foreach (var figure in quantity.Figures)
                        {
                            if (figure is Angle angle)
                                count *= CalClassicAngle(angle);
                            else if (figure is Line line)
                                count *= CalClassicLine(line);
                            else if (figure is Circle circle)
                                count *= CalClassicCircle(circle);
                        }
                    }
                    if (count != 1)
                    {
                        output.ClassicProductionEquationCount += count;
                        output.MergedProductionEquationCount += 1;
                    }
                    else
                        output.UnrelatedProductionEquationCount += 1;
                }
                else
                {
                    var quantities = zscriptBuilder.GetEquationQuantities(item.ToString());
                    ulong count = 1;
                    foreach (var quantity in quantities)
                    {
                        if (quantity is GeoQuantity g)
                        {
                            foreach (var figure in g.Figures)
                            {
                                if (figure is Angle angle)
                                    count *= CalClassicAngle(angle);
                                else if (figure is Line line)
                                    count *= CalClassicLine(line);
                                else if (figure is Circle circle)
                                    count *= CalClassicCircle(circle);
                            }
                        }
                    }
                    if (count != 1)
                    {
                        output.ClassicResidualEquationCount += count;
                        output.MergedResidualEquationCount += 1;
                    }
                    else
                        output.UnrelatedResidualEquationCount += 1;
                }
            }
        }
        foreach (var kvp in classicPredicateCounts)
            output.ClassicPredicateDistribution.Add(GetFriendlyTypeName(kvp.Key), kvp.Value);
    }
    Dictionary<Type, ulong> classicPredicateCounts = new Dictionary<Type, ulong>();
    void CollectRuleMetrics()
    {
        foreach (var ruleInfo in ruleSolver.RuleInfos)
        {
            var isSemiConditional = ruleInfo.RuleMethod.GetCustomAttribute<SemiConditionRuleAttribute>() is not null;
            if (isSemiConditional) continue;
            var ruleSignature = ruleInfo.ToString();
            if (ruleInfo.LeftTypes.Exists(classicPredicateCounts.Keys.Contains))
            {
                ulong currentMergedCount = 1;
                ulong currentClassicCount = 1;
                bool canProcess = true;
                foreach (var leftType in ruleInfo.LeftTypes)
                {
                    if (!knowledgeBase.Categories.TryGetValue(leftType, out var category))
                    {
                        canProcess = false;
                        break;
                    }
                    var categoryCount = (ulong)category.Count;
                    currentMergedCount *= categoryCount;
                    if (classicPredicateCounts.ContainsKey(leftType))
                        currentClassicCount *= classicPredicateCounts.GetValueOrDefault(leftType, categoryCount);
                    else
                        currentClassicCount *= categoryCount;
                }
                if (canProcess)
                {
                    output.ClassicRulePairDistribution.Add(ruleSignature, currentClassicCount);
                    output.MergeRulePairDistribution.Add(ruleSignature, currentMergedCount);
                }
            }
            else
            {
                bool canProcess = true;
                ulong currentCount = 1;
                foreach (var leftType in ruleInfo.LeftTypes)
                {
                    if (!knowledgeBase.Categories.TryGetValue(leftType, out var category))
                    {
                        canProcess = false;
                        break;
                    }
                    var categoryCount = (ulong)category.Count;
                    currentCount *= categoryCount;
                }
                if (canProcess)
                    output.UnrelatedRulePairDistribution.Add(ruleSignature, currentCount);
            }
        }
        output.ClassicRulePairCount = SumDictionaryValues(output.ClassicRulePairDistribution);
        output.MergedRulePairCount = SumDictionaryValues(output.MergeRulePairDistribution);
        output.UnrelatedRulePairCount = SumDictionaryValues(output.UnrelatedRulePairDistribution);
    }
    void CollectSemiMetrics()
    {
        ulong TryCalculateProductCount(IReadOnlyList<Type> types)
        {
            ulong product = 1;
            foreach (var type in types)
            {
                if (!knowledgeBase.Categories.TryGetValue(type, out var category)) return 0;
                product *= classicPredicateCounts.GetValueOrDefault(type, (ulong)category.Count);
            }
            return product;
        }

        var triangleRuleConfigs = new[]
        {
            (Name: nameof(RuleCongTri.RuleCT001TriangleCongruenceSSS),
             OriginalTypes: new List<Type> { typeof(Triangle), typeof(Triangle), typeof(SegmentLengthEqual), typeof(SegmentLengthEqual), typeof(SegmentLengthEqual) },
             OptimizedTypes: new List<Type> { typeof(Triangle), typeof(Triangle) }),

            (Name: nameof(RuleCongTri.RuleCT002TriangleCongruenceSAS),
             OriginalTypes: new List<Type> { typeof(Triangle), typeof(Triangle), typeof(SegmentLengthEqual), typeof(AngleSizeEqual), typeof(SegmentLengthEqual) },
             OptimizedTypes: new List<Type> { typeof(Triangle), typeof(Triangle) }),

            (Name: nameof(RuleCongTri.RuleCT003TriangleCongruenceASA),
             OriginalTypes: new List<Type> { typeof(Triangle), typeof(Triangle), typeof(AngleSizeEqual), typeof(SegmentLengthEqual), typeof(AngleSizeEqual) },
             OptimizedTypes: new List<Type> { typeof(Triangle), typeof(Triangle) }),

            (Name: nameof(RuleCongTri.RuleCT004TriangleCongruenceAAS),
             OriginalTypes: new List<Type> { typeof(Triangle), typeof(Triangle), typeof(AngleSizeEqual), typeof(AngleSizeEqual), typeof(SegmentLengthEqual) },
             OptimizedTypes: new List<Type> { typeof(Triangle), typeof(Triangle) }),

            (Name: nameof(RuleCongTri.RuleCT005TriangleCongruenceHL),
             OriginalTypes: new List<Type> { typeof(RightTriangle), typeof(RightTriangle), typeof(SegmentLengthEqual) },
             OptimizedTypes: new List<Type> { typeof(RightTriangle), typeof(RightTriangle) }),

            (Name: nameof(RuleCongTri.RuleCT006TriangleSimilarityAA),
             OriginalTypes: new List<Type> { typeof(Triangle), typeof(Triangle), typeof(AngleSizeEqual), typeof(AngleSizeEqual) },
             OptimizedTypes: new List<Type> { typeof(Triangle), typeof(Triangle) }),

            (Name: nameof(RuleCongTri.RuleCT007TriangleSimilaritySAS),
             OriginalTypes: new List<Type> { typeof(Triangle), typeof(Triangle), typeof(SegmentLengthEqual), typeof(AngleSizeEqual), typeof(SegmentLengthEqual) },
             OptimizedTypes: new List<Type> { typeof(Triangle), typeof(Triangle), typeof(SegmentLengthRatio) }),

            (Name: nameof(RuleCongTri.RuleCT008TriangleSimilaritySSS),
             OriginalTypes: new List<Type> { typeof(Triangle), typeof(Triangle), typeof(SegmentLengthRatio), typeof(SegmentLengthRatio), typeof(SegmentLengthRatio) },
             OptimizedTypes: new List<Type> { typeof(Triangle), typeof(Triangle), typeof(SegmentLengthRatio) }),

             (Name: nameof(DistanceQuantityRule.RuleDQ006AngleBisectorEquidistantToSides),
             OriginalTypes: new List<Type> { typeof(AngularBisectorLine), typeof(LinePerpendicular), typeof(LinePerpendicular) },
             OptimizedTypes: new List<Type> { typeof(AngularBisectorLine) }),
             (Name: nameof(QuadrilateralRules.RuleDQ02QuadrilateralWithOnePairOfParallelAndEqualOppositeSidesIsParallelogram),
             OriginalTypes: new List<Type> { typeof(Quadriliateral), typeof(LineParallel), typeof(SegmentLengthEqual) },
             OptimizedTypes: new List<Type> { typeof(Quadriliateral) }),
             (Name: nameof(QuadrilateralRules.RuleDQ03QuadrilateralWithTwoPairsOfParallelOppositeSidesIsParallelogram),
             OriginalTypes: new List<Type> { typeof(Quadriliateral), typeof(SegmentLengthEqual), typeof(SegmentLengthEqual) },
             OptimizedTypes: new List<Type> { typeof(Quadriliateral) }),
             (Name: nameof(QuadrilateralRules.RuleDQ04QuadrilateralWithTwoPairsOfEqualOppositeSidesIsParallelogram),
             OriginalTypes: new List<Type> { typeof(Quadriliateral), typeof(AngleSizeEqual), typeof(AngleSizeEqual) },
             OptimizedTypes: new List<Type> { typeof(Quadriliateral) })
        };

        foreach (var config in triangleRuleConfigs)
        {
            var originalPairCount = TryCalculateProductCount(config.OriginalTypes);
            var optimizedPairCount = TryCalculateProductCount(config.OptimizedTypes);

            if (originalPairCount > 0 && optimizedPairCount > 0)
            {
                output.ClassicSemiRulePairDistribution.Add(config.Name, originalPairCount);
                output.SemiRulePairDistribution.Add(config.Name, optimizedPairCount);
            }
        }
        output.ClassicSemiRulePairCount = SumDictionaryValues(output.ClassicSemiRulePairDistribution);
        output.SemiRulePairCount = SumDictionaryValues(output.SemiRulePairDistribution);
    }
    void CollectEqualityChainMetrics()
    {
        output.MatrixSparsities.Add(eqSolver.DistanceLinearMatrix.CalculateSparsity());
        output.MatrixSparsities.Add(eqSolver.AngularLinearMatrix.CalculateSparsity());
        output.MatrixSparsities.Add(eqSolver.DistanceProductionMatrix.CalculateSparsity());

        output.QuantityCount = knowledgeBase.Quantities.Count;
        HashSet<Quantity> quantityInECs = [];
        foreach (var kv in knowledgeBase.EqualityChains)
        {
            foreach (var kv2 in kv.Value)
            {
                foreach (var quantity in kv2.CoffDict.Keys)
                {
                    quantityInECs.Add(quantity);
                }
            }
        }
        output.InEqualityChainQuantityCount = quantityInECs.Count;
    }

    private string GetFriendlyTypeName(Type type)
    {
        if (GeoInferenceApp.IsZhOrEn)
        {
            var aliases = type.GetAlias();
            string typeName = aliases.Any() ? aliases[0] : type.Name;
            if (typeName.Contains("<T>"))
            {
                var firstCtorParamType = type.GetConstructors()[0].GetParameters().First().ParameterType;
                var genericArgAlias = ZAlias.GetAlias(firstCtorParamType)[0];
                typeName = typeName.Replace("<T>", genericArgAlias);
            }
            return typeName;
        }
        else
        {
            if (type.Name.Contains("`1"))
            {
                var firstCtorParamType = type.GetConstructors()[0].GetParameters().First().ParameterType;
                var genericArgAlias = firstCtorParamType.Name;
                var typeName = type.Name.Replace("`1", genericArgAlias);
                return typeName;
            }
            else
                return type.Name;

        }


    }
    void AccumulateClassicCount(Type predicateType, ulong count)
    {
        classicPredicateCounts.TryGetValue(predicateType, out var current);
        classicPredicateCounts[predicateType] = current + count;

    }
    static IEnumerable<MethodInfo> findRules()
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
        return Rules.Where(m => m.Name.StartsWith("Rule"));
    }
    public static Dictionary<string, (int paramsCount, int semiCount)> Get()
    {
        var result = new Dictionary<string, (int paramsCount, int semiCount)>();
        var data = findRules();
        foreach (var rule in data)
        {
            if (rule.GetCustomAttribute<SemiConditionRuleAttribute>() is null)
            {
                result.Add(rule.Name, (rule.GetParameters().Count(), rule.GetParameters().Count()));
            }
        }
        {
            result.Add(nameof(RuleCongTri.RuleCT001TriangleCongruenceSSS), (5, 2));
            result.Add(nameof(RuleCongTri.RuleCT002TriangleCongruenceSAS), (5, 2));
            result.Add(nameof(RuleCongTri.RuleCT003TriangleCongruenceASA), (5, 2));
            result.Add(nameof(RuleCongTri.RuleCT004TriangleCongruenceAAS), (5, 2));
            result.Add(nameof(RuleCongTri.RuleCT005TriangleCongruenceHL), (3, 2));
            result.Add(nameof(RuleCongTri.RuleCT006TriangleSimilarityAA), (4, 2));
            result.Add(nameof(RuleCongTri.RuleCT007TriangleSimilaritySAS), (5, 3));
            result.Add(nameof(RuleCongTri.RuleCT008TriangleSimilaritySSS), (5, 3));

            result.Add(nameof(DistanceQuantityRule.RuleDQ006AngleBisectorEquidistantToSides), (3, 1));
            result.Add(nameof(QuadrilateralRules.RuleDQ02QuadrilateralWithOnePairOfParallelAndEqualOppositeSidesIsParallelogram), (3, 1));
            result.Add(nameof(QuadrilateralRules.RuleDQ03QuadrilateralWithTwoPairsOfParallelOppositeSidesIsParallelogram), (3, 1));
            result.Add(nameof(QuadrilateralRules.RuleDQ04QuadrilateralWithTwoPairsOfEqualOppositeSidesIsParallelogram), (3, 1));
        }
        return result;
    }
    ulong SumDictionaryValues(Dictionary<string, ulong> dict)
    {
        ulong tmp = 0;
        foreach (var value in dict.Values)
            tmp += value;
        return tmp;
    }
    ulong CalClassicAngle(Angle angle)
    {
        return (ulong)angle.Edge1.Count * (ulong)angle.Edge2.Count;
    }
    ulong CalClassicLine(Line line)
    {
        var expandedCount = CalculateCombinations((ulong)line.Properties.Count, 2);
        return expandedCount;
    }
    ulong CalClassicCircle(Circle circle)
    {
        var expandedCount = CalculateCombinations((ulong)circle.Properties.Count - 1, 3);
        if (expandedCount == 0)
            expandedCount = 1;
        return expandedCount;
    }
    private static ulong CalculateCombinations(ulong n, ulong k)
    {
        if (k > n) return 0;
        if (k == 0 || k == n) return 1;
        if (k > n / 2) k = n - k; 

        ulong result = 1;
        for (ulong i = 1; i <= k; i++)
        {
            
            result = result * (n - i + 1) / i;
        }
        return result;
    }
}
