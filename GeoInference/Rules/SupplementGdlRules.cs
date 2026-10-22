
[RuleType(RuleType.Tradition)]
public class SupplementGdlRules : RuleClass
{
    #region 1. 平行线的角判定（3 条：角关系 → 平行，与 RuleAQ007/008/009 方向相反）

    /// <summary>
    /// </summary>
    [Alias("同位角相等，两直线平行")]
    public void RuleGdl01ParallelJudgmentByCorrespondingAngles(AngleSizeEqual equal, PointsOnLineSameSide sameSide)
    {
        var s0 = sameSide[0];
        var s1 = sameSide[1];
        var s2 = sameSide[2];
        var s3 = sameSide[3];
        var transversal = GetLine(s0, s1);
        var line1 = GetLine(s0, s2);
        var line2 = GetLine(s1, s3);
        if (transversal is null || line1 is null || line2 is null) return;
        if (line1 == line2 || line1 == transversal || line2 == transversal) return;

        var tFirst = transversal.Properties.First();
        var tLast = transversal.Properties.Last();
        var a1 = GetAngle(tFirst, s0, s2);
        var a2 = GetAngle(tFirst, s1, s3);
        var a3 = GetAngle(tLast, s0, s2);
        var a4 = GetAngle(tLast, s1, s3);
        if (!MatchesAngleEqual(equal, a1, a2) && !MatchesAngleEqual(equal, a3, a4)) return;

        LineParallel pred = new LineParallel(line1, line2);
        pred.AddReason();
        pred.AddCondition(equal, sameSide);
        updater.Add(pred);
    }

    /// <summary>
    /// </summary>
    [Alias("内错角相等，两直线平行")]
    public void RuleGdl02ParallelJudgmentByAlternateInteriorAngles(AngleSizeEqual equal, PointsOnLineDifferentSide diffSide)
    {
        var s0 = diffSide[0];
        var s1 = diffSide[1];
        var s2 = diffSide[2];
        var s3 = diffSide[3];
        var transversal = GetLine(s0, s1);
        var line1 = GetLine(s0, s2);
        var line2 = GetLine(s1, s3);
        if (transversal is null || line1 is null || line2 is null) return;
        if (line1 == line2 || line1 == transversal || line2 == transversal) return;

        var a1 = GetAngle(s2, s0, s1);
        var a2 = GetAngle(s3, s1, s0);
        if (!MatchesAngleEqual(equal, a1, a2)) return;

        LineParallel pred = new LineParallel(line1, line2);
        pred.AddReason();
        pred.AddCondition(equal, diffSide);
        updater.Add(pred);
    }

    /// <summary>
    /// </summary>
    [Alias("同旁内角互补，两直线平行")]
    public void RuleGdl03ParallelJudgmentByIpsilateralInternalAngles(PointsOnLineSameSide sameSide)
    {
        var s0 = sameSide[0];
        var s1 = sameSide[1];
        var s2 = sameSide[2];
        var s3 = sameSide[3];
        var a1 = GetAngle(s2, s0, s1);
        var a2 = GetAngle(s3, s1, s0);
        if (a1 is null || a2 is null) return;

        var eq = KnowledgeBase.LinearEquations.Values.FirstOrDefault(e =>
            e.Coff.Count == 2 && e.Constant == 180 &&
            e.Coff.TryGetValue(a1.Size, out var c1) && c1 == 1 &&
            e.Coff.TryGetValue(a2.Size, out var c2) && c2 == 1);
        if (eq is null) return;

        var line1 = GetLine(s0, s2);
        var line2 = GetLine(s1, s3);
        if (line1 is null || line2 is null || line1 == line2) return;

        LineParallel pred = new LineParallel(line1, line2);
        pred.AddReason();
        pred.AddCondition(sameSide, eq);
        updater.Add(pred);
    }

    #endregion

    #region 2. 垂直平分线判定（1 条）

