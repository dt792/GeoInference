namespace GeoInference.Definitions.Algebras.ZExpr;

public class RealSumNode : RealNode
{

    public ARationalNode Rational { get; set; } = Zero;
    public List<RealNode> Addends { get; set; } = new();
    public List<RealNode> Subtrahends { get; set; } = new();
    public Dictionary<RealNode, RealNode> CoffDict { get; set; } = new(new ZExprValueComparer());
    public override string ToString()
    {
        var str = "(";
        bool first = true;
        if (Rational != Zero)
        {
            str += $"{Rational}";
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
    public override RealSumNode Clone()
    {
        RealSumNode node = new RealSumNode();
        node.Rational = Rational.Clone();
        node.Addends = Addends.Select(s => s.Clone()).ToList();
        node.Subtrahends = Subtrahends.Select(s => s.Clone()).ToList();
        return node;
    }
    private void AdjustChildProductNodeSigns()
    {
        for (int i = 0; i < Addends.Count; i++)
        {
            if (Addends[i] is RealProductNode product)
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
            if (Subtrahends[i] is RealProductNode product)
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
    public override RealNode Simplify()
    {
        if (IsSimplified) return this;
        IsSimplified = true;
        for (int i = 0; i < Addends.Count; i++)
        {
            Addends[i] = Addends[i].Simplify();
        }
        for (int i = 0; i < Subtrahends.Count; i++)
        {
            Subtrahends[i] = Subtrahends[i].Simplify();
        }
        AdjustChildProductNodeSigns();
        MergeChildSumNodes();
        MergeIdenticalNodes();
        MergeRationalNumbers();
        Rational = Rational.Simplify();
        var check = CheckDegeneration();
        if (check.Item1) { return check.Item2; }
        AdjustChildProductNodeSigns();
        return this;
    }
    void MergeChildSumNodes()
    {
        var addRationalNodes = Addends.Where(node => node is RealSumNode).Select(node => (RealSumNode)node).ToList();
        var subRationalNodes = Subtrahends.Where(node => node is RealSumNode).Select(node => (RealSumNode)node).ToList();
        if (addRationalNodes.Count == 0 && subRationalNodes.Count == 0) return;
        foreach (var sum in addRationalNodes)
        {

            Rational = (ARationalNode)Rational.Add(sum.Rational);
            Addends.AddRange(sum.Addends);
            Subtrahends.AddRange(sum.Subtrahends);
        }
        foreach (var sum in subRationalNodes)
        {
            Rational = (ARationalNode)Rational.Sub(sum.Rational);
            Addends.AddRange(sum.Subtrahends);
            Subtrahends.AddRange(sum.Addends);
        }
        for (int i = 0; i < addRationalNodes.Count(); i++)
            Addends.Remove(addRationalNodes[i]);
        for (int i = 0; i < subRationalNodes.Count(); i++)
            Subtrahends.Remove(subRationalNodes[i]);
    }
    void MergeIdenticalNodes()
    {
        CoffDict.Clear();

        for (int i = 0; i < Addends.Count; i++)
        {
            if (Addends[i] is RealProductNode productNode)
            {
                var (r, e) = productNode.SplitRational();
                if (CoffDict.ContainsKey(e))
                    CoffDict[e] = (RealNode)CoffDict[e].Add(r);
                else CoffDict.Add(e, r);
            }
            else
            {
                if (CoffDict.ContainsKey(Addends[i]))
                    CoffDict[Addends[i]] = (RealNode)CoffDict[Addends[i]].Add(1);
                else CoffDict.Add(Addends[i], 1);
            }
        }
        for (int i = 0; i < Subtrahends.Count; i++)
        {
            if (Subtrahends[i] is RealProductNode productNode)
            {
                var (r, e) = productNode.SplitRational();
                if (CoffDict.ContainsKey(e))
                    CoffDict[e] = (RealNode)CoffDict[e].Sub(r);
                else CoffDict.Add(e, r.Opposite());
            }
            else
            {
                if (CoffDict.ContainsKey(Subtrahends[i]))
                    CoffDict[Subtrahends[i]] = (RealNode)CoffDict[Subtrahends[i]].Add(-1);
                else CoffDict.Add(Subtrahends[i], -1);
            }
        }
        Addends.Clear();
        Subtrahends.Clear();
        foreach (var kv in CoffDict.ToList())
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

            if (kv.Value > 0)
            {
                var p = (RealNode)kv.Value.Mul(kv.Key);
                p = p.Simplify();
                if (p is ARationalNode aRational)
                {
                    Rational = Rational + aRational;
                }
                else
                {
                    Addends.Add(p);
                }

            }
            else
            {
                var p = (RealNode)kv.Value.Mul(-kv.Key);
                p = p.Simplify();
                if (p is ARationalNode aRational)
                {
                    Rational = Rational + aRational;
                }
                else
                {
                    Subtrahends.Add(p);
                }
            }
        }
    }
    void MergeRationalNumbers()
    {
        for (int i = 0; i < Addends.Count(); i++)
        {
            if (Addends[i] is ARationalNode v)
            {
                Rational = (ARationalNode)Rational.Add(v);
                Addends.Remove(v);
                i -= 1;
            }
        }
        for (int i = 0; i < Subtrahends.Count(); i++)
        {
            if (Subtrahends[i] is ARationalNode v)
            {
                Rational = (ARationalNode)Rational.Sub(v);
                Subtrahends.Remove(v);
                i -= 1;
            }
        }
    }
    public (bool, RealNode) CheckDegeneration()
    {
        if (Addends.Count == 0 && Subtrahends.Count == 0) return (true, Rational);
        if (Addends.Count == 1 && Subtrahends.Count == 0 && Rational == Zero) return (true, Addends[0]);
        if (Addends.Count == 0 && Subtrahends.Count == 1 && Rational == Zero) return (true, new RealProductNode() { IsPositive = false, Multipliers = new() { Subtrahends[0] } }.Simplify());
        return (false, null);
    }
    public override ZExpr Add(ZExpr r)
    {
        if (r is IntNode i)
        {
            var temp = (RealSumNode)this.Clone();
            temp.Rational += i;
            var result = temp.Simplify();
            return result;
        }
        else if (r is RealSumNode sum)
        {
            var temp = (RealSumNode)this.Clone();
            temp.Rational += sum.Rational;
            temp.Addends.AddRange(sum.Addends.Select(e => e.Clone()));
            temp.Subtrahends.AddRange(sum.Subtrahends.Select(e => e.Clone()));
            var result = temp.Simplify();
            return result;
        }
        return base.Add(r);
    }
    public override ZExpr Sub(ZExpr r)
    {
        if (r is IntNode i)
        {
            var temp = (RealSumNode)this.Clone();
            temp.Rational -= i;
            var result = temp.Simplify();
            return result;
        }
        else if (r is RealSumNode sum)
        {
            var temp = (RealSumNode)this.Clone();
            temp.Rational -= sum.Rational;
            temp.Addends.AddRange(sum.Subtrahends.Select(e => e.Clone()));
            temp.Subtrahends.AddRange(sum.Addends.Select(e => e.Clone()));
            var result = temp.Simplify();
            return result;
        }
        return base.Add(r);
    }


    public override RealNode Opposite()
    {
        var temp = new RealSumNode();
        temp.Rational = this.Rational.Opposite();
        temp.Addends.AddRange(Subtrahends.Select(e => e.Clone()));
        temp.Subtrahends.AddRange(Addends.Select(e => e.Clone()));
        return temp;
    }
    public override double GetApproximation()
    {
        var temp = Rational.GetApproximation();
        foreach (var item in Addends)
        {
            temp += item.GetApproximation();
        }
        foreach (var item in Subtrahends)
        {
            temp -= item.GetApproximation();
        }
        return temp;
    }
}
