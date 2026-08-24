
[Alias("菱形")]
/// <summary>

/// </summary>
public class Rhombus : Quadriliateral
{

    public Rhombus(Point p1, Point p2, Point p3, Point p4) : base(p1, p2, p3, p4)
    {
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"菱形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}"
     : $"Rhombus{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
        this.NormalizeForPolygon();
    }


}
