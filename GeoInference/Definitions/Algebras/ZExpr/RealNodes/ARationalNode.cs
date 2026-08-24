namespace GeoInference.Definitions.Algebras.ZExpr;

public abstract class ARationalNode : RealNode
{
    #region overrides
    public static ARationalNode operator +(ARationalNode ZExpr1, ARationalNode ZExpr2) => (ARationalNode)ZExpr1.Add(ZExpr2);

    public static ARationalNode operator -(ARationalNode ZExpr1, ARationalNode ZExpr2) => (ARationalNode)ZExpr1.Sub(ZExpr2);

    public static ARationalNode operator *(ARationalNode ZExpr1, ARationalNode ZExpr2) => (ARationalNode)ZExpr1.Mul(ZExpr2);

    public static ARationalNode operator /(ARationalNode ZExpr1, ARationalNode ZExpr2) => (ARationalNode)ZExpr1.Div(ZExpr2);

    public static ARationalNode operator -(ARationalNode ZExpr) => (ARationalNode)ZExpr.Opposite();
    #endregion
    public override abstract ARationalNode Invert();
    public override abstract ARationalNode Opposite();
    public override abstract ARationalNode Clone();
    public override abstract ARationalNode Simplify();
}
