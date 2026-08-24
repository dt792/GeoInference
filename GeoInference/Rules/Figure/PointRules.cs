
[RuleType(RuleType.Tradition)]
internal class PointRules : RuleClass
{
    [Alias("线段中点的性质")]
    public void RuleSeg001IfMIsMidpointOfABThenMAEqualsMBEqualsHalfAB(Midpoint midpoint)
    {
        Segment segment1 = GetSegment(midpoint[1], midpoint[2]) as Segment;
        Segment segment2 = GetSegment(midpoint[0], midpoint[1]) as Segment;
        Segment segment3 = GetSegment(midpoint[0], midpoint[2]) as Segment;

        QuantityRatio segmentLengthEqual = new QuantityRatio(segment2.Length, segment3.Length);
        QuantityRatio lengthRatio1 = new QuantityRatio(segment1.Length, segment2.Length, 2);
        QuantityRatio lengthRatio2 = new QuantityRatio(segment1.Length, segment3.Length, 2);

        segmentLengthEqual.AddReason();
        segmentLengthEqual.AddCondition(midpoint);
        updater.Add(segmentLengthEqual);
        lengthRatio1.AddReason();
        lengthRatio1.AddCondition(midpoint);
        updater.Add(lengthRatio1);
        lengthRatio2.AddReason();
        lengthRatio2.AddCondition(midpoint);
        updater.Add(lengthRatio2);
    }

    [Alias("点相对于直线的同侧判定")]
    public void RulePoint001DetermineIfPointsAreOnSameSide(Point p1, Point p2, Point p3, Point p4)
    {
        if (p1 == p2 || p2 == p3 || p3 == p4 || p1 == p3 || p1 == p4 || p2 == p4) return;
        var judge = GetJudgeIpsilateral(p1, p2, p3, p4);
        if (judge is not null)
            updater.Add(judge);
    }

    [Alias("线段中点的判定（一）")]
    public void RuleSeg002MidpointDetermination1(SegmentLengthEqual equal, PointOnSeg pointWithIn)
    {
        var (c, nc1, nc2) = FindCommon(equal[0], equal[1]);
        if (c is not null)
        {
            if (pointWithIn[0] == c && pointWithIn[1] == nc1 && pointWithIn[2] == nc2 ||
                pointWithIn[0] == c && pointWithIn[1] == nc2 && pointWithIn[2] == nc1)
            {
                Midpoint pred = new Midpoint((Point)pointWithIn[0], (Point)pointWithIn[1], (Point)pointWithIn[2]);
                pred.AddReason();
                pred.AddCondition(equal, pointWithIn);
                updater.Add(pred);
            }
        }
    }

    [Alias("线段中点的判定（二）")]
    public void RuleSeg003MidpointDetermination2(SegmentLengthRatio ratio, PointOnSeg pointWithIn)
    {
        var ab = GetSegment(pointWithIn[0], pointWithIn[1]);
        var ac = GetSegment(pointWithIn[0], pointWithIn[2]);
        var bc = GetSegment(pointWithIn[1], pointWithIn[2]);
        if (ratio[0] == bc)
        {
            if (ratio[1] == ab || ratio[1] == ac)
            {
                if (ratio.Expr == 2)
                {
                    Midpoint pred = new Midpoint((Point)pointWithIn[0], (Point)pointWithIn[1], (Point)pointWithIn[2]);
                    pred.AddReason();
                    pred.AddCondition(ratio, pointWithIn);
                    updater.Add(pred);
                }
            }
        }
        else if (ratio[1] == bc)
        {
            if (ratio[0] == ab || ratio[0] == ac)
            {
                if (ratio.Expr == Expr.Half)
                {
                    Midpoint pred = new Midpoint((Point)pointWithIn[0], (Point)pointWithIn[1], (Point)pointWithIn[2]);
                    pred.AddReason();
                    pred.AddCondition(ratio, pointWithIn);
                    updater.Add(pred);
                }
            }
        }
    }

    [Alias("垂直推出垂足")]
    public void RulePoint002LinePerpendicularInfersPedal(LinePerpendicular perpendicular)
    {
        var cross = FindCIntersection(perpendicular.Line1, perpendicular.Line2);
        if (cross is not null)
        {
            Predicate pred = new Pedal((Point)cross, perpendicular.Line1, perpendicular.Line2);
            pred.AddReason();
            pred.AddCondition(perpendicular);
            updater.Add(pred);
        }
    }

    [Alias("垂直平分线的判定")]
    public void RulePoint003PerpendicularBisectorDetermination(LinePerpendicular perpendicular, Midpoint midpoint)
    {
        var line = GetLine(midpoint[1], midpoint[2]);
        if (perpendicular.Line1 == line && perpendicular.Line2.Contains(midpoint[0]))
        {
            var seg = GetSegment(midpoint[1], midpoint[2]);
            VerticalBisectorLine pred = new VerticalBisectorLine(seg, perpendicular.Line2);
            pred.AddReason();
            pred.AddCondition(perpendicular);
            updater.Add(pred);
        }
        else if (perpendicular.Line2 == line && perpendicular.Line1.Contains(midpoint[0]))
        {
            var seg = GetSegment(midpoint[1], midpoint[2]);
            VerticalBisectorLine pred = new VerticalBisectorLine(seg, perpendicular.Line1);
            pred.AddReason();
            pred.AddCondition(perpendicular);
            updater.Add(pred);
        }
    }

    [Alias("点介于两点之间的判定")]
    public void RuleSeg004PointBetweenTwoPointsDetermination(Line line)
    {
        for (int i = 0; i < line.Properties.Count; i++)
        {
            for (int j = i + 1; j < line.Properties.Count; j++)
            {
                for (int k = j + 1; k < line.Properties.Count; k++)
                {
                    var head = line.Properties[i];
                    var tail = line.Properties[k];
                    var mid = line.Properties[j];
                    PointOnSeg pred = new PointOnSeg((Point)mid, (Point)head, (Point)tail);
                    pred.AddReason();
                    pred.AddCondition(line);
                    updater.Add(pred);
                }
            }
        }
    }

    [Alias("点在线段上的分段长度关系")]
    public void RuleSeg005PointOnSegmentLengthRelation(PointOnSeg p)
    {
        var ab = GetSegment(p.Point, p.EndPoint1);
        var bc = GetSegment(p.Point, p.EndPoint2);
        var ac = GetSegment(p.EndPoint1, p.EndPoint2);
        LinearEquation pred = new LinearEquation(new() { { ab.Length, 1 }, { bc.Length, 1 }, { ac.Length, -1 } }, 0);
        pred.AddReason();
        pred.AddCondition(p);
        updater.Add(pred);
    }

    [Alias("直角三角形斜边中线等于斜边的一半")]
    public void RuleSeg006MidpointOfHypotenuseOfRightTriangleConnectedToVertexIsHalfOfHypotenuse(RightTriangle rightTriangle, Midpoint midpoint)
    {
        bool flag = false;
        if (rightTriangle[1] == midpoint[1] && rightTriangle[2] == midpoint[2])
        {
            flag = true;
        }
        else if (rightTriangle[1] == midpoint[2] && rightTriangle[2] == midpoint[1])
        {
            flag = true;
        }
        if (!flag) return;

        Segment half = GetSegment(rightTriangle[0], midpoint[0]) as Segment;
        Segment side = GetSegment(rightTriangle[1], rightTriangle[2]) as Segment;
        if (half is null) return;
        QuantityRatio pred = new QuantityRatio(side.Length, half.Length, 2);

        pred.AddReason();
        pred.AddCondition(rightTriangle, midpoint);
        updater.Add(pred);
    }

}
