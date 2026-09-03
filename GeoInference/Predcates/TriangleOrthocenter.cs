
[Alias("三角形垂心", "三角形的垂心")]
public class TriangleOrthocenter : Predicate
{
    public TriangleOrthocenter(Point point, Triangle triangle)
    {
        Add(point, triangle);
        Normalize();
        SetHashCode();

    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
      ? $"{Properties[1]}的垂心是{Properties[0]}"
      : $"{Properties[0]} is the orthocenter of {Properties[1]}";

    public override void Normalize()
    {

    }
}