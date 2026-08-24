
[Alias("线段长度比值", "线段长度比例", "线段比值", "线段比例")]
/// <summary>

/// </summary>
public class SegmentLengthRatio : Predicate
{
    public Segment Seg1 { get => (Segment)Properties[0]; }

    public Segment Seg2 { get => (Segment)Properties[1]; }

    // <summary>
    
    /// </summary>
    public SegmentLengthRatio(Segment segment1, Segment segment2, Expr expr)
    {
        Add(segment1, segment2, expr);

        Normalize();
        SetHashCode();
    }


    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"{Properties[0]}与{Properties[1]}的比值是{Expr}"
     : $"Ratio of {Properties[0]} to {Properties[1]} is {Expr}";

    public override void Normalize()
    {
        if (Properties[0].PosIndex > Properties[1].PosIndex)
        {
            var temp = Properties[0];
            Properties[0] = Properties[1];
            Properties[1] = temp;
            Expr = Expr.Invert();
        }
    }

}
