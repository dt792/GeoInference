
using System.Reflection;

public class PermutationAttribute : Attribute { }
public class RuleSolver
{
    [DI] KnowledgeBase knowledgeBase { get; set; }
    [DI] KnowledgeBaseUpdater Updater { get; set; }
    public Queue<RulePredPair> Pairs { get; set; } = [];
    public bool HasNext { get => Pairs.Count > 0; }
    public RulePredPair Next() => Pairs.Dequeue();
    public void Solve(RulePredPair pair)
    {
        pair.RuleInfo.RuleMethod.Invoke(pair.RuleInfo.RuleClass, pair.Args);
        
    }
    [DI] ZSingletonDI DI { get; set; }
    public List<RuleInfo> RuleInfos { get; set; } = [];
    ZDict<string, RuleInfo> CachedRuleInfos { get; set; } = [];
    public void LoadRules(IEnumerable<MethodInfo> methodInfos)
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
            var newKnowledges1 = KnowledgeList[0][OldPositions[0]..NewPositions[0]].Where(k => k.IsAvailable);
            foreach (var k1 in newKnowledges1)
            {
                Pairs.Enqueue(new(ruleInfo, [k1]));
            }
            ruleInfo.LeftIndexs[0] = NewPositions[0];
        }
        void makePair2()
        {
            var oldKnowledges1 = KnowledgeList[0][0..OldPositions[0]].Where(k => k.IsAvailable);
            var newKnowledges1 = KnowledgeList[0][OldPositions[0]..NewPositions[0]].Where(k => k.IsAvailable);

            var oldKnowledges2 = KnowledgeList[1][0..OldPositions[1]].Where(k => k.IsAvailable);
            var newKnowledges2 = KnowledgeList[1][OldPositions[1]..NewPositions[1]].Where(k => k.IsAvailable);

            foreach (var k1 in newKnowledges1)
            {
                foreach (var k2 in oldKnowledges2)
                {
                    Pairs.Enqueue(new(ruleInfo, [k1, k2]));
                }
            }
            foreach (var k1 in oldKnowledges1.Union(newKnowledges1))
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
            var oldKnowledges1 = KnowledgeList[0][0..OldPositions[0]].Where(k => k.IsAvailable);
            var newKnowledges1 = KnowledgeList[0][OldPositions[0]..NewPositions[0]].Where(k => k.IsAvailable);

            var oldKnowledges2 = KnowledgeList[1][0..OldPositions[1]].Where(k => k.IsAvailable);
            var newKnowledges2 = KnowledgeList[1][OldPositions[1]..NewPositions[1]].Where(k => k.IsAvailable);

            var oldKnowledges3 = KnowledgeList[2][0..OldPositions[2]].Where(k => k.IsAvailable);
            var newKnowledges3 = KnowledgeList[2][OldPositions[2]..NewPositions[2]].Where(k => k.IsAvailable);

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
            foreach (var k1 in oldKnowledges1.Union(newKnowledges1))
            {
                foreach (var k2 in newKnowledges2)
                {
                    foreach (var k3 in oldKnowledges3)
                    {
                        Pairs.Enqueue(new(ruleInfo, [k1, k2, k3]));
                    }
                }
            }
            foreach (var k1 in oldKnowledges1.Union(newKnowledges1))
            {
                foreach (var k2 in oldKnowledges2.Union(newKnowledges2))
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
            var oldKnowledges1 = KnowledgeList[0][0..OldPositions[0]].Where(k => k.IsAvailable);
            var newKnowledges1 = KnowledgeList[0][OldPositions[0]..NewPositions[0]].Where(k => k.IsAvailable);

            var oldKnowledges2 = KnowledgeList[1][0..OldPositions[1]].Where(k => k.IsAvailable);
            var newKnowledges2 = KnowledgeList[1][OldPositions[1]..NewPositions[1]].Where(k => k.IsAvailable);

            var oldKnowledges3 = KnowledgeList[2][0..OldPositions[2]].Where(k => k.IsAvailable);
            var newKnowledges3 = KnowledgeList[2][OldPositions[2]..NewPositions[2]].Where(k => k.IsAvailable);

            var oldKnowledges4 = KnowledgeList[3][0..OldPositions[3]].Where(k => k.IsAvailable);
            var newKnowledges4 = KnowledgeList[3][OldPositions[3]..NewPositions[3]].Where(k => k.IsAvailable);

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

            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
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

            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2.Union(oldKnowledges2))
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
            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2.Union(oldKnowledges2))
                {
                    foreach (var k3 in newKnowledges3.Union(oldKnowledges3))
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
            var oldKnowledges1 = KnowledgeList[0][0..OldPositions[0]].Where(k => k.IsAvailable);
            var newKnowledges1 = KnowledgeList[0][OldPositions[0]..NewPositions[0]].Where(k => k.IsAvailable);

            var oldKnowledges2 = KnowledgeList[1][0..OldPositions[1]].Where(k => k.IsAvailable);
            var newKnowledges2 = KnowledgeList[1][OldPositions[1]..NewPositions[1]].Where(k => k.IsAvailable);

            var oldKnowledges3 = KnowledgeList[2][0..OldPositions[2]].Where(k => k.IsAvailable);
            var newKnowledges3 = KnowledgeList[2][OldPositions[2]..NewPositions[2]].Where(k => k.IsAvailable);

            var oldKnowledges4 = KnowledgeList[3][0..OldPositions[3]].Where(k => k.IsAvailable);
            var newKnowledges4 = KnowledgeList[3][OldPositions[3]..NewPositions[3]].Where(k => k.IsAvailable);

            var oldKnowledges5 = KnowledgeList[4][0..OldPositions[4]].Where(k => k.IsAvailable);
            var newKnowledges5 = KnowledgeList[4][OldPositions[4]..NewPositions[4]].Where(k => k.IsAvailable);
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

            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
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

            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2.Union(oldKnowledges2))
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
            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2.Union(oldKnowledges2))
                {
                    foreach (var k3 in newKnowledges3.Union(oldKnowledges3))
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
            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2.Union(oldKnowledges2))
                {
                    foreach (var k3 in newKnowledges3.Union(oldKnowledges3))
                    {
                        foreach (var k4 in newKnowledges4.Union(oldKnowledges4))
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
            var oldKnowledges1 = KnowledgeList[0][0..OldPositions[0]].Where(k => k.IsAvailable);
            var newKnowledges1 = KnowledgeList[0][OldPositions[0]..NewPositions[0]].Where(k => k.IsAvailable);

            var oldKnowledges2 = KnowledgeList[1][0..OldPositions[1]].Where(k => k.IsAvailable);
            var newKnowledges2 = KnowledgeList[1][OldPositions[1]..NewPositions[1]].Where(k => k.IsAvailable);

            var oldKnowledges3 = KnowledgeList[2][0..OldPositions[2]].Where(k => k.IsAvailable);
            var newKnowledges3 = KnowledgeList[2][OldPositions[2]..NewPositions[2]].Where(k => k.IsAvailable);

            var oldKnowledges4 = KnowledgeList[3][0..OldPositions[3]].Where(k => k.IsAvailable);
            var newKnowledges4 = KnowledgeList[3][OldPositions[3]..NewPositions[3]].Where(k => k.IsAvailable);

            var oldKnowledges5 = KnowledgeList[4][0..OldPositions[4]].Where(k => k.IsAvailable);
            var newKnowledges5 = KnowledgeList[4][OldPositions[4]..NewPositions[4]].Where(k => k.IsAvailable);
            var oldKnowledges6 = KnowledgeList[5][0..OldPositions[5]].Where(k => k.IsAvailable);
            var newKnowledges6 = KnowledgeList[5][OldPositions[5]..NewPositions[5]].Where(k => k.IsAvailable);
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
            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
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
            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2.Union(oldKnowledges2))
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
            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2.Union(oldKnowledges2))
                {
                    foreach (var k3 in newKnowledges3.Union(oldKnowledges3))
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
            foreach (var k1 in newKnowledges1.Union(oldKnowledges1))
            {
                foreach (var k2 in newKnowledges2.Union(oldKnowledges2))
                {
                    foreach (var k3 in newKnowledges3.Union(oldKnowledges3))
                    {
                        foreach (var k4 in newKnowledges4.Union(oldKnowledges4))
                        {
                            foreach (var k5 in newKnowledges5.Union(oldKnowledges5))
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

        (List<Predicate[]> oldPairs, List<Predicate[]> newPairs) makeSameTypePairs(List<Predicate> ks, int oldPos, int newPos, int count)
        {
            List<Predicate[]> oldPairs = [];
            List<Predicate[]> newPairs = [];
            if (count == 1)
            {
                oldPairs = ks[0..oldPos].Where(k => k.IsAvailable).Select(p => new Predicate[] { p }).ToList();
                newPairs = ks[oldPos..newPos].Where(k => k.IsAvailable).Select(p => new Predicate[] { p }).ToList();
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
                        Pairs.Enqueue(new(ruleInfo, k1.Union(k2).ToArray()));
                    }
                }
                foreach (var k1 in newChunk1.Union(oldChunk1))
                {
                    foreach (var k2 in newChunk2)
                    {
                        Pairs.Enqueue(new(ruleInfo, k1.Union(k2).ToArray()));
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
                            Pairs.Enqueue(new(ruleInfo, k1.Union(k2).Union(k3).ToArray()));
                        }
                    }
                }
                foreach (var k1 in newChunk1.Union(oldChunk1))
                {
                    foreach (var k2 in newChunk2)
                    {
                        foreach (var k3 in oldChunk3)
                        {
                            Pairs.Enqueue(new(ruleInfo, k1.Union(k2).Union(k3).ToArray()));
                        }
                    }
                }
                foreach (var k1 in newChunk1.Union(oldChunk1))
                {
                    foreach (var k2 in newChunk2.Union(oldChunk2))
                    {
                        foreach (var k3 in newChunk3)
                        {
                            Pairs.Enqueue(new(ruleInfo, k1.Union(k2).Union(k3).ToArray()));
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
                                Pairs.Enqueue(new(ruleInfo, k1.Union(k2).Union(k3).Union(k4).ToArray()));
                            }
                        }
                    }
                }
                foreach (var k1 in newChunk1.Union(oldChunk1))
                {
                    foreach (var k2 in newChunk2)
                    {
                        foreach (var k3 in oldChunk3)
                        {
                            foreach (var k4 in oldChunk4)
                            {
                                Pairs.Enqueue(new(ruleInfo, k1.Union(k2).Union(k3).Union(k4).ToArray()));
                            }
                        }
                    }
                }
                foreach (var k1 in newChunk1.Union(oldChunk1))
                {
                    foreach (var k2 in newChunk2.Union(oldChunk2))
                    {
                        foreach (var k3 in newChunk3)
                        {
                            foreach (var k4 in oldChunk4)
                            {
                                Pairs.Enqueue(new(ruleInfo, k1.Union(k2).Union(k3).Union(k4).ToArray()));
                            }
                        }
                    }
                }
                foreach (var k1 in newChunk1.Union(oldChunk1))
                {
                    foreach (var k2 in newChunk2.Union(oldChunk2))
                    {
                        foreach (var k3 in newChunk3.Union(oldChunk3))
                        {
                            foreach (var k4 in newChunk4)
                            {
                                Pairs.Enqueue(new(ruleInfo, k1.Union(k2).Union(k3).Union(k4).ToArray()));
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