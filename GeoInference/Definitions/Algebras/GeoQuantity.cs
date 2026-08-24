
public class GeoQuantity : Quantity
{
    public Figure[] Figures { get; set; }
    public string PropName { get; init; }

    public GeoQuantity(Figure[] figures, string propName) : base()
    {
        Figures = figures;
        PropName = propName;
        if (propName == Size) Unit = QuantityClassifications.Angle;
        else if (propName == Cos) Unit = QuantityClassifications.Cos;
        else if (propName == Sin) Unit = QuantityClassifications.Sin;
        else if (propName == Tan) Unit = QuantityClassifications.Tan;
        else Unit = QuantityClassifications.Distance;
        
    }
    public override string ToString()
    {
        return $"{StringTool.ComposeList(Figures, "_", f => f.ForQuantity())}_{PropName}";
    }
}
