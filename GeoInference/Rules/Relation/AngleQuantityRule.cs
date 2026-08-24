
[RuleType(RuleType.Tradition)]
public class AngleQuantityRule : RuleClass
{
    [Alias("垂直推出直角")]
    public void RuleAQ001PerpendicularYields90DegreeAngle(Pedal pedal)
    {
        var i = (Point)pedal[0];
        var line1 = (Line)pedal[1];
        var line2 = (Line)pedal[2];
        foreach (var p1 in line1.Properties)
        {
            foreach (var p2 in line2.Properties)
            {
                var angle = GetAngle(p1, i, p2);
                if (angle is null) continue;
                QuantityValue pred = new QuantityValue(angle.Size, 90);
                pred.AddReason();
                pred.AddCondition(pedal);
                updater.Add(pred);
            }
        }
    }

    [Alias("角的加法")]
    public void RuleAQ002AngleAddition(Angle angle, Point point)
    {
        var v = angle.Vertex;
        var ePoint1 = angle.Edge1.First();
        var ePoint2 = angle.Edge2.First();
        ExprVector oA = new ExprVector(ePoint1.X - v.X, ePoint1.Y - v.Y, ePoint1.Z - v.Z);
        ExprVector oB = new ExprVector(ePoint2.X - v.X, ePoint2.Y - v.Y, ePoint2.Z - v.Z);
        ExprVector oP = new ExprVector(point.X - v.X, point.Y - v.Y, point.Z - v.Z);
        var cross1 = oA.X * oB.Y - oA.Y * oB.X;
        var cross2 = oA.X * oP.Y - oA.Y * oP.X;
        var cross3 = oB.X * oP.Y - oB.Y * oP.X;

        if (cross1 > 0 && cross2 > 0 && cross3 < 0 || cross1 < 0 && cross2 < 0 && cross3 > 0)
        {
            var a1 = GetAngle(point, v, ePoint1);
            var a2 = GetAngle(point, v, ePoint2);
            if (a1 is null || a2 is null) return;

            var pred = new LinearEquation(new() { { a1.Size, 1 }, { a2.Size, 1 }, { angle.Size, -1 } }, 0);
            pred.AddReason();
            pred.AddCondition(angle, point);
            updater.Add(pred);

        }
    }

    [Alias("邻补角互补")]
    public void RuleAQ003SupplementaryAngles(Line line, Point point)
    {
        var edge1 = line[0];
        var edge2 = line.Properties.Last();
        for (int i = 1; i < line.Properties.Count - 1; i++)
        {
            var mid = line[i];
            Angle angle1 = GetAngle(edge1, mid, point);
            Angle angle2 = GetAngle(edge2, mid, point);
            if (angle1 is not null && angle2 is not null)
            {
                var pred = new LinearEquation(new() { { angle1.Size, 1 }, { angle2.Size, 1 } }, 180);
                pred.AddReason();
                pred.AddCondition(line, point);
                updater.Add(pred);
            }
        }
    }

    [Alias("周角")]
    public void RuleAQ004CircularAngle(Point center, Point p1, Point p2, Point p3)
    {
        return;
    }

