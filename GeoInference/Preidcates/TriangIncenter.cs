
[Alias("三角形内心", "三角形的内心")]

public class TriangleIncenter : Predicate
{
    public TriangleIncenter(Point point, Triangle triangle)
    {
        Add(point, triangle);
        Normalize();
        SetHashCode();

    }


    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[1]}的内心是{Properties[0]}"
    : $"{Properties[0]} is the incenter of {Properties[1]}";

    public override void Normalize()
    {

    }
}