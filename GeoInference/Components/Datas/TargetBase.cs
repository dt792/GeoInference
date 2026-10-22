using GeoInference.Definitions.Knowledges;

public class ProvePredicateTarget : Target
{
    public Predicate Target { get; set; }
    public override string ToString() =>
       GeoInferenceApp.UseChinese ? $"证明：{Target}成立" : $"Proof: {Target} holds.";
}
public class ProveEquationTarget : Target
{
    public Equation Target { get; set; }
    public string Left { get; set; }
    public string Right { get; set; }
    public List<GeoQuantity> GeoQuantitys = null;
    public override string ToString() => $"证明：{Target}成立";
}

public class SolveQuantityValueTarget : Target
{
    public Predicate GeoQuantityKnowledge = null;
    public Quantity GeoQuantity = null;
    public override string ToString() =>
         GeoInferenceApp.UseChinese ? $"求：{GeoQuantity}的值" : $"Find the value of {GeoQuantity}.";
}
public class SolveQuantityRatioTarget : Target
{
    public Quantity GeoQuantity1 = null;
    public Quantity GeoQuantity2 = null;
    public override string ToString() =>
        GeoInferenceApp.UseChinese ? $"求：{GeoQuantity1}/{GeoQuantity2}的值" : $"Find the value of {GeoQuantity1}/{GeoQuantity2}.";
}
public class SolveLinearTarget : Target
{
    public List<Knowledge> Conditions { get; set; } = [];
    public Expr Tmp { get; set; } = Expr.Zero;
    public Expr Target { get; set; }
    public Dictionary<Quantity, Expr> CoffDict { get; set; } = [];
    public override string ToString() =>
        GeoInferenceApp.UseChinese ? $"求：{StringTool.ComposeList(CoffDict, "+", p => $"{p.Value}*{p.Key}")}的值"
        : $"Find the value of {StringTool.ComposeList(CoffDict, "+", p => $"{p.Value}*{p.Key}")}.";
}
public class SolveProductionTarget : Target
{
    public List<Knowledge> Conditions { get; set; } = [];
    public Expr Tmp { get; set; } = Expr.One;
    public Expr Target { get; set; }
    public Dictionary<Quantity, Expr> CoffDict { get; set; } = [];
    public override string ToString() =>
        GeoInferenceApp.UseChinese ? $"求：{StringTool.ComposeList(CoffDict, "*", p => $"({p.Key})^({p.Value})")}的值"
        : $"Find the value of {StringTool.ComposeList(CoffDict, "*", p => $"({p.Key})^({p.Value})")}.";
}
public class SolveExprTarget : Target
{
    /// <summary>
    
    /// </summary>
    public Expr Target { get; set; }
    public string Tmp { get; set; }
    public List<Quantity> Quantities { get; set; } = [];
    public List<Knowledge> Conditions { get; set; } = [];

    public override string ToString() =>
       GeoInferenceApp.UseChinese ? $"求：{Target}的值"
       : $"Find the value of {Target}.";
}
public abstract class Target
{
    public int Index { get; set; }
    public bool IsSuccess { get; set; }
    public Knowledge Conclusion { get; set; }
    public string Answer { get; set; }
}
public class TargetBase
{
    public bool AllSolved { get; set; }
    public List<Target> Targets { get; set; } = [];
    public List<ProvePredicateTarget> ProvePredicateTargets { get; set; } = [];
    public List<ProveEquationTarget> ProveEquationTargets { get; set; } = [];

    public List<SolveQuantityValueTarget> SolveQuantityValueTargets { get; set; } = [];
    public List<SolveQuantityRatioTarget> SolveQuantityRatioTargets { get; set; } = [];
    public List<SolveLinearTarget> SolveLinearTargets { get; set; } = [];
    public List<SolveProductionTarget> SolveProductionTargets { get; set; } = [];
    public List<SolveExprTarget> SolveExprTargets { get; set; } = [];

    #region Checkpoint
    /// <summary>
    /// </summary>
    public class TargetState
    {
        public Target Target { get; set; }
        public bool IsSuccess { get; set; }
        public Knowledge Conclusion { get; set; }
        public string Answer { get; set; }
        public Predicate ProvePredicate { get; set; }
        public Dictionary<Quantity, Expr> CoffDict { get; set; }
        public Expr Tmp { get; set; }
        public string ExprTmp { get; set; }
        public List<Quantity> ExprQuantities { get; set; }
        public int ConditionsCount { get; set; }
    }
    /// <summary>
    /// </summary>
    public class TargetBaseState
    {
        public bool AllSolved { get; set; }
        public List<ProvePredicateTarget> ProvePredicateTargets { get; set; }
        public List<ProveEquationTarget> ProveEquationTargets { get; set; }
        public List<SolveQuantityValueTarget> SolveQuantityValueTargets { get; set; }
        public List<SolveQuantityRatioTarget> SolveQuantityRatioTargets { get; set; }
        public List<SolveLinearTarget> SolveLinearTargets { get; set; }
        public List<SolveProductionTarget> SolveProductionTargets { get; set; }
        public List<SolveExprTarget> SolveExprTargets { get; set; }
        public List<TargetState> Targets { get; set; } = [];
    }

