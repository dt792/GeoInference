namespace GeoInference.Definitions.Algebras.ZExpr;

public abstract class RealNode : ZExpr
{
    public static implicit operator RealNode(int i)
    {
        return FromInt(i);
    }
    public abstract double GetApproximation();

    #region overrides
    public static RealNode operator +(RealNode ZExpr1, RealNode ZExpr2) => (RealNode)ZExpr1.Add(ZExpr2);

    public static RealNode operator -(RealNode ZExpr1, RealNode ZExpr2) => (RealNode)ZExpr1.Sub(ZExpr2);

    public static RealNode operator *(RealNode ZExpr1, RealNode ZExpr2) => (RealNode)ZExpr1.Mul(ZExpr2);

    public static RealNode operator /(RealNode ZExpr1, RealNode ZExpr2) => (RealNode)ZExpr1.Div(ZExpr2);

    public static RealNode operator -(RealNode ZExpr) => ZExpr.Opposite();
    #endregion

    #region tools
    public static int FindGCD(int num1, int num2)
    {
        while (num2 != 0)
        {
            int remainder = num1 % num2;
            num1 = num2;
            num2 = remainder;
        }

        return num1;
    }
    public static ZExpr FindGCD(ZExpr a, ZExpr b)
    {
        if (b == 0)
        {
            return a;
        }
        return FindGCD(b, a % b);
    }
    #endregion

    public override abstract RealNode Clone();
    public abstract override RealNode Simplify();

    #region op
    #region 
    public override ZExpr Add(ZExpr r)
    {
        if (r is RealNode real)
        {
            RealSumNode realSum = new RealSumNode();
            realSum.Addends.Add(this.Clone());
            realSum.Addends.Add(real.Clone());
            var result = realSum.Simplify();
            return result;
        }
        else if (r is SumNode sum)
        {
            var temp = sum.Clone();
            temp.Constant = temp.Constant + this;
            var result = temp.Simplify();
            return temp;
        }
        return base.Add(r);
    }
    public override ZExpr Sub(ZExpr r)
    {
        if (r is RealNode real)
        {
            RealSumNode realSum = new RealSumNode();
            realSum.Addends.Add(this.Clone());
            realSum.Subtrahends.Add(real.Clone());
            var result = realSum.Simplify();
            return result;
        }
        else if (r is SumNode sum)
        {
            var temp = sum.Clone();
            temp.Constant = temp.Constant - this;
            var result = temp.Simplify();
            return temp;
        }
        return base.Add(r);
    }
    public override ZExpr Mul(ZExpr r)
    {
        if (r is RealNode real)
        {
            RealProductNode realSum = new RealProductNode();
            realSum.Multipliers.Add(this.Clone());
            realSum.Multipliers.Add(real.Clone());
            var result = realSum.Simplify();
            return result;
        }
        else if (r is ProductNode product)
        {
            var temp = product.Clone();
            temp.Constant = temp.Constant * this;
            var result = temp.Simplify();
            return temp;
        }
        return base.Mul(r);
    }
    public override ZExpr Div(ZExpr r)
    {
        if (r is RealNode real)
        {
            RealProductNode realSum = new RealProductNode();
            realSum.Multipliers.Add(this.Clone());
            realSum.Divisors.Add(real.Clone());
            var result = realSum.Simplify();
            return result;
        }
        else if (r is ProductNode product)
        {
            var temp = product.Clone();
            temp.Constant = temp.Constant / this;
            var result = temp.Simplify();
            return temp;
        }
        return base.Mul(r);
    }


    public override RealNode Pow(ZExpr r)
    {
        if (r is RealNode real)
        {
            RealPowerNode powerNode = new RealPowerNode();
            powerNode.Base = this.Clone();
            powerNode.Exponent = real.Clone();
            return powerNode;
        }
        throw new NotImplementedException("Exponents that are not rational numbers are currently not supported.");
    }
    public override RealNode Sqrt()
    {
        return (RealNode)this.Pow(AHalf);
    }
    #endregion

    #region 
    public override RealNode Opposite()
    {
        RealProductNode result = new RealProductNode();
        result.IsPositive = false;
        result.Multipliers.Add(this);
        return result;
    }
    public override RealNode Invert()
    {
        RealProductNode result = new RealProductNode();
        result.Divisors.Add(this);
        return result;
    }


    #endregion

    #endregion
}
