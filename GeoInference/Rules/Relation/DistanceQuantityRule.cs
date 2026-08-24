
[RuleType(RuleType.Tradition)]
public class DistanceQuantityRule : RuleClass
{
    [Alias("共线线段的长度关系")]
    public void RuleDQ001CollinearSegmentLengthRelation(Line line)
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
                    var whole = GetSegment(head, tail);
                    var s1 = GetSegment(mid, head);
                    var s2 = GetSegment(tail, mid);
                    var pred = new LinearEquation(new() { { s1.Length, 1 }, { s2.Length, 1 }, { whole.Length, -1 } }, 0);
                    pred.AddReason();
                    pred.AddCondition(line);
                    updater.Add(pred);
                }
            }
        }
    }

    [Alias("正方形四边等长")]
    public void RuleDQ002SquareSideLengths(SquareEdgeLength edgeLength)
    {
        var square = edgeLength[0];
        var s1 = GetSegment(square[0], square[1]);
        var s2 = GetSegment(square[1], square[2]);
        var s3 = GetSegment(square[2], square[3]);
        var s4 = GetSegment(square[3], square[0]);
        SegmentLength pred1 = new SegmentLength(s1, edgeLength.Expr);
        pred1.AddReason();
        pred1.AddCondition(edgeLength);
        updater.Add(pred1);
        SegmentLength pred2 = new SegmentLength(s2, edgeLength.Expr);
        pred2.AddReason();
        pred2.AddCondition(edgeLength);
        updater.Add(pred2);
        SegmentLength pred3 = new SegmentLength(s3, edgeLength.Expr);
        pred3.AddReason();
        pred3.AddCondition(edgeLength);
        updater.Add(pred3);
        SegmentLength pred4 = new SegmentLength(s4, edgeLength.Expr);
        pred4.AddReason();
        pred4.AddCondition(edgeLength);
        updater.Add(pred4);
    }

    [Alias("平行线间的距离处处相等")]
    public void RuleDQ003ParallelLineDistance(LineParallel lineParallel)
    {
        var ltl = GetLineToLineDistance(lineParallel.Line1, lineParallel.Line2);
        if (ltl is null) return;
        foreach (var item in lineParallel.Line1.Points)
        {
            var ptl = GetPointToLineDistance(item, lineParallel.Line2);
            if (ptl is not null)
            {
                QuantityRatio pred = new QuantityRatio(ptl, ltl);
                pred.AddReason();
                pred.AddCondition(lineParallel);
                updater.Add(pred);
            }
        }
        foreach (var item in lineParallel.Line2.Points)
        {
            var ptl = GetPointToLineDistance(item, lineParallel.Line1);
            if (ptl is not null)
            {
                QuantityRatio pred = new QuantityRatio(ptl, ltl);
                pred.AddReason();
                pred.AddCondition(lineParallel);
                updater.Add(pred);
            }
        }
    }

    [Alias("垂直平分线推出垂直")]
    public void RuleDQ004PerpendicularBisectorYieldsPerpendicularity(VerticalBisectorLine verticalBisectorLine)
    {
        var line = GetLine(verticalBisectorLine[0][0], verticalBisectorLine[0][1]);
        LinePerpendicular pred = new LinePerpendicular(line, (Line)verticalBisectorLine[1]);
        pred.AddReason();
        pred.AddCondition(verticalBisectorLine);
        updater.Add(pred);
    }

    [Alias("垂直平分线上的点到线段两端距离相等")]
    public void RuleDQ005EquidistantFromEndpointsOnPerpendicularBisector(VerticalBisectorLine verticalBisectorLine)
    {
        foreach (var item in verticalBisectorLine[1].Properties)
        {
            var s1 = GetSegment(verticalBisectorLine[0][0], item);
            var s2 = GetSegment(verticalBisectorLine[0][1], item);
            if (s1 is null || s2 is null) continue;
            QuantityRatio pred = new QuantityRatio(s1.Length, s2.Length);
            pred.AddReason();
            pred.AddCondition(verticalBisectorLine);
            updater.Add(pred);
        }
    }


    [SemiConditionRule]
    [Alias("角平分线上的点到角两边距离相等")]
    public void RuleDQ006AngleBisectorEquidistantToSides(AngularBisectorLine bisectorLine)
    {
        var line = bisectorLine.Bisector;
        var angle = bisectorLine.Angle;
        var edge1 = GetLine(bisectorLine.Angle.Vertex, bisectorLine.Angle.Edge1[0]);
        var edge2 = GetLine(bisectorLine.Angle.Vertex, bisectorLine.Angle.Edge2[0]);
        foreach (var point in line.Properties)
        {
            if (point == bisectorLine.Angle.Vertex) continue;
            foreach (var e1 in edge1.Properties)
            {
                if (e1 == bisectorLine.Angle.Vertex) continue;
                foreach (var e2 in edge2.Properties)
                {
                    if (e2 == bisectorLine.Angle.Vertex) continue;
                    var line1 = GetLine(e1, point);
                    var line2 = GetLine(e2, point);
                    if (line1 is not null && line2 is not null)
                    {
                        var oa = GetSegment(bisectorLine.Angle.Vertex, e1);
                        var ob = GetSegment(bisectorLine.Angle.Vertex, e2);

                        var pa = GetSegment(point, e1);
                        var pb = GetSegment(point, e2);

                        CondictionalKnowledge c1 = new() { Knowledge = new QuantityRatio(oa.Length, ob.Length) };
                        c1.AddCondiction(new LinePerpendicular(edge1, line1), new LinePerpendicular(edge2, line2));
                        c1.Knowledge.AddReason();
                        c1.Knowledge.AddCondition(bisectorLine);
                        updater.AddCondictionalKnowledgePair(c1);
                        CondictionalKnowledge c2 = new() { Knowledge = new QuantityRatio(pa.Length, pb.Length) };
                        c2.AddCondiction(new LinePerpendicular(edge1, line1), new LinePerpendicular(edge2, line2));
                        c2.Knowledge.AddReason();
                        c2.Knowledge.AddCondition(bisectorLine);
                        updater.AddCondictionalKnowledgePair(c2);
                    }
                }
            }
        }
    }

    [Alias("平行线分线段成比例定理")]
    public void RuleDQ007ProportionalSegmentsOnTransversals(LineParallel paralle1, LineParallel paralle2, Line line1, Line line2)
    {
        if (paralle1 == paralle2) return;
        var (cc, nc1, nc2) = FindCommon(paralle1, paralle2);
        if (cc is null) return;
        var a = FindCIntersection(cc, line1);
        var b = FindCIntersection(nc1, line1);
        var c = FindCIntersection(nc2, line1);
        var d = FindCIntersection(cc, line2);
        var e = FindCIntersection(nc1, line2);
        var f = FindCIntersection(nc2, line2);
        if (a is null || b is null || c is null || d is null || e is null || f is null) return;
        var abS = GetSegment(a, b);
        var bcS = GetSegment(b, c);
        var caS = GetSegment(c, a);

        var deS = GetSegment(d, e);
        var efS = GetSegment(e, f);
        var fdS = GetSegment(f, d);
        var pred1 = new ProductionEquation(1, [(abS.Length, 1), (efS.Length, 1), (deS.Length, -1), (bcS.Length, -1)]);
        pred1.AddReason();
        pred1.AddCondition(paralle1, paralle2);
        updater.Add(pred1);
        var pred2 = new ProductionEquation(1, [(bcS.Length, 1), (fdS.Length, 1), (efS.Length, -1), (caS.Length, -1)]);
        pred2.AddReason();
        pred2.AddCondition(paralle1, paralle2);
        updater.Add(pred2);
        var pred3 = new ProductionEquation(1, [(caS.Length, 1), (deS.Length, 1), (fdS.Length, -1), (abS.Length, -1)]);
        pred3.AddReason();
        pred3.AddCondition(paralle1, paralle2);
        updater.Add(pred3);
    }
}
