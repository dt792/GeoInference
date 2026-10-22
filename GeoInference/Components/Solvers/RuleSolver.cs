
using System.Reflection;

public class PermutationAttribute : Attribute { }
public class RuleSolver
{
    [DI] public KnowledgeBase knowledgeBase { get; set; }
    [DI] public KnowledgeBaseUpdater Updater { get; set; }
    public Queue<RulePredPair> Pairs { get; set; } = [];
    public ulong ActualRuleInstantiationCount { get; private set; } = 0;
    public bool HasNext { get => Pairs.Count > 0; }
    public RulePredPair Next() => Pairs.Dequeue();
    public void Solve(RulePredPair pair)
    {
        ActualRuleInstantiationCount++;
        var diagTs = SemiRuleDiagnostics.OnRuleInvokeStart(pair.RuleInfo);
        pair.RuleInfo.Invoker(pair.RuleInfo.RuleClass, pair.Args);
        SemiRuleDiagnostics.OnRuleInvokeEnd(pair.RuleInfo, diagTs);
    }
    static IEnumerable<Predicate> AvailableRange(List<Predicate> list, int start, int end)
    {
        for (int i = start; i < end; i++)
            if (list[i].IsAvailable)
                yield return list[i];
    }
    static IEnumerable<T> Concat2<T>(IEnumerable<T> first, IEnumerable<T> second)
    {
        foreach (var item in first) yield return item;
        foreach (var item in second) yield return item;
    }
    static Predicate[] ConcatArrays(Predicate[] a, Predicate[] b)
    {
        var result = new Predicate[a.Length + b.Length];
        Array.Copy(a, result, a.Length);
        Array.Copy(b, 0, result, a.Length, b.Length);
        return result;
    }
    static Predicate[] ConcatArrays(Predicate[] a, Predicate[] b, Predicate[] c)
    {
        var result = new Predicate[a.Length + b.Length + c.Length];
        Array.Copy(a, result, a.Length);
        Array.Copy(b, 0, result, a.Length, b.Length);
        Array.Copy(c, 0, result, a.Length + b.Length, c.Length);
        return result;
    }
    static Predicate[] ConcatArrays(Predicate[] a, Predicate[] b, Predicate[] c, Predicate[] d)
    {
        var result = new Predicate[a.Length + b.Length + c.Length + d.Length];
        Array.Copy(a, result, a.Length);
        Array.Copy(b, 0, result, a.Length, b.Length);
        Array.Copy(c, 0, result, a.Length + b.Length, c.Length);
        Array.Copy(d, 0, result, a.Length + b.Length + c.Length, d.Length);
        return result;
    }
    [DI] public ZSingletonDI DI { get; set; }
    public List<RuleInfo> RuleInfos { get; set; } = [];
    ZDict<string, RuleInfo> CachedRuleInfos { get; set; } = [];
    /// <summary>
    /// </summary>
    public List<List<int>> CaptureRuleState()
    {
        return RuleInfos.Select(r => new List<int>(r.LeftIndexs)).ToList();
    }
    /// <summary>
    /// </summary>
    public void RestoreRuleState(List<List<int>> state)
    {
        Pairs.Clear();
        for (int i = 0; i < RuleInfos.Count && i < state.Count; i++)
        {
            for (int j = 0; j < RuleInfos[i].LeftIndexs.Count && j < state[i].Count; j++)
            {
                RuleInfos[i].LeftIndexs[j] = state[i][j];
            }
        }
    }
    public virtual void LoadRules(IEnumerable<MethodInfo> methodInfos)
    {
        foreach (var ruleInfo in RuleInfos)
        {
            CachedRuleInfos[ruleInfo.Name] = ruleInfo;
        }
        RuleInfos.Clear();
        foreach (var method in methodInfos)
        {
            var type = method.DeclaringType;
            if (!DI.Getters.Exists(g => g.ActualType == type))
                DI.Set(type);
            var ruleClass = DI.Get(type);
            if (CachedRuleInfos.ContainsKey(method.Name))
            {
                RuleInfos.Add(CachedRuleInfos[method.Name]);
            }
            else
            {
                RuleInfo runningRule = new RuleInfo(method);
                runningRule.RuleClass = (RuleClass)ruleClass;
                RuleInfos.Add(runningRule);
            }
        }
    }
    public void MakeNew()
    {
        foreach (var ruleInfo in RuleInfos)
        {
            var attri = ruleInfo.RuleMethod.GetCustomAttribute<PermutationAttribute>();
            if (attri is null)
            {
                makePermutation(ruleInfo);
            }
            else
            {
                makeCombination(ruleInfo);
            }
        }
    }
    void makePermutation(RuleInfo ruleInfo)
    {
        List<int> OldPositions = (ruleInfo.LeftIndexs);
        if (!ruleInfo.LeftTypes.TrueForAll(type => knowledgeBase.Categories.ContainsKey(type))) return;
        List<int> NewPositions = ruleInfo.LeftTypes.Select(type => knowledgeBase.Categories[type].Count).ToList();
        List<List<Predicate>> KnowledgeList = ruleInfo.LeftTypes.Select(type => knowledgeBase.Categories[type]).ToList();

        switch (ruleInfo.LeftTypes.Count)
        {
            case 1:
                makePair1(); break;
            case 2:
                makePair2(); break;
            case 3:
                makePair3(); break;
            case 4:
                makePair4(); break;
            case 5:
                makePair5(); break;
            case 6:
                makePair6(); break;
            default:
                throw new NotImplementedException("Too many rule parameters.");
        }

        void makePair1()
        {
            var newKnowledges1 = AvailableRange(KnowledgeList[0], OldPositions[0], NewPositions[0]);
            foreach (var k1 in newKnowledges1)
            {
                Pairs.Enqueue(new(ruleInfo, [k1]));
            }
            ruleInfo.LeftIndexs[0] = NewPositions[0];
        }
        void makePair2()
        {
            var oldKnowledges1 = AvailableRange(KnowledgeList[0], 0, OldPositions[0]);
            var newKnowledges1 = AvailableRange(KnowledgeList[0], OldPositions[0], NewPositions[0]);

            var oldKnowledges2 = AvailableRange(KnowledgeList[1], 0, OldPositions[1]);
            var newKnowledges2 = AvailableRange(KnowledgeList[1], OldPositions[1], NewPositions[1]);

            foreach (var k1 in newKnowledges1)
            {
                foreach (var k2 in oldKnowledges2)
                {
                    Pairs.Enqueue(new(ruleInfo, [k1, k2]));
                }
            }
            foreach (var k1 in Concat2(oldKnowledges1, newKnowledges1))
            {
                foreach (var k2 in newKnowledges2)
                {
                    Pairs.Enqueue(new(ruleInfo, [k1, k2]));
                }
            }
            ruleInfo.LeftIndexs[0] = NewPositions[0];
            ruleInfo.LeftIndexs[1] = NewPositions[1];
        }
        void makePair3()
        {
            var oldKnowledges1 = AvailableRange(KnowledgeList[0], 0, OldPositions[0]);
            var newKnowledges1 = AvailableRange(KnowledgeList[0], OldPositions[0], NewPositions[0]);

            var oldKnowledges2 = AvailableRange(KnowledgeList[1], 0, OldPositions[1]);
            var newKnowledges2 = AvailableRange(KnowledgeList[1], OldPositions[1], NewPositions[1]);

            var oldKnowledges3 = AvailableRange(KnowledgeList[2], 0, OldPositions[2]);
            var newKnowledges3 = AvailableRange(KnowledgeList[2], OldPositions[2], NewPositions[2]);

            foreach (var k1 in newKnowledges1)
            {
                foreach (var k2 in oldKnowledges2)
                {
                    foreach (var k3 in oldKnowledges3)
                    {
                        Pairs.Enqueue(new(ruleInfo, [k1, k2, k3]));
                    }
                }
            }
            foreach (var k1 in Concat2(oldKnowledges1, newKnowledges1))
            {
                foreach (var k2 in newKnowledges2)
                {
                    foreach (var k3 in oldKnowledges3)
                    {
                        Pairs.Enqueue(new(ruleInfo, [k1, k2, k3]));
                    }
                }
            }
            foreach (var k1 in Concat2(oldKnowledges1, newKnowledges1))
            {
                foreach (var k2 in Concat2(oldKnowledges2, newKnowledges2))
                {
                    foreach (var k3 in newKnowledges3)
                    {
                        Pairs.Enqueue(new(ruleInfo, [k1, k2, k3]));
                    }
                }
            }

            ruleInfo.LeftIndexs[0] = NewPositions[0];
            ruleInfo.LeftIndexs[1] = NewPositions[1];
            ruleInfo.LeftIndexs[2] = NewPositions[2];
        }
        void makePair4()
        {
            var oldKnowledges1 = AvailableRange(KnowledgeList[0], 0, OldPositions[0]);
            var newKnowledges1 = AvailableRange(KnowledgeList[0], OldPositions[0], NewPositions[0]);

            var oldKnowledges2 = AvailableRange(KnowledgeList[1], 0, OldPositions[1]);
            var newKnowledges2 = AvailableRange(KnowledgeList[1], OldPositions[1], NewPositions[1]);

            var oldKnowledges3 = AvailableRange(KnowledgeList[2], 0, OldPositions[2]);
            var newKnowledges3 = AvailableRange(KnowledgeList[2], OldPositions[2], NewPositions[2]);

            var oldKnowledges4 = AvailableRange(KnowledgeList[3], 0, OldPositions[3]);
            var newKnowledges4 = AvailableRange(KnowledgeList[3], OldPositions[3], NewPositions[3]);

            foreach (var k1 in newKnowledges1)
            {
                foreach (var k2 in oldKnowledges2)
                {
                    foreach (var k3 in oldKnowledges3)
                    {
                        foreach (var k4 in oldKnowledges4)
                        {
                            Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4]));
                        }
                    }
                }
            }

            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2)
                {
                    foreach (var k3 in oldKnowledges3)
                    {
                        foreach (var k4 in oldKnowledges4)
                        {
                            Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4]));
                        }
                    }
                }
            }

            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in Concat2(newKnowledges2, oldKnowledges2))
                {
                    foreach (var k3 in newKnowledges3)
                    {
                        foreach (var k4 in oldKnowledges4)
                        {
                            Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4]));
                        }
                    }
                }
            }
            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in Concat2(newKnowledges2, oldKnowledges2))
                {
                    foreach (var k3 in Concat2(newKnowledges3, oldKnowledges3))
                    {
                        foreach (var k4 in newKnowledges4)
                        {
                            Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4]));
                        }
                    }
                }
            }

            ruleInfo.LeftIndexs[0] = NewPositions[0];
            ruleInfo.LeftIndexs[1] = NewPositions[1];
            ruleInfo.LeftIndexs[2] = NewPositions[2];
            ruleInfo.LeftIndexs[3] = NewPositions[3];
        }
        void makePair5()
        {
            var oldKnowledges1 = AvailableRange(KnowledgeList[0], 0, OldPositions[0]);
            var newKnowledges1 = AvailableRange(KnowledgeList[0], OldPositions[0], NewPositions[0]);

            var oldKnowledges2 = AvailableRange(KnowledgeList[1], 0, OldPositions[1]);
            var newKnowledges2 = AvailableRange(KnowledgeList[1], OldPositions[1], NewPositions[1]);

            var oldKnowledges3 = AvailableRange(KnowledgeList[2], 0, OldPositions[2]);
            var newKnowledges3 = AvailableRange(KnowledgeList[2], OldPositions[2], NewPositions[2]);

            var oldKnowledges4 = AvailableRange(KnowledgeList[3], 0, OldPositions[3]);
            var newKnowledges4 = AvailableRange(KnowledgeList[3], OldPositions[3], NewPositions[3]);

            var oldKnowledges5 = AvailableRange(KnowledgeList[4], 0, OldPositions[4]);
            var newKnowledges5 = AvailableRange(KnowledgeList[4], OldPositions[4], NewPositions[4]);
            foreach (var k1 in newKnowledges1)
            {
                foreach (var k2 in oldKnowledges2)
                {
                    foreach (var k3 in oldKnowledges3)
                    {
                        foreach (var k4 in oldKnowledges4)
                        {
                            foreach (var k5 in oldKnowledges5)
                            {
                                Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4, k5]));
                            }
                        }
                    }
                }
            }

            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2)
                {
                    foreach (var k3 in oldKnowledges3)
                    {
                        foreach (var k4 in oldKnowledges4)
                        {
                            foreach (var k5 in oldKnowledges5)
                            {
                                Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4, k5]));
                            }
                        }
                    }
                }
            }

            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in Concat2(newKnowledges2, oldKnowledges2))
                {
                    foreach (var k3 in newKnowledges3)
                    {
                        foreach (var k4 in oldKnowledges4)
                        {
                            foreach (var k5 in oldKnowledges5)
                            {
                                Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4, k5]));
                            }
                        }
                    }
                }
            }
            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in Concat2(newKnowledges2, oldKnowledges2))
                {
                    foreach (var k3 in Concat2(newKnowledges3, oldKnowledges3))
                    {
                        foreach (var k4 in newKnowledges4)
                        {
                            foreach (var k5 in oldKnowledges5)
                            {
                                Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4, k5]));
                            }
                        }
                    }
                }
            }
            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in Concat2(newKnowledges2, oldKnowledges2))
                {
                    foreach (var k3 in Concat2(newKnowledges3, oldKnowledges3))
                    {
                        foreach (var k4 in Concat2(newKnowledges4, oldKnowledges4))
                        {
                            foreach (var k5 in newKnowledges5)
                            {
                                Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4, k5]));
                            }
                        }
                    }
                }
            }
            ruleInfo.LeftIndexs[0] = NewPositions[0];
            ruleInfo.LeftIndexs[1] = NewPositions[1];
            ruleInfo.LeftIndexs[2] = NewPositions[2];
            ruleInfo.LeftIndexs[3] = NewPositions[3];
            ruleInfo.LeftIndexs[4] = NewPositions[4];
        }
        void makePair6()
        {
            var oldKnowledges1 = AvailableRange(KnowledgeList[0], 0, OldPositions[0]);
            var newKnowledges1 = AvailableRange(KnowledgeList[0], OldPositions[0], NewPositions[0]);

            var oldKnowledges2 = AvailableRange(KnowledgeList[1], 0, OldPositions[1]);
            var newKnowledges2 = AvailableRange(KnowledgeList[1], OldPositions[1], NewPositions[1]);

            var oldKnowledges3 = AvailableRange(KnowledgeList[2], 0, OldPositions[2]);
            var newKnowledges3 = AvailableRange(KnowledgeList[2], OldPositions[2], NewPositions[2]);

            var oldKnowledges4 = AvailableRange(KnowledgeList[3], 0, OldPositions[3]);
            var newKnowledges4 = AvailableRange(KnowledgeList[3], OldPositions[3], NewPositions[3]);

            var oldKnowledges5 = AvailableRange(KnowledgeList[4], 0, OldPositions[4]);
            var newKnowledges5 = AvailableRange(KnowledgeList[4], OldPositions[4], NewPositions[4]);
            var oldKnowledges6 = AvailableRange(KnowledgeList[5], 0, OldPositions[5]);
            var newKnowledges6 = AvailableRange(KnowledgeList[5], OldPositions[5], NewPositions[5]);
            foreach (var k1 in newKnowledges1)
            {
                foreach (var k2 in oldKnowledges2)
                {
                    foreach (var k3 in oldKnowledges3)
                    {
                        foreach (var k4 in oldKnowledges4)
                        {
                            foreach (var k5 in oldKnowledges5)
                            {
                                foreach (var k6 in oldKnowledges6)
                                {
                                    Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4, k5, k6]));
                                }
                            }
                        }
                    }
                }
            }
            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2)
                {
                    foreach (var k3 in oldKnowledges3)
                    {
                        foreach (var k4 in oldKnowledges4)
                        {
                            foreach (var k5 in oldKnowledges5)
                            {
                                foreach (var k6 in oldKnowledges6)
                                {
                                    Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4, k5, k6]));
                                }
                            }
                        }
                    }
                }
            }
            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in Concat2(newKnowledges2, oldKnowledges2))
                {
                    foreach (var k3 in newKnowledges3)
                    {
                        foreach (var k4 in oldKnowledges4)
                        {
                            foreach (var k5 in oldKnowledges5)
                            {
                                foreach (var k6 in oldKnowledges6)
                                {
                                    Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4, k5, k6]));
                                }
                            }
                        }
                    }
                }
            }
            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in Concat2(newKnowledges2, oldKnowledges2))
                {
                    foreach (var k3 in Concat2(newKnowledges3, oldKnowledges3))
                    {
                        foreach (var k4 in newKnowledges4)
                        {
                            foreach (var k5 in oldKnowledges5)
                            {
                                foreach (var k6 in oldKnowledges6)
                                {
                                    Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4, k5, k6]));
                                }
                            }
                        }
                    }
                }
            }
            foreach (var k1 in Concat2(newKnowledges1, oldKnowledges1))
            {
                foreach (var k2 in Concat2(newKnowledges2, oldKnowledges2))
                {
                    foreach (var k3 in Concat2(newKnowledges3, oldKnowledges3))
                    {
                        foreach (var k4 in Concat2(newKnowledges4, oldKnowledges4))
                        {
                            foreach (var k5 in Concat2(newKnowledges5, oldKnowledges5))
                            {
                                foreach (var k6 in newKnowledges6)
                                {
                                    Pairs.Enqueue(new(ruleInfo, [k1, k2, k3, k4, k5, k6]));
                                }
                            }
                        }
                    }
                }
            }

            ruleInfo.LeftIndexs[0] = NewPositions[0];
            ruleInfo.LeftIndexs[1] = NewPositions[1];
            ruleInfo.LeftIndexs[2] = NewPositions[2];
            ruleInfo.LeftIndexs[3] = NewPositions[3];
            ruleInfo.LeftIndexs[4] = NewPositions[4];
            ruleInfo.LeftIndexs[5] = NewPositions[5];
        }
    }
    void makeCombination(RuleInfo ruleInfo)
    {
        List<int> OldPositions = new(ruleInfo.LeftIndexs);
        if (!ruleInfo.LeftTypes.TrueForAll(type => knowledgeBase.Categories.ContainsKey(type))) return;
        List<int> NewPositions = ruleInfo.LeftTypes.Select(type => knowledgeBase.Categories[type].Count).ToList();
        List<List<Predicate>> KnowledgeList = ruleInfo.LeftTypes.Select(type => knowledgeBase.Categories[type]).ToList();

        Dictionary<Type, List<int>> InChunk = new Dictionary<Type, List<int>>();
        int index = 0;
        foreach (var type in ruleInfo.LeftTypes)
        {
            if (InChunk.ContainsKey(type))
            {
                InChunk[type].Add(index);
            }
            else
            {
                InChunk.Add(type, new List<int>() { index });
            }
            index++;
        }
        Dictionary<List<int>, (List<Predicate[]>, List<Predicate[]>)> Chunks = new();
        foreach (var kv in InChunk)
        {
            var c = makeSameTypePairs(KnowledgeList[kv.Value.First()],
                OldPositions[kv.Value.First()],
                NewPositions[kv.Value.First()], kv.Value.Count);
            Chunks.Add(kv.Value, c);
        }
        ComposeChunks(Chunks.Select(kv => (kv.Key, kv.Value)).ToList());

        for (int i = 0; i < ruleInfo.LeftIndexs.Count; i++)
        {
            ruleInfo.LeftIndexs[i] = NewPositions[i];
        }

        (List<Predicate[]> oldPairs, List<Predicate[]> newPairs) makeSameTypePairs(List<Predicate> ks, int oldPos, int newPos, int count)
        {
            List<Predicate[]> oldPairs = [];
            List<Predicate[]> newPairs = [];
            if (count == 1)
            {
                for (int i = 0; i < oldPos; i++)
                    if (ks[i].IsAvailable)
                        oldPairs.Add([ks[i]]);
                for (int i = oldPos; i < newPos; i++)
                    if (ks[i].IsAvailable)
                        newPairs.Add([ks[i]]);
            }
            else if (count == 2)
            {
                for (int i = 0; i < ks.Count; i++)
                {
                    if (!ks[i].IsAvailable) continue;
                    for (int j = i + 1; j < ks.Count; j++)
                    {
                        if (!ks[j].IsAvailable) continue;

                        if (i < oldPos && j < oldPos)
                            oldPairs.Add([ks[i], ks[j]]);
                        else
                            newPairs.Add([ks[i], ks[j]]);
                    }
                }
            }
            else if (count == 3)
            {
                for (int i = 0; i < ks.Count; i++)
                {
                    if (!ks[i].IsAvailable) continue;
                    for (int j = i + 1; j < ks.Count; j++)
                    {
                        if (!ks[j].IsAvailable) continue;
                        for (int k = j + 1; k < ks.Count; k++)
                        {
                            if (!ks[k].IsAvailable) continue;
                            if (i < oldPos && j < oldPos && k < oldPos)
                                oldPairs.Add([ks[i], ks[j], ks[k]]);
                            else
                                newPairs.Add([ks[i], ks[j], ks[k]]);
                        }
                    }
                }
            }
            else if (count == 4)
            {
                for (int i = 0; i < ks.Count; i++)
                {
                    if (!ks[i].IsAvailable) continue;
                    for (int j = i + 1; j < ks.Count; j++)
                    {
                        if (!ks[j].IsAvailable) continue;
                        for (int k = j + 1; k < ks.Count; k++)
                        {
                            if (!ks[k].IsAvailable) continue;
                            for (int l = k + 1; l < ks.Count; l++)
                            {
                                if (!ks[l].IsAvailable) continue;
                                if (i < oldPos && j < oldPos && k < oldPos && l < oldPos)
                                    oldPairs.Add([ks[i], ks[j], ks[k], ks[l]]);
                                else
                                    newPairs.Add([ks[i], ks[j], ks[k], ks[l]]);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception();
            }

            return (oldPairs, newPairs);
        }
        void ComposeChunks(List<(List<int>, (List<Predicate[]>, List<Predicate[]>))> Chunks)
        {
            if (Chunks.Count == 1)
            {
                Chunks[0].Item2.Item2.ForEach(item => Pairs.Enqueue(new(ruleInfo, item)));
            }
            else if (Chunks.Count == 2)
            {
                var oldChunk1 = Chunks[0].Item2.Item1;
                var newChunk1 = Chunks[0].Item2.Item2;
                var oldChunk2 = Chunks[1].Item2.Item1;
                var newChunk2 = Chunks[1].Item2.Item2;
                foreach (var k1 in newChunk1)
                {
                    foreach (var k2 in oldChunk2)
                    {
                        Pairs.Enqueue(new(ruleInfo, ConcatArrays(k1, k2)));
                    }
                }
                foreach (var k1 in Concat2(newChunk1, oldChunk1))
                {
                    foreach (var k2 in newChunk2)
                    {
                        Pairs.Enqueue(new(ruleInfo, ConcatArrays(k1, k2)));
                    }
                }
            }
            else if (Chunks.Count == 3)
            {
                var oldChunk1 = Chunks[0].Item2.Item1;
                var newChunk1 = Chunks[0].Item2.Item2;
                var oldChunk2 = Chunks[1].Item2.Item1;
                var newChunk2 = Chunks[1].Item2.Item2;
                var oldChunk3 = Chunks[2].Item2.Item1;
                var newChunk3 = Chunks[2].Item2.Item2;
                foreach (var k1 in newChunk1)
                {
                    foreach (var k2 in oldChunk2)
                    {
                        foreach (var k3 in oldChunk3)
                        {
                            Pairs.Enqueue(new(ruleInfo, ConcatArrays(k1, k2, k3)));
                        }
                    }
                }
                foreach (var k1 in Concat2(newChunk1, oldChunk1))
                {
                    foreach (var k2 in newChunk2)
                    {
                        foreach (var k3 in oldChunk3)
                        {
                            Pairs.Enqueue(new(ruleInfo, ConcatArrays(k1, k2, k3)));
                        }
                    }
                }
                foreach (var k1 in Concat2(newChunk1, oldChunk1))
                {
                    foreach (var k2 in Concat2(newChunk2, oldChunk2))
                    {
                        foreach (var k3 in newChunk3)
                        {
                            Pairs.Enqueue(new(ruleInfo, ConcatArrays(k1, k2, k3)));
                        }
                    }
                }
            }
            else if (Chunks.Count == 4)
            {
                var oldChunk1 = Chunks[0].Item2.Item1;
                var newChunk1 = Chunks[0].Item2.Item2;
                var oldChunk2 = Chunks[1].Item2.Item1;
                var newChunk2 = Chunks[1].Item2.Item2;
                var oldChunk3 = Chunks[2].Item2.Item1;
                var newChunk3 = Chunks[2].Item2.Item2;
                var oldChunk4 = Chunks[3].Item2.Item1;
                var newChunk4 = Chunks[3].Item2.Item2;
                foreach (var k1 in newChunk1)
                {
                    foreach (var k2 in oldChunk2)
                    {
                        foreach (var k3 in oldChunk3)
                        {
                            foreach (var k4 in oldChunk4)
                            {
                                Pairs.Enqueue(new(ruleInfo, ConcatArrays(k1, k2, k3, k4)));
                            }
                        }
                    }
                }
                foreach (var k1 in Concat2(newChunk1, oldChunk1))
                {
                    foreach (var k2 in newChunk2)
                    {
                        foreach (var k3 in oldChunk3)
                        {
                            foreach (var k4 in oldChunk4)
                            {
                                Pairs.Enqueue(new(ruleInfo, ConcatArrays(k1, k2, k3, k4)));
                            }
                        }
                    }
                }
                foreach (var k1 in Concat2(newChunk1, oldChunk1))
                {
                    foreach (var k2 in Concat2(newChunk2, oldChunk2))
                    {
                        foreach (var k3 in newChunk3)
                        {
                            foreach (var k4 in oldChunk4)
                            {
                                Pairs.Enqueue(new(ruleInfo, ConcatArrays(k1, k2, k3, k4)));
                            }
                        }
                    }
                }
                foreach (var k1 in Concat2(newChunk1, oldChunk1))
                {
                    foreach (var k2 in Concat2(newChunk2, oldChunk2))
                    {
                        foreach (var k3 in Concat2(newChunk3, oldChunk3))
                        {
                            foreach (var k4 in newChunk4)
                            {
                                Pairs.Enqueue(new(ruleInfo, ConcatArrays(k1, k2, k3, k4)));
                            }
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}