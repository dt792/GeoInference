
[RuleType(RuleType.Tradition)]
public class RuleTriHeart : RuleClass
{
    [Alias("三角形垂心的判定")]
    public void RuleTriH001OrthocenterDetermination(Triangle tri, LinePerpendicular perpendicular1, LinePerpendicular perpendicular2)
    {
        if (perpendicular1 == perpendicular2) return;
        var a = (Point)tri[0];
        var b = (Point)tri[1];
        var c = (Point)tri[2];
        var ab = GetLine(tri[0], tri[1]);
        var bc = GetLine(tri[1], tri[2]);
        var ca = GetLine(tri[2], tri[0]);
        Line v1 = null, v2 = null;
        Line nv1 = null, nv2 = null;
        if (ab == perpendicular1.Line1 && perpendicular1.Line2.Contains(c))
        { nv1 = perpendicular1.Line1; v1 = perpendicular1.Line2; }
        else if (ab == perpendicular1.Line2 && perpendicular1.Line1.Contains(c))
        { nv1 = perpendicular1.Line2; v1 = perpendicular1.Line1; }
        else if (bc == perpendicular1.Line1 && perpendicular1.Line2.Contains(a))
        { nv1 = perpendicular1.Line1; v1 = perpendicular1.Line2; }
        else if (bc == perpendicular1.Line2 && perpendicular1.Line1.Contains(a))
        { nv1 = perpendicular1.Line2; v1 = perpendicular1.Line1; }
        else if (ca == perpendicular1.Line1 && perpendicular1.Line2.Contains(b))
        { nv1 = perpendicular1.Line1; v1 = perpendicular1.Line2; }
        else if (ca == perpendicular1.Line2 && perpendicular1.Line1.Contains(b))
        { nv1 = perpendicular1.Line2; v1 = perpendicular1.Line1; }

        if (ab == perpendicular2.Line1 && perpendicular2.Line2.Contains(c))
        { nv2 = perpendicular2.Line1; v2 = perpendicular2.Line2; }
        else if (ab == perpendicular2.Line2 && perpendicular2.Line1.Contains(c))
        { nv2 = perpendicular2.Line2; v2 = perpendicular2.Line1; }
        else if (bc == perpendicular2.Line1 && perpendicular2.Line2.Contains(a))
        { nv2 = perpendicular2.Line1; v2 = perpendicular2.Line2; }
        else if (bc == perpendicular2.Line2 && perpendicular2.Line1.Contains(a))
        { nv2 = perpendicular2.Line2; v2 = perpendicular2.Line1; }
        else if (ca == perpendicular2.Line1 && perpendicular2.Line2.Contains(b))
        { nv2 = perpendicular2.Line1; v2 = perpendicular2.Line2; }
        else if (ca == perpendicular2.Line2 && perpendicular2.Line1.Contains(b))
        { nv2 = perpendicular2.Line2; v2 = perpendicular2.Line1; }
        if (v1 is null || v2 is null) return;
        if (nv1 == nv2) return;
        var cc = FindCIntersection(v1, v2);
        if (cc is not null)
        {
            TriangleOrthocenter pred = new((Point)cc, tri);
            pred.AddReason();
            pred.AddCondition(tri, perpendicular1, perpendicular2);
            updater.Add(pred);
        }
    }

    [Alias("三角形重心的判定")]
    public void RuleTriH002CentroidDetermination(Triangle tri, Midpoint mid1, Midpoint mid2)
    {
        if (mid1 == mid2) return;
        var a = (Point)tri[0];
        var b = (Point)tri[1];
        var c = (Point)tri[2];
        var ab = GetSegment(tri[0], tri[1]);
        var bc = GetSegment(tri[1], tri[2]);
        var ca = GetSegment(tri[2], tri[0]);
        var ml1 = GetSegment(mid1[1], mid1[2]);
        var ml2 = GetSegment(mid2[1], mid2[2]);
        Point v1 = null, v2 = null;
        if (ml1 == ab)
            v1 = c;
        else if (ml1 == bc)
            v1 = a;
        else if (ml1 == ca)
            v1 = b;
        else
            return;
        if (ml2 == ab)
            v2 = c;
        else if (ml2 == bc)
            v2 = a;
        else if (ml2 == ca)
            v2 = b;
        else
            return;
        var l1 = GetLine(mid1[0], v1);
        var l2 = GetLine(mid2[0], v2);
        if (l1 is null || l2 is null) return;
        var cc = FindCIntersection(l1, l2);
        if (cc is not null)
        {
            TriangleCentroid pred = new((Point)cc, tri);
            pred.AddReason();
            pred.AddCondition(tri, mid1, mid2);
            updater.Add(pred);
        }
    }

