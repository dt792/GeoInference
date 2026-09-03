
[Alias("角的Sin值")]

public class AngleSin : Predicate
{
    public Angle Angle { get => (Angle)Properties[0]; }
    public AngleSin(Angle angle, Expr expr)
    {
        Add(angle, expr);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"{Properties[0]}的大小的Sin={Expr}"
     : $"Sin of {Properties[0]} = {Expr}";

    public override void Normalize()
    {


    }
}
