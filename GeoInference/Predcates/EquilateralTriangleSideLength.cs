namespace GeoInference.MergeKnowledges;

[Alias("等边三角形边长", "等边三角形的边长")]
/// <summary>

/// </summary>
public class EquilateralTriangleSideLength : Predicate
{
    /// <summary>
    
    /// </summary>
    public static ulong ClassIndex { get; set; }
    /// <summary>
    
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public EquilateralTriangleSideLength(EquilateralTriangle triangle, Expr expr)
    {
        Add(triangle, expr);
        Normalize();
        SetHashCode();
    }


    public override string ToString() => GeoInferenceApp.IsZhOrEn
     ? $"{Properties[0]}的边长是{Expr}"
     : $"Side length of {Properties[0]} is {Expr}";

    public override void Normalize()
    {
    }

}
