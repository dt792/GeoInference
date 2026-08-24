
[RuleType(RuleType.Tradition)]
public class RuleCongQuad : RuleClass
{
    [Alias("全等四边形的性质")]
    public void RuleCEQ001PropertiesOfCongruentQuadrilateral(CongruentQuadriliateral quad)
    {
        var abS = GetSegment(quad[0], quad[1]);
        var bcS = GetSegment(quad[1], quad[2]);
        var cdS = GetSegment(quad[2], quad[3]);
        var daS = GetSegment(quad[3], quad[0]);
        var abcA = GetAngle(quad[0], quad[1], quad[2]);
        var bcdA = GetAngle(quad[1], quad[2], quad[3]);
        var cdaA = GetAngle(quad[2], quad[3], quad[0]);
        var dabA = GetAngle(quad[3], quad[0], quad[1]);

        var efS = GetSegment(quad[4], quad[5]);
        var fgS = GetSegment(quad[5], quad[6]);
        var ghS = GetSegment(quad[6], quad[7]);
        var heS = GetSegment(quad[7], quad[4]);
        var efgA = GetAngle(quad[4], quad[5], quad[6]);
        var fghA = GetAngle(quad[5], quad[6], quad[7]);
        var gheA = GetAngle(quad[6], quad[7], quad[4]);
        var hefA = GetAngle(quad[7], quad[4], quad[5]);

        var quad1 = GetQuadriliateral((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);
        var quad2 = GetQuadriliateral((Point)quad[4], (Point)quad[5], (Point)quad[6], (Point)quad[7]);
        if (abS != efS)
        {
            var pred = new QuantityRatio(abS.Length, efS.Length);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (bcS != fgS)
        {
            var pred = new QuantityRatio(bcS.Length, fgS.Length);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (cdS != ghS)
        {
            var pred = new QuantityRatio(cdS.Length, ghS.Length);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (daS != heS)
        {
            var pred = new QuantityRatio(daS.Length, heS.Length);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }

        if (abcA != efgA)
        {
            var pred = new QuantityRatio(abcA.Size, efgA.Size);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (bcdA != fghA)
        {
            var pred = new QuantityRatio(bcdA.Size, fghA.Size);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (cdaA != gheA)
        {
            var pred = new QuantityRatio(cdaA.Size, gheA.Size);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (dabA != hefA)
        {
            var pred = new QuantityRatio(dabA.Size, hefA.Size);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (quad1 != quad2)
        {
            var pred = new QuantityRatio(quad1.Area, quad2.Area);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
    }

    [Alias("相似四边形的性质")]
    public void RuleCEQ002PropertiesOfSimilarQuadrilateral(SimilarQuadriliateral quad)
    {
        var abS = GetSegment(quad[0], quad[1]);
        var bcS = GetSegment(quad[1], quad[2]);
        var cdS = GetSegment(quad[2], quad[3]);
        var daS = GetSegment(quad[3], quad[0]);
        var abcA = GetAngle(quad[0], quad[1], quad[2]);
        var bcdA = GetAngle(quad[1], quad[2], quad[3]);
        var cdaA = GetAngle(quad[2], quad[3], quad[0]);
        var dabA = GetAngle(quad[3], quad[0], quad[1]);

        var efS = GetSegment(quad[4], quad[5]);
        var fgS = GetSegment(quad[5], quad[6]);
        var ghS = GetSegment(quad[6], quad[7]);
        var heS = GetSegment(quad[7], quad[4]);
        var efgA = GetAngle(quad[4], quad[5], quad[6]);
        var fghA = GetAngle(quad[5], quad[6], quad[7]);
        var gheA = GetAngle(quad[6], quad[7], quad[4]);
        var hefA = GetAngle(quad[7], quad[4], quad[5]);

        var quad1 = GetQuadriliateral((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);
        var quad2 = GetQuadriliateral((Point)quad[4], (Point)quad[5], (Point)quad[6], (Point)quad[7]);
        if (abS != efS && bcS != fgS)
        {
            var pred = new ProductionEquation(new() { { abS.Length, 1 }, { fgS.Length, 1 }, { efS.Length, -1 }, { bcS.Length, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (bcS != fgS && cdS != ghS)
        {
            var pred = new ProductionEquation(new() { { bcS.Length, 1 }, { ghS.Length, 1 }, { fgS.Length, -1 }, { cdS.Length, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (cdS != ghS && daS != heS)
        {
            var pred = new ProductionEquation(new() { { cdS.Length, 1 }, { heS.Length, 1 }, { ghS.Length, -1 }, { daS.Length, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (daS != heS && abS != efS)
        {
            var pred = new ProductionEquation(new() { { daS.Length, 1 }, { efS.Length, 1 }, { heS.Length, -1 }, { abS.Length, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }

        if (abS != efS && cdS != ghS)
        {
            var pred = new ProductionEquation(new() { { abS.Length, 1 }, { ghS.Length, 1 }, { efS.Length, -1 }, { cdS.Length, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (bcS != fgS && daS != heS)
        {
            var pred = new ProductionEquation(new() { { bcS.Length, 1 }, { heS.Length, 1 }, { fgS.Length, -1 }, { daS.Length, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }

        if (abcA != efgA)
        {
            var pred = new QuantityRatio(abcA.Size, efgA.Size);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (bcdA != fghA)
        {
            var pred = new QuantityRatio(bcdA.Size, fghA.Size);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (cdaA != gheA)
        {
            var pred = new QuantityRatio(cdaA.Size, gheA.Size);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (dabA != hefA)
        {
            var pred = new QuantityRatio(dabA.Size, hefA.Size);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (abS != efS)
        {
            var pred = new ProductionEquation(new() { { abS.Length,2 }, { quad2.Area, 1 }, { efS.Length, -2 }, { quad1.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (bcS != fgS)
        {
            var pred = new ProductionEquation(new() { { bcS.Length, 2 }, { quad2.Area, 1 }, { fgS.Length, -2 }, { quad1.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (cdS != ghS)
        {
            var pred = new ProductionEquation(new() { { cdS.Length, 2 }, { quad2.Area, 1 }, { ghS.Length, -2 }, { quad1.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
        if (daS != heS)
        {
            var pred = new ProductionEquation(new() { { daS.Length, 2 }, { quad2.Area, 1 }, { heS.Length, -2 }, { quad1.Area, -1 } }, 1);
            pred.AddReason();
            pred.AddCondition(quad);
            updater.Add(pred);
        }
    }
}
