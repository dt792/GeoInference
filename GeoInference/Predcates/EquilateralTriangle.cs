
[Alias("等边三角形", "正三角形")]
/// <summary>

/// </summary>
public class EquilateralTriangle : Triangle
{
    /// <summary>
    
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public EquilateralTriangle(Point p1, Point p2, Point p3) : base(p1, p2, p3)
    {
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
      ? $"等边三角形{Properties[0]}{Properties[1]}{Properties[2]}"
      : $"EquilateralTriangle{Properties[0]}{Properties[1]}{Properties[2]}";

    public override void Normalize()
    {
        Sort();
    }
}
