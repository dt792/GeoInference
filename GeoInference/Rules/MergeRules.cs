using System;
using System.Collections.Generic;
using System.Text;

namespace GeoInference.Rules;


[RuleType(RuleType.BaseFigureSearching)]
public class FineLinesRules : RuleClass
{
    [Alias("线段确定直线")]
    public void RuleFL001LineFromSegment(Segment segment)
    {
        var pred = new Line((Point)segment[0], (Point)segment[1]);
        pred.AddReason();
        pred.AddCondition(segment);
        updater.Add(pred);
    }
    [Alias("三角形的边与连线生成")]
    public void RuleFL002TriangleLineRelationship(Triangle triangle)
    {
        List<Knowledge> preds = new List<Knowledge>();
        List<Knowledge> knowledge = new List<Knowledge>();
        knowledge.Add(new Segment((Point)triangle[0], (Point)triangle[1]));
        knowledge.Add(new Segment((Point)triangle[1], (Point)triangle[2]));
        knowledge.Add(new Segment((Point)triangle[0], (Point)triangle[2]));
        knowledge.Add(new Line((Point)triangle[0], (Point)triangle[1]));
        knowledge.Add(new Line((Point)triangle[1], (Point)triangle[2]));
        knowledge.Add(new Line((Point)triangle[0], (Point)triangle[2]));
        foreach (var k in knowledge)
        {
            k.AddReason();
            k.AddCondition(triangle);
            updater.Add(k);
        }
    }
    [Alias("四边形的边与连线生成")]
    public void RuleFL003QuadrilateralLineRelationship(Quadriliateral quad)
    {
        List<Knowledge> preds = new List<Knowledge>();
        List<Knowledge> knowledge = new List<Knowledge>();
        knowledge.Add(new Segment((Point)quad[0], (Point)quad[1]));
        knowledge.Add(new Segment((Point)quad[1], (Point)quad[2]));
        knowledge.Add(new Segment((Point)quad[2], (Point)quad[3]));
        knowledge.Add(new Segment((Point)quad[3], (Point)quad[0]));
        knowledge.Add(new Line((Point)quad[0], (Point)quad[1]));
        knowledge.Add(new Line((Point)quad[1], (Point)quad[2]));
        knowledge.Add(new Line((Point)quad[2], (Point)quad[3]));
        knowledge.Add(new Line((Point)quad[3], (Point)quad[0]));
        foreach (var k in knowledge)
        {
            k.AddReason();
            k.AddCondition(quad);
            updater.Add(k); ;
        }
    }
    [Alias("点在线段上推出共线")]
    public void RuleFL004PointOnLineGetCollinear(PointOnSeg pb)
    {
        Line pred = new Line((Point)pb[0], (Point)pb[1], (Point)pb[2]);
        pred.AddReason();
        pred.AddCondition(pb);
        updater.Add(pred);
    }
    [Alias("直线交点推出共线")]
    public void RuleFL005LineIntersectionGetCollinear(LineIntersection lp)
    {
        List<Point> list1 = new List<Point>(lp[1].Properties.Select(p => (Point)p));
        list1.Add((Point)lp[0]);
        Line pred = new Line(list1.Distinct().ToArray());

        pred.AddReason();
        pred.AddCondition(lp);

        List<Point> list2 = new List<Point>(lp[2].Properties.Select(p => (Point)p));
        list2.Add((Point)lp[0]);
        Line pred2 = new Line(list2.Distinct().ToArray());

        pred2.AddReason();
        pred2.AddCondition(lp);
        updater.Add(pred);
        updater.Add(pred2);
    }

    [Alias("线段生成直线")]
    public void RuleFL006GenerateLine(Segment seg)
    {
        Line pred = new Line((Point)seg[0], (Point)seg[1]);
        pred.AddReason();
        pred.AddCondition();
        updater.Add(pred);
    }
}
[RuleType(RuleType.AutoGeneration)]
public class AutoGenerateRules : RuleClass
{
    [Alias("三角形的自动构造")]
    public void RuleAutoGen001TriangleGeneration(Point p1, Point p2, Point p3)
    {
        if (p1 == p2 || p1 == p3 || p2 == p3) return ;
        if (HasSegment(p1, p2) && HasSegment(p1, p3) && HasSegment(p2, p3))
        {
            if (!HasColine(p1, p2, p3))
            {
                Triangle pred = new Triangle(p1, p2, p3);
                pred.AddReason();
                updater.Add(pred);
            }
        }
    }
    [Alias("四边形的自动构造")]
    public void RuleAutoGen002QuadrilateralGeneration(Point p1, Point p2, Point p3, Point p4)
    {
        if (p1 == p2 || p1 == p3 || p1 == p4 || p3 == p2 || p4 == p2 || p3 == p4) return ;
        if (HasSegment(p1, p2) && HasSegment(p2, p3) &&
            HasSegment(p3, p4) && HasSegment(p4, p1))
        {
            if (HasColine(p1, p2, p3) || HasColine(p1, p2, p4) ||
                HasColine(p1, p3, p4) || HasColine(p2, p3, p4))
            {
                return;
            }
            var judge = GetJudgeIpsilateral(p1, p3, p2, p4);
            if (judge is PointsOnLineDifferentSide p)
            {
                if (GetJudgeIpsilateral(p1, p2, p3, p4) is PointsOnLineSameSide &&
                    GetJudgeIpsilateral(p2, p3, p4, p1) is PointsOnLineSameSide &&
                    GetJudgeIpsilateral(p3, p4, p1, p2) is PointsOnLineSameSide &&
                    GetJudgeIpsilateral(p4, p1, p2, p3) is PointsOnLineSameSide)
                {
                    Quadriliateral pred = new Quadriliateral(p1, p2, p3, p4);
                    pred.AddReason();
                    pred.AddCondition();
                    updater.Add(pred);
                }
            }

        }
    }
}
