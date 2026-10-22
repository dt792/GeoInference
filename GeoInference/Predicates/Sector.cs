
[Alias("扇形")]
public class Sector : PlaneFigure
{
    /// <summary>
    
    /// </summary>
    public Sector(params Point[] points)
    {
        Add(points);
        Normalize();
        SetHashCode();
    }
    public override string ForQuantity() => $"Sector{StringTool.ComposeList(Properties, "")}";
    public override string ToString() => GeoInferenceApp.UseChinese
    ? $"扇形{StringTool.ComposeList(Properties, "")}"
    : $"Sector{StringTool.ComposeList(Properties, "")}";
    public override void Normalize()
    {
    }

}
