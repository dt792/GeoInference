
/// <summary>
/// </summary>
public class KnowledgeCheckException : Exception
{
    public KnowledgeCheckException(string message) : base(message) { }
}

/// <summary>
/// </summary>
public class KnowledgeCheckPlugIn : ISingletonInit, IInferencePlugIn
{
    [DI] KnowledgeBaseUpdater Updater { get; set; }
    [DI] KnowledgeBase KB { get; set; }

    public void Init()
    {
        Updater.PredicateAdded += Check;
    }

    private void Check(Predicate pred)
    {
        switch (pred)
        {
            case PointOnSeg pos:
                CheckPointOnSeg(pos);
                break;
            case LineParallel lp:
                CheckLineParallel(lp);
                break;
                break;
        }
    }

    /// <summary>
    /// </summary>
    private void CheckPointOnSeg(PointOnSeg pos)
    {
        if (pos.Point.HashCode == pos.EndPoint1.HashCode || pos.Point.HashCode == pos.EndPoint2.HashCode)
            throw new KnowledgeCheckException($"非法知识：PointOnSeg 中在线段上的点等于端点：{pos}");
        if (pos.EndPoint1.HashCode == pos.EndPoint2.HashCode)
            throw new KnowledgeCheckException($"非法知识：PointOnSeg 中线段的两个端点相同：{pos}");
    }

    /// <summary>
    /// </summary>
    private void CheckLineParallel(LineParallel lp)
    {
        var commonPoint = FirstCommonPoint(lp.Line1, lp.Line2);
        if (commonPoint is not null)
            throw new KnowledgeCheckException($"矛盾知识：平行直线 {lp.Line1} 与 {lp.Line2} 存在公共点 {commonPoint}，平行线不能有交点：{lp}");

        if (KB.Categories.TryGetValue(typeof(LineIntersection), out var intersections))
        {
            foreach (LineIntersection li in intersections.Where(p => p.IsAvailable))
            {
                if (lp.Properties[0].HashCode == li.Properties[1].HashCode
                    && lp.Properties[1].HashCode == li.Properties[2].HashCode)
                    throw new KnowledgeCheckException($"矛盾知识:{lp} 与已有交点知识 {li} 冲突,平行线不能有交点");
            }
        }
    }

    /// <summary>
    /// </summary>
    private static Point FirstCommonPoint(Line a, Line b)
    {
        var bCodes = new HashSet<ulong>(b.Properties.Select(p => p.HashCode));
        return (Point)a.Properties.FirstOrDefault(p => bCodes.Contains(p.HashCode));
    }
}
