using GeoInference.Definitions.Knowledges;
[Alias("几何量比值")]
public class QuantityRatio : Equation
{
    public Quantity Quantity1 { get; set; }
    public Quantity Quantity2 { get; set; }
    public Expr Ratio { get; set; }
    public QuantityRatio(Quantity leftPart, Quantity rightPart)
    {
        Quantity1 = leftPart;
        Quantity2 = rightPart;
        Ratio = Expr.One;

        Normalize();
        SetHashCode();
        
    }
    public QuantityRatio(Quantity leftPart, Quantity rightPart, Expr expr)
    {
        Quantity1 = leftPart;
        Quantity2 = rightPart;
        Ratio = expr;

        Normalize();
        SetHashCode();

        if (Ratio.Value == "undefined" || Ratio.Value == "infinity")
            IsAvailable = false;
    }
    public override string ToString() => $"{Quantity1}/{Quantity2}={Ratio}";
    public override void SetHashCode()
    {
        HashCode = ClassIndexDict[GetType().FullName] << 54;
        HashCode |= Quantity1.Index;
        HashCode |= Quantity2.Index << 27;
    }
    public override void Normalize()
    {
        if (Quantity1.Index > Quantity2.Index)
        {
            (Quantity1, Quantity2) = (Quantity2, Quantity1);
            Ratio = Ratio.Invert();
        }
    }

    public Knowledge ToPred()
    {
        if (Quantity1.Unit != Quantity2.Unit) return null;
        if (Quantity1 is GeoQuantity q1 && Quantity2 is GeoQuantity q2)
        {
            if (Ratio == Expr.One)
            {
                var diameterSegmentEqual = ToSegmentEqualCircleDiameter(q1, q2) ?? ToSegmentEqualCircleDiameter(q2, q1);
                if (diameterSegmentEqual is not null)
                {
                    diameterSegmentEqual.Reason = Reason;
                    diameterSegmentEqual.Conditions.AddRange(Conditions);
                    return diameterSegmentEqual;
                }
            }
            if (q1.Figures[0] is Segment seg1 && q2.Figures[0] is Segment seg2)
            {
                if (Ratio == Expr.One)
                {
                    SegmentLengthEqual pred = new SegmentLengthEqual(seg1, seg2);
                    pred.Reason = Reason;
                    pred.Conditions.AddRange(Conditions);
                    return pred;
                }
                else
                {
                    SegmentLengthRatio pred = new SegmentLengthRatio(seg1, seg2, Ratio);
                    pred.Reason = Reason;
                    pred.Conditions.AddRange(Conditions);
                    return pred;
                }
            }
            else if (q1.Figures[0] is Angle a1 && q2.Figures[0] is Angle a2)
            {
                if (q1.PropName == Quantity.Sin) return null;
                if (Ratio == Expr.One)
                {
                    AngleSizeEqual pred = new AngleSizeEqual(a1, a2);
                    pred.Reason = Reason;
                    pred.Conditions.AddRange(Conditions);
                    return pred;
                }
                else
                {
                    AngleSizeRatio pred = new AngleSizeRatio(a1, a2, Ratio);
                    pred.Reason = Reason;
                    pred.Conditions.AddRange(Conditions);
                    return pred;
                }
            }
            else if (q1.Figures[0] is Arc arc1 && q2.Figures[0] is Arc arc2)
            {
                if (Ratio == Expr.One)
                {
                    ArcLengthEqual pred = new ArcLengthEqual(arc1, arc2);
                    pred.Reason = Reason;
                    pred.Conditions.AddRange(Conditions);
                    return pred;
                }
            }
        }
        return null;
    }

    private static SegmentEqualCircleDiameter ToSegmentEqualCircleDiameter(GeoQuantity diameterQuantity, GeoQuantity segmentQuantity)
    {
        if (diameterQuantity.PropName != Quantity.Diameter) return null;
        if (segmentQuantity.PropName != Quantity.Length) return null;
        if (diameterQuantity.Figures[0] is not Circle circle) return null;
        if (segmentQuantity.Figures[0] is not Segment segment) return null;
        return new SegmentEqualCircleDiameter(circle, segment);
    }
}
