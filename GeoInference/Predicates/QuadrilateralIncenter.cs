
[Alias("四边形内心", "四边形的内心", "QuadriliateralIncenter")]
/// <summary>
/// </summary>
public class QuadrilateralIncenter : Predicate
{
    public QuadrilateralIncenter(Quadrilateral quadrilateral, Point point)
    {
        Add(quadrilateral, point);
        Normalize();
        SetHashCode();
    }

    public Quadrilateral Quadrilateral => (Quadrilateral)Properties[0];
    public Point Incenter => (Point)Properties[1];

    public override string ToString() => GeoInferenceApp.UseChinese
      ? $"{Properties[0]}的内心是{Properties[1]}"
      : $"{Properties[1]} is the incenter of {Properties[0]}";

    public override void Normalize()
    {

    }
}
