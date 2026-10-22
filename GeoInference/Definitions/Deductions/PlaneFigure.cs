namespace GeoInference.Knowledges;

public abstract class PlaneFigure : Figure
{
    GeoQuantity area;
    public GeoQuantity Area => GetQuantity(ref area, GeoQuantity.Area);

    GeoQuantity perimeter;
    public GeoQuantity Perimeter => GetQuantity(ref perimeter, GeoQuantity.Perimeter);

    public void NormalizeForPolygon()
    {
        var cProperties = this.Properties;
        int size = cProperties.Count;
        int flag = 0;
        Point point = cProperties[0] as Point;
        Point[] pointPreds = new Point[size];
        cProperties.CopyTo(pointPreds);

        for (int i = 1; i < size; i++)
        {
            if (cProperties[i].PosIndex < point.PosIndex)
            {
                flag = i;
                point = cProperties[i] as Point;
            }
        }
        for (int i = 0; i < size; i++)
        {
            int z = (i - flag + size) % size;
            cProperties[z] = pointPreds[i];
        }
        if (cProperties[1].PosIndex > cProperties[size - 1].PosIndex)
        {

            var temp = cProperties[1];
            cProperties[1] = cProperties[size - 1];
            cProperties[size - 1] = temp;

        }
    }
    public void NormalizeForTrapezoid()
    {
        var cProperties = this.Properties;
        int size = cProperties.Count;
        
        int flag = 0;
        Point point = (Point)cProperties[0];

        Point[] pointPreds = new Point[size];
        cProperties.CopyTo(pointPreds);

        for (int i = 1; i < size; i++)
        {
            if (cProperties[i].PosIndex < point.PosIndex)
            {
                flag = i;
                point = (Point)cProperties[i];
            }
        }
        
        if (flag > 1)
        {
            cProperties[0] = pointPreds[2];
            cProperties[1] = pointPreds[3];
            cProperties[2] = pointPreds[0];
            cProperties[3] = pointPreds[1];
        }
        
        if (cProperties[0].PosIndex > cProperties[1].PosIndex)
        {
            var temp = cProperties[1];
            cProperties[1] = cProperties[0];
            cProperties[0] = temp;

            temp = cProperties[3];
            cProperties[3] = cProperties[2];
            cProperties[2] = temp;
        }
    }
    /// <summary>
    /// </summary>
    public void NormalizeForRightTrapezoid()
    {
        var cProperties = this.Properties;
        if (cProperties[0].PosIndex > cProperties[3].PosIndex)
        {
            var temp = cProperties[0];
            cProperties[0] = cProperties[3];
            cProperties[3] = temp;

            temp = cProperties[1];
            cProperties[1] = cProperties[2];
            cProperties[2] = temp;
        }
    }
}
