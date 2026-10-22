[Alias("线段长度等于圆直径", "线段的长度等于圆的直径")]
public class SegmentEqualCircleDiameter : Predicate
{
    /// <summary>
    /// 线段长度等于圆直径
    /// </summary>
    public SegmentEqualCircleDiameter(Circle circle, Segment segment)
    {
        Add(circle, segment);
        Normalize();
        SetHashCode();
    }
    public Circle Circle { get => (Circle)Properties[0]; }
    public Segment Seg { get => (Segment)Properties[1]; }
    public override string ToString() => GeoInferenceApp.UseChinese
     ? $"{Properties[1]}的长度等于{Properties[0]}的直径"
     : $"The length of {Properties[1]} equals the diameter of {Properties[0]}";
    public override void Normalize()
    {
    }
}
