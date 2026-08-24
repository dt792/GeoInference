
using GeoInference.Definitions.Knowledges;
using System.Runtime.CompilerServices;
namespace GeoInference.Knowledges;

public abstract class Predicate : Knowledge
{
   
    public Predicate this[int index] => Properties[index];
    public List<Predicate> Properties { get; set; } = new List<Predicate>();
    public Expr Expr { get; set; }
    #region 
    protected void Add(params object[] objs)
    {
        foreach (var obj in objs)
        {
            if (obj is null)
                throw new ArgumentNullException();
            else if (obj is Predicate pred)
            {
                Properties.Add(pred);
                if (Level <= pred.Level)
                {
                    Level = pred.Level + 1;
                }
            }

            else if (obj is Expr expr)
            {
                if (expr.Value == "undefined")
                    IsAvailable = false;
                Expr = expr;
            }
            else
                throw new ArgumentException();
        }
    }
    public void SetNameHashCode(string name)
    {
        HashCode = ClassIndexDict[GetType().FullName] << 54 | (uint)name.GetHashCode();
    }
    public override void SetHashCode()
    {
        HashCode = ClassIndexDict[GetType().FullName] << 54;
        for (int k = 0; k < Properties.Count && k < 9; ++k)
        {
            if (k == 0)
                HashCode |= Properties[k].PosIndex;
            else
                HashCode |= (ulong)Properties[k].PosIndex << k * 6;
        }
    }
    #endregion

    #region Tools
    public bool Contains(Predicate knowledge)
    {
        return Properties.Contains(knowledge);
    }
    public virtual Predicate Clone()
    {
        var newPred = (Predicate)RuntimeHelpers.GetUninitializedObject(GetType());
        newPred.Properties = new List<Predicate>(Properties);
        newPred.Conditions = new List<Knowledge>();
        newPred.Expr = Expr;
        newPred.IsAvailable = IsAvailable;
        newPred.HashCode = HashCode;
        return newPred;
    }
    public override string ToString()
    {
        throw new Exception("(￣、￣)");
    }

    public static bool operator ==(Predicate k1, Predicate k2)
    {
        return k1.HashCode == k2.HashCode;
    }
    public static bool operator !=(Predicate k1, Predicate k2)
    {
        return k1.HashCode != k2.HashCode;
    }
    #endregion

    #region Normalizations
    public void NormalizeForPrism()
    {
        int size = this.Properties.Count;
        int halfsize = size / 2;
        int minindex = 0;
        uint minpos = 100;
        Point[] pointPreds = new Point[size];
        Properties.CopyTo(pointPreds);

        List<int> firstT = pointPreds.Take(3).Select(p => (int)p.PosIndex).Order().ToList();
        List<int> lastT = pointPreds.Skip(3).Take(3).Select(p => (int)p.PosIndex).Order().ToList();
        bool needtod = false;
        for (int i = 0; i < halfsize; i++)
        {
            if (firstT[i] == lastT[i])
            {

            }
            else
            {
                needtod = firstT[i] > lastT[i];
                break;
            }
        }
        if (needtod)
        {
            for (int i = 0; i < halfsize; i++)
            {
                var tmp = pointPreds[i];
                pointPreds[i] = pointPreds[i + halfsize];
                pointPreds[i + halfsize] = tmp;
            }
        }

        for (int i = 0; i < size; i++)
        {
            if (pointPreds[i].PosIndex < minpos)
            {
                minindex = i;
                minpos = pointPreds[i].PosIndex;
            }
        }
        var clock = pointPreds[(minindex + 1) % halfsize].PosIndex > pointPreds[(minindex + halfsize - 1) % halfsize].PosIndex;
        if (clock)
        {
            for (int i = 0; i < halfsize / 2; i++)
            {
                Point temp = (Point)pointPreds[i];
                pointPreds[i] = pointPreds[halfsize - i - 1];
                pointPreds[halfsize - i - 1] = temp;

                temp = (Point)pointPreds[i + halfsize];
                pointPreds[i + halfsize] = pointPreds[halfsize - i - 1 + halfsize];
                pointPreds[halfsize - i - 1 + halfsize] = temp;
            }
        }
        minpos = 100;
        minindex = 0;
        for (int i = 0; i < size; i++)
        {
            if (pointPreds[i].PosIndex < minpos)
            {
                minindex = i;
                minpos = pointPreds[i].PosIndex;
            }
        }
        Point[] pointPreds2 = new Point[size];
        for (int i = 0; i < halfsize; i++)
        {
            Properties[(i + halfsize - minindex) % halfsize] = pointPreds[i];
            Properties[(i + halfsize - minindex) % halfsize + halfsize] = pointPreds[i + halfsize];
        }
    }
    public abstract void Normalize();
    public void Sort()
    {
        Properties.Sort(new Comparison<Predicate>((pred1, pred2) =>
        {
            if (pred1.PosIndex == pred2.PosIndex)
                return 0;
            else if (pred1.PosIndex > pred2.PosIndex)
                return 1;
            else
                return -1;
        }));
    }
    public void Sort(int pos1, int pos2)
    {
        if (Properties[pos1].PosIndex > Properties[pos2].PosIndex)
        {
            var temp = Properties[pos1];
            Properties[pos1] = Properties[pos2];
            Properties[pos2] = temp;
        }
    }
    public void Sort(params int[] postions)
    {
        List<Predicate> knowledgesForSort = new List<Predicate>();
        foreach (var pos in postions)
        {
            knowledgesForSort.Add(Properties[pos]);
        }
        knowledgesForSort.Sort(new Comparison<Predicate>((pred1, pred2) =>
        {
            if (pred1.PosIndex == pred2.PosIndex)
                return 0;
            else if (pred1.PosIndex > pred2.PosIndex)
                return 1;
            else
                return -1;
        }));
        int index = 0;
        foreach (var pos in postions)
        {
            Properties[pos] = knowledgesForSort[index];
            index++;
        }
    }
    #endregion
}