    /// <summary>
    /// perpendicular_bisector_judgment_distance_equal：
    /// </summary>
    [Alias("到线段两端距离相等的点在其垂直平分线上")]
    public void RuleGdl04PerpendicularBisectorJudgmentByEqualDistance(Midpoint midpoint, SegmentLengthEqual equal)
    {
        var a = midpoint[1];
        var b = midpoint[2];
        var m = midpoint[0];
        var result = FindCommon(equal.Seg1, equal.Seg2);
        if (result.common is null) return;
        var p = result.common;
        if (!((result.notcommon1 == a && result.notcommon2 == b) ||
              (result.notcommon1 == b && result.notcommon2 == a))) return;
        if (p == m) return;
        if (HasColine(p, a, b)) return;

        var seg = GetSegment(a, b);
        var line = GetLine(p, m);
        if (seg is null || line is null) return;

        VerticalBisectorLine pred = new VerticalBisectorLine(seg, line);
        pred.AddReason();
        pred.AddCondition(midpoint, equal);
        updater.Add(pred);
    }

    #endregion

    #region 3. 角平分线（2 条）

    /// <summary>
    /// </summary>
    [Alias("角平分线分对边成比例")]
    public void RuleGdl05AngleBisectorDividesOppositeSideProportionally(Triangle tri, AngularBisectorLine bisector)
    {
        var match = MatchBisectorFootInTriangle(tri, bisector);
        if (match is null) return;
        var (vertex, e1, e2, d) = match.Value;

        var ab = GetSegment(vertex, e1);
        var ac = GetSegment(vertex, e2);
        var bd = GetSegment(d, e1);
        var dc = GetSegment(d, e2);
        if (ab is null || ac is null || bd is null || dc is null) return;

        ProductionEquation pred = new ProductionEquation(
            new() { { ab.Length, 1 }, { dc.Length, 1 }, { ac.Length, -1 }, { bd.Length, -1 } }, 1);
        pred.AddReason();
        pred.AddCondition(tri, bisector);
        updater.Add(pred);
    }

    /// <summary>
    /// </summary>
    [Alias("角平分线长度公式")]
    public void RuleGdl06AngleBisectorLengthFormula(Triangle tri, AngularBisectorLine bisector)
    {
        var match = MatchBisectorFootInTriangle(tri, bisector);
        if (match is null) return;
        var (vertex, e1, e2, d) = match.Value;

        var ab = GetSegment(vertex, e1);
        var ac = GetSegment(vertex, e2);
        var bd = GetSegment(d, e1);
        var dc = GetSegment(d, e2);
        var ad = GetSegment(vertex, d);
        if (ab is null || ac is null || bd is null || dc is null || ad is null) return;

        Equation pred = new Equation(ad.Length.Pow(2), ab.Length * ac.Length - bd.Length * dc.Length);
        pred.AddReason();
        pred.AddCondition(tri, bisector);
        updater.Add(pred);
    }

    /// <summary>
    /// </summary>
    private (Point vertex, Point e1, Point e2, Point d)? MatchBisectorFootInTriangle(Triangle tri, AngularBisectorLine bisector)
    {
        var vertex = bisector.Angle.Vertex;
        if (!tri.Properties.Contains(vertex)) return null;
        Point e1 = null, e2 = null;
        foreach (Point v in tri.Properties)
        {
            if (v == vertex) continue;
            if (bisector.Angle.Edge1.Contains(v)) e1 = v;
            if (bisector.Angle.Edge2.Contains(v)) e2 = v;
        }
        if (e1 is null || e2 is null) return null;
        var opposite = GetLine(e1, e2);
        if (opposite is null) return null;
        var d = FindCIntersection(bisector.Bisector, opposite);
        if (d is null) return null;
        if (!IsBetweenPoint((Point)d, e1, e2)) return null;
        return ((Point)vertex, e1, e2, (Point)d);
    }

    #endregion

    #region 4. 相似三角形判定（1 条）

