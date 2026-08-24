
[Alias("全等四边形")]
public class CongruentQuadriliateral : Predicate
{
    public CongruentQuadriliateral(Point t11, Point t12, Point t13, Point t14, Point t21, Point t22, Point t23, Point t24)
    {
        Add(t11, t12, t13, t14, t21, t22, t23, t24);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
        int size = this.Properties.Count;
        int num = size / 2;
        int flag = 0;
        Point point = (Point)Properties[0];
        Point[] pointPreds = new Point[size];
        Properties.CopyTo(pointPreds);
        for (int i = 1; i < num; i++)
        {
            if (Properties[i].PosIndex < point.PosIndex)
            {
                flag = i;
                point = (Point)Properties[i];
            }
        }
        for (int i = 0; i < num; i++)
        {
            int z = (i - flag + num) % num;
            Properties[z] = pointPreds[i];
            Properties[z + num] = pointPreds[i + num];
        }
        if (Properties[1].PosIndex > Properties[num - 1].PosIndex)
        {
            for (int i = 1; i <= num / 2; i++)
            {
                Point temp = (Point)Properties[i];
                Properties[i] = Properties[num - i];
                Properties[num - i] = temp;
                Point temp1 = (Point)Properties[i + num];
                Properties[i + num] = Properties[2 * num - i];
                Properties[2 * num - i] = temp1;
            }
        }
    }
    public override string ToString() => GeoInferenceApp.IsZhOrEn
    ? $"四边形{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}与四边形{Properties[4]}{Properties[5]}{Properties[6]}{Properties[7]}全等"
    : $"Quadrilateral {Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]} is congruent to Quadrilateral {Properties[4]}{Properties[5]}{Properties[6]}{Properties[7]}";
}
