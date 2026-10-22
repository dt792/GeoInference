﻿
[Alias("正方形")]
/// <summary>

/// </summary>
public class Square : Quadrilateral
{

    public Square(Point p1, Point p2, Point p3, Point p4) : base(p1, p2, p3, p4)
    {
    }
    public override string ToString() => GeoInferenceApp.UseChinese
     ? $"正方形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}"
     : $"Square{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
        this.NormalizeForPolygon();
    }
}
