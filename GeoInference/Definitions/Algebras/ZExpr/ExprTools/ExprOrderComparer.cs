namespace GeoInference.Definitions.Algebras.ZExpr;

public class ExprOrderComparer : IComparer<ZExpr>
{
    public static Dictionary<Type, int> typeOrder = new Dictionary<Type, int>() {
        { typeof(IntNode), 0 },
        { typeof(PiNode), 1 },
        { typeof(FractionNode), 2 },
        { typeof(RealSumNode), 3 },
        { typeof(RealProductNode), 4 },
        { typeof(RealPowerNode), 5 },

        { typeof(QuantityNode), 10 } ,

        { typeof(SinNode), 25 } ,
        { typeof(CosNode), 26 },
        { typeof(TanNode), 27 } ,

        { typeof(SumNode), 31 },
        { typeof(ProductNode), 32 },
        { typeof(PowerNode), 32 },
    };
    ZExprValueComparer ExprValue = new ZExprValueComparer();
    public int Compare(ZExpr? x, ZExpr? y)
    {
        var a = typeOrder[x.GetType()];
        var b = typeOrder[y.GetType()];
        if (a != b)
            return a.CompareTo(b);
        else
            return string.Compare(x.ToString(), y.ToString(), StringComparison.Ordinal);
    }
}
