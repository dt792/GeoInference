namespace GeoInference.Definitions.Algebras.ZExpr;

public class FractionNode : ARationalNode
{
    public bool IsPositive { get; set; } = true;
    public IntNode Numerator { get; set; } = FromInt(1);
    public IntNode Denominator { get; set; } = FromInt(1);
    public override string ToString()
    {
        if (IsPositive)
            return $"{Numerator}/{Denominator}";
        else
            return $"-{Numerator}/{Denominator}";
    }
    public override FractionNode Clone()
    {
        FractionNode rationalNode = new FractionNode();
        rationalNode.IsPositive = IsPositive;
        rationalNode.Numerator = Numerator;
        rationalNode.Denominator = Denominator;
        return rationalNode;
    }

    public override ARationalNode Simplify()
    {
        if (Numerator.Value < 0) { IsPositive = !IsPositive; Numerator = (IntNode)(-Numerator.Clone()); }
        if (Denominator.Value < 0) { IsPositive = !IsPositive; Denominator = (IntNode)(-Denominator.Clone()); }

        if (Numerator.Value == 0) return Zero;
        if (Denominator.Value == 0) throw new ArgumentException();
        if (Numerator.Value == Denominator.Value)
        {
            if (IsPositive) return One;
            else return NegativeOne;
        }
        if (Denominator.Value == 1)
        {
            if (IsPositive) return Numerator;
            else return (Numerator.Opposite());
        }

        var a = FindGCD(Numerator.Value, Denominator.Value);
        if (a > 1)
        {
            Numerator = FromInt(Numerator.Value / a);
            Denominator = FromInt(Denominator.Value / a);
        }

        return this;
    }

    public override ZExpr Add(ZExpr r)
    {
        if (r is IntNode i)
        {
            FractionNode result = new FractionNode();
            if (IsPositive)
            {
                result.Numerator = FromInt(Numerator.Value + Denominator.Value * i.Value);
                result.Denominator = Denominator;
            }
            else
            {
                result.Numerator = -Numerator.Value + Numerator.Value * i.Value;
                result.Denominator = Denominator;
            }
            return result.Simplify();
        }
        else if (r is FractionNode rational)
        {
            var temp = (FractionNode)this.Clone();
            temp.Numerator = FromInt(temp.Numerator.Value * rational.Denominator.Value + temp.Denominator.Value * rational.Numerator.Value);
            temp.Denominator = FromInt(temp.Denominator.Value * rational.Denominator.Value);
            var result = temp.Simplify();
            return result;
        }
        return base.Add(r);
    }

    public override ZExpr Sub(ZExpr r)
    {
        if (r is IntNode i)
        {
            FractionNode result = new FractionNode();
            if (IsPositive)
            {
                result.Numerator = FromInt(Numerator.Value - Denominator.Value * i.Value);
                result.Denominator = Denominator;
            }
            else
            {
                result.Numerator = -Numerator.Value - Numerator.Value * i.Value;
                result.Denominator = Denominator;
            }
            return result.Simplify();
        }
        else if (r is FractionNode rational)
        {
            var temp = (FractionNode)this.Clone();
            temp.Numerator = FromInt(temp.Numerator.Value * rational.Denominator.Value - temp.Denominator.Value * rational.Numerator.Value);
            temp.Denominator = FromInt(temp.Denominator.Value * rational.Denominator.Value);
            var result = temp.Simplify();
            return result;
        }
        return base.Sub(r);
    }

    public override ZExpr Mul(ZExpr r)
    {
        if (r is IntNode i)
        {
            if (i.Value == 0)
            {
                return 0;
            }

            FractionNode temp = new FractionNode();
            if (i.IsEven)
            {
                temp.IsPositive = true;
            }
            else
            {
                temp.IsPositive = IsPositive;
            }

            temp.Numerator = Numerator.Value * i.Value;
            temp.Denominator = Denominator;
            var result = temp.Simplify();
            return result;
        }
        else if (r is FractionNode rational)
        {
            var temp = (FractionNode)this.Clone();
            temp.Numerator = temp.Numerator.Value * rational.Numerator.Value;
            temp.Denominator = temp.Denominator.Value * rational.Denominator.Value;
            var result = temp.Simplify();
            return result;
        }
        else
        {
            var temp = new RealProductNode();
            temp.Rational = this;
            temp.Multipliers.Add((RealNode)r.Clone());
            return temp;
        }
    }

    public override ZExpr Div(ZExpr r)
    {
        if (r is IntNode i)
        {
            FractionNode result = new FractionNode();
            result.IsPositive = IsPositive;
            result.Numerator = Numerator.Value;
            result.Denominator = Denominator.Value * i.Value;
            return result.Simplify();
        }
        else if (r is FractionNode rational)
        {
            var temp = (FractionNode)this.Clone();
            temp.Numerator = temp.Numerator.Value * rational.Denominator.Value;
            temp.Denominator = temp.Denominator.Value * rational.Numerator.Value;
            var result = temp.Simplify();
            return result;
        }
        else
        {
            var temp = new RealProductNode();
            temp.Rational = this;
            temp.Multipliers.Add((RealNode)r.Clone());
            return temp;
        }
    }
    public override RealNode Pow(ZExpr r)
    {
        if (r is IntNode i)
        {
            FractionNode result = new FractionNode();
            if (i.Value == 0)
            {
                return FromInt(1);
            }
            if (i.Value > 0)
            {
                if (i.IsEven)
                    result.IsPositive = true;
                else
                    result.IsPositive = IsPositive;

                result.Numerator = (int)Math.Pow(Numerator.Value, i.Value);
                result.Denominator = (int)Math.Pow(Denominator.Value, i.Value);
                return (RealNode)result.Simplify();
            }
            if (i.Value < 0)
            {
                if (i.IsEven)
                    result.IsPositive = true;
                else
                    result.IsPositive = IsPositive;

                result.Numerator = (int)Math.Pow(Denominator.Value, -i.Value);
                result.Denominator = (int)Math.Pow(Numerator.Value, -i.Value);
                return (RealNode)result.Simplify();
            }
        }
        return base.Pow(r);
    }
    public override FractionNode Opposite()
    {
        var temp = this.Clone();
        temp.IsPositive = !temp.IsPositive;
        return temp;
    }
    public override ARationalNode Invert()
    {
        var temp = new FractionNode();
        temp.IsPositive = IsPositive;
        temp.Numerator = this.Denominator.Clone();
        temp.Denominator = this.Numerator.Clone();
        var result = temp.Simplify();
        return result;
    }
    public override double GetApproximation()
    {
        if (IsPositive)
            return this.Numerator.GetApproximation() / this.Denominator.GetApproximation();
        else
            return -(this.Numerator.GetApproximation() / this.Denominator.GetApproximation());
    }
}
