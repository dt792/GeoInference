
public class Var : Quantity
{
    public Var(string name) : base()
    {
        Name = name;
    }
    public string Name { get; set; }

    public override string ToString() => Name;
    public static implicit operator string(Var v) => v.Name;
    public static implicit operator Expr(Var v) => v.Name;
}
