namespace GeoInference.Definitions.Algebras.ZExpr;

public class ArcTanNode : TrigonometricNode
{
    public override string ToString()
    {
        return $"arctan({Expr})";
    }
    public override ArcTanNode Clone()
    {
        ArcTanNode node = new ArcTanNode();
        node.Expr = Expr.Clone();
        return node;
    }
}
