

namespace GeoInference.Knowledges;

public class ExprVector
{
    public Expr X { get; }
    public Expr Y { get; }
    public Expr Z { get; }

    public ExprVector(Expr x, Expr y, Expr z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public static Expr Dot(ExprVector ev1, ExprVector ev2) => ev1.Dot(ev2);
    public static ExprVector CrossProduct(ExprVector ev1, ExprVector ev2) => ev1.CrossProduct(ev2);
    public Expr this[int index]
    {
        get
        {
            if (index == 0) return X;
            else if (index == 1) return Y;
            else if (index == 2) return Z;
            else return null;
        }
    }

    public Expr Length
    {
        get
        {
            return (X.Pow(2) + Y.Pow(2) + Z.Pow(2)).Sqrt().Simplify();
        }
    }
    public static ExprVector operator -(ExprVector ev) => ev.Opposite();
    public static ExprVector operator +(ExprVector ev1, ExprVector ev2) => ev1.Add(ev2);
    public static ExprVector operator -(ExprVector ev1, ExprVector ev2) => ev1.Sub(ev2);
    public ExprVector Opposite()
    {
        return new ExprVector(X.Opposite(), Y.Opposite(), Z.Opposite());
    }
    public ExprVector Add(ExprVector ev)
    {
        return new ExprVector(X + ev.X, Y + ev.Y, Z + ev.Z);
    }
    public ExprVector Sub(ExprVector ev)
    {
        return new ExprVector(X - ev.X, Y - ev.Y, Z - ev.Z);
    }

    public Expr Dot(ExprVector ev2)
    {
        return (X * ev2.X + Y * ev2.Y + Z * ev2.Z);
    }
    public ExprVector CrossProduct(ExprVector ev2)
    {
        Expr x1 = X, y1 = Y, z1 = Z;
        Expr x2 = ev2.X, y2 = ev2.Y, z2 = ev2.Z;
        return new ExprVector(y1 * z2 - y2 * z1, z1 * x2 - z2 * x1, x1 * y2 - x2 * y1);
    }

    public bool IsParallel(ExprVector ev2)
    {
        Expr ratio = 1;
        if (X == 0 && ev2.X == 0)
        {
            if (Y == 0 && ev2.Y == 0)
            {
                return true;
            }
            else
            {
                ratio = Y / ev2.Y;
                if (Z == 0 && ev2.Z == 0)
                {
                    return true;
                }
                else
                {
                    return (ratio == Z / ev2.Z);
                }
            }
        }
        else
        {
            ratio = X / ev2.X;
            if (Y == 0 && ev2.Y == 0)
            {
                if (Z == 0 && ev2.Z == 0)
                {
                    return true;
                }
                else
                {
                    return (ratio == Z / ev2.Z);
                }
            }
            else
            {
                bool temp = (ratio == Y / ev2.Y);
                if (temp)
                {
                    if (Z == 0 && ev2.Z == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return (ratio == Z / ev2.Z);
                    }
                }
                else
                {
                    return false;
                }
            }
        }

    }

    public bool IsPerpendicular(ExprVector ev2)
    {
        var result = Dot(this, ev2);
        return result == 0;
    }

    public override string ToString()
    {
        return $"({X},{Y},{Z})";
    }

}
