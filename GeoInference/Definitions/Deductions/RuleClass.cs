
using System.Numerics;

public enum RuleType
{
    Undefined,

    Internal,
    BaseFigureSearching,
    AutoGeneration,

    Tradition,
    Analytic,
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RuleTypeAttribute : Attribute
{
    public RuleTypeAttribute(RuleType ruleType)
    {
        RuleType = ruleType;
    }
    public RuleType RuleType { get; init; }
}

public class RuleClass
{
    [DI] protected KnowledgeBaseUpdater updater;
    [DI] protected KnowledgeBase KnowledgeBase { get; set; }

    #region GetFigure
    public virtual Point GetPoint(string name)
    {
        return (Point)KnowledgeBase.Categories[typeof(Point)].First(p => ((Point)p).Name == name);
    }
    public virtual Segment GetSegment(Predicate p1, Predicate p2)
    {
        Segment segment = new Segment((Point)p1, (Point)p2);
        if (KnowledgeBase.Predicates.TryGetValue(segment.HashCode, out var existing))
            return (Segment)existing;
        return null;
    }
    public virtual Arc GetArc(Predicate p1, Predicate p2, Predicate p3)
    {
        var lines = KnowledgeBase.Categories[typeof(Arc)].Where(l => l.IsAvailable);
        foreach (Arc pred in lines)
        {
            if (pred[0] == p1 && pred[1] == p2 && pred[2] == p3 || pred[0] == p1 && pred[1] == p3 && pred[2] == p2)
                return pred;
        }
        return null;
    }

    public virtual Angle GetAngle(Predicate p1, Predicate v, Predicate p2)
    {
        foreach (Angle angle in KnowledgeBase.Categories[typeof(Angle)])
        {
            if (!angle.IsAvailable || angle.Vertex != v)
                continue;
            if ((angle.Edge1.Contains(p1) && angle.Edge2.Contains(p2) || angle.Edge2.Contains(p1) && angle.Edge1.Contains(p2)))
                return angle;
        }
        return null;
    }
    public virtual Sector GetSector(Predicate p1, Predicate p2, Predicate p3)
    {
        var lines = KnowledgeBase.Categories[typeof(Sector)].Where(l => l.IsAvailable);
        foreach (Sector pred in lines)
        {
            if (pred[0] == p1 && pred[1] == p2 && pred[2] == p3)
                return pred;
        }
        return null;
    }
    public virtual Line GetLine(Predicate p1, Predicate p2)
    {
        if (p1 == p2) return null;
        var lines = KnowledgeBase.Categories[typeof(Line)].Where(l => l.IsAvailable);
        foreach (var pred in lines)
        {
            if (pred.Properties.Contains(p1) && pred.Properties.Contains(p2))
                return (Line)pred;
        }
        return null;
    }
    public virtual Circle GetCircle(Predicate c, Predicate e)
    {
        foreach (Circle pred in KnowledgeBase.Categories[typeof(Circle)])
        {
            if (!pred.IsAvailable || pred.Center != c)
                continue;
            if (pred.Properties.Contains(e))
                return pred;
        }
        return null;
    }
    public virtual Triangle GetTriangle(Point p1, Point p2, Point p3)
    {
        if (p1 == p2 || p2 == p3 || p1 == p3)
            return null;
        foreach (var pred in KnowledgeBase.Categories[typeof(Triangle)].Where(a => a.IsAvailable))
        {
            if (pred.Properties.Contains(p1) && pred.Properties.Contains(p2) && pred.Properties.Contains(p3))
                return (Triangle)pred;
        }
        return null;
    }
    public virtual Quadrilateral GetQuadrilateral(Point p1, Point p2, Point p3, Point p4)
    {
        if (p1 == p2 || p1 == p3 || p1 == p4 || p2 == p3 || p2 == p4 || p3 == p4)
            return null;
        foreach (var pred in KnowledgeBase.Categories[typeof(Quadrilateral)].Where(a => a.IsAvailable))
        {
            if (pred.Properties.Contains(p1) && pred.Properties.Contains(p2) && pred.Properties.Contains(p3) && pred.Properties.Contains(p4))
                return (Quadrilateral)pred;
        }
        return null;
    }
    #endregion
    #region Judge
    public virtual bool HasSegment(Predicate p1, Predicate p2)
    {
        Segment segment = new Segment((Point)p1, (Point)p2);
        return KnowledgeBase.Predicates.ContainsKey(segment.HashCode);
    }
    public virtual bool HasColine(params IEnumerable<Predicate> points)
    {
        var pts = points as ICollection<Predicate> ?? points.ToList();
        foreach (var line in KnowledgeBase.Categories[typeof(Line)])
        {
            HashSet<Predicate> lineProps = new(line.Properties);
            if (pts.All(lineProps.Contains))
                return true;
        }
        return false;
    }
    #endregion

