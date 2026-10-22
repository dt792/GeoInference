using GeoInference.Definitions.Knowledges;

using GeoInference.Knowledges;

/// <summary>
/// </summary>
public class InferenceSnapshot
{
    #region KnowledgeBase
    public Dictionary<Type, int> CategoryCounts { get; set; } = [];
    public HashSet<ulong> PredicateKeys { get; set; } = [];
    public Dictionary<Predicate, bool> PredicateAvailability { get; set; } = [];
    public HashSet<string> QuantityKeys { get; set; } = [];
    public HashSet<ulong> QuantityValueKeys { get; set; } = [];
    public HashSet<ulong> QuantityRatioKeys { get; set; } = [];
    public HashSet<ulong> LinearEquationKeys { get; set; } = [];
    public HashSet<ulong> ProductionEquationKeys { get; set; } = [];
    public HashSet<ulong> RemainingEquationKeys { get; set; } = [];
    public HashSet<ulong> EquationKeys { get; set; } = [];
    public int NewKnowledgeCount { get; set; }
    public int LastRoundKnowledgeCount { get; set; }
    public int ConditionalPairCount { get; set; }
    public Dictionary<ulong, int> InversedConditionalCounts { get; set; } = [];
    public List<ConditionalKnowledgeState> ConditionalKnowledgeStates { get; set; } = [];
    #endregion

    public List<List<int>> RuleLeftIndexs { get; set; } = [];
    public EquationSolver.EquationSolverState EquationState { get; set; }
    public TargetBase.TargetBaseState TargetState { get; set; }

    public int EqualityChainCurIndex { get; set; }
    public int EquationSystemCurIndex { get; set; }
}

/// <summary>
/// </summary>
public class ConditionalKnowledgeState
{
    public ConditionalKnowledge Knowledge { get; set; }
    public List<ulong> NullConditionKeys { get; set; } = [];
    public bool IsEstablish { get; set; }
    public int KnowledgeConditionsCount { get; set; }
}

/// <summary>
/// </summary>
public class InferenceCheckpoint
{
    [DI] KnowledgeBase knowledgeBase;
    [DI] RuleSolver ruleSolver;
    [DI] EquationSolver equationSolver;
    [DI] TargetBase targetBase;

    /// <summary>
    /// </summary>
    public InferenceSnapshot Capture()
    {
        return new InferenceSnapshot
        {
            CategoryCounts = knowledgeBase.Categories.ToDictionary(kv => kv.Key, kv => kv.Value.Count),
            PredicateKeys = new HashSet<ulong>(knowledgeBase.Predicates.Keys),
            PredicateAvailability = knowledgeBase.Predicates.Values.ToDictionary(p => p, p => p.IsAvailable),
            QuantityKeys = new HashSet<string>(knowledgeBase.Quantities.Keys),
            QuantityValueKeys = new HashSet<ulong>(knowledgeBase.QuantityValues.Keys),
            QuantityRatioKeys = new HashSet<ulong>(knowledgeBase.QuantityRatios.Keys),
            LinearEquationKeys = new HashSet<ulong>(knowledgeBase.LinearEquations.Keys),
            ProductionEquationKeys = new HashSet<ulong>(knowledgeBase.ProductionEquations.Keys),
            RemainingEquationKeys = new HashSet<ulong>(knowledgeBase.RemainingEquations.Keys),
            EquationKeys = new HashSet<ulong>(knowledgeBase.Equations.Keys),
            NewKnowledgeCount = knowledgeBase.NewKnowledges.Count,
            LastRoundKnowledgeCount = knowledgeBase.LastRoundKnowledges.Count,
            ConditionalPairCount = knowledgeBase.ConditionalKnowledgePairs.Count,
            InversedConditionalCounts = knowledgeBase.InversedConditionalKnowledgePairDict
                .ToDictionary(kv => kv.Key, kv => kv.Value.Count),
            ConditionalKnowledgeStates = knowledgeBase.ConditionalKnowledgePairs
                .Select(ck => new ConditionalKnowledgeState
                {
                    Knowledge = ck,
                    NullConditionKeys = ck.ConditionDict.Where(kv => kv.Value is null).Select(kv => kv.Key).ToList(),
                    IsEstablish = ck.IsEstablish,
                    KnowledgeConditionsCount = ck.Knowledge.Conditions.Count,
                })
                .ToList(),
            RuleLeftIndexs = ruleSolver.CaptureRuleState(),
            EquationState = equationSolver.CaptureState(),
            TargetState = targetBase.CaptureState(),
            EqualityChainCurIndex = EqualityChain.curIndex,
            EquationSystemCurIndex = EquationSystem.CurIndex,
        };
    }

