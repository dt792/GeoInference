
[Alias("点在异侧")]
public class PointsOnLineDifferentSide : Predicate
{
    public PointsOnLineDifferentSide(Point point1, Point point2, Point point3, Point point4)
    {
        Add(point1, point2, point3, point4);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"{Properties[2]}、{Properties[3]}在{Properties[0]}{Properties[1]}不同侧"
     : $"{Properties[2]} and {Properties[3]} are on opposite sides of {Properties[0]}{Properties[1]}";

    public override void Normalize()
    {

    }
}

