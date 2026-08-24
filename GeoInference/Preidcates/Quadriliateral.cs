
[Alias("四边形")]
/// <summary>

/// </summary>
public class Quadriliateral : PlaneFigure
{
    public Point P1 => (Point)Properties[0];
    public Point P2 => (Point)Properties[1];
    public Point P3 => (Point)Properties[2];
    public Point P4 => (Point)Properties[3];
    /// <summary>
    
    /// </summary>
    public Quadriliateral(Point p1, Point p2, Point p3, Point p4)
    {
        Add(p1, p2, p3, p4);
        Normalize();
        SetHashCode();
    }
    public override string ForQuantity() => $"Quad{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"四边形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}"
    : $"Quadrilateral{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";
    public override void Normalize()
    {
        this.NormalizeForPolygon();
    }
}