    static TargetState CaptureTarget(Target target)
    {
        var state = new TargetState
        {
            Target = target,
            IsSuccess = target.IsSuccess,
            Conclusion = target.Conclusion,
            Answer = target.Answer,
        };
        switch (target)
        {
            case ProvePredicateTarget provePredicate:
                state.ProvePredicate = provePredicate.Target;
                break;
            case SolveLinearTarget solveLinear:
                state.CoffDict = new Dictionary<Quantity, Expr>(solveLinear.CoffDict);
                state.Tmp = solveLinear.Tmp;
                state.ConditionsCount = solveLinear.Conditions.Count;
                break;
            case SolveProductionTarget solveProduction:
                state.CoffDict = new Dictionary<Quantity, Expr>(solveProduction.CoffDict);
                state.Tmp = solveProduction.Tmp;
                state.ConditionsCount = solveProduction.Conditions.Count;
                break;
            case SolveExprTarget solveExpr:
                state.ExprTmp = solveExpr.Tmp;
                state.ExprQuantities = new List<Quantity>(solveExpr.Quantities);
                state.ConditionsCount = solveExpr.Conditions.Count;
                break;
        }
        return state;
    }

    static void RestoreTarget(TargetState state)
    {
        var target = state.Target;
        target.IsSuccess = state.IsSuccess;
        target.Conclusion = state.Conclusion;
        target.Answer = state.Answer;
        switch (target)
        {
            case ProvePredicateTarget provePredicate:
                provePredicate.Target = state.ProvePredicate;
                break;
            case SolveLinearTarget solveLinear:
                solveLinear.CoffDict = new Dictionary<Quantity, Expr>(state.CoffDict);
                solveLinear.Tmp = state.Tmp;
                TruncateConditions(solveLinear.Conditions, state.ConditionsCount);
                break;
            case SolveProductionTarget solveProduction:
                solveProduction.CoffDict = new Dictionary<Quantity, Expr>(state.CoffDict);
                solveProduction.Tmp = state.Tmp;
                TruncateConditions(solveProduction.Conditions, state.ConditionsCount);
                break;
            case SolveExprTarget solveExpr:
                solveExpr.Tmp = state.ExprTmp;
                solveExpr.Quantities = new List<Quantity>(state.ExprQuantities);
                TruncateConditions(solveExpr.Conditions, state.ConditionsCount);
                break;
        }
    }

    static void TruncateConditions(List<Knowledge> conditions, int count)
    {
        if (conditions.Count > count)
            conditions.RemoveRange(count, conditions.Count - count);
    }

    public TargetBaseState CaptureState()
    {
        return new TargetBaseState
        {
            AllSolved = AllSolved,
            ProvePredicateTargets = new List<ProvePredicateTarget>(ProvePredicateTargets),
            ProveEquationTargets = new List<ProveEquationTarget>(ProveEquationTargets),
            SolveQuantityValueTargets = new List<SolveQuantityValueTarget>(SolveQuantityValueTargets),
            SolveQuantityRatioTargets = new List<SolveQuantityRatioTarget>(SolveQuantityRatioTargets),
            SolveLinearTargets = new List<SolveLinearTarget>(SolveLinearTargets),
            SolveProductionTargets = new List<SolveProductionTarget>(SolveProductionTargets),
            SolveExprTargets = new List<SolveExprTarget>(SolveExprTargets),
            Targets = Targets.Select(CaptureTarget).ToList(),
        };
    }

    public void RestoreState(TargetBaseState state)
    {
        AllSolved = state.AllSolved;
        RestoreList(ProvePredicateTargets, state.ProvePredicateTargets);
        RestoreList(ProveEquationTargets, state.ProveEquationTargets);
        RestoreList(SolveQuantityValueTargets, state.SolveQuantityValueTargets);
        RestoreList(SolveQuantityRatioTargets, state.SolveQuantityRatioTargets);
        RestoreList(SolveLinearTargets, state.SolveLinearTargets);
        RestoreList(SolveProductionTargets, state.SolveProductionTargets);
        RestoreList(SolveExprTargets, state.SolveExprTargets);
        foreach (var targetState in state.Targets)
        {
            RestoreTarget(targetState);
        }
    }

    static void RestoreList<T>(List<T> current, List<T> snapshot)
    {
        current.Clear();
        current.AddRange(snapshot);
    }
    #endregion
}
