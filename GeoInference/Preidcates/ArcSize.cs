
namespace GeoInference.MergeKnowledges;

[Alias("弧的角度")]
public class ArcSize : Predicate
{
    // <summary>
    
    /// </summary>
    public ArcSize(Arc arc1, Expr arc2)
    {
        Add(arc1, arc2);
        Normalize();
        SetHashCode();
    }
    public Arc Arc { get => (Arc)Properties[0]; }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"{Properties[0]}的大小是{Expr}度"
     : $"Measure of {Properties[0]} is {Expr} degrees";
    public override void Normalize()
    {
        Sort();
    }
}