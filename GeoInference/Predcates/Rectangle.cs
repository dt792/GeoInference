
[Alias("长方形")]
/// <summary>

/// </summary>
public class Rectangle : Quadriliateral
{
    public Rectangle(Point p1, Point p2, Point p3, Point p4) : base(p1, p2, p3, p4)
    {
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"矩形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}"
    : $"Rectangle{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";
    public override void Normalize()
    {
        this.NormalizeForPolygon();
    }

}
