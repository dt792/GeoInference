
[Alias("弧中点")]
public class ArcMidpoint : Predicate
{
    public Point Point { get => (Point)Properties[0]; }
    /// <summary>
    
    /// </summary>
    
    
    
    public ArcMidpoint(Point p1, Arc endPoint1)
    {
        Add(p1, endPoint1);
        Normalize();
        SetHashCode();
    }

    public override void Normalize()
    {

    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[0]}是{Properties[1]}的中点"
    : $"{Properties[0]} is the midpoint of {Properties[1]}";
}
