
public class EqualityChain
{
    public const string Value = "value";

    public static int curIndex;
    public int index;
    public EqualityChain()
    {
        index = curIndex++;
    }
    public int lastLength;
    public bool noAuctual { get; set; } = true;
    public QuantityClassifications Unit { get => CoffDict.First().Key.Unit; }
    public Expr ActualValue { get; set; } = null;
    public List<(string node1, string node2, Knowledge reason)> Reasons { get; set; } = [];
    public ZDict<string, ZDict<string, Knowledge>> ReasonGraph { get; set; } = [];
    public Dictionary<Quantity, Expr> CoffDict { get; set; } = [];
    #region Expand
    public void AddValue(QuantityValue quantityValue)
    {
        if (ActualValue is null)
        {
            if (!CoffDict.ContainsKey(quantityValue.Quantity))
            {
                CoffDict.Add(quantityValue.Quantity, 1);
                ActualValue = quantityValue.Expr * CoffDict[quantityValue.Quantity];
            }
            else
            {
                ActualValue = quantityValue.Expr * CoffDict[quantityValue.Quantity];
            }
            SetReason(quantityValue.Quantity.ToString(), "value", quantityValue);
        }
        else
        {
            if (!CoffDict.ContainsKey(quantityValue.Quantity))
            {
                CoffDict.Add(quantityValue.Quantity, ActualValue / quantityValue.Expr);
                SetReason(quantityValue.Quantity.ToString(), "value", quantityValue);
            }
        }
    }
    public void AddRatio(Quantity thisBridge, Quantity newQuantity, Expr ratio, Knowledge reason)
    {
       

        CoffDict.Add(newQuantity, (ratio * CoffDict[thisBridge]).Simplify());
        SetReason(thisBridge.ToString(), newQuantity.ToString(), reason);
    }
    public void CombineByValue(EqualityChain continuedEquality)
    {
        var actualValueRatio = ActualValue / continuedEquality.ActualValue;
        foreach (var kv in continuedEquality.CoffDict.ToList())
        {
            if (CoffDict.ContainsKey(kv.Key)) { continue; }
            var ratio = ActualValue / continuedEquality.ActualValue * continuedEquality.CoffDict[kv.Key];
            CoffDict.Add(kv.Key, ratio);
        }
        foreach (var qreason in continuedEquality.Reasons)
        {
            SetReason(qreason.node1, qreason.node2, qreason.reason);
        }
    }
    public void CombineByRatio(EqualityChain continuedEquality, Quantity thisBridge, Quantity absorbedBridge, Expr ratio, QuantityRatio reason)
    {
        if (!CoffDict.ContainsKey(absorbedBridge))
        {
            CoffDict.Add(absorbedBridge, ratio * CoffDict[thisBridge]);
        }
        foreach (var kv in continuedEquality.CoffDict.ToList())
        {
            var innerRatio = CoffDict[absorbedBridge] / continuedEquality.CoffDict[absorbedBridge] * continuedEquality.CoffDict[kv.Key];
            if (CoffDict.ContainsKey(kv.Key)) { continue; }
            CoffDict.Add(kv.Key, innerRatio);
        }
        SetReason(thisBridge.ToString(), absorbedBridge.ToString(), reason);
        foreach (var qreason in continuedEquality.Reasons)
        {
            SetReason(qreason.node1, qreason.node2, qreason.reason);
        }
        if (ActualValue is null && continuedEquality.ActualValue is not null)
        {
            ActualValue = continuedEquality.ActualValue / continuedEquality.CoffDict[absorbedBridge] * CoffDict[absorbedBridge];
        }
    }
    #endregion
    #region Inference
    public IEnumerable<Knowledge> Inference()
    {
        if (CoffDict.First().Key.Unit == QuantityClassifications.Sin) return [];
        List<Knowledge> result = [];
        if (ActualValue is not null)
        {
            foreach (var item in CoffDict)
            {
                Quantity quantity = item.Key;
                QuantityValue pred = new QuantityValue(quantity, ActualValue / item.Value);
                var c = FindShortestPath(quantity.ToString(), "value");
                if (GeoInferenceApp.IsZhOrEn)
                    pred.AddReason("连等式发现");
                else
                    pred.AddReason("DiscoverdByEqualityChain");
                pred.AddCondition(c);
                SetReason(quantity, "value", pred);
                result.Add(pred);
            }
        }
        foreach (var q1 in CoffDict.Take(lastLength))
        {
            foreach (var q2 in CoffDict.Skip(lastLength))
            {
                QuantityRatio pred = new(q1.Key, q2.Key, q2.Value / q1.Value);
                var c = FindShortestPath(q1.Key.ToString(), q2.Key.ToString());
                pred.AddReason();
                pred.AddCondition(c);
                SetReason(q1.Key, q2, pred);
                result.Add(pred);
                if (!noAuctual)
                {
                    QuantityValue pred2 = new QuantityValue(q1.Key, ActualValue / CoffDict[q1.Key]);
                    var c2 = FindShortestPath(q1.Key.ToString(), "value");
                    if (GeoInferenceApp.IsZhOrEn)
                        pred2.AddReason("连等式发现");
                    else
                        pred2.AddReason("DiscoverdByEqualityChain");
                    pred2.AddCondition(c2);
                    SetReason(q1.Key, "value", pred2);
                    result.Add(pred2);
                }
            }
        }
        var newQuantitys = CoffDict.Skip(lastLength).ToList();
        for (int i = 0; i < newQuantitys.Count(); i++)
        {
            for (int j = i + 1; j < newQuantitys.Count(); j++)
            {
                var q1 = newQuantitys[i];
                var q2 = newQuantitys[j];
                QuantityRatio pred = new(q1.Key, q2.Key, q2.Value / q1.Value);
                var c = FindShortestPath(q1.Key.ToString(), q2.Key.ToString());
                if (GeoInferenceApp.IsZhOrEn)
                    pred.AddReason("连等式发现");
                else
                    pred.AddReason("DiscoverdByEqualityChain");
                pred.AddCondition(c);
                SetReason(q1.Key, q2, pred);
                result.Add(pred);
                if (!noAuctual)
                {
                    QuantityValue pred2 = new QuantityValue(q1.Key, ActualValue / CoffDict[q1.Key]);
                    var c2 = FindShortestPath(q1.Key.ToString(), "value");
                    if (GeoInferenceApp.IsZhOrEn)
                        pred2.AddReason("连等式发现");
                    else
                        pred2.AddReason("DiscoverdByEqualityChain");
                    pred2.AddCondition(c2);
                    SetReason(q1.Key, "value", pred2);
                    result.Add(pred2);
                }
            }
        }
        lastLength = CoffDict.Count;
        return result;
    }
    public void SetReason(object node1, object node2, Knowledge reason)
    {
        if (node1.ToString().GetHashCode() > node2.ToString().GetHashCode())
        {
            var tmp = node1;
            node1 = node2;
            node2 = tmp;
        }
        
        for (int i = 0; i < Reasons.Count; i++)
        {
            var item = Reasons[i];
            if (item.Item1 == node1.ToString() && item.Item2 == node2.ToString())
            {
                if (item.reason.Level > reason.Level)
                {
                    Reasons[i] = (item.node1, item.node2, reason);
                    ReasonGraph[node1.ToString()][node2.ToString()] = reason;
                    ReasonGraph[node2.ToString()][node1.ToString()] = reason;
                }
                return;
            }
        }
        Reasons.Add((node1.ToString(), node2.ToString(), reason));
        ReasonGraph[node1.ToString()][node2.ToString()] = reason;
        ReasonGraph[node2.ToString()][node1.ToString()] = reason;
    }
    public List<Knowledge> FindShortestPath(string start, string end)
    {
        if (!ReasonGraph.ContainsKey(start) || string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
            return new List<Knowledge>();
        if (start == end)
            return new List<Knowledge>();

        var visited = new HashSet<string>();
        var queue = new Queue<string>();
        var previousNodes = new Dictionary<string, string>();
        var previousEdges = new Dictionary<string, Knowledge>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            string currentNode = queue.Dequeue();
            if (currentNode == end)
                break;

            if (ReasonGraph.TryGetValue(currentNode, out var neighbors))
            {
                foreach (var (neighbor, edge) in neighbors)
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        previousNodes[neighbor] = currentNode;
                        previousEdges[neighbor] = edge;
                        if (neighbor == end)
                            break;
                    }
                }
            }
        }

        return ReconstructPath(end, previousNodes, previousEdges);
    }
    private List<Knowledge> ReconstructPath(
        string end,
        Dictionary<string, string> previousNodes,
        Dictionary<string, Knowledge> previousEdges)
    {
        var path = new List<Knowledge>();
        string current = end;
        while (previousNodes.ContainsKey(current))
        {
            path.Add(previousEdges[current]);
            current = previousNodes[current];
        }
        path.Reverse();
        return path;
    }
    #endregion
    #region Use
    public Expr ToValue(Quantity quantity)
    {
        return ActualValue / CoffDict[quantity];
    }
    public Expr To(Quantity toQuantity, Quantity fromQuantity)
    {
        return toQuantity * CoffDict[toQuantity] / CoffDict[fromQuantity];
    }
    public Knowledge GetValueReason(Quantity quantity)
    {
        string f = quantity.ToString(), s = "value";
        if (quantity.ToString().GetHashCode() > "value".GetHashCode())
        {
            f = "value";
            s = quantity.ToString();
        }
        foreach (var item in Reasons)
        {
            if (item.node1 == f && item.node2 == s || item.node1 == s && item.node2 == f)
            {
                return item.reason;
            }
        }
        QuantityValue pred = new QuantityValue(quantity, ActualValue / CoffDict[quantity]);
        Reasons.Add((quantity.ToString(), "value".ToString(), pred));
        return pred;
    }
    public Knowledge GetRatioReason(Quantity q1, Quantity q2)
    {
        string f = q1.ToString(), s = q2.ToString();
        if (q1.ToString().GetHashCode() > q2.ToString().GetHashCode())
        {
            f = q2.ToString();
            s = q1.ToString();
        }
        foreach (var item in Reasons)
        {
            if (item.Item1 == f && item.Item2 == s)
            {
                return item.reason;
            }
            else if (item.Item1 == s && item.Item2 == f)
            {
                return item.reason;
            }
        }
        QuantityRatio ratio = new QuantityRatio(q1, q2, CoffDict[q2] / CoffDict[q1]);
        Reasons.Add((q1.ToString(), q2.ToString(), ratio));
        return ratio;
    }
    #endregion
    public override string ToString()
    {
        string str = "(";
        if (ActualValue is not null)
        {
            str += $"{ActualValue}=";
        }
        foreach (var kv in CoffDict)
        {
            str += $"{kv.Value}*{kv.Key}=";
        }
        str = str.Remove(str.Length - 1);

        str += $")";
        return str;
    }
}
