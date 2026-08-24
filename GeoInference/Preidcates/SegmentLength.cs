


[Alias("线段的长度", "线段长度")]
/// <summary>

/// </summary>
public class SegmentLength : Predicate
{
    public GeoQuantity GeoQuantity => Segment.Length;
    public Segment Segment { get => (Segment)Properties[0]; }
    // <summary>
    
    /// </summary>
    public SegmentLength(Segment segment, Expr expr)
    {
        Add(segment, expr);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[0]}的长度是{Expr}"
    : $"Length of {Properties[0]} is {Expr}";

    public override void Normalize()
    {
    }

}
