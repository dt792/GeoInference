
using GeoInference.Definitions.Knowledges;
namespace GeoInference.Knowledges;

public abstract class Predicate : Knowledge
{
   
    public Predicate this[int index] => Properties[index];
    public List<Predicate> Properties { get; set; } = new List<Predicate>();
    public Expr Expr { get; set; }
    #region 
    protected void Add(params Predicate[] preds)
    {
        foreach (var pred in preds)
        {
            if (pred is null)
                throw new ArgumentNullException();
            Properties.Add(pred);
            if (Level <= pred.Level)
            {
                Level = pred.Level + 1;
            }
        }
    }
    protected void Add(Expr expr)
    {
        if (expr.Value == "undefined")
            IsAvailable = false;
        Expr = expr;
    }
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
        for (int k = 0; k < Properties.Count; ++k)
        {
            if (k == 0)
                HashCode |= Properties[k].PosIndex;
            else if (k < 6)
                HashCode |= (ulong)Properties[k].PosIndex << k * 9;
            else
                HashCode ^= (ulong)Properties[k].PosIndex << ((k - 5) * 7);
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
        var newPred = (Predicate)MemberwiseClone();
        newPred.Properties = new List<Predicate>(Properties);
        newPred.Conditions = new List<Knowledge>();
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
    /// <summary>
    /// </summary>
    public void NormalizeForPairedQuadrilateral()
    {
        int size = this.Properties.Count;
        int num = size / 2;
        int flag = 0;
        Point point = (Point)Properties[0];
        Point[] pointPreds = new Point[size];
        Properties.CopyTo(pointPreds);
        for (int i = 1; i < num; i++)
        {
            if (Properties[i].PosIndex < point.PosIndex)
            {
                flag = i;
                point = (Point)Properties[i];
            }
        }
        for (int i = 0; i < num; i++)
        {
            int z = (i - flag + num) % num;
            Properties[z] = pointPreds[i];
            Properties[z + num] = pointPreds[i + num];
        }
        if (Properties[1].PosIndex > Properties[num - 1].PosIndex)
        {
            for (int i = 1; i <= num / 2; i++)
            {
                Point temp = (Point)Properties[i];
                Properties[i] = Properties[num - i];
                Properties[num - i] = temp;
                Point temp1 = (Point)Properties[i + num];
                Properties[i + num] = Properties[2 * num - i];
                Properties[2 * num - i] = temp1;
            }
        }
    }
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