    public virtual GeoQuantity GetPointToLineDistance(Point p, Line line)
    {
        var q = new GeoQuantity([p, line], GeoQuantity.Distance);
        updater.TryAddGeoQuantity(q);
        return KnowledgeBase.Quantities[q.ToString()] as GeoQuantity;
    }
    public virtual GeoQuantity GetLineToLineDistance(Line p, Line plane)
    {
        var q = new GeoQuantity([p, plane], GeoQuantity.Distance);
        updater.TryAddGeoQuantity(q);
        return KnowledgeBase.Quantities[q.ToString()] as GeoQuantity;
    }

    #region Tools
    public List<Point> GetPoints(params Predicate[] pms)
    {
        List<Point> points = new List<Point>();
        foreach (var pm in pms)
        {
            foreach (var item in pm.Properties)
            {
                if (item is Point p)
                {
                    if (points.IndexOf(p) == -1)
                        points.Add(p);
                }
                else
                {
                    foreach (var pitem in item.Properties)
                    {
                        if (pitem is Point pi)
                        {
                            if (points.IndexOf(pi) == -1)
                                points.Add(pi);
                        }
                    }
                }
            }
        }
        return points;
    }
    public virtual Predicate GetJudgeIpsilateral(Point point1, Point point2, Point point3, Point point4)
    {
        var a = new PointsOnLineSameSide(point1, point2, point3, point4);
        if (KnowledgeBase.Predicates.TryGetValue(a.HashCode, out var existingA))
            return existingA;
        var b = new PointsOnLineDifferentSide(point1, point2, point3, point4);
        if (KnowledgeBase.Predicates.TryGetValue(b.HashCode, out var existingB))
            return existingB;
        var dd = _GetJudgeIpsilateral(point1, point2, point3, point4);
        if (dd is not null)
        {
            updater.Add(dd);
            return dd;
        }

        return null;
    }
    public virtual Predicate GetPointWithin(Point point1, Point point2, Point point3)
    {
        var a = new PointOnSeg(point1, point2, point3);
        if (KnowledgeBase.Predicates.TryGetValue(a.HashCode, out var existing))
            return existing;
        var pred = _GetPointWithin(point1, point2, point3);
        if (pred is not null)
        {
            updater.Add(pred);
        }
        return pred;
    }
    internal LineIntersection GetLineIntersection(Line? line1, Line? line2)
    {
        var lines = KnowledgeBase.Categories[typeof(LineIntersection)].Where(l => l.IsAvailable);
        foreach (var pred in lines)
        {
            if (pred.Properties.Contains(line1) && pred.Properties.Contains(line2))
                return (LineIntersection)pred;
        }
        return null;
    }
    public Predicate _GetPointWithin(Point point1, Point point2, Point point3)
    {
        Predicate pred = new PointOnSeg(point1, point2, point3);
        if (point1.X == point2.X || point1.X == point3.X)
        {
            if (point2.Y > point1.Y && point1.Y > point3.Y)
            {
                return pred;
            }
            else if (point3.Y > point1.Y && point1.Y > point2.Y)
            {
                return pred;
            }
        }
        else
        {
            if (point2.X > point1.X && point1.X > point3.X)
            {
                return pred;
            }
            else if (point3.X > point1.X && point1.X > point2.X)
            {
                return pred;
            }
        }
        return null;
    }
    public Predicate _GetJudgeIpsilateral(Point point1, Point point2, Point point3, Point point4)
    {
        var (X1, Y1) = (point1.X, point1.Y);
        var (X2, Y2) = (point2.X, point2.Y);
        var (X3, Y3) = (point3.X, point3.Y);
        var (X4, Y4) = (point4.X, point4.Y);
        if (X1 - X2 == 0)
        {
            if (X3 > X1 && X4 > X1 || X3 < X1 && X4 < X1)
            {
                Predicate pred = new PointsOnLineSameSide(point1, point2, point3, point4);
                return pred;
            }
            else if (X3 < X1 && X4 > X1 || X3 > X1 && X4 < X1)
            {
                Predicate pred = new PointsOnLineDifferentSide(point1, point2, point3, point4);
                return pred;
            }
            else
            {
                return null;
            }
        }
        else if (Y1 - Y2 == 0)
        {
            if (Y3 > Y1 && Y4 > Y1 || Y3 < Y1 && Y4 < Y1)
            {
                Predicate pred = new PointsOnLineSameSide(point1, point2, point3, point4);
                return pred;
            }
            else if (Y3 < Y1 && Y4 > Y1 || Y3 > Y1 && Y4 < Y1)
            {
                Predicate pred = new PointsOnLineDifferentSide(point1, point2, point3, point4);
                return pred;
            }
            else
            {
                return null;
            }

        }
        else
        {
            
            double F_X3 = Fx(X1, X2, Y1, Y2, X3);
            double F_X4 = Fx(X1, X2, Y1, Y2, X4);
            if (F_X3 > Y3 && F_X4 > Y4 || F_X3 < Y3 && F_X4 < Y4)
            {
                Predicate pred = new PointsOnLineSameSide(point1, point2, point3, point4);
                return pred;
            }
            else
            {
                double F_2 = Fx(X1, X2, Y1, Y2, X3);
                double F_3 = Fx(X1, X2, Y1, Y2, X4);
                Predicate pred = new PointsOnLineDifferentSide(point1, point2, point3, point4);
                return pred;
            }
        }
    }
    public double Fx(double x1, double x2, double y1, double y2, double x)
    {
        return (y2 - y1) / (x2 - x1) * (x - x1) + y1;
    }
    public bool IsBetweenAngle(Point A, Point B, Point C, Point P)
    {
        return IsVectorBetweenCross(new Vector2((float)(B.X - A.X), (float)(B.Y - A.Y)), new Vector2((float)(C.X - A.X), (float)(C.Y - A.Y)), new Vector2((float)(P.X - A.X), (float)(P.Y - A.Y)));
    }
    public static bool IsVectorBetweenCross(Vector2 A, Vector2 B, Vector2 P)
    {
        
        float crossAB = A.X * B.Y - A.Y * B.X;
        float crossAP = A.X * P.Y - A.Y * P.X;
        float crossPB = P.X * B.Y - P.Y * B.X; 

        const float epsilon = 1e-6f;

        return (crossAP * crossAB >= -epsilon) && (crossPB * crossAB >= -epsilon);
    }
    public bool IsInQuadrilateral(Predicate quadrilateral, Point p1, Point p2, Point p3)
    {
        if (quadrilateral.Properties.Count != 4)
            throw new Exception("错误使用IsInQuadrilateral函数");
        if (quadrilateral[1] == p1 && quadrilateral[0] == p2 && quadrilateral[3] == p3 ||
            quadrilateral[3] == p1 && quadrilateral[0] == p2 && quadrilateral[1] == p3)
        {
            return true;
        }
        else if (quadrilateral[0] == p1 && quadrilateral[1] == p2 && quadrilateral[2] == p3 ||
            quadrilateral[2] == p1 && quadrilateral[1] == p2 && quadrilateral[0] == p3)
        {
            return true;
        }
        else if (quadrilateral[1] == p1 && quadrilateral[2] == p2 && quadrilateral[3] == p3 ||
            quadrilateral[3] == p1 && quadrilateral[2] == p2 && quadrilateral[1] == p3)
        {
            return true;
        }
        else if (quadrilateral[2] == p1 && quadrilateral[3] == p2 && quadrilateral[0] == p3 ||
            quadrilateral[0] == p1 && quadrilateral[3] == p2 && quadrilateral[2] == p3)
        {
            return true;
        }
        else return false;

    }
    #endregion

