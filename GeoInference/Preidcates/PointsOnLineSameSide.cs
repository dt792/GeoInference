
[Alias("点在同侧")]
public class PointsOnLineSameSide : Predicate
{
    public PointsOnLineSameSide(Point point1, Point point2, Point point3, Point point4)
    {
        Add(point1, point2, point3, point4);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
      ? $"{Properties[2]}、{Properties[3]}在{Properties[0]}{Properties[1]}同侧"
      : $"{Properties[2]} and {Properties[3]} are on the same side of {Properties[0]}{Properties[1]}";

    public override void Normalize()
    {

    }
}

