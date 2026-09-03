
[Alias("弧相似")]
public class SimilarArc : Predicate
{
    public Arc Line1 { get => (Arc)Properties[0]; }
    public Arc Line2 { get => (Arc)Properties[1]; }
    public SimilarArc(Arc arc1, Arc arc2)
    {
        Add(arc1, arc2);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
        Sort();
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"{Properties[0]}与{Properties[1]}相似"
     : $"{Properties[0]} is similar to {Properties[1]}";

}
