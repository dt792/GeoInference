
[Alias("直线", "共线")]
public class Line : Figure
{
    public List<Point> Points { get => Properties.Select(p => (Point)p).ToList(); }
    public Line(params Point[] points)
    {
        if (points.Count() < 2)
        {
            throw new ArgumentException();
        }
        Add(points);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
        List<Point> points = new List<Point>(Properties.Select(p => (Point)p));
        if (points[0].X != points[1].X)
        {
            points.Sort((a, b) => a.X.CompareTo(b.X));
        }
        else if (points[0].Y != points[1].Y)
        {
            points.Sort((a, b) => a.Y.CompareTo(b.Y));
        }
        else
        {
            points.Sort((a, b) => a.Z.CompareTo(b.Z));
        }
        Properties.Clear();
        Point minPoint = points[0];
        foreach (Point p in points)
        {
            if (p.PosIndex < minPoint.PosIndex) { minPoint = p; }
            Properties.Add(p);
        }
        if (Properties.IndexOf(minPoint) > Properties.Count / 2.0 - 1)
        {
            Properties.Reverse();
        }
    }
    public string ToPrint() => $"{StringTool.ComposeList(Properties, "")}";
    public override string ForQuantity() => $"Line{StringTool.ComposeList(Properties, "")}";
    public override string ToString() => $"Line{StringTool.ComposeList(Properties, "")}";

}
