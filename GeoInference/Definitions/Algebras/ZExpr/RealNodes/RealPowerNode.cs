namespace GeoInference.Definitions.Algebras.ZExpr;

public class RealPowerNode : RealNode
{
    public RealNode Base { get; set; }
    public RealNode Exponent { get; set; }
    public override RealPowerNode Clone()
    {
        RealPowerNode realPowerNode = new RealPowerNode();
        realPowerNode.Exponent = (RealNode)Exponent.Clone();
        realPowerNode.Base = (RealNode)Base.Clone();
        return realPowerNode;
    }

    public override RealNode Simplify()
    {
        if (IsSimplified) return this;
        IsSimplified = true;
        Base = (RealNode)Base.Simplify();
        Exponent = (RealNode)Exponent.Simplify();

        if (Base is FractionNode r && Exponent is IntNode iexp)
        {
            FractionNode rationalNode = new FractionNode();
            rationalNode.IsPositive = iexp.IsEven;
            rationalNode.Numerator = (IntNode)r.Numerator.Pow(iexp);
            rationalNode.Denominator = (IntNode)r.Denominator.Pow(iexp);
            return rationalNode;
        }
        else if (Base is IntNode iNode)
        {
            var factors = iNode.GetPrimeFactors();

            if (factors.Count != 1 || factors.First().Value != 1)
            {
                RealProductNode product = new RealProductNode();
                foreach (var item in factors)
                {
                    product.Multipliers.Add(Let(item.Key).Pow(Exponent.Clone().Mul(item.Value)));
                }
                return product.Simplify();
            }
            if (Exponent is FractionNode rationalNode)
            {
                var tempE = Exponent.Clone();
                if (tempE > 1)
                {
                    RealProductNode product = new RealProductNode();
                    while (tempE > 1)
                    {
                        tempE = (ARationalNode)(tempE - 1);
                        product.Rational = (ARationalNode)(product.Rational * Base);
                    }
                    product.Multipliers.Add(Base.Clone().Pow(tempE));
                    return product.Simplify();
                }

            }
        }
        else if (Base is FractionNode rational)
        {
            RealProductNode product = new RealProductNode();
            product.Multipliers.Add(rational.Numerator.Pow(Base));
            product.Divisors.Add(rational.Denominator.Pow(Base));
            return product;
        }
        else if (Base is RealProductNode product)
        {
            RealProductNode newProduct = new RealProductNode();
            if (product.Rational is IntNode intNode) { newProduct.Multipliers.Add(intNode.Pow(Exponent)); }
            else if (product.Rational is FractionNode rNode)
            {
                newProduct.Multipliers.Add(rNode.Numerator.Pow(Exponent));
                newProduct.Divisors.Add(rNode.Denominator.Pow(Exponent));
            }
            foreach (var item in product.Multipliers)
            {
                newProduct.Multipliers.Add(item.Pow(Exponent));
            }
            foreach (var item in product.Divisors)
            {
                newProduct.Divisors.Add(item.Pow(Exponent));
            }

            var result = newProduct.Simplify();
            return result;
        }
        else if (Base is RealPowerNode power)
        {
            RealPowerNode newPower = new RealPowerNode();
            newPower.Base = power.Base;
            newPower.Exponent = (ARationalNode)Exponent.Mul(power.Exponent).Simplify();
            var result = newPower.Simplify();
            return result;
        }

        var check = CheckDegeneration();
        if (check.Item1) { return check.Item2; }
        return this;
    }
    public (bool, RealNode) CheckDegeneration()
    {
        if (Exponent == Zero) return (true, One);
        if (Exponent == One) return (true, Base);
        return (false, null);
    }
    public override string ToString()
    {
        var front = "";
        front += $"({Base})";
        front += "^";
        front += $"({Exponent})";
        return front;
    }
    public override RealNode Pow(ZExpr e)
    {
        var temp = this.Clone();
        temp.Exponent = (RealNode)(temp.Exponent * e);
        var result = temp.Simplify();
        return (RealNode)result;
    }
    public override double GetApproximation()
    {
        return Math.Pow(Base.GetApproximation(), Exponent.GetApproximation());
    }
}
