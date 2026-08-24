
[Alias("正方形的边长")]
/// <summary>

/// </summary>
public class SquareEdgeLength : Predicate
{
    /// <summary>
    
    /// </summary>
    public static ulong ClassIndex { get; set; }

    public SquareEdgeLength(Square square, Expr value)
    {
        Add(square, value);
        Normalize();
        SetHashCode();
    }


    public override string ToString() => GeoInferenceApp.IsZhOrEn
      ? $"{Properties[0]}的边长为{Expr}"
      : $"Side length of {Properties[0]} is {Expr}";

    public override void Normalize()
    {
    }

    public override void SetHashCode()
    {
        HashCode = ClassIndex << 54;
        for (int k = 0; k < Properties.Count && k < 6; ++k)
        {
            if (k == 0) HashCode |= (ulong)Properties[k].PosIndex;
            else HashCode |= (ulong)Properties[k].PosIndex << (k * 9);
        }
    }

}
