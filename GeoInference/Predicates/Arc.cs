
[Alias("弧")]
public class Arc : PlaneFigure
{
    GeoQuantity majorarclength;
    public GeoQuantity MajorArcLength => GetQuantity(ref majorarclength, GeoQuantity.MajorArcLength);
    GeoQuantity minorarclength;
    public GeoQuantity MinorArcLength => GetQuantity(ref minorarclength, GeoQuantity.MinorArcLength);
    GeoQuantity size;
    public GeoQuantity Size => GetQuantity(ref size, GeoQuantity.Size);
    /// <summary>
    
    /// </summary>
    public Arc(params Point[] points)
    {
        Add(points);
        Normalize();
        SetHashCode();
    }
    public override string ForQuantity() => $"Arc{StringTool.ComposeList(Properties, "")}";
    public override string ToString() => GeoInferenceApp.UseChinese
    ? $"弧{StringTool.ComposeList(Properties, "")}"
    : $"Arc{StringTool.ComposeList(Properties, "")}";
    public override void Normalize()
    {
        Sort(1, 2);
    }
}