    [Alias("角平分线的判定")]
    public void RuleAQ005AngleBisectorInference(Angle angle, Point point, AngleSizeEqual angleSizeEqual)
    {
        var v = angle.Vertex;
        var ePoint1 = angle.Edge1.First();
        var ePoint2 = angle.Edge2.First();
        ExprVector oA = new ExprVector(ePoint1.X - v.X, ePoint1.Y - v.Y, ePoint1.Z - v.Z);
        ExprVector oB = new ExprVector(ePoint2.X - v.X, ePoint2.Y - v.Y, ePoint2.Z - v.Z);
        ExprVector oP = new ExprVector(point.X - v.X, point.Y - v.Y, point.Z - v.Z);
        var cross1 = oA.X * oB.Y - oA.Y * oB.X;
        var cross2 = oA.X * oP.Y - oA.Y * oP.X;
        var cross3 = oB.X * oP.Y - oB.Y * oP.X;
        var a1 = GetAngle(point, v, ePoint1);
        var a2 = GetAngle(point, v, ePoint2);
        if (a1 is null || a2 is null) return;
        if (a1 == angleSizeEqual.Angle1 && a2 == angleSizeEqual.Angle2 || a1 == angleSizeEqual.Angle2 && a2 == angleSizeEqual.Angle1)
        {
            var l = GetLine(angle.Vertex, point);
            var pred = new AngularBisectorLine(angle, l);
            pred.AddReason();
            pred.AddCondition(angle, point, angleSizeEqual);
            updater.Add(pred);
        }
    }

    [Alias("角平分线的性质")]
    public void RuleAQ006AngleBisectorDividesAngle(AngularBisectorLine bisectorLine)
    {
        foreach (Point item in bisectorLine.Bisector.Points)
        {
            if (bisectorLine.Angle.Vertex == item) continue;
            var sub1 = GetAngle(bisectorLine.Angle.Edge1[0], bisectorLine.Angle.Vertex, item);
            var sub2 = GetAngle(bisectorLine.Angle.Edge2[0], bisectorLine.Angle.Vertex, item);
            var pred = new QuantityRatio(sub1.Size, sub2.Size);
            pred.AddReason();
            pred.AddCondition(bisectorLine);
            updater.Add(pred);

            if (IsBetweenAngle(bisectorLine.Angle.Vertex, bisectorLine.Angle.Edge1[0], bisectorLine.Angle.Edge2[0], item))
            {
                var pred1 = new QuantityRatio(bisectorLine.Angle.Size, sub1.Size, 2);
                pred1.AddReason();
                pred1.AddCondition(bisectorLine);
                updater.Add(pred1);
                var pred2 = new QuantityRatio(bisectorLine.Angle.Size, sub2.Size, 2);
                pred2.AddReason();
                pred2.AddCondition(bisectorLine);
                updater.Add(pred2);
            }
        }
    }

    [Alias("两直线平行，内错角相等")]
    public void RuleAQ007AlternateInteriorAnglesEqual(LineParallel lineParallel, PointsOnLineDifferentSide diffSide)
    {
        foreach (var p1 in lineParallel.Line1.Properties)
        {
            foreach (var p2 in lineParallel.Line2.Properties)
            {
                if (p1 == diffSide[0] && p2 == diffSide[1] || p1 == diffSide[1] && p2 == diffSide[0])
                {
                    if (lineParallel.Line1.Properties.Contains(diffSide[2]) && lineParallel.Line2.Properties.Contains(diffSide[3]))
                    {
                        var angle1 = GetAngle(diffSide[2], p1, p2);
                        var angle2 = GetAngle(diffSide[3], p2, p1);
                        if (angle1 is null || angle2 is null) return;
                        QuantityRatio pred = new QuantityRatio(angle1.Size, angle2.Size);
                        pred.AddReason();
                        pred.AddCondition(lineParallel, diffSide);
                        updater.Add(pred);
                    }
                    else if (lineParallel.Line1.Properties.Contains(diffSide[3]) && lineParallel.Line2.Properties.Contains(diffSide[2]))
                    {
                        var angle1 = GetAngle(diffSide[3], p1, p2);
                        var angle2 = GetAngle(diffSide[2], p2, p1);
                        if (angle1 is null || angle2 is null) return;
                        QuantityRatio pred = new QuantityRatio(angle1.Size, angle2.Size);
                        pred.AddReason();
                        pred.AddCondition(lineParallel, diffSide);
                        updater.Add(pred);
                    }
                }
            }
        }
    }

