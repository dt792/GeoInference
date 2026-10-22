
[Alias("线段")]
public class Segment : Figure
{
    GeoQuantity length;
    public GeoQuantity Length => GetQuantity(ref length, GeoQuantity.Length);
    public Segment(Point p1, Point p2)
    {

        Add(p1, p2);
        Normalize();
        SetHashCode();
    }


    public override void Normalize()
    {
        Sort();
    }
    public override string ForQuantity() => $"{Properties[0]}{Properties[1]}";
    public override string ToString() => $"{Properties[0]}{Properties[1]}";

}
