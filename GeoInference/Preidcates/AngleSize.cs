
[Alias("角的大小", "角大小", "角的值")]

public class AngleSize : Predicate
{
    public Angle Angle { get => (Angle)Properties[0]; }

    public AngleSize(Angle angle, Expr expr)
    {
        Add(angle, expr);
        Normalize();
        SetHashCode();
        if (StrContains("角DA_C_OE的大小是90度"))
            ;
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"{Properties[0]}的大小是{Expr}度"
     : $"Measure of {Properties[0]} is {Expr} degrees";

    public override void Normalize()
    {


    }
}
