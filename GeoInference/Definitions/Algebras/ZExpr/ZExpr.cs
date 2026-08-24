namespace GeoInference.Definitions.Algebras.ZExpr;

/// <summary>
///
/// </summary>
public abstract class ZExpr
{
    public Expr ToExpr() { return new Expr(ToString()); }
    #region
    public static Func<PiNode> FromPi { get; set; } = () => new PiNode() { };

    public static Func<int, IntNode> FromInt { get; set; } = (i) =>
    {
        if (IntNode.CacheIntNodes.ContainsKey(i))
        {
            return IntNode.CacheIntNodes[i];
        }
        else
        {
            IntNode iNode = new IntNode(i);
            IntNode.CacheIntNodes.Add(i, iNode);
            return iNode;
        }
    };

    public static Func<Quantity, QuantityNode> FromMut { get; set; } = (mut) =>
    {
        if (QuantityNode.CacheMutNodes.ContainsKey(mut))
        {
            return QuantityNode.CacheMutNodes[mut];
        }
        else
        {
            QuantityNode mutNode = new QuantityNode(mut);
            QuantityNode.CacheMutNodes.Add(mut, mutNode);
            return mutNode;
        }
    };
    public static implicit operator ZExpr(int i)
    {
        return FromInt(i);
    }

    public static implicit operator ZExpr(GeoQuantity mut)
    {
        return FromMut(mut);
    }

    public static IntNode Let(int i)
    {
        return FromInt(i);
    }


    #endregion

    #region 
    public static PiNode Pi { get => FromPi(); }
    public static ARationalNode Zero { get => FromInt(0); }

    public static ARationalNode AHalf
    { get { return (ARationalNode)(FromInt(1) / FromInt(2)); } }

    public static IntNode One { get => FromInt(1); }
    public static IntNode Two { get => FromInt(2); }
    public static IntNode Three { get => FromInt(3); }
    public static IntNode NegativeOne { get => FromInt(-1); }
    #endregion

    public bool IsSimplified { get; set; } = false;

    public abstract ZExpr Simplify();

    public abstract ZExpr Clone();

    #region 

    /// <summary>
    
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public virtual ZExpr Add(ZExpr expr)
    {
        if (expr == Zero) return this.Clone();
        ZExpr result = null;
        if (this is RealNode realNode)
        {
            if (expr is RealNode real)
            {
                throw new Exception();
            }
            else if (expr is QuantityNode or TrigonometricNode or ProductNode or PowerNode)
            {
                var temp = new SumNode();
                temp.Constant = realNode.Clone();
                temp.AddToAddends(expr.Clone());
                return temp;
            }
            else if (expr is SumNode sum)
            {
                var temp = sum.Clone();
                temp.Constant = temp.Constant + realNode;
                return temp;
            }
        }
        else if (this is QuantityNode or TrigonometricNode or ProductNode or PowerNode)
        {
            if (this is ProductNode p1 && expr is ProductNode p2)
            {
                if (p1.IsSingle && p2.IsSingle)
                {
                    if (p1.Single == p2.Single)
                    {
                        ProductNode temp = new ProductNode();
                        temp.Constant = p1.Constant + p2.Constant;
                        temp.AddToMultipliers(p1.Single.Clone());
                        return temp;
                    }
                }
            }

            if (expr is RealNode real)
            {
                var temp = new SumNode();
                temp.Constant = real.Clone();
                temp.AddToAddends(expr.Clone());
                return temp;
            }
            else if (expr is QuantityNode or TrigonometricNode or ProductNode or PowerNode)
            {
                var temp = new SumNode();
                temp.AddToAddends(this.Clone());
                temp.AddToAddends(expr.Clone());
                return temp;
            }
            else if (expr is SumNode sum)
            {
                var temp = sum.Clone();
                temp.AddToAddends(expr.Clone());
                result = temp.Simplify();
                return result;
            }
        }
        else if (this is SumNode sumNode)
        {
            if (expr is RealNode real)
            {
                var temp = sumNode.Clone();
                temp.Constant = temp.Constant + real.Clone();
                return temp;
            }
            else if (expr is QuantityNode or TrigonometricNode or ProductNode or PowerNode)
            {
                var temp = sumNode.Clone();
                temp.AddToAddends(expr.Clone());
                result = temp.Simplify();
                return result;
            }
            else if (expr is SumNode sum)
            {
                var temp = sumNode.Clone();
                temp.Constant = temp.Constant + sum.Constant;
                foreach (var item in sum.Addends)
                {
                    temp.AddToAddends(item);
                }
                foreach (var item in sum.Subtrahends)
                {
                    temp.AddToSubtrahends(item);
                }
                result = temp.Simplify();
                return result;
            }
        }
        throw new Exception();
    }

