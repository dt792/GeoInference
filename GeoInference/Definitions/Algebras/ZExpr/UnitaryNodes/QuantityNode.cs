namespace GeoInference.Definitions.Algebras.ZExpr;

public class QuantityNode : ZExpr
{
    public static Dictionary<Quantity, QuantityNode> CacheMutNodes { get; set; } = new();

    public Quantity Quantity { get; set; }
    public QuantityNode(Quantity mut)
    {
        Quantity = mut;
    }
    public override ZExpr Clone()
    {
        return this;
    }
    public override ZExpr Simplify()
    {
        return this;
    }

    public override string ToString()
    {
        return Quantity.ToString();
    }

}
