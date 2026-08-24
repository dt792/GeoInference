
[Alias("弧")]
public class Arc : PlaneFigure
{
    GeoQuantity majorarclength;
    public GeoQuantity MajorArcLength
    {
        get
        {
            if (ReferenceEquals(majorarclength, null))
            {
                majorarclength = new GeoQuantity([this], GeoQuantity.MajorArcLength);
            }
            return majorarclength;
        }
    }
    GeoQuantity minorarclength;
    public GeoQuantity MinorArcLength
    {
        get
        {
            if (ReferenceEquals(minorarclength, null))
            {
                minorarclength = new GeoQuantity([this], GeoQuantity.MinorArcLength);
            }
            return minorarclength;
        }
    }
    GeoQuantity size;
    public GeoQuantity Size
    {
        get
        {
            if (ReferenceEquals(size, null))
            {
                size = new GeoQuantity([this], GeoQuantity.Size);
            }
            return size;
        }
    }
    /// <summary>
    
    /// </summary>
    public Arc(params Point[] points)
    {
        Add(points);
        Normalize();
        SetHashCode();
    }
    public string ToPrint() => $"{StringTool.ComposeList(Properties, "")}";
    public override string ForQuantity() => $"Arc{StringTool.ComposeList(Properties, "")}";
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"弧{StringTool.ComposeList(Properties, "")}"
    : $"Arc{StringTool.ComposeList(Properties, "")}";
    public override void Normalize()
    {
        Sort(1, 2);
    }
}
