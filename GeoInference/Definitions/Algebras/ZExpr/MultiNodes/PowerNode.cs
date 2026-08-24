namespace GeoInference.Definitions.Algebras.ZExpr;

public class PowerNode : ZExpr
{
    public ZExpr Exponent { get; set; }
    public ZExpr Base { get; set; }

    public override ZExpr Simplify()
    {
        if (Base is QuantityNode && Exponent == ZExpr.One) return Base;

        Base = Base.Simplify();
        Exponent = Exponent.Simplify();
        var intBase = Base as IntNode;
        var powerBase = Base as PowerNode;
        var sumBase = Base as SumNode;
        var productBase = Base as ProductNode;
        var fractionBase = Base as FractionNode;

        var intExp = Exponent as IntNode;
        var productExp = Exponent as FractionNode;
        if (Exponent.CompareTo(0) == ExprCompareResult.Less)
        {
            ProductNode product = new();
            ZExpr inner = new PowerNode()
            {
                Base = Base,
                Exponent = (ARationalNode)Exponent.Opposite()
            };
            inner = inner.Simplify();
            product.Divisors.Add(inner);
            return product;
        }
        if (intBase is not null && intExp is not null)
        {
            return Let((int)Math.Pow(intBase.Value, intExp.Value));
        }
        else if (powerBase is not null)
        {
            Base = powerBase.Base;
            Exponent = (ARationalNode)Exponent.Mul(powerBase.Exponent).Simplify();
            return this;
        }
        else if (productBase is not null)
        {
            ProductNode product = new();
            foreach (var mul in productBase.Multipliers)
            {
                PowerNode powerNode = new PowerNode() { Exponent = (ARationalNode)Exponent.Clone(), Base = mul };
                product.Multipliers.Add(powerNode);
            }
            foreach (var mul in productBase.Divisors)
            {
                PowerNode powerNode = new PowerNode() { Exponent = (ARationalNode)Exponent.Clone(), Base = mul };
                product.Divisors.Add(powerNode);
            }
            var result = product.Simplify();
            return result;
        }
        else if (fractionBase is not null)
        {
            ProductNode product = new();
            PowerNode powerNode1 = new PowerNode() { Exponent = Exponent.Clone(), Base = fractionBase.Numerator };
            product.Multipliers.Add(powerNode1);
            PowerNode powerNode2 = new PowerNode() { Exponent = Exponent.Clone(), Base = fractionBase.Denominator };
            product.Multipliers.Add(powerNode2);
            var result = product.Simplify();
            return result;
        }
        var check = CheckDegeneration();
        if (check.Item1) { return check.Item2; }
        return this;
    }
    public (bool, ZExpr) CheckDegeneration()
    {

        if (Exponent.Equals(0)) return (true, 1);
        if (Exponent.Equals(1)) return (true, Base);
        if (Exponent is RealNode e && Base is RealNode b)
        {
            return (true, new RealPowerNode() { Base = b, Exponent = e });
        }
        return (false, null);
    }
    public override ZExpr Clone()
    {
        return new PowerNode() { Base = Base, Exponent = Exponent };
    }
    public override string ToString()
    {
        var front = "";
        front += $"({Base})";
        front += "^";
        front += $"({Exponent})";
        return front;
    }
}