    /// <summary>
    /// </summary>
    [Alias("直角三角形相似判定（HL）")]
    [SemiConditionRule]
    public void RuleGdl07RightTriangleSimilarityHL(RightTriangle triangle1, RightTriangle triangle2, SegmentLengthRatio ratio)
    {
        if (triangle1 == triangle2) return;
        if (triangle1.Properties.All(triangle2.Properties.Contains)) return;
        Point a = (Point)triangle1[0];
        Point b = (Point)triangle1[1];
        Point c = (Point)triangle1[2];
        Point d = (Point)triangle2[0];
        Point e = (Point)triangle2[1];
        Point f = (Point)triangle2[2];

        var ab = GetSegment(triangle1[0], triangle1[1]);
        var bc = GetSegment(triangle1[1], triangle1[2]);
        var ca = GetSegment(triangle1[0], triangle1[2]);
        var de = GetSegment(triangle2[0], triangle2[1]);
        var ef = GetSegment(triangle2[1], triangle2[2]);
        var fd = GetSegment(triangle2[0], triangle2[2]);

        Segment seg1, seg2;
        Expr r;
        if (ratio.Seg1.Properties.TrueForAll(triangle1.Properties.Contains) && ratio.Seg2.Properties.TrueForAll(triangle2.Properties.Contains))
        {
            seg1 = ratio.Seg1; seg2 = ratio.Seg2; r = ratio.Expr;
        }
        else if (ratio.Seg2.Properties.TrueForAll(triangle1.Properties.Contains) && ratio.Seg1.Properties.TrueForAll(triangle2.Properties.Contains))
        {
            seg2 = ratio.Seg1; seg1 = ratio.Seg2; r = ratio.Expr.Invert();
        }
        else
            return;

        void AddSimilarity(Point d1, Point e1, Point f1, Segment leg1, Segment leg2, Segment hyp1, Segment hyp2, bool hypGiven)
        {
            var condSeg1 = hypGiven ? leg1 : hyp1;
            var condSeg2 = hypGiven ? leg2 : hyp2;
            ConditionalKnowledge ck = new() { Knowledge = new SimilarTriangles(a, b, c, d1, e1, f1) };
            ck.AddCondition(new SegmentLengthRatio(condSeg1, condSeg2, r));
            ck.Knowledge.AddReason();
            ck.Knowledge.AddCondition(triangle1, triangle2, ratio);
            updater.AddConditionalKnowledgePair(ck);
        }

        if (seg1 == bc && seg2 == ef)
        {
            AddSimilarity(d, e, f, ab, de, bc, ef, true);
            AddSimilarity(d, e, f, ca, fd, bc, ef, true);
            AddSimilarity(d, f, e, ab, fd, bc, ef, true);
            AddSimilarity(d, f, e, ca, de, bc, ef, true);
        }
        else if (seg1 == ab && seg2 == de)
        {
            AddSimilarity(d, e, f, ab, de, bc, ef, false);
        }
        else if (seg1 == ca && seg2 == fd)
        {
            AddSimilarity(d, e, f, ca, fd, bc, ef, false);
        }
        else if (seg1 == ab && seg2 == fd)
        {
            AddSimilarity(d, f, e, ab, fd, bc, ef, false);
        }
        else if (seg1 == ca && seg2 == de)
        {
            AddSimilarity(d, f, e, ca, de, bc, ef, false);
        }
    }

    #endregion

    #region 5. 圆（4 条）

    /// <summary>
    /// </summary>
    [Alias("弦切角定理")]
    public void RuleGdl08ChordTangentAngleTheorem(Circle circle, CircleTangent tangent)
    {
        if (tangent.Circle != circle) return;
        if (FindCIntersection(circle.Properties, tangent.Tangent.Properties) is not Point c) return;
        var onCircle = circle.Properties.Skip(1).ToList();
        foreach (Point p in onCircle)
        {
            if (p == c) continue;
            foreach (Point t in tangent.Tangent.Properties)
            {
                if (t == c) continue;
                var tangentAngle = GetAngle(t, c, p);
                if (tangentAngle is null) continue;
                foreach (Point q in onCircle)
                {
                    if (q == c || q == p) continue;
                    var inscribed = GetAngle(c, q, p);
                    if (inscribed is null) continue;
                    if (IsBetweenAngle(c, t, p, q))
                    {
                        LinearEquation pred = new LinearEquation(
                            new() { { tangentAngle.Size, 1 }, { inscribed.Size, 1 } }, 180);
                        pred.AddReason();
                        pred.AddCondition(circle, tangent);
                        updater.Add(pred);
                    }
                    else
                    {
                        QuantityRatio pred = new QuantityRatio(tangentAngle.Size, inscribed.Size);
                        pred.AddReason();
                        pred.AddCondition(circle, tangent);
                        updater.Add(pred);
                    }
                }
            }
        }
    }

