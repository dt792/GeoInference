
[Alias("直线的交点", "线段的交点")]
public class LineIntersection : Predicate
{

    public LineIntersection(Point point, Line line1, Line line2)
    {
        Add(point, line1, line2);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
      ? $"{Properties[0]}是{Properties[1]}与{Properties[2]}的交点"
      : $"{Properties[0]} is the intersection of {Properties[1]} and {Properties[2]}";

    public override void Normalize()
    {
        Sort(1, 2);
    }
}