    protected static bool IsOnMinorArc(
      Point center,
      Point pointA,
     Point pointB,
     Point pointP)
    {
        return IsOnMinorArc((center.X, center.Y), (pointA.X, pointA.Y), (pointB.X, pointB.Y), (pointP.X, pointP.Y)) == true;
    }

    /// <summary>
    /// Determines whether point P on the circle lies on the minor arc or major arc relative to points A and B.
    /// </summary>
    /// <param name="center">Center of the circle</param>
    /// <param name="pointA">Arc endpoint A</param>
    /// <param name="pointB">Arc endpoint B</param>
    /// <param name="pointP">Point P to be evaluated (assumed to be on the circle)</param>
    /// <returns>true: on the minor arc; false: on the major arc; null: coincides with A/B, A and B coincide, or A and B are diametrically opposite points</returns>
    protected static bool? IsOnMinorArc(
        (double x, double y) center,
        (double x, double y) pointA,
        (double x, double y) pointB,
        (double x, double y) pointP)
    {
        const double Epsilon = 1e-10;

        double angleA = NormalizeAngle(Math.Atan2(pointA.y - center.y, pointA.x - center.x));
        double angleB = NormalizeAngle(Math.Atan2(pointB.y - center.y, pointB.x - center.x));
        double angleP = NormalizeAngle(Math.Atan2(pointP.y - center.y, pointP.x - center.x));

        double theta = NormalizeAngle(angleB - angleA);
        double phi = NormalizeAngle(angleP - angleA);

        if (Math.Abs(theta) < Epsilon || Math.Abs(theta - 2 * Math.PI) < Epsilon)
            return null;

        if (Math.Abs(phi) < Epsilon || Math.Abs(phi - theta) < Epsilon)
            return null;

        if (Math.Abs(theta - Math.PI) < Epsilon)
            return false;

        bool onArcAB = (phi < theta);

        if (theta < Math.PI)
        {
            return onArcAB;
        }
        else
        {
            return !onArcAB;
        }
    }