    /// <summary>
    /// </summary>
    [Alias("圆幂定理（切割线定理）")]
    public void RuleGdl09CircularPowerTangentAndSecant(Circle circle, CircleTangent tangent, Line secant)
    {
        if (tangent.Circle != circle) return;
        if (secant == tangent.Tangent) return;
        var tp = FindCIntersection(circle.Properties, tangent.Tangent.Properties);
        var external = FindCIntersection(tangent.Tangent.Properties, secant.Properties);
        if (tp is null || external is null) return;
        var onCircle = circle.Properties.Skip(1).ToList();
        var cross = secant.Properties.Where(onCircle.Contains).ToList();
        if (cross.Count != 2) return;

        var pt = GetSegment(external, tp);
        var pa = GetSegment(external, cross[0]);
        var pb = GetSegment(external, cross[1]);
        if (pt is null || pa is null || pb is null) return;

        ProductionEquation pred = new ProductionEquation(
            new() { { pt.Length, 2 }, { pa.Length, -1 }, { pb.Length, -1 } }, 1);
        pred.AddReason();
        pred.AddCondition(circle, tangent, secant);
        updater.Add(pred);
    }
    /// <summary>
    /// </summary>
    [Alias("圆幂定理（切线-割线的角度形式）")]
    public void RuleGdl10CircularPowerTangentAndSecantAngle(Circle circle, CircleTangent tangent, Angle angle)
    {
        if (tangent.Circle != circle) return;
        var tp = FindCIntersection(circle.Properties, tangent.Tangent.Properties);
        if (tp is not Point tpp) return;
        var onCircle = circle.Properties.Skip(1).ToList();
        if (onCircle.Contains(angle.Vertex)) return;
        var e1 = angle.Edge1.Where(onCircle.Contains).ToList();
        var e2 = angle.Edge2.Where(onCircle.Contains).ToList();
        List<Point> sec;
        if (e1.Count == 1 && e1[0] == tpp && e2.Count == 2)
            sec = e2;
        else if (e2.Count == 1 && e2[0] == tpp && e1.Count == 2)
            sec = e1;
        else
            return;

        sec = SortByDistanceFrom(angle.Vertex, sec);

        var nearArc = GetArc(circle.Center, tpp, sec[0]);
        var farArc = GetArc(circle.Center, tpp, sec[^1]);
        if (nearArc is null || farArc is null) return;

        // 截弧不含另一交点：若远点在劣弧 TN 上则近截弧为优弧，若近点在劣弧 TF 上则远截弧为优弧
        bool nearIsMinor = IsInterceptedArcMinor(circle.Center, tpp, sec[0], sec[^1]);
        bool farIsMinor = IsInterceptedArcMinor(circle.Center, tpp, sec[^1], sec[0]);

        LinearEquation pred = ExteriorAngleArcEquation(angle.Size, nearArc, nearIsMinor, farArc, farIsMinor);
        pred.AddReason();
        pred.AddCondition(circle, tangent, angle);
        updater.Add(pred);
    }

    /// <summary>
    /// </summary>
    [Alias("扇形周长公式")]
    public void RuleGdl11SectorPerimeterFormula(Sector sector)
    {
        var c = sector[0];
        var f = sector[1];
        var e = sector.Properties.Last();
        var arc = GetArc(c, f, e);
        var radius = GetSegment(c, f);
        if (arc is null || radius is null) return;

        LinearEquation pred = new LinearEquation(
            new() { { arc.MinorArcLength, 1 }, { radius.Length, 2 }, { sector.Perimeter, -1 } }, 0);
        pred.AddReason();
        pred.AddCondition(sector);
        updater.Add(pred);
    }

    #endregion

    #region 6. 其他（4 条）

