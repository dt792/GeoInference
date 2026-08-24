[Alias("知识图谱生成器")]
public class KnowledgeGraphMaker : IOutputMaker
{
    [DI] KnowledgeBase knowledgeBase { get; set; }

    KnowledgeGraph output;

    string GetTypeName(Type type)
    {
        var alias = type.GetAlias();
        if (alias.Length == 0)
            return type.Name;
        return alias[0];
    }

    string KnowledgeToString(Knowledge knowledge)
    {
        if (knowledge is QuantityValue qv)
            return $"{qv.Quantity}={qv.Expr}";
        return knowledge.ToString();
    }

    public (string, List<KnowledgeInfo>) MakeCateInfos(Type type, List<Predicate> knowledges)
    {
        var name = GetTypeName(type);
        var list = new List<KnowledgeInfo>();
        for (int i = 0; i < knowledges.Count; i++)
        {
            var knowledge = knowledges[i];
            KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
            knowledgeInfo.Content = KnowledgeToString(knowledge);
            knowledgeInfo.Reason = knowledge.Reason;
            knowledgeInfo.IsAvailable = knowledge.IsAvailable;
            MakeConditionInfo(knowledge, knowledgeInfo);
            list.Add(knowledgeInfo);
        }
        return (name, list);
    }

    public void MakeConditionInfo(Knowledge knowledge, KnowledgeInfo knowledgeInfo)
    {
        for (int j = 0; j < knowledge.Conditions.Count; j++)
        {
            var condition = knowledge.Conditions[j];
            ConditionInfo condictionInfo = new ConditionInfo();
            string conditionTypeName;
            if (condition is Equation)
            {
                conditionTypeName = "Equations";
            }
            else
            {
                conditionTypeName = GetTypeName(condition.GetType());
            }
            condictionInfo.Type = conditionTypeName;
            condictionInfo.Index = (int)condition.PosIndex;
            knowledgeInfo.Conditions.Add(condictionInfo);
        }
    }
    [DI] EquationSolver equationSolver;
    public object Make()
    {
        output = new KnowledgeGraph();
        lock (knowledgeBase)
        {
            //var a = equationSolver.TooComplexEqsyms.Union(equationSolver.ReadyToTryEqSyms).Union(equationSolver.TriedEqSyms).First(e => e.PosIndex == 0);
            #region Predicates (Categories)
            foreach (var cate in knowledgeBase.Categories)
            {
                if (cate.Value.Count == 0) { continue; }
                var type = cate.Key;
                if (type.IsAssignableTo(typeof(Figure)))
                {
                    var result = MakeCateInfos(cate.Key, cate.Value);
                    output.Figures.Add(new() { Name = result.Item1, KnowledgeInfos = result.Item2 });
                }
                else if (type.IsAssignableTo(typeof(Predicate)))
                {
                    var result = MakeCateInfos(cate.Key, cate.Value);
                    output.Relations.Add(new() { Name = result.Item1, KnowledgeInfos = result.Item2 });
                }
            }
            #endregion

            #region QuantityValues
            Dictionary<string, List<KnowledgeInfo>> qvGroups = [];
            foreach (var qv in knowledgeBase.QuantityValues.Values)
            {
                KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                knowledgeInfo.Content = KnowledgeToString(qv);
                knowledgeInfo.Reason = qv.Reason;
                knowledgeInfo.IsAvailable = qv.IsAvailable;
                MakeConditionInfo(qv, knowledgeInfo);
                AddToGroup(qvGroups, GetQuantityValueCateName(qv), knowledgeInfo);
            }
            foreach (var kv in qvGroups)
            {
                output.GeoQuantityValues.Add(new()
                {
                    Name = kv.Key,
                    KnowledgeInfos = kv.Value
                });
            }
            #endregion

            #region QuantityRatios
            var quantityRatios = new List<KnowledgeInfo>();
            foreach (var qr in knowledgeBase.QuantityRatios.Values)
            {
                KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                knowledgeInfo.Content = KnowledgeToString(qr);
                knowledgeInfo.Reason = qr.Reason;
                knowledgeInfo.IsAvailable = qr.IsAvailable;
                MakeConditionInfo(qr, knowledgeInfo);
                quantityRatios.Add(knowledgeInfo);
            }
            if (quantityRatios.Count > 0)
            {
                output.GeoQuantityRelations.Add(new()
                {
                    Name = GetTypeName(typeof(QuantityRatio)),
                    KnowledgeInfos = quantityRatios
                });
            }
            #endregion

            #region Equations
            HashSet<ulong> equationHashCodes = [];
            Dictionary<string, List<KnowledgeInfo>> eqGroups = [];
            foreach (var equation in knowledgeBase.Equations.Values)
            {
                if (!equationHashCodes.Add(equation.HashCode)) continue;
                AddEquationInfo(equation, eqGroups);
            }
            foreach (var equation in knowledgeBase.LinearEquations.Values)
            {
                if (!equationHashCodes.Add(equation.HashCode)) continue;
                AddEquationInfo(equation, eqGroups);
            }
            foreach (var equation in knowledgeBase.ProductionEquations.Values)
            {
                if (!equationHashCodes.Add(equation.HashCode)) continue;
                AddEquationInfo(equation, eqGroups);
            }
            foreach (var equation in knowledgeBase.RemainingEquations.Values)
            {
                if (!equationHashCodes.Add(equation.HashCode)) continue;
                AddEquationInfo(equation, eqGroups);
            }
            foreach (var kv in eqGroups)
            {
                output.GeoEquationCateInfos.Add(new()
                {
                    Name = kv.Key,
                    KnowledgeInfos = kv.Value
                });
            }
            #endregion

            #region ConditionalKnowledgePairs
            Dictionary<ulong, ConditionalKnowledgeInfo> dict = [];
            foreach (var ck in knowledgeBase.ConditionalKnowledgePairs)
            {
                var typeName = GetTypeName(ck.Knowledge.GetType());
                ConditionalKnowledgeCateInfo cate = null;
                if (!output.ConditionalKnowledgeInfos.Exists(x => x.Cate == typeName))
                {
                    cate = new() { Cate = typeName };
                    output.ConditionalKnowledgeInfos.Add(cate);
                }
                else
                {
                    cate = output.ConditionalKnowledgeInfos.First(x => x.Cate == typeName);
                }

                ConditionalKnowledgeInfo info;
                if (dict.ContainsKey(ck.Knowledge.HashCode))
                {
                    info = dict[ck.Knowledge.HashCode];
                }
                else
                {
                    info = new ConditionalKnowledgeInfo();
                    info.CondictionalKnowledge = KnowledgeToString(ck.Knowledge);
                    dict.Add(ck.Knowledge.HashCode, info);
                    cate.ConditionalKnowledgeInfos.Add(info);
                }

                ConditionGroupInfo conditionGroupInfo = new();
                conditionGroupInfo.Reason = ck.Knowledge.Reason;
                foreach (var condiction in ck.Targets.Values)
                {
                    conditionGroupInfo.Condictions.Add(KnowledgeToString(condiction));
                }
                info.CondictionGroupInfos.Add(conditionGroupInfo);
            }
            #endregion

            #region EqualityChains
            foreach (var kv in knowledgeBase.EqualityChains)
            {
                if (kv.Value.Count == 0) continue;
                ContinuedEqualityCateInfo cateInfo = new ContinuedEqualityCateInfo();
                cateInfo.Cate = kv.Key.ToString();
                foreach (var kv2 in kv.Value)
                {
                    ContinuedEqualityInfo info = new ContinuedEqualityInfo();
                    info.Content = kv2.ToString();
                    cateInfo.ContinuedEqualityInfos.Add(info);
                }
                output.ContinuedEqualityCateInfos.Add(cateInfo);
            }
            #endregion

            #region EqSolver
            AddEqSymCate("待求解方程组", equationSolver.ReadyToTryEqSyms);
            AddEqSymCate("过于复杂方程组", equationSolver.TooComplexEqsyms);
            AddEqSymCate("已尝试方程组", equationSolver.TriedEqSyms);
            #endregion
        }
        return output;
    }

