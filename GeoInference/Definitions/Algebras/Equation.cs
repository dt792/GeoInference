
using GeoInference.Definitions.Knowledges;

public enum GeoEquationTypes
{
    Unknow,
    Value,
    Ratio,
    DistanceLinear,
    AngularLinear,
    DistanceProduction,
    AngularProduction,
    DistanceOther,
    AngularOther,
    MixedOther,
}
public class Equation : Knowledge
{
    public GeoEquationTypes Type { get; set; }
    public QuantityClassifications Unit { get; set; }
    public Expr LeftPart { get; set; }
    public Expr RightPart { get; set; }

    public List<ulong> AllConditionHashCode { get; set; } = new List<ulong> { };
    public Expr CoExpr { get; set; }
    public List<Quantity> Quantities { get; set; }
    public Equation()
    {

    }
    public Equation(Expr leftPart, Expr rightPart)
    {
        LeftPart = leftPart;
        RightPart = rightPart;
        Normalize();
        SetHashCode();
        CoExpr = $"({LeftPart})-({RightPart})";
    }

    public override void SetHashCode()
    {
        HashCode = (ulong)ToString().GetHashCode();
    }
    public override string ToString() => $"{LeftPart}={RightPart}";

    public virtual void Normalize()
    {
        if (LeftPart.ToString().CompareTo(RightPart.ToString()) > 0)
        {
            (LeftPart, RightPart) = (RightPart, LeftPart);
        }
    }
}