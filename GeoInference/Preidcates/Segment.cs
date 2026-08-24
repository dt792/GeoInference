
[Alias("线段")]
public class Segment : Figure
{
    GeoQuantity length;
    public GeoQuantity Length
    {
        get
        {
            if (ReferenceEquals(length, null))
            {
                length = new GeoQuantity([this], GeoQuantity.Length);
            }
            return length;
        }
    }
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
