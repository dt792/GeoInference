namespace GeoInference.Definitions.Algebras.ZExpr;

public class ArcCosNode : TrigonometricNode
{
    public override string ToString()
    {
        return $"arccos({Expr})";
    }
    public override ArcCosNode Clone()
    {
        ArcCosNode node = new ArcCosNode();
        node.Expr = Expr.Clone();
        return node;
    }
}
