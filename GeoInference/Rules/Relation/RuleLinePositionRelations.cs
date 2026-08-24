
[RuleType(RuleType.Tradition)]
public class RuleLinePositionRelations : RuleClass
{
    [Alias("平行线的传递性")]
    public void RuleLR001TransitivityOfParallelLines(LineParallel lineParallel1, LineParallel lineParallel2)
    {
        if (lineParallel1 == lineParallel2) return;
        Line line1;
        Line line2;
        var result = this.FindCommon(lineParallel1, lineParallel2);
        if (result.common is null)
        {
            return;
        }
        else
        {
            line1 = result.notcommon1 as Line;
            line2 = result.notcommon2 as Line;
        }
        LineParallel pred = new LineParallel(line1, line2);
        pred.AddReason();
        pred.AddCondition(lineParallel1, lineParallel2);
        updater.Add(pred);
    }

    [Alias("平行与垂直的传递性")]
    public void RuleLR002TransitivityOfParallelAndPerpendicularLines(LineParallel lineParallel, LinePerpendicular linePerpendicular)
    {
        Line line1;
        Line line2;
        var result = this.FindCommon(lineParallel, linePerpendicular);
        if (result.common is null)
        {
            return;
        }
        else
        {
            line1 = result.notcommon1 as Line;
            line2 = result.notcommon2 as Line;
        }
        LinePerpendicular pred = new LinePerpendicular(line1, line2);
        pred.AddReason();
        pred.AddCondition(lineParallel, linePerpendicular);
        updater.Add(pred);
    }

    [Alias("垂直于同一直线的两直线平行")]
    public void RuleLR003PerpendicularsToSameLineAreParallel(LinePerpendicular perpendicular1, LinePerpendicular perpendicular2)
    {
        if (perpendicular1 == perpendicular2) return;
        Line line1;
        Line line2;
        List<Point> planePoints = new List<Point>();
        foreach (var point in perpendicular1[0].Properties.Union(perpendicular1[1].Properties).Union(perpendicular2[0].Properties).Union(perpendicular2[1].Properties))
        {
            if (!planePoints.Contains(point))
                planePoints.Add(point as Point);
        }
        var result = this.FindCommon(perpendicular1, perpendicular2);
        if (result.common is null)
        {
            return;
        }
        else
        {
            line1 = result.notcommon1 as Line;
            line2 = result.notcommon2 as Line;
        }

        LineParallel pred = new LineParallel(line1, line2);
        pred.AddReason();
        pred.AddCondition(perpendicular1, perpendicular2);
        updater.Add(pred);
    }

    [Alias("直角判定两直线垂直")]
    public void RuleLR004RightAngleImpliesPerpendicularity(AngleSize angleValue)
    {
        Line line1;
        Line line2;
        if (angleValue.Expr != "90") return;
        line1 = GetLine(angleValue.Angle.Vertex, angleValue.Angle.Edge1[0]) as Line;
        line2 = GetLine(angleValue.Angle.Vertex, angleValue.Angle.Edge2[0]) as Line;
        LinePerpendicular pred = new LinePerpendicular(line1, line2);
        pred.AddReason();
        pred.AddCondition(angleValue);
        updater.Add(pred);
    }

}
