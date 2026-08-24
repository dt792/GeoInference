
[Alias("直角三角形")]
/// <summary>

/// </summary>
public class RightTriangle : Triangle
{
    /// <summary>
    
    /// </summary>
    
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public RightTriangle(Point p1, Point p2, Point p3) : base(p1, p2, p3)
    {
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"直角三角形{Properties[0]}{Properties[1]}{Properties[2]}"
    : $"RightTriangle{Properties[0]}{Properties[1]}{Properties[2]}";


    public override void Normalize()
    {
        this.Sort(1, 2);
    }

}
