namespace GeoInference.Definitions.Algebras.ZExpr;

public class IntNode : ARationalNode
{
    public static Dictionary<int, IntNode> CacheIntNodes { get; set; } = new();
    public static implicit operator IntNode(int i)
    {
        return FromInt(i);
    }
    public Dictionary<int, int> GetPrimeFactors()
    {
        if (PrimeFactorsCache.ContainsKey(Value))
            return PrimeFactorsCache[Value];
        else
            return PrimeFactors(Value);
    }
    static Dictionary<int, Dictionary<int, int>> PrimeFactorsCache = new();
    public static Dictionary<int, int> PrimeFactors(int n)
    {
        var origin = n;
        if (n == 0) return new Dictionary<int, int>();
        Dictionary<int, int> factors = new Dictionary<int, int>();

        while (n % 2 == 0)
        {
            if (factors.ContainsKey(2))
                factors[2]++;
            else
                factors.Add(2, 1);
            n /= 2;
        }
        for (int i = 3; i <= Math.Sqrt(n); i += 2)
        {
            while (n % i == 0)
            {
                if (factors.ContainsKey(i))
                    factors[i]++;
                else
                    factors.Add(i, 1);
                n /= i;
            }
        }

        if (n > 2)
        {
            factors.Add(n, 1);
        }
        PrimeFactorsCache.Add(origin, factors);
        return factors;
    }

    public IntNode(int value)
    {
        Value = value;
    }
    public int Value { get; set; }
    public bool IsEven { get => Value % 2 == 0; }

    public override IntNode Clone()
    {
        return FromInt(Value);
    }
    public override ARationalNode Simplify()
    {
        return this;
    }
    public override string ToString()
    {
        return $"{Value}";
    }
    public override ZExpr Add(ZExpr r)
    {
        if (Value == 0)
        {
            return r.Clone();
        }
        if (r is IntNode i)
        {
            return Value + i.Value;
        }
        else if (r is RealSumNode sum)
        {
            var temp = (RealSumNode)sum.Clone();
            temp.Rational = (ARationalNode)temp.Rational.Add(this);
            var result = temp.Simplify();
            return result;
        }
        else if (r is FractionNode product)
        {
            var temp = (FractionNode)product.Clone();
            if (temp.IsPositive)
            {
                temp.Numerator = FromInt(temp.Numerator.Value + temp.Denominator.Value * Value);
            }
            else
            {
                temp.Numerator = FromInt(-temp.Numerator.Value + temp.Denominator.Value * Value);
            }
            var result = temp.Simplify();
            return result;
        }
        return base.Add(r);
    }
    public override ZExpr Sub(ZExpr r)
    {
        if (Value == 0)
        {
            return r.Opposite();
        }
        if (r is IntNode i)
        {
            return Value - i.Value;
        }
        else if (r is RealSumNode sum)
        {
            var temp = (RealSumNode)sum.Clone();
            temp.Rational = (ARationalNode)temp.Rational.Sub(this);
            var result = temp.Simplify();
            return result;
        }
        else if (r is FractionNode product)
        {
            var temp = (FractionNode)product.Clone();
            if (temp.IsPositive)
            {
                temp.Numerator = new IntNode(temp.Denominator.Value * Value - temp.Numerator.Value);
            }
            else
            {
                temp.Numerator = new IntNode(-temp.Numerator.Value - temp.Denominator.Value * Value);
            }
            var result = temp.Simplify();
            return result;
        }
        return base.Sub(r);
    }
    public override ZExpr Mul(ZExpr expr)
    {
        if (Value == 0) return 0;
        if (expr is IntNode i)
        {
            return Value * i.Value;
        }
        else if (expr is FractionNode product)
        {
            var temp = (FractionNode)product;
            temp.Numerator = FromInt(temp.Numerator.Value * this.Value);
            return temp;
        }
        else
        {
            return base.Mul(expr);
            var temp = new RealProductNode();
            temp.Rational = this;
            temp.Multipliers.Add((RealNode)expr.Clone());
            return temp;
        }
    }
    public override ZExpr Div(ZExpr expr)
    {
        if (Value == 0) return 0;
        if (expr is IntNode i)
        {
            if (Value % i.Value == 0)
            {
                return Value / i.Value;
            }
            else
            {
                FractionNode temp = new FractionNode();
                temp.Numerator = this;
                temp.Denominator = i;
                var result = temp.Simplify();
                return result;
            }
        }
        else if (expr is FractionNode frac)
        {
            var n = Value * frac.Denominator.Value;
            var m = frac.Numerator.Value;
            var gcd = FindGCD(n, m);
            n = n / gcd;
            m = m / gcd;
            if (m == 1)
            {
                return n;
            }
            else
            {
                FractionNode temp = new FractionNode();
                temp.Numerator = n;
                temp.Denominator = m;
                return temp;
            }
        }
        else if (expr is RealProductNode product)
        {
            var temp = product.Invert();
            if (temp is RealProductNode p)
            {
                p.Rational = (ARationalNode)p.Rational.Mul(this);
                var result = temp.Simplify();
                return result;
            }
            else
            {
                return Value * temp;
            }

        }
        else
        {
            return base.Div(expr);
            var temp = new RealProductNode();
            temp.Rational = this;
            temp.Divisors.Add((RealNode)expr.Clone());
            return temp;
        }
    }
    public override RealNode Pow(ZExpr e)
    {
        if (e is IntNode i)
        {
            if (i.Value == 0) { return FromInt(1); }
            else if (i.Value > 0)
            {
                return FromInt((int)Math.Pow(Value, i.Value));
            }
            else
            {
                FractionNode product = new FractionNode();
                product.Numerator = FromInt(1);
                product.Denominator = FromInt((int)Math.Pow(Value, -i.Value));
                return product;
            }
        }
        return base.Pow(e);
    }

    public override IntNode Opposite()
    {
        return FromInt(-Value);
    }
    public override ARationalNode Invert()
    {
        if (Value > 0)
        {
            FractionNode realProductNode = new FractionNode();
            realProductNode.Denominator = FromInt(Value);
            return realProductNode;
        }
        else if (Value < 0)
        {
            FractionNode realProductNode = new FractionNode();
            realProductNode.IsPositive = false;
            realProductNode.Denominator = FromInt(-Value);
            return realProductNode;
        }
        else
        {
            throw new ArgumentException("CannotDivideByZero");
        }
    }

    public override double GetApproximation()
    {
        return Value;
    }
}