    /// <summary>
    /// </summary>
    [Alias("直角梯形的判定（有一个角是直角）")]
    public void RuleGdl12RightTrapezoidJudgmentByRightAngle(Trapezoid trapezoid, AngleSize angleSize)
    {
        if (angleSize.Expr != "90") return;
        var q0 = (Point)trapezoid[0];
        var q1 = (Point)trapezoid[1];
        var q2 = (Point)trapezoid[2];
        var q3 = (Point)trapezoid[3];
        var a0 = GetAngle(q1, q0, q3);
        var a1 = GetAngle(q0, q1, q2);
        var a2 = GetAngle(q1, q2, q3);
        var a3 = GetAngle(q2, q3, q0);

        RightTrapezoid pred = null;
        if (angleSize.Angle == a1 || angleSize.Angle == a2)
        {
            pred = new RightTrapezoid(q0, q1, q2, q3);
        }
        else if (angleSize.Angle == a0)
        {
            pred = new RightTrapezoid(q1, q0, q3, q2);
        }
        else if (angleSize.Angle == a3)
        {
            pred = new RightTrapezoid(q2, q3, q0, q1);
        }
        if (pred is null) return;
        pred.AddReason();
        pred.AddCondition(trapezoid, angleSize);
        updater.Add(pred);
    }

    /// <summary>
    /// </summary>
    [Alias("圆内接四边形的外心在各边的垂直平分线上")]
    public void RuleGdl13CircumcenterOfCyclicQuadrilateralOnPerpendicularBisector(
        Circle circle, Quadrilateral quad, VerticalBisectorLine verticalBisector)
    {
        if (!quad.Properties.TrueForAll(circle.Properties.Skip(1).Contains)) return;
        var s1 = GetSegment(quad[0], quad[1]);
        var s2 = GetSegment(quad[1], quad[2]);
        var s3 = GetSegment(quad[2], quad[3]);
        var s4 = GetSegment(quad[3], quad[0]);
        var isSide = s1 is not null && verticalBisector.Seg == s1
                  || s2 is not null && verticalBisector.Seg == s2
                  || s3 is not null && verticalBisector.Seg == s3
                  || s4 is not null && verticalBisector.Seg == s4;
        if (!isSide) return;

        var o = (Point)circle.Center;
        var pts = verticalBisector.Bisector.Properties.OfType<Point>().Where(p => p != o).ToList();
        for (int i = 0; i < pts.Count; i++)
        {
            for (int j = i + 1; j < pts.Count; j++)
            {
                var pred = CreateColinePredicate(o, pts[i], pts[j]);
                if (pred is null) continue;
                pred.AddReason();
                pred.AddCondition(circle, quad, verticalBisector);
                updater.Add(pred);
            }
        }
    }

    /// <summary>
    /// </summary>
    [Alias("平行四边形面积的正弦公式")]
    public void RuleGdl14ParallelogramAreaFormulaSine(Parallelogram para)
    {
        var quad = GetQuadrilateral((Point)para[0], (Point)para[1], (Point)para[2], (Point)para[3]);
        if (quad is null) return;
        var ab = GetSegment(para[0], para[1]);
        var bc = GetSegment(para[1], para[2]);
        var da = GetSegment(para[3], para[0]);
        var abc = GetAngle(para[0], para[1], para[2]);
        var dab = GetAngle(para[1], para[0], para[3]);
        if (ab is null || bc is null || da is null || abc is null || dab is null) return;

        ProductionEquation pred1 = new ProductionEquation(
            new() { { ab.Length, 1 }, { bc.Length, 1 }, { abc.Sin, 1 }, { quad.Area, -1 } }, 1);
        pred1.AddReason();
        pred1.AddCondition(para);
        updater.Add(pred1);

        ProductionEquation pred2 = new ProductionEquation(
            new() { { da.Length, 1 }, { ab.Length, 1 }, { dab.Sin, 1 }, { quad.Area, -1 } }, 1);
        pred2.AddReason();
        pred2.AddCondition(para);
        updater.Add(pred2);
    }

    /// <summary>
    /// </summary>
    [Alias("筝形面积的正弦公式")]
    public void RuleGdl15KiteAreaFormulaSine(Kite kite)
    {
        var quad = GetQuadrilateral((Point)kite[0], (Point)kite[1], (Point)kite[2], (Point)kite[3]);
        if (quad is null) return;
        var ab = GetSegment(kite[0], kite[1]);
        var bc = GetSegment(kite[1], kite[2]);
        var cd = GetSegment(kite[2], kite[3]);
        var da = GetSegment(kite[3], kite[0]);
        var abc = GetAngle(kite[0], kite[1], kite[2]);
        var cda = GetAngle(kite[2], kite[3], kite[0]);
        if (ab is null || bc is null || cd is null || da is null || abc is null || cda is null) return;

        ProductionEquation pred1 = new ProductionEquation(
            new() { { ab.Length, 1 }, { bc.Length, 1 }, { abc.Sin, 1 }, { quad.Area, -1 } }, 1);
        pred1.AddReason();
        pred1.AddCondition(kite);
        updater.Add(pred1);

        ProductionEquation pred2 = new ProductionEquation(
            new() { { cd.Length, 1 }, { da.Length, 1 }, { cda.Sin, 1 }, { quad.Area, -1 } }, 1);
        pred2.AddReason();
        pred2.AddCondition(kite);
        updater.Add(pred2);
    }

