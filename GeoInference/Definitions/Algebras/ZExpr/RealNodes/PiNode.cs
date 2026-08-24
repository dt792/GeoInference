namespace GeoInference.Definitions.Algebras.ZExpr;

public class PiNode : RealNode
{
    public override PiNode Clone()
    {
        return this;
    }

    public override PiNode Simplify()
    {
        return this;
    }

    public override string ToString()
    {
        return $"Pi";
    }
    public override double GetApproximation()
    {
        return Math.PI;
    }
}
