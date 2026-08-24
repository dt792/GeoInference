
[Alias("垂直平分线")]

public class VerticalBisectorLine : Predicate
{
    public Segment Seg { get => (Segment)Properties[0]; }
    public Line Bisector { get => (Line)Properties[1]; }
    /// <summary>
    /// 
    /// </summary>
    
    
    public VerticalBisectorLine(Segment segment, Line line)
    {
        Add(segment);
        Add(line);
        Normalize();
        SetHashCode();
    }

    public override void Normalize()
    {

    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[1]}是{Properties[0]}的垂直平分线"
    : $"{Properties[1]} is the perpendicular bisector of {Properties[0]}";
}