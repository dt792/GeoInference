
[Alias("角的值相等", "角的大小相等")]

public class AngleSizeEqual : Predicate
{
    public Angle Angle1 { get => (Angle)Properties[0]; }
    public Angle Angle2 { get => (Angle)Properties[1]; }
    public AngleSizeEqual(Angle angle1, Angle angle2)
    {
        Add(angle1, angle2);
        Normalize();
        SetHashCode();
        if (angle1 == angle2)
            IsAvailable = false;
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[0]}与{Properties[1]}的大小相等"
    : $"{Properties[0]} and {Properties[1]} are equal in measure";


    public override void Normalize()
    {
        Sort();
    }
}
