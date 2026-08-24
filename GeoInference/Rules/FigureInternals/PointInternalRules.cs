
[RuleType(RuleType.Internal)]

public class PointInternalRules : RuleClass
{
    [Alias("中点推出点在线段上")]
    public void RulePointInternal001MidpointImpliesBetweenness(Midpoint midpoint)
    {
        PointOnSeg pred = new PointOnSeg((Point)midpoint[0], (Point)midpoint.EndPoint1, (Point)midpoint.EndPoint2);
        pred.AddReason();
        pred.AddCondition(midpoint);
        updater.Add(pred);
    }

    [Alias("三等分点推出点在线段上")]
    public void RulePointInternal002TrisectionPointImpliesBetweenness(TrisectionPoint trisectionPoint)
    {
        PointOnSeg pred1 = new PointOnSeg((Point)trisectionPoint[0], (Point)trisectionPoint[2], (Point)trisectionPoint[3]);
        PointOnSeg pred2 = new PointOnSeg((Point)trisectionPoint[1], (Point)trisectionPoint[2], (Point)trisectionPoint[3]);
        pred1.AddReason();
        pred1.AddCondition(trisectionPoint);
        updater.Add(pred1);
        pred2.AddReason();
        pred2.AddCondition(trisectionPoint);
        updater.Add(pred2);
    }

    [Alias("垂足推出直线相交")]
    public void RulePointInternal003PedalYieldsLineIntersection(Pedal pedal)
    {
        LineIntersection pred = new LineIntersection((Point)pedal[0], (Line)pedal[1], (Line)pedal[2]);
        pred.AddReason();
        pred.AddCondition(pedal);
        updater.Add(pred);
    }

    [Alias("垂足推出直线垂直")]
    public void RulePointInternal004PedalYieldsPerpendicularity(Pedal pedal)
    {
        LinePerpendicular pred = new LinePerpendicular((Line)pedal[1], (Line)pedal[2]);
        pred.AddReason();
        pred.AddCondition(pedal);
        updater.Add(pred);
    }

}
