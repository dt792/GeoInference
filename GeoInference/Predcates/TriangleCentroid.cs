

[Alias("三角形重心", "三角形的重心")]

public class TriangleCentroid : Predicate
{
    public TriangleCentroid(Point point, Triangle triangle)
    {
        Add(point, triangle);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
      ? $"{Properties[1]}的重心是{Properties[0]}"
      : $"{Properties[0]} is the centroid of {Properties[1]}";
    public override void Normalize()
    {

    }
}
