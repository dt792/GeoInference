
[Alias("弧全等")]
public class CongruentArc : Predicate
{
    public Arc Arc1 { get => (Arc)Properties[0]; }
    public Arc Arc2 { get => (Arc)Properties[1]; }
    public CongruentArc(Arc arc1, Arc arc2)
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
     ? $"{Properties[0]}与{Properties[1]}全等"
     : $"{Properties[0]} is congruent to {Properties[1]}";
}
