
public class LinearEquation : Equation
{
    public Dictionary<Quantity, Expr> Coff { get; set; } = [];
    public Expr Constant { get; set; }
    public LinearEquation(Dictionary<Quantity, Expr> coff, Expr expr)
    {
        var d = coff.ToList();
        d.Sort((x, y) => x.Key.Index.CompareTo(y.Key.Index));
        Coff = d.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Constant = expr;
        SetHashCode();
        CoExpr = ToString();
    }

    public void SetHashCode()
    {
        HashCode = (ulong)ToString().GetHashCode();
    }
    public override string ToString()
    {
        var sortedCoff = Coff.OrderBy(kvp => kvp.Key.Index).ToList();
        var terms = sortedCoff.Select(kvp =>
        {
            return $"({kvp.Value})*({kvp.Key})";
        });

        string leftSide = string.Join("+", terms);

        if (string.IsNullOrEmpty(leftSide))
        {
            leftSide = "0";
        }
        string rightSide = Constant?.ToString() ?? "0";
        return $"{leftSide}={rightSide}";
    }
}