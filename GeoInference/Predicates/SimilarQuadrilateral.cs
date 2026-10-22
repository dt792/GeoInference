﻿
[Alias("相似四边形")]
public class SimilarQuadrilateral : Predicate
{
    public SimilarQuadrilateral(Point t11, Point t12, Point t13, Point t14, Point t21, Point t22, Point t23, Point t24)
    {
        Add(t11, t12, t13, t14, t21, t22, t23, t24);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
        NormalizeForPairedQuadrilateral();
    }
    public override string ToString() => GeoInferenceApp.UseChinese
    ? $"四边形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}与四边形{Properties[4]}{Properties[5]}{Properties[6]}{Properties[7]}相似"
    : $"Quadrilateral {Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]} is similar to Quadrilateral {Properties[4]}{Properties[5]}{Properties[6]}{Properties[7]}";
}
