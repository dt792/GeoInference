
public class EquationSolverNoCompassMatrix : EquationSolver
{
    #region Update
    public override void AddQV(QuantityValue qvalue)
    {
        if (qvalue.Quantity.Unit == QuantityClassifications.Angle)
        {
            AngularLinearMatrix.AddEquation(new() { { qvalue.Quantity, 1 } }, qvalue.Expr, qvalue);
            if (qvalue.Expr != 0)
                AngularProductionMatrix.AddEquation(new() { { qvalue.Quantity, 1 } }, qvalue.Expr);
        }
        else
        {
            DistanceLinearMatrix.AddEquation(new() { { qvalue.Quantity, 1 } }, qvalue.Expr, qvalue);
            if (qvalue.Expr != 0)
                DistanceProductionMatrix.AddEquation(new() { { qvalue.Quantity, 1 } }, qvalue.Expr);
        }

        
        if (qvalue.Expr != 0)
        {
            var valueeq = knowledgeBase.EqualityChains[qvalue.Quantity.Unit].FirstOrDefault(eq => eq.ActualValue is not null);
            
            if (valueeq is not null)
            {
                if (knowledgeBase.IndexContinuedDict.ContainsKey(qvalue.Quantity))
                {
                    var hadeq = knowledgeBase.IndexContinuedDict[qvalue.Quantity];
                    if (valueeq != hadeq)
                    {
                        hadeq.AddValue(qvalue);
                        if (valueeq.index < hadeq.index)
                        {
                            valueeq.CombineByValue(hadeq);
                            foreach (var item in hadeq.CoffDict)
                            {
                                knowledgeBase.IndexContinuedDict[item.Key] = valueeq;
                            }
                            knowledgeBase.EqualityChains[hadeq.Unit].Remove(hadeq);
                            MakeNewKnowledge(hadeq);
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
                            MakeNewKnowledge(valueeq);
                            ECUpdated(hadeq);
                        }

                    }
                }
                else
                {
                    valueeq.AddValue(qvalue);
                    knowledgeBase.IndexContinuedDict[qvalue.Quantity] = valueeq;
                    MakeNewKnowledge(knowledgeBase.IndexContinuedDict[qvalue.Quantity]);
                    ECUpdated(valueeq);
                }
            }
            
            else
            {
                valueeq = new EqualityChain();
                valueeq.AddValue(qvalue);
                knowledgeBase.EqualityChains[qvalue.Quantity.Unit].Add(valueeq);
                knowledgeBase.IndexContinuedDict[qvalue.Quantity] = valueeq;
                MakeNewKnowledge(knowledgeBase.IndexContinuedDict[qvalue.Quantity]);
                ECUpdated(valueeq);
            }
        }
    }
    public override void AddQR(QuantityRatio qratio)
    {
        if (qratio.Quantity1.Unit != qratio.Quantity2.Unit)
            return;
        
        if (qratio.Quantity1.Unit == QuantityClassifications.Angle)
        {
            AngularLinearMatrix.AddEquation(new() { { qratio.Quantity1, 1 }, { qratio.Quantity2, qratio.Ratio.Opposite() } }, 0, qratio);
            AngularProductionMatrix.AddEquation(new() { { qratio.Quantity1, 1 }, { qratio.Quantity2, -1 } }, qratio.Ratio);
        }
        else
        {
            DistanceLinearMatrix.AddEquation(new() { { qratio.Quantity1, 1 }, { qratio.Quantity2, qratio.Ratio.Opposite() } }, 0, qratio);
            DistanceProductionMatrix.AddEquation(new() { { qratio.Quantity1, 1 }, { qratio.Quantity2, -1 } }, qratio.Ratio);
        }
        
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
    #endregion

    #region Matrix
    public override void SolveMatrix()
    {
        {
            var nks1 = DistanceLinearMatrix.DiscoverRelations();
            QuantityEquationMatrixUpdated?.Invoke(DistanceLinearMatrix);
            foreach (var item in nks1)
            {
                updater.Add(item);
            }
        }
        {
            var nks1 = AngularLinearMatrix.DiscoverRelations();
            QuantityEquationMatrixUpdated?.Invoke(AngularLinearMatrix);
            foreach (var item in nks1)
            {
                updater.Add(item);
            }
        }
        {
            var nks1 = DistanceProductionMatrix.DiscoverRelations();
            foreach (var item in nks1)
            {
                updater.Add(item);
            }
        }
        {
            var nks1 = AngularProductionMatrix.DiscoverRelations();
            foreach (var item in nks1)
            {
                updater.Add(item);
            }
        } 
}
    #endregion
}
