[RuleType(RuleType.Tradition)]
internal class CircleRules : RuleClass
{
    [Alias("扇形面积公式")]
    public void RuleCc01SectorAreaCalculationFormula(Sector sector, Angle angleSize)
    {
        var c = sector[0];
        var f = sector[1];
        var e = sector.Properties.Last();
        var circle = GetCircle(c, f);
        var angle = GetAngle(f, c, e);
        if (angle is not null)
        {
            if (angle == angleSize)
            {
                var pred = new ProductionEquation(new() { { circle.Area ,1}, { angle.Size, 1 },{ sector.Area ,-1} } ,360);
                pred.AddReason();
                pred.AddCondition(circle);
                updater.Add(pred);
            }
        }
    }

    [Alias("圆的半径公式")]
    public void RuleCc02CircleRadiusFormula(Circle circle)
    {
        var pred2 = new QuantityRatio(circle.Diameter, circle.Radius ,2);
        pred2.AddReason();
        pred2.AddCondition(circle);
        updater.Add(pred2);
    }

    [Alias("圆的周长公式")]
    public void RuleCc03CirclePerimeterCalculationFormula(Circle circle)
    {
        var pred = new QuantityRatio(circle.Perimeter, circle.Radius ,2 * Expr.Pi);
        pred.AddReason();
        pred.AddCondition(circle);
        updater.Add(pred);
    }

    [Alias("圆的面积公式")]
    public void RuleCc04CircleAreaCalculationFormula(Circle circle)
    {
        var pred = new ProductionEquation(new() { { circle.Area, 1 }, { circle.Radius, -2 } }, Expr.Pi);
        pred.AddReason();
        pred.AddCondition(circle);
        updater.Add(pred);
    }

    [Alias("圆心到圆上点的距离等于半径")]
    public void RuleCc05DistanceFromCenterToCirclePointIsRadius(Circle circle)
    {
        foreach (Point item in circle.Properties.Skip(1))
        {
            var seg = GetSegment(item, circle.Center);
            if (seg is not null)
            {
                var pred = new QuantityRatio(circle.Radius, seg.Length);
                pred.AddCondition(circle);
                pred.AddReason();
                updater.Add(pred);
            }
        }
    }

    [Alias("过圆心的线段为直径")]
    public void RuleCc06SegmentThroughCenterEqualsDiameter(Circle circle, Line line)
    {
        if (!line.Properties.Contains(circle.Center)) return;
        var cIndex = line.Properties.IndexOf(circle.Center);
        var e1 = line.Properties.Take(cIndex).FirstOrDefault(circle.Properties.Contains);
        var e2 = line.Properties.Skip(cIndex + 1).FirstOrDefault(circle.Properties.Contains);
        if (e1 is not null && e2 is not null)
        {
            var segment = GetSegment(e1, e2);
            var pred = new QuantityRatio(segment.Length, circle.Diameter, 1);
            pred.AddCondition(circle);
            pred.AddReason();
            updater.Add(pred);

            var sector = GetSector(circle.Center, e1, e2);
            var pred2 = new QuantityRatio(circle.Area,sector.Area ,2);
            pred2.AddCondition(circle);
            pred2.AddReason();
            updater.Add(pred2);

            var sector2 = GetSector(circle.Center, e2, e1);
            var pred3 = new QuantityRatio(circle.Area, sector2.Area, 2);
            pred3.AddCondition(circle);
            pred3.AddReason();
            updater.Add(pred3);

            var seg = GetSegment(e1, e2);
            CircleDiameter pred4 = new CircleDiameter(circle, seg);
            pred4.AddCondition(circle);
            pred4.AddReason();
            updater.Add(pred4);
        }
    }

    [Alias("圆上各点到圆心的距离相等")]
    public void RuleCc07AllPointsOnCircleEquidistantFromCenter(Circle circle)
    {
        var o = circle.Properties[0];
        List<Segment> segs = [];
        foreach (var item in circle.Properties.Skip(1))
        {
            var seg = GetSegment(o, item);
            if (seg is not null)
                segs.Add(seg);
        }
        foreach (var item in segs.Skip(1))
        {
            QuantityRatio pred = new QuantityRatio(segs.First().Length, item.Length);
            pred.AddReason();
            pred.AddCondition(circle);
            updater.Add(pred);
        }
    }

