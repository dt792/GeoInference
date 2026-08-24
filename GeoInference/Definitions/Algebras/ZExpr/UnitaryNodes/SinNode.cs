namespace GeoInference.Definitions.Algebras.ZExpr;

public class SinNode : TrigonometricNode
{
    public override string ToString()
    {
        return $"sin({Expr})";
    }
    public override SinNode Clone()
    {
        SinNode node = new SinNode();
        node.Expr = Expr.Clone();
        return node;
    }
}