    /// <summary>
    
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public virtual ZExpr Sub(ZExpr expr)
    {
        if (expr == Zero) return this.Clone();
        ZExpr result = null;
        if (this is RealNode realNode)
        {
            if (expr is RealNode real)
            {
                throw new Exception();
            }
            else if (expr is QuantityNode or TrigonometricNode or ProductNode or PowerNode)
            {
                var temp = new SumNode();
                temp.Constant = realNode.Clone();
                temp.AddToSubtrahends(expr.Clone());
                return temp;
            }
            else if (expr is SumNode sum)
            {
                var temp = sum.Clone().Opposite();
                temp.Constant = temp.Constant - realNode;
                return temp;
            }
        }
        else if (this is QuantityNode or TrigonometricNode or ProductNode or PowerNode)
        {
            if (this is ProductNode p1 && expr is ProductNode p2)
            {
                if (p1.IsSingle && p2.IsSingle)
                {
                    if (p1.Single == p2.Single)
                    {
                        ProductNode temp = new ProductNode();
                        temp.Constant = p1.Constant - p2.Constant;
                        temp.AddToMultipliers(p1.Single.Clone());
                        return temp;
                    }
                }
            }

            if (expr is RealNode real)
            {
                var temp = new SumNode();
                temp.Constant = real.Clone();
                temp.AddToSubtrahends(expr.Clone());
                return temp;
            }
            else if (expr is QuantityNode or TrigonometricNode or ProductNode or PowerNode)
            {
                var temp = new SumNode();
                temp.AddToAddends(this.Clone());
                temp.AddToSubtrahends(expr.Clone());
                return temp;
            }
            else if (expr is SumNode sum)
            {
                var temp = sum.Clone().Opposite();
                temp.AddToAddends(this.Clone());
                result = temp.Simplify();
                return result;
            }
        }
        else if (this is SumNode sumNode)
        {
            if (expr is RealNode real)
            {
                var temp = sumNode.Clone();
                temp.Constant = temp.Constant - real.Clone();
                return temp;
            }
            else if (expr is QuantityNode or TrigonometricNode or ProductNode or PowerNode)
            {
                var temp = sumNode.Clone();
                temp.AddToSubtrahends(expr.Clone());
                result = temp.Simplify();
                return result;
            }
            else if (expr is SumNode sum)
            {
                var temp = sumNode.Clone();
                temp.Constant = temp.Constant - sum.Constant;
                foreach (var item in sum.Addends)
                {
                    temp.AddToSubtrahends(item);
                }
                foreach (var item in sum.Subtrahends)
                {
                    temp.AddToAddends(item);
                }
                result = temp.Simplify();
                return result;
            }
        }
        throw new Exception();
    }

    /// <summary>
    
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public virtual ZExpr Mul(ZExpr expr)
    {
        if (expr == One) return this.Clone();
        if (expr == Zero) return Zero;
        ZExpr result = null;
        if (this is RealNode realNode)
        {

            if (expr is RealNode real)
            {
                throw new Exception();
            }
            else if (expr is QuantityNode or TrigonometricNode or SumNode or PowerNode)
            {
                var temp = new ProductNode();
                temp.Constant = realNode.Clone();
                temp.AddToMultipliers(expr.Clone());
                return temp;
            }
            else if (expr is ProductNode product)
            {
                var temp = product.Clone();
                temp.Constant = temp.Constant * this.Clone();
                temp.AddToMultipliers(product.Clone());
                return temp;
            }
        }
        else if (this is QuantityNode or TrigonometricNode or SumNode or PowerNode)
        {
            if (this is PowerNode p1 && expr is PowerNode p2)
            {
                if (p1.Base == p2.Base)
                {
                    PowerNode temp = new PowerNode();
                    temp.Exponent = p1.Exponent + p2.Exponent;
                    temp.Base = p1.Clone();
                    return temp;
                }
            }

            if (expr is RealNode real)
            {
                var temp = new ProductNode();
                temp.Constant = real.Clone();
                temp.AddToMultipliers(this);
                return temp;
            }
            else if (expr is QuantityNode or TrigonometricNode or SumNode or PowerNode)
            {
                var temp = new ProductNode();
                temp.AddToMultipliers(this.Clone());
                temp.AddToMultipliers(expr.Clone());
                return temp;
            }
            else if (expr is ProductNode product)
            {
                var temp = product.Clone();
                temp.AddToMultipliers(this.Clone());
                result = temp.Simplify();
                return result;
            }
        }
        else if (this is ProductNode productNode)
        {
            if (expr is RealNode real)
            {
                var temp = productNode.Clone();
                temp.Constant = temp.Constant * real.Clone();
                result = temp.Simplify();
                return result;
            }
            else if (expr is QuantityNode or TrigonometricNode or SumNode or PowerNode)
            {
                var temp = productNode.Clone();
                temp.AddToMultipliers(expr.Clone());
                result = temp.Simplify();
                return result;
            }
            else if (expr is ProductNode product)
            {
                var temp = productNode.Clone();
                foreach (var item in product.Multipliers)
                {
                    temp.AddToMultipliers(item);
                }
                foreach (var item in product.Divisors)
                {
                    temp.AddToDivisors(item);
                }
                result = temp.Simplify();
                return result;
            }
        }
        throw new Exception();
    }

