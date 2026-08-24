
[RuleType(RuleType.Internal)]
public class TriInternalRules : RuleClass
{
    [Alias("等边三角形是等腰三角形")]
    public void RuleTriInternal001EquilateralTriangleIsIsosceles(EquilateralTriangle tri)
    {
        IsoscelesTriangle pred = new IsoscelesTriangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
        pred = new IsoscelesTriangle((Point)tri[1], (Point)tri[2], (Point)tri[0]);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
        pred = new IsoscelesTriangle((Point)tri[2], (Point)tri[0], (Point)tri[1]);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
    }

    [Alias("等腰直角三角形是等腰三角形和直角三角形")]
    public void RuleTriInternal002IsoscelesRightTriangleIsIsoscelesAndRight(IsoscelesRightTriangle tri)
    {
        IsoscelesTriangle pred = new IsoscelesTriangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
        RightTriangle pred2 = new RightTriangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
        pred2.AddReason();
        pred2.AddCondition(tri);
        updater.Add(pred2);
    }

    [Alias("等腰三角形是三角形")]
    public void RuleTriInternal003IsoscelesTriangleIsTriangle(IsoscelesTriangle tri)
    {
        Triangle pred = new Triangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
    }

    [Alias("直角三角形是三角形")]
    public void RuleTriInternal004RightTriangleIsTriangle(RightTriangle tri)
    {
        Triangle pred = new Triangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
        pred.AddReason();
        pred.AddCondition(tri);
        updater.Add(pred);
    }

}