    [Alias("两直线平行，同旁内角互补")]
    public void RuleAQ008ConsecutiveInteriorAnglesSupplementary(LineParallel lineParallel, PointsOnLineSameSide sameSide)
    {
        foreach (var p1 in lineParallel.Line1.Properties)
        {
            foreach (var p2 in lineParallel.Line2.Properties)
            {
                if (p1 == sameSide[0] && p2 == sameSide[1] || p1 == sameSide[1] && p2 == sameSide[0])
                {
                    if (lineParallel.Line1.Properties.Contains(sameSide[2]) && lineParallel.Line2.Properties.Contains(sameSide[3]))
                    {
                        var angle1 = GetAngle(sameSide[2], p1, p2);
                        var angle2 = GetAngle(sameSide[3], p2, p1);
                        if (angle1 is null || angle2 is null) return;
                        Equation pred = new LinearEquation(new() { { angle1.Size, 1 }, { angle2.Size, 1 } }, 180);
                        pred.AddReason();
                        pred.AddCondition(lineParallel, sameSide);
                        updater.Add(pred);
                    }
                    else if (lineParallel.Line1.Properties.Contains(sameSide[3]) && lineParallel.Line2.Properties.Contains(sameSide[2]))
                    {
                        var angle1 = GetAngle(sameSide[3], p1, p2);
                        var angle2 = GetAngle(sameSide[2], p2, p1);
                        if (angle1 is null || angle2 is null) return;
                        Equation pred = new LinearEquation(new() { { angle1.Size, 1 }, { angle2.Size, 1 } }, 180);
                        pred.AddReason();
                        pred.AddCondition(lineParallel, sameSide);
                        updater.Add(pred);
                    }
                }
            }
        }
    }

    [Alias("两直线平行，同位角相等")]
    public void RuleAQ009CorrespondingAnglesEqual(LineParallel lineParallel, Line line, PointsOnLineSameSide sameSide)
    {
        var c1 = FindCIntersection(lineParallel[0], line);
        var c2 = FindCIntersection(lineParallel[1], line);
        if (c1 is null || c2 is null) return;
        if (c1 == sameSide[0] && c2 == sameSide[1] || c1 == sameSide[1] && c2 == sameSide[0])
        {
            if (lineParallel[0].Properties.Contains(sameSide[2]) && lineParallel[1].Properties.Contains(sameSide[3]))
            {
                var a1 = GetAngle(line.Properties.First(), c1, sameSide[2]);
                var a2 = GetAngle(line.Properties.First(), c2, sameSide[3]);
                if (a1 is not null && a2 is not null)
                {
                    var pred = new QuantityRatio(a1.Size, a2.Size);
                    pred.AddReason();
                    pred.AddCondition(lineParallel, line, sameSide);
                    updater.Add(pred);
                }
                var a3 = GetAngle(line.Properties.Last(), c1, sameSide[2]);
                var a4 = GetAngle(line.Properties.Last(), c2, sameSide[3]);
                if (a3 is not null && a4 is not null)
                {
                    var pred = new QuantityRatio(a3.Size, a4.Size);
                    pred.AddReason();
                    pred.AddCondition(lineParallel, line, sameSide);
                    updater.Add(pred);
                }
            }
            else if (lineParallel[0].Properties.Contains(sameSide[3]) && lineParallel[1].Properties.Contains(sameSide[2]))
            {
                var a1 = GetAngle(line.Properties.First(), c1, sameSide[3]);
                var a2 = GetAngle(line.Properties.First(), c2, sameSide[2]);
                if (a1 is not null && a2 is not null)
                {
                    var pred = new QuantityRatio(a1.Size, a2.Size);
                    pred.AddReason();
                    pred.AddCondition(lineParallel, line, sameSide);
                    updater.Add(pred);
                }
                var a3 = GetAngle(line.Properties.Last(), c1, sameSide[3]);
                var a4 = GetAngle(line.Properties.Last(), c2, sameSide[2]);
                if (a3 is not null && a4 is not null)
                {
                    var pred = new QuantityRatio(a3.Size, a4.Size);
                    pred.AddReason();
                    pred.AddCondition(lineParallel, line, sameSide);
                    updater.Add(pred);
                }
            }
        }
    }