    [Alias("三角形外心的判定")]
    public void RuleTriH003CircumcenterDetermination(Triangle tri, SegmentLengthEqual eq1, SegmentLengthEqual eq2)
    {
        if (eq1 == eq2) return;
        var (c, nc1, nc2) = FindCommon(eq1, eq2);
        if (c is null) return;
        var a = DictionaryTool.CountItemNum(c.Properties.Union(nc1.Properties).Union(nc2.Properties));
        if (a.ContainsKey(tri[0]) && a.ContainsKey(tri[1]) && a.ContainsKey(tri[2]))
            ;
        else
            return;
        Point p = null;
        foreach (var item in a)
        {
            if (!tri.Properties.Contains(item.Key) && item.Value == 3)
                p = (Point)item.Key;
        }
        if (p is not null)
        {
            TriangleCircumcenter pred = new((Point)p, tri);
            pred.AddReason();
            pred.AddCondition(tri, eq1, eq2);
            updater.Add(pred);
        }
    }

    [Alias("三角形内心的判定")]
    public void RuleTriH004IncenterDetermination(Triangle tri, AngularBisectorLine a1, AngularBisectorLine a2)
    {
        if (a1 == a2) return;
        var c = FindCIntersection(a1.Bisector, a2.Bisector);
        if (c is null) return;
        Angle abc = GetAngle(tri[0], tri[1], tri[2]);
        Angle bca = GetAngle(tri[1], tri[2], tri[0]);
        Angle cab = GetAngle(tri[2], tri[0], tri[1]);
        List<Angle> s = [abc, bca, cab];
        if (s.Contains(a1.Angle) && s.Contains(a2.Angle))
        {
            TriangleIncenter pred = new((Point)c, tri);
            pred.AddReason();
            pred.AddCondition(tri, a1, a2);
            updater.Add(pred);
        }
    }

    [Alias("三角形垂心的性质")]
    public void RuleTriH005OrthocenterProperties(TriangleOrthocenter ort)
    {
        var tri = ort[1];
        var a = (Point)tri[0];
        var b = (Point)tri[1];
        var c = (Point)tri[2];
        var ab = GetLine(tri[0], tri[1]);
        var bc = GetLine(tri[1], tri[2]);
        var ca = GetLine(tri[2], tri[0]);
        var oa = GetLine(ort[0], tri[0]);
        var ob = GetLine(ort[0], tri[1]);
        var oc = GetLine(ort[0], tri[2]);
        if (oa is not null)
        {
            var pred = new LinePerpendicular(bc, oa);
            pred.AddReason();
            pred.AddCondition(ort);
            updater.Add(pred);
        }
        if (ob is not null)
        {
            var pred = new LinePerpendicular(ca, ob);
            pred.AddReason();
            pred.AddCondition(ort);
            updater.Add(pred);
        }
        if (oc is not null)
        {
            var pred = new LinePerpendicular(ab, oc);
            pred.AddReason();
            pred.AddCondition(ort);
            updater.Add(pred);
        }
    }

