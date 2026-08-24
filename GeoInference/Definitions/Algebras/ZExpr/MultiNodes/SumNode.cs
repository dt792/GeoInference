namespace GeoInference.Definitions.Algebras.ZExpr;

public class SumNode : ZExpr
{
    public ZExpr Constant { get; set; } = ZExpr.Zero;

    public List<ZExpr> Addends { get; internal set; } = new();
    public List<ZExpr> Subtrahends { get; set; } = new();
    public void AddToAddends(ZExpr expr)
    {
        Addends.Add(expr);
        IsSimplified = false;
    }
    public void AddToSubtrahends(ZExpr expr)
    {
        Subtrahends.Add(expr);
        IsSimplified = false;
    }
    public override SumNode Clone()
    {
        SumNode node = new SumNode();
        node.Constant = (RealNode)Constant.Clone();
        node.Addends = Addends.Select(s => s.Clone()).ToList();
        node.Subtrahends = Subtrahends.Select(s => s.Clone()).ToList();
        return node;
    }

    public override ZExpr Simplify()
    {
        for (int i = 0; i < Addends.Count; i++)
        {
            Addends[i] = Addends[i].Simplify();
        }
        for (int i = 0; i < Subtrahends.Count; i++)
        {
            Subtrahends[i] = Subtrahends[i].Simplify();
        }
        MergeRationalNumbers();
        MergeChildSumNodes();

        AdjustChildProductNodeSigns();
        MergeIdenticalNodes();
        Constant = (RealNode)Constant.Simplify();
        var check = CheckDegeneration();
        if (check.Item1) { return check.Item2; }
        return this;
    }

    private void MergeChildSumNodes()
    {
        var addRationalNodes = Addends.Where(node => node is SumNode).ToList();
        var subRationalNodes = Subtrahends.Where(node => node is SumNode).ToList();
        if (addRationalNodes.Count == 0 && subRationalNodes.Count == 0) return;
        foreach (var rationalNode in addRationalNodes)
        {
            Constant = (RealNode)Constant.Add(((SumNode)rationalNode).Constant);
            Addends.AddRange(((SumNode)rationalNode).Addends);
            Subtrahends.AddRange(((SumNode)rationalNode).Subtrahends);
        }
        foreach (var rationalNode in subRationalNodes)
        {
            Constant = (RealNode)Constant.Sub(((SumNode)rationalNode).Constant);
            Addends.AddRange(((SumNode)rationalNode).Subtrahends);
            Subtrahends.AddRange(((SumNode)rationalNode).Addends);
        }
        for (int i = 0; i < addRationalNodes.Count(); i++)
        {
            Addends.Remove(addRationalNodes[i]);
        }
        for (int i = 0; i < subRationalNodes.Count(); i++)
        {
            Subtrahends.Remove(subRationalNodes[i]);
        }
    }

    private void AdjustChildProductNodeSigns()
    {
        for (int i = 0; i < Addends.Count; i++)
        {
            if (Addends[i] is ProductNode product)
            {
                if (!product.IsPositive)
                {
                    product.IsPositive = true;
                    Addends.RemoveAt(i);
                    Subtrahends.Add(product);
                    i--;
                }
            }
        }
        for (int i = 0; i < Subtrahends.Count; i++)
        {
            if (Subtrahends[i] is ProductNode product)
            {
                if (!product.IsPositive)
                {
                    product.IsPositive = true;
                    Subtrahends.RemoveAt(i);
                    Addends.Add(product);
                    i--;
                }
            }
        }
    }

    private void MergeRationalNumbers()
    {
        for (int i = 0; i < Addends.Count(); i++)
        {
            if (Addends[i] is RealNode v)
            {
                Constant = Constant.Add(v);
                Addends.Remove(v);
                i -= 1;
            }
        }
        for (int i = 0; i < Subtrahends.Count(); i++)
        {
            if (Subtrahends[i] is RealNode v)
            {
                Constant = Constant.Sub(v);
                Subtrahends.Remove(v);
                i -= 1;
            }
        }
    }

