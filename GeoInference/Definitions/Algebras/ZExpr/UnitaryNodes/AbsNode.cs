namespace GeoInference.Definitions.Algebras.ZExpr;

public class AbsNode : ZExpr
{
    public ZExpr Expr { get; set; }
    public override ZExpr Simplify()
    {
        if (Expr is QuantityNode g)
        {
            return g;
        }
        return this;
    }
    public override string ToString()
    {
        return $"abs({Expr})";
    }
    public override AbsNode Clone()
    {
        AbsNode node = new AbsNode();
        node.Expr = Expr.Clone();
        return node;
    }
}
