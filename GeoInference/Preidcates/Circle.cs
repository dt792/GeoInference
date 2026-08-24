
[Alias("圆")]
public class Circle : PlaneFigure
{
    GeoQuantity radius;
    public GeoQuantity Radius
    {
        get
        {
            if (radius is null)
            {
                radius = new GeoQuantity([this], GeoQuantity.Radius);
            }

            return radius;
        }
    }

    GeoQuantity diameter;
    public GeoQuantity Diameter
    {
        get
        {
            if (diameter is null)
            {
                diameter = new GeoQuantity([this], GeoQuantity.Diameter);
            }

            return diameter;
        }
    }
    public Point Center { get => (Point)Properties[0]; }
    /// <summary>
    
    /// </summary>
    public Circle(params Point[] points)
    {
        Add(points);
        Normalize();
        SetHashCode();
    }
    public override string ForQuantity() => $"Circle{StringTool.ComposeList(Properties.Take(1), "")}{StringTool.ComposeList(Properties.Skip(1), "")}";
    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"圆{StringTool.ComposeList(Properties, "")}"
     : $"Circle{StringTool.ComposeList(Properties, "")}";

    public override void Normalize()
    {

        
        var sortedPoints = Properties.Skip(1)
            .Select(p => new
            {
                Point = p,
                Angle = Math.Atan2(((Point)p).Y - Center.Y, ((Point)p).X - Center.X) 
            })
            .Select(item => new
            {
                item.Point,
                
                AdjustedAngle = item.Angle < 0 ? item.Angle + 2 * Math.PI : item.Angle
            })
            .OrderBy(item => item.AdjustedAngle) 
            .Select(item => item.Point)
            .ToList();
        for (int i = 1; i < Properties.Count; i++)
        {
            Properties[i] = sortedPoints[i - 1];
        }
    }

}
