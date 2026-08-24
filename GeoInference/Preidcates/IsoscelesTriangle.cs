
[Alias("等腰三角形")]
/// <summary>

/// </summary>
public class IsoscelesTriangle : Triangle
{
    /// <summary>
    
    /// </summary>
    
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public IsoscelesTriangle(Point p1, Point p2, Point p3) : base(p1, p2, p3)
    {
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"等腰三角形{Properties[0]}{Properties[1]}{Properties[2]}"
    : $"IsoscelesTriangle{Properties[0]}{Properties[1]}{Properties[2]}";
    public override void Normalize()
    {
        if (Properties[1].PosIndex > Properties[2].PosIndex)
        {
            var temp = Properties[1];
            Properties[1] = Properties[2];
            Properties[2] = temp;
        }
    }

}
