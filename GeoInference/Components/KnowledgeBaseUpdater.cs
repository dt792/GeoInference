public class KnowledgeBaseUpdater
{
    [DI] protected KnowledgeBase knowledgeBase;
    [DI] protected TargetBase targetBase;

    #region Overall Processing
    public Action KnowledgeAdding { get; set; }
    public Action<Predicate> PredicateAdded { get; set; }
    public Action<Equation> EquationAdded { get; set; }

    public void AddConditionalKnowledgePair(ConditionalKnowledge knowledge)
    {
        Add(knowledge);
    }

    public virtual Knowledge Add(Knowledge knowledge)
    {
        KnowledgeAdding?.Invoke();
        if (!knowledge.IsAvailable)
            return null;
        if (knowledge is Predicate pred)
            return Add(pred);
        else if (knowledge is QuantityValue qv)
            return Add(qv);
        else if (knowledge is QuantityRatio qr)
            return Add(qr);
        else if (knowledge is LinearEquation le)
            return Add(le);
        else if (knowledge is ProductionEquation pe)
            return Add(pe);
        else if (knowledge is Equation eq)
            return Add(eq);
        else
            throw new Exception("Unknown knowledge type");
    }

    public virtual Knowledge Add(Predicate pred)
    {
        KnowledgeAdding?.Invoke();
        if (knowledgeBase.Predicates.TryGetValue(pred.HashCode, out var existing))
        {
            return existing;
        }

        if (pred is Line line)
            return Add(line);
        else if (pred is Angle angle)
            return Add(angle);
        else if (pred is Circle circle)
            return Add(circle);

        return AddPredicateBase(pred);
    }
    #endregion

    #region General Addition
    public virtual Predicate AddPredicateBase(Predicate pred)
    {
        if (knowledgeBase.Predicates.TryGetValue(pred.HashCode, out var existing))
            return existing;

        _add(pred);
        if (pred is Figure f)
            MakeGeoQuantity(f);
        _specialSizeConvert(pred);
        knowledgeBase.NewKnowledges.Add(pred);
        KnowledgeTimeline.Record();
        return pred;
    }

    public void TryAddGeoQuantity(GeoQuantity quantity)
    {
        var key = quantity.ToString();
        if (!knowledgeBase.Quantities.ContainsKey(key))
        {
            knowledgeBase.Quantities.Add(key, quantity);
        }
    }

    void _specialSizeConvert(Knowledge value)
    {
        if (value is AngleSize asize)
        {
            var exprStr = asize.Expr.ToString();
            if (Expr.SizeToCosSpecialValues.TryGetValue(exprStr, out var cosValue))
            {
                QuantityValue size = new QuantityValue(asize.Angle.Cos, cosValue);
                size.AddCondition(value.Conditions);
                size.Reason = value.Reason;
                Add(size);
            }
            if (Expr.SizeToSinSpecialValues.TryGetValue(exprStr, out var sinValue))
            {
                QuantityValue size = new QuantityValue(asize.Angle.Sin, sinValue);
                size.AddCondition(value.Conditions);
                size.Reason = value.Reason;
                Add(size);
            }
            if (Expr.SizeToTanSpecialValues.TryGetValue(exprStr, out var tanValue))
            {
                QuantityValue size = new QuantityValue(asize.Angle.Tan, tanValue);
                size.AddCondition(value.Conditions);
                size.Reason = value.Reason;
                Add(size);
            }
        }
        else if (value is AngleCos acos)
        {
            var exprStr = acos.Expr.ToString();
            if (Expr.CosToSizeSpecialValues.TryGetValue(exprStr, out var sizeValue))
            {
                AngleSize size = new AngleSize(acos.Angle, sizeValue);
                size.AddCondition(value.Conditions);
                size.Reason = value.Reason;
                Add(size);

                QuantityValue qv = new QuantityValue(acos.Angle.Size, sizeValue);
                qv.AddCondition(value.Conditions);
                qv.Reason = value.Reason;
                Add(qv);
            }
            AngleSin sin = new AngleSin(acos.Angle, (1 - acos.Expr.Pow(2)).Sqrt());
            sin.AddCondition(value.Conditions);
            sin.Reason = value.Reason;
            Add(sin);
        }
    }

    public void MakeGeoQuantity(Figure figure)
    {
        if (figure is Segment seg)
        {
            knowledgeBase.Quantities.Add(seg.ToString(), seg.Length);
            TryAddGeoQuantity(seg.Length);
        }
        else if (figure is Arc arc)
        {
            TryAddGeoQuantity(arc.MajorArcLength);
            TryAddGeoQuantity(arc.MinorArcLength);
            TryAddGeoQuantity(arc.Size);
        }
        else if (figure is Angle angle)
        {
            knowledgeBase.Quantities.Add(angle.ToString(), angle.Size);
            TryAddGeoQuantity(angle.Size);
            TryAddGeoQuantity(angle.Sin);
            TryAddGeoQuantity(angle.Cos);
            TryAddGeoQuantity(angle.Tan);
        }
        else if (figure.GetType() == typeof(Triangle))
        {
            TryAddGeoQuantity(((Triangle)figure).Area);
            TryAddGeoQuantity(((Triangle)figure).Perimeter);
        }
        else if (figure.GetType() == typeof(Quadrilateral))
        {
            TryAddGeoQuantity(((Quadrilateral)figure).Area);
            TryAddGeoQuantity(((Quadrilateral)figure).Perimeter);
        }
        else if (figure.GetType() == typeof(Sector))
        {
            TryAddGeoQuantity(((Sector)figure).Area);
            TryAddGeoQuantity(((Sector)figure).Perimeter);
        }
        else if (figure.GetType() == typeof(Circle))
        {
            TryAddGeoQuantity(((Circle)figure).Area);
            TryAddGeoQuantity(((Circle)figure).Perimeter);
            TryAddGeoQuantity(((Circle)figure).Radius);
            TryAddGeoQuantity(((Circle)figure).Diameter);
        }
    }

    public Predicate _add(Predicate pred)
    {
        knowledgeBase.Predicates.Add(pred.HashCode, pred);

        var categoryType = pred.GetType();
        if (!knowledgeBase.Categories.TryGetValue(categoryType, out var categoryInfo))
        {
            categoryInfo = new List<Predicate>() { };
            knowledgeBase.Categories.Add(categoryType, categoryInfo);
        }
        pred.PosIndex = (uint)categoryInfo.Count;
        categoryInfo.Add(pred);
        PredicateAdded?.Invoke(pred);
        return pred;
    }

    public virtual Knowledge Add(QuantityValue qv)
    {
        KnowledgeAdding?.Invoke();
        if (!qv.IsAvailable)
            return null;
        if (knowledgeBase.QuantityValues.TryGetValue(qv.HashCode, out var existing))
        {
            return existing;
        }
        else
        {
            qv.PosIndex = (uint)knowledgeBase.QuantityValues.Count;
            knowledgeBase.QuantityValues.Add(qv.HashCode, qv);
            knowledgeBase.NewKnowledges.Add(qv);
            KnowledgeTimeline.Record();

            EquationAdded?.Invoke(qv);
            var p = qv.ToPred();
            if (p is not null)
                Add(p);
            return qv;
        }
    }

    public virtual Knowledge Add(QuantityRatio qr)
    {
        KnowledgeAdding?.Invoke();
        if (!qr.IsAvailable)
            return null;
        if (knowledgeBase.QuantityRatios.TryGetValue(qr.HashCode, out var existing))
        {
            return existing;
        }
        else
        {
            qr.PosIndex = (uint)knowledgeBase.QuantityRatios.Count;
            knowledgeBase.QuantityRatios.Add(qr.HashCode, qr);
            knowledgeBase.NewKnowledges.Add(qr);
            KnowledgeTimeline.Record();
            EquationAdded?.Invoke(qr);
            var p = qr.ToPred();
            if (p is not null)
                Add(p);
            EquationAdded?.Invoke(qr);
            return qr;
        }
    }

    public virtual Knowledge Add(LinearEquation linerEquation)
    {
        KnowledgeAdding?.Invoke();
        if (!knowledgeBase.Equations.ContainsKey(linerEquation.HashCode))
        {
            linerEquation.PosIndex = (uint)knowledgeBase.Equations.Count;
            knowledgeBase.LinearEquations.Add(linerEquation.HashCode, linerEquation);
            knowledgeBase.Equations.Add(linerEquation.HashCode, linerEquation);
            knowledgeBase.NewKnowledges.Add(linerEquation);
            KnowledgeTimeline.Record();
            EquationAdded?.Invoke(linerEquation);
            _supplementaryTrigConvert(linerEquation);
        }
        return linerEquation;
    }

    /// <summary>
    /// </summary>
    void _supplementaryTrigConvert(LinearEquation eq)
    {
        if (eq.Coff.Count != 2 || eq.Constant != 180) return;
        var items = eq.Coff.ToList();
        if (items[0].Value != 1 || items[1].Value != 1) return;
        if (items[0].Key is not GeoQuantity q1 || items[1].Key is not GeoQuantity q2) return;
        if (q1.PropName != Quantity.Size || q2.PropName != Quantity.Size) return;
        if (q1.Figures?.FirstOrDefault() is not Angle a1 || q2.Figures?.FirstOrDefault() is not Angle a2) return;

        void AddRatio(Quantity x, Quantity y, Expr ratio)
        {
            QuantityRatio pred = new QuantityRatio(x, y, ratio);
            pred.Reason = "互补角的三角函数关系";
            pred.AddCondition(eq);
            Add(pred);
        }
        AddRatio(a1.Cos, a2.Cos, -1);
        AddRatio(a1.Sin, a2.Sin, 1);
        AddRatio(a1.Tan, a2.Tan, -1);
    }

    public virtual Knowledge Add(ProductionEquation productionEquation)
    {
        KnowledgeAdding?.Invoke();
        if (!knowledgeBase.Equations.ContainsKey(productionEquation.HashCode))
        {
            productionEquation.PosIndex = (uint)knowledgeBase.Equations.Count;
            knowledgeBase.ProductionEquations.Add(productionEquation.HashCode, productionEquation);
            knowledgeBase.Equations.Add(productionEquation.HashCode, productionEquation);
            knowledgeBase.NewKnowledges.Add(productionEquation);
            KnowledgeTimeline.Record();
            EquationAdded?.Invoke(productionEquation);
        }
        return productionEquation;
    }

    public virtual Knowledge Add(Equation equation)
    {
        KnowledgeAdding?.Invoke();
        if (!knowledgeBase.Equations.ContainsKey(equation.HashCode))
        {
            equation.PosIndex = (uint)knowledgeBase.Equations.Count;
            knowledgeBase.Equations.Add(equation.HashCode, equation);
            knowledgeBase.NewKnowledges.Add(equation);
            KnowledgeTimeline.Record();
            EquationAdded?.Invoke(equation);
        }
        return equation;
    }

    public void Add(ConditionalKnowledge ck)
    {
        KnowledgeAdding?.Invoke();
        var diagTs = SemiRuleDiagnostics.OnCkAddStart();
        if (knowledgeBase.ConditionalKnowledgePairDict.TryGetValue(ck.Knowledge.HashCode, out var bucket))
        {
            foreach (var item in bucket)
            {
                SemiRuleDiagnostics.OnCkDedupCompare();
                if (item.Targets.Count == ck.Targets.Count)
                {
                    bool flag = true;
                    foreach (var kv in ck.Targets)
                    {
                        if (!item.Targets.TryGetValue(kv.Key, out var itemTarget))
                        {
                            flag = false;
                        }
                        else
                        {
                            if (kv.Value.Expr is not null)
                            {
                                if (itemTarget.Expr != kv.Value.Expr)
                                {
                                    flag = false;
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        SemiRuleDiagnostics.OnCkDedupEnd(diagTs);
                        return;
                    }
                }
            }
        }
        SemiRuleDiagnostics.OnCkDedupEnd(diagTs);
        knowledgeBase.ConditionalKnowledgePairs.Add(ck);
        if (bucket is null)
        {
            bucket = [];
            knowledgeBase.ConditionalKnowledgePairDict.Add(ck.Knowledge.HashCode, bucket);
        }
        bucket.Add(ck);
        foreach (var item in ck.ConditionDict)
        {
            if (knowledgeBase.Predicates.TryGetValue(item.Key, out var existingPred))
            {
                var target = ck.Targets[item.Key];
                if (target.Expr is not null)
                {
                    if (target.Expr == existingPred.Expr)
                    {
                        ck.ConditionDict[item.Key] = existingPred;
                    }
                }
                else
                {
                    ck.ConditionDict[item.Key] = existingPred;
                }
            }
        }
        ck.Check();
        if (ck.IsEstablish)
        {
            SemiRuleDiagnostics.OnCkEstablished();
            Add(ck.Knowledge);
        }
        else
        {
            SemiRuleDiagnostics.OnCkWaiting();
            foreach (var item in ck.ConditionDict)
            {
                if (item.Value is null)
                    if (knowledgeBase.InversedConditionalKnowledgePairDict.TryGetValue(item.Key, out var ckList))
                    {
                        ckList.Add(ck);
                    }
                    else
                    {
                        knowledgeBase.InversedConditionalKnowledgePairDict.Add(item.Key, [ck]);
                    }
            }
        }
    }
    #endregion

    #region Merge
    public void MergeFigure(List<Knowledge> knowledges)
    {
        var lines = knowledges.Where(p => p is Line).Select(p => (Line)p).ToList();
        var segments = knowledges.Where(p => p is Segment).Select(p => (Segment)p).ToList();
        foreach (var seg in segments)
        {
            Add(seg);
        }
        foreach (var line in lines)
        {
            var canonical = (Line)Add(line);
            for (int i = 0; i < canonical.Properties.Count; i++)
            {
                for (int j = i + 1; j < canonical.Properties.Count; j++)
                {
                    Segment seg = new Segment((Point)canonical[i], (Point)canonical[j]);
                    Add(seg);
                }
            }
        }

        foreach (var point in knowledgeBase.Categories[typeof(Point)])
        {
            foreach (var item in knowledgeBase.Categories[typeof(Line)])
            {
                GeoQuantity quantity = new GeoQuantity([(Figure)point, (Figure)item], GeoQuantity.Distance);
                TryAddGeoQuantity(quantity);
            }
        }
        foreach (var line in knowledgeBase.Categories[typeof(Line)])
        {
            foreach (var item in knowledgeBase.Categories[typeof(Line)])
            {
                if (ReferenceEquals(line, item)) continue;
                GeoQuantity quantity = new GeoQuantity([(Figure)line, (Figure)item], GeoQuantity.Distance);
                TryAddGeoQuantity(quantity);
            }
        }
    }

    public (List<T> intersection, List<T> left1, List<T> left2) FindIntersection<T>(List<T> pms1, List<T> pms2) where T : Predicate
    {
        HashSet<T> set1 = new(pms1);
        HashSet<T> set2 = new(pms2);
        List<T> intersection = [];
        List<T> left1 = [];
        List<T> left2 = [];
        foreach (var p in pms1)
        {
            if (set2.Contains(p))
                intersection.Add(p);
            else
                left1.Add(p);
        }
        foreach (var p in pms2)
        {
            if (!set1.Contains(p))
                left2.Add(p);
        }
        return (intersection, left1, left2);
    }

    void UpdateTargetBase(Predicate newKnowledge, Predicate[] oldKnowledges)
    {
        HashSet<Predicate> oldSet = new(oldKnowledges);
        var list = targetBase.ProvePredicateTargets.Where(a => !a.IsSuccess).ToList();
        foreach (var item in list)
        {
            var knowledge = item.Target;
            Predicate clone = null;
            for (int i = 0; i < knowledge.Properties.Count; i++)
            {
                var part = knowledge.Properties[i];
                if (oldSet.Contains(part))
                {
                    if (clone is null)
                        clone = knowledge.Clone();
                    clone.Properties[i] = newKnowledge;
                }
            }
            if (clone is not null)
            {
                knowledge.IsAvailable = false;
                clone.IsAvailable = true;
                clone.AddReason();
                clone.Conditions.Clear();
                clone.AddCondition(knowledge, newKnowledge);
                clone.Normalize();
                clone.SetHashCode();
                item.Target = clone;
            }
        }
    }

    public void UpdateKnowledgeBase(Predicate newKnowledge, Predicate[] oldKnowledges)
    {
        ZLog.Info($"update {StringTool.ComposeList(oldKnowledges)}->{newKnowledge}");
        UpdateTargetBase(newKnowledge, oldKnowledges);

        var list = knowledgeBase.Predicates.Values.Where(a => a.IsAvailable).ToList();
        foreach (var item in oldKnowledges)
        {
            item.IsAvailable = false;
        }
        HashSet<Predicate> oldSet = new(oldKnowledges);
        foreach (var knowledge in list)
        {
            Predicate clone = null;
            for (int i = 0; i < knowledge.Properties.Count; i++)
            {
                var part = knowledge.Properties[i];
                if (oldSet.Contains(part))
                {
                    if (clone is null)
                        clone = knowledge.Clone();
                    clone.Properties[i] = newKnowledge;
                }
            }
            if (clone is not null)
            {
                knowledge.IsAvailable = false;
                clone.IsAvailable = true;
                clone.AddReason();
                clone.Normalize();
                clone.Conditions.Clear();
                clone.AddCondition(knowledge, newKnowledge);
                clone.SetHashCode();
                if(clone.IsAvailable&&!knowledgeBase.Predicates.ContainsKey(clone.HashCode))
                    _add(clone);
            }
        }
    }
    public void DegenerateDB(List<Predicate> items)
    {
        foreach (var item in items)
        {
            item.IsAvailable = false;
            
        }

        HashSet<Knowledge> degenerated = new(items);

        static IEnumerable<Quantity> GetQuantities(Equation equation)
        {
            if (equation.Quantities is not null)
                foreach (var quantity in equation.Quantities)
                    yield return quantity;
            if (equation is QuantityValue quantityValue)
            {
                if (quantityValue.Quantity is not null)
                    yield return quantityValue.Quantity;
            }
            else if (equation is QuantityRatio quantityRatio)
            {
                if (quantityRatio.Quantity1 is not null)
                    yield return quantityRatio.Quantity1;
                if (quantityRatio.Quantity2 is not null)
                    yield return quantityRatio.Quantity2;
            }
            else if (equation is LinearEquation linearEquation)
            {
                foreach (var quantity in linearEquation.Coff.Keys)
                    yield return quantity;
            }
            else if (equation is ProductionEquation productionEquation)
            {
                foreach (var quantity in productionEquation.Coff.Keys)
                    yield return quantity;
            }
        }

        bool PropertiesContainDegenerate(Knowledge knowledge)
        {
            if (knowledge is Predicate pred)
                return pred.Properties.Any(degenerated.Contains);
            if (knowledge is Equation equation)
                return GetQuantities(equation).OfType<GeoQuantity>()
                    .Any(quantity => quantity.Figures.Any(degenerated.Contains));
            return false;
        }

        IEnumerable<Knowledge> AvailableKnowledges()
        {
            foreach (var knowledge in knowledgeBase.Predicates.Values)
                if (knowledge.IsAvailable)
                    yield return knowledge;
            foreach (var knowledge in knowledgeBase.Equations.Values)
                if (knowledge.IsAvailable)
                    yield return knowledge;
            foreach (var knowledge in knowledgeBase.QuantityValues.Values)
                if (knowledge.IsAvailable)
                    yield return knowledge;
            foreach (var knowledge in knowledgeBase.QuantityRatios.Values)
                if (knowledge.IsAvailable)
                    yield return knowledge;
        }

        bool changed = true;
        while (changed)
        {
            changed = false;
            foreach (var knowledge in AvailableKnowledges())
            {
                if (PropertiesContainDegenerate(knowledge) || knowledge.Conditions.Any(degenerated.Contains))
                {
                    knowledge.IsAvailable = false;
                    degenerated.Add(knowledge);
                    changed = true;
                    ZLog.Info($"Degenerate: {knowledge}");
                }
            }
        }
    }
    public void Degenerate(Line line)
    {
        HashSet<Predicate> lineProperties = new(line.Properties);
        List<Predicate> needForUpdateLines = [];
        foreach (var item in knowledgeBase.Categories[typeof(Triangle)].Where(l => l.IsAvailable))
        {
            if (item.Properties.TrueForAll(lineProperties.Contains))
            {
                needForUpdateLines.Add(item);
            }
        }
        foreach (var item in knowledgeBase.Categories[typeof(Quadrilateral)].Where(l => l.IsAvailable))
        {
            if (item.Properties.Count(lineProperties.Contains) > 2)
            {
                needForUpdateLines.Add(item);
            }
        }
        foreach (Angle item in knowledgeBase.Categories[typeof(Angle)].Where(l => l.IsAvailable))
        {
            if (item.Edge1.Exists(lineProperties.Contains) && item.Edge2.Exists(lineProperties.Contains) && lineProperties.Contains(item.Vertex))
            {
                needForUpdateLines.Add(item);
                ZLog.Info($"Degenerate: {item} Trige{line}");
            }
        }
        DegenerateDB(needForUpdateLines);
    }
    public virtual bool HasColine(params IEnumerable<Predicate> points)
    {
        var pts = points as ICollection<Predicate> ?? points.ToList();
        foreach (var line in knowledgeBase.Categories[typeof(Line)])
        {
            HashSet<Predicate> lineProps = new(line.Properties);
            if (pts.All(lineProps.Contains))
                return true;
        }
        return false;
    }

    public virtual Predicate Add(Line line)
    {
        if (knowledgeBase.Predicates.TryGetValue(line.HashCode, out var existing))
            return existing;

        var wrap = knowledgeBase.Categories[typeof(Line)].Where(l => l.IsAvailable).FirstOrDefault(p => line.Properties.TrueForAll(p.Properties.Contains));
        if (wrap is not null)
            return wrap;

        Line newLine = null;
        #region UpdateLine
        List<Predicate> needForUpdateLines = [];
        List<Predicate> allLinePoints = new List<Predicate>(line.Properties);
        HashSet<Predicate> allLinePointsSet = new(line.Properties);
        HashSet<Predicate> needForUpdateSet = [];

        void FindColinearPoints()
        {
            List<Predicate> roundFound = [];
            foreach (var item in knowledgeBase.Categories[typeof(Line)].Where(l => l.IsAvailable))
            {
                if (needForUpdateSet.Contains(item))
                    continue;
                if (item.Properties.Count(allLinePointsSet.Contains) >= 2)
                {
                    roundFound.Add(item);
                }
            }
            if (roundFound.Count == 0)
                return;
            needForUpdateLines.AddRange(roundFound);
            foreach (var l in roundFound)
            {
                needForUpdateSet.Add(l);
                foreach (var point in l.Properties)
                {
                    if (allLinePointsSet.Add(point))
                    {
                        allLinePoints.Add(point);
                    }
                }
            }
            FindColinearPoints();
        }
        FindColinearPoints();

        if (needForUpdateLines.Count > 0)
        {
            if (allLinePoints.TrueForAll(line.Properties.Contains))
            {
                newLine = line;
                AddPredicateBase(line); 
            }
            else
            {
                line.IsAvailable = false;
                newLine = new Line(allLinePoints.Select(p => (Point)p).ToArray());
                newLine.AddReason();
                newLine.AddCondition(line);
                newLine.AddCondition(needForUpdateLines);
            }
            Degenerate(newLine);
            UpdateKnowledgeBase(newLine, needForUpdateLines.ToArray());
        }
        #endregion

        ZLog.Info($"Update line:{line}");

        Predicate returnLine;
        if (newLine is null)
            returnLine = AddPredicateBase(line);
        else
            returnLine = AddPredicateBase(newLine);
        var effectiveLine = (Line)returnLine;

        #region AddSegs
        foreach (var item in PermutationCombinationTool.GetCombination(effectiveLine.Properties, 2))
        {
            Segment pred = new Segment((Point)item[0], (Point)item[1]);
            pred.AddReason();
            pred.AddCondition(effectiveLine);
            Add(pred);
        }
        #endregion

        #region UpdateAngle
        foreach (var knowledge in knowledgeBase.Categories[typeof(Line)])
        {
            Line line1 = effectiveLine;
            Line line2 = (Line)knowledge;

            var (i, l1, l2) = FindIntersection(line1.Points, line2.Points);
            if (i.Count() == 1)
            {
                LineIntersection lip = new LineIntersection((Point)i[0], line1, line2);
                lip.AddReason("Update by line update");
                lip.AddCondition(line1, line2);

                var d = Add(lip);
                var pos1 = line1.Properties.IndexOf(i[0]);
                var pos2 = line2.Properties.IndexOf(i[0]);
                List<List<Predicate>> linepart1 = new List<List<Predicate>>();
                List<List<Predicate>> linepart2 = new List<List<Predicate>>();
                if (pos1 == 0)
                {
                    linepart1.Add(line1.Properties.Skip(pos1 + 1).ToList());
                }
                else if (pos1 == line1.Properties.Count - 1)
                {
                    linepart1.Add(line1.Properties.Take(pos1).Reverse().ToList());
                }
                else
                {
                    linepart1.Add(line1.Properties.Skip(pos1 + 1).ToList());
                    linepart1.Add(line1.Properties.Take(pos1).Reverse().ToList());
                }
                if (pos2 == 0)
                {
                    linepart2.Add(line2.Properties.Skip(pos2 + 1).ToList());
                }
                else if (pos2 == line2.Properties.Count - 1)
                {
                    linepart2.Add(line2.Properties.Take(pos2).Reverse().ToList());
                }
                else
                {
                    linepart2.Add(line2.Properties.Skip(pos2 + 1).ToList());
                    linepart2.Add(line2.Properties.Take(pos2).Reverse().ToList());
                }

                foreach (var part1 in linepart1)
                {
                    foreach (var part2 in linepart2)
                    {
                        Angle pred = new Angle(part1.Select(p => (Point)p).ToList(), i[0], part2.Select(p => (Point)p).ToList());
                        pred.AddReason("Update by line update");
                        pred.AddCondition(line1, line2);
                        bool flag = false;
                        foreach (Line lined in knowledgeBase.Categories[typeof(Line)].Where(l => l.IsAvailable))
                        {
                            if (pred.Edge1.Exists(lined.Properties.Contains) && pred.Edge2.Exists(lined.Properties.Contains) && lined.Properties.Contains(pred.Vertex))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            Add(pred);
                            ZLog.Info($"Update line:{line1}, {line2} -> Generate angle: {pred}");

                        }
                    }
                }
            }
        }
        #endregion
        return returnLine;
    }

    public virtual Predicate Add(Angle newAngle)
    {
        if (knowledgeBase.Predicates.TryGetValue(newAngle.HashCode, out var existing))
            return existing;

        var Angles = knowledgeBase.Categories[typeof(Angle)];
        List<Point> newAngle_edge1 = newAngle.Edge1;
        List<Point> newAngle_edge2 = newAngle.Edge2;
        Angle oldAngle = null;
        foreach (Angle angle in Angles.Where(a => a.IsAvailable))
        {
            List<Point> angle_edge1 = angle.Edge1;
            List<Point> angle_edge2 = angle.Edge2;
            bool containsAll = newAngle_edge1.All(item => angle_edge1.Contains(item)) && newAngle_edge2.All(item => angle_edge2.Contains(item));
            bool containsAll1 = newAngle_edge1.All(item => angle_edge2.Contains(item)) && newAngle_edge2.All(item => angle_edge1.Contains(item));
            if ((angle.Vertex == newAngle.Vertex && containsAll) || (angle.Vertex == newAngle.Vertex && containsAll1))
            {
                oldAngle = angle;
            }
        }
        if (oldAngle is not null)
        {
            return oldAngle;
        }
        if (1 + newAngle_edge2.Count + newAngle_edge1.Count > 3)
        {
            List<Angle> needToUpdates = new();
            foreach (Angle angle in Angles)
            {
                List<Point> angle_edge1 = angle.Edge1;
                List<Point> angle_edge2 = angle.Edge2;
                bool containsAll = angle_edge1.All(item => newAngle_edge1.Contains(item)) && angle_edge2.All(item => newAngle_edge2.Contains(item));
                bool containsAll1 = angle_edge1.All(item => newAngle_edge2.Contains(item)) && angle_edge2.All(item => newAngle_edge1.Contains(item));
                if ((angle.IsAvailable == true && angle.Vertex == newAngle.Vertex && containsAll) || (angle.IsAvailable == true && angle.Vertex == newAngle.Vertex && containsAll1))
                {
                    needToUpdates.Add(angle);
                }
            }
            if (needToUpdates.Count == 0)
            {
                return AddPredicateBase(newAngle); 
            }
            else
            {
                needToUpdates.ForEach(A => A.IsAvailable = false);
                HashSet<Predicate> needToUpdateSet = new(needToUpdates);
                List<Predicate> updatedKnowledges = new();
                foreach (var knowledge in knowledgeBase.Predicates.Values)
                {
                    Predicate clone = null;
                    for (int i = 0; i < knowledge.Properties.Count; i++)
                    {
                        var part = knowledge.Properties[i];

                        if (needToUpdateSet.Contains(part))
                        {
                            if (clone is null)
                                clone = knowledge.Clone();
                            clone.Properties[i] = newAngle;
                        }
                    }
                    if (clone is not null)
                    {
                        knowledge.IsAvailable = false;
                        clone.IsAvailable = true;
                        clone.AddReason();
                        clone.Conditions.Clear();
                        clone.AddCondition(knowledge, newAngle);
                        updatedKnowledges.Add(clone);
                    }
                }
                AddPredicateBase(newAngle); 
                foreach (Predicate updatedKnowledge in updatedKnowledges)
                {
                    updatedKnowledge.SetHashCode();
                    Add(updatedKnowledge);
                }
                return newAngle;
            }
        }
        else
        {
            return AddPredicateBase(newAngle); 
        }
    }

    public virtual Predicate Add(Circle circle)
    {
        if (knowledgeBase.Predicates.TryGetValue(circle.HashCode, out var existing))
            return existing;

        var addedCircle = AddPredicateBase(circle);

        for (int i = 1; i < circle.Properties.Count; i++)
        {
            for (int j = i + 1; j < circle.Properties.Count; j++)
            {
                Arc arc = new Arc((Point)circle[0], (Point)circle[i], (Point)circle[j]);
                arc.AddReason();
                arc.AddCondition(circle);
                Add(arc);

                if (GetSegment(circle[0], circle[i]) is not null && GetSegment(circle[0], circle[j]) is not null)
                {
                    Sector sector = new Sector((Point)circle[0], (Point)circle[i], (Point)circle[j]);
                    sector.AddReason();
                    sector.AddCondition(circle);
                    Add(sector); 
                }
            }
        }
        return addedCircle;
    }

    public virtual Segment GetSegment(Predicate p1, Predicate p2)
    {
        Segment segment = new Segment((Point)p1, (Point)p2);
        if (knowledgeBase.Predicates.TryGetValue(segment.HashCode, out var existing))
            return (Segment)existing;
        return null;
    }
    #endregion
}