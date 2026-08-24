namespace GeoInference.Definitions.Algebras.ZExpr;

public class ZExprValueComparer : IEqualityComparer<ZExpr>
{
    public bool Equals(ZExpr? x, ZExpr? y)
    {
        if (ReferenceEquals(x, y)) return true;
        return x.ToString() == y.ToString();
    }

    public int GetHashCode(ZExpr obj)
    {
        if (obj == null) return 0;
        var code = obj.ToString().GetHashCode();
        return code;
    }
}
