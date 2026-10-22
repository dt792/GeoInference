
using Newtonsoft.Json.Linq;

using System.Text.RegularExpressions;

public class PairedEqInfo
{
    public Equation Equation { get; set; }
    public string EquationStr { get; set; }
    public List<Quantity> Quantities { get; set; } = [];
    public List<Knowledge> SimplifiedReasons { get; set; } = [];
    public override string ToString()
    {
        return Equation.ToString();
    }
}
public class EquationSolver
{
    public Action MatrixInferenced { get; set; }
    protected MapleApp mapleApp = MapleApp.Instance;
    public List<PairedEqInfo> AngularLinerPairedInfos { get; set; } = [];
    public virtual MapleBaseLinearMatrix AngularLinearMatrix { get; set; } = new();
    public List<PairedEqInfo> AngularOtherPairedInfos { get; set; } = [];
    public virtual MapleBaseProductionMatrix AngularProductionMatrix { get; set; } = new();
    public List<PairedEqInfo> AngularProductionPairedInfos { get; set; } = [];

    public Action<EqualityChain> ContinuedEqualityUpdated { get; set; }

    public List<PairedEqInfo> DistanceLinerPairedInfos { get; set; } = [];
    public virtual MapleBaseLinearMatrix DistanceLinearMatrix { get; set; } = new();
    public List<PairedEqInfo> DistanceOtherPairedInfos { get; set; } = [];
    public virtual MapleBaseProductionMatrix DistanceProductionMatrix { get; set; } = new();
    public List<PairedEqInfo> DistanceProductionPairedInfos { get; set; } = [];

    public List<PairedEqInfo> OtherMixedPairedInfos { get; set; } = [];
    public Action<MapleBaseLinearMatrix> QuantityEquationMatrixUpdated { get; set; }

    public List<EquationSystem> ReadyToTryEqSyms { get; set; } = [];
    public List<EquationSystem> TooComplexEqsyms { get; set; } = [];
    public List<EquationSystem> TriedEqSyms { get; set; } = [];
    [DI] public ZScriptBuilder builder { get; set; }
    [DI] public KnowledgeBase knowledgeBase { get; set; }

