
[Alias("平行四边形")]
/// <summary>

/// </summary>
public class Parallelogram : Quadriliateral
{

    public Parallelogram(Point p1, Point p2, Point p3, Point p4) : base(p1, p2, p3, p4)
    {
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"平行四边形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}"
    : $"Parallelogram{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
        this.NormalizeForPolygon();
    }
}
