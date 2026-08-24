
[Alias("角的Cos值")]

public class AngleCos : Predicate
{
    public Angle Angle { get => (Angle)Properties[0]; }
    public AngleCos(Angle angle, Expr expr)
    {
        Add(angle, expr);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"{Properties[0]}的大小的Cos={Expr}"
    : $"Cos of {Properties[0]} = {Expr}";

    public override void Normalize()
    {


    }
}