    /// <summary>
    
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
    public virtual ZExpr Div(ZExpr expr)
    {
        if (expr == One) return this.Clone();
        if (expr == Zero) throw new Exception();
        ZExpr result = null;
        if (this is RealNode realNode)
        {
            if (expr is RealNode real)
            {
                throw new Exception();
            }
            else if (expr is QuantityNode or TrigonometricNode or SumNode or PowerNode)
            {
                var temp = new ProductNode();
                temp.Constant = realNode.Clone();
                temp.AddToDivisors(expr.Clone());
                return temp;
            }
            else if (expr is ProductNode product)
            {
                var temp = product.Invert().Clone();
                if (temp is ProductNode pTemp)
                {
                    pTemp.Constant = pTemp.Constant * this.Clone();
                    pTemp.AddToMultipliers(product.Clone());
                    return temp;
                }
                else
                {
                    return this * temp;
                }
            }
        }
        else if (this is QuantityNode or TrigonometricNode or SumNode or PowerNode)
        {
            if (this is PowerNode p1 && expr is PowerNode p2)
            {
                if (p1.Base == p2.Base)
                {
                    PowerNode temp = new PowerNode();
                    temp.Exponent = p1.Exponent - p2.Exponent;
                    temp.Base = p1.Clone();
                    return temp;
                }
            }

            if (expr is RealNode real)
            {
                var temp = new ProductNode();
                temp.Constant = real.Clone();
                temp.AddToDivisors(this);
                return temp;
            }
            else if (expr is QuantityNode or TrigonometricNode or SumNode or PowerNode)
            {
                var temp = new ProductNode();
                temp.AddToMultipliers(this.Clone());
                temp.AddToDivisors(expr.Clone());
                return temp;
            }
            else if (expr is ProductNode product)
            {
                var temp = new ProductNode();
                temp.IsPositive = product.IsPositive;
                temp.Constant = product.Constant.Invert();
                foreach (var item in product.Multipliers)
                {
                    temp.AddToDivisors(item);
                }
                foreach (var item in product.Divisors)
                {
                    temp.AddToMultipliers(item);
                }
                temp.AddToMultipliers(this.Clone());
                result = temp.Simplify();
                return result;
            }
        }
        else if (this is ProductNode productNode)
        {
            if (expr is RealNode real)
            {
                var temp = productNode.Clone();
                temp.Constant = temp.Constant / real.Clone();
                result = temp.Simplify();
                return result;
            }
            else if (expr is QuantityNode or TrigonometricNode or SumNode or PowerNode)
            {
                var temp = productNode.Clone();
                temp.AddToDivisors(expr.Clone());
                result = temp.Simplify();
                return result;
            }
            else if (expr is ProductNode product)
            {
                var temp = productNode.Clone();
                temp.Constant = productNode.Constant / product.Constant;
                foreach (var item in product.Multipliers)
                {
                    temp.AddToDivisors(item);
                }
                foreach (var item in product.Divisors)
                {
                    temp.AddToMultipliers(item);
                }
                result = temp.Simplify();
                return result;
            }
        }
        throw new Exception();
    }

    public virtual ZExpr Pow(ZExpr e)
    {
        if (this is RealNode realNode)
        {
            return realNode.Pow(e.Clone());
        }
        else if (this is PowerNode power)
        {
            power.Exponent = power.Exponent * e.Clone();
            return this;
        }
        else
        {
            PowerNode powerNode = new PowerNode();
            powerNode.Base = this.Clone();
            powerNode.Exponent = (ZExpr)e.Clone();
            return powerNode;
        }
    }

    #endregion Basic Operations

    #region 