    #endregion

    #region 7. 切线判定与直径判定（2 条）

    /// <summary>
    /// tangent_of_circle_judgment_perpendicular：
    /// </summary>
    [Alias("切线的判定（过半径外端且垂直于半径）")]
    public void RuleGdl16TangentJudgmentByPerpendicular(Circle circle, LinePerpendicular perpendicular)
    {
        Line radiusLine = null;
        Line tangentLine = null;
        if (perpendicular[0].Properties.Contains(circle.Center) && !perpendicular[1].Properties.Contains(circle.Center))
        {
            radiusLine = (Line)perpendicular[0];
            tangentLine = (Line)perpendicular[1];
        }
        else if (perpendicular[1].Properties.Contains(circle.Center) && !perpendicular[0].Properties.Contains(circle.Center))
        {
            radiusLine = (Line)perpendicular[1];
            tangentLine = (Line)perpendicular[0];
        }
        else
            return;

        if (FindCIntersection(radiusLine.Properties, tangentLine.Properties) is not Point touch) return;
        if (!circle.Properties.Skip(1).Contains(touch)) return;

        CircleTangent pred = new CircleTangent(circle, tangentLine);
        pred.AddReason();
        pred.AddCondition(circle, perpendicular);
        updater.Add(pred);
    }

    /// <summary>
    /// diameter_of_circle_judgment_right_angle：
    /// </summary>
    [Alias("直径的判定（90°圆周角）")]
    public void RuleGdl17DiameterJudgmentByRightAngle(Circle circle, AngleSize angleSize)
    {
        if (angleSize.Expr != "90") return;
        var vertex = angleSize.Angle.Vertex;
        var onCircle = circle.Properties.Skip(1).ToList();
        if (!onCircle.Contains(vertex)) return;

        var p1 = angleSize.Angle.Edge1.FirstOrDefault(p => p != vertex && onCircle.Contains(p));
        var p2 = angleSize.Angle.Edge2.FirstOrDefault(p => p != vertex && onCircle.Contains(p));
        if (p1 is null || p2 is null || p1 == p2) return;

        var seg = GetSegment(p1, p2);
        if (seg is null) return;

        CircleDiameter pred = new CircleDiameter(circle, seg);
        pred.AddReason();
        pred.AddCondition(circle, angleSize);
        updater.Add(pred);
    }

    #endregion

    #region 8. 直径的长度判定与中位线平行性质（3 条）

    /// <summary>
    /// diameter_of_circle_judgment_length_equal：
    /// 输入"线段长度等于圆直径"谓词（由 QuantityRatio.ToPred 在比值方程产生时自动回填，
    /// 方程求解后续推出的相等关系同样会触发），检查线段两端点是否在圆上。
    /// </summary>
    [Alias("直径的判定（弦长等于直径）")]
    public void RuleGdl18DiameterJudgmentByLengthEqual(SegmentEqualCircleDiameter diameterEqual)
    {
        var circle = diameterEqual.Circle;
        var segment = diameterEqual.Seg;
        var onCircle = circle.Properties.Skip(1).ToList();
        if (!onCircle.Contains(segment[0]) || !onCircle.Contains(segment[1])) return;

        CircleDiameter pred = new CircleDiameter(circle, segment);
        pred.AddReason();
        pred.AddCondition(diameterEqual);
        updater.Add(pred);
    }