    protected static double NormalizeAngle(double angle)
    {
        double normalized = angle % (2 * Math.PI);
        if (normalized < 0)
        {
            normalized += 2 * Math.PI;
        }
        return normalized;
    }

    /// <summary>
    /// Sorts points by their distance from the vertex, from near to far.
    /// </summary>
    protected static List<Point> SortByDistanceFrom(Point vertex, IEnumerable<Point> points)
    {
        return points.OrderBy(p =>
            (p.X - vertex.X) * (p.X - vertex.X) + (p.Y - vertex.Y) * (p.Y - vertex.Y)).ToList();
    }

    /// <summary>
    /// Determines whether point P lies outside the circle.
    /// </summary>
    protected static bool IsOutsideCircle(Circle circle, Point p)
    {
        if (circle.Properties.Skip(1).OfType<Point>().FirstOrDefault() is not Point rp)
            return false;
        double r2 = (rp.X - circle.Center.X) * (rp.X - circle.Center.X) + (rp.Y - circle.Center.Y) * (rp.Y - circle.Center.Y);
        double d2 = (p.X - circle.Center.X) * (p.X - circle.Center.X) + (p.Y - circle.Center.Y) * (p.Y - circle.Center.Y);
        return d2 > r2;
    }

    /// <summary>
    /// Determines whether the arc AB intercepted by an external secant/tangent angle is the minor arc.
    /// The intercepted arc is the one NOT containing the other intersection points of the angle's sides
    /// with the circle; if any of them lies on the minor arc AB, the intercepted arc is the major arc
    /// (whose measure is 360 - Size).
    /// </summary>
    protected static bool IsInterceptedArcMinor(Point center, Point a, Point b, params Point[] others)
    {
        return !others.Any(o => IsOnMinorArc(center, a, b, o));
    }