    private void MergeIdenticalNodes()
    {
        Dictionary<ZExpr, ZExpr> dict = new(new ZExprValueComparer());
        for (int i = 0; i < Addends.Count; i++)
        {
            if (Addends[i] is QuantityNode atomExpr)
            {
                if (dict.ContainsKey(atomExpr))
                    dict[atomExpr] = dict[atomExpr].Add(1);
                else dict.Add(atomExpr, 1);
                Addends.RemoveAt(i);
                i -= 1;
                continue;
            }
            else if (Addends[i] is ProductNode productNode)
            {
                if (productNode.Multipliers.Count == 1 && productNode.Divisors.Count == 0)
                {
                    if (productNode.Multipliers[0] is QuantityNode a)
                    {
                        if (dict.ContainsKey(a))
                            dict[a] = dict[a].Add(productNode.Constant);
                        else dict.Add(a, productNode.Constant);
                        Addends.RemoveAt(i);
                        i -= 1;
                        continue;
                    }
                }
            }
            if (dict.ContainsKey(Addends[i]))
                dict[Addends[i]] = dict[Addends[i]].Add(1);
            else dict.Add(Addends[i], 1);
            Addends.RemoveAt(i);
            i -= 1;
        }
        for (int i = 0; i < Subtrahends.Count; i++)
        {
            if (Subtrahends[i] is QuantityNode atomExpr)
            {
                if (dict.ContainsKey(atomExpr))
                    dict[atomExpr] = dict[atomExpr].Sub(1);
                else dict.Add(atomExpr, -1);
                Subtrahends.RemoveAt(i);
                i -= 1;
                continue;
            }
            else if (Subtrahends[i] is ProductNode productNode)
            {
                if (productNode.Multipliers.Count == 1 && productNode.Divisors.Count == 0)
                {
                    if (productNode.Multipliers[0] is QuantityNode a)
                    {
                        if (dict.ContainsKey(a))
                            dict[a] = dict[a].Sub(productNode.Constant);
                        else dict.Add(a, productNode.Constant.Opposite());
                        Subtrahends.RemoveAt(i);
                        i -= 1;
                        continue;
                    }
                }
            }
            if (dict.ContainsKey(Subtrahends[i]))
                dict[Subtrahends[i]] = dict[Subtrahends[i]].Sub(1);
            else dict.Add(Subtrahends[i], -1);
            Subtrahends.RemoveAt(i);
            i -= 1;
        }
        foreach (var kv in dict)
        {
            if (kv.Value == Zero)
            {
                continue;
            }
            if (kv.Value == ZExpr.One)
            {
                Addends.Add(kv.Key); continue;
            }

            if (kv.Value == ZExpr.NegativeOne)
            {
                Subtrahends.Add(kv.Key); continue;
            }

            var p = kv.Key.Mul(kv.Value).Simplify();
            if (p is RealNode)
            {
                Constant = (RealNode)Constant.Add(p);
            }
            else if (p is ProductNode pp)
            {
                if (pp.IsPositive)
                {
                    Addends.Add(pp);
                }
                else
                {
                    pp.IsPositive = true;
                    Subtrahends.Add(pp);
                }
            }
            else
            {
                Addends.Add(p);
            }
        }
        for (int i = 0; i < Subtrahends.Count; i++)
        {
            for (int j = 0; j < Addends.Count; j++)
            {
                if (Addends[j] == Subtrahends[i])
                {
                    Addends.RemoveAt(j);
                    Subtrahends.RemoveAt(i);
                    i -= 1;
                    break;
                }
            }
        }
    }


    public (bool, ZExpr) CheckDegeneration()
    {
        if (Addends.Count == 0 && Subtrahends.Count == 0) return (true, Constant);
        if (Addends.Count == 1 && Subtrahends.Count == 0 && Constant == ZExpr.Zero) return (true, Addends[0]);
        if (Addends.Count == 0 && Subtrahends.Count == 1 && Constant == ZExpr.Zero) return (true, (ZExpr)new ProductNode() { IsPositive = false, Multipliers = new() { Subtrahends[0] } });
        return (false, null);
    }

    public override string ToString()
    {
        var str = "(";
        bool first = true;
        if (Constant.CompareTo(0) != ExprCompareResult.Equal)
        {
            str += $"{Constant}";
            first = false;
        }
        foreach (ZExpr e in Addends)
        {
            if (first)
                first = false;
            else
                str += "+";
            str += e.ToString();
        }
        if (Subtrahends.Count > 0)
        {
            str += "-";
            first = true;
            foreach (ZExpr e in Subtrahends)
            {
                if (first)
                    first = false;
                else
                    str += "-";
                str += e.ToString();
            }
        }
        return str + ")";
    }


    public override SumNode Opposite()
    {
        Constant = (RealNode)Constant.Opposite();
        var temp = Addends;
        Addends = Subtrahends;
        Subtrahends = temp;
        return this;
    }
}