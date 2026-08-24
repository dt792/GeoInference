
[Alias("点")]
public class Point : Figure
{
    public string Name { get; set; }
    public double X { get; set; } = double.MinValue;
    public double Y { get; set; } = double.MinValue;
    public double Z { get; set; } = double.MinValue;
    public Point(string name)
    {
        Name = name;
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
    }
    public override void SetHashCode()
    {
        SetNameHashCode(Name);
    }
    public override string ForQuantity() => $"{Name}";
    public override string ToString() => $"{Name}";
}
