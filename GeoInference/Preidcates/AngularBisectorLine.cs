
[Alias("角平分线")]
public class AngularBisectorLine : Predicate
{
    public Angle Angle { get => (Angle)Properties[0]; }
    public Line Bisector { get => (Line)Properties[1]; }
    public List<Point> PointOnBisector { get; set; }
    public AngularBisectorLine(Angle angle, Line line)
    {
        Add(angle, line);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"{Properties[1]}是{Properties[0]}的角平分线"
     : $"{Properties[1]} is the angle bisector of {Properties[0]}";
}
