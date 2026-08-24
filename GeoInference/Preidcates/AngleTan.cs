
namespace GeoInference.MergeKnowledges;

[Alias("角的Tan值")]

public class AngleTan : Predicate
{
    public Angle Angle { get => (Angle)Properties[0]; }
    public AngleTan(Angle angle, Expr expr)
    {
        Add(angle, expr);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => GeoInferenceApp.IsZhOrEn
      ? $"{Properties[0]}的大小的Tan={Expr}"
      : $"Tan of {Properties[0]} = {Expr}";

    public override void Normalize()
    {


    }
}
