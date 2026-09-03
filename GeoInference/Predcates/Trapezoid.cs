
[Alias("梯形")]
/// <summary>

/// </summary>
public class Trapezoid : Quadriliateral
{
    /// <summary>
    
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    public Trapezoid(Point p1, Point p2, Point p3, Point p4) : base(p1, p2, p3, p4)
    {
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"梯形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}"
    : $"Trapezoid{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";
    public override void Normalize()
    {
        this.NormalizeForTrapezoid();
    }

}
