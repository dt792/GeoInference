
[RuleType(RuleType.Tradition)]
internal class QuadrilateralRules : RuleClass
{
    [Alias("正方形的性质")]
    public void RuleQuadAttri01SquareProperties(Square quad)
    {
        var abL = GetLine(quad[0], quad[1]);
        var bcL = GetLine(quad[1], quad[2]);
        var cdL = GetLine(quad[2], quad[3]);
        var daL = GetLine(quad[3], quad[0]);

        var acL = GetLine(quad[0], quad[2]);
        var bdL = GetLine(quad[1], quad[3]);

        var abS = GetSegment(quad[0], quad[1]);
        var bcS = GetSegment(quad[1], quad[2]);
        var cdS = GetSegment(quad[2], quad[3]);
        var daS = GetSegment(quad[3], quad[0]);

        var acS = GetSegment(quad[0], quad[2]);
        var bdS = GetSegment(quad[1], quad[3]);

        LineParallel lp;
        LinePerpendicular lperp;
        QuantityRatio qr;

        lp = new LineParallel(abL, cdL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);
        lp = new LineParallel(bcL, daL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);
        lperp = new LinePerpendicular(abL, bcL);
        lperp.AddReason();
        lperp.AddCondition(quad);
        updater.Add(lperp);
        lperp = new LinePerpendicular(bcL, cdL);
        lperp.AddReason();
        lperp.AddCondition(quad);
        updater.Add(lperp);
        lperp = new LinePerpendicular(cdL, daL);
        lperp.AddReason();
        lperp.AddCondition(quad);
        updater.Add(lperp);
        lperp = new LinePerpendicular(daL, abL);
        lperp.AddReason();
        lperp.AddCondition(quad);
        updater.Add(lperp);
        qr = new QuantityRatio(abS.Length, bcS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(bcS.Length, cdS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(cdS.Length, daS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(daS.Length, abS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        if (acS is not null && bdS is not null)
        {
            qr = new QuantityRatio(acS.Length, bdS.Length);
            qr.AddReason();
            qr.AddCondition(quad);
            updater.Add(qr);
        }
        if (acL is not null && bdL is not null)
        {
            lperp = new LinePerpendicular(acL, bdL);
            lperp.AddReason();
            lperp.AddCondition(quad);
            updater.Add(lperp);
        }

        var abcA = GetAngle(quad[0], quad[1], quad[2]);
        var bcaA = GetAngle(quad[1], quad[2], quad[3]);
        var cdaA = GetAngle(quad[2], quad[3], quad[0]);
        var dabA = GetAngle(quad[3], quad[0], quad[1]);

        QuantityValue qv;
        qv = new QuantityValue(abcA.Size, 90);
        qv.AddReason();
        qv.AddCondition(quad);
        updater.Add(qv);
        qv = new QuantityValue(bcaA.Size, 90);
        qv.AddReason();
        qv.AddCondition(quad);
        updater.Add(qv);
        qv = new QuantityValue(cdaA.Size, 90);
        qv.AddReason();
        qv.AddCondition(quad);
        updater.Add(qv);
        qv = new QuantityValue(dabA.Size, 90);
        qv.AddReason();
        qv.AddCondition(quad);
        updater.Add(qv);

        qr = new QuantityRatio(abcA.Size, cdaA.Size);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(bcaA.Size, dabA.Size);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
    }

    [Alias("矩形的性质")]
    public void RuleQuadAttri02RectangleProperties(Rectangle quad)
    {
        var abL = GetLine(quad[0], quad[1]);
        var bcL = GetLine(quad[1], quad[2]);
        var cdL = GetLine(quad[2], quad[3]);
        var daL = GetLine(quad[3], quad[0]);

        var abS = GetSegment(quad[0], quad[1]);
        var bcS = GetSegment(quad[1], quad[2]);
        var cdS = GetSegment(quad[2], quad[3]);
        var daS = GetSegment(quad[3], quad[0]);

        var acS = GetSegment(quad[0], quad[2]);
        var bdS = GetSegment(quad[1], quad[3]);

        LineParallel lp;
        LinePerpendicular lperp;
        QuantityRatio qr;

        lp = new LineParallel(abL, cdL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);
        lp = new LineParallel(bcL, daL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);
        lperp = new LinePerpendicular(abL, bcL);
        lperp.AddReason();
        lperp.AddCondition(quad);
        updater.Add(lperp);
        lperp = new LinePerpendicular(bcL, cdL);
        lperp.AddReason();
        lperp.AddCondition(quad);
        updater.Add(lperp);
        lperp = new LinePerpendicular(cdL, daL);
        lperp.AddReason();
        lperp.AddCondition(quad);
        updater.Add(lperp);
        lperp = new LinePerpendicular(daL, abL);
        lperp.AddReason();
        lperp.AddCondition(quad);
        updater.Add(lperp);
        qr = new QuantityRatio(abS.Length, cdS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(bcS.Length, daS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        if (acS is not null && bdS is not null)
        {
            qr = new QuantityRatio(acS.Length, bdS.Length);
            qr.AddReason();
            qr.AddCondition(quad);
            updater.Add(qr);
        }

        var abcA = GetAngle(quad[0], quad[1], quad[2]);
        var bcaA = GetAngle(quad[1], quad[2], quad[3]);
        var cdaA = GetAngle(quad[2], quad[3], quad[0]);
        var dabA = GetAngle(quad[3], quad[0], quad[1]);

        QuantityValue qv;
        qv = new QuantityValue(abcA.Size, 90);
        qv.AddReason();
        qv.AddCondition(quad);
        updater.Add(qv);
        qv = new QuantityValue(bcaA.Size, 90);
        qv.AddReason();
        qv.AddCondition(quad);
        updater.Add(qv);
        qv = new QuantityValue(cdaA.Size, 90);
        qv.AddReason();
        qv.AddCondition(quad);
        updater.Add(qv);
        qv = new QuantityValue(dabA.Size, 90);
        qv.AddReason();
        qv.AddCondition(quad);
        updater.Add(qv);

        qr = new QuantityRatio(abcA.Size, cdaA.Size);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(bcaA.Size, dabA.Size);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
    }

    [Alias("菱形的性质")]
    public void RuleQuadAttri03RhombusProperties(Rhombus quad)
    {
        var abL = GetLine(quad[0], quad[1]);
        var bcL = GetLine(quad[1], quad[2]);
        var cdL = GetLine(quad[2], quad[3]);
        var daL = GetLine(quad[3], quad[0]);

        var acL = GetLine(quad[0], quad[2]);
        var bdL = GetLine(quad[1], quad[3]);

        var abS = GetSegment(quad[0], quad[1]);
        var bcS = GetSegment(quad[1], quad[2]);
        var cdS = GetSegment(quad[2], quad[3]);
        var daS = GetSegment(quad[3], quad[0]);

        var acS = GetSegment(quad[0], quad[2]);
        var bdS = GetSegment(quad[1], quad[3]);

        LineParallel lp;
        QuantityRatio qr;

        lp = new LineParallel(abL, cdL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);
        lp = new LineParallel(bcL, daL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);

        if (acL is not null && bdL is not null)
        {
            VerticalBisectorLine vb = new VerticalBisectorLine(acS, bdL);
            vb.AddReason();
            vb.AddCondition(quad);
            updater.Add(vb);
            vb = new VerticalBisectorLine(bdS, acL);
            vb.AddReason();
            vb.AddCondition(quad);
            updater.Add(vb);
            LinePerpendicular lperp = new LinePerpendicular(acL, bdL);
            lperp.AddReason();
            lperp.AddCondition(quad);
            updater.Add(lperp);
        }

        qr = new QuantityRatio(abS.Length, cdS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(bcS.Length, daS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(abS.Length, bcS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(bcS.Length, cdS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(cdS.Length, daS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(daS.Length, abS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);

        var abcA = GetAngle(quad[0], quad[1], quad[2]);
        var bcaA = GetAngle(quad[1], quad[2], quad[3]);
        var cdaA = GetAngle(quad[2], quad[3], quad[0]);
        var dabA = GetAngle(quad[3], quad[0], quad[1]);

        qr = new QuantityRatio(abcA.Size, cdaA.Size);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(bcaA.Size, dabA.Size);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
    }

    [Alias("平行四边形的性质")]
    public void RuleQuadAttri04ParallelogramProperties(Parallelogram quad)
    {
        var abL = GetLine(quad[0], quad[1]);
        var bcL = GetLine(quad[1], quad[2]);
        var cdL = GetLine(quad[2], quad[3]);
        var daL = GetLine(quad[3], quad[0]);

        var abS = GetSegment(quad[0], quad[1]);
        var bcS = GetSegment(quad[1], quad[2]);
        var cdS = GetSegment(quad[2], quad[3]);
        var daS = GetSegment(quad[3], quad[0]);

        LineParallel lp;
        QuantityRatio qr;

        lp = new LineParallel(abL, cdL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);
        lp = new LineParallel(bcL, daL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);

        qr = new QuantityRatio(abS.Length, cdS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(bcS.Length, daS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);

        var abcA = GetAngle(quad[0], quad[1], quad[2]);
        var bcaA = GetAngle(quad[1], quad[2], quad[3]);
        var cdaA = GetAngle(quad[2], quad[3], quad[0]);
        var dabA = GetAngle(quad[3], quad[0], quad[1]);

        qr = new QuantityRatio(abcA.Size, cdaA.Size);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(bcaA.Size, dabA.Size);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
    }

    [Alias("梯形的性质")]
    public void RuleQuadAttri05TrapezoidProperties(Trapezoid quad)
    {
        var abL = GetLine(quad[0], quad[1]);
        var cdL = GetLine(quad[2], quad[3]);

        LineParallel lp = new LineParallel(abL, cdL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);
    }

    [Alias("等腰梯形的性质")]
    public void RuleQuadAttri06IsoscelesTrapezoidProperties(IsoscelesTrapezoid quad)
    {
        var abL = GetLine(quad[0], quad[1]);
        var cdL = GetLine(quad[2], quad[3]);

        var bcS = GetSegment(quad[1], quad[2]);
        var daS = GetSegment(quad[3], quad[0]);

        LineParallel lp = new LineParallel(abL, cdL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);
        QuantityRatio qr = new QuantityRatio(bcS.Length, daS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        var acS = GetSegment(quad[0], quad[2]);
        var bdS = GetSegment(quad[1], quad[3]);
        if (acS is not null && bdS is not null)
        {
            qr = new QuantityRatio(acS.Length, bdS.Length);
            qr.AddReason();
            qr.AddCondition(quad);
            updater.Add(qr);
        }
    }

    [Alias("直角梯形的性质")]
    public void RuleQuadAttri07RightTrapezoidProperties(RightTrapezoid quad)
    {
        var abL = GetLine(quad[0], quad[1]);
        var bcL = GetLine(quad[1], quad[2]);
        var cdL = GetLine(quad[2], quad[3]);

        var abS = GetSegment(quad[0], quad[1]);
        var bcS = GetSegment(quad[1], quad[2]);
        var cdS = GetSegment(quad[2], quad[3]);
        var daS = GetSegment(quad[3], quad[0]);

        LineParallel lp;
        LinePerpendicular lperp;
        QuantityRatio qr;
        QuantityValue qv;

        lp = new LineParallel(abL, cdL);
        lp.AddReason();
        lp.AddCondition(quad);
        updater.Add(lp);
        lperp = new LinePerpendicular(abL, bcL);
        lperp.AddReason();
        lperp.AddCondition(quad);
        updater.Add(lperp);
        lperp = new LinePerpendicular(bcL, cdL);
        lperp.AddReason();
        lperp.AddCondition(quad);
        updater.Add(lperp);

        var abcA = GetAngle(quad[0], quad[1], quad[2]);
        var bcdA = GetAngle(quad[1], quad[2], quad[3]);

        qr = new QuantityRatio(abcA.Size, bcdA.Size);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(abcA.Size, bcdA.Size);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qv = new QuantityValue(abcA.Size, 90);
        qv.AddReason();
        qv.AddCondition(quad);
        updater.Add(qv);
    }

    [Alias("筝形的性质")]
    public void RuleQuadAttri08KiteProperties(Kite quad)
    {
        var acL = GetLine(quad[0], quad[2]);
        var bdL = GetLine(quad[1], quad[3]);
        var abS = GetSegment(quad[0], quad[1]);
        var bcS = GetSegment(quad[1], quad[2]);
        var cdS = GetSegment(quad[2], quad[3]);
        var daS = GetSegment(quad[3], quad[0]);
        if (acL is not null && bdL is not null)
        {
            LinePerpendicular lperp = new LinePerpendicular(acL, bdL);
            lperp.AddReason();
            lperp.AddCondition(quad);
            updater.Add(lperp);
        }
        QuantityRatio qr = new QuantityRatio(abS.Length, daS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
        qr = new QuantityRatio(bcS.Length, cdS.Length);
        qr.AddReason();
        qr.AddCondition(quad);
        updater.Add(qr);
    }

    [Alias("梯形的判定")]
    public void RuleDQ01TrapezoidDetermination(Quadriliateral quadriliateral, LineParallel parallel)
    {
        Line ab = GetLine((Point)quadriliateral[0], (Point)quadriliateral[1]);
        Line bc = GetLine((Point)quadriliateral[1], (Point)quadriliateral[2]);
        Line cd = GetLine((Point)quadriliateral[2], (Point)quadriliateral[3]);
        Line da = GetLine((Point)quadriliateral[0], (Point)quadriliateral[3]);
        if (!IsTrapezoid((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]))
            return;
        if (ab == parallel.Line1 && cd == parallel.Line2 || ab == parallel.Line2 && cd == parallel.Line1)
        {
            Trapezoid pred = new Trapezoid((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]);
            pred.AddReason();
            pred.AddCondition(quadriliateral, parallel);
            updater.Add(pred);
        }
        else if (bc == parallel.Line1 && da == parallel.Line2 || bc == parallel.Line2 && da == parallel.Line1)
        {
            Trapezoid pred = new Trapezoid((Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3], (Point)quadriliateral[0]);
            pred.AddReason();
            pred.AddCondition(quadriliateral, parallel);
            updater.Add(pred);
        }
    }

    [SemiConditionRule]
    [Alias("平行四边形的判定（一组对边平行且相等）")]
    public void RuleDQ02QuadrilateralWithOnePairOfParallelAndEqualOppositeSidesIsParallelogram(Quadriliateral quadriliateral)
    {
        Line ab = GetLine((Point)quadriliateral[0], (Point)quadriliateral[1]);
        Line bc = GetLine((Point)quadriliateral[1], (Point)quadriliateral[2]);
        Line cd = GetLine((Point)quadriliateral[2], (Point)quadriliateral[3]);
        Line da = GetLine((Point)quadriliateral[0], (Point)quadriliateral[3]);
        Segment Sab = GetSegment((Point)quadriliateral[0], (Point)quadriliateral[1]);
        Segment Sbc = GetSegment((Point)quadriliateral[1], (Point)quadriliateral[2]);
        Segment Scd = GetSegment((Point)quadriliateral[2], (Point)quadriliateral[3]);
        Segment Sda = GetSegment((Point)quadriliateral[0], (Point)quadriliateral[3]);

        CondictionalKnowledge c1 = new() { Knowledge = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]) };
        c1.AddCondiction(new LineParallel(ab, cd), new SegmentLengthEqual(Sab, Scd));
        c1.Knowledge.AddReason();
        c1.Knowledge.AddCondition(quadriliateral);
        updater.AddCondictionalKnowledgePair(c1);

        CondictionalKnowledge c11 = new() { Knowledge = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]) };
        c11.AddCondiction(new LineParallel(bc, da), new SegmentLengthEqual(Sbc, Sda));
        c11.Knowledge.AddReason();
        c11.Knowledge.AddCondition(quadriliateral);
        updater.AddCondictionalKnowledgePair(c11);
    }

    [SemiConditionRule]
    [Alias("平行四边形的判定（两组对边分别平行）")]
    public void RuleDQ03QuadrilateralWithTwoPairsOfParallelOppositeSidesIsParallelogram(Quadriliateral quadriliateral)
    {
        Line ab = GetLine((Point)quadriliateral[0], (Point)quadriliateral[1]);
        Line bc = GetLine((Point)quadriliateral[1], (Point)quadriliateral[2]);
        Line cd = GetLine((Point)quadriliateral[2], (Point)quadriliateral[3]);
        Line da = GetLine((Point)quadriliateral[0], (Point)quadriliateral[3]);
        CondictionalKnowledge c1 = new() { Knowledge = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]) };
        c1.AddCondiction(new LineParallel(ab, cd), new LineParallel(bc, da));
        c1.Knowledge.AddReason();
        c1.Knowledge.AddCondition();
        updater.AddCondictionalKnowledgePair(c1);
    }

    [SemiConditionRule]
    [Alias("平行四边形的判定（两组对边分别相等）")]
    public void RuleDQ04QuadrilateralWithTwoPairsOfEqualOppositeSidesIsParallelogram(Quadriliateral quadriliateral)
    {
        Segment segment1 = GetSegment((Point)quadriliateral[0], (Point)quadriliateral[1]);
        Segment segment2 = GetSegment((Point)quadriliateral[2], (Point)quadriliateral[3]);
        Segment segment3 = GetSegment((Point)quadriliateral[0], (Point)quadriliateral[3]);
        Segment segment4 = GetSegment((Point)quadriliateral[1], (Point)quadriliateral[2]);
        CondictionalKnowledge c1 = new() { Knowledge = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]) };
        c1.AddCondiction(new SegmentLengthEqual(segment1, segment2), new SegmentLengthEqual(segment3, segment4));
        c1.Knowledge.AddReason();
        c1.Knowledge.AddCondition();
        updater.AddCondictionalKnowledgePair(c1);
    }

    [SemiConditionRule]
    [Alias("平行四边形的判定（两组对角分别相等）")]
    public void RuleDQ05QuadrilateralWithEqualOppositeAnglesIsParallelogram(Quadriliateral quadriliateral)
    {
        Angle angle1 = GetAngle((Point)quadriliateral[1], (Point)quadriliateral[0], (Point)quadriliateral[3]);
        Angle angle2 = GetAngle((Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]);
        Angle angle3 = GetAngle((Point)quadriliateral[0], (Point)quadriliateral[3], (Point)quadriliateral[2]);
        Angle angle4 = GetAngle((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2]);
        CondictionalKnowledge c1 = new() { Knowledge = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]) };
        c1.AddCondiction(new AngleSizeEqual(angle1, angle2), new AngleSizeEqual(angle3, angle4));
        c1.Knowledge.AddReason();
        c1.Knowledge.AddCondition();
        updater.AddCondictionalKnowledgePair(c1);
    }

    [Alias("平行四边形的判定（对角线互相平分）")]
    public void RuleDQ06QuadrilateralWithMutuallyBisectingDiagonalsIsParallelogram(Quadriliateral quadriliateral, Midpoint midpoint, Midpoint midpoint1)
    {
        if (midpoint1 == midpoint) return;
        Point point = (Point)midpoint[0];
        Point point1 = (Point)midpoint1[0];
        if (point != point1)
        {
            return;
        }
        Segment segment1 = GetSegment((Point)quadriliateral[0], (Point)quadriliateral[2]);
        Segment segment2 = GetSegment((Point)quadriliateral[1], (Point)quadriliateral[3]);

        Segment segment3 = GetSegment(midpoint[1], midpoint[2]);
        Segment segment4 = GetSegment(midpoint1[1], midpoint1[2]);
        if (segment1 is null || segment2 is null) return;
        if ((segment3 == segment1 && segment4 == segment2) || (segment3 == segment2 && segment4 == segment1))
        {
            Parallelogram pred = new Parallelogram((Point)quadriliateral[0], (Point)quadriliateral[1], (Point)quadriliateral[2], (Point)quadriliateral[3]);
            pred.AddReason();
            pred.AddCondition(quadriliateral, midpoint, midpoint1);
            updater.Add(pred);
        }
    }

    [Alias("矩形的判定（有一个角是直角的平行四边形）")]
    public void RuleDQ07ParallelogramWithOneRightAngleIsRectangle(Parallelogram parallelogram, AngleSize angleSize)
    {
        Expr expr1 = angleSize.Expr;
        Angle angle = (Angle)angleSize[0];
        if (expr1.CompareTo(90) != ExprCompareResult.Equal)
        {
            return;
        }
        var i = parallelogram.Properties.IndexOf(angle.Vertex);
        if (i != -1)
        {
            var p1 = parallelogram.Properties[(i + 1) % 4];
            var p2 = parallelogram.Properties[(i + 3) % 4];
            if (angle.Edge1.Contains(p1) && angle.Edge2.Contains(p2) ||
                angle.Edge1.Contains(p2) && angle.Edge2.Contains(p1))
            {
                Rectangle pred = new Rectangle((Point)parallelogram[0], (Point)parallelogram[1], (Point)parallelogram[2], (Point)parallelogram[3]);
                pred.AddReason();
                pred.AddCondition(parallelogram, angleSize);
                updater.Add(pred);
            }
        }
    }

    [Alias("矩形的判定（对角线相等的平行四边形）")]
    public void RuleDQ08ParallelogramWithEqualDiagonalsIsRectangle(Parallelogram parallelogram, SegmentLengthEqual segmentLengthEqual)
    {
        Segment segment1 = GetSegment((Point)parallelogram[0], (Point)parallelogram[2]);
        Segment segment2 = GetSegment((Point)parallelogram[1], (Point)parallelogram[3]);
        if (segment1 is null || segment2 is null) return;
        Segment segment3 = (Segment)segmentLengthEqual[0];
        Segment segment4 = (Segment)segmentLengthEqual[1];
        if ((segment1 == segment3 && segment2 == segment4) || (segment1 == segment4 && segment2 == segment3))
        {
            Rectangle pred = new Rectangle((Point)parallelogram[0], (Point)parallelogram[1], (Point)parallelogram[2], (Point)parallelogram[3]);
            pred.AddReason();
            pred.AddCondition(parallelogram, segmentLengthEqual);
            updater.Add(pred);
        }
    }

    [Alias("筝形的判定（两组邻边分别相等）")]
    public void RuleDQ09QuadrilateralWithTwoPairsOfEqualAdjacentSidesIsKite(Quadriliateral quad, SegmentLengthEqual equal1, SegmentLengthEqual equal2)
    {
        if (equal1 == equal2) return;
        Segment ab = GetSegment((Point)quad[0], (Point)quad[1]);
        Segment ad = GetSegment((Point)quad[0], (Point)quad[3]);
        Segment bc = GetSegment((Point)quad[1], (Point)quad[2]);
        Segment cd = GetSegment((Point)quad[2], (Point)quad[3]);
        if (ab == equal1.Seg1 && ad == equal1.Seg2 || ab == equal1.Seg2 && ad == equal1.Seg1)
        {
            if (bc == equal2.Seg1 && cd == equal2.Seg2 || bc == equal2.Seg2 && cd == equal2.Seg1)
            {
                Kite pred = new Kite((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);
                pred.AddReason();
                pred.AddCondition(quad, equal1, equal2);
                updater.Add(pred);
            }
        }
    }

    [Alias("筝形的判定（垂直平分线）")]
    public void RuleDQ10PerpendicularBisectorDeterminesKite(VerticalBisectorLine verticalBisectorLine)
    {
        for (int i = 0; i < verticalBisectorLine[1].Properties.Count; i++)
        {
            for (int j = i + 1; j < verticalBisectorLine[1].Properties.Count; j++)
            {
                var quad = GetQuadriliateral((Point)verticalBisectorLine[1][i], (Point)verticalBisectorLine[0][0], (Point)verticalBisectorLine[1][i], (Point)verticalBisectorLine[0][1]);
                if (quad is not null)
                {
                    Kite pred = new Kite((Point)verticalBisectorLine[1][i], (Point)verticalBisectorLine[0][0], (Point)verticalBisectorLine[1][i], (Point)verticalBisectorLine[0][1]);
                    pred.AddReason();
                    pred.AddCondition(quad, verticalBisectorLine);
                    updater.Add(pred);
                }
            }
        }
    }

    [Alias("菱形的判定（邻边相等的平行四边形）")]
    public void RuleDQ11ParallelogramWithEqualAdjacentSidesIsRhombus(Parallelogram quad, SegmentLengthEqual equal)
    {
        var abS = GetSegment(quad[0], quad[1]);
        var bcS = GetSegment(quad[1], quad[2]);
        var cdS = GetSegment(quad[2], quad[3]);
        var daS = GetSegment(quad[3], quad[0]);
        if (equal.Contains(abS) && equal.Contains(bcS))
        {
            Rhombus pred = new Rhombus((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);
            pred.AddReason();
            pred.AddCondition(quad, equal);
            updater.Add(pred);
        }
        else if (equal.Contains(bcS) && equal.Contains(cdS))
        {
            Rhombus pred = new Rhombus((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);
            pred.AddReason();
            pred.AddCondition(quad, equal);
            updater.Add(pred);
        }
        else if (equal.Contains(cdS) && equal.Contains(daS))
        {
            Rhombus pred = new Rhombus((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);
            pred.AddReason();
            pred.AddCondition(quad, equal);
            updater.Add(pred);
        }
        else if (equal.Contains(daS) && equal.Contains(abS))
        {
            Rhombus pred = new Rhombus((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);
            pred.AddReason();
            pred.AddCondition(quad, equal);
            updater.Add(pred);
        }
    }

    [Alias("菱形的判定（既是筝形又是平行四边形）")]
    public void RuleDQ12QuadrilateralThatIsBothKiteAndParallelogramIsRhombus(Kite kite, Parallelogram parallelogram)
    {
        var result = this.FindIntersection(kite.Properties, parallelogram.Properties);
        if (result.intersection.Count == 4)
        {
            Rhombus pred = new Rhombus((Point)parallelogram[0], (Point)parallelogram[1], (Point)parallelogram[2], (Point)parallelogram[3]);
            pred.AddReason();
            pred.AddCondition(kite, parallelogram);
            updater.Add(pred);
        }
    }

    [Alias("正方形的判定（既是菱形又是矩形）")]
    public void RuleDQ13QuadrilateralThatIsBothRhombusAndRectangleIsSquare(Rhombus rhombus, Rectangle rectangle)
    {
        var result = this.FindIntersection(rhombus.Properties, rectangle.Properties);
        if (result.intersection.Count == 4)
        {
            Square pred = new Square((Point)rhombus[0], (Point)rhombus[1], (Point)rhombus[2], (Point)rhombus[3]);
            pred.AddReason();
            pred.AddCondition(rhombus, rectangle);
            updater.Add(pred);
        }
    }

    [Alias("等腰梯形的判定（两腰相等）")]
    public void RuleDQ14TrapezoidWithEqualLegsIsIsoscelesTrapezoid(Trapezoid trapezoid, SegmentLengthEqual segmentLengthEqual)
    {
        Segment segment1 = (Segment)segmentLengthEqual[0];
        Segment segment2 = (Segment)segmentLengthEqual[1];
        Segment segment3 = GetSegment((Point)trapezoid[0], (Point)trapezoid[3]);
        Segment segment4 = GetSegment((Point)trapezoid[1], (Point)trapezoid[2]);
        if ((segment3 == segment1 && segment4 == segment2) || (segment3 == segment2 && segment4 == segment1))
        {
            IsoscelesTrapezoid pred = new IsoscelesTrapezoid((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
            pred.AddReason();
            pred.AddCondition(trapezoid, segmentLengthEqual);
            updater.Add(pred);
        }
    }

    [Alias("等腰梯形的判定（同一底上的两角相等）")]
    public void RuleDQ15TrapezoidWithEqualBaseAnglesIsIsoscelesTrapezoid(Trapezoid trapezoid, AngleSizeEqual angleSizeEqual)
    {
        Angle angle1 = (Angle)angleSizeEqual[0];
        Angle angle2 = (Angle)angleSizeEqual[1];
        Angle angle3 = GetAngle((Point)trapezoid[0], (Point)trapezoid[3], (Point)trapezoid[2]);
        Angle angle4 = GetAngle((Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
        if ((angle3 == angle1 && angle4 == angle2) || (angle3 == angle2 && angle4 == angle1))
        {
            IsoscelesTrapezoid pred = new IsoscelesTrapezoid((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
            pred.AddReason();
            pred.AddCondition(trapezoid, angleSizeEqual);
            updater.Add(pred);
        }
    }

    [Alias("等腰梯形的判定（对角线相等）")]
    public void RuleDQ16IsoscelesTrapezoidDiagonalDetermination(Trapezoid trapezoid, SegmentLengthEqual segmentLengthEqual)
    {
        Segment segment1 = (Segment)segmentLengthEqual[0];
        Segment segment2 = (Segment)segmentLengthEqual[1];
        Segment segment3 = GetSegment((Point)trapezoid[0], (Point)trapezoid[2]);
        Segment segment4 = GetSegment((Point)trapezoid[1], (Point)trapezoid[3]);
        if (segment3 is null || segment4 is null) return;
        if ((segment3 == segment1 && segment4 == segment2) || (segment3 == segment2 && segment4 == segment1))
        {
            IsoscelesTrapezoid pred = new IsoscelesTrapezoid((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
            pred.AddReason();
            pred.AddCondition(trapezoid, segmentLengthEqual);
            updater.Add(pred);
        }
    }

    [Alias("平行四边形的对角线互相平分")]
    public void Rule01ParallelogramDiagonalsBisectEachOther(Parallelogram parallelogram, LineIntersection lip)
    {
        var line1 = GetLine(parallelogram[0], parallelogram[2]);
        var line2 = GetLine(parallelogram[1], parallelogram[3]);
        if (line1 is null || line2 is null) return;
        if (line1 == lip[1] && line2 == lip[2] || line2 == lip[1] && line1 == lip[2])
        {
            Midpoint pred1 = new Midpoint((Point)lip[0], (Point)parallelogram[0], (Point)parallelogram[2]);
            pred1.AddReason();
            pred1.AddCondition(parallelogram, lip);
            updater.Add(pred1);
            Midpoint pred2 = new Midpoint((Point)lip[0], (Point)parallelogram[1], (Point)parallelogram[3]);
            pred2.AddReason();
            pred2.AddCondition(parallelogram, lip);
            updater.Add(pred2);
        }
    }

    [Alias("菱形的对角线互相垂直")]
    public void Rule02RhombusDiagonalsArePerpendicular(Rhombus parallelogram, LineIntersection lip)
    {
        var line1 = GetLine(parallelogram[0], parallelogram[2]);
        var line2 = GetLine(parallelogram[1], parallelogram[3]);
        if (line1 is null || line2 is null) return;
        if (line1 == lip[1] && line2 == lip[2] || line2 == lip[1] && line1 == lip[2])
        {
            LinePerpendicular pred = new LinePerpendicular((Line)lip[1], (Line)lip[2]);
            pred.AddReason();
            pred.AddCondition(parallelogram, lip);
            updater.Add(pred);
        }
    }

    [Alias("矩形的对角线相等")]
    public void Rule03RectangleDiagonalsAreEqual(Rectangle parallelogram)
    {
        var line1 = GetSegment(parallelogram[0], parallelogram[2]);
        var line2 = GetSegment(parallelogram[1], parallelogram[3]);
        if (line1 is null || line2 is null) return;
        QuantityRatio pred = new QuantityRatio(line1.Length, line2.Length);
        pred.AddReason();
        pred.AddCondition(parallelogram);
        updater.Add(pred);
    }

    [Alias("梯形中位线定理的逆定理")]
    public void RuleLR01ConverseOfTrapezoidMidsegmentTheorem(Trapezoid quad, Midpoint md1, LineParallel parallel)
    {
        var abL = GetLine(quad[0], quad[1]);
        var cdL = GetLine(quad[2], quad[3]);
        Line perMid = null;
        if (parallel.Line1 == abL || parallel.Line1 == cdL)
        {
            perMid = parallel.Line2;
        }
        else if (parallel.Line2 == abL || parallel.Line2 == cdL)
        {
            perMid = parallel.Line1;
        }
        else
            return;
        var abS = GetSegment(quad[0], quad[1]);
        var cdS = GetSegment(quad[2], quad[3]);
        if (quad[0] == md1[1] && quad[3] == md1[2] ||
            quad[0] == md1[2] && quad[3] == md1[1])
        {
            if (perMid.Contains(md1[0]))
            {
                var otherSide = GetLine(quad[1], quad[2]);
                var c = FindCIntersection(otherSide, perMid);
                if (c is not null)
                {
                    var mid = GetSegment(md1[0], c);
                    Equation pred = new LinearEquation(new() { { abS.Length, Expr.Half }, { cdS.Length, Expr.Half }, { mid.Length, -1 } }, 0);
                    pred.AddReason();
                    pred.AddCondition(md1, quad, parallel);
                    updater.Add(pred);
                    Midpoint pred1 = new Midpoint((Point)c, (Point)quad[1], (Point)quad[2]);
                    pred1.AddReason();
                    pred1.AddCondition(md1, quad, parallel);
                    updater.Add(pred1);
                }
            }
        }
        else if (quad[1] == md1[1] && quad[2] == md1[2] ||
            quad[1] == md1[2] && quad[2] == md1[1])
        {
            if (perMid.Contains(md1[0]))
            {
                var otherSide = GetLine(quad[0], quad[3]);
                var c = FindCIntersection(otherSide, perMid);
                if (c is not null)
                {
                    var mid = GetSegment(md1[0], c);
                    Equation pred = new LinearEquation(new() { { abS.Length, Expr.Half }, { cdS.Length, Expr.Half }, { mid.Length, -1 } }, 0);
                    pred.AddReason();
                    pred.AddCondition(md1, quad, parallel);
                    updater.Add(pred);
                    Midpoint pred1 = new Midpoint((Point)c, (Point)quad[0], (Point)quad[3]);
                    pred1.AddReason();
                    pred1.AddCondition(md1, quad, parallel);
                    updater.Add(pred1);
                }
            }
        }
    }

    [Alias("四边形内角和定理")]
    public void RuleQuadGeoQuantity01QuadrilateralInteriorAngleSumFormula(Quadriliateral quad)
    {
        var angle1 = GetAngle(quad[0], quad[1], quad[2]);
        var angle2 = GetAngle(quad[1], quad[2], quad[3]);
        var angle3 = GetAngle(quad[2], quad[3], quad[0]);
        var angle4 = GetAngle(quad[3], quad[0], quad[1]);

        Equation pred = new LinearEquation(new() { { angle1.Size, 1 }, { angle2.Size, 1 }, { angle3.Size, 1 }, { angle4.Size, 1 } }, 360);
        pred.AddReason();
        pred.AddCondition(quad);
        updater.Add(pred);
    }

    [Alias("四边形面积公式")]
    public void RuleQuadGeo01QuadrilateralAreaFormula(Quadriliateral square)
    {
        var a1 = GetTriangle((Point)square[0], (Point)square[2], (Point)square[1]);
        var a2 = GetTriangle((Point)square[0], (Point)square[2], (Point)square[3]);
        if (a1 is not null && a2 is not null)
        {
            Equation pred = new LinearEquation(new() { { a1.Area, 1 }, { a2.Area, 1 }, { square.Area, -1 } }, 0);
            pred.AddReason();
            pred.AddCondition(square);
            updater.Add(pred);
        }
        var a3 = GetTriangle((Point)square[1], (Point)square[3], (Point)square[0]);
        var a4 = GetTriangle((Point)square[1], (Point)square[3], (Point)square[2]);
        if (a3 is not null && a4 is not null)
        {
            Equation pred = new LinearEquation(new() { { a3.Area, 1 }, { a4.Area, 1 }, { square.Area, -1 } }, 0);
            pred.AddReason();
            pred.AddCondition(square);
            updater.Add(pred);
        }
    }

    [Alias("四边形周长公式")]
    public void RuleQuadGeo02QuadrilateralPerimeterFormula(Quadriliateral square)
    {
        var ab = GetSegment((Point)square[0], (Point)square[1]);
        var bc = GetSegment((Point)square[1], (Point)square[2]);
        var cd = GetSegment((Point)square[2], (Point)square[3]);
        var da = GetSegment((Point)square[3], (Point)square[0]);
        Equation pred = new LinearEquation(new() { { ab.Length, 1 }, { bc.Length, 1 }, { cd.Length, 1 }, { da.Length, 1 }, { square.Perimeter, -1 } }, 0);
        pred.AddReason();
        pred.AddCondition(square);
        updater.Add(pred);
    }

    [Alias("菱形周长公式")]
    public void RuleQuadGeo03RhombusPerimeterFormula(Rhombus rh)
    {
        var quad = GetQuadriliateral((Point)rh[0], (Point)rh[1], (Point)rh[2], (Point)rh[3]);
        var ab = GetSegment((Point)rh[0], (Point)rh[1]);
        var bc = GetSegment((Point)rh[1], (Point)rh[2]);
        var cd = GetSegment((Point)rh[2], (Point)rh[3]);
        var da = GetSegment((Point)rh[3], (Point)rh[0]);
        QuantityRatio qr;
        qr = new QuantityRatio(quad.Perimeter, ab.Length, 4);
        qr.AddReason();
        qr.AddCondition(rh);
        updater.Add(qr);
        qr = new QuantityRatio(quad.Perimeter, bc.Length, 4);
        qr.AddReason();
        qr.AddCondition(rh);
        updater.Add(qr);
        qr = new QuantityRatio(quad.Perimeter, cd.Length, 4);
        qr.AddReason();
        qr.AddCondition(rh);
        updater.Add(qr);
        qr = new QuantityRatio(quad.Perimeter, da.Length, 4);
        qr.AddReason();
        qr.AddCondition(rh);
        updater.Add(qr);
    }

    [Alias("平行四边形面积公式")]
    public void RuleQuadGeo04ParallelogramAreaFormula(Parallelogram para, LinePerpendicular perpendicular)
    {
        var quad = GetQuadriliateral((Point)para[0], (Point)para[1], (Point)para[2], (Point)para[3]);
        var ab = GetLine((Point)para[0], (Point)para[1]);
        var bc = GetLine((Point)para[1], (Point)para[2]);
        var cd = GetLine((Point)para[2], (Point)para[3]);
        var da = GetLine((Point)para[3], (Point)para[0]);
        var Sab = GetSegment((Point)para[0], (Point)para[1]);
        var Sbc = GetSegment((Point)para[1], (Point)para[2]);
        var Scd = GetSegment((Point)para[2], (Point)para[3]);
        var Sda = GetSegment((Point)para[3], (Point)para[0]);
        if (perpendicular[0] == ab)
        {
            var li1 = GetLineIntersection(cd, (Line)perpendicular[1]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new ProductionEquation(new() { { Sab.Length, 1 }, { height.Length, 1 }, { quad.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }
        else if (perpendicular[1] == ab)
        {
            var li1 = GetLineIntersection(cd, (Line)perpendicular[0]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new ProductionEquation(new() { { Sab.Length, 1 }, { height.Length, 1 }, { quad.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }

        if (perpendicular[0] == bc)
        {
            var li1 = GetLineIntersection(da, (Line)perpendicular[1]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new ProductionEquation(new() { { Sbc.Length, 1 }, { height.Length, 1 }, { quad.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }
        else if (perpendicular[1] == bc)
        {
            var li1 = GetLineIntersection(da, (Line)perpendicular[0]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new ProductionEquation(new() { { Sbc.Length, 1 }, { height.Length, 1 }, { quad.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }

        if (perpendicular[0] == cd)
        {
            var li1 = GetLineIntersection(ab, (Line)perpendicular[1]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new ProductionEquation(new() { { Scd.Length, 1 }, { height.Length, 1 }, { quad.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }
        else if (perpendicular[1] == cd)
        {
            var li1 = GetLineIntersection(ab, (Line)perpendicular[0]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new ProductionEquation(new() { { Scd.Length, 1 }, { height.Length, 1 }, { quad.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }

        if (perpendicular[0] == da)
        {
            var li1 = GetLineIntersection(bc, (Line)perpendicular[1]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new ProductionEquation(new() { { Sda.Length, 1 }, { height.Length, 1 }, { quad.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }
        else if (perpendicular[1] == da)
        {
            var li1 = GetLineIntersection(bc, (Line)perpendicular[0]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new ProductionEquation(new() { { Sda.Length, 1 }, { height.Length, 1 }, { quad.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }
    }

    [Alias("梯形面积公式")]
    public void RuleQuadGeo05TrapezoidAreaFormula(Trapezoid para, LinePerpendicular perpendicular)
    {
        var quad = GetQuadriliateral((Point)para[0], (Point)para[1], (Point)para[2], (Point)para[3]);
        var ab = GetLine((Point)para[0], (Point)para[1]);
        var cd = GetLine((Point)para[2], (Point)para[3]);
        var Sab = GetSegment((Point)para[0], (Point)para[1]);
        var Scd = GetSegment((Point)para[2], (Point)para[3]);
        if (perpendicular[0] == ab)
        {
            var li1 = GetLineIntersection(cd, (Line)perpendicular[1]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new Equation((Sab.Length + Scd.Length) * height.Length / 2, quad.Area);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }
        else if (perpendicular[1] == ab)
        {
            var li1 = GetLineIntersection(cd, (Line)perpendicular[0]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new Equation((Sab.Length + Scd.Length) * height.Length / 2, quad.Area);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }

        if (perpendicular[0] == cd)
        {
            var li1 = GetLineIntersection(ab, (Line)perpendicular[1]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new Equation((Sab.Length + Scd.Length) * height.Length / 2, quad.Area);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }
        else if (perpendicular[1] == cd)
        {
            var li1 = GetLineIntersection(ab, (Line)perpendicular[0]);
            var li2 = GetLineIntersection((Line)perpendicular[0], (Line)perpendicular[1]);
            if (li1 is null || li2 is null) return;
            var height = GetSegment(li1[0], li2[0]);
            Equation pred = new Equation((Sab.Length + Scd.Length) * height.Length / 2, quad.Area);
            pred.AddReason();
            pred.AddCondition(para);
            updater.Add(pred);
        }
    }

    [Alias("正方形面积公式")]
    public void RuleQuadGeo06SquareAreaFormula(Square square)
    {
        var edge = GetSegment(square[0], square[1]);
        var quad = GetQuadriliateral((Point)square.Properties[0], (Point)square.Properties[1], (Point)square.Properties[2], (Point)square.Properties[3]);
        Equation pred = new ProductionEquation(new() { { edge.Length, 2 }, { quad.Area, -1 } }, 1);
        pred.AddReason();
        pred.AddCondition(square);
        updater.Add(pred);
    }

    [Alias("矩形面积公式")]
    public void RuleQuadGeo07RectangleAreaFormula(Rectangle square)
    {
        var edge1 = GetSegment(square[0], square[1]);
        var edge2 = GetSegment(square[1], square[2]);
        var quad = GetQuadriliateral((Point)square.Properties[0], (Point)square.Properties[1], (Point)square.Properties[2], (Point)square.Properties[3]);
        Equation pred = new ProductionEquation(new() { { edge1.Length, 1 }, { edge2.Length, 1 }, { quad.Area, -1 } }, 1);
        pred.AddReason();
        pred.AddCondition(square);
        updater.Add(pred);
    }

    [Alias("筝形面积公式")]
    public void RuleQuadGeo08KiteAreaFormula(Kite square)
    {
        var edge1 = GetSegment(square[0], square[2]);
        var edge2 = GetSegment(square[1], square[3]);
        if (edge1 is null || edge2 is null) return;
        var quad = GetQuadriliateral((Point)square.Properties[0], (Point)square.Properties[1], (Point)square.Properties[2], (Point)square.Properties[3]);
        Equation pred = new ProductionEquation(new() { { edge1.Length, 1 }, { edge2.Length, 1 }, { quad.Area, -1 } }, 2);
        pred.AddReason();
        pred.AddCondition(square);
        updater.Add(pred);
    }

    [Alias("梯形中位线定理")]
    public void RuleQuadAttri09TrapezoidMidsegmentTheorem(Trapezoid quad, Midpoint mid1, Midpoint mid2)
    {
        if (mid1 == mid2) return;
        var abS = GetSegment(quad[0], quad[1]);
        var bcS = GetSegment(quad[1], quad[2]);
        var cdS = GetSegment(quad[2], quad[3]);
        var daS = GetSegment(quad[3], quad[0]);

        var midS1 = GetSegment(mid1[1], mid1[2]);
        var midS2 = GetSegment(mid2[1], mid2[2]);
        if (bcS == midS1 && daS == midS2 || bcS == midS2 && daS == midS1)
        {
            var midL = GetSegment(mid1[0], mid2[0]);
            if (midL is not null)
            {
                Equation pred = new LinearEquation(new() { { abS.Length, 1 }, { cdS.Length, 1 }, { midL.Length, -2 } }, 0);
                pred.AddReason();
                pred.AddCondition(quad, mid1, mid2);
                updater.Add(pred);
            }
        }
    }

}
