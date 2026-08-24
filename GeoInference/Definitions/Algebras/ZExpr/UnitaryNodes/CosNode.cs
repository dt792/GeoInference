namespace GeoInference.Definitions.Algebras.ZExpr;

public class CosNode : TrigonometricNode
{
    public override string ToString()
    {
        return $"cos({Expr})";
    }
    public override CosNode Clone()
    {
        CosNode node = new CosNode();
        node.Expr = Expr.Clone();
        return node;
    }
}
