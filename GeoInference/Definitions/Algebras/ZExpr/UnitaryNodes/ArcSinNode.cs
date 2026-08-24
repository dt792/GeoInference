namespace GeoInference.Definitions.Algebras.ZExpr;

public class ArcSinNode : TrigonometricNode
{
    public override string ToString()
    {
        return $"arcsin({Expr})";
    }
    public override ArcSinNode Clone()
    {
        ArcSinNode node = new ArcSinNode();
        node.Expr = Expr.Clone();
        return node;
    }
}
