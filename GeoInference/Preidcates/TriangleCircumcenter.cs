

[Alias("三角形外心", "三角形的外心")]

public class TriangleCircumcenter : Predicate
{
    public TriangleCircumcenter(Point point, Triangle triangle)
    {
        Add(point, triangle);
        Normalize();
        SetHashCode();

    }


    public override string ToString() => GeoInferenceApp.IsZhOrEn
      ? $"{Properties[1]}的外心是{Properties[0]}"
      : $"{Properties[0]} is the circumcenter of {Properties[1]}";

    public override void Normalize()
    {

    }
}