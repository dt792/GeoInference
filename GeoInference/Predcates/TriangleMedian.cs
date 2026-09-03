
[Alias("三角形中线", "三角形的中线")]

public class TriangleMedian : Predicate
{
    public TriangleMedian(Segment seg, Triangle triangle)
    {
        Add(seg, triangle);
        Normalize();
        SetHashCode();

    }


    public override string ToString() => GeoInferenceApp.IsZhOrEn
      ? $"{Properties[1]}的中线是{Properties[0]}"
      : $"{Properties[0]} is the median of {Properties[1]}";

    public override void Normalize()
    {

    }
}