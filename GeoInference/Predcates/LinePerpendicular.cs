
[Alias("直线垂直", "线段垂直")]
public class LinePerpendicular : Predicate
{
    public LinePerpendicular(Line line1, Line line2)
    {
        Add(line1, line2);
        Normalize();
        SetHashCode();
    }
    public Line Line1 { get => (Line)Properties[0]; }
    public Line Line2 { get => (Line)Properties[1]; }
    public override void Normalize()
    {
        Sort();
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
      ? $"{Properties[0]}⊥{Properties[1]}"
      : $"{Properties[0]} ⊥ {Properties[1]}";
}
