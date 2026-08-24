
public enum QuantityClassifications
{
    Cos,
    Sin,
    Tan,
    Angle,
    Distance,
}
public class Quantity
{
    public const string Size = "Size";
    public const string Cos = "Cos";
    public const string Sin = "Sin";
    public const string Tan = "Tan";

    public const string Length = "Length";
    public const string Area = "Area";
    public const string Perimeter = "Perimeter";

    public const string MajorArcLength = "MajorArcLength";
    public const string MinorArcLength = "MinorArcLength";
    public const string Radius = "Radius";
    public const string Diameter = "Diameter";

    public const string Distance = "Distance";
    public const string Ratio = "Ratio";
    public QuantityClassifications Unit { get; set; } = QuantityClassifications.Distance;
    protected Quantity()
    {
        Index = CurIndex++;

    }
    public static ulong CurIndex { get; set; }
    public ulong Index { get; set; }

    public static Expr operator +(Quantity expr1, Quantity expr2) => Expr.FromQuantity(expr1) + Expr.FromQuantity(expr2);

    public static Expr operator -(Quantity expr1, Quantity expr2) => Expr.FromQuantity(expr1) - Expr.FromQuantity(expr2);

    public static Expr operator *(Quantity expr1, Quantity expr2) => Expr.FromQuantity(expr1) * Expr.FromQuantity(expr2);

    public static Expr operator /(Quantity expr1, Quantity expr2) => Expr.FromQuantity(expr1) / Expr.FromQuantity(expr2);

    public static Expr operator +(Quantity expr1, Expr expr2) => Expr.FromQuantity(expr1) + expr2;

    public static Expr operator -(Quantity expr1, Expr expr2) => Expr.FromQuantity(expr1) - expr2;

    public static Expr operator *(Quantity expr1, Expr expr2) => Expr.FromQuantity(expr1) * expr2;

    public static Expr operator /(Quantity expr1, Expr expr2) => Expr.FromQuantity(expr1) / expr2;

    public Expr Add(Quantity expr2) => Expr.FromQuantity(this) + Expr.FromQuantity(expr2);

    public Expr Sub(Quantity expr2) => Expr.FromQuantity(this) - Expr.FromQuantity(expr2);

    public Expr Mul(Quantity expr2) => Expr.FromQuantity(this) * Expr.FromQuantity(expr2);

    public Expr Div(Quantity expr2) => Expr.FromQuantity(this) / Expr.FromQuantity(expr2);

    public Expr Add(Expr expr2) => Expr.FromQuantity(this) + expr2;

    public Expr Sub(Expr expr2) => Expr.FromQuantity(this) - expr2;

    public Expr Mul(Expr expr2) => Expr.FromQuantity(this) * expr2;

    public Expr Div(Expr expr2) => Expr.FromQuantity(this) / expr2;

    public Expr Pow(Expr expr2) => Expr.FromQuantity(this).Pow(expr2);

    public static Func<string, Quantity> Parse { get; set; }

}
