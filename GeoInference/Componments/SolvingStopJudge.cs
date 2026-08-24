internal class SolvingStopJudge
{
    [DI] TargetBase TargetBase;
    [DI] ZScriptBuilder builder { get; set; }
    [DI] KnowledgeBase knowledgeBase { get; set; }
    [DI] EquationSolver EquationSolver { get; set; }
    MapleApp MapleApp { get; set; } = MapleApp.Instance;
    public void Judge(Knowledge pred)
    {
        if (pred.StrContains("SectorOBA_Area=12*Pi"))
            ;
        if (pred is QuantityValue value)
        {
            foreach (var target in TargetBase.SolveQuantityValueTargets.ToArray())
            {
                if (target.GeoQuantity == value.Quantity)
                {
                    target.IsSuccess = true;
                    target.Conclusion = pred;
                    target.Answer = value.Expr.ToString();
                    TargetBase.SolveQuantityValueTargets.Remove(target);
                    TargetBase.AllSolved = TargetBase.Targets.TrueForAll(t => t.IsSuccess);
                    break;
                }
            }
        }
        else if (pred is QuantityRatio ratio)
        {
            foreach (var target in TargetBase.SolveQuantityRatioTargets.ToArray())
            {
                if (target.GeoQuantity1 == ratio.Quantity1 && target.GeoQuantity2 == ratio.Quantity2)
                {
                    target.IsSuccess = true;
                    target.Conclusion = pred;
                    target.Answer = ratio.Ratio.ToString();
                    TargetBase.SolveQuantityRatioTargets.Remove(target);
                    TargetBase.AllSolved = TargetBase.Targets.TrueForAll(t => t.IsSuccess);
                    break;
                }
                else if (target.GeoQuantity1 == ratio.Quantity2 && target.GeoQuantity2 == ratio.Quantity1)
                {
                    target.IsSuccess = true;
                    target.Conclusion = pred;
                    target.Answer = ratio.Ratio.Invert().ToString();
                    TargetBase.SolveQuantityRatioTargets.Remove(target);
                    TargetBase.AllSolved = TargetBase.Targets.TrueForAll(t => t.IsSuccess);
                    break;
                }
            }
        }
        else
        {
            foreach (var target in TargetBase.ProvePredicateTargets.ToArray())
            {
                if (target.Target.HashCode == pred.HashCode)
                {
                    target.IsSuccess = true;
                    target.Conclusion = pred;
                    TargetBase.ProvePredicateTargets.Remove(target);
                    TargetBase.AllSolved = TargetBase.Targets.TrueForAll(t => t.IsSuccess);
                    break;
                }
            }
        }
    }
    public void Judge(EqualityChain ce)
    {
        foreach (var target in TargetBase.SolveLinearTargets.ToList())
        {
            List<Quantity> quantities = target.CoffDict.Keys.Where(ce.CoffDict.ContainsKey).ToList();
            if (ce.ActualValue is not null)
            {
                foreach (var q in quantities)
                {
                    var expr = ce.ToValue(q);
                    target.Tmp += expr.Mul(target.CoffDict[q]);
                    target.Conditions.Add(ce.GetValueReason(q));
                    target.CoffDict.Remove(q);
                }
                if (target.CoffDict.Keys.Count == 0)
                {
                    var eq = new Equation(target.Target, target.Tmp);
                    eq.LeftPart = target.Target;
                    eq.RightPart = target.Tmp;
                    eq.AddCondition(target.Conditions);
                    target.Conclusion = eq;
                    target.IsSuccess = true;
                    target.Answer = target.Tmp.ToString();
                    TargetBase.SolveLinearTargets.Remove(target);
                    TargetBase.AllSolved = TargetBase.Targets.TrueForAll(t => t.IsSuccess);
                }
            }
            else
            {
                var fq = ce.CoffDict.First().Key;
                bool t = false;
                foreach (var q in quantities)
                {
                    if (q == fq) continue;
                    if (!target.CoffDict.ContainsKey(fq))
                        target.CoffDict.Add(fq, 0);
                    t = true;
                    target.CoffDict[fq] += ce.CoffDict[fq] / ce.CoffDict[q] * target.CoffDict[q];
                    target.Conditions.Add(ce.GetRatioReason(q, fq));
                    target.CoffDict.Remove(q);
                }
                if (t)
                {
                    Judge(EquationSolver.DistanceLinearMatrix);
                    Judge(EquationSolver.AngularLinearMatrix);
                }
            }
        }
        foreach (var target in TargetBase.SolveProductionTargets.ToList())
        {
            List<Quantity> quantities = target.CoffDict.Keys.Where(ce.CoffDict.ContainsKey).ToList();
            if (ce.ActualValue is not null)
            {
                foreach (var q in quantities)
                {
                    var expr = ce.ToValue(q);
                    target.Tmp *= expr.Mul(target.CoffDict[q]);
                    target.CoffDict.Remove(q);
                }
                if (target.CoffDict.Keys.Count == 0)
                {
                    var eq = new Equation(target.Tmp, target.Tmp);
                    eq.LeftPart = target.Target;
                    eq.RightPart = target.Tmp;
                    eq.AddCondition(target.Conditions);
                    target.Conclusion = eq;
                    target.IsSuccess = true;
                    target.Answer = target.Tmp.ToString();
                    TargetBase.SolveProductionTargets.Remove(target);
                    TargetBase.AllSolved = TargetBase.Targets.TrueForAll(t => t.IsSuccess);
                }
            }
            else
            {
                if (quantities.Count >= 2)
                {
                    var fq = quantities[0];
                    foreach (var q in quantities.Skip(1))
                    {
                        target.CoffDict[fq] += ce.To(q, fq);
                        target.CoffDict.Remove(q);
                    }
                }
            }
        }

        foreach (var item in TargetBase.SolveExprTargets.ToArray())
        {
            if (TrySimplify(item))
            {
                item.Quantities = builder.GetExprQuantities(item.Tmp);
                if (item.Quantities.Count == 0)
                {
                    var eq = new Equation(item.Target, item.Tmp);
                    eq.LeftPart = item.Target;
                    eq.RightPart = item.Tmp;
                    eq.AddCondition(item.Conditions);
                    item.Conclusion = eq;
                    item.IsSuccess = true;
                    item.Answer = item.Tmp.ToString();
                    TargetBase.SolveExprTargets.Remove(item);
                    TargetBase.AllSolved = TargetBase.Targets.TrueForAll(t => t.IsSuccess);
                }
            }
        }
    }
    public void Judge(MapleBaseLinearMatrix matrix)
    {
        (Dictionary<Quantity, Expr>, Expr) Add(SumNode sum)
        {
            Dictionary<Quantity, Expr> dict = []; Expr constant = null;
            foreach (var item in sum.Addends)
            {
                if (item is QuantityNode gN)
                {
                    dict.Add(gN.Quantity, 1);
                }
                else if (item is ProductNode p && p.IsSingle)
                {
                    if (p.Single is QuantityNode gn)
                    {
                        dict.Add(gn.Quantity, p.Constant.ToString());
                    }
                }
            }
            foreach (var item in sum.Subtrahends)
            {
                if (item is QuantityNode gN)
                {
                    dict.Add(gN.Quantity, -1);
                }
                else if (item is ProductNode p && p.IsSingle)
                {
                    if (p.Single is QuantityNode gn)
                    {
                        dict.Add(gn.Quantity, p.Constant.Opposite().ToString());
                    }
                }
            }
            constant = sum.Constant.ToString();
            return (dict, constant);
        }

        foreach (var target in TargetBase.SolveLinearTargets.ToArray())
        {
            var b = matrix.TryEvaluateExpression(target.CoffDict, 0);
            if (b.success)
            {
                target.IsSuccess = true;
                var eq = new Equation(target.Target, target.Tmp);
                eq.LeftPart = target.Target;
                eq.RightPart = b.value + target.Tmp;
                target.Conclusion = eq;
                target.Answer = eq.RightPart.ToString();
                TargetBase.SolveLinearTargets.Remove(target);
                TargetBase.AllSolved = TargetBase.Targets.TrueForAll(t => t.IsSuccess);
            }
        }
        foreach (var item in TargetBase.SolveExprTargets.ToArray())
        {
            var z = builder.ParseZExpr(item.Tmp.ToString());
            if (z is SumNode sumNode)
            {
                var a = Add(sumNode);
                var b = matrix.TryEvaluateExpression(a.Item1, a.Item2);
                if (b.success)
                {
                    item.IsSuccess = true;
                    var eq = new Equation(item.Target, item.Tmp);
                    eq.LeftPart = item.Target;
                    eq.RightPart = b.value;
                    item.Conclusion = eq;
                    item.Answer = eq.RightPart.ToString();
                    TargetBase.SolveExprTargets.Remove(item);
                    TargetBase.AllSolved = TargetBase.Targets.TrueForAll(t => t.IsSuccess);
                }
            }
            else if (z is ProductNode productNode)
            {
                if (productNode.Multipliers.Count == 1 && productNode.Divisors.Count == 0)
                {
                    Dictionary<Quantity, Expr> dict = []; Expr constant = 0;
                    if (productNode.Multipliers[0] is QuantityNode gn)
                    {
                        dict.Add(gn.Quantity, productNode.Constant.ToString());
                    }
                    else if (productNode.Multipliers[0] is SumNode s)
                    {
                        var dd = Add(s);
                        foreach (var vv in dd.Item1)
                        {
                            dict.Add(vv.Key, vv.Value * productNode.Constant.ToExpr());
                        }
                        constant = dd.Item2 * productNode.Constant.ToExpr();
                    }

                    var b = matrix.TryEvaluateExpression(dict, constant);
                    if (b.success)
                    {
                        item.IsSuccess = true;
                        var eq = new Equation(item.Target, item.Tmp);
                        eq.LeftPart = item.Target;
                        eq.RightPart = b.value;
                        item.Conclusion = eq;
                        item.Answer = eq.RightPart.ToString();
                        TargetBase.SolveExprTargets.Remove(item);
                        TargetBase.AllSolved = TargetBase.Targets.TrueForAll(t => t.IsSuccess);
                    }
                }
            }
        }
    }
    public bool TrySimplify(SolveExprTarget exprTarget)
    {
        var isSimplified = false;
        ZListDict<EqualityChain, GeoQuantity> dict = [];
        foreach (var quantity in exprTarget.Quantities)
        {
            if (quantity is GeoQuantity geoQuantity)
            {
                if (knowledgeBase.IndexContinuedDict.ContainsKey(geoQuantity))
                {
                    dict[knowledgeBase.IndexContinuedDict[geoQuantity]].Add(geoQuantity);
                }
            }
        }
        foreach (var kv in dict)
        {
            if (kv.Key.ActualValue is not null)
            {
                foreach (var kvp in kv.Value)
                {
                    isSimplified = true;
                    exprTarget.Conditions.Add(kv.Key.GetValueReason(kvp));
                    exprTarget.Tmp = exprTarget.Tmp.Replace(kvp.ToString(), $"({kv.Key.ToValue(kvp)})");
                    exprTarget.Quantities.Remove(kvp);
                }
            }
            else
            {
                var d = kv.Key.CoffDict.First().Key;
                for (int i = 0; i < kv.Value.Count; i++)
                {
                    if (kv.Value[i] != d)
                    {
                        isSimplified = true;

                        exprTarget.Conditions.Add(kv.Key.GetRatioReason(kv.Value[i], d));
                        var ratio = kv.Key.CoffDict[d] / kv.Key.CoffDict[kv.Value[i]];
                        exprTarget.Tmp = exprTarget.Tmp.Replace(kv.Value[i].ToString(), $"(({ratio})*({d}))");
                        exprTarget.Quantities.Remove(kv.Value[i]);
                    }

                }
            }
        }
        if (isSimplified)
        {
            exprTarget.Tmp = MapleApp.Run($"simplify({exprTarget.Tmp})");
        }
        return isSimplified;
    }
}