    /// <summary>
    /// Builds the exterior-angle equation 2·angle = farArc − nearArc.
    /// An intercepted major arc is expressed as 360 - Size.
    /// </summary>
    protected static LinearEquation ExteriorAngleArcEquation(GeoQuantity angleSize, Arc nearArc, bool nearIsMinor, Arc farArc, bool farIsMinor)
    {
        Dictionary<Quantity, Expr> coff = new()
        {
            { angleSize, 2 },
            { nearArc.Size, nearIsMinor ? 1 : -1 },
            { farArc.Size, farIsMinor ? -1 : 1 }
        };
        Expr constant = (farIsMinor ? 0 : 360) - (nearIsMinor ? 0 : 360);
        return new LinearEquation(coff, constant);
    }
    public static bool IsTrapezoid(params Point[] points) => TrapezoidHelper.IsTrapezoid(points);
    public static class TrapezoidHelper
    {
        private const double Epsilon = 1e-4;
        private static double Hypot(double x, double y) => Math.Sqrt(x * x + y * y);
        public static bool IsTrapezoid(params Point[] points)
        {
            if (points.Length != 4)
                throw new ArgumentException("必须提供四个点");

            var hull = GetConvexHull(points);
            if (hull.Count != 4)
                return false;

            var edges = new Vector[4];
            for (int i = 0; i < 4; i++)
            {
                Point p1 = hull[i];
                Point p2 = hull[(i + 1) % 4];
                edges[i] = new Vector(p2.X - p1.X, p2.Y - p1.Y);
            }

            bool parallel0 = AreParallel(edges[0], edges[2]); 
            bool parallel1 = AreParallel(edges[1], edges[3]); 

            return parallel0 ^ parallel1; 
        }
        private static List<Point> GetConvexHull(Point[] points)
        {
            var list = new List<Point>(points);
            if (list.Count < 3) return list;

            Point lowest = list[0];
            foreach (var p in list)
            {
                if (p.Y < lowest.Y || (Math.Abs(p.Y - lowest.Y) < Epsilon && p.X < lowest.X))
                    lowest = p;
            }

            list.Sort((a, b) =>
            {
                if (a.Equals(lowest)) return -1;
                if (b.Equals(lowest)) return 1;

                double angleA = Math.Atan2(a.Y - lowest.Y, a.X - lowest.X);
                double angleB = Math.Atan2(b.Y - lowest.Y, b.X - lowest.X);
                if (Math.Abs(angleA - angleB) < Epsilon)
                {
                    
                    double distA2 = (a.X - lowest.X) * (a.X - lowest.X) + (a.Y - lowest.Y) * (a.Y - lowest.Y);
                    double distB2 = (b.X - lowest.X) * (b.X - lowest.X) + (b.Y - lowest.Y) * (b.Y - lowest.Y);
                    return distA2.CompareTo(distB2);
                }
                return angleA.CompareTo(angleB);
            });

            var hull = new List<Point>();
            foreach (var p in list)
            {
                while (hull.Count >= 2 && Cross(hull[^2], hull[^1], p) <= 0)
                    hull.RemoveAt(hull.Count - 1);
                hull.Add(p);
            }
            return hull;
        }

