
[Alias("等腰梯形")]
/// <summary>

/// </summary>
public class IsoscelesTrapezoid : Quadriliateral
{
    /// <summary>
    
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    public IsoscelesTrapezoid(Point p1, Point p2, Point p3, Point p4) : base(p1, p2, p3, p4)
    {
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
        ? $"等腰梯形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}"
        : $"IsoscelesTrapezoid{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";
    public override void Normalize()
    {
        this.NormalizeForTrapezoid();
    }

}
