
[RuleType(RuleType.Tradition)]
public class ArcRules : RuleClass
{
    [Alias("由圆生成弧与扇形")]
    public void RuleArc001GenerateArcsAndSectorsFromCircle(Circle circle)
    {
        for (int i = 1; i < circle.Properties.Count; i++)
        {
            for (int j = i + 1; j < circle.Properties.Count; j++)
            {
                Arc pred1 = new Arc(circle.Center, (Point)circle[i], (Point)circle[j]);
                pred1.AddReason();
                pred1.AddCondition(circle);
                updater.Add(pred1);

                var s1 = GetSegment(circle.Center, circle[i]);
                var s2 = GetSegment(circle.Center, circle[j]);
                if (s1 is not null && s2 is not null)
                {
                    Sector pred2 = new Sector(circle.Center, (Point)circle[i], (Point)circle[j]);
                    pred2.AddReason();
                    pred2.AddCondition(circle);
                    updater.Add(pred2);
                    Sector pred3 = new Sector(circle.Center, (Point)circle[j], (Point)circle[i]);
                    pred3.AddReason();
                    pred3.AddCondition(circle);
                    updater.Add(pred3);
                }
            }
        }
    }
    [Alias("圆上弧长的加法")]
    public void RuleArc002ArcLengthAdditionOnCircle(Circle circle)
    {
        var onCircle = circle.Properties.Skip(1).ToList();
        var combos = PermutationCombinationTool.GetCombination(onCircle, 3);
        foreach (var combo in combos)
        {
            var p1 = (Point)combo[0];
            var p2 = (Point)combo[1];
            var p3 = (Point)combo[2];

            void TryAddArcSum(Point a, Point b, Point mid)
            {
                var whole = GetArc(circle.Center, a, b);
                var s1 = GetArc(circle.Center, a, mid);
                var s2 = GetArc(circle.Center, mid, b);
                if (whole is null || s1 is null || s2 is null) return;
                var pred = new LinearEquation(
                    new() { { s1.MinorArcLength, 1 }, { s2.MinorArcLength, 1 }, { whole.MinorArcLength, -1 } }, 0);
                pred.AddReason();
                pred.AddCondition(circle);
                updater.Add(pred);
            }

            if (HasColine(p1, p2, circle.Center))
            {
                TryAddArcSum(p1, p2, p3);
            }
            else
            {
                if (IsOnMinorArc(circle.Center, p1, p2, p3))
                    TryAddArcSum(p1, p2, p3);
                if (IsOnMinorArc(circle.Center, p2, p1, p3))
                    TryAddArcSum(p1, p2, p3);
            }
            if (HasColine(p1, p3, circle.Center))
            {
                TryAddArcSum(p1, p3, p2);
            }
            else
            {
                if (IsOnMinorArc(circle.Center, p1, p3, p2))
                    TryAddArcSum(p1, p3, p2);
                if (IsOnMinorArc(circle.Center, p3, p1, p2))
                    TryAddArcSum(p1, p3, p2);
            }
            if (HasColine(p2, p3, circle.Center))
            {
                TryAddArcSum(p2, p3, p1);
            }
            else
            {
                if (IsOnMinorArc(circle.Center, p2, p3, p1))
                    TryAddArcSum(p2, p3, p1);
                if (IsOnMinorArc(circle.Center, p3, p2, p1))
                    TryAddArcSum(p2, p3, p1);
            }
        }
    }
    [Alias("弧长与圆心角的比例关系")]
    public void RuleArc004ArcAndCentralAngleRatio(Arc arc)
    {
        var angle = GetAngle(arc[1], arc[0], arc[2]);
        if (angle is not null)
        {
            QuantityRatio pred = new QuantityRatio(angle.Size, arc.Size);
            pred.AddReason();
            pred.AddCondition(arc, angle);
            updater.Add(pred);
        }
    }
    [Alias("弧长与圆周角的比例关系")]
    public void RuleArc005ArcAndInscribedAngleRatio(Circle circle, Arc arc)
    {
        if (arc[0] != circle.Center) return;
        foreach (var item in circle.Properties.Skip(1))
        {
            if (HasColine(arc[1], circle.Center, arc[2])) continue;
            var angle = GetAngle(arc[1], item, arc[2]);
            if (angle is not null)
            {
                if (!IsOnMinorArc(circle.Center, (Point)arc[1], (Point)arc[2], (Point)item))
                {
                    QuantityRatio pred = new QuantityRatio(arc.Size, angle.Size, 2);
                    pred.AddReason();
                    pred.AddCondition(arc, angle);
                    updater.Add(pred);
                }
                else
                {
                    LinearEquation pred2 = new LinearEquation(new() { { angle.Size, 1 }, { arc.Size, Expr.One / 2 } }, 180);
                    pred2.AddReason();
                    pred2.AddCondition(arc, angle);
                    updater.Add(pred2);
                }
            }
        }
    }
    [Alias("弧长与圆周长之比")]
    public void RuleArc006CalculateArcLengthToPerimeterRatio(Circle circle, Arc arcSize)
    {
        List<Predicate> onCircle = circle.Properties.Skip(1).ToList();
        if (arcSize[0] == circle.Center && onCircle.Contains(arcSize[1]) && onCircle.Contains(arcSize[2]))
        {
            ProductionEquation pred = new ProductionEquation(new() { { arcSize.MinorArcLength ,-1}, { circle.Perimeter, 1 }, { arcSize.Size, 1 } } ,360);
            pred.AddReason();
            pred.AddCondition(circle, arcSize);
            updater.Add(pred);
        }
    }
    #region Congruence
    [Alias("弧的中点推出等弧")]
    public void RuleArc003DeriveCongruentArcsFromArcMidpoint(ArcMidpoint arcMidpoint)
    {
        var r1 = GetArc(arcMidpoint[1][0], arcMidpoint[1][1], arcMidpoint[0]);
        var r2 = GetArc(arcMidpoint[1][0], arcMidpoint[0], arcMidpoint[1][2]);

        CongruentArc pred3 = new CongruentArc(r1, r2);
        pred3.AddReason();
        pred3.AddCondition(arcMidpoint);
        updater.Add(pred3);
    }
    [Alias("等弧的判定（等弦）")]
    public void RuleArc007ArcCongruenceByEqualChords(Circle circle, SegmentLengthEqual equal)
    {
        List<Predicate> onCircle = circle.Properties.Skip(1).ToList();
        if (equal.Seg1.Properties.TrueForAll(onCircle.Contains) &&
            equal.Seg2.Properties.TrueForAll(onCircle.Contains))
        {
            var arc1 = GetArc(circle.Center, equal.Seg1[0], equal.Seg1[1]);
            var arc2 = GetArc(circle.Center, equal.Seg2[0], equal.Seg2[1]);
            if (arc1 is not null && arc2 is not null)
            {
                CongruentArc pred = new CongruentArc(arc1, arc2);
                pred.AddReason();
                pred.AddCondition(circle, equal);
                updater.Add(pred);
            }
        }
    }
    [Alias("等弧的判定（等弧长）")]
    public void RuleArc008ArcCongruenceByEqualArcLengths(Circle circle, ArcLengthEqual equal)
    {
        if (circle.Center != equal.Arc1[0] || circle.Center != equal.Arc2[0]) return;
        CongruentArc pred3 = new CongruentArc(equal.Arc1, equal.Arc2);
        pred3.AddReason();
        pred3.AddCondition(circle, equal);
        updater.Add(pred3);
    }
    [Alias("等弧的判定（等圆周角）")]
    public void RuleArc009ArcCongruenceByEqualInscribedAngles(Circle circle, AngleSizeEqual equal)
    {
        List<Predicate> onCircle = circle.Properties.Skip(1).ToList();
        if (!onCircle.Contains(equal.Angle1.Vertex) ||
            !onCircle.Contains(equal.Angle2.Vertex)) return;
        var c11 = FindCIntersection(onCircle, equal.Angle1.Edge1);
        var c12 = FindCIntersection(onCircle, equal.Angle1.Edge2);
        var c21 = FindCIntersection(onCircle, equal.Angle2.Edge1);
        var c22 = FindCIntersection(onCircle, equal.Angle2.Edge2);
        if (c11 is null || c12 is null || c21 is null || c22 is null) return;
        var arc1 = GetArc(circle.Center, c11, c12);
        var arc2 = GetArc(circle.Center, c21, c22);
        if (arc1 == arc2) return;
        CongruentArc pred3 = new CongruentArc(arc1, arc2);
        pred3.AddReason();
        pred3.AddCondition(circle, equal);
        updater.Add(pred3);
    }
    [Alias("等弧的判定（等圆心角）")]
    public void RuleArc010ArcCongruenceByEqualCentralAngles(Circle circle, AngleSizeEqual equal)
    {
        if (circle.Center != equal.Angle1.Vertex || circle.Center != equal.Angle2.Vertex) return;
        List<Predicate> onCircle = circle.Properties.Skip(1).ToList();
        var c11 = FindCIntersection(onCircle, equal.Angle1.Edge1);
        var c12 = FindCIntersection(onCircle, equal.Angle1.Edge2);
        var c21 = FindCIntersection(onCircle, equal.Angle2.Edge1);
        var c22 = FindCIntersection(onCircle, equal.Angle2.Edge2);
        if (c11 is null || c12 is null || c21 is null || c22 is null) return;
        var arc1 = GetArc(circle.Center, c11, c12);
        var arc2 = GetArc(circle.Center, c21, c22);
        if (arc1 == arc2) return;
        CongruentArc pred3 = new CongruentArc(arc1, arc2);
        pred3.AddReason();
        pred3.AddCondition(circle, equal);
        updater.Add(pred3);
    }
    [Alias("等弧的性质")]
    public void RuleArc011PropertiesOfCongruentArcs(CongruentArc cong)
    {
        {
            var pred = new QuantityRatio(cong.Arc1.MinorArcLength, cong.Arc2.MinorArcLength);
            pred.AddReason();
            pred.AddCondition(cong);
            updater.Add(pred);
        }
        {
            var pred = new QuantityRatio(cong.Arc1.Size, cong.Arc2.Size);
            pred.AddReason();
            pred.AddCondition(cong);
            updater.Add(pred);
        }
        var angle1 = GetAngle(cong.Arc1[1], cong.Arc1[0], cong.Arc1[2]);
        var angle2 = GetAngle(cong.Arc2[1], cong.Arc2[0], cong.Arc2[2]);
        if (angle1 is not null && angle2 is not null)
        {
            var pred1 = new QuantityRatio(angle1.Size, angle2.Size);
            pred1.AddReason();
            pred1.AddCondition(cong);
            updater.Add(pred1);
        }
        var seg1 = GetSegment(cong.Arc1[1], cong.Arc1[2]);
        var seg2 = GetSegment(cong.Arc2[1], cong.Arc2[2]);
        if (seg1 is not null && seg2 is not null)
        {
            var pred2 = new QuantityRatio(seg1.Length, seg2.Length);
            pred2.AddReason();
            pred2.AddCondition(cong);
            updater.Add(pred2);
        }
    }
    #endregion

}