    [Alias("三角形重心的性质")]
    public void RuleTriH006CentroidProperties(TriangleCentroid centroid, Midpoint midpoint)
    {
        var tri = (Triangle)centroid[1];
        var on = GetSegment(midpoint[1], midpoint[2]);
        var abS = GetSegment(tri[0], tri[1]);
        var bcS = GetSegment(tri[1], tri[2]);
        var caS = GetSegment(tri[2], tri[0]);
        if (on == abS)
        {
            var top = GetSegment(centroid[0], tri[2]);
            var bottom = GetSegment(centroid[0], midpoint[0]);
            if (top is null || bottom is null) return;
            var pred = new QuantityRatio(top.Length, bottom.Length, 2);
            pred.AddReason();
            pred.AddCondition(centroid, midpoint);
            updater.Add(pred);
        }
        else if (on == bcS)
        {
            var top = GetSegment(centroid[0], tri[0]);
            var bottom = GetSegment(centroid[0], midpoint[0]);
            if (top is null || bottom is null) return;
            var pred = new QuantityRatio(top.Length, bottom.Length, 2);
            pred.AddReason();
            pred.AddCondition(centroid, midpoint);
            updater.Add(pred);
        }
        else if (on == caS)
        {
            var top = GetSegment(centroid[0], tri[1]);
            var bottom = GetSegment(centroid[0], midpoint[0]);
            if (top is null || bottom is null) return;
            var pred = new QuantityRatio(top.Length, bottom.Length, 2);
            pred.AddReason();
            pred.AddCondition(centroid, midpoint);
            updater.Add(pred);
        }
    }

    [Alias("三角形外心的性质")]
    public void RuleTriH007CircumcenterProperties(TriangleCircumcenter circumcenter)
    {
        var tri = (Triangle)circumcenter[1];
        var aoS = GetSegment(circumcenter[0], tri[0]);
        var boS = GetSegment(circumcenter[0], tri[1]);
        var coS = GetSegment(circumcenter[0], tri[2]);
        if (aoS is not null && boS is not null)
        {
            var pred = new QuantityRatio(aoS.Length, boS.Length);
            pred.AddReason();
            pred.AddCondition(circumcenter);
            updater.Add(pred);
        }
        if (coS is not null && boS is not null)
        {
            var pred = new QuantityRatio(coS.Length, boS.Length);
            pred.AddReason();
            pred.AddCondition(circumcenter);
            updater.Add(pred);
        }
        if (aoS is not null && coS is not null)
        {
            var pred = new QuantityRatio(aoS.Length, coS.Length);
            pred.AddReason();
            pred.AddCondition(circumcenter);
            updater.Add(pred);
        }
    }

    [Alias("三角形中线重合于一点")]
    public void RuleTriH009CentroidOnMedian(TriangleCentroid centroid, Midpoint midpoint)
    {
        var tri = (Triangle)centroid[1];
        var g = (Point)centroid[0];
        var m = (Point)midpoint[0];
        if (g == m) return;
        Point top = null;
        if (tri[1] == midpoint[1] && tri[2] == midpoint[2] || tri[2] == midpoint[1] && tri[1] == midpoint[2])
            top = (Point)tri[0];
        else if (tri[0] == midpoint[1] && tri[2] == midpoint[2] || tri[2] == midpoint[1] && tri[0] == midpoint[2])
            top = (Point)tri[1];
        else if (tri[0] == midpoint[1] && tri[1] == midpoint[2] || tri[1] == midpoint[1] && tri[0] == midpoint[2])
            top = (Point)tri[2];
        else
            return;
        var seg = GetSegment(top, m);
        if (seg is null) return;
        TriangleMedian median = new(seg, tri);
        median.AddReason();
        median.AddCondition(centroid, midpoint);
        updater.Add(median);
        PointOnSeg pos = new(g, top, m);
        pos.AddReason();
        pos.AddCondition(centroid, midpoint);
        updater.Add(pos);
    }

    [Alias("三角形内心的性质")]
    public void RuleTriH008IncenterProperties(TriangleIncenter incenter)
    {
        var tri = (Triangle)incenter[1];
        var aoS = GetLine(incenter[0], tri[0]);
        var boS = GetLine(incenter[0], tri[1]);
        var coS = GetLine(incenter[0], tri[2]);

        var abcA = GetAngle(tri[0], tri[1], tri[2]);
        var bcaA = GetAngle(tri[1], tri[2], tri[0]);
        var cabA = GetAngle(tri[2], tri[0], tri[1]);

        if (aoS is not null && cabA is not null)
        {
            var pred = new AngularBisectorLine(cabA, aoS);
            pred.AddReason();
            pred.AddCondition(incenter);
            updater.Add(pred);
        }
        if (boS is not null && abcA is not null)
        {
            var pred = new AngularBisectorLine(abcA, boS);
            pred.AddReason();
            pred.AddCondition(incenter);
            updater.Add(pred);
        }
        if (coS is not null && bcaA is not null)
        {
            var pred = new AngularBisectorLine(bcaA, coS);
            pred.AddReason();
            pred.AddCondition(incenter);
            updater.Add(pred);
        }
    }

