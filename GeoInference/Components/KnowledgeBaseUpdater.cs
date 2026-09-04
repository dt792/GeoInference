public class KnowledgeBaseUpdater
{
    [DI] KnowledgeBase knowledgeBase;
    [DI] TargetBase targetBase;

    #region Overall Processing
    public Action<Predicate> PredicateAdded { get; set; }
    public Action<Equation> EquationAdded { get; set; }

    public void AddCondictionalKnowledgePair(CondictionalKnowledge knowledge)
    {
        Add(knowledge);
    }

    public Knowledge Add(Knowledge knowledge)
    {
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

    public Knowledge Add(Predicate pred)
    {
        if (knowledgeBase.Predicates.ContainsKey(pred.HashCode))
        {
            return knowledgeBase.Predicates[pred.HashCode];
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
    /// <summary>
    
    /// </summary>
    public Predicate AddPredicateBase(Predicate pred)
    {
        if (knowledgeBase.Predicates.ContainsKey(pred.HashCode))
            return knowledgeBase.Predicates[pred.HashCode];

        _add(pred);
        if (pred is Figure f)
            MakeGeoQuantity(f);
        _specialSizeConvert(pred);
        knowledgeBase.NewKnowledges.Add(pred);
        return pred;
    }

    public void TryAddGeoQuantity(GeoQuantity quantity)
    {
        if (!knowledgeBase.Quantities.ContainsKey(quantity.ToString()))
        {
            knowledgeBase.Quantities.Add(quantity.ToString(), quantity);
        }
    }

    void _specialSizeConvert(Knowledge value)
    {
        if (value is AngleSize asize)
        {
            if (Expr.SizeToCosSpecialValues.ContainsKey(asize.Expr.ToString()))
            {
                QuantityValue size = new QuantityValue(asize.Angle.Cos, Expr.SizeToCosSpecialValues[asize.Expr.ToString()]);
                size.AddCondition(value.Conditions);
                size.Reason = value.Reason;
                Add(size);
            }
            if (Expr.SizeToSinSpecialValues.ContainsKey(asize.Expr.ToString()))
            {
                QuantityValue size = new QuantityValue(asize.Angle.Sin, Expr.SizeToSinSpecialValues[asize.Expr.ToString()]);
                size.AddCondition(value.Conditions);
                size.Reason = value.Reason;
                Add(size);
            }
            if (Expr.SizeToTanSpecialValues.ContainsKey(asize.Expr.ToString()))
            {
                QuantityValue size = new QuantityValue(asize.Angle.Tan, Expr.SizeToTanSpecialValues[asize.Expr.ToString()]);
                size.AddCondition(value.Conditions);
                size.Reason = value.Reason;
                Add(size);
            }
        }
        else if (value is AngleCos acos)
        {
            if (Expr.CosToSizeSpecialValues.ContainsKey(acos.Expr.ToString()))
            {
                AngleSize size = new AngleSize(acos.Angle, Expr.CosToSizeSpecialValues[acos.Expr.ToString()]);
                size.AddCondition(value.Conditions);
                size.Reason = value.Reason;
                Add(size);

                QuantityValue qv = new QuantityValue(acos.Angle.Size, Expr.CosToSizeSpecialValues[acos.Expr.ToString()]);
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
        else if (figure.GetType() == typeof(Quadriliateral))
        {
            TryAddGeoQuantity(((Quadriliateral)figure).Area);
            TryAddGeoQuantity(((Quadriliateral)figure).Perimeter);
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
        if (!knowledgeBase.Categories.ContainsKey(pred.GetType()))
        {
            var categoryInfo = new List<Predicate>() { };
            knowledgeBase.Categories.Add(categoryType, categoryInfo);
        }
        pred.PosIndex = (uint)knowledgeBase.Categories[categoryType].Count;
        knowledgeBase.Categories[categoryType].Add(pred);
        PredicateAdded?.Invoke(pred);
        return pred;
    }

    public Knowledge Add(QuantityValue qv)
    {
        if (!qv.IsAvailable)
            return null;
        if (!knowledgeBase.QuantityValues.ContainsKey(qv.HashCode))
        {
            qv.PosIndex = (uint)knowledgeBase.QuantityValues.Count;
            knowledgeBase.QuantityValues.Add(qv.HashCode, qv);
            knowledgeBase.NewKnowledges.Add(qv);

            EquationAdded?.Invoke(qv);
            var p = qv.ToPred();
            if (p is not null)
                Add(p);
            return qv;
        }
        else
        {
            return knowledgeBase.QuantityValues[qv.HashCode];
        }
    }

    public Knowledge Add(QuantityRatio qr)
    {
        if (!qr.IsAvailable)
            return null;
        if (!knowledgeBase.QuantityRatios.ContainsKey(qr.HashCode))
        {
            qr.PosIndex = (uint)knowledgeBase.QuantityRatios.Count;
            knowledgeBase.QuantityRatios.Add(qr.HashCode, qr);
            knowledgeBase.NewKnowledges.Add(qr);
            EquationAdded?.Invoke(qr);
            var p = qr.ToPred();
            if (p is not null)
                Add(p);
            EquationAdded?.Invoke(qr);
            return qr;
        }
        else
        {
            return knowledgeBase.QuantityRatios[qr.HashCode];
        }
    }

    public Knowledge Add(LinearEquation linerEquation)
    {
        if (!knowledgeBase.Equations.ContainsKey(linerEquation.HashCode))
        {
            linerEquation.PosIndex = (uint)knowledgeBase.Equations.Count;
            knowledgeBase.LinearEquations.Add(linerEquation.HashCode, linerEquation);
            knowledgeBase.Equations.Add(linerEquation.HashCode, linerEquation);
            knowledgeBase.NewKnowledges.Add(linerEquation);
            EquationAdded?.Invoke(linerEquation);
        }
        return linerEquation;
    }

    public Knowledge Add(ProductionEquation productionEquation)
    {
        if (!knowledgeBase.Equations.ContainsKey(productionEquation.HashCode))
        {
            productionEquation.PosIndex = (uint)knowledgeBase.Equations.Count;
            knowledgeBase.ProductionEquations.Add(productionEquation.HashCode, productionEquation);
            knowledgeBase.Equations.Add(productionEquation.HashCode, productionEquation);
            knowledgeBase.NewKnowledges.Add(productionEquation);
            EquationAdded?.Invoke(productionEquation);
        }
        return productionEquation;
    }

    public Knowledge Add(Equation equation)
    {
        if (!knowledgeBase.Equations.ContainsKey(equation.HashCode))
        {
            equation.PosIndex = (uint)knowledgeBase.Equations.Count;
            knowledgeBase.Equations.Add(equation.HashCode, equation);
            knowledgeBase.NewKnowledges.Add(equation);
            EquationAdded?.Invoke(equation);
        }
        return equation;
    }

    public void Add(CondictionalKnowledge ck)
    {
        foreach (var item in knowledgeBase.ConditionalKnowledgePairs.Where(cj => cj.Knowledge.HashCode == ck.Knowledge.HashCode))
        {
            if (item.Targets.Count == ck.Targets.Count)
            {
                bool flag = true;
                foreach (var kv in ck.Targets)
                {
                    if (!item.Targets.ContainsKey(kv.Key))
                    {
                        flag = false;
                    }
                    else
                    {
                        if (kv.Value.Expr is not null)
                        {
                            if (item.Targets[kv.Key].Expr != kv.Value.Expr)
                            {
                                flag = false;
                            }
                        }
                    }
                }
                if (flag)
                    return;
            }
        }
        knowledgeBase.ConditionalKnowledgePairs.Add(ck);
        foreach (var item in ck.ConditionDict)
        {
            if (knowledgeBase.Predicates.ContainsKey(item.Key))
            {
                if (ck.Targets[item.Key].Expr is not null)
                {
                    if (ck.Targets[item.Key].Expr == knowledgeBase.Predicates[item.Key].Expr)
                    {
                        ck.ConditionDict[item.Key] = knowledgeBase.Predicates[item.Key];
                    }
                }
                else
                {
                    ck.ConditionDict[item.Key] = knowledgeBase.Predicates[item.Key];
                }
            }
        }
        ck.Check();
        if (ck.IsEstablish)
        {
            Add(ck.Knowledge);
        }
        else
        {
            foreach (var item in ck.ConditionDict)
            {
                if (item.Value is null)
                    if (knowledgeBase.InversedConditionalKnowledgePairDict.ContainsKey(item.Key))
                    {
                        knowledgeBase.InversedConditionalKnowledgePairDict[item.Key].Add(ck);
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
        Dictionary<Knowledge, List<Knowledge>> replaceDict = [];
        var lines = knowledges.Where(p => p is Line).Select(p => (Line)p).ToList();
        var segments = knowledges.Where(p => p is Segment).Select(p => (Segment)p).ToList();
        foreach (var seg in segments)
        {
            Add(seg);
        }
        foreach (var line in lines)
        {
            Add(line);
            for (int i = 0; i < line.Properties.Count; i++)
            {
                for (int j = i + 1; j < line.Properties.Count; j++)
                {
                    Segment seg = new Segment((Point)line[i], (Point)line[j]);
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
                GeoQuantity quantity = new GeoQuantity([(Figure)line, (Figure)item], GeoQuantity.Distance);
                TryAddGeoQuantity(quantity);
            }
        }
    }

    public (List<T> intersection, List<T> left1, List<T> left2) FindIntersection<T>(List<T> pms1, List<T> pms2) where T : Predicate
    {
        List<T> intersection = pms1.Where(p => pms2.Contains(p)).ToList();
        List<T> left1 = pms1.Where(p => !pms2.Contains(p)).ToList();
        List<T> left2 = pms2.Where(p => !pms1.Contains(p)).ToList();
        return (intersection, left1, left2);
    }

    void UpdateTargetBase(Predicate newKnowledge, Predicate[] oldKnowledges)
    {
        var list = targetBase.ProvePredicateTargets.Where(a => !a.IsSuccess).ToList();
        foreach (var item in list)
        {
            var knowledge = item.Target;
            Predicate clone = null;
            for (int i = 0; i < knowledge.Properties.Count; i++)
            {
                var part = knowledge.Properties[i];
                if (oldKnowledges.Contains(part))
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

        foreach (var item in targetBase.SolveQuantityValueTargets)
        {
            if (oldKnowledges.Contains(item.GeoQuantityKnowledge))
            {

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
        foreach (var knowledge in list)
        {
            Predicate clone = null;
            for (int i = 0; i < knowledge.Properties.Count; i++)
            {
                var part = knowledge.Properties[i];
                if (oldKnowledges.Contains(part))
                {
                    if (clone is null)
                        clone = knowledge.Clone();
                    clone.Properties[i] = newKnowledge;
                    clone.SetHashCode();
                }
            }
            if (clone is not null)
            {
                knowledge.IsAvailable = false;
                clone.IsAvailable = true;
                clone.AddReason();
                clone.Conditions.Clear();
                clone.AddCondition(knowledge, newKnowledge);
                clone.SetHashCode();
                _add(clone);
            }
        }
    }

    public virtual bool HasColine(params IEnumerable<Predicate> points)
    {
        Line lineTemplete = new Line(points.Select(p => (Point)p).ToArray());
        List<Predicate> lines = new();
        foreach (var line in knowledgeBase.Categories[typeof(Line)])
        {
            if (points.ToList().TrueForAll(p => line.Properties.Contains(p)))
                return true;
        }
        return false;
    }

    public Predicate Add(Line line)
    {
        if (knowledgeBase.Predicates.ContainsKey(line.HashCode))
            return knowledgeBase.Predicates[line.HashCode];

        var wrap = knowledgeBase.Categories[typeof(Line)].FirstOrDefault(p => line.Properties.TrueForAll(p.Properties.Contains));
        if (wrap is not null)
            return wrap;

        Line newLine = null;
        #region UpdateLine
        List<Predicate> needForUpdateLines = [];
        foreach (var item in knowledgeBase.Categories[typeof(Line)].Where(l => l.IsAvailable))
        {
            if (item.Properties.Count(line.Properties.Contains) >= 2)
            {
                needForUpdateLines.Add(item);
            }
        }
        if (needForUpdateLines.Count > 0)
        {
            List<Predicate> allLinePoints = new List<Predicate>();
            foreach (var plane in needForUpdateLines)
            {
                foreach (var point in plane.Properties)
                {
                    if (!allLinePoints.Contains(point))
                    {
                        allLinePoints.Add((Point)point);
                    }
                }
            }

            if (allLinePoints.TrueForAll(line.Properties.Contains))
            {
                newLine = line;
                AddPredicateBase(line); 
            }
            else
            {
                allLinePoints.AddRange(line.Properties);
                var points = allLinePoints.Distinct();
                line.IsAvailable = false;
                newLine = new Line(points.Select(p => (Point)p).ToArray());
                newLine.AddReason();
                newLine.AddCondition(line);
                newLine.AddCondition(needForUpdateLines);
            }
            UpdateKnowledgeBase(newLine, needForUpdateLines.ToArray());
        }
        #endregion

        ZLog.Info($"Update line:{line}");

        #region AddSegs
        foreach (var item in PermutationCombinationTool.GetCombination(line.Properties, 2))
        {
            Segment pred = new Segment((Point)item[0], (Point)item[1]);
            pred.AddReason();
            pred.AddCondition(line);
            Add(pred);
        }
        #endregion

        Predicate returnLine = null;
        if (newLine is null)
            returnLine = AddPredicateBase(line); 
        else
            returnLine = AddPredicateBase(newLine); 

        #region UpdateAngle
        foreach (var knowledge in knowledgeBase.Categories[typeof(Line)])
        {
            Line line1 = line;
            Line line2 = (Line)knowledge;

            var (i, l1, l2) = FindIntersection(line.Points, line2.Points);
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
                        Add(pred);
                        ZLog.Info($"Update line:{line1}, {line2} -> Generate angle: {pred}");
                    }
                }
            }
        }
        #endregion
        return returnLine;
    }

    public Predicate Add(Angle newAngle)
    {
        if (knowledgeBase.Predicates.ContainsKey(newAngle.HashCode))
            return knowledgeBase.Predicates[newAngle.HashCode];

        var Angles = knowledgeBase.Categories[typeof(Angle)];
        List<Point> newAngle_edge1 = newAngle.Edge1;
        List<Point> newAngle_edge2 = newAngle.Edge2;
        Angle oldAngle = null;
        foreach (Angle angle in Angles)
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
                List<Predicate> updatedKnowledges = new();
                foreach (var knowledge in knowledgeBase.Predicates.Values)
                {
                    Predicate clone = null;
                    for (int i = 0; i < knowledge.Properties.Count; i++)
                    {
                        var part = knowledge.Properties[i];

                        if (needToUpdates.Contains(part))
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

    public Predicate Add(Circle circle)
    {
        if (knowledgeBase.Predicates.ContainsKey(circle.HashCode))
            return knowledgeBase.Predicates[circle.HashCode];

        
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
        if (knowledgeBase.Predicates.ContainsKey(segment.HashCode))
            return (Segment)knowledgeBase.Predicates[segment.HashCode];
        return null;
    }
    #endregion
}