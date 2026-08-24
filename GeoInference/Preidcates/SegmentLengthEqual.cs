
[Alias("线段长度相等", "线段的长度相等")]
public class SegmentLengthEqual : Predicate
{

    // <summary>
    
    /// </summary>
    public SegmentLengthEqual(Segment segment1, Segment segment2)
    {
        Add(segment1, segment2);
        Normalize();
        SetHashCode();
        if (segment1 == segment2)
            IsAvailable = false;
    }
    public Segment Seg1 { get => (Segment)Properties[0]; }
    public Segment Seg2 { get => (Segment)Properties[1]; }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"{Properties[0]}与{Properties[1]}的长度相等"
     : $"{Properties[0]} and {Properties[1]} are equal in length";
    public override void Normalize()
    {
        Sort();
    }
}