    [Alias("对顶角相等")]
    public void RuleAQ010VerticalAnglesEqual(LineIntersection lip)
    {
        var a1 = GetAngle(lip[1].Properties.First(), lip[0], lip[2].Properties.First());
        var a2 = GetAngle(lip[1].Properties.Last(), lip[0], lip[2].Properties.Last());
        var a3 = GetAngle(lip[1].Properties.First(), lip[0], lip[2].Properties.Last());
        var a4 = GetAngle(lip[1].Properties.Last(), lip[0], lip[2].Properties.First());
        if (a1 is not null && a2 is not null)
        {
            var pred = new QuantityRatio(a1.Size, a2.Size);
            pred.AddReason();
            pred.AddCondition(lip);
            updater.Add(pred);
        }
        if (a3 is not null && a4 is not null)
        {
            var pred = new QuantityRatio(a3.Size, a4.Size);
            pred.AddReason();
            pred.AddCondition(lip);
            updater.Add(pred);
        }
    }

    [Alias("圆内接四边形的对角互补")]
    public void RuleAQ011CyclicQuadrilateralOppositeAnglesSupplementary(Quadriliateral quad, Circle circle)
    {
        if (quad.Properties.TrueForAll(circle.Properties.Skip(1).Contains))
        {
            var abc = GetAngle(quad[0], quad[1], quad[2]);
            var bcd = GetAngle(quad[1], quad[2], quad[3]);
            var cda = GetAngle(quad[2], quad[3], quad[0]);
            var dab = GetAngle(quad[3], quad[0], quad[1]);
            var pred1 = new LinearEquation(new() { { abc.Size, 1 }, { cda.Size, 1 } }, 180);
            pred1.AddReason();
            pred1.AddCondition(quad, circle);
            updater.Add(pred1);
            var pred2 = new LinearEquation(new() { { bcd.Size, 1 }, { dab.Size, 1 } }, 180);
            pred2.AddReason();
            pred2.AddCondition(quad, circle);
            updater.Add(pred2);
        }
    }

    [Alias("等角的三角函数值相等")]
    public void RuleAQ012TrigonometricRelations(AngleSizeEqual equal)
    {
        var pred1 = new QuantityRatio(equal.Angle1.Sin, equal.Angle2.Sin);
        pred1.AddReason();
        pred1.AddCondition(equal);
        updater.Add(pred1);
        var pred2 = new QuantityRatio(equal.Angle1.Cos, equal.Angle2.Cos);
        pred2.AddReason();
        pred2.AddCondition(equal);
        updater.Add(pred2);
        var pred3 = new QuantityRatio(equal.Angle1.Tan, equal.Angle2.Tan);
        pred3.AddReason();
        pred3.AddCondition(equal);
        updater.Add(pred3);
    }

    [Alias("余弦化正弦公式")]
    public void RuleAQ013CosineToSineConversion(AngleCos angleCos)
    {
        var expr = (1 - angleCos.Expr.Pow(2)).Sqrt();
        QuantityValue pred = new QuantityValue(angleCos.Angle.Sin, expr);
        pred.AddReason();
        pred.AddCondition(angleCos);
        updater.Add(pred);
    }

    [Alias("正切的计算公式")]
    public void RuleAQ014CalculateTangent(AngleSin angleSin, AngleCos angleCos)
    {
        if (angleSin.Angle != angleCos.Angle) return;
        if (angleCos.Expr == 0) return;
        QuantityValue pred = new QuantityValue(angleCos.Angle.Tan, angleSin.Expr / angleCos.Expr);
        pred.AddReason();
        pred.AddCondition(angleSin, angleCos);
        updater.Add(pred);
    }
}