    [Alias("三角形垂心在高线上")]
    public void RuleTriH010OrthocenterOnAltitude(TriangleOrthocenter ort, LinePerpendicular perpendicular)
    {
        var tri = (Triangle)ort[1];
        var o = (Point)ort[0];
        for (int i = 0; i < 3; i++)
        {
            var top = (Point)tri[i];
            var side = GetLine(tri[(i + 1) % 3], tri[(i + 2) % 3]);
            if (side is null) continue;
            Line alt = null;
            if (perpendicular.Line1 == side && perpendicular.Line2.Contains(top))
                alt = perpendicular.Line2;
            else if (perpendicular.Line2 == side && perpendicular.Line1.Contains(top))
                alt = perpendicular.Line1;
            if (alt is null || alt == side) continue;
            AddColineConclusion(ort, perpendicular, o, alt);
        }
    }

    [Alias("三角形外心在垂直平分线上")]
    public void RuleTriH011CircumcenterOnPerpendicularBisector(TriangleCircumcenter circumcenter, VerticalBisectorLine verticalBisector)
    {
        var tri = (Triangle)circumcenter[1];
        var o = (Point)circumcenter[0];
        var s1 = GetSegment(tri[0], tri[1]);
        var s2 = GetSegment(tri[1], tri[2]);
        var s3 = GetSegment(tri[2], tri[0]);
        var isSide = s1 is not null && verticalBisector.Seg == s1
                  || s2 is not null && verticalBisector.Seg == s2
                  || s3 is not null && verticalBisector.Seg == s3;
        if (!isSide) return;
        AddColineConclusion(circumcenter, verticalBisector, o, verticalBisector.Bisector);
    }

    [Alias("三角形内心在角平分线上")]
    public void RuleTriH012IncenterOnAngleBisector(TriangleIncenter incenter, AngularBisectorLine bisector)
    {
        var tri = (Triangle)incenter[1];
        var i = (Point)incenter[0];
        Angle abc = GetAngle(tri[0], tri[1], tri[2]);
        Angle bca = GetAngle(tri[1], tri[2], tri[0]);
        Angle cab = GetAngle(tri[2], tri[0], tri[1]);
        List<Angle> s = [abc, bca, cab];
        if (!s.Contains(bisector.Angle)) return;
        AddColineConclusion(incenter, bisector, i, bisector.Bisector);
    }

    private void AddColineConclusion(Predicate condition1, Predicate condition2, Point c, Line line)
    {
        var pts = line.Properties.OfType<Point>().Where(p => p != c).ToList();
        for (int i = 0; i < pts.Count; i++)
        {
            for (int j = i + 1; j < pts.Count; j++)
            {
                var pred = CreateColinePredicate(c, pts[i], pts[j]);
                if (pred is null) continue;
                pred.AddReason();
                pred.AddCondition(condition1, condition2);
                updater.Add(pred);
            }
        }
    }

    private static PointOnSeg CreateColinePredicate(Point a, Point b, Point c)
    {
        if (IsBetween(a, b, c)) return new PointOnSeg(a, b, c);
        if (IsBetween(b, a, c)) return new PointOnSeg(b, a, c);
        if (IsBetween(c, a, b)) return new PointOnSeg(c, a, b);
        return null;
    }

    private static bool IsBetween(Point mid, Point p1, Point p2)
    {
        const double Epsilon = 1e-9;
        return (mid.X - p1.X) * (p2.X - mid.X) >= -Epsilon && (mid.Y - p1.Y) * (p2.Y - mid.Y) >= -Epsilon;
    }
}