    [Alias("圆周角定理")]
    public void RuleCc08InscribedAngleIsHalfCentralAngle(Circle circle)
    {
        var a = PermutationCombinationTool.GetCombination(circle.Properties.Skip(1).ToList(), 3);
        foreach (var item in a)
        {
            var C = (Point)circle.Center;
            var A = (Point)item[0];
            var B = (Point)item[1];
            var P = (Point)item[2];
            var b = IsOnMinorArc((C.X, C.Y), (A.X, A.Y), (B.X, B.Y), (P.X, P.Y));
            if (b is false)
            {
                var angle1 = GetAngle(A, C, B);
                var angle2 = GetAngle(A, P, B);
                if (angle1 is not null && angle2 is not null)
                {
                    var pred = new QuantityRatio(angle1.Size, angle2.Size, 2);
                    pred.AddCondition(circle);
                    pred.AddReason();
                    updater.Add(pred);
                }
            }

            b = IsOnMinorArc((C.X, C.Y), (A.X, A.Y), (P.X, P.Y), (B.X, B.Y));
            if (b is false)
            {
                var angle1 = GetAngle(A, C, P);
                var angle2 = GetAngle(A, B, P);
                if (angle1 is not null && angle2 is not null)
                {
                    var pred = new QuantityRatio(angle1.Size, angle2.Size, 2);
                    pred.AddCondition(circle);
                    pred.AddReason();
                    updater.Add(pred);
                }
            }

            b = IsOnMinorArc((C.X, C.Y), (B.X, B.Y), (P.X, P.Y), (A.X, A.Y));
            if (b is false)
            {
                var angle1 = GetAngle(P, C, B);
                var angle2 = GetAngle(P, A, B);
                if (angle1 is not null && angle2 is not null)
                {
                    var pred = new QuantityRatio(angle1.Size, angle2.Size, 2);
                    pred.AddCondition(circle);
                    pred.AddReason();
                    updater.Add(pred);
                }
            }
        }
    }

    [Alias("直径所对的圆周角是直角")]
    public void RuleCc09DiameterInscribedAngle90Degrees(CircleDiameter circleDiameter)
    {
        var circle = circleDiameter.Circle;
        foreach (var item in circle.Properties.Skip(1))
        {
            if (circleDiameter.Diameter.Properties.Contains(item))
                continue;
            var angle = GetAngle(circleDiameter.Diameter[0], item, circleDiameter.Diameter[1]);
            if (angle is null) continue;
            QuantityValue pred = new(angle.Size, 90);
            pred.AddCondition(circle);
            pred.AddReason();
            updater.Add(pred);
        }
    }

    [Alias("垂径定理")]
    public void RuleCc10PerpendicularDiameterTheorem(Circle circle, LinePerpendicular perpendicular)
    {
        if (perpendicular[0].Properties.Contains(circle.Center) && !perpendicular[1].Properties.Contains(circle.Center))
        {
            var l = perpendicular[1].Properties.Where(p => p != circle.Center).FirstOrDefault(circle.Properties.Contains);
            if (l is not null)
            {
                var (i, p1, p2) = FindIntersection(perpendicular[1].Properties, circle.Properties);
                if (i.Count == 2)
                {
                    var bebisector = GetSegment((Point)i[0], (Point)i[1]);
                    VerticalBisectorLine pred = new VerticalBisectorLine(bebisector, (Line)perpendicular[0]);
                    pred.AddReason();
                    pred.AddCondition(circle, perpendicular);
                    updater.Add(pred);
                }
            }
        }
        else if (perpendicular[1].Properties.Contains(circle.Center) && !perpendicular[0].Properties.Contains(circle.Center))
        {
            var l = perpendicular[0].Properties.Where(p => p != circle.Center).FirstOrDefault(circle.Properties.Contains);
            if (l is not null)
            {
                var (i, p1, p2) = FindIntersection(perpendicular[0].Properties, circle.Properties);
                if (i.Count == 2)
                {
                    var bebisector = GetSegment((Point)i[0], (Point)i[1]);
                    VerticalBisectorLine pred = new VerticalBisectorLine(bebisector, (Line)perpendicular[1]);
                    pred.AddReason();
                    pred.AddCondition(circle, perpendicular);
                    updater.Add(pred);
                }
            }
        }
    }

    [Alias("垂径定理的推论")]
    public void RuleCc11PerpendicularDiameterTheoremPart(Circle circle, VerticalBisectorLine verticalBisector)
    {
        if (verticalBisector.Bisector.Properties.Contains(circle.Center) && verticalBisector.Seg.Properties.TrueForAll(circle.Properties.Skip(1).Contains))
        {
            var ps = circle.Properties.Skip(1).Where(verticalBisector.Bisector.Properties.Contains);
            foreach (Point p in ps)
            {
                if (IsOnMinorArc(circle.Center, (Point)verticalBisector.Seg[0], (Point)verticalBisector.Seg[1], (Point)p))
                {
                    var arc = GetArc(circle.Center, verticalBisector.Seg[0], verticalBisector.Seg[1]);
                    ArcMidpoint pred = new ArcMidpoint(p, arc);
                    pred.AddReason();
                    pred.AddCondition(circle, verticalBisector);
                    updater.Add(pred);
                }
            }
        }
    }

