using GeoInference.Definitions.Knowledges;




public class ProvePredicateTarget : Target
{
    public Predicate Target { get; set; }
    public override string ToString() =>
       GeoInferenceApp.IsZhOrEn ? $"证明：{Target}成立" : $"Proof: {Target} holds.";
}
public class ProveLinearTarget : Target
{
    public Dictionary<Quantity, Expr> Dict = [];
    public List<Knowledge> Conditions { get; set; } = [];
    public override string ToString() => $"求：{Dict}的值";
}
public class ProveProductionTarget : Target
{
    public Dictionary<Quantity, Expr> Dict = [];
    public List<Knowledge> Conditions { get; set; } = [];
    public override string ToString() => $"求：{Dict}的值";
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
         GeoInferenceApp.IsZhOrEn ? $"求：{GeoQuantity}的值" : $"Find the value of {GeoQuantity}.";
}
public class SolveQuantityRatioTarget : Target
{
    public Quantity GeoQuantity1 = null;
    public Quantity GeoQuantity2 = null;
    public override string ToString() =>
        GeoInferenceApp.IsZhOrEn ? $"求：{GeoQuantity1}/{GeoQuantity2}的值" : $"Find the value of {GeoQuantity1}/{GeoQuantity2}.";
}
public class SolveLinearTarget : Target
{
    public List<Knowledge> Conditions { get; set; } = [];
    public Expr Tmp { get; set; } = Expr.Zero;
    public Expr Target { get; set; }
    public Dictionary<Quantity, Expr> CoffDict { get; set; } = [];
    public override string ToString() =>
        GeoInferenceApp.IsZhOrEn ? $"求：{StringTool.ComposeList(CoffDict, "+", p => $"{p.Value}*{p.Key}")}的值"
        : $"Find the value of {StringTool.ComposeList(CoffDict, "+", p => $"{p.Value}*{p.Key}")}.";
}
public class SolveProductionTarget : Target
{
    public List<Knowledge> Conditions { get; set; } = [];
    public Expr Tmp { get; set; } = Expr.One;
    public Expr Target { get; set; }
    public Dictionary<Quantity, Expr> CoffDict { get; set; } = [];
    public override string ToString() =>
        GeoInferenceApp.IsZhOrEn ? $"求：{StringTool.ComposeList(CoffDict, "*", p => $"({p.Value})^({p.Key})")}的值"
        : $"Find the value of {StringTool.ComposeList(CoffDict, "*", p => $"({p.Value})^({p.Key})")}.";
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
       GeoInferenceApp.IsZhOrEn ? $"求：{Target}的值"
       : $"Find the value of {Target}.";
}
public abstract class Target
{
    public int Index { get; set; }
    public bool IsSuccess { get; set; }
    public Knowledge Conclusion { get; set; }
    public string Answer { get; set; }
}
internal class TargetBase
{
    public bool AllSolved { get; set; }
    public List<Target> Targets { get; set; } = [];
    public List<ProvePredicateTarget> ProvePredicateTargets { get; set; } = [];
    public List<ProveLinearTarget> ProveLinearTargets { get; set; } = [];
    public List<ProveProductionTarget> ProveProductionTargets { get; set; } = [];
    public List<ProveEquationTarget> ProveEquationTargets { get; set; } = [];

    public List<SolveQuantityValueTarget> SolveQuantityValueTargets { get; set; } = [];
    public List<SolveQuantityRatioTarget> SolveQuantityRatioTargets { get; set; } = [];
    public List<SolveLinearTarget> SolveLinearTargets { get; set; } = [];
    public List<SolveProductionTarget> SolveProductionTargets { get; set; } = [];
    public List<SolveExprTarget> SolveExprTargets { get; set; } = [];

    public List<Expr> TakeMinValue { get; set; } = [];
    public List<Expr> TakeMaxValue { get; set; } = [];
}
