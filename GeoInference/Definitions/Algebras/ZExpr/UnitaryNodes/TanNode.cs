namespace GeoInference.Definitions.Algebras.ZExpr;

public class TanNode : TrigonometricNode
{
    public override string ToString()
    {
        return $"tan({Expr})";
    }
    public override TanNode Clone()
    {
        TanNode node = new TanNode();
        node.Expr = Expr.Clone();
        return node;
    }
}
