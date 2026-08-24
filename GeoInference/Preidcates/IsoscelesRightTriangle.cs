
[Alias("直角等腰三角形", "等腰直角三角形")]
/// <summary>

/// </summary>
public class IsoscelesRightTriangle : Triangle
{
    /// <summary>
    
    /// </summary>
    
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public IsoscelesRightTriangle(Point p1, Point p2, Point p3) : base(p1, p2, p3)
    {
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"等腰直角三角形{Properties[0]}{Properties[1]}{Properties[2]}"
     : $"IsoscelesRightTriangle{Properties[0]}{Properties[1]}{Properties[2]}";

    public override void Normalize()
    {
        this.Sort(1, 2);
    }

}
