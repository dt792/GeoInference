
[Alias("圆切线")]
/// <summary>

/// </summary>
public class CircleTangent : Predicate
{
    public Circle Circle { get => (Circle)Properties[0]; }
    public Line Tangent { get => (Line)Properties[1]; }

    // <summary>
    
    /// </summary>
    public CircleTangent(Circle circle, Line line)
    {
        Add(circle, line);
        SetHashCode();
    }
    public override void Normalize()
    {
        this.Sort();
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[1]}是{Properties[0]}的切线"
    : $"{Properties[1]} is tangent to {Properties[0]}";
}
