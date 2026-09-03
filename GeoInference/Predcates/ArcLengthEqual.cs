
[Alias("弧长相等", "弧的长度相等")]
public class ArcLengthEqual : Predicate
{
    // <summary>
    
    /// </summary>
    public ArcLengthEqual(Arc arc1, Arc arc2)
    {
        Add(arc1, arc2);
        Normalize();
        SetHashCode();
    }
    public Arc Arc1 { get => (Arc)Properties[0]; }
    public Arc Arc2 { get => (Arc)Properties[1]; }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[0]}与{Properties[1]}的长度相等"
    : $"{Properties[0]} and {Properties[1]} are equal in length";
    public override void Normalize()
    {
        Sort();
    }
}