    /// <summary>
    /// midsegment_of_quadrilateral_property_parallel（梯形）：
    /// </summary>
    [Alias("梯形中位线平行于两底")]
    public void RuleGdl19TrapezoidMidsegmentParallelToBases(Trapezoid trapezoid, Midpoint mid1, Midpoint mid2)
    {
        if (mid1 == mid2) return;
        var bcS = GetSegment(trapezoid[1], trapezoid[2]);
        var daS = GetSegment(trapezoid[3], trapezoid[0]);
        var midS1 = GetSegment(mid1[1], mid1[2]);
        var midS2 = GetSegment(mid2[1], mid2[2]);
        if (bcS is null || daS is null || midS1 is null || midS2 is null) return;
        if (!(bcS == midS1 && daS == midS2 || bcS == midS2 && daS == midS1)) return;

        var midL = GetLine(mid1[0], mid2[0]);
        var abL = GetLine(trapezoid[0], trapezoid[1]);
        var cdL = GetLine(trapezoid[2], trapezoid[3]);
        if (midL is null || abL is null || cdL is null) return;
        if (midL == abL || midL == cdL) return;

        LineParallel pred1 = new LineParallel(midL, abL);
        pred1.AddReason();
        pred1.AddCondition(trapezoid, mid1, mid2);
        updater.Add(pred1);
        LineParallel pred2 = new LineParallel(midL, cdL);
        pred2.AddReason();
        pred2.AddCondition(trapezoid, mid1, mid2);
        updater.Add(pred2);
    }

    /// <summary>
    /// midsegment_of_quadrilateral_property_parallel（平行四边形）：
    /// </summary>
    [Alias("平行四边形对边中点连线平行于另两边")]
    public void RuleGdl20ParallelogramMidsegmentParallelToBases(Parallelogram parallelogram, Midpoint mid1, Midpoint mid2)
    {
        if (mid1 == mid2) return;
        var abS = GetSegment(parallelogram[0], parallelogram[1]);
        var cdS = GetSegment(parallelogram[2], parallelogram[3]);
        var bcS = GetSegment(parallelogram[1], parallelogram[2]);
        var daS = GetSegment(parallelogram[3], parallelogram[0]);
        var midS1 = GetSegment(mid1[1], mid1[2]);
        var midS2 = GetSegment(mid2[1], mid2[2]);
        if (abS is null || cdS is null || bcS is null || daS is null || midS1 is null || midS2 is null) return;

        Line baseL1, baseL2;
        if (abS == midS1 && cdS == midS2 || abS == midS2 && cdS == midS1)
        {
            baseL1 = GetLine(parallelogram[1], parallelogram[2]);
            baseL2 = GetLine(parallelogram[3], parallelogram[0]);
        }
        else if (bcS == midS1 && daS == midS2 || bcS == midS2 && daS == midS1)
        {
            baseL1 = GetLine(parallelogram[0], parallelogram[1]);
            baseL2 = GetLine(parallelogram[2], parallelogram[3]);
        }
        else return;
        if (baseL1 is null || baseL2 is null) return;

        var midL = GetLine(mid1[0], mid2[0]);
        if (midL is null || midL == baseL1 || midL == baseL2) return;

        LineParallel pred1 = new LineParallel(midL, baseL1);
        pred1.AddReason();
        pred1.AddCondition(parallelogram, mid1, mid2);
        updater.Add(pred1);
        LineParallel pred2 = new LineParallel(midL, baseL2);
        pred2.AddReason();
        pred2.AddCondition(parallelogram, mid1, mid2);
        updater.Add(pred2);
    }

    #endregion

    #region 私有辅助

    private static bool MatchesAngleEqual(AngleSizeEqual equal, Angle a1, Angle a2)
    {
        if (a1 is null || a2 is null) return false;
        return equal.Angle1 == a1 && equal.Angle2 == a2 ||
               equal.Angle1 == a2 && equal.Angle2 == a1;
    }

    private static PointOnSeg CreateColinePredicate(Point a, Point b, Point c)
    {
        if (IsBetweenPoint(a, b, c)) return new PointOnSeg(a, b, c);
        if (IsBetweenPoint(b, a, c)) return new PointOnSeg(b, a, c);
        if (IsBetweenPoint(c, a, b)) return new PointOnSeg(c, a, b);
        return null;
    }

    private static bool IsBetweenPoint(Point mid, Point p1, Point p2)
    {
        const double Epsilon = 1e-9;
        return (mid.X - p1.X) * (p2.X - mid.X) >= -Epsilon && (mid.Y - p1.Y) * (p2.Y - mid.Y) >= -Epsilon;
    }

    #endregion
}
