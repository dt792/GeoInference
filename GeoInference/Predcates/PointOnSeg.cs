
[Alias("点在线段上", "点在线上", "点在两点之间")]
public class PointOnSeg : Predicate
{
    public PointOnSeg(Point point, Point endPoint1, Point endPoint2)
    {
        Add(point, endPoint1, endPoint2);
        Normalize();
        SetHashCode();
    }
    public Point Point { get => (Point)Properties[0]; }
    public Point EndPoint1 { get => (Point)Properties[1]; }
    public Point EndPoint2 { get => (Point)Properties[2]; }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
       ? $"{Properties[0]}在{Properties[1]}{Properties[2]}上"
       : $"{Properties[0]} lies on {Properties[1]}{Properties[2]}";


    public override void Normalize()
    {
    }
}