    [DI] public KnowledgeBaseUpdater updater { get; set; }
    [DI] public GeoInferenceApp App;
    #region Update
    public virtual void Update(Knowledge KnowledgeAdded)
    {
        if (KnowledgeAdded is QuantityValue qv)
        {
            AddQV(qv);
        }
        else if (KnowledgeAdded is QuantityRatio qr)
        {
            AddQR(qr);
        }
        else if (KnowledgeAdded is LinearEquation liner)
        {
            if (liner.Coff.Keys.ToList().TrueForAll(p => p.Unit == QuantityClassifications.Distance || p.Unit == QuantityClassifications.Sin || p.Unit == QuantityClassifications.Cos || p.Unit == QuantityClassifications.Tan))
            {
                liner.Type = GeoEquationTypes.DistanceLinear;
                AddLinerEquation(liner);
            }
            else if (liner.Coff.Keys.ToList().TrueForAll(p => p.Unit == QuantityClassifications.Angle))
            {
                liner.Type = GeoEquationTypes.AngularLinear;
                AddLinerEquation(liner);
            }
            else
            {
                liner.Type = GeoEquationTypes.MixedOther;
                AddOtherEquation(liner);
            }
        }
        else if (KnowledgeAdded is ProductionEquation pequation)
        {
            if (pequation.Coff.Keys.ToList().TrueForAll(p => p.Unit == QuantityClassifications.Distance || p.Unit == QuantityClassifications.Sin || p.Unit == QuantityClassifications.Cos || p.Unit == QuantityClassifications.Tan))
            {
                pequation.Type = GeoEquationTypes.DistanceProduction;
                AddProductionEquation(pequation);
            }
            else if (pequation.Coff.Keys.ToList().TrueForAll(p => p.Unit == QuantityClassifications.Angle))
            {
                pequation.Type = GeoEquationTypes.AngularProduction;
                AddProductionEquation(pequation);
            }
            else
            {
                pequation.Type = GeoEquationTypes.MixedOther;
                AddOtherEquation(pequation);
            }
        }
        else if (KnowledgeAdded is Equation equation)
        {
            List<Quantity> quantities = builder.GetEquationQuantities(equation.ToString());
            if (quantities.TrueForAll(p => p.Unit == QuantityClassifications.Distance))
            {
                equation.Type = GeoEquationTypes.DistanceOther;
                AddOtherEquation(equation);
            }
            else if (quantities.TrueForAll(p => p.Unit == QuantityClassifications.Angle))
            {
                equation.Type = GeoEquationTypes.AngularOther;
                AddOtherEquation(equation);
            }
            else
            {
                equation.Type = GeoEquationTypes.MixedOther;
                AddOtherEquation(equation);
            }
        }
    }
    public virtual void AddQV(QuantityValue qvalue)
    {
        if (qvalue.Expr == 0)
            return;
        var unit = qvalue.Quantity.Unit;
        if (knowledgeBase.IndexContinuedDict.TryGetValue(qvalue.Quantity, out var hadeq))
        {
            if (hadeq.ActualValue is not null)
            {
                hadeq.SetReason(qvalue.Quantity.ToString(), EqualityChain.Value, qvalue);
                return;
            }
            var valueeq = knowledgeBase.EqualityChains[unit].FirstOrDefault(eq => eq.ActualValue is not null);
            hadeq.AddValue(qvalue);
            if (valueeq is null)
            {
                MakeNewKnowledge(hadeq);
                ECUpdated(hadeq);
                return;
            }
            if (valueeq.index < hadeq.index)
            {
                valueeq.CombineByValue(hadeq);
                foreach (var item in hadeq.CoffDict)
                {
                    knowledgeBase.IndexContinuedDict[item.Key] = valueeq;
                }
                knowledgeBase.EqualityChains[hadeq.Unit].Remove(hadeq);
                MakeNewKnowledge(valueeq);
                ECUpdated(valueeq);
            }
            else
            {
                hadeq.CombineByValue(valueeq);
                foreach (var item in valueeq.CoffDict)
                {
                    knowledgeBase.IndexContinuedDict[item.Key] = hadeq;
                }
                knowledgeBase.EqualityChains[hadeq.Unit].Remove(valueeq);
                MakeNewKnowledge(hadeq);
                ECUpdated(hadeq);
            }
            return;
        }
        var valuedChain = knowledgeBase.EqualityChains[unit].FirstOrDefault(eq => eq.ActualValue is not null);
        if (valuedChain is not null)
        {
            valuedChain.AddValue(qvalue);
            knowledgeBase.IndexContinuedDict[qvalue.Quantity] = valuedChain;
            MakeNewKnowledge(valuedChain);
            ECUpdated(valuedChain);
        }
        else
        {
            valuedChain = new EqualityChain();
            valuedChain.AddValue(qvalue);
            knowledgeBase.EqualityChains[unit].Add(valuedChain);
            knowledgeBase.IndexContinuedDict[qvalue.Quantity] = valuedChain;
            MakeNewKnowledge(valuedChain);
            ECUpdated(valuedChain);
        }
    }
    public virtual void AddQR(QuantityRatio qratio)
    {
        if (qratio.Quantity1.Unit != qratio.Quantity2.Unit)
            return;
        var gq1 = qratio.Quantity1;
        var gq2 = qratio.Quantity2;
        var ratio = qratio.Ratio;
        EqualityChain eq1 = knowledgeBase.IndexContinuedDict.ContainsKey(gq1) ? knowledgeBase.IndexContinuedDict[gq1] : null;
        EqualityChain eq2 = knowledgeBase.IndexContinuedDict.ContainsKey(gq2) ? knowledgeBase.IndexContinuedDict[gq2] : null;
        if (eq1 is null && eq2 is null)
        {
            EqualityChain continuedEquality = new EqualityChain();
            continuedEquality.CoffDict.Add(gq1, 1);
            continuedEquality.AddRatio(gq1, gq2, ratio, qratio);
            knowledgeBase.EqualityChains[gq1.Unit].Add(continuedEquality);
            knowledgeBase.IndexContinuedDict.Add(gq1, continuedEquality);
            knowledgeBase.IndexContinuedDict.Add(gq2, continuedEquality);
            MakeNewKnowledge(continuedEquality);
            ECUpdated(continuedEquality);
        }
        else if (eq1 is not null && eq2 is null)
        {
            eq1.AddRatio(gq1, gq2, ratio, qratio);
            knowledgeBase.IndexContinuedDict.Add(gq2, eq1);
            MakeNewKnowledge(eq1);
            ECUpdated(eq1);
        }
        else if (eq1 is null && eq2 is not null)
        {
            eq2.AddRatio(gq2, gq1, ratio.Invert(), qratio);
            knowledgeBase.IndexContinuedDict.Add(gq1, eq2);
            MakeNewKnowledge(eq2);
            ECUpdated(eq2);
        }
        else if (eq1 is not null && eq2 is not null)
        {
            if (eq1 != eq2)
            {
                if (eq1.index < eq2.index)
                {
                    eq1.CombineByRatio(eq2, gq1, gq2, ratio, qratio);
                    foreach (var item in eq1.CoffDict)
                    {
                        knowledgeBase.IndexContinuedDict[item.Key] = eq1;
                    }
                    knowledgeBase.EqualityChains[gq1.Unit].Remove(eq2);
                    MakeNewKnowledge(eq1);
                    ECUpdated(eq1);
                }
                else
                {
                    eq2.CombineByRatio(eq1, gq2, gq1, ratio.Invert(), qratio);
                    foreach (var item in eq2.CoffDict)
                    {
                        knowledgeBase.IndexContinuedDict[item.Key] = eq2;
                    }
                    knowledgeBase.EqualityChains[gq1.Unit].Remove(eq1);
                    MakeNewKnowledge(eq2);
                    ECUpdated(eq2);
                }
            }
            else
            {
                eq1.SetReason(gq1.ToString(), gq2.ToString(), qratio);
            }
        }
    }
    public virtual void AddLinerEquation(LinearEquation eq)
    {
        if (eq.Type == GeoEquationTypes.DistanceLinear)
        {
            DistanceLinearMatrix.AddEquation(eq.Coff, eq.Constant, eq);
            foreach (var item in DistanceProductionPairedInfos.Union(DistanceOtherPairedInfos).Union(OtherMixedPairedInfos))
            {
                EquationSystem eqSym = new EquationSystem();
                eqSym.Equations.Add(eq.ToString());
                eqSym.Equations.Add(item.EquationStr);
                eqSym.Conditions.Add(eq);
                eqSym.Conditions.Add(item.Equation);
                UpdateQuantityAndState(eqSym);
                if (eqSym.State != EqSymStates.NoValid  )
                {
                    TrySimplifyEqSym(eqSym);
                    UpdateQuantityAndState(eqSym);
                    FirstMove(eqSym);
                }
            }
            DistanceLinerPairedInfos.Add(new PairedEqInfo() { Equation = eq, EquationStr = eq.ToString(), Quantities = builder.GetEquationQuantities(eq.ToString()) });
        }
        else if (eq.Type == GeoEquationTypes.AngularLinear)
        {
            AngularLinearMatrix.AddEquation(eq.Coff, eq.Constant, eq);
            foreach (var item in AngularProductionPairedInfos.Union(AngularOtherPairedInfos).Union(OtherMixedPairedInfos))
            {
                EquationSystem eqSym = new EquationSystem();
                eqSym.Equations.Add(eq.ToString());
                eqSym.Equations.Add(item.EquationStr);
                eqSym.Conditions.Add(eq);
                eqSym.Conditions.Add(item.Equation);
                UpdateQuantityAndState(eqSym);
                if (eqSym.State != EqSymStates.NoValid )
                {
                    TrySimplifyEqSym(eqSym);
                    UpdateQuantityAndState(eqSym);
                    FirstMove(eqSym);
                }
                ang.Add(eqSym);
            }
            AngularLinerPairedInfos.Add(new PairedEqInfo() { Equation = eq, EquationStr = eq.ToString(), Quantities = builder.GetEquationQuantities(eq.ToString()) });
        }
    }
    public virtual void AddProductionEquation(ProductionEquation eq)
    {
        if (eq.Type == GeoEquationTypes.DistanceProduction)
        {
            DistanceProductionMatrix.AddEquation(eq.Coff, eq.Constant, eq);
            foreach (var item in DistanceLinerPairedInfos.Union(DistanceOtherPairedInfos).Union(OtherMixedPairedInfos))
            {
                EquationSystem eqSym = new EquationSystem();
                eqSym.Equations.Add(eq.ToString());
                eqSym.Equations.Add(item.EquationStr);
                eqSym.Conditions.Add(eq);
                eqSym.Conditions.Add(item.Equation);
                UpdateQuantityAndState(eqSym);
                if (eqSym.State != EqSymStates.NoValid )
                {
                    TrySimplifyEqSym(eqSym);
                    UpdateQuantityAndState(eqSym);
                    FirstMove(eqSym);
                }
            }

            DistanceProductionPairedInfos.Add(new PairedEqInfo() { Equation = eq, EquationStr = eq.ToString(), Quantities = builder.GetEquationQuantities(eq.ToString()) });
        }
        else if (eq.Type == GeoEquationTypes.AngularProduction)
        {
            AngularProductionMatrix.AddEquation(eq.Coff, eq.Constant, eq);
            foreach (var item in AngularLinerPairedInfos.Union(AngularOtherPairedInfos).Union(OtherMixedPairedInfos))
            {
                EquationSystem eqSym = new EquationSystem();
                eqSym.Equations.Add(eq.ToString());
                eqSym.Equations.Add(item.EquationStr);
                eqSym.Conditions.Add(eq);
                eqSym.Conditions.Add(item.Equation);
                UpdateQuantityAndState(eqSym);
                if (eqSym.State != EqSymStates.NoValid )
                {
                    TrySimplifyEqSym(eqSym);
                    UpdateQuantityAndState(eqSym);
                    FirstMove(eqSym);
                }
            }
            AngularProductionPairedInfos.Add(new PairedEqInfo() { Equation = eq, EquationStr = eq.ToString(), Quantities = builder.GetEquationQuantities(eq.ToString()) });
        }
    }
    public virtual void AddOtherEquation(Equation eq)
    {
        {
            EquationSystem eqSym = new EquationSystem();
            eqSym.Equations.Add(eq.ToString());
            eqSym.Conditions.Add(eq);
            UpdateQuantityAndState(eqSym);
            if (eqSym.State != EqSymStates.NoValid)
            {
                TrySimplifyEqSym(eqSym);
                UpdateQuantityAndState(eqSym);
                FirstMove(eqSym);
            }
        }
        if (eq.Type == GeoEquationTypes.DistanceOther)
        {
            foreach (var item in DistanceLinerPairedInfos.Union(DistanceProductionPairedInfos).Union(DistanceOtherPairedInfos).Union(OtherMixedPairedInfos))
            {
                EquationSystem eqSym = new EquationSystem();
                eqSym.Equations.Add(eq.ToString());
                eqSym.Equations.Add(item.EquationStr);
                eqSym.Conditions.Add(eq);
                eqSym.Conditions.Add(item.Equation);
                UpdateQuantityAndState(eqSym);
                if (eqSym.State != EqSymStates.NoValid )
                {
                    TrySimplifyEqSym(eqSym);
                    UpdateQuantityAndState(eqSym);
                    FirstMove(eqSym);
                }
            }
            DistanceOtherPairedInfos.Add(new PairedEqInfo() { Equation = eq, EquationStr = eq.ToString(), Quantities = builder.GetEquationQuantities(eq.ToString()) });
        }
        else if (eq.Type == GeoEquationTypes.AngularOther)
        {
            foreach (var item in AngularLinerPairedInfos.Union(AngularProductionPairedInfos).Union(AngularOtherPairedInfos).Union(OtherMixedPairedInfos))
            {
                EquationSystem eqSym = new EquationSystem();
                eqSym.Equations.Add(eq.ToString());
                eqSym.Equations.Add(item.EquationStr);
                eqSym.Conditions.Add(eq);
                eqSym.Conditions.Add(item.Equation);
                UpdateQuantityAndState(eqSym);
                if (eqSym.State != EqSymStates.NoValid)
                {
                    TrySimplifyEqSym(eqSym);
                    UpdateQuantityAndState(eqSym);
                    FirstMove(eqSym);
                }
                
            }
            AngularOtherPairedInfos.Add(new PairedEqInfo() { Equation = eq, EquationStr = eq.ToString(), Quantities = builder.GetEquationQuantities(eq.ToString()) });
        }
        else if (eq.Type == GeoEquationTypes.MixedOther)
        {
            foreach (var item in DistanceLinerPairedInfos.Union(DistanceProductionPairedInfos).Union(AngularLinerPairedInfos).Union(AngularProductionPairedInfos).Union(DistanceOtherPairedInfos).Union(AngularOtherPairedInfos).Union(OtherMixedPairedInfos))
            {
                EquationSystem eqSym = new EquationSystem();
                eqSym.Equations.Add(eq.ToString());
                eqSym.Equations.Add(item.EquationStr);
                eqSym.Conditions.Add(eq);
                eqSym.Conditions.Add(item.Equation);
                UpdateQuantityAndState(eqSym);
                if (eqSym.State != EqSymStates.NoValid)
                {
                    TrySimplifyEqSym(eqSym);
                    UpdateQuantityAndState(eqSym);
                    FirstMove(eqSym);
                }
            }
            OtherMixedPairedInfos.Add(new PairedEqInfo() { Equation = eq, EquationStr = eq.ToString(), Quantities = builder.GetEquationQuantities(eq.ToString()) });

        }
    }
    #endregion
    #region EqSym
    public virtual bool SolveEqSym(EquationSystem sym)
    {
        sym.State = EqSymStates.Tried;

        List<Knowledge> nk = [];
        var formulas = sym.Equations.Select(e => e.ToString()).ToList();
        foreach (var item in sym.Quantities)
        {
            if (item is GeoQuantity geoQuantity)
            {
                if (geoQuantity.Unit != QuantityClassifications.Cos && geoQuantity.Unit != QuantityClassifications.Tan)
                    formulas.Add($"{item}>0");
            }
            else
            {
                formulas.Add($"{item}>0");
            }
        }
        string command = "";
        string varDefine = "";
        string formularDefine = "";
        string formularToken = "";

        varDefine = StringTool.ComposeList(sym.Quantities);
        for (int i = 0; i < formulas.Count; i++)
        {
            formularDefine += $"a{i}:={formulas[i]};";
            formularToken += i == 0 ? $"a{i}" : $",a{i}";
        }
        command = $"{formularDefine}solve([{formularToken}],[{varDefine}])assuming real;";
        var result = mapleApp.Run(command);

        if (result == "")
        {
            return false;
        }
        result = result.Substring(1, result.Length - 2);
        result = result.Replace(" ", "");
        List<Dictionary<string, string>> output = new List<Dictionary<string, string>>();
        var matches = Regex.Matches(result, "\\[([\\s\\S]+?)\\]");
        if (matches.Count == 1)
        {
            foreach (Match match in matches)
            {
                bool conditionflag = false;
                var temp = new Dictionary<string, string>();
                foreach (var varNameValue in match.Groups[1].Value.Split(','))
                {
                    if (varNameValue.Contains("<="))
                    {
                        var l = varNameValue.Split("<=")[0];
                        var r = varNameValue.Split("<=")[1];
                        if (l != "0")
                        {
                            conditionflag = true; break;
                        }
                        continue;
                    }
                    else if (varNameValue.Contains("<"))
                    {
                        var l = varNameValue.Split("<")[0];
                        var r = varNameValue.Split("<")[1];
                        if (l != "0")
                        {
                            conditionflag = true; break;
                        }
                        continue;
                    }
                    else if (varNameValue.Contains(">="))
                    {
                        var l = varNameValue.Split(">=")[0];
                        var r = varNameValue.Split(">=")[1];
                        if (r != "0")
                        {
                            conditionflag = true; break;
                        }
                        continue;
                    }
                    else if (varNameValue.Contains(">"))
                    {
                        var l = varNameValue.Split(">")[0];
                        var r = varNameValue.Split(">")[1];
                        if (r != "0")
                        {
                            conditionflag = true; break;
                        }
                        continue;
                    }
                    if (!varNameValue.Contains('='))
                    { conditionflag = true; break; }
                    var varName = varNameValue.Split('=')[0];
                    var varValue = varNameValue.Split('=')[1];
                    if (temp.ContainsKey(varName)) continue;
                    temp.Add(varName, varValue);
                }
                if (!conditionflag)
                    output.Add(temp);
            }
        }
        foreach (var item in output)
        {
            foreach (var kv in item)
            {
                Equation equation = new Equation(kv.Key, kv.Value);
                var a = builder.ParseEq(equation.ToString());
                if (a is not null)
                {
                    if (a is QuantityRatio ratio)
                    {
                        if (ratio.Quantity1.Unit == ratio.Quantity2.Unit)
                        {
                            if (GeoInferenceApp.UseChinese)
                                a.AddReason("方程组求解");
                            else
                                a.AddReason("DiscoveredByEquationSystem");
                            sym.Conditions = sym.Conditions.Distinct().ToList();
                            a.AddCondition(sym.Conditions);
                            nk.Add(a);
                        }
                    }
                    else if (a is QuantityValue)
                    {
                        if (GeoInferenceApp.UseChinese)
                            a.AddReason("方程组求解");
                        else
                            a.AddReason("DiscoveredByEquationSystem");
                        sym.Conditions = sym.Conditions.Distinct().ToList();
                        a.AddCondition(sym.Conditions);
                        nk.Add(a);
                    }

                }
            }
        }
        if (nk.Count == 0)
        {
            TriedEqSyms.Add(sym);
            return false;
        }
        else
        {
            foreach (var item in nk)
            {
                updater.Add(item);
            }
            return true;
        }
    }
    public virtual void FirstMove(EquationSystem sym)
    {
        if (sym.State == EqSymStates.ReadyToTry)
        {
            ReadyToTryEqSyms.Add(sym);
        }
        else if (sym.State == EqSymStates.TooComplex)
        {
            TooComplexEqsyms.Add(sym);
        }
    }
    public virtual void Move(EquationSystem sym)
    {
        if (sym.State == EqSymStates.ReadyToTry)
        {
            TriedEqSyms.Remove(sym);
            ReadyToTryEqSyms.Remove(sym);
            TooComplexEqsyms.Remove(sym);
            ReadyToTryEqSyms.Add(sym);
        }
        else if (sym.State == EqSymStates.NoValid)
        {
            TriedEqSyms.Remove(sym);
            ReadyToTryEqSyms.Remove(sym);
            TooComplexEqsyms.Remove(sym);
        }
        else if (sym.State == EqSymStates.TooComplex)
        {
            TriedEqSyms.Remove(sym);
            ReadyToTryEqSyms.Remove(sym);
            TooComplexEqsyms.Remove(sym);
            TooComplexEqsyms.Add(sym);
        }
    }
    public virtual void Reset()
    {
        ReadyToTryEqSyms = ReadyToTryEqSyms.DistinctBy(sym => sym.Eqs).ToList();
        ReadyToTryEqSyms.Sort((a, b) => a.ToString().Count().CompareTo(b.ToString().Count()));
    }
    public virtual void SolveEqSym()
    {
        Reset();
        while (!App.IsActivedStop && ReadyToTryEqSyms.Count > 0)
        {
            var nextEqSym = ReadyToTryEqSyms[0];
            ReadyToTryEqSyms.RemoveAt(0);
            var b = SolveEqSym(nextEqSym);
            if (b)
                break;
        }
    }
    #endregion
    #region EC
    public virtual void MakeNewKnowledge(EqualityChain ec)
    {
        var a = ec.Inference();
        foreach (var item in a)
        {
            updater.Add(item);
        }
    }
    public virtual bool TrySimplifyEqSym(EquationSystem sym)
    {
        var isSimplified = false;
        sym.Quantities = sym.Quantities.Distinct().ToList();
        ZListDict<EqualityChain, Quantity> dict = [];
        foreach (var quantity in sym.Quantities)
        {
            if (knowledgeBase.IndexContinuedDict.ContainsKey(quantity))
            {
                dict[knowledgeBase.IndexContinuedDict[quantity]].Add(quantity);
            }
        }
        var a = false;
        var b = false;
        foreach (var kv in dict)
        {
            if (kv.Key.ActualValue is not null)
            {
                foreach (var kvp in kv.Value)
                {
                    for (int i = 0; i < sym.Equations.Count; i++)
                    {
                        sym.Conditions.Add(kv.Key.GetValueReason(kvp));
                        sym.Equations[i] = sym.Equations[i].Replace(kvp.ToString(), $"({kv.Key.ToValue(kvp)})");
                        sym.Quantities.Remove(kvp);
                        isSimplified = true;
                        
                    }
                }
            }
            else
            {
                for (int j = 0; j < sym.Equations.Count; j++)
                {
                    for (int i = 1; i < kv.Value.Count; i++)
                    {
                        sym.Conditions.Add(kv.Key.GetRatioReason(kv.Value[i], kv.Value[0]));
                        var ratio = kv.Key.CoffDict[kv.Value[0]] / kv.Key.CoffDict[kv.Value[i]];
                        sym.Equations[j] = sym.Equations[j].Replace(kv.Value[i].ToString(), $"(({ratio})*{kv.Value[0]})");
                        sym.Quantities.Remove(kv.Value[i]);
                        isSimplified = true;

                        if (j == 0)
                        {
                            a = true;
                        }
                        if (j == 1)
                        {
                            b = true;
                        }
                        
                    }
                }
            }
        }
        if (a & b)
            eqSysInfos.Add(sym);
        
        return isSimplified;
    }
    List<EquationSystem> eqSysInfos = [];
    List<EquationSystem> ang = [];
    public virtual void ECUpdated(EqualityChain ce)
    {
        ContinuedEqualityUpdated?.Invoke(ce);
        foreach (var eqSym in ReadyToTryEqSyms.ToArray())
        {
            if (eqSym.Quantities.Exists(ce.CoffDict.Keys.Contains))
            {
                if (TrySimplifyEqSym(eqSym))
                {
                    UpdateQuantityAndState(eqSym);
                    Move(eqSym);
                }
            }
        }
        foreach (var eqSym in TooComplexEqsyms.ToList())
        {
            if (eqSym.Quantities.Exists(ce.CoffDict.Keys.Contains))
            {
                if (TrySimplifyEqSym(eqSym))
                {
                    UpdateQuantityAndState(eqSym);
                    Move(eqSym);
                }
            }
        }
        foreach (var eqSym in TriedEqSyms.ToList())
        {
            if (eqSym.Quantities.Exists(ce.CoffDict.Keys.Contains))
            {
                if (TrySimplifyEqSym(eqSym))
                {
                    UpdateQuantityAndState(eqSym);
                    Move(eqSym);
                }
            }
        }

        Reset();
    }
    public virtual void UpdateQuantityAndState(EquationSystem sym)
    {
        if (sym.Equations.Count == 1)
        {
            sym.Quantities.Clear();
            sym.Quantities.AddRange(builder.GetEquationQuantities(sym.Equations[0]));

            sym.Quantities = sym.Quantities.Distinct().ToList();
            if (sym.Quantities.Count == 0)
                sym.State = EqSymStates.NoValid;
            else if (sym.Quantities.Count > 2)
                sym.State = EqSymStates.TooComplex;
            else
                sym.State = EqSymStates.ReadyToTry;
        }
        else if (sym.Equations.Count == 2)
        {
            sym.Quantities.Clear();
            var a = builder.GetEquationQuantities(sym.Equations[0]).Distinct();
            var b = builder.GetEquationQuantities(sym.Equations[1]).Distinct();
            sym.Quantities.Clear();
            sym.Quantities.AddRange(a.Union(b));
            if (sym.Quantities.Count == 0)
                sym.State = EqSymStates.NoValid;
            else if (!a.Where(b.Contains).Any())
                sym.State = EqSymStates.NoValid;
            else if (sym.Quantities.Count > 3)
                sym.State = EqSymStates.TooComplex;
            else
                sym.State = EqSymStates.ReadyToTry;
        }
        else
        {
            sym.State = EqSymStates.NoValid;
        }
    }
    #endregion
    #region Matrix
    public virtual void SolveMatrix()
    {
        {
            var cc = DistanceLinearMatrix.GetVariableNames();
            foreach (var item in knowledgeBase.EqualityChains[QuantityClassifications.Distance])
            {
                Dictionary<Quantity, Expr> CoffDict = [];
                if (item.CoffDict.Count == 1)
                {
                    CoffDict.Add(item.CoffDict.Keys.First(), item.CoffDict.Values.First());
                    var b = DistanceLinearMatrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, item.CoffDict.Keys.First(), item.Reasons.Select(r => r.reason));
                }
                else
                {
                    foreach (var kv in item.CoffDict.Skip(1))
                    {
                        if (cc.Contains(kv.Key))
                        {
                            CoffDict.Add(kv.Key, kv.Value);
                        }
                    }
                    if (CoffDict.Count > 0)
                    {
                        CoffDict.Add(item.CoffDict.Keys.First(), item.CoffDict.Values.First());
                        var b = DistanceLinearMatrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, item.CoffDict.Keys.First(), item.Reasons.Select(r => r.reason));
                    }
                }
            }
            var nks1 = DistanceLinearMatrix.DiscoverRelations();
            QuantityEquationMatrixUpdated?.Invoke(DistanceLinearMatrix);
            foreach (var item in nks1)
            {
                updater.Add(item);
            }
        }
        {
            var cc = AngularLinearMatrix.GetVariableNames();
            foreach (var item in knowledgeBase.EqualityChains[QuantityClassifications.Angle])
            {
                Dictionary<Quantity, Expr> CoffDict = [];
                if (item.CoffDict.Count == 1)
                {
                    CoffDict.Add(item.CoffDict.Keys.First(), item.CoffDict.Values.First());
                    var b = AngularLinearMatrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, item.CoffDict.Keys.First(), item.Reasons.Select(r => r.reason));
                }
                else
                {
                    foreach (var kv in item.CoffDict.Skip(1))
                    {
                        if (cc.Contains(kv.Key))
                        {
                            CoffDict.Add(kv.Key, kv.Value);
                        }
                    }
                    if (CoffDict.Count > 0)
                    {
                        CoffDict.Add(item.CoffDict.Keys.First(), item.CoffDict.Values.First());
                        var b = AngularLinearMatrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, item.CoffDict.Keys.First(), item.Reasons.Select(r => r.reason));
                    }
                }
                
            }
            var nks1 = AngularLinearMatrix.DiscoverRelations();
            if (Environment.GetEnvironmentVariable("GEO_DIAG_MATRIX") == "1")
            {
                Console.WriteLine("[DiagMatrix] ==== Angular RREF ====");
                Console.WriteLine(AngularLinearMatrix.GetCompleteMatrixString());
                Console.WriteLine("[DiagMatrix] ==== Discovered ====");
                foreach (var item in nks1) Console.WriteLine("[DiagMatrix]   " + item);
            }
            QuantityEquationMatrixUpdated?.Invoke(AngularLinearMatrix);
            foreach (var item in nks1)
            {
                updater.Add(item);
            }
        }
        {
            var cc = DistanceProductionMatrix.GetVariableNames();
            foreach (var item in knowledgeBase.EqualityChains[QuantityClassifications.Distance])
            {
                Dictionary<Quantity, Expr> CoffDict = [];
                foreach (var kv in item.CoffDict.Skip(1))
                {
                    if (cc.Contains(kv.Key))
                    {
                        CoffDict.Add(kv.Key, kv.Value);
                    }
                }
                var baseQuantity = item.CoffDict.Keys.First();
                if (CoffDict.Count > 0 || cc.Contains(baseQuantity))
                {
                    CoffDict.Add(baseQuantity, item.CoffDict[baseQuantity]);
                    var b = DistanceProductionMatrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, baseQuantity, item.Reasons.Select(r => r.reason));
                }
            }
            foreach (var item in knowledgeBase.EqualityChains[QuantityClassifications.Sin])
            {
                Dictionary<Quantity, Expr> CoffDict = [];
                foreach (var kv in item.CoffDict.Skip(1))
                {
                    if (cc.Contains(kv.Key))
                    {
                        CoffDict.Add(kv.Key, kv.Value);
                    }
                }
                var baseQuantity = item.CoffDict.Keys.First();
                if (CoffDict.Count > 0 || cc.Contains(baseQuantity))
                {
                    CoffDict.Add(baseQuantity, item.CoffDict[baseQuantity]);
                    var b = DistanceProductionMatrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, baseQuantity, item.Reasons.Select(r => r.reason));
                }
            }
            foreach (var item in knowledgeBase.EqualityChains[QuantityClassifications.Cos])
            {
                Dictionary<Quantity, Expr> CoffDict = [];
                foreach (var kv in item.CoffDict.Skip(1))
                {
                    if (cc.Contains(kv.Key))
                    {
                        CoffDict.Add(kv.Key, kv.Value);
                    }
                }
                var baseQuantity = item.CoffDict.Keys.First();
                if (CoffDict.Count > 0 || cc.Contains(baseQuantity))
                {
                    CoffDict.Add(baseQuantity, item.CoffDict[baseQuantity]);
                    var b = DistanceProductionMatrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, baseQuantity, item.Reasons.Select(r => r.reason));
                }
            }
            foreach (var item in knowledgeBase.EqualityChains[QuantityClassifications.Tan])
            {
                Dictionary<Quantity, Expr> CoffDict = [];
                foreach (var kv in item.CoffDict.Skip(1))
                {
                    if (cc.Contains(kv.Key))
                    {
                        CoffDict.Add(kv.Key, kv.Value);
                    }
                }
                var baseQuantity = item.CoffDict.Keys.First();
                if (CoffDict.Count > 0 || cc.Contains(baseQuantity))
                {
                    CoffDict.Add(baseQuantity, item.CoffDict[baseQuantity]);
                    var b = DistanceProductionMatrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, baseQuantity, item.Reasons.Select(r => r.reason));
                }
            }
            var nks1 = DistanceProductionMatrix.DiscoverRelations();
            foreach (var item in nks1)
            {
                updater.Add(item);
            }
        }
        {
            var cc = AngularProductionMatrix.GetVariableNames();
            foreach (var item in knowledgeBase.EqualityChains[QuantityClassifications.Angle])
            {
                Dictionary<Quantity, Expr> CoffDict = [];
                foreach (var kv in item.CoffDict.Skip(1))
                {
                    if (cc.Contains(kv.Key))
                    {
                        CoffDict.Add(kv.Key, kv.Value);
                    }
                }
                var baseQuantity = item.CoffDict.Keys.First();
                if (CoffDict.Count > 0 || cc.Contains(baseQuantity))
                {
                    CoffDict.Add(baseQuantity, item.CoffDict[baseQuantity]);
                    var b = AngularProductionMatrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, baseQuantity, item.Reasons.Select(r => r.reason));
                }
            }
            var nks1 = AngularProductionMatrix.DiscoverRelations();
            foreach (var item in nks1)
            {
                updater.Add(item);
            }
        }
        MatrixInferenced?.Invoke();
    }
    #endregion
    #region Checkpoint
    /// <summary>
    /// </summary>
    public class EquationSolverState
    {
        public int AngularLinerCount { get; set; }
        public int AngularOtherCount { get; set; }
        public int AngularProductionCount { get; set; }
        public int DistanceLinerCount { get; set; }
        public int DistanceOtherCount { get; set; }
        public int DistanceProductionCount { get; set; }
        public int OtherMixedCount { get; set; }
        public List<EquationSystem> ReadyToTry { get; set; } = [];
        public List<EquationSystem> Tried { get; set; } = [];
        public List<EquationSystem> TooComplex { get; set; } = [];
        public List<EquationSystem> EqSysInfos { get; set; } = [];
        public List<EquationSystem> Ang { get; set; } = [];
        public Dictionary<QuantityClassifications, List<EqualityChain>> Chains { get; set; } = [];
    }

    /// <summary>
    /// </summary>
    public virtual EquationSolverState CaptureState()
    {
        return new EquationSolverState
        {
            AngularLinerCount = AngularLinerPairedInfos.Count,
            AngularOtherCount = AngularOtherPairedInfos.Count,
            AngularProductionCount = AngularProductionPairedInfos.Count,
            DistanceLinerCount = DistanceLinerPairedInfos.Count,
            DistanceOtherCount = DistanceOtherPairedInfos.Count,
            DistanceProductionCount = DistanceProductionPairedInfos.Count,
            OtherMixedCount = OtherMixedPairedInfos.Count,
            ReadyToTry = ReadyToTryEqSyms.Select(s => s.Clone()).ToList(),
            Tried = TriedEqSyms.Select(s => s.Clone()).ToList(),
            TooComplex = TooComplexEqsyms.Select(s => s.Clone()).ToList(),
            EqSysInfos = eqSysInfos.Select(s => s.Clone()).ToList(),
            Ang = ang.Select(s => s.Clone()).ToList(),
            Chains = knowledgeBase.EqualityChains.ToDictionary(
                kv => kv.Key,
                kv => kv.Value.Select(c => c.Clone()).ToList()),
        };
    }

    /// <summary>
    /// </summary>
    public virtual void RestoreState(EquationSolverState state)
    {
        TruncateList(AngularLinerPairedInfos, state.AngularLinerCount);
        TruncateList(AngularOtherPairedInfos, state.AngularOtherCount);
        TruncateList(AngularProductionPairedInfos, state.AngularProductionCount);
        TruncateList(DistanceLinerPairedInfos, state.DistanceLinerCount);
        TruncateList(DistanceOtherPairedInfos, state.DistanceOtherCount);
        TruncateList(DistanceProductionPairedInfos, state.DistanceProductionCount);
        TruncateList(OtherMixedPairedInfos, state.OtherMixedCount);

        ReadyToTryEqSyms = state.ReadyToTry.Select(s => s.Clone()).ToList();
        TriedEqSyms = state.Tried.Select(s => s.Clone()).ToList();
        TooComplexEqsyms = state.TooComplex.Select(s => s.Clone()).ToList();
        eqSysInfos = state.EqSysInfos.Select(s => s.Clone()).ToList();
        ang = state.Ang.Select(s => s.Clone()).ToList();

        foreach (var unit in knowledgeBase.EqualityChains.Keys.ToList())
        {
            var list = knowledgeBase.EqualityChains[unit];
            list.Clear();
            if (state.Chains.TryGetValue(unit, out var chains))
            {
                list.AddRange(chains.Select(c => c.Clone()));
            }
        }
        knowledgeBase.IndexContinuedDict.Clear();
        foreach (var list in knowledgeBase.EqualityChains.Values)
        {
            foreach (var chain in list)
            {
                foreach (var quantity in chain.CoffDict.Keys)
                {
                    knowledgeBase.IndexContinuedDict[quantity] = chain;
                }
            }
        }

        RebuildMatrices();
    }

    static void TruncateList<T>(List<T> list, int count)
    {
        if (list.Count > count)
            list.RemoveRange(count, list.Count - count);
    }

    protected virtual MapleBaseLinearMatrix CreateLinearMatrix() => new();
    protected virtual MapleBaseProductionMatrix CreateProductionMatrix() => new();

    void RebuildMatrices()
    {
        var oldDistanceLinear = DistanceLinearMatrix;
        var oldAngularLinear = AngularLinearMatrix;
        var oldDistanceProduction = DistanceProductionMatrix;
        var oldAngularProduction = AngularProductionMatrix;

        DistanceLinearMatrix = RebuildLinearMatrix(DistanceLinerPairedInfos, QuantityClassifications.Distance);
        AngularLinearMatrix = RebuildLinearMatrix(AngularLinerPairedInfos, QuantityClassifications.Angle);
        DistanceProductionMatrix = RebuildProductionMatrix(DistanceProductionPairedInfos, QuantityClassifications.Distance);
        AngularProductionMatrix = RebuildProductionMatrix(AngularProductionPairedInfos, QuantityClassifications.Angle);

        ReleaseMatrix(oldDistanceLinear);
        ReleaseMatrix(oldAngularLinear);
        ReleaseMatrix(oldDistanceProduction);
        ReleaseMatrix(oldAngularProduction);
    }

    MapleBaseLinearMatrix RebuildLinearMatrix(List<PairedEqInfo> infos, QuantityClassifications unit)
    {
        var matrix = CreateLinearMatrix();
        foreach (var info in infos)
        {
            if (info.Equation is LinearEquation le)
                matrix.AddEquation(le.Coff, le.Constant, le);
        }
        ReplayLinearReductions(matrix, unit);
        return matrix;
    }

    MapleBaseProductionMatrix RebuildProductionMatrix(List<PairedEqInfo> infos, QuantityClassifications unit)
    {
        var matrix = CreateProductionMatrix();
        foreach (var info in infos)
        {
            if (info.Equation is ProductionEquation pe)
                matrix.AddEquation(pe.Coff, pe.Constant, pe);
        }
        ReplayProductionReductions(matrix, unit);
        return matrix;
    }

    /// <summary>
    /// </summary>
    protected virtual void ReplayLinearReductions(MapleBaseLinearMatrix matrix, QuantityClassifications unit)
    {
        var matrixVars = matrix.GetVariableNames();
        foreach (var item in knowledgeBase.EqualityChains[unit])
        {
            Dictionary<Quantity, Expr> CoffDict = [];
            if (item.CoffDict.Count == 1)
            {
                CoffDict.Add(item.CoffDict.Keys.First(), item.CoffDict.Values.First());
                matrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, item.CoffDict.Keys.First(), item.Reasons.Select(r => r.reason));
            }
            else
            {
                foreach (var kv in item.CoffDict.Skip(1))
                {
                    if (matrixVars.Contains(kv.Key))
                    {
                        CoffDict.Add(kv.Key, kv.Value);
                    }
                }
                if (CoffDict.Count > 0)
                {
                    CoffDict.Add(item.CoffDict.Keys.First(), item.CoffDict.Values.First());
                    matrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, item.CoffDict.Keys.First(), item.Reasons.Select(r => r.reason));
                }
            }
        }
    }

    /// <summary>
    /// </summary>
    protected virtual void ReplayProductionReductions(MapleBaseProductionMatrix matrix, QuantityClassifications matrixUnit)
    {
        var matrixVars = matrix.GetVariableNames();
        var units = matrixUnit == QuantityClassifications.Angle
            ? new[] { QuantityClassifications.Angle }
            : new[]
            {
                QuantityClassifications.Distance,
                QuantityClassifications.Sin,
                QuantityClassifications.Cos,
                QuantityClassifications.Tan
            };
        foreach (var unit in units)
        {
            foreach (var item in knowledgeBase.EqualityChains[unit])
            {
                Dictionary<Quantity, Expr> CoffDict = [];
                foreach (var kv in item.CoffDict.Skip(1))
                {
                    if (matrixVars.Contains(kv.Key))
                    {
                        CoffDict.Add(kv.Key, kv.Value);
                    }
                }
                var baseQuantity = item.CoffDict.Keys.First();
                if (CoffDict.Count > 0 || matrixVars.Contains(baseQuantity))
                {
                    CoffDict.Add(baseQuantity, item.CoffDict[baseQuantity]);
                    matrix.ReduceColumnsByContinuedEquality(CoffDict, item.ActualValue, baseQuantity, item.Reasons.Select(r => r.reason));
                }
            }
        }
    }

    protected virtual void ReleaseMatrix(MapleBaseLinearMatrix matrix)
    {
        if (matrix is null) return;
        try { matrix.RunCommand($"unassign('{matrix.MatrixName}'):"); } catch { }
    }

    protected virtual void ReleaseMatrix(MapleBaseProductionMatrix matrix)
    {
        if (matrix is null) return;
        try { matrix.RunCommand($"unassign('{matrix.MatrixName}'):"); } catch { }
    }
    #endregion
}