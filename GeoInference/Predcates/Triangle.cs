
[Alias("三角形")]
public class Triangle : PlaneFigure
{
    public Point P1 => (Point)Properties[0];
    public Point P2 => (Point)Properties[1];
    public Point P3 => (Point)Properties[2];
    /// <summary>
    
    /// </summary>
    public Triangle(Point p1, Point p2, Point p3)
    {
        Add(p1, p2, p3);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"三角形{Properties[0]}{Properties[1]}{Properties[2]}"
    : $"Triangle{Properties[0]}{Properties[1]}{Properties[2]}";
    public override string ForQuantity() => $"Tri{Properties[0]}{Properties[1]}{Properties[2]}";
    public override void Normalize()
    {
        Sort();
    }

}
