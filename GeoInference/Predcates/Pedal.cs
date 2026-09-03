
[Alias("垂足", "垂点")]

public class Pedal : Predicate
{
    public Line Line1 { get => (Line)Properties[1]; }
    public Line Line2 { get => (Line)Properties[2]; }

    //public Point point { get =>(Point) Properties[0]; }
    /// <summary>
    
    /// </summary>
    
    /// <param name="line"></param>
    /// <param name="line1"></param>

    public Pedal(Point point, Line line, Line line1)
    {
        Add(point, line, line1);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
        Sort(1, 2);
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
       ? $"{Properties[1]}和{Properties[2]}的垂足是{Properties[0]}"
       : $"The foot of perpendicular from {Properties[1]} to {Properties[2]} is {Properties[0]}";

}

