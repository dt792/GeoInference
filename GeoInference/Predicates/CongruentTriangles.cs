﻿
[Alias("全等三角形")]
public class CongruentTriangles : Predicate
{
    public CongruentTriangles(Point t11, Point t12, Point t13, Point t21, Point t22, Point t23)
    {
        Add(t11, t12, t13, t21, t22, t23);
        NormalizeForPrism();
        SetHashCode();
    }
    public override void Normalize()
    {
        NormalizeForPrism();
    }
    public override string ToString() => GeoInferenceApp.UseChinese
     ? $"三角形{Properties[0]}{Properties[1]}{Properties[2]}与三角形{Properties[3]}{Properties[4]}{Properties[5]}全等"
     : $"Triangle {Properties[0]}{Properties[1]}{Properties[2]} is congruent to Triangle {Properties[3]}{Properties[4]}{Properties[5]}";
}
