
[Alias("三等分点")]
public class TrisectionPoint : Predicate
{
    /// <summary>
    
    /// </summary>
    
    
    
    
    public TrisectionPoint(Point p1, Point p2, Point p3, Point p4)
    {
        Add(p1, p2, p3, p4);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
        //Sort(1, 2);
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[0]}和{Properties[1]}是线段{Properties[2]}{Properties[3]}的三等分点"
    : $"{Properties[0]} and {Properties[1]} are the trisection points of segment {Properties[2]}{Properties[3]}";

}