    void AddEquationInfo(Equation equation, Dictionary<string, List<KnowledgeInfo>> eqGroups)
    {
        KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
        knowledgeInfo.Reason = equation.Reason;
        knowledgeInfo.IsAvailable = equation.IsAvailable;
        knowledgeInfo.Content = $"{equation}";
        MakeConditionInfo(equation, knowledgeInfo);
        output.GeoEquations.Add(knowledgeInfo);
        AddToGroup(eqGroups, GetEquationCateName(equation), knowledgeInfo);
    }

    void AddEqSymCate(string name, List<EquationSystem> syms)
    {
        if (syms.Count == 0) return;
        var infos = new List<KnowledgeInfo>();
        foreach (var sym in syms)
        {
            infos.Add(new KnowledgeInfo
            {
                Content = sym.ToString(),
                IsAvailable = true
            });
        }
        output.EqSymCateInfos.Add(new()
        {
            Name = name,
            KnowledgeInfos = infos
        });
    }

    static string GetQuantityValueCateName(QuantityValue qv)
    {
        if (qv.Quantity is GeoQuantity gq)
        {
            return gq.PropName switch
            {
                Quantity.Size => "角",
                Quantity.Cos => "余弦",
                Quantity.Sin => "正弦",
                Quantity.Tan => "正切",
                Quantity.Length => "长度",
                Quantity.Area => "面积",
                Quantity.Perimeter => "周长",
                Quantity.MajorArcLength => "优弧长",
                Quantity.MinorArcLength => "劣弧长",
                Quantity.Radius => "半径",
                Quantity.Diameter => "直径",
                Quantity.Distance => "距离",
                Quantity.Ratio => "比值",
                _ => gq.PropName,
            };
        }
        return "其他";
    }

    static string GetEquationCateName(Equation equation)
    {
        var row = equation switch
        {
            LinearEquation => "线性方程",
            ProductionEquation => "乘积方程",
            _ => "其他方程",
        };
        var col = equation.Type switch
        {
            GeoEquationTypes.DistanceLinear or GeoEquationTypes.DistanceProduction or GeoEquationTypes.DistanceOther => "距离",
            GeoEquationTypes.AngularLinear or GeoEquationTypes.AngularProduction or GeoEquationTypes.AngularOther => "角度",
            _ => "混合",
        };
        return $"{row}-{col}";
    }

    static void AddToGroup(Dictionary<string, List<KnowledgeInfo>> groups, string name, KnowledgeInfo info)
    {
        if (!groups.TryGetValue(name, out var list))
        {
            list = [];
            groups[name] = list;
        }
        list.Add(info);
    }
}
