


[RuleType(RuleType.Tradition)]
public class RuleCongTri : RuleClass
{
    [Alias("三角形全等判定（SSS）")]
    [SemiConditionRule]
    public void RuleCT001TriangleCongruenceSSS(Triangle triangle1, Triangle triangle2)
    {
        if (triangle1 == triangle2) return;
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

        CondictionalKnowledge c1 = new()
        {
            Knowledge = new CongruentTriangles(a, b, c, d, e, f),
        };
        c1.AddCondiction(new SegmentLengthEqual(ab, de),
            new SegmentLengthEqual(bc, ef),
            new SegmentLengthEqual(ca, fd));
        c1.Knowledge.AddReason();
        c1.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c1);

        CondictionalKnowledge c2 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c2.AddCondiction(new SegmentLengthEqual(ab, fd),
            new SegmentLengthEqual(bc, ef),
            new SegmentLengthEqual(ca, de));
        c2.Knowledge.AddReason();
        c2.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c2);

        CondictionalKnowledge c3 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c3.AddCondiction(new SegmentLengthEqual(ab, ef),
            new SegmentLengthEqual(bc, fd),
            new SegmentLengthEqual(ca, de));
        c3.Knowledge.AddReason();
        c3.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c3);

        CondictionalKnowledge c4 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c4.AddCondiction(new SegmentLengthEqual(ab, de),
            new SegmentLengthEqual(bc, fd),
            new SegmentLengthEqual(ca, ef));
        c4.Knowledge.AddReason();
        c4.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c4);

        CondictionalKnowledge c5 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c5.AddCondiction(new SegmentLengthEqual(ab, fd),
            new SegmentLengthEqual(bc, de),
            new SegmentLengthEqual(ca, ef));
        c5.Knowledge.AddReason();
        c5.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c5);

        CondictionalKnowledge c6 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c6.AddCondiction(new SegmentLengthEqual(ab, ef),
            new SegmentLengthEqual(bc, de),
            new SegmentLengthEqual(ca, fd));
        c6.Knowledge.AddReason();
        c6.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c6);
    }

    [Alias("三角形全等判定（SAS）")]
    [SemiConditionRule]
    public void RuleCT002TriangleCongruenceSAS(Triangle triangle1, Triangle triangle2)
    {
        if (triangle1 == triangle2) return;
        Point a = (Point)triangle1[0];
        Point b = (Point)triangle1[1];
        Point c = (Point)triangle1[2];
        Point d = (Point)triangle2[0];
        Point e = (Point)triangle2[1];
        Point f = (Point)triangle2[2];
        var abc = GetAngle(triangle1[0], triangle1[1], triangle1[2]);
        var bca = GetAngle(triangle1[1], triangle1[2], triangle1[0]);
        var cab = GetAngle(triangle1[2], triangle1[0], triangle1[1]);
        var def = GetAngle(triangle2[0], triangle2[1], triangle2[2]);
        var efd = GetAngle(triangle2[1], triangle2[2], triangle2[0]);
        var fde = GetAngle(triangle2[2], triangle2[0], triangle2[1]);
        var ab = GetSegment(triangle1[0], triangle1[1]);
        var bc = GetSegment(triangle1[1], triangle1[2]);
        var ca = GetSegment(triangle1[0], triangle1[2]);

        var de = GetSegment(triangle2[0], triangle2[1]);
        var ef = GetSegment(triangle2[1], triangle2[2]);
        var fd = GetSegment(triangle2[0], triangle2[2]);

        CondictionalKnowledge c1 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c1.AddCondiction(new AngleSizeEqual(cab, fde),
            new SegmentLengthEqual(ca, fd),
            new SegmentLengthEqual(ab, de));
        c1.Knowledge.AddReason();
        c1.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c1);

        CondictionalKnowledge c2 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c2.AddCondiction(new AngleSizeEqual(abc, def),
            new SegmentLengthEqual(ab, de),
            new SegmentLengthEqual(bc, ef));
        c2.Knowledge.AddReason();
        c2.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c2);

        CondictionalKnowledge c3 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c3.AddCondiction(new AngleSizeEqual(bca, efd),
            new SegmentLengthEqual(bc, ef),
            new SegmentLengthEqual(ca, fd));
        c3.Knowledge.AddReason();
        c3.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c3);

        CondictionalKnowledge c4 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c4.AddCondiction(new AngleSizeEqual(cab, fde),
            new SegmentLengthEqual(ca, de),
            new SegmentLengthEqual(ab, fd));
        c4.Knowledge.AddReason();
        c4.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c4);

        CondictionalKnowledge c5 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c5.AddCondiction(new AngleSizeEqual(abc, efd),
            new SegmentLengthEqual(ab, fd),
            new SegmentLengthEqual(bc, ef));
        c5.Knowledge.AddReason();
        c5.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c5);

        CondictionalKnowledge c6 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c6.AddCondiction(new AngleSizeEqual(bca, def),
            new SegmentLengthEqual(bc, ef),
            new SegmentLengthEqual(ca, de));
        c6.Knowledge.AddReason();
        c6.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c6);

        CondictionalKnowledge c7 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c7.AddCondiction(new AngleSizeEqual(cab, def),
            new SegmentLengthEqual(ca, de),
            new SegmentLengthEqual(ab, ef));
        c7.Knowledge.AddReason();
        c7.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c7);

        CondictionalKnowledge c8 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c8.AddCondiction(new AngleSizeEqual(abc, efd),
            new SegmentLengthEqual(ab, ef),
            new SegmentLengthEqual(bc, fd));
        c8.Knowledge.AddReason();
        c8.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c8);

        CondictionalKnowledge c9 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c9.AddCondiction(new AngleSizeEqual(bca, fde),
            new SegmentLengthEqual(bc, fd),
            new SegmentLengthEqual(ca, de));
        c9.Knowledge.AddReason();
        c9.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c9);

        CondictionalKnowledge c10 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c10.AddCondiction(new AngleSizeEqual(cab, def),
            new SegmentLengthEqual(ca, ef),
            new SegmentLengthEqual(ab, de));
        c10.Knowledge.AddReason();
        c10.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c10);

        CondictionalKnowledge c11 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c11.AddCondiction(new AngleSizeEqual(abc, fde),
            new SegmentLengthEqual(ab, de),
            new SegmentLengthEqual(bc, fd));
        c11.Knowledge.AddReason();
        c11.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c11);

        CondictionalKnowledge c12 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c12.AddCondiction(new AngleSizeEqual(bca, efd),
            new SegmentLengthEqual(bc, fd),
            new SegmentLengthEqual(ca, ef));
        c12.Knowledge.AddReason();
        c12.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c12);

        CondictionalKnowledge c13 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c13.AddCondiction(new AngleSizeEqual(cab, efd),
            new SegmentLengthEqual(ca, ef),
            new SegmentLengthEqual(ab, fd));
        c13.Knowledge.AddReason();
        c13.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c13);

        CondictionalKnowledge c14 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c14.AddCondiction(new AngleSizeEqual(abc, fde),
            new SegmentLengthEqual(ab, fd),
            new SegmentLengthEqual(bc, de));
        c14.Knowledge.AddReason();
        c14.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c14);

        CondictionalKnowledge c15 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c15.AddCondiction(new AngleSizeEqual(bca, def),
            new SegmentLengthEqual(bc, de),
            new SegmentLengthEqual(ca, ef));
        c15.Knowledge.AddReason();
        c15.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c15);

        CondictionalKnowledge c16 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c16.AddCondiction(new AngleSizeEqual(cab, efd),
            new SegmentLengthEqual(ca, fd),
            new SegmentLengthEqual(ab, ef));
        c16.Knowledge.AddReason();
        c16.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c16);

        CondictionalKnowledge c17 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c17.AddCondiction(new AngleSizeEqual(abc, def),
            new SegmentLengthEqual(ab, ef),
            new SegmentLengthEqual(bc, de));
        c17.Knowledge.AddReason();
        c17.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c17);

        CondictionalKnowledge c18 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c18.AddCondiction(new AngleSizeEqual(bca, fde),
            new SegmentLengthEqual(bc, de),
            new SegmentLengthEqual(ca, fd));
        c18.Knowledge.AddReason();
        c18.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c18);
    }

    [Alias("三角形全等判定（ASA）")]
    [SemiConditionRule]
    public void RuleCT003TriangleCongruenceASA(Triangle triangle1, Triangle triangle2)
    {
        if (triangle1 == triangle2) return;

        Point a = (Point)triangle1[0];
        Point b = (Point)triangle1[1];
        Point c = (Point)triangle1[2];
        Point d = (Point)triangle2[0];
        Point e = (Point)triangle2[1];
        Point f = (Point)triangle2[2];
        var abc = GetAngle(triangle1[0], triangle1[1], triangle1[2]);
        var bca = GetAngle(triangle1[1], triangle1[2], triangle1[0]);
        var cab = GetAngle(triangle1[2], triangle1[0], triangle1[1]);
        var def = GetAngle(triangle2[0], triangle2[1], triangle2[2]);
        var efd = GetAngle(triangle2[1], triangle2[2], triangle2[0]);
        var fde = GetAngle(triangle2[2], triangle2[0], triangle2[1]);
        var ab = GetSegment(triangle1[0], triangle1[1]);
        var bc = GetSegment(triangle1[1], triangle1[2]);
        var ca = GetSegment(triangle1[0], triangle1[2]);

        var de = GetSegment(triangle2[0], triangle2[1]);
        var ef = GetSegment(triangle2[1], triangle2[2]);
        var fd = GetSegment(triangle2[0], triangle2[2]);

        CondictionalKnowledge c1 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c1.AddCondiction(new AngleSizeEqual(cab, fde),
            new SegmentLengthEqual(ab, de),
            new AngleSizeEqual(abc, def));
        c1.Knowledge.AddReason();
        c1.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c1);

        CondictionalKnowledge c2 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c2.AddCondiction(new AngleSizeEqual(abc, def),
            new SegmentLengthEqual(bc, ef),
            new AngleSizeEqual(bca, efd));
        c2.Knowledge.AddReason();
        c2.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c2);

        CondictionalKnowledge c3 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c3.AddCondiction(new AngleSizeEqual(bca, efd),
            new SegmentLengthEqual(ca, fd),
            new AngleSizeEqual(cab, fde));
        c3.Knowledge.AddReason();
        c3.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c3);

        CondictionalKnowledge c4 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c4.AddCondiction(new AngleSizeEqual(cab, fde),
            new SegmentLengthEqual(ab, fd),
            new AngleSizeEqual(abc, efd));
        c4.Knowledge.AddReason();
        c4.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c4);

        CondictionalKnowledge c5 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c5.AddCondiction(new AngleSizeEqual(abc, efd),
            new SegmentLengthEqual(bc, ef),
            new AngleSizeEqual(bca, def));
        c5.Knowledge.AddReason();
        c5.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c5);

        CondictionalKnowledge c6 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c6.AddCondiction(new AngleSizeEqual(bca, def),
            new SegmentLengthEqual(ca, de),
            new AngleSizeEqual(cab, fde));
        c6.Knowledge.AddReason();
        c6.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c6);

        CondictionalKnowledge c7 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c7.AddCondiction(new AngleSizeEqual(cab, def),
            new SegmentLengthEqual(ab, ef),
            new AngleSizeEqual(abc, efd));
        c7.Knowledge.AddReason();
        c7.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c7);

        CondictionalKnowledge c8 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c8.AddCondiction(new AngleSizeEqual(abc, efd),
            new SegmentLengthEqual(bc, fd),
            new AngleSizeEqual(bca, fde));
        c8.Knowledge.AddReason();
        c8.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c8);

        CondictionalKnowledge c9 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c9.AddCondiction(new AngleSizeEqual(bca, fde),
            new SegmentLengthEqual(ca, de),
            new AngleSizeEqual(cab, def));
        c9.Knowledge.AddReason();
        c9.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c9);

        CondictionalKnowledge c10 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c10.AddCondiction(new AngleSizeEqual(cab, def),
            new SegmentLengthEqual(ab, de),
            new AngleSizeEqual(abc, fde));
        c10.Knowledge.AddReason();
        c10.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c10);

        CondictionalKnowledge c11 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c11.AddCondiction(new AngleSizeEqual(abc, fde),
            new SegmentLengthEqual(bc, fd),
            new AngleSizeEqual(bca, efd));
        c11.Knowledge.AddReason();
        c11.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c11);

        CondictionalKnowledge c12 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c12.AddCondiction(new AngleSizeEqual(bca, efd),
            new SegmentLengthEqual(ca, ef),
            new AngleSizeEqual(cab, def));
        c12.Knowledge.AddReason();
        c12.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c12);

        CondictionalKnowledge c13 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c13.AddCondiction(new AngleSizeEqual(cab, efd),
            new SegmentLengthEqual(ab, fd),
            new AngleSizeEqual(abc, fde));
        c13.Knowledge.AddReason();
        c13.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c13);

        CondictionalKnowledge c14 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c14.AddCondiction(new AngleSizeEqual(abc, fde),
            new SegmentLengthEqual(bc, de),
            new AngleSizeEqual(bca, def));
        c14.Knowledge.AddReason();
        c14.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c14);

        CondictionalKnowledge c15 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c15.AddCondiction(new AngleSizeEqual(bca, def),
            new SegmentLengthEqual(ca, ef),
            new AngleSizeEqual(cab, efd));
        c15.Knowledge.AddReason();
        c15.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c15);

        CondictionalKnowledge c16 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c16.AddCondiction(new AngleSizeEqual(cab, efd),
            new SegmentLengthEqual(ab, ef),
            new AngleSizeEqual(abc, def));
        c16.Knowledge.AddReason();
        c16.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c16);

        CondictionalKnowledge c17 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c17.AddCondiction(new AngleSizeEqual(abc, def),
            new SegmentLengthEqual(bc, de),
            new AngleSizeEqual(bca, fde));
        c17.Knowledge.AddReason();
        c17.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c17);

        CondictionalKnowledge c18 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c18.AddCondiction(new AngleSizeEqual(bca, fde),
            new SegmentLengthEqual(ca, fd),
            new AngleSizeEqual(cab, efd));
        c18.Knowledge.AddReason();
        c18.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c18);
    }

    [Alias("三角形全等判定（AAS）")]
    [SemiConditionRule]
    public void RuleCT004TriangleCongruenceAAS(Triangle triangle1, Triangle triangle2)
    {
        if (triangle1 == triangle2) return;
        Point a = (Point)triangle1[0];
        Point b = (Point)triangle1[1];
        Point c = (Point)triangle1[2];
        Point d = (Point)triangle2[0];
        Point e = (Point)triangle2[1];
        Point f = (Point)triangle2[2];
        var abc = GetAngle(triangle1[0], triangle1[1], triangle1[2]);
        var bca = GetAngle(triangle1[1], triangle1[2], triangle1[0]);
        var cab = GetAngle(triangle1[2], triangle1[0], triangle1[1]);
        var def = GetAngle(triangle2[0], triangle2[1], triangle2[2]);
        var efd = GetAngle(triangle2[1], triangle2[2], triangle2[0]);
        var fde = GetAngle(triangle2[2], triangle2[0], triangle2[1]);
        var ab = GetSegment(triangle1[0], triangle1[1]);
        var bc = GetSegment(triangle1[1], triangle1[2]);
        var ca = GetSegment(triangle1[0], triangle1[2]);

        var de = GetSegment(triangle2[0], triangle2[1]);
        var ef = GetSegment(triangle2[1], triangle2[2]);
        var fd = GetSegment(triangle2[0], triangle2[2]);

        CondictionalKnowledge c1 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c1.AddCondiction(new AngleSizeEqual(cab, fde),
            new AngleSizeEqual(abc, def),
            new SegmentLengthEqual(bc, ef));
        c1.Knowledge.AddReason();
        c1.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c1);

        CondictionalKnowledge c11 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c11.AddCondiction(new AngleSizeEqual(cab, fde),
            new AngleSizeEqual(abc, def),
            new SegmentLengthEqual(ca, fd));
        c11.Knowledge.AddReason();
        c11.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c11);

        CondictionalKnowledge c2 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c2.AddCondiction(new AngleSizeEqual(abc, def),
            new AngleSizeEqual(bca, efd),
            new SegmentLengthEqual(ab, de));
        c2.Knowledge.AddReason();
        c2.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c2);

        CondictionalKnowledge c21 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c21.AddCondiction(new AngleSizeEqual(abc, def),
            new AngleSizeEqual(bca, efd),
            new SegmentLengthEqual(ca, fd));
        c21.Knowledge.AddReason();
        c21.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c21);

        CondictionalKnowledge c3 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c3.AddCondiction(new AngleSizeEqual(bca, efd),
            new AngleSizeEqual(cab, fde),
            new SegmentLengthEqual(ab, de));
        c3.Knowledge.AddReason();
        c3.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c3);

        CondictionalKnowledge c31 = new() { Knowledge = new CongruentTriangles(a, b, c, d, e, f), };
        c31.AddCondiction(new AngleSizeEqual(bca, efd),
            new AngleSizeEqual(cab, fde),
            new SegmentLengthEqual(bc, ef));
        c31.Knowledge.AddReason();
        c31.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c31);

        CondictionalKnowledge c4 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c4.AddCondiction(new AngleSizeEqual(cab, fde),
            new AngleSizeEqual(abc, efd),
            new SegmentLengthEqual(ca, de));
        c4.Knowledge.AddReason();
        c4.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c4);

        CondictionalKnowledge c41 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c41.AddCondiction(new AngleSizeEqual(cab, fde),
            new AngleSizeEqual(abc, efd),
            new SegmentLengthEqual(bc, ef));
        c41.Knowledge.AddReason();
        c41.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c41);

        CondictionalKnowledge c5 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c5.AddCondiction(new AngleSizeEqual(abc, efd),
            new AngleSizeEqual(bca, def),
            new SegmentLengthEqual(ab, fd));
        c5.Knowledge.AddReason();
        c5.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c5);

        CondictionalKnowledge c51 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c51.AddCondiction(new AngleSizeEqual(abc, efd),
            new AngleSizeEqual(bca, def),
            new SegmentLengthEqual(ca, de));
        c51.Knowledge.AddReason();
        c51.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c51);

        CondictionalKnowledge c6 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c6.AddCondiction(new AngleSizeEqual(bca, def),
            new AngleSizeEqual(cab, fde),
            new SegmentLengthEqual(ab, fd));
        c6.Knowledge.AddReason();
        c6.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c6);

        CondictionalKnowledge c61 = new() { Knowledge = new CongruentTriangles(a, b, c, d, f, e) };
        c61.AddCondiction(new AngleSizeEqual(bca, def),
            new AngleSizeEqual(cab, fde),
            new SegmentLengthEqual(bc, ef));
        c61.Knowledge.AddReason();
        c61.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c61);

        CondictionalKnowledge c7 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c7.AddCondiction(new AngleSizeEqual(cab, def),
            new AngleSizeEqual(abc, efd),
            new SegmentLengthEqual(ca, de));
        c7.Knowledge.AddReason();
        c7.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c7);

        CondictionalKnowledge c71 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c71.AddCondiction(new AngleSizeEqual(cab, def),
            new AngleSizeEqual(abc, efd),
            new SegmentLengthEqual(bc, fd));
        c71.Knowledge.AddReason();
        c71.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c71);

        CondictionalKnowledge c8 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c8.AddCondiction(new AngleSizeEqual(abc, efd),
            new AngleSizeEqual(bca, fde),
            new SegmentLengthEqual(ab, ef));
        c8.Knowledge.AddReason();
        c8.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c8);

        CondictionalKnowledge c81 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c81.AddCondiction(new AngleSizeEqual(abc, efd),
            new AngleSizeEqual(bca, fde),
            new SegmentLengthEqual(ca, de));
        c81.Knowledge.AddReason();
        c81.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c81);

        CondictionalKnowledge c9 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c9.AddCondiction(new AngleSizeEqual(bca, fde),
            new AngleSizeEqual(cab, def),
            new SegmentLengthEqual(ab, ef));
        c9.Knowledge.AddReason();
        c9.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c9);

        CondictionalKnowledge c91 = new() { Knowledge = new CongruentTriangles(a, b, c, e, f, d) };
        c91.AddCondiction(new AngleSizeEqual(bca, fde),
            new AngleSizeEqual(cab, def),
            new SegmentLengthEqual(bc, fd));
        c91.Knowledge.AddReason();
        c91.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c91);

        CondictionalKnowledge c10 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c10.AddCondiction(new AngleSizeEqual(cab, def),
            new AngleSizeEqual(abc, fde),
            new SegmentLengthEqual(ca, ef));
        c10.Knowledge.AddReason();
        c10.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c10);

        CondictionalKnowledge c101 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c101.AddCondiction(new AngleSizeEqual(cab, def),
            new AngleSizeEqual(abc, fde),
            new SegmentLengthEqual(bc, fd));
        c101.Knowledge.AddReason();
        c101.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c101);

        CondictionalKnowledge c102 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c102.AddCondiction(new AngleSizeEqual(abc, fde),
            new AngleSizeEqual(bca, efd),
            new SegmentLengthEqual(ab, de));
        c102.Knowledge.AddReason();
        c102.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c102);

        CondictionalKnowledge c103 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c103.AddCondiction(new AngleSizeEqual(abc, fde),
            new AngleSizeEqual(bca, efd),
            new SegmentLengthEqual(ca, ef));
        c103.Knowledge.AddReason();
        c103.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c103);

        CondictionalKnowledge c104 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c104.AddCondiction(new AngleSizeEqual(bca, efd),
            new AngleSizeEqual(cab, def),
            new SegmentLengthEqual(ab, de));
        c104.Knowledge.AddReason();
        c104.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c104);

        CondictionalKnowledge c105 = new() { Knowledge = new CongruentTriangles(a, b, c, e, d, f) };
        c105.AddCondiction(new AngleSizeEqual(bca, efd),
            new AngleSizeEqual(cab, def),
            new SegmentLengthEqual(bc, fd));
        c105.Knowledge.AddReason();
        c105.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c105);

        CondictionalKnowledge c106 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c106.AddCondiction(new AngleSizeEqual(cab, efd),
            new AngleSizeEqual(abc, fde),
            new SegmentLengthEqual(ca, ef));
        c106.Knowledge.AddReason();
        c106.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c106);

        CondictionalKnowledge c107 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c107.AddCondiction(new AngleSizeEqual(cab, efd),
            new AngleSizeEqual(abc, fde),
            new SegmentLengthEqual(bc, de));
        c107.Knowledge.AddReason();
        c107.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c107);

        CondictionalKnowledge c108 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c108.AddCondiction(new AngleSizeEqual(abc, fde),
            new AngleSizeEqual(bca, def),
            new SegmentLengthEqual(ab, fd));
        c108.Knowledge.AddReason();
        c108.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c108);

        CondictionalKnowledge c109 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c109.AddCondiction(new AngleSizeEqual(abc, fde),
            new AngleSizeEqual(bca, def),
            new SegmentLengthEqual(ca, ef));
        c109.Knowledge.AddReason();
        c109.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c109);

        CondictionalKnowledge c110 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c110.AddCondiction(new AngleSizeEqual(bca, def),
            new AngleSizeEqual(cab, efd),
            new SegmentLengthEqual(ab, fd));
        c110.Knowledge.AddReason();
        c110.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c110);

        CondictionalKnowledge c111 = new() { Knowledge = new CongruentTriangles(a, b, c, f, d, e) };
        c111.AddCondiction(new AngleSizeEqual(bca, def),
            new AngleSizeEqual(cab, efd),
            new SegmentLengthEqual(bc, de));
        c111.Knowledge.AddReason();
        c111.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c111);

        CondictionalKnowledge c112 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c112.AddCondiction(new AngleSizeEqual(cab, efd),
            new AngleSizeEqual(abc, def),
            new SegmentLengthEqual(ca, fd));
        c112.Knowledge.AddReason();
        c112.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c112);

        CondictionalKnowledge c113 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c113.AddCondiction(new AngleSizeEqual(cab, efd),
            new AngleSizeEqual(abc, def),
            new SegmentLengthEqual(bc, de));
        c113.Knowledge.AddReason();
        c113.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c113);

        CondictionalKnowledge c114 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c114.AddCondiction(new AngleSizeEqual(abc, def),
            new AngleSizeEqual(bca, fde),
            new SegmentLengthEqual(ab, ef));
        c114.Knowledge.AddReason();
        c114.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c114);

        CondictionalKnowledge c115 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c115.AddCondiction(new AngleSizeEqual(abc, def),
            new AngleSizeEqual(bca, fde),
            new SegmentLengthEqual(ca, fd));
        c115.Knowledge.AddReason();
        c115.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c115);

        CondictionalKnowledge c116 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c116.AddCondiction(new AngleSizeEqual(bca, fde),
            new AngleSizeEqual(cab, efd),
            new SegmentLengthEqual(bc, de));
        c116.Knowledge.AddReason();
        c116.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c116);

        CondictionalKnowledge c117 = new() { Knowledge = new CongruentTriangles(a, b, c, f, e, d) };
        c117.AddCondiction(new AngleSizeEqual(bca, fde),
            new AngleSizeEqual(cab, efd),
            new SegmentLengthEqual(ab, ef));
        c117.Knowledge.AddReason();
        c117.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c117);
    }

    [Alias("直角三角形全等判定（HL）")]
    [SemiConditionRule]
    public void RuleCT005TriangleCongruenceHL(RightTriangle triangle1, RightTriangle triangle2)
    {
        if (triangle1 == triangle2) return;
        var ab = GetSegment(triangle1[0], triangle1[1]);
        var bc = GetSegment(triangle1[1], triangle1[2]);
        var ca = GetSegment(triangle1[0], triangle1[2]);

        var de = GetSegment(triangle2[0], triangle2[1]);
        var ef = GetSegment(triangle2[1], triangle2[2]);
        var fd = GetSegment(triangle2[0], triangle2[2]);

        CondictionalKnowledge c1 = new()
        {
            Knowledge = new CongruentTriangles((Point)triangle1[0], (Point)triangle1[1], (Point)triangle1[2],
        (Point)triangle2[0], (Point)triangle2[1], (Point)triangle2[2]),
        };
        c1.AddCondiction(new SegmentLengthEqual(ab, de),
            new SegmentLengthEqual(bc, ef));
        c1.Knowledge.AddReason();
        c1.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c1);

        CondictionalKnowledge c2 = new()
        {
            Knowledge = new CongruentTriangles((Point)triangle1[0], (Point)triangle1[1], (Point)triangle1[2],
        (Point)triangle2[0], (Point)triangle2[1], (Point)triangle2[2]),
        };
        c2.AddCondiction(new SegmentLengthEqual(ca, fd),
            new SegmentLengthEqual(bc, ef));
        c2.Knowledge.AddReason();
        c2.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c2);

        CondictionalKnowledge c3 = new()
        {
            Knowledge = new CongruentTriangles((Point)triangle1[0], (Point)triangle1[1], (Point)triangle1[2],
        (Point)triangle2[0], (Point)triangle2[2], (Point)triangle2[1]),
        };
        c3.AddCondiction(new SegmentLengthEqual(ca, de),
            new SegmentLengthEqual(bc, ef));
        c3.Knowledge.AddReason();
        c3.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c3);

        CondictionalKnowledge c4 = new()
        {
            Knowledge = new CongruentTriangles((Point)triangle1[0], (Point)triangle1[1], (Point)triangle1[2],
        (Point)triangle2[0], (Point)triangle2[2], (Point)triangle2[1]),
        };
        c4.AddCondiction(new SegmentLengthEqual(ab, fd),
            new SegmentLengthEqual(bc, ef));
        c4.Knowledge.AddReason();
        c4.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c4);
    }

    [Alias("三角形相似判定（AA）")]
    [SemiConditionRule]
    public void RuleCT006TriangleSimilarityAA(Triangle triangle1, Triangle triangle2)
    {
        if (triangle1 == triangle2) return;
        Point a = (Point)triangle1[0];
        Point b = (Point)triangle1[1];
        Point c = (Point)triangle1[2];
        Point d = (Point)triangle2[0];
        Point e = (Point)triangle2[1];
        Point f = (Point)triangle2[2];
        var abc = GetAngle(triangle1[0], triangle1[1], triangle1[2]);
        var bca = GetAngle(triangle1[1], triangle1[2], triangle1[0]);
        var cab = GetAngle(triangle1[2], triangle1[0], triangle1[1]);
        var def = GetAngle(triangle2[0], triangle2[1], triangle2[2]);
        var efd = GetAngle(triangle2[1], triangle2[2], triangle2[0]);
        var fde = GetAngle(triangle2[2], triangle2[0], triangle2[1]);

        CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f), };
        c1.AddCondiction(new AngleSizeEqual(cab, fde),
            new AngleSizeEqual(abc, def));
        c1.Knowledge.AddReason();
        c1.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c1);

        CondictionalKnowledge c2 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f), };
        c2.AddCondiction(new AngleSizeEqual(abc, def),
            new AngleSizeEqual(bca, efd));
        c2.Knowledge.AddReason();
        c2.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c2);

        CondictionalKnowledge c3 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f), };
        c3.AddCondiction(new AngleSizeEqual(bca, efd),
            new AngleSizeEqual(cab, fde));
        c3.Knowledge.AddReason();
        c3.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c3);

        CondictionalKnowledge c4 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
        c4.AddCondiction(new AngleSizeEqual(cab, fde),
            new AngleSizeEqual(abc, efd));
        c4.Knowledge.AddReason();
        c4.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c4);

        CondictionalKnowledge c5 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
        c5.AddCondiction(new AngleSizeEqual(abc, efd),
            new AngleSizeEqual(bca, def));
        c5.Knowledge.AddReason();
        c5.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c5);

        CondictionalKnowledge c6 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
        c6.AddCondiction(new AngleSizeEqual(bca, def),
            new AngleSizeEqual(cab, fde));
        c6.Knowledge.AddReason();
        c6.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c6);

        CondictionalKnowledge c7 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
        c7.AddCondiction(new AngleSizeEqual(cab, def),
            new AngleSizeEqual(abc, efd));
        c7.Knowledge.AddReason();
        c7.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c7);

        CondictionalKnowledge c8 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
        c8.AddCondiction(new AngleSizeEqual(abc, efd),
            new AngleSizeEqual(bca, fde));
        c8.Knowledge.AddReason();
        c8.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c8);

        CondictionalKnowledge c9 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
        c9.AddCondiction(new AngleSizeEqual(bca, fde),
            new AngleSizeEqual(cab, def));
        c9.Knowledge.AddReason();
        c9.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c9);

        CondictionalKnowledge c10 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
        c10.AddCondiction(new AngleSizeEqual(cab, def),
            new AngleSizeEqual(abc, fde));
        c10.Knowledge.AddReason();
        c10.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c10);

        CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
        c11.AddCondiction(new AngleSizeEqual(abc, fde),
            new AngleSizeEqual(bca, efd));
        c11.Knowledge.AddReason();
        c11.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c11);

        CondictionalKnowledge c12 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
        c12.AddCondiction(new AngleSizeEqual(bca, efd),
            new AngleSizeEqual(cab, def));
        c12.Knowledge.AddReason();
        c12.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c12);

        CondictionalKnowledge c13 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
        c13.AddCondiction(new AngleSizeEqual(cab, efd),
            new AngleSizeEqual(abc, fde));
        c13.Knowledge.AddReason();
        c13.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c13);

        CondictionalKnowledge c14 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
        c14.AddCondiction(new AngleSizeEqual(abc, fde),
            new AngleSizeEqual(bca, def));
        c14.Knowledge.AddReason();
        c14.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c14);

        CondictionalKnowledge c15 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
        c15.AddCondiction(new AngleSizeEqual(bca, def),
            new AngleSizeEqual(cab, efd));
        c15.Knowledge.AddReason();
        c15.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c15);

        CondictionalKnowledge c16 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
        c16.AddCondiction(new AngleSizeEqual(cab, efd),
            new AngleSizeEqual(abc, def));
        c16.Knowledge.AddReason();
        c16.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c16);

        CondictionalKnowledge c17 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
        c17.AddCondiction(new AngleSizeEqual(abc, def),
            new AngleSizeEqual(bca, fde));
        c17.Knowledge.AddReason();
        c17.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c17);

        CondictionalKnowledge c18 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
        c18.AddCondiction(new AngleSizeEqual(bca, fde),
            new AngleSizeEqual(cab, efd));
        c18.Knowledge.AddReason();
        c18.Knowledge.AddCondition(triangle1, triangle2);
        updater.AddCondictionalKnowledgePair(c18);
    }

    [Alias("三角形相似判定（SAS）")]
    [SemiConditionRule]
    public void RuleCT007TriangleSimilaritySAS(Triangle triangle1, Triangle triangle2, SegmentLengthRatio ratio)
    {
        if (triangle1 == triangle2) return;
        Point a = (Point)triangle1[0];
        Point b = (Point)triangle1[1];
        Point c = (Point)triangle1[2];
        Point d = (Point)triangle2[0];
        Point e = (Point)triangle2[1];
        Point f = (Point)triangle2[2];
        var abc = GetAngle(triangle1[0], triangle1[1], triangle1[2]);
        var bca = GetAngle(triangle1[1], triangle1[2], triangle1[0]);
        var cab = GetAngle(triangle1[2], triangle1[0], triangle1[1]);
        var def = GetAngle(triangle2[0], triangle2[1], triangle2[2]);
        var efd = GetAngle(triangle2[1], triangle2[2], triangle2[0]);
        var fde = GetAngle(triangle2[2], triangle2[0], triangle2[1]);
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
        if (seg1 == ab && seg2 == de)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f) };
            c1.AddCondiction(new AngleSizeEqual(cab, fde), new SegmentLengthRatio(ca, fd, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c12 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f) };
            c12.AddCondiction(new AngleSizeEqual(abc, def), new SegmentLengthRatio(bc, ef, r));
            c12.Knowledge.AddReason();
            c12.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c12);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
            c11.AddCondiction(new AngleSizeEqual(cab, def), new SegmentLengthRatio(ca, ef, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);

            CondictionalKnowledge c112 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
            c112.AddCondiction(new AngleSizeEqual(abc, fde), new SegmentLengthRatio(bc, fd, r));
            c112.Knowledge.AddReason();
            c112.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c112);
        }
        else if (seg1 == ab && seg2 == ef)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
            c1.AddCondiction(new AngleSizeEqual(cab, def), new SegmentLengthRatio(ca, de, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c12 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
            c12.AddCondiction(new AngleSizeEqual(abc, efd), new SegmentLengthRatio(bc, fd, r));
            c12.Knowledge.AddReason();
            c12.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c12);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
            c11.AddCondiction(new AngleSizeEqual(cab, efd), new SegmentLengthRatio(ca, fd, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);

            CondictionalKnowledge c112 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
            c112.AddCondiction(new AngleSizeEqual(abc, def), new SegmentLengthRatio(bc, de, r));
            c112.Knowledge.AddReason();
            c112.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c112);
        }
        else if (seg1 == ab && seg2 == fd)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
            c1.AddCondiction(new AngleSizeEqual(cab, efd), new SegmentLengthRatio(ca, ef, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c12 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
            c12.AddCondiction(new AngleSizeEqual(abc, fde), new SegmentLengthRatio(bc, de, r));
            c12.Knowledge.AddReason();
            c12.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c12);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
            c11.AddCondiction(new AngleSizeEqual(cab, fde), new SegmentLengthRatio(ca, de, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);

            CondictionalKnowledge c112 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
            c112.AddCondiction(new AngleSizeEqual(abc, efd), new SegmentLengthRatio(bc, ef, r));
            c112.Knowledge.AddReason();
            c112.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c112);
        }

        if (seg1 == bc && seg2 == de)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
            c1.AddCondiction(new AngleSizeEqual(abc, fde), new SegmentLengthRatio(ab, fd, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c12 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
            c12.AddCondiction(new AngleSizeEqual(bca, def), new SegmentLengthRatio(ca, ef, r));
            c12.Knowledge.AddReason();
            c12.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c12);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
            c11.AddCondiction(new AngleSizeEqual(abc, def), new SegmentLengthRatio(ab, ef, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);

            CondictionalKnowledge c112 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
            c112.AddCondiction(new AngleSizeEqual(bca, fde), new SegmentLengthRatio(ca, fd, r));
            c112.Knowledge.AddReason();
            c112.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c112);
        }
        else if (seg1 == bc && seg2 == ef)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f) };
            c1.AddCondiction(new AngleSizeEqual(abc, def), new SegmentLengthRatio(ab, de, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c12 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f) };
            c12.AddCondiction(new AngleSizeEqual(bca, efd), new SegmentLengthRatio(ca, fd, r));
            c12.Knowledge.AddReason();
            c12.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c12);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
            c11.AddCondiction(new AngleSizeEqual(abc, efd), new SegmentLengthRatio(ab, fd, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);

            CondictionalKnowledge c112 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
            c112.AddCondiction(new AngleSizeEqual(bca, def), new SegmentLengthRatio(ca, de, r));
            c112.Knowledge.AddReason();
            c112.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c112);
        }
        else if (seg1 == bc && seg2 == fd)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
            c1.AddCondiction(new AngleSizeEqual(abc, fde), new SegmentLengthRatio(ab, de, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c12 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
            c12.AddCondiction(new AngleSizeEqual(bca, efd), new SegmentLengthRatio(ca, ef, r));
            c12.Knowledge.AddReason();
            c12.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c12);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
            c11.AddCondiction(new AngleSizeEqual(abc, efd), new SegmentLengthRatio(ab, ef, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);

            CondictionalKnowledge c112 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
            c112.AddCondiction(new AngleSizeEqual(bca, fde), new SegmentLengthRatio(ca, de, r));
            c112.Knowledge.AddReason();
            c112.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c112);
        }

        if (seg1 == ca && seg2 == de)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
            c1.AddCondiction(new AngleSizeEqual(cab, fde), new SegmentLengthRatio(ab, fd, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c12 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
            c12.AddCondiction(new AngleSizeEqual(bca, def), new SegmentLengthRatio(bc, ef, r));
            c12.Knowledge.AddReason();
            c12.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c12);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
            c11.AddCondiction(new AngleSizeEqual(cab, def), new SegmentLengthRatio(ab, ef, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);

            CondictionalKnowledge c112 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
            c112.AddCondiction(new AngleSizeEqual(bca, fde), new SegmentLengthRatio(bc, fd, r));
            c112.Knowledge.AddReason();
            c112.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c112);
        }
        else if (seg1 == ca && seg2 == ef)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
            c1.AddCondiction(new AngleSizeEqual(cab, efd), new SegmentLengthRatio(ab, fd, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c12 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
            c12.AddCondiction(new AngleSizeEqual(bca, def), new SegmentLengthRatio(bc, de, r));
            c12.Knowledge.AddReason();
            c12.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c12);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
            c11.AddCondiction(new AngleSizeEqual(cab, def), new SegmentLengthRatio(ab, de, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);

            CondictionalKnowledge c112 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
            c112.AddCondiction(new AngleSizeEqual(bca, efd), new SegmentLengthRatio(bc, fd, r));
            c112.Knowledge.AddReason();
            c112.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c112);
        }
        else if (seg1 == ca && seg2 == fd)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
            c1.AddCondiction(new AngleSizeEqual(cab, efd), new SegmentLengthRatio(ab, ef, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c12 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
            c12.AddCondiction(new AngleSizeEqual(bca, fde), new SegmentLengthRatio(bc, de, r));
            c12.Knowledge.AddReason();
            c12.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c12);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f) };
            c11.AddCondiction(new AngleSizeEqual(cab, fde), new SegmentLengthRatio(ab, de, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);

            CondictionalKnowledge c112 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f) };
            c112.AddCondiction(new AngleSizeEqual(bca, efd), new SegmentLengthRatio(bc, ef, r));
            c112.Knowledge.AddReason();
            c112.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c112);
        }
    }

    [Alias("三角形相似判定（SSS）")]
    [SemiConditionRule]
    public void RuleCT008TriangleSimilaritySSS(Triangle triangle1, Triangle triangle2, SegmentLengthRatio ratio)
    {
        if (triangle1 == triangle2) return;
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
        var abc = GetAngle(triangle1[0], triangle1[1], triangle1[2]);
        var bca = GetAngle(triangle1[1], triangle1[2], triangle1[0]);
        var cab = GetAngle(triangle1[2], triangle1[0], triangle1[1]);
        var def = GetAngle(triangle2[0], triangle2[1], triangle2[2]);
        var efd = GetAngle(triangle2[1], triangle2[2], triangle2[0]);
        var fde = GetAngle(triangle2[2], triangle2[0], triangle2[1]);
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
        if (seg1 == ab && seg2 == de)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f) };
            c1.AddCondiction(new SegmentLengthRatio(ca, fd, r), new SegmentLengthRatio(bc, ef, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
            c11.AddCondiction(new SegmentLengthRatio(ca, ef, r), new SegmentLengthRatio(bc, fd, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);
        }
        else if (seg1 == ab && seg2 == ef)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
            c1.AddCondiction(new SegmentLengthRatio(ca, de, r), new SegmentLengthRatio(bc, fd, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
            c11.AddCondiction(new SegmentLengthRatio(ca, fd, r), new SegmentLengthRatio(bc, de, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);
        }
        else if (seg1 == ab && seg2 == fd)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
            c1.AddCondiction(new SegmentLengthRatio(ca, ef, r), new SegmentLengthRatio(bc, de, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
            c11.AddCondiction(new SegmentLengthRatio(ca, de, r), new SegmentLengthRatio(bc, ef, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);
        }

        if (seg1 == bc && seg2 == de)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
            c1.AddCondiction(new SegmentLengthRatio(ca, ef, r), new SegmentLengthRatio(ab, fd, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
            c11.AddCondiction(new SegmentLengthRatio(ca, fd, r), new SegmentLengthRatio(ab, ef, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);
        }
        else if (seg1 == bc && seg2 == ef)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f) };
            c1.AddCondiction(new SegmentLengthRatio(ca, fd, r), new SegmentLengthRatio(ab, de, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
            c11.AddCondiction(new SegmentLengthRatio(ca, de, r), new SegmentLengthRatio(ab, fd, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);
        }
        else if (seg1 == bc && seg2 == fd)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
            c1.AddCondiction(new SegmentLengthRatio(ca, de, r), new SegmentLengthRatio(ab, ef, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
            c11.AddCondiction(new SegmentLengthRatio(ca, ef, r), new SegmentLengthRatio(ab, de, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);
        }

        if (seg1 == ca && seg2 == de)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, d, f, e) };
            c1.AddCondiction(new SegmentLengthRatio(ab, fd, r), new SegmentLengthRatio(bc, ef, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, e, f, d) };
            c11.AddCondiction(new SegmentLengthRatio(ab, ef, r), new SegmentLengthRatio(bc, fd, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);
        }
        else if (seg1 == ca && seg2 == ef)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, f, d, e) };
            c1.AddCondiction(new SegmentLengthRatio(ab, fd, r), new SegmentLengthRatio(bc, de, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, e, d, f) };
            c11.AddCondiction(new SegmentLengthRatio(ab, de, r), new SegmentLengthRatio(bc, fd, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);
        }
        else if (seg1 == ca && seg2 == fd)
        {
            CondictionalKnowledge c1 = new() { Knowledge = new SimilarTriangles(a, b, c, f, e, d) };
            c1.AddCondiction(new SegmentLengthRatio(ab, ef, r), new SegmentLengthRatio(bc, de, r));
            c1.Knowledge.AddReason();
            c1.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c1);

            CondictionalKnowledge c11 = new() { Knowledge = new SimilarTriangles(a, b, c, d, e, f) };
            c11.AddCondiction(new SegmentLengthRatio(ab, de, r), new SegmentLengthRatio(bc, ef, r));
            c11.Knowledge.AddReason();
            c11.Knowledge.AddCondition(triangle1, triangle2);
            updater.AddCondictionalKnowledgePair(c11);
        }
    }

    #region Properties
    [Alias("全等三角形的性质")]
    public void RuleCT011PropertiesOfCongruentTriangles(CongruentTriangles tri)
    {
        var abS = GetSegment(tri[0], tri[1]);
        var bcS = GetSegment(tri[1], tri[2]);
        var caS = GetSegment(tri[2], tri[0]);

        var abcA = GetAngle(tri[0], tri[1], tri[2]);
        var bcaA = GetAngle(tri[1], tri[2], tri[0]);
        var cabA = GetAngle(tri[2], tri[0], tri[1]);

        var deS = GetSegment(tri[3], tri[4]);
        var efS = GetSegment(tri[4], tri[5]);
        var fdS = GetSegment(tri[5], tri[3]);

        var defA = GetAngle(tri[3], tri[4], tri[5]);
        var efdA = GetAngle(tri[4], tri[5], tri[3]);
        var fdeA = GetAngle(tri[5], tri[3], tri[4]);

        QuantityRatio qr;
        if (abS != deS)
        {
            qr = new QuantityRatio(abS.Length, deS.Length);
            qr.AddReason();
            qr.AddCondition(tri);
            updater.Add(qr);
        }
        if (bcS != efS)
        {
            qr = new QuantityRatio(bcS.Length, efS.Length);
            qr.AddReason();
            qr.AddCondition(tri);
            updater.Add(qr);
        }
        if (caS != fdS)
        {
            qr = new QuantityRatio(caS.Length, fdS.Length);
            qr.AddReason();
            qr.AddCondition(tri);
            updater.Add(qr);
        }

        if (abcA != defA)
        {
            qr = new QuantityRatio(abcA.Size, defA.Size);
            qr.AddReason();
            qr.AddCondition(tri);
            updater.Add(qr);
        }
        if (bcaA != efdA)
        {
            qr = new QuantityRatio(bcaA.Size, efdA.Size);
            qr.AddReason();
            qr.AddCondition(tri);
            updater.Add(qr);
        }
        if (cabA != fdeA)
        {
            qr = new QuantityRatio(cabA.Size, fdeA.Size);
            qr.AddReason();
            qr.AddCondition(tri);
            updater.Add(qr);
        }
        var tri1 = GetTriangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
        var tri2 = GetTriangle((Point)tri[3], (Point)tri[4], (Point)tri[5]);
        qr = new QuantityRatio(tri1.Area, tri2.Area);
        qr.AddReason();
        qr.AddCondition(tri);
        updater.Add(qr);
    }

    [Alias("相似三角形的性质")]
    public void RuleCT012PropertiesOfSimilarTriangles(SimilarTriangles tri)
    {
        var abS = GetSegment(tri[0], tri[1]);
        var bcS = GetSegment(tri[1], tri[2]);
        var caS = GetSegment(tri[2], tri[0]);

        var abcA = GetAngle(tri[0], tri[1], tri[2]);
        var bcaA = GetAngle(tri[1], tri[2], tri[0]);
        var cabA = GetAngle(tri[2], tri[0], tri[1]);

        var deS = GetSegment(tri[3], tri[4]);
        var efS = GetSegment(tri[4], tri[5]);
        var fdS = GetSegment(tri[5], tri[3]);

        var defA = GetAngle(tri[3], tri[4], tri[5]);
        var efdA = GetAngle(tri[4], tri[5], tri[3]);
        var fdeA = GetAngle(tri[5], tri[3], tri[4]);

        ProductionEquation pe;
        if (abS != deS && bcS != efS)
        {
            pe = new ProductionEquation(1, [(abS.Length, 1), (efS.Length, 1), (deS.Length, -1), (bcS.Length, -1)]);
            pe.AddReason();
            pe.AddCondition(tri);
            updater.Add(pe);
        }
        if (bcS != efS && caS != fdS)
        {
            pe = new ProductionEquation(1, [(bcS.Length, 1), (fdS.Length, 1), (efS.Length, -1), (caS.Length, -1)]);
            pe.AddReason();
            pe.AddCondition(tri);
            updater.Add(pe);
        }
        if (caS != fdS && abS != deS)
        {
            pe = new ProductionEquation(1, [(caS.Length, 1), (deS.Length, 1), (fdS.Length, -1), (abS.Length, -1)]);
            pe.AddReason();
            pe.AddCondition(tri);
            updater.Add(pe);
        }

        QuantityRatio qr;
        if (abcA != defA)
        {
            qr = new QuantityRatio(abcA.Size, defA.Size);
            qr.AddReason();
            qr.AddCondition(tri);
            updater.Add(qr);
        }
        if (bcaA != efdA)
        {
            qr = new QuantityRatio(bcaA.Size, efdA.Size);
            qr.AddReason();
            qr.AddCondition(tri);
            updater.Add(qr);
        }
        if (cabA != fdeA)
        {
            qr = new QuantityRatio(cabA.Size, fdeA.Size);
            qr.AddReason();
            qr.AddCondition(tri);
            updater.Add(qr);
        }
        var tri1 = GetTriangle((Point)tri[0], (Point)tri[1], (Point)tri[2]);
        var tri2 = GetTriangle((Point)tri[3], (Point)tri[4], (Point)tri[5]);
        if (abS != deS)
        {
            pe = new ProductionEquation(1, [(abS.Length, 2), (tri2.Area, 1), (deS.Length, -2), (tri1.Area, -1)]);
            pe.AddReason();
            pe.AddCondition(tri);
            updater.Add(pe);
        }
        if (bcS != efS)
        {
            pe = new ProductionEquation(1, [(bcS.Length, 2), (tri2.Area, 1), (efS.Length, -2), (tri1.Area, -1)]);
            pe.AddReason();
            pe.AddCondition(tri);
            updater.Add(pe);
        }
        if (caS != fdS)
        {
            pe = new ProductionEquation(1, [(caS.Length, 2), (tri2.Area, 1), (fdS.Length, -2), (tri1.Area, -1)]);
            pe.AddReason();
            pe.AddCondition(tri);
            updater.Add(pe);
        }
    }
    #endregion

}