        private static double Cross(Point o, Point a, Point b)
        {
            return (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);
        }

        private struct Vector
        {
            public double X, Y;
            public Vector(double x, double y) { X = x; Y = y; }
        }

        private static bool AreParallel(Vector v1, Vector v2)
        {
            return Math.Abs(v1.X * v2.Y - v1.Y * v2.X) < Epsilon;
        }
    }
    public (bool isMatch, Predicate other1, Predicate other2) FindOther
        (Predicate pm11, Predicate pm12, Predicate pm21, Predicate pm22, Predicate pm31, Predicate pm32)
    {
        if (pm11 == pm21 && pm12 == pm31)
        {
            return (true, pm22, pm32);
        }
        else if (pm11 == pm22 && pm12 == pm31)
        {
            return (true, pm21, pm32);
        }
        else if (pm11 == pm21 && pm12 == pm32)
        {
            return (true, pm22, pm31);
        }
        else if (pm11 == pm22 && pm12 == pm32)
        {
            return (true, pm21, pm31);
        }
        else return (false, null, null);
    }

    public (Predicate common, Predicate notcommon1, Predicate notcommon2) FindCommon(Predicate pm1, Predicate pm2, Predicate pm3, Predicate pm4)
    {
        if (pm1 == pm3)
        {
            return (pm1, pm2, pm4);
        }
        else if (pm1 == pm4)
        {
            return (pm1, pm2, pm3);
        }
        else if (pm2 == pm3)
        {
            return (pm2, pm1, pm4);
        }
        else if (pm2 == pm4)
        {
            return (pm2, pm1, pm3);
        }
        else return (null, null, null);
    }
    public (Predicate common, Predicate notcommon1, Predicate notcommon2) FindCommon(Predicate pm1, Predicate pm2)
    {
        if (pm1.Properties.Count == 2 && pm2.Properties.Count == 2)
            return FindCommon(pm1[0], pm1[1], pm2[0], pm2[1]);
        throw new Exception("错误使用FindCommon函数");
    }
    public (T common, T notcommon1, T notcommon2) FindCommon<T>(Predicate pm1, Predicate pm2) where T : Predicate
    {
        if (pm1.Properties.Count == 2 && pm2.Properties.Count == 2)
        {
            var (c, nc1, nc2) = FindCommon(pm1[0], pm1[1], pm2[0], pm2[1]);
            return ((T)c, (T)nc1, (T)nc2);
        }

        throw new Exception("错误使用FindCommon函数");
    }
    public Predicate FindCIntersection(IEnumerable<Predicate> pms1, IEnumerable<Predicate> pms2)
    {
        HashSet<Predicate> set2 = new(pms2);
        foreach (var item in pms1)
        {
            if (set2.Contains(item))
                return item;
        }
        return null;
    }
    public Predicate FindCIntersection(Predicate pms1, Predicate pms2)
    {
        return FindCIntersection(pms1.Properties, pms2.Properties);
    }
    public (List<Predicate> intersection, List<Predicate> left1, List<Predicate> left2) FindIntersection(Predicate pms1, Predicate pms2)
    {
        return FindIntersection(pms1.Properties, pms2.Properties);
    }
    public (List<T> intersection, List<T> left1, List<T> left2) FindIntersection<T>(List<T> pms1, List<T> pms2) where T : Predicate
    {
        HashSet<T> set1 = new(pms1);
        HashSet<T> set2 = new(pms2);
        List<T> intersection = pms1.Where(set2.Contains).ToList();
        List<T> left1 = pms1.Where(p => !set2.Contains(p)).ToList();
        List<T> left2 = pms2.Where(p => !set1.Contains(p)).ToList();
        return (intersection, left1, left2);
    }
    public (List<Predicate> intersection, List<Predicate> left1, List<Predicate> left2) FindIntersection(List<Predicate> pms1, List<Predicate> pms2)
    {
        return FindIntersection<Predicate>(pms1, pms2);
    }

}
