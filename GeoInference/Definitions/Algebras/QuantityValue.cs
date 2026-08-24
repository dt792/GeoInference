using GeoInference.Definitions.Knowledges;
using GeoInference.MergeKnowledges;

public class QuantityValue : Equation
{
    public Quantity Quantity { get; set; }
    public Expr Expr { get; set; }
    public QuantityValue(Quantity geoQuantity, Expr expr)
    {
        Quantity = geoQuantity;
        Expr = expr;
        SetHashCode();
        if (Expr.Value == "undefined"|| Expr.Value == "infinity")
            IsAvailable = false;
    }

    public override void SetHashCode()
    {
        HashCode = ClassIndexDict[nameof(QuantityValue)] << 54;
        HashCode |= Quantity.Index;
    }
    public Knowledge ToPred()
    {
        if (Quantity is GeoQuantity q)
        {
            if (q.Figures.Count() == 1)
            {
                var Figure = q.Figures.First();
                var PropName = q.PropName;
                if (Figure is Angle angle)
                {
                    if (PropName is Quantity.Size)
                    {
                        return new AngleSize(angle, Expr);
                    }
                    else if (PropName is Quantity.Cos)
                    {
                        return new AngleCos(angle, Expr);
                    }
                    else if (PropName is Quantity.Sin)
                    {
                        return new AngleSin(angle, Expr);
                    }
                    else if (PropName is Quantity.Tan)
                    {
                        return new AngleTan(angle, Expr);
                    }

                }
                else if (Figure is Segment segment)
                {
                    return new SegmentLength(segment, Expr);
                }
                else if (Figure is Arc arc)
                {
                    if (PropName is Quantity.Size)
                    {
                        return new ArcSize(arc, Expr);
                    }
                }
            }
        }
        return null;

    }
    public override string ToString()
    {
        return $"{Quantity}={Expr}";
    }
}
