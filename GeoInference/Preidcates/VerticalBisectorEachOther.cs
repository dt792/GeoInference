

[Alias("线段互相垂直平分")]

public class VerticalBisectorEachOther : Predicate
{
    public VerticalBisectorEachOther(Segment segment, Segment segment1)
    {
        Add(segment, segment1);
        Normalize();
        SetHashCode();
    }

    public override void Normalize()
    {
        Sort(0, 1);
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[0]}和{Properties[1]}互相垂直平分"
    : $"{Properties[0]} and {Properties[1]} are perpendicular bisectors of each other";

}