    [Alias("圆外角定理")]
    public void RuleCc12ExteriorAngleTheoremOfCircle(Circle circle, Angle angle)
    {
        var edge11 = angle.Edge1.Where(circle.Properties.Skip(1).Contains);
        var edge22 = angle.Edge2.Where(circle.Properties.Skip(1).Contains);
        if (angle.Edge1.Where(circle.Properties.Skip(1).Contains).Count() == 2 &&
            angle.Edge2.Where(circle.Properties.Skip(1).Contains).Count() == 2)
        {
            Arc minArc = GetArc(circle.Center, (Point)edge11.First(), (Point)edge22.First());
            Arc maxArc = GetArc(circle.Center, (Point)edge11.Last(), (Point)edge22.Last());
            Equation know = new LinearEquation(new() { { angle.Size ,2},{ minArc.Size ,1},{ maxArc.Size,-1 } },0);
            know.AddReason();
            know.AddCondition(circle, angle);
            updater.Add(know);
        }
    }

    [Alias("圆幂定理")]
    public void RuleCc13PowerOfPointTheorem(Circle circle, Angle angle)
    {
        var edge11 = angle.Edge1.Where(circle.Properties.Skip(1).Contains);
        var edge22 = angle.Edge2.Where(circle.Properties.Skip(1).Contains);
        if (angle.Edge1.Where(circle.Properties.Skip(1).Contains).Count() == 2 &&
            angle.Edge2.Where(circle.Properties.Skip(1).Contains).Count() == 2)
        {
            Segment s11 = GetSegment(angle.Vertex, edge11.First());
            Segment s12 = GetSegment(angle.Vertex, edge11.Last());
            Segment s21 = GetSegment(angle.Vertex, edge22.First());
            Segment s22 = GetSegment(angle.Vertex, edge22.Last());

            ProductionEquation know = new ProductionEquation(new() { { s11.Length , 1 }, { s12.Length, 1 }, { s21.Length, -1 }, { s22.Length, -1 } },1);
            know.AddReason();
            know.AddCondition(circle, angle);
            updater.Add(know);
        }
    }

    [Alias("等圆心角所对的弦相等")]
    public void RuleCc14EqualCentralAnglesImplyEqualChords(Circle circle, AngleSizeEqual equal)
    {
        if (circle.Center != equal.Angle1.Vertex || circle.Center != equal.Angle2.Vertex) return;
        var ac11 = FindCIntersection(circle.Properties, equal.Angle1.Edge1);
        var ac12 = FindCIntersection(circle.Properties, equal.Angle1.Edge2);
        var ac21 = FindCIntersection(circle.Properties, equal.Angle2.Edge1);
        var ac22 = FindCIntersection(circle.Properties, equal.Angle2.Edge2);
        if (ac11 is null || ac12 is null || ac21 is null || ac22 is null) return;
        var seg1 = GetSegment(ac11, ac12);
        var seg2 = GetSegment(ac21, ac22);
        if (seg1 is not null && seg2 is not null)
        {
            var pred2 = new QuantityRatio(seg1.Length, seg2.Length);
            pred2.AddReason();
            pred2.AddCondition(circle, equal);
            updater.Add(pred2);
        }
    }

    [Alias("圆的切线垂直于过切点的半径")]
    public void RuleCc15TangentPerpendicularToRadius(CircleTangent ct)
    {
        var c = FindCIntersection(ct[0].Properties, ct[1].Properties);
        var ra = GetLine(c, ct[0][0]);
        if (ra is not null)
        {
            LinePerpendicular pred = new LinePerpendicular(ra, (Line)ct[1]);
            pred.AddReason();
            pred.AddCondition(ct);
            updater.Add(pred);
        }
    }

    [Alias("切线长定理")]
    public void RuleCc16TangentSegmentsFromExternalPointAreEqual(CircleTangent ct1, CircleTangent ct2)
    {
        if (ct1 == ct2) return;
        if (ct1.Circle != ct2.Circle) return;
        var i = FindCIntersection(ct1[1].Properties, ct2[1].Properties);
        if (i is not null)
        {
            var c1 = FindCIntersection(ct1[0].Properties, ct1[1].Properties);
            var c2 = FindCIntersection(ct2[0].Properties, ct2[1].Properties);
            var s1 = GetSegment(i, c1);
            var s2 = GetSegment(i, c2);
            if (s1 is not null && s2 is not null)
            {
                QuantityRatio pred = new QuantityRatio(s1.Length, s2.Length);
                pred.AddReason();
                pred.AddCondition(ct1, ct2);
                updater.Add(pred);
            }
        }
    }
}
