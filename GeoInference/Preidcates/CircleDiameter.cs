
[Alias("圆直径")]
/// <summary>

/// </summary>
public class CircleDiameter : Predicate
{
    public Circle Circle { get => (Circle)Properties[0]; }
    public Segment Diameter { get => (Segment)Properties[1]; }

    // <summary>
    
    /// </summary>
    public CircleDiameter(Circle circle, Segment line)
    {
        Add(circle, line);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"{Properties[1]}是{Properties[0]}的直径"
     : $"{Properties[1]} is the diameter of {Properties[0]}";

    public override void Normalize()
    {
    }
}
