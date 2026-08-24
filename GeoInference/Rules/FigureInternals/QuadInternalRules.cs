
[RuleType(RuleType.Internal)]
internal class QuadInternalRules : RuleClass
{
    [Alias("正方形是矩形和菱形")]
    public void RuleQuadInternal001SquareIsRectangleAndRhombus(Square square)
    {
        Rectangle rect = new Rectangle((Point)square[0], (Point)square[1], (Point)square[2], (Point)square[3]);
        rect.AddReason();
        rect.AddCondition(square);
        updater.Add(rect);
        Rhombus rhombus = new Rhombus((Point)square[0], (Point)square[1], (Point)square[2], (Point)square[3]);
        rhombus.AddReason();
        rhombus.AddCondition(square);
        updater.Add(rhombus);
        Quadriliateral quadriliateral = new Quadriliateral((Point)square[0], (Point)square[1], (Point)square[2], (Point)square[3]);
        quadriliateral.AddReason();
        quadriliateral.AddCondition(square);
        updater.Add(quadriliateral);
    }

    [Alias("矩形是平行四边形")]
    public void RuleQuadInternal002RectangleIsParallelogram(Rectangle rect)
    {
        Parallelogram parallelogram = new Parallelogram((Point)rect[0], (Point)rect[1], (Point)rect[2], (Point)rect[3]);
        parallelogram.AddReason();
        parallelogram.AddCondition(rect);
        updater.Add(parallelogram);
        Quadriliateral quadriliateral = new Quadriliateral((Point)rect[0], (Point)rect[1], (Point)rect[2], (Point)rect[3]);
        quadriliateral.AddReason();
        quadriliateral.AddCondition(rect);
        updater.Add(quadriliateral);
    }

    [Alias("菱形是平行四边形和筝形")]
    public void RuleQuadInternal003RhombusIsParallelogramAndKite(Rhombus rhombus)
    {
        Parallelogram parallelogram = new Parallelogram((Point)rhombus[0], (Point)rhombus[1], (Point)rhombus[2], (Point)rhombus[3]);
        parallelogram.AddReason();
        parallelogram.AddCondition(rhombus);
        updater.Add(parallelogram);
        Kite kite = new Kite((Point)rhombus[0], (Point)rhombus[1], (Point)rhombus[2], (Point)rhombus[3]);
        kite.AddReason();
        kite.AddCondition(rhombus);
        updater.Add(kite);
    }

    [Alias("筝形是四边形")]
    public void RuleQuadInternal004KiteIsQuadrilateral(Kite quad)
    {
        Quadriliateral parallelogram = new Quadriliateral((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);
        parallelogram.AddReason();
        parallelogram.AddCondition(quad);
        updater.Add(parallelogram);
    }

    [Alias("平行四边形是四边形")]
    public void RuleQuadInternal005ParallelogramIsQuadrilateral(Parallelogram parallel)
    {
        Quadriliateral parallelogram = new Quadriliateral((Point)parallel[0], (Point)parallel[1], (Point)parallel[2], (Point)parallel[3]);
        parallelogram.AddReason();
        parallelogram.AddCondition(parallel);
        updater.Add(parallelogram);
    }

    [Alias("等腰梯形是梯形")]
    public void RuleQuadInternal006IsoscelesTrapezoidIsTrapezoid(IsoscelesTrapezoid trapezoid)
    {
        Trapezoid pred = new Trapezoid((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
        pred.AddReason();
        pred.AddCondition(trapezoid);
        updater.Add(pred);
    }

    [Alias("直角梯形是四边形")]
    public void RuleQuadInternal007RightTrapezoidIsQuadrilateral(RightTrapezoid trapezoid)
    {
        Quadriliateral pred = new Quadriliateral((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
        pred.AddReason();
        pred.AddCondition(trapezoid);
        updater.Add(pred);
    }

    [Alias("梯形是四边形")]
    public void RuleQuadInternal008TrapezoidIsQuadrilateral(Trapezoid trapezoid)
    {
        Quadriliateral pred = new Quadriliateral((Point)trapezoid[0], (Point)trapezoid[1], (Point)trapezoid[2], (Point)trapezoid[3]);
        pred.AddReason();
        pred.AddCondition(trapezoid);
        updater.Add(pred);
    }
}
