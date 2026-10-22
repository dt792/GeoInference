﻿
[Alias("角", "三点角")]
public class Angle : Figure
{
    public Point Vertex { get; set; }
    public List<Point> Edge1 { get; set; }
    public List<Point> Edge2 { get; set; }
    public Angle(List<Point> edge1, Point p2, List<Point> edge2)
    {
        Edge1 = edge1;
        Edge2 = edge2;
        Add(edge1.ToArray());
        Vertex = p2;
        Add(p2);
        Add(edge2.ToArray());
        Normalize();
        SetHashCode();
    }

    public override string ForQuantity() => $"Angle{StringTool.ComposeList(Edge1, "")}_{Vertex}_{StringTool.ComposeList(Edge2, "")}";
    public override string ToString() => GeoInferenceApp.UseChinese
    ? $"角{StringTool.ComposeList(Edge1, "")}_{Vertex}_{StringTool.ComposeList(Edge2, "")}"
    : $"Angle_{StringTool.ComposeList(Edge1, "")}_{Vertex}_{StringTool.ComposeList(Edge2, "")}";
    public override void Normalize()
    {
        if (Edge1[0].PosIndex > Edge2[0].PosIndex)
        {

            var temp = Edge1;
            Edge1 = Edge2;
            Edge2 = temp;
        }
        int index = 0;
        for (int i = 0; i < Edge1.Count; i++)
        {
            this.Properties[i] = Edge1[i];
            index++;
        }
        this.Properties[index++] = Vertex;
        for (int i = 0; i < Edge2.Count; i++)
        {
            this.Properties[index] = Edge2[i];
            index++;
        }

    }
    public override void SetHashCode()
    {
        HashCode = ClassIndexDict[GetType().FullName] << 54;
        for (int k = 0; k < Properties.Count && k < 12; ++k)
        {
            if (k == 0)
                HashCode |= Properties[k].PosIndex;
            else
                HashCode |= (ulong)Properties[k].PosIndex << k * 4;
        }
    }
    #region Quantity
    GeoQuantity size;
    public GeoQuantity Size => GetQuantity(ref size, GeoQuantity.Size);
    GeoQuantity cos;
    public GeoQuantity Cos => GetQuantity(ref cos, GeoQuantity.Cos);
    GeoQuantity sin;
    public GeoQuantity Sin => GetQuantity(ref sin, GeoQuantity.Sin);
    GeoQuantity tan;
    public GeoQuantity Tan => GetQuantity(ref tan, GeoQuantity.Tan);
    #endregion
}