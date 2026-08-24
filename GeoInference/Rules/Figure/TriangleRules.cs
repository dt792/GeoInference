
[RuleType(RuleType.Tradition)]
internal class TriangleRules : RuleClass
{
    [Alias("等边三角形的性质")]
    public void RuleTriAttri01EquilateralTriangleProperties(EquilateralTriangle tri)
    {
        var abS = GetSegment(tri[0], tri[1]);
        var bcS = GetSegment(tri[1], tri[2]);
        var caS = GetSegment(tri[2], tri[0]);

        var abcA = GetAngle(tri[0], tri[1], tri[2]);
        var bcaA = GetAngle(tri[1], tri[2], tri[0]);
        var cabA = GetAngle(tri[2], tri[0], tri[1]);

        QuantityRatio pred;
        pred = new QuantityRatio(abS.Length, bcS.Length);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
        pred = new QuantityRatio(bcS.Length, caS.Length);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
        pred = new QuantityRatio(caS.Length, abS.Length);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);

        QuantityValue qv;
        qv = new QuantityValue(abcA.Size, 60);
        qv.AddReason();
        qv.AddCondition(tri);
        updater.Add(qv);
        qv = new QuantityValue(bcaA.Size, 60);
        qv.AddReason();
        qv.AddCondition(tri);
        updater.Add(qv);
        qv = new QuantityValue(cabA.Size, 60);
        qv.AddReason();
        qv.AddCondition(tri);
        updater.Add(qv);

        AngleCos ac;
        ac = new AngleCos(abcA, Expr.Half);
        ac.AddReason();
        ac.AddCondition(tri);
        updater.Add(ac);
        ac = new AngleCos(bcaA, Expr.Half);
        ac.AddReason();
        ac.AddCondition(tri);
        updater.Add(ac);
        ac = new AngleCos(cabA, Expr.Half);
        ac.AddReason();
        ac.AddCondition(tri);
        updater.Add(ac);

        pred = new QuantityRatio(abcA.Size, bcaA.Size);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
        pred = new QuantityRatio(bcaA.Size, cabA.Size);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
        pred = new QuantityRatio(cabA.Size, abcA.Size);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
    }

    [Alias("等腰三角形的性质")]
    public void RuleTriAttri02IsoscelesTriangleProperties(IsoscelesTriangle tri)
    {
        var abS = GetSegment(tri[0], tri[1]);
        var caS = GetSegment(tri[2], tri[0]);

        var abcA = GetAngle(tri[0], tri[1], tri[2]);
        var bcaA = GetAngle(tri[1], tri[2], tri[0]);

        QuantityRatio pred;
        pred = new QuantityRatio(caS.Length, abS.Length);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);

        pred = new QuantityRatio(abcA.Size, bcaA.Size);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
    }

    [Alias("直角三角形的性质")]
    public void RuleTriAttri03RightTriangleProperties(RightTriangle tri)
    {
        var abL = GetLine(tri[0], tri[1]);
        var acL = GetLine(tri[0], tri[2]);

        var abS = GetSegment(tri[0], tri[1]);
        var bcS = GetSegment(tri[1], tri[2]);
        var caS = GetSegment(tri[2], tri[0]);

        var abcA = GetAngle(tri[0], tri[1], tri[2]);
        var bcaA = GetAngle(tri[1], tri[2], tri[0]);
        var cabA = GetAngle(tri[2], tri[0], tri[1]);

        LinePerpendicular lp = new LinePerpendicular(abL, acL);
        lp.AddReason();
        lp.AddCondition(tri);
        updater.Add(lp);

        QuantityValue qv = new QuantityValue(cabA.Size, 90);
        qv.AddReason();
        qv.AddCondition(tri);
        updater.Add(qv);

        Equation eq = new Equation(abS.Length.Pow(2) + caS.Length.Pow(2), bcS.Length.Pow(2));
        eq.AddReason();
        eq.AddCondition(tri);
        updater.Add(eq);

        LinearEquation le = new LinearEquation(new() { { abcA.Size, 1 }, { bcaA.Size, 1 } }, 90);
        le.AddReason();
        le.AddCondition(tri);
        updater.Add(le);

        Pedal pedal = new Pedal((Point)tri[0], abL, acL);
        pedal.AddReason();
        pedal.AddCondition(tri);
        updater.Add(pedal);
    }

    [Alias("等腰三角形的判定（两边相等）")]
    public void RuleTg01IsoscelesTriangleSideEqualityDetermination(SegmentLengthEqual equal)
    {
        var result = this.FindCommon(equal.Seg1, equal.Seg2);
        if (result.common is null) return;
        if (GetTriangle((Point)result.common, (Point)result.notcommon1, (Point)result.notcommon2) is null) return;

        IsoscelesTriangle pred = new IsoscelesTriangle((Point)result.common, (Point)result.notcommon1, (Point)result.notcommon2);
        pred.AddReason();
        pred.AddCondition(equal);
        updater.Add(pred);
    }

    [Alias("等腰三角形的判定（两角相等）")]
    public void RuleTg02IsoscelesTriangleAngleEqualityDetermination(AngleSizeEqual equal)
    {
        var l11 = GetLine(equal.Angle1.Vertex, equal.Angle1.Edge1[0]);
        var l12 = GetLine(equal.Angle1.Vertex, equal.Angle1.Edge2[0]);
        var l21 = GetLine(equal.Angle2.Vertex, equal.Angle2.Edge1[0]);
        var l22 = GetLine(equal.Angle2.Vertex, equal.Angle2.Edge2[0]);
        var result = this.FindCommon(l11, l12, l21, l22);
        if (result.common is null) return;
        var inter = FindCIntersection(result.notcommon1, result.notcommon2);
        if (inter is null) return;
        if (GetTriangle((Point)equal.Angle1.Vertex, (Point)equal.Angle2.Vertex, (Point)inter) is null) return;

        IsoscelesTriangle pred = new IsoscelesTriangle((Point)inter, equal.Angle1.Vertex, (Point)equal.Angle2.Vertex);
        pred.AddReason();
        pred.AddCondition(equal);
        updater.Add(pred);
    }

    [Alias("等边三角形的判定（三边相等）")]
    public void RuleTg03EquilateralTriangleThreeSideLengthsDetermination(SegmentLengthEqual equal1, SegmentLengthEqual equal2)
    {
        if (equal1 == equal2) return;
        var result = FindCommon(equal1, equal2);
        if (result.common is null) return;
        var d = result.common.Properties.Concat(result.notcommon1.Properties).Concat(result.notcommon2.Properties);
        var pointNum = ZDict.CountItemNum(d);
        var list = pointNum.ToList();
        if (pointNum.Count == 3 && list.TrueForAll(kv => kv.Value == 2))
        {
            if (GetTriangle((Point)list[0].Key, (Point)list[1].Key, (Point)list[2].Key) is null) return;
            EquilateralTriangle pred = new EquilateralTriangle((Point)list[0].Key, (Point)list[1].Key, (Point)list[2].Key);
            pred.AddReason();
            pred.AddCondition(equal1, equal2);
            updater.Add(pred);
        }
    }

    [Alias("等边三角形的判定（60°角）")]
    public void RuleTg04EquilateralTriangle60DegreeAngleDetermination(IsoscelesTriangle triangle, AngleSize angleValue)
    {
        if (!angleValue[0].Properties.TrueForAll(p => triangle.Properties.Contains(p))) return;
        if (angleValue.Expr != "60") return;

        EquilateralTriangle pred = new EquilateralTriangle(triangle[0] as Point, triangle[1] as Point, triangle[2] as Point);
        pred.AddReason();
        pred.AddCondition(triangle, angleValue);
        updater.Add(pred);
    }

    [Alias("直角三角形的判定（90°角）")]
    public void RuleTg05RightTriangle90DegreeAngleDetermination(AngleSize angleValue)
    {
        if (angleValue.Expr != "90") return;
        foreach (var p1 in angleValue.Angle.Edge1)
        {
            foreach (var p2 in angleValue.Angle.Edge2)
            {
                if (GetTriangle(angleValue.Angle.Vertex, p1, p2) is null) continue;
                RightTriangle pred = new RightTriangle(angleValue.Angle.Vertex, p1, p2);
                pred.AddReason();
                pred.AddCondition(angleValue);
                updater.Add(pred);
            }
        }
    }

    [Alias("直角三角形的判定（两边垂直）")]
    public void RuleTg06RightTriangleLinePerpendicularityDetermination(LinePerpendicular perpendicular)
    {
        Point intersection = (Point)perpendicular.Line1.Properties.FirstOrDefault(p => perpendicular.Line2.Properties.Contains(p));
        if (intersection is not null)
        {
            foreach (var p1 in perpendicular.Line1.Properties)
            {
                if (p1 != intersection)
                {
                    foreach (var p2 in perpendicular.Line2.Properties)
                    {
                        if (p2 != intersection)
                        {
                            if (GetTriangle(intersection, (Point)p1, (Point)p2) is null) continue;
                            RightTriangle pred = new RightTriangle(intersection, (Point)p1, (Point)p2);
                            pred.AddReason();
                            pred.AddCondition(perpendicular);
                            updater.Add(pred);
                        }
                    }
                }
            }
        }
    }

    [Alias("直角三角形的判定（勾股定理逆定理）")]
    public void RuleTg07RightTriangleThreeSideLengthsDetermination(SegmentLength length1, SegmentLength length2, SegmentLength length3)
    {
        if (length1 == length2 || length1 == length3 || length2 == length3) return;

        Point p1, p2, p3;

        Dictionary<Point, int> pointCount = new Dictionary<Point, int>();
        List<SegmentLength> lengths = new List<SegmentLength>() { length1, length2, length3, };
        foreach (var length in lengths)
        {
            foreach (Point point in length.Properties[0].Properties)
            {
                if (pointCount.ContainsKey(point))
                {
                    pointCount[point]++;
                }
                else
                {
                    pointCount.Add(point, 1);
                }
            }
        }
        if (pointCount.Count != 3 || !pointCount.ToList().TrueForAll(kv => kv.Value == 2))
            return;

        SegmentLength longLength = length1;
        SegmentLength shortLength1 = length2;
        SegmentLength shortLength2 = length3;
        if (length1.Expr > length2.Expr)
        {
            if (length1.Expr < length3.Expr)
            {
                longLength = length3;
                shortLength1 = length1;
                shortLength2 = length2;
            }
        }
        else
        {
            if (length2.Expr > length3.Expr)
            {
                longLength = length2;
                shortLength1 = length1;
                shortLength2 = length3;
            }
            else
            {
                longLength = length3;
                shortLength1 = length1;
                shortLength2 = length2;
            }
        }

        var a = (shortLength1.Expr * shortLength1.Expr + shortLength2.Expr * shortLength2.Expr);
        var b = longLength.Expr * longLength.Expr;
        if (a != b)
            return;

        p1 = pointCount.First(kv => !longLength.Properties[0].Properties.Contains(kv.Key)).Key as Point;
        p2 = longLength.Properties[0][0] as Point;
        p3 = longLength.Properties[0][1] as Point;
        if (GetTriangle(p1, p2, p3) is null) return;

        RightTriangle pred = new RightTriangle(p1, p2, p3);
        pred.AddReason();
        pred.AddCondition(length1, length2, length3);
        updater.Add(pred);
    }

    [Alias("等腰直角三角形的判定")]
    public void RuleTg08IsoscelesRightTriangleDetermination(IsoscelesTriangle iso, RightTriangle rt)
    {
        if (iso[0] == rt[0] && iso[1] == rt[1] && iso[2] == rt[2])
        {
            IsoscelesRightTriangle pred = new IsoscelesRightTriangle((Point)iso[0], (Point)iso[1], (Point)iso[2]);
            pred.AddReason();
            pred.AddCondition(iso, rt);
            updater.Add(pred);
        }
    }

    [Alias("三角形中位线定理")]
    public void RuleLR01MidsegmentTheorem(Midpoint md1, Midpoint md2)
    {
        if (md1 == md2) return;

        Point m1 = (Point)md1[0], a1 = (Point)md1[1], b1 = (Point)md1[2];
        Point m2 = (Point)md2[0], a2 = (Point)md2[1], b2 = (Point)md2[2];
        if (m1 == m2) return;
        Point p1 = null; Point p2 = null;
        if (a1 == a2)
        {
            p1 = b1;
            p2 = b2;
            if (HasColine(a1, p1, p2))
                return;
        }
        else if (a1 == b2)
        {
            p1 = b1;
            p2 = a2;
            if (HasColine(a1, p1, p2))
                return;
        }
        else if (a2 == b1)
        {
            p1 = a1;
            p2 = b2;
            if (HasColine(a2, p1, p2))
                return;
        }
        else if (b1 == b2)
        {
            p1 = a1;
            p2 = a2;
            if (HasColine(b1, p1, p2))
                return;
        }
        else return;

        var line1 = GetLine(m1, m2) as Line;
        var line2 = GetLine(p1, p2) as Line;

        var s1 = GetSegment(m1, m2) as Segment;
        var s2 = GetSegment(p1, p2) as Segment;
        if (line1 is null || line2 is null) return;

        LineParallel pred1 = new LineParallel(line1, line2);
        QuantityRatio pred2 = new QuantityRatio(s1.Length, s2.Length, Expr.Half);
        pred1.AddReason();
        pred2.AddReason();
        pred1.AddCondition(md1, md2);
        pred2.AddCondition(md1, md2);
        updater.Add(pred1);
        updater.Add(pred2);
    }

    [Alias("三角形中位线定理的逆定理")]
    public void RuleLR02ConverseOfMidsegmentTheorem(PointOnSeg pwithin, Midpoint md1, LineParallel parallel)
    {
        Point m1 = (Point)md1[0], a1 = (Point)md1[1], b1 = (Point)md1[2];
        Point m2 = (Point)pwithin[0], a2 = (Point)pwithin[1], b2 = (Point)pwithin[2];
        if (m1 == m2) return;
        Point p1 = null; Point p2 = null;
        if (a1 == a2)
        {
            p1 = b1;
            p2 = b2;
        }
        else if (a1 == b2)
        {
            p1 = b1;
            p2 = a2;
        }
        else if (a2 == b1)
        {
            p1 = a1;
            p2 = b2;
        }
        else if (b1 == b2)
        {
            p1 = a1;
            p2 = a2;
        }
        else return;

        var line1 = GetLine(m1, m2) as Line;
        var line2 = GetLine(p1, p2) as Line;
        if (line1 is null || line2 is null) return;
        if (line1 == parallel.Line1 && line2 == parallel.Line2 ||
            line1 == parallel.Line2 && line2 == parallel.Line1)
        {
            Midpoint pred = new Midpoint((Point)pwithin[0], (Point)pwithin[1], (Point)pwithin[2]);
            pred.AddReason();
            pred.AddCondition(md1, pwithin, parallel);
            updater.Add(pred);
        }
    }

    [Alias("等腰三角形底边中线的性质")]
    public void RuleTr01IsoscelesTriangleMedianTheorem(IsoscelesTriangle triangle, Midpoint midpoint)
    {
        Line line1;
        Line line2;

        if (triangle[1] == midpoint[1] && triangle[2] == midpoint[2])
        {
            line1 = GetLine(triangle[0], midpoint[0]);
            line2 = GetLine(midpoint[1], midpoint[2]);
        }
        else
            return;
        if (line1 is null || line2 is null)
            return;

        LinePerpendicular pred = new LinePerpendicular(line1, line2);
        pred.AddReason();
        pred.AddCondition(triangle, midpoint);
        updater.Add(pred);

        var angle = GetAngle(triangle[1], triangle[0], triangle[2]);
        AngularBisectorLine pred2 = new AngularBisectorLine(angle, line1);
        pred2.AddReason();
        pred2.AddCondition(triangle, midpoint);
        updater.Add(pred2);
    }

    [Alias("等腰三角形三线合一（顶角平分线）")]
    public void RuleTr02IsoscelesTriangleThreeLinesCoincideTheorem1(IsoscelesTriangle triangle, AngularBisectorLine angleBisector)
    {
        var angle = GetAngle(triangle[1], triangle[0], triangle[2]);
        if (angle != angleBisector[0]) return;

        Line line2 = GetLine(triangle[1], triangle[2]);
        LinePerpendicular pred = new LinePerpendicular((Line)angleBisector[1], line2);
        pred.AddReason();
        pred.AddCondition(triangle, angleBisector);
        updater.Add(pred);
        var c = FindCIntersection((Line)angleBisector[1], line2);
        if (c is not null)
        {
            Midpoint pred1 = new Midpoint((Point)c, (Point)triangle[1], (Point)triangle[2]);
            pred1.AddReason();
            pred1.AddCondition(triangle, angleBisector);
            updater.Add(pred1);
        }
    }

    [Alias("等腰三角形三线合一（底边上的高）")]
    public void RuleTr03IsoscelesTriangleThreeLinesCoincideTheorem2(IsoscelesTriangle triangle, LinePerpendicular perpendicular)
    {
        var line2 = GetLine(triangle[1], triangle[2]);

        if (line2 == perpendicular.Line1 && perpendicular.Line2.Contains(triangle[0]))
        {
            var angle = GetAngle(triangle[1], triangle[0], triangle[2]);
            AngularBisectorLine pred2 = new AngularBisectorLine(angle, perpendicular.Line2);
            pred2.AddReason();
            pred2.AddCondition(triangle, perpendicular);
            updater.Add(pred2);
            var c = FindCIntersection(perpendicular.Line1, perpendicular.Line2);
            if (c is not null)
            {
                Midpoint pred1 = new Midpoint((Point)c, (Point)triangle[1], (Point)triangle[2]);
                pred1.AddReason();
                pred1.AddCondition(triangle, perpendicular);
                updater.Add(pred1);
            }
        }
        else if (line2 == perpendicular.Line2 && perpendicular.Line1.Contains(triangle[0]))
        {
            var angle = GetAngle(triangle[1], triangle[0], triangle[2]);
            AngularBisectorLine pred2 = new AngularBisectorLine(angle, perpendicular.Line1);
            pred2.AddReason();
            pred2.AddCondition(triangle, perpendicular);
            updater.Add(pred2);
            var c = FindCIntersection(perpendicular.Line1, perpendicular.Line2);
            if (c is not null)
            {
                Midpoint pred1 = new Midpoint((Point)c, (Point)triangle[1], (Point)triangle[2]);
                pred1.AddReason();
                pred1.AddCondition(triangle, perpendicular);
                updater.Add(pred1);
            }
        }
    }

    [Alias("直角三角形由角求三边之比")]
    public void RuleTr04CalculateThreeSideRatiosFromOneAngleInRightTriangle(RightTriangle triangle, AngleCos angleValue)
    {
        return;
    }

    [Alias("三角形内角和定理")]
    public void RuleTriGeoQuantity01TriangleInteriorAngleSumFormula(Triangle triangle)
    {
        var angle1 = GetAngle(triangle[0], triangle[1], triangle[2]);
        var angle2 = GetAngle(triangle[1], triangle[2], triangle[0]);
        var angle3 = GetAngle(triangle[2], triangle[0], triangle[1]);
        Equation pred = new LinearEquation(new() { { angle1.Size, 1 }, { angle2.Size, 1 }, { angle3.Size, 1 } }, 180);
        pred.AddReason();
        pred.AddCondition(triangle);
        updater.Add(pred);
    }

    [Alias("直角三角形30°角的边长关系")]
    public void RuleTriGeoQuantity02RightTriangle30DegreeAngleSideLengthRelation(RightTriangle triangle)
    {
        var abc = GetAngle(triangle[0], triangle[1], triangle[2]);
        var acb = GetAngle(triangle[0], triangle[2], triangle[1]);
        var ab = GetSegment(triangle[0], triangle[1]);
        var bc = GetSegment(triangle[1], triangle[2]);
        var ac = GetSegment(triangle[0], triangle[2]);
        Equation pred = new ProductionEquation(new() { { ac.Length, 1 }, { abc.Sin, -1 }, { bc.Length, -1 } }, 1);
        pred.AddReason();
        pred.AddCondition(triangle);
        Equation pred1 = new ProductionEquation(new() { { ab.Length, 1 }, { acb.Sin, -1 }, { bc.Length, -1 } }, 1);
        pred1.AddReason();
        pred1.AddCondition(triangle);
        updater.Add(pred);
        updater.Add(pred1);
    }

    [Alias("直角三角形的正切关系")]
    public void RuleTriGeoQuantity03RightTriangleTan(RightTriangle tri)
    {
        var c = GetSegment(tri[0], tri[1]);
        var a = GetSegment(tri[1], tri[2]);
        var b = GetSegment(tri[2], tri[0]);
        var B = GetAngle(tri[0], tri[1], tri[2]);
        var C = GetAngle(tri[1], tri[2], tri[0]);
        var A = GetAngle(tri[2], tri[0], tri[1]);
        Equation pred = new ProductionEquation(new() { { c.Length, 1 }, { b.Length, -1 }, { C.Tan, -1 } }, 1);
        pred.AddReason();
        pred.AddCondition(tri);
        Equation pred1 = new ProductionEquation(new() { { b.Length, 1 }, { c.Length, -1 }, { B.Tan, -1 } }, 1);
        pred1.AddReason();
        pred1.AddCondition(tri);
        updater.Add(pred);
        updater.Add(pred1);
    }

    [Alias("勾股定理")]
    public void RuleTriGeoQuantity04RightTriangleSideLengthRelation(RightTriangle triangle)
    {
        var ab = GetSegment(triangle[0], triangle[1]);
        var ac = GetSegment(triangle[0], triangle[2]);
        var bc = GetSegment(triangle[1], triangle[2]);
        Equation pred = new Equation(ab.Length.Pow(2) + ac.Length.Pow(2), bc.Length.Pow(2));
        pred.AddReason();
        pred.AddCondition(triangle);
        updater.Add(pred);
    }

    [Alias("三角形面积公式")]
    public void RuleTriGeoQuantity05TriangleAreaFormula(Triangle triangle, Pedal pedal)
    {
        Segment bottom = null, height = null;
        if (pedal[1].Contains(triangle[0]) && pedal[1].Contains(triangle[1]) && pedal[2].Contains(triangle[2]))
        {
            bottom = GetSegment(triangle[0], triangle[1]);
            height = GetSegment(pedal[0], triangle[2]);
        }
        else if (pedal[2].Contains(triangle[0]) && pedal[2].Contains(triangle[1]) && pedal[1].Contains(triangle[2]))
        {
            bottom = GetSegment(triangle[0], triangle[1]);
            height = GetSegment(pedal[0], triangle[2]);
        }
        if (pedal[1].Contains(triangle[0]) && pedal[1].Contains(triangle[2]) && pedal[2].Contains(triangle[1]))
        {
            bottom = GetSegment(triangle[0], triangle[2]);
            height = GetSegment(pedal[0], triangle[1]);
        }
        else if (pedal[2].Contains(triangle[0]) && pedal[2].Contains(triangle[2]) && pedal[1].Contains(triangle[1]))
        {
            bottom = GetSegment(triangle[0], triangle[2]);
            height = GetSegment(pedal[0], triangle[1]);
        }
        if (pedal[1].Contains(triangle[1]) && pedal[1].Contains(triangle[2]) && pedal[2].Contains(triangle[0]))
        {
            bottom = GetSegment(triangle[1], triangle[2]);
            height = GetSegment(pedal[0], triangle[0]);
        }
        else if (pedal[2].Contains(triangle[1]) && pedal[2].Contains(triangle[2]) && pedal[1].Contains(triangle[0]))
        {
            bottom = GetSegment(triangle[1], triangle[2]);
            height = GetSegment(pedal[0], triangle[0]);
        }
        if (bottom is null || height is null) return;
        Equation pred = new ProductionEquation(new() { { triangle.Area, 1 }, { bottom.Length, -1 }, { height.Length, -1 } }, Expr.Half);
        pred.AddReason();
        pred.AddCondition(triangle, pedal);
        updater.Add(pred);
    }

    [Alias("三角形面积的正弦公式")]
    public void RuleTriGeoQuantity06TriangleSineAreaFormula(Triangle tri)
    {
        var c = GetSegment(tri[0], tri[1]);
        var a = GetSegment(tri[1], tri[2]);
        var b = GetSegment(tri[2], tri[0]);
        var B = GetAngle(tri[0], tri[1], tri[2]);
        var C = GetAngle(tri[1], tri[2], tri[0]);
        var A = GetAngle(tri[2], tri[0], tri[1]);
        ProductionEquation pred1 = new ProductionEquation(new() { { a.Length, 1 }, { b.Length, 1 }, { C.Sin, 1 }, { tri.Area, -1 } }, 2);
        pred1.AddReason();
        pred1.AddCondition(tri);
        updater.Add(pred1);
        ProductionEquation pred2 = new ProductionEquation(new() { { a.Length, 1 }, { c.Length, 1 }, { B.Sin, 1 }, { tri.Area, -1 } }, 2);
        pred2.AddReason();
        pred2.AddCondition(tri);
        updater.Add(pred2);
        ProductionEquation pred3 = new ProductionEquation(new() { { b.Length, 1 }, { c.Length, 1 }, { A.Sin, 1 }, { tri.Area, -1 } }, 2);
        pred3.AddReason();
        pred3.AddCondition(tri);
        updater.Add(pred3);
    }

    [Alias("三角形周长公式")]
    public void RuleTriGeoQuantity07TrianglePerimeterFormula(Triangle triangle)
    {
        var ab = GetSegment((Point)triangle[0], (Point)triangle[1]);
        var bc = GetSegment((Point)triangle[1], (Point)triangle[2]);
        var ca = GetSegment((Point)triangle[2], (Point)triangle[0]);
        Equation pred = new LinearEquation(new() { { ab.Length, 1 }, { bc.Length, 1 }, { ca.Length, 1 }, { triangle.Perimeter, -1 } }, 0);
        pred.AddReason();
        pred.AddCondition(triangle);
        updater.Add(pred);
    }

    [Alias("等边三角形面积公式")]
    public void RuleTriGeoQuantity08EquilateralTriangleAreaFormula(EquilateralTriangle triangle)
    {
        Segment ab = GetSegment(triangle[0], triangle[1]);
        Segment bc = GetSegment(triangle[1], triangle[2]);
        Segment ca = GetSegment(triangle[2], triangle[0]);
        Triangle tri = GetTriangle((Point)triangle[0], (Point)triangle[1], (Point)triangle[2]);
        ProductionEquation pred1 = new ProductionEquation(new() { { ab.Length, 2 }, { tri.Area, -1 } }, 4 / Expr.Three.Sqrt());
        pred1.AddReason();
        pred1.AddCondition(triangle);
        updater.Add(pred1);
        ProductionEquation pred2 = new ProductionEquation(new() { { bc.Length, 2 }, { tri.Area, -1 } }, 4 / Expr.Three.Sqrt());
        pred2.AddReason();
        pred2.AddCondition(triangle);
        updater.Add(pred2);
        ProductionEquation pred3 = new ProductionEquation(new() { { ca.Length, 2 }, { tri.Area, -1 } }, 4 / Expr.Three.Sqrt());
        pred3.AddReason();
        pred3.AddCondition(triangle);
        updater.Add(pred3);
    }

    [Alias("等腰直角三角形的边长关系")]
    public void RuleTriGeoQuantity09IsoscelesRightTriangleSideLengthRelation(IsoscelesRightTriangle triangle)
    {
        var ab = GetSegment(triangle[0], triangle[1]);
        var ac = GetSegment(triangle[0], triangle[2]);
        var bc = GetSegment(triangle[1], triangle[2]);
        QuantityRatio pred = new QuantityRatio(ab.Length, bc.Length, Expr.Two.Sqrt().Invert());
        pred.AddReason();
        pred.AddCondition(triangle);
        updater.Add(pred);
        QuantityRatio pred2 = new QuantityRatio(ac.Length, bc.Length, Expr.Two.Sqrt().Invert());
        pred2.AddReason();
        pred2.AddCondition(triangle);
        updater.Add(pred2);
    }

    [Alias("正弦定理")]
    public void RuleTriGeoQuantity10TriangleSineRule(Triangle tri)
    {
        var c = GetSegment(tri[0], tri[1]);
        var a = GetSegment(tri[1], tri[2]);
        var b = GetSegment(tri[2], tri[0]);
        var B = GetAngle(tri[0], tri[1], tri[2]);
        var C = GetAngle(tri[1], tri[2], tri[0]);
        var A = GetAngle(tri[2], tri[0], tri[1]);
        ProductionEquation pred1 = new ProductionEquation(new() { { a.Length, 1 }, { B.Sin, 1 }, { b.Length, -1 }, { A.Sin, -1 } }, 1);
        pred1.AddReason();
        pred1.AddCondition(tri);
        updater.Add(pred1);
        ProductionEquation pred2 = new ProductionEquation(new() { { a.Length, 1 }, { C.Sin, 1 }, { c.Length, -1 }, { A.Sin, -1 } }, 1);
        pred2.AddReason();
        pred2.AddCondition(tri);
        updater.Add(pred2);
        ProductionEquation pred3 = new ProductionEquation(new() { { b.Length, 1 }, { C.Sin, 1 }, { c.Length, -1 }, { B.Sin, -1 } }, 1);
        pred3.AddReason();
        pred3.AddCondition(tri);
        updater.Add(pred3);
    }

    [Alias("余弦定理")]
    public void RuleTriGeoQuantity11TriangleSideLengthCosineRule(Triangle tri)
    {
        var c = GetSegment(tri[0], tri[1]);
        var a = GetSegment(tri[1], tri[2]);
        var b = GetSegment(tri[2], tri[0]);
        var B = GetAngle(tri[0], tri[1], tri[2]);
        var C = GetAngle(tri[1], tri[2], tri[0]);
        var A = GetAngle(tri[2], tri[0], tri[1]);
        Equation pred = new Equation(a.Length.Pow(2), b.Length.Pow(2) + c.Length.Pow(2) - b.Length * c.Length * 2 * A.Cos);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
        Equation pred1 = new Equation(b.Length.Pow(2), a.Length.Pow(2) + c.Length.Pow(2) - a.Length * c.Length * 2 * B.Cos);
        pred1.AddReason();
        pred1.AddCondition(tri);
        updater.Add(pred1);
        Equation pred2 = new Equation(c.Length.Pow(2), a.Length.Pow(2) + b.Length.Pow(2) - a.Length * b.Length * 2 * C.Cos);
        pred2.AddReason();
        pred2.AddCondition(tri);
        updater.Add(pred2);
    }

    [Alias("三角形面积的分割关系")]
    public void RuleTriGeoQuantity12TriangleAreaSplitRelation(Triangle tri, PointOnSeg withInPoints)
    {
        if (tri[0] == withInPoints[1] && tri[1] == withInPoints[2] || tri[1] == withInPoints[1] && tri[0] == withInPoints[2])
        {
            var e = tri[2];
            var tri1 = GetTriangle((Point)e, (Point)withInPoints[0], (Point)withInPoints[1]);
            var tri2 = GetTriangle((Point)e, (Point)withInPoints[0], (Point)withInPoints[2]);
            if (tri1 is null || tri2 is null) return;
            Equation know = new LinearEquation(new() { { tri1.Area, 1 }, { tri2.Area, 1 }, { tri.Area, -1 } }, 0);
            know.AddReason();
            know.AddCondition(tri, withInPoints);
            updater.Add(know);
        }
        else if (tri[1] == withInPoints[1] && tri[2] == withInPoints[2] || tri[2] == withInPoints[1] && tri[1] == withInPoints[2])
        {
            var e = tri[0];
            var tri1 = GetTriangle((Point)e, (Point)withInPoints[0], (Point)withInPoints[1]);
            var tri2 = GetTriangle((Point)e, (Point)withInPoints[0], (Point)withInPoints[2]);
            if (tri1 is null || tri2 is null) return;
            Equation know = new LinearEquation(new() { { tri1.Area, 1 }, { tri2.Area, 1 }, { tri.Area, -1 } }, 0);
            know.AddReason();
            know.AddCondition(tri, withInPoints);
            updater.Add(know);
        }
        else if (tri[2] == withInPoints[1] && tri[1] == withInPoints[2] || tri[1] == withInPoints[1] && tri[2] == withInPoints[2])
        {
            var e = tri[0];
            var tri1 = GetTriangle((Point)e, (Point)withInPoints[0], (Point)withInPoints[1]);
            var tri2 = GetTriangle((Point)e, (Point)withInPoints[0], (Point)withInPoints[2]);
            if (tri1 is null || tri2 is null) return;
            Equation know = new LinearEquation(new() { { tri1.Area, 1 }, { tri2.Area, 1 }, { tri.Area, -1 } }, 0);
            know.AddReason();
            know.AddCondition(tri, withInPoints);
            updater.Add(know);
        }
    }

    [Alias("三角形面积的加法关系")]
    public void RuleTriGeoQuantity13TriangleAddAreaFormula(Triangle triangle, PointOnSeg pointWithInPoints)
    {
        Segment seg = GetSegment(pointWithInPoints.EndPoint1, pointWithInPoints.EndPoint2);
        Segment ab = GetSegment(triangle[0], triangle[1]);
        Segment bc = GetSegment(triangle[1], triangle[2]);
        Segment ca = GetSegment(triangle[2], triangle[0]);
        Point top = null;
        if (seg == ab)
            top = (Point)triangle[2];
        else if (seg == bc)
            top = (Point)triangle[0];
        else if (seg == ca)
            top = (Point)triangle[1];
        else
            return;
        Triangle subTri1 = GetTriangle(pointWithInPoints.EndPoint1, top, pointWithInPoints.Point);
        Triangle subTri2 = GetTriangle(pointWithInPoints.EndPoint2, top, pointWithInPoints.Point);
        if (subTri1 is not null && subTri2 is not null && subTri1 != subTri2)
        {
            Equation pred = new LinearEquation(new() { { subTri1.Area, 1 }, { subTri2.Area, 1 }, { triangle.Area, -1 } }, 0);
            pred.AddReason();
            pred.AddCondition(triangle, pointWithInPoints);
            updater.Add(pred);
        }
    }
}
