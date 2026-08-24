
[Alias("中点")]
public class Midpoint : PointOnSeg
{
    public Point Point { get => (Point)Properties[0]; }
    public Point EndPoint1 { get => (Point)Properties[1]; }
    public Point EndPoint2 { get => (Point)Properties[2]; }
    /// <summary>
    
    /// </summary>
    
    
    
    public Midpoint(Point p1, Point endPoint1, Point endPoint2) : base(p1, endPoint1, endPoint2)
    {
        Add(p1, endPoint1, endPoint2);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
        Sort(1, 2);
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[0]}是{Properties[1]}{Properties[2]}的中点"
    : $"{Properties[0]} is the midpoint of {Properties[1]}{Properties[2]}";
}
