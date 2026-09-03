
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
        if (StrContains("OBA"))
            ;
    }
    public override string ForQuantity() => $"Sector{StringTool.ComposeList(Properties, "")}";
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"扇形{StringTool.ComposeList(Properties, "")}"
    : $"Sector{StringTool.ComposeList(Properties, "")}";
    public override void Normalize()
    {
        //Sort(1,2);
    }

}
