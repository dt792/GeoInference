
[Alias("风筝形", "风筝")]
/// <summary>

/// </summary>
public class Kite : Quadriliateral
{
    /// <summary>
    ///   p2
    
    ///   p4
    /// </summary>
    public Kite(Point p1, Point p2, Point p3, Point p4) : base(p1, p2, p3, p4)
    {
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"筝形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}"
     : $"Kite{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
        Sort(0, 2);
    }
}