    public virtual ZExpr Invert()
    {
        if (this is ProductNode product)
        {
            ProductNode productNode = new ProductNode();
            foreach (var item in product.Multipliers)
            {
                productNode.AddToDivisors(item.Clone());
            }
            foreach (var item in product.Divisors)
            {
                productNode.AddToMultipliers(item.Clone());
            }
            productNode.IsPositive = product.IsPositive;
            productNode.Constant = (RealNode)product.Constant.Invert();
            return productNode.Simplify();
        }
        else
        {
            ProductNode productNode = new ProductNode();
            productNode.Divisors.Add(this.Clone());
            return productNode.Simplify();
        }

    }

    public virtual ZExpr Opposite()
    {
        if (this is RealNode realNode)
        {
            return realNode.Opposite();
        }
        if (this is ProductNode p)
        {
            p = (ProductNode)p.Clone();
            p.IsPositive = !p.IsPositive;
            return p;
        }
        if (this is SumNode s)
        {
            s.Constant = s.Constant.Opposite();
            var temp = s.Addends;
            s.Addends = s.Subtrahends;
            s.Subtrahends = temp;
            return s;
        }
        else
        {
            var sumNode = new ProductNode();
            sumNode.Multipliers.Add(this);
            sumNode.IsPositive = !sumNode.IsPositive;
            return sumNode;
        }
    }

    public ZExpr Abs()
    {
        if (this is IntNode i)
        {
            var c = i.Clone();
            var a = Math.Abs(c.Value);
            return a;
        }
        else if (this is FractionNode r)
        {
            var c = r.Clone();
            c.IsPositive = true;
            return c;
        }
        else if (this is RealProductNode rp)
        {
            var c = rp.Clone();
            c.IsPositive = true;
            return c;
        }
        else if (this is ProductNode p)
        {
            var c = p.Clone();
            c.IsPositive = true;
            return c;
        }
        return this.Clone();
    }

    public virtual ZExpr Sqrt()
    {
        return Pow(AHalf);
    }

    public virtual ZExpr Mod(ZExpr x)
    {
        return int.Parse(ToString()) % int.Parse(x.ToString());
    }

    #endregion Common Functions

    #region 
    public ZExpr Sin()
    {
        return this;
    }
    public ZExpr Cos()
    {
        return this;
    }
    public ZExpr Tan()
    {
        return this;
    }
    public ZExpr ArcSin()
    {
        return this;
    }
    public ZExpr ArcCos()
    {
        return this;
    }
    public ZExpr ArcTan()
    {
        return this;
    }
    #endregion

    #region 

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public virtual new bool Equals(ZExpr obj)
    { return CompareTo(obj) == ExprCompareResult.Equal; }

    public virtual ExprCompareResult CompareTo(ZExpr expr)
    {
        if (ReferenceEquals(this, expr))
            return ExprCompareResult.Equal;

        if (this is RealNode i && expr is RealNode j)
        {
            int result = i.GetApproximation().CompareTo(j.GetApproximation());
            if (result == 0)
            {
                return ExprCompareResult.Equal;
            }
            else if (result == 1) { return ExprCompareResult.Greater; }
            else if (result == -1) { return ExprCompareResult.Less; }
        }
        return ExprCompareResult.Unknown;
    }

    #endregion Comparison

    #region 

    public static ZExpr operator +(ZExpr ZExpr1, ZExpr ZExpr2) => ZExpr1.Add(ZExpr2);

    public static ZExpr operator -(ZExpr ZExpr1, ZExpr ZExpr2) => ZExpr1.Sub(ZExpr2);

    public static ZExpr operator *(ZExpr ZExpr1, ZExpr ZExpr2) => ZExpr1.Mul(ZExpr2);

    public static ZExpr operator /(ZExpr ZExpr1, ZExpr ZExpr2) => ZExpr1.Div(ZExpr2);

    public static ZExpr operator -(ZExpr ZExpr) => ZExpr.Opposite();

    public static ZExpr operator %(ZExpr ZExpr1, ZExpr ZExpr2) => ZExpr1.Mod(ZExpr2);

    public static bool operator >(ZExpr ZExpr1, ZExpr ZExpr2) => ZExpr1.CompareTo(ZExpr2) == ExprCompareResult.Greater ? true : false;

    public static bool operator <(ZExpr ZExpr1, ZExpr ZExpr2) => ZExpr1.CompareTo(ZExpr2) == ExprCompareResult.Less ? true : false;

    public static bool operator ==(ZExpr ZExpr1, ZExpr ZExpr2) => ZExpr1.CompareTo(ZExpr2) == ExprCompareResult.Equal ? true : false;

    public static bool operator !=(ZExpr ZExpr1, ZExpr ZExpr2) => ZExpr1.CompareTo(ZExpr2) != ExprCompareResult.Equal ? true : false;

    #endregion Operator Overloading
    public bool StrEqual(string str)
    {
        return ToString() == str;
    }
}