    /// <summary>
    /// </summary>
    public void Restore(InferenceSnapshot snapshot)
    {
        RestoreKnowledgeBase(snapshot);
        targetBase.RestoreState(snapshot.TargetState);
        equationSolver.RestoreState(snapshot.EquationState);
        ruleSolver.RestoreRuleState(snapshot.RuleLeftIndexs);
        EqualityChain.curIndex = snapshot.EqualityChainCurIndex;
        EquationSystem.CurIndex = snapshot.EquationSystemCurIndex;
    }

    void RestoreKnowledgeBase(InferenceSnapshot snapshot)
    {
        foreach (var kv in snapshot.CategoryCounts)
        {
            if (knowledgeBase.Categories.TryGetValue(kv.Key, out var list) && list.Count > kv.Value)
            {
                list.RemoveRange(kv.Value, list.Count - kv.Value);
            }
        }
        foreach (var key in knowledgeBase.Categories.Keys.ToList())
        {
            if (!snapshot.CategoryCounts.ContainsKey(key))
            {
                knowledgeBase.Categories.Remove(key);
            }
        }

        RemoveNewKeys(knowledgeBase.Predicates, snapshot.PredicateKeys);
        foreach (var predicate in knowledgeBase.Predicates.Values)
        {
            if (snapshot.PredicateAvailability.TryGetValue(predicate, out var isAvailable))
            {
                predicate.IsAvailable = isAvailable;
            }
        }
        RemoveNewKeys(knowledgeBase.Quantities, snapshot.QuantityKeys);
        RemoveNewKeys(knowledgeBase.QuantityValues, snapshot.QuantityValueKeys);
        RemoveNewKeys(knowledgeBase.QuantityRatios, snapshot.QuantityRatioKeys);
        RemoveNewKeys(knowledgeBase.LinearEquations, snapshot.LinearEquationKeys);
        RemoveNewKeys(knowledgeBase.ProductionEquations, snapshot.ProductionEquationKeys);
        RemoveNewKeys(knowledgeBase.RemainingEquations, snapshot.RemainingEquationKeys);
        RemoveNewKeys(knowledgeBase.Equations, snapshot.EquationKeys);

        TruncateList(knowledgeBase.NewKnowledges, snapshot.NewKnowledgeCount);
        TruncateList(knowledgeBase.LastRoundKnowledges, snapshot.LastRoundKnowledgeCount);
        TruncateList(knowledgeBase.ConditionalKnowledgePairs, snapshot.ConditionalPairCount);

        knowledgeBase.ConditionalKnowledgePairDict.Clear();
        foreach (var ck in knowledgeBase.ConditionalKnowledgePairs)
        {
            if (!knowledgeBase.ConditionalKnowledgePairDict.TryGetValue(ck.Knowledge.HashCode, out var bucket))
            {
                bucket = [];
                knowledgeBase.ConditionalKnowledgePairDict.Add(ck.Knowledge.HashCode, bucket);
            }
            bucket.Add(ck);
        }

        foreach (var kv in snapshot.InversedConditionalCounts)
        {
            if (knowledgeBase.InversedConditionalKnowledgePairDict.TryGetValue(kv.Key, out var list)
                && list.Count > kv.Value)
            {
                list.RemoveRange(kv.Value, list.Count - kv.Value);
            }
        }
        foreach (var key in knowledgeBase.InversedConditionalKnowledgePairDict.Keys.ToList())
        {
            if (!snapshot.InversedConditionalCounts.ContainsKey(key))
            {
                knowledgeBase.InversedConditionalKnowledgePairDict.Remove(key);
            }
        }

        foreach (var ckState in snapshot.ConditionalKnowledgeStates)
        {
            var ck = ckState.Knowledge;
            foreach (var key in ckState.NullConditionKeys)
            {
                if (ck.ConditionDict.ContainsKey(key))
                {
                    ck.ConditionDict[key] = null;
                }
            }
            ck.IsEstablish = ckState.IsEstablish;
            TruncateList(ck.Knowledge.Conditions, ckState.KnowledgeConditionsCount);
        }
    }

    static void RemoveNewKeys<TKey, TValue>(Dictionary<TKey, TValue> dict, HashSet<TKey> snapshotKeys) where TKey : notnull
    {
        foreach (var key in dict.Keys.ToList())
        {
            if (!snapshotKeys.Contains(key))
            {
                dict.Remove(key);
            }
        }
    }

    static void TruncateList<T>(List<T> list, int count)
    {
        if (list.Count > count)
        {
            list.RemoveRange(count, list.Count - count);
        }
    }
}
