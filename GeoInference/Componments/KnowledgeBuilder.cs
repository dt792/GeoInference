using System.Reflection;
using System.Text.RegularExpressions;

public enum ZScriptErrorType
{
    SyntaxError,
    UndefinedPoint,
    UndefinedPredicate,
    ParameterMismatch,
    InvalidFormat,
    GenericPredicateNotSupported,
    EquationFormatError,
    UnknownGeometricQuantity
}

public class ZScriptDetailedException : Exception
{
    public int LineIndex { get; }
    public int ColumnIndex { get; }
    public ZScriptErrorType ErrorType { get; }
    public string LineContent { get; }

    public ZScriptDetailedException(ZScriptErrorType errorType, string message, int lineIndex, int columnIndex, string lineContent)
        : base($"[{errorType}] Line {lineIndex}, Col {columnIndex}: {message}\nContent: {lineContent}")
    {
        LineIndex = lineIndex;
        ColumnIndex = columnIndex;
        ErrorType = errorType;
        LineContent = lineContent;
    }
}

public class ZScriptRunnerConfig
{
    public bool IsAutoFindPoint { get; set; } = false;
    public bool IsAcceptAuxiliary { get; set; } = true;
    public bool IsThrowZScriptExextion { get; set; } = true;
}

public class ZScriptBuilder
{
    [DI] private KnowledgeBase knowledgeBase = null!;
    [DI] private KnowledgeBaseUpdater updater = null!;
    [DI] private ZScriptRunnerConfig config = null!;
    [DI] private TargetBase targetBase = null!;

    public Dictionary<string, Type> StringTypeMap { get; private set; } = new();
    public Dictionary<string, Point> PointRefs { get; } = new();

    public int QuestionIndex { get; set; }
    public int CurLineIndex { get; set; }
    public string CurLine { get; set; } = string.Empty;

    private List<string> _sortedPointNames = null!;

    private static readonly Regex PointCoordRegex = new(@"^([^()]+)\(([^()]+)\)$", RegexOptions.Compiled);
    private static readonly Regex PointNameRegex = new(@"^[A-Z]\d?", RegexOptions.Compiled);

    private readonly List<(string Pattern, Func<Match, GeoQuantity> Handler)> _geoQuantityHandlers;

    public ZScriptBuilder()
    {
        _geoQuantityHandlers = BuildGeoQuantityHandlers();
    }

    private void ThrowError(ZScriptErrorType type, string message, int col = -1)
    {
        if (col < 0) col = 0;
        throw new ZScriptDetailedException(type, message, CurLineIndex, col, CurLine);
    }

    Knowledge Parse(SumNode sum)
    {
        sum.Simplify();
        if (sum.Addends.Count == 1 && sum.Subtrahends.Count == 0)
        {
            if (sum.Addends[0] is ProductNode product)
            {
                if (product.IsSingle && product.Single is QuantityNode node)
                {
                    var Value = (-sum.Constant.ToExpr() / product.Constant.ToExpr());
                    var NewKnowledge = new QuantityValue(node.Quantity, Value);
                    return NewKnowledge;
                }
                else if (product.Multipliers.Count == 1 && product.Divisors.Count == 1
                    && product.Multipliers[0] is QuantityNode mut1 &&
                    product.Divisors[0] is QuantityNode mut2)
                {
                    var NewKnowledge = new QuantityRatio(mut1.Quantity, mut2.Quantity, (sum.Constant.ToExpr().Opposite()));
                    return NewKnowledge;
                }
            }
            else if (sum.Addends[0] is QuantityNode node)
            {
                var Value = sum.Constant.ToExpr().Opposite();
                var NewKnowledge = new QuantityValue(node.Quantity, Value);
                return NewKnowledge;
            }
        }
        else if (sum.Addends.Count == 0 && sum.Subtrahends.Count == 1)
        {
            if (sum.Subtrahends[0] is ProductNode product)
            {
                if (product.IsSingle)
                {
                    if (product.Single is QuantityNode mut)
                    {
                        var Value = (sum.Constant.ToExpr() / product.Constant.ToExpr());
                        var NewKnowledge = new QuantityValue(mut.Quantity, Value);
                        return NewKnowledge;
                    }
                }
                else if (product.Multipliers.Count == 1 && product.Divisors.Count == 1
                    && product.Multipliers[0] is QuantityNode mut1 &&
                    product.Divisors[0] is QuantityNode mut2)
                {
                    var NewKnowledge = new QuantityRatio(mut1.Quantity, mut2.Quantity, sum.Constant);
                    return NewKnowledge;
                }
            }
            else if (sum.Subtrahends[0] is QuantityNode prop)
            {
                var Value = sum.Constant;
                var NewKnowledge = new QuantityValue(prop.Quantity, Value);
                return NewKnowledge;
            }
        }
        else if (sum.Addends.Count == 1 && sum.Subtrahends.Count == 1 && sum.Constant == 0)
        {
            if (sum.Addends[0] is ProductNode LP)
            {
                if (LP.IsSingle && LP.Single is QuantityNode node1)
                {
                    if (sum.Subtrahends[0] is QuantityNode node2)
                    {
                        var NewKnowledge = new QuantityRatio(node1.Quantity, node2.Quantity, ((Expr)LP.Constant.ToExpr()).Invert());
                        return NewKnowledge;
                    }
                    else if (sum.Subtrahends[0] is ProductNode rP)
                    {
                        if (rP.IsSingle && rP.Single is QuantityNode node22)
                        {
                            var NewKnowledge = new QuantityRatio(node1.Quantity, node22.Quantity,
                                ((Expr)rP.Constant) / ((Expr)LP.Constant));
                            return NewKnowledge;
                        }
                    }
                }
            }
            else if (sum.Addends[0] is QuantityNode prop)
            {
                if (sum.Subtrahends[0] is QuantityNode node)
                {
                    var NewKnowledge = new QuantityRatio(prop.Quantity, node.Quantity, 1);
                    return NewKnowledge;
                }
                else if (sum.Subtrahends[0] is ProductNode rP)
                {
                    if (rP.IsSingle && rP.Single is QuantityNode node2)
                    {
                        var NewKnowledge = new QuantityRatio(prop.Quantity, node2.Quantity, rP.Constant);
                        return NewKnowledge;
                    }
                }
            }
        }
        else
        {
            bool flag = true;
            Dictionary<Quantity, Expr> dict = [];
            foreach (var item in sum.Addends)
            {
                if (item is QuantityNode gn)
                {
                    dict.Add(gn.Quantity, 1);
                }
                else if (item is ProductNode p && p.IsSingle && p.Multipliers[0] is QuantityNode gn2)
                {
                    dict.Add(gn2.Quantity, p.Constant);
                }
                else
                {
                    flag = false;
                    break;
                }
            }
            foreach (var item in sum.Subtrahends)
            {
                if (item is QuantityNode gn)
                {
                    dict.Add(gn.Quantity, -1);
                }
                else if (item is ProductNode p && p.IsSingle && p.Multipliers[0] is QuantityNode gn2)
                {
                    dict.Add(gn2.Quantity, p.Constant.Opposite());
                }
                else
                {
                    flag = false;
                    break;
                }
            }
            if (flag)
            {
                var lineEquation = new LinearEquation(dict, sum.Constant.Opposite().ToExpr());
                return lineEquation;
            }
        }
        return null;
    }

    public Knowledge ParseEq(string content)
    {
        var parts = content.Split('=');
        if (parts.Length != 2)
            ThrowError(ZScriptErrorType.EquationFormatError, "Equation must contain exactly one '=' sign.", CurLine.IndexOf(content));

        var left = ZExprParser.Parse(parts[0]);
        var right = ZExprParser.Parse(parts[1]);

        Dictionary<Quantity, Expr> coff = [];
        foreach (var item in GetEquationQuantities(content))
        {
            coff.Add(item, 0);
        }
        Expr expr = 1;

        if (left is ProductNode p)
        {
            expr /= p.Constant.ToExpr();
            foreach (var item in p.Multipliers)
            {
                if (item is QuantityNode g) coff[g.Quantity] += 1;
                else if (item is PowerNode power && power.Base is QuantityNode gn) coff[gn.Quantity] += power.Exponent;
                else goto Skip;
            }
            foreach (var item in p.Divisors)
            {
                if (item is QuantityNode g) coff[g.Quantity] += -1;
                else if (item is PowerNode power && power.Base is QuantityNode gn) coff[gn.Quantity] += power.Exponent.Opposite();
                else goto Skip;
            }
        }
        else if (left is QuantityNode g) coff[g.Quantity] += 1;
        else if (left is PowerNode power && power.Base is QuantityNode gn) coff[gn.Quantity] += power.Exponent;
        else goto Skip;

        if (right is ProductNode pp)
        {
            expr *= pp.Constant.ToExpr();
            foreach (var item in pp.Multipliers)
            {
                if (item is QuantityNode g) coff[g.Quantity] += -1;
                else if (item is PowerNode power && power.Base is QuantityNode gn) coff[gn.Quantity] += power.Exponent.Opposite();
                else goto Skip;
            }
            foreach (var item in pp.Divisors)
            {
                if (item is QuantityNode g) coff[g.Quantity] += 1;
                else if (item is PowerNode power && power.Base is QuantityNode gn) coff[gn.Quantity] += power.Exponent;
                else goto Skip;
            }
        }
        else if (right is QuantityNode g) coff[g.Quantity] += -1;
        else if (right is PowerNode power && power.Base is QuantityNode gn) coff[gn.Quantity] += power.Exponent.Opposite();
        else goto Skip;

        if (coff.Keys.Any(s => s.Unit == QuantityClassifications.Distance) &&
            coff.Keys.Any(s => s.Unit == QuantityClassifications.Angle))
            return null;

        if (coff.Count == 2)
        {
            var list = coff.ToList();
            if (list[0].Key is Quantity q1 && list[1].Key is Quantity q2)
            {
                if (list[0].Value == 1 && list[1].Value == -1)
                {
                    var NewKnowledge = new QuantityRatio(q1, q2, expr) { Reason = "2" };
                    return NewKnowledge;
                }
                else if (list[0].Value == -1 && list[1].Value == 1)
                {
                    var NewKnowledge = new QuantityRatio(q1, q2, expr.Invert()) { Reason = "2" };
                    return NewKnowledge;
                }
                else
                {
                    return new ProductionEquation(coff, expr);
                }
            }
        }
        else if (coff.Count > 2)
        {
            return new ProductionEquation(coff, expr);
        }

    Skip:
        Equation equation = new Equation(left.ToExpr(), right.ToExpr());
        Expr coExpr = equation.CoExpr.Simplify();
        ZExpr coZExpr = ZExprParser.Parse(coExpr.ToString()).Simplify();

        Knowledge knowledge = null;
        if (coZExpr is SumNode sum)
        {
            knowledge = Parse(sum);
        }
        else if (coZExpr is ProductNode product)
        {
            var sumNodes = product.Multipliers.Where(c => c is SumNode);
            if (sumNodes.Count() == 1) knowledge = Parse((SumNode)sumNodes.First());
        }

        if (knowledge is not null)
        {
            knowledge.Reason = equation.Reason;
            knowledge.Conditions.AddRange(equation.Conditions);
            return knowledge;
        }
        return equation;
    }

    #region Loading & Building

    public void LoadPredicates(IEnumerable<Type> predicateTypes)
    {
        var duplicateDict = new Dictionary<string, (string, string)>();
        foreach (var type in predicateTypes.Distinct())
        {
            if (type.IsGenericTypeDefinition)
            {
                ThrowError(ZScriptErrorType.GenericPredicateNotSupported, $"Generic predicate type '{type.Name}' is not supported.");
                continue;
            }

            StringTypeMap.Add(type.Name, type);
            var aliases = ZAlias.GetAlias(type);
            foreach (var alias in aliases)
            {
                if (StringTypeMap.ContainsKey(alias))
                {
                    duplicateDict.TryAdd(alias, (StringTypeMap[alias].Name, type.Name));
                    continue;
                }

                if (alias.Contains("<T>") || alias.Contains("<"))
                {
                    ThrowError(ZScriptErrorType.GenericPredicateNotSupported, $"Generic alias '{alias}' is not supported.");
                    continue;
                }

                StringTypeMap.Add(alias, type);
            }
        }

        if (duplicateDict.Count > 0)
        {
            var first = duplicateDict.First();
            ThrowError(ZScriptErrorType.InvalidFormat, $"Duplicate Chinese alias found in predicate definition '{first.Key}'：{first.Value.Item1} 与 {first.Value.Item2}");
        }

        StringTypeMap = StringTypeMap.OrderByDescending(kv => kv.Key.Length)
                                     .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    public void Build(string script)
    {
        ZScriptInput.ParseQuantity = ParseQuantity;
        script = ZScriptInput.FormatStandard(script);
        var lines = script.Split('\n').Select(s => s.Trim()).ToList();
        CurLineIndex = 0;
        QuestionIndex = 0;

        if (config.IsAutoFindPoint && !script.Contains(ZScriptInput.PointsDef))
        {
            var points = ExtractPointsFromScript(script);
            if (points.Any())
            {
                var pointDefLine = $"{ZScriptInput.PointsDef}{string.Join(" ", points)}";
                lines.Insert(0, pointDefLine);
            }
        }

        foreach (var rawLine in lines)
        {
            CurLineIndex++;
            CurLine = rawLine.Trim();
            if (string.IsNullOrEmpty(CurLine) || CurLine.StartsWith("//"))
                continue;
            if (CurLine.StartsWith(ZScriptInput.PointsDef)) ProcessPointsLine(CurLine);
            else if (CurLine.StartsWith(ZScriptInput.VarsDef)) ProcessVarsLine(CurLine);
            else if (CurLine.StartsWith(ZScriptInput.LinesDef)) ProcessLines(CurLine);
            else if (CurLine.StartsWith(ZScriptInput.ProveDef)) ProcessProveLine(CurLine);
            else if (CurLine.StartsWith(ZScriptInput.SolveDef)) ProcessSolveLine(CurLine);
            else if (CurLine.StartsWith(ZScriptInput.MinDef)) ProcessMinLine(CurLine);
            else if (CurLine.StartsWith(ZScriptInput.MaxDef)) ProcessMaxLine(CurLine);
            else if (CurLine.StartsWith(ZScriptInput.AuxDef) && config.IsAcceptAuxiliary) ProcessAuxiliaryLine(CurLine);
            else ProcessGeneralKnowledgeLine(CurLine);
            //try
            //{

            //}
            //catch (ZScriptDetailedException)
            //{
            //    throw; // Rethrow detailed exceptions untouched
            //}
            //catch (Exception ex)
            //{
            //    ThrowError(ZScriptErrorType.SyntaxError, $"Unhandled parsing error: {ex.Message}", 0);
            //}
        }
    }

    #endregion

    #region Line Processors

    private void ProcessPointsLine(string line)
    {
        var prefix = ZScriptInput.PointsDef;
        int startIdx = line.IndexOf(prefix) + prefix.Length;
        var content = line.Substring(startIdx).Trim();
        if (string.IsNullOrEmpty(content)) return;

        int searchStart = startIdx;
        foreach (var pointInfo in content.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            int col = line.IndexOf(pointInfo, searchStart);
            searchStart = col + pointInfo.Length;

            var match = PointCoordRegex.Match(pointInfo);
            if (match.Success)
            {
                var pointName = match.Groups[1].Value.Trim();
                var coordStr = match.Groups[2].Value.Trim();
                var coords = coordStr.Split(',');
                var point = new Point(pointName);
                double x = 0, y = 0, z = 0;
                if (coords.Length == 3)
                {
                    if (!double.TryParse(coords[0], out x) ||
                        !double.TryParse(coords[1], out y) ||
                        !double.TryParse(coords[2], out z))
                        ThrowError(ZScriptErrorType.InvalidFormat, $"Invalid 3D coordinate format in '{pointInfo}'", col);
                    point.X = x; point.Y = y; point.Z = z;
                }
                else if (coords.Length == 2)
                {
                    if (!double.TryParse(coords[0], out x) || !double.TryParse(coords[1], out y))
                        ThrowError(ZScriptErrorType.InvalidFormat, $"Invalid 2D coordinate format in '{pointInfo}'", col);
                    point.X = x; point.Y = y; point.Z = 0;
                }
                else
                {
                    ThrowError(ZScriptErrorType.InvalidFormat, $"Pseudo-coordinate '{pointInfo}' format error. Expected 2 or 3 comma-separated values.", col);
                }

                point = (Point)updater.Add(point);
                PointRefs[pointName] = point;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(pointInfo) || !PointNameRegex.IsMatch(pointInfo))
                    ThrowError(ZScriptErrorType.InvalidFormat, $"Invalid point name format '{pointInfo}'", col);

                var point = new Point(pointInfo);
                updater.Add(point);
                PointRefs[pointInfo] = point;
            }
        }
        _sortedPointNames = PointRefs.Keys.OrderByDescending(p => p.Length).ToList();
    }

    private void ProcessLines(string line)
    {
        var prefix = ZScriptInput.LinesDef;
        int startIdx = line.IndexOf(prefix) + prefix.Length;
        var content = line.Substring(startIdx).Trim();
        if (string.IsNullOrEmpty(content)) return;

        int searchStart = startIdx;
        foreach (var pair in content.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            int col = line.IndexOf(pair, searchStart);
            searchStart = col + pair.Length;

            var points = GetPointsFromStr(pair, col);
            if (points.Count < 2)
                ThrowError(ZScriptErrorType.InvalidFormat, $"A line must be defined by at least 2 points, found '{pair}'", col);

            var segment = new Line(points.ToArray());
            updater.Add(segment);
        }
    }

    private void ProcessVarsLine(string line)
    {
        var prefix = ZScriptInput.VarsDef;
        int startIdx = line.IndexOf(prefix) + prefix.Length;
        var content = line.Substring(startIdx).Trim();
        if (string.IsNullOrEmpty(content)) return;

        int searchStart = startIdx;
        foreach (var pair in content.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            int col = line.IndexOf(pair, searchStart);
            searchStart = col + pair.Length;

            var parts = pair.Split(":");
            if (parts.Length != 2)
                ThrowError(ZScriptErrorType.InvalidFormat, $"Variable definition must be in 'Name:Unit' format, found '{pair}'", col);

            var name = parts[0];
            var unit = parts[1];

            if (!Enum.TryParse(typeof(QuantityClassifications), unit, true, out var result))
                ThrowError(ZScriptErrorType.InvalidFormat, $"Unknown quantity unit '{unit}' for variable '{name}'", col);

            Var var = new Var(name) { Unit = (QuantityClassifications)result! };
            knowledgeBase.Quantities.TryAdd(name, var);
        }
    }

    private void ProcessProveLine(string line)
    {
        var content = line.Replace(ZScriptInput.ProveDef, string.Empty).Trim();
        var knowledge = ParseKnowledge(content);
        var targetIndex = QuestionIndex++;

        switch (knowledge)
        {
            case Predicate pred:
                var target = new ProvePredicateTarget { Index = targetIndex, Target = pred };
                targetBase.Targets.Add(target);
                targetBase.ProvePredicateTargets.Add(target);
                break;
            case Equation eq:
                var target2 = new ProveEquationTarget { Index = targetIndex, Target = eq };
                targetBase.Targets.Add(target2);
                break;
        }
    }

    private void ProcessSolveLine(string line)
    {
        var content = line.Replace(ZScriptInput.SolveDef, string.Empty).Trim();
        var expr = ParseZExpr(content);
        var targetIndex = QuestionIndex++;

        switch (expr)
        {
            case QuantityNode qNode:
                var valueTarget = new SolveQuantityValueTarget { Index = targetIndex, GeoQuantity = qNode.Quantity };
                targetBase.SolveQuantityValueTargets.Add(valueTarget);
                targetBase.Targets.Add(valueTarget);
                break;
            case SumNode sum:
                var linearTarget = new SolveLinearTarget { Index = targetIndex, Target = expr.ToExpr() };
                foreach (var item in sum.Addends)
                {
                    if (item is QuantityNode q) linearTarget.CoffDict.Add(q.Quantity, 1);
                    else if (item is ProductNode pp && pp.IsSingle) linearTarget.CoffDict.Add(((QuantityNode)pp.Multipliers[0]).Quantity, pp.Constant.ToExpr());
                }
                foreach (var item in sum.Subtrahends)
                {
                    if (item is QuantityNode q) linearTarget.CoffDict.Add(q.Quantity, -1);
                    else if (item is ProductNode pp && pp.IsSingle) linearTarget.CoffDict.Add(((QuantityNode)pp.Multipliers[0]).Quantity, pp.Constant.Opposite().ToExpr());
                }
                targetBase.SolveLinearTargets.Add(linearTarget);
                targetBase.Targets.Add(linearTarget);
                break;
            case ProductNode product:
                if (product.Multipliers.Count == 1 && product.Divisors.Count == 1 && product.Multipliers[0] is QuantityNode q1 && product.Divisors[0] is QuantityNode q2)
                {
                    var ratioTarget = new SolveQuantityRatioTarget { Index = targetIndex, GeoQuantity1 = q1.Quantity, GeoQuantity2 = q2.Quantity };
                    targetBase.SolveQuantityRatioTargets.Add(ratioTarget);
                    targetBase.Targets.Add(ratioTarget);
                }
                else
                {
                    var productionTarget = new SolveProductionTarget { Index = targetIndex, Target = expr.ToExpr() };
                    foreach (var item in product.Multipliers)
                    {
                        if (item is QuantityNode q) productionTarget.CoffDict.Add(q.Quantity, 1);
                        else if (item is PowerNode pp) productionTarget.CoffDict.Add(((QuantityNode)pp.Base).Quantity, pp.Exponent.ToExpr());
                    }
                    foreach (var item in product.Divisors)
                    {
                        if (item is QuantityNode q) productionTarget.CoffDict.Add(q.Quantity, -1);
                        else if (item is PowerNode pp) productionTarget.CoffDict.Add(((QuantityNode)pp.Base).Quantity, pp.Exponent.Opposite().ToExpr());
                    }
                    targetBase.SolveProductionTargets.Add(productionTarget);
                    targetBase.Targets.Add(productionTarget);
                }
                break;
            default:
                var exprTarget = new SolveExprTarget { Index = targetIndex, Target = content, Tmp = expr.ToString(), Quantities = GetExprQuantities(content) };
                targetBase.SolveExprTargets.Add(exprTarget);
                targetBase.Targets.Add(exprTarget);
                break;
        }
    }

    private void ProcessMinLine(string line) => targetBase.TakeMinValue.Add(line.Replace(ZScriptInput.MinDef, string.Empty).Trim());
    private void ProcessMaxLine(string line) => targetBase.TakeMaxValue.Add(line.Replace(ZScriptInput.MaxDef, string.Empty).Trim());
    private void ProcessAuxiliaryLine(string line) => updater.Add(ParseKnowledge(line.Replace(ZScriptInput.AuxDef, string.Empty).Trim()));
    private void ProcessGeneralKnowledgeLine(string line) => updater.Add(ParseKnowledge(line));

    #endregion

    #region Parser Logic

    public Knowledge ParseKnowledge(string content)
    {
        if (content.Contains('=')) return ParseEq(content);
        if (content.Contains('>') || content.Contains('<')) return ParseInequation(content);
        return ParsePredicate(content);
    }

    private Knowledge ParseInequation(string content)
    {
        throw new NotImplementedException();
    }

    public Predicate ParsePredicate(string content)
    {
        var parts = content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            int col = CurLine.IndexOf(content);
            ThrowError(ZScriptErrorType.SyntaxError, "Does not conform to predicate statement input specification. Expected 'PredicateName Args'.", col >= 0 ? col : 0);
        }

        int predCol = CurLine.IndexOf(parts[0]);
        var predicateType = ParsePredicateType(parts[0], predCol);

        int argsCol = CurLine.IndexOf(parts[1]);
        var args = parts[1].Split(',').ToList();
        var pred= CombinePredicateWithArgs(predicateType, args, argsCol);
        return (Predicate)updater.Add(pred);
    }


    public List<Quantity> GetEquationQuantities(string content)
    {
        var parts = content.Split("=");
        ZExpr left = ParseZExpr(parts[0]);
        ZExpr right = ParseZExpr(parts[1]);

        void GetQuantity(List<Quantity> quantities, ZExpr zExpr)
        {
            if (zExpr is QuantityNode quantityNode) quantities.Add(quantityNode.Quantity);
            else if (zExpr is SumNode sumNode) foreach (var item in sumNode.Addends.Union(sumNode.Subtrahends)) GetQuantity(quantities, item);
            else if (zExpr is ProductNode productNode) foreach (var item in productNode.Multipliers.Union(productNode.Divisors)) GetQuantity(quantities, item);
            else if (zExpr is PowerNode power) GetQuantity(quantities, power.Base);
        }

        List<Quantity> QQ = [];
        GetQuantity(QQ, left);
        GetQuantity(QQ, right);
        return QQ.Distinct().ToList();
    }

    public List<Quantity> GetExprQuantities(string content)
    {
        var expr = ParseZExpr(content);
        var quantities = new List<Quantity>();
        CollectQuantities(expr, quantities);
        return quantities.Distinct().ToList();
    }

    public Quantity ParseQuantity(string content)
    {
        if (knowledgeBase.Quantities.TryGetValue(content, out var existingVar))
            return existingVar;
        try
        {
            return ParseGeoQuantity(content);
        }
        catch
        {
            Var var = new Var(content);
            knowledgeBase.Quantities.Add(content, var);
            return var;
        }
    }

    public GeoQuantity ParseGeoQuantity(string str)
    {
        foreach (var (pattern, handler) in _geoQuantityHandlers)
        {
            var match = Regex.Match(str, pattern);
            if (match.Success) return handler(match);
        }
        ThrowError(ZScriptErrorType.UnknownGeometricQuantity, $"Unknown Geometric Quantity: '{str}'", CurLine.IndexOf(str));
        return null!;
    }

    public ZExpr ParseZExpr(string content) => ZExprParser.Parse(content);

    #endregion

    #region Predicate Type Resolution & Construction

    private Type ParsePredicateType(string content, int col = 0)
    {
        if (StringTypeMap.TryGetValue(content, out var type))
            return type;

        ThrowError(ZScriptErrorType.UndefinedPredicate, $"No corresponding predicate class found for '{content}'.", col);
        return null!;
    }

    private Predicate CombinePredicateWithArgs(Type predicateType, List<string> args, int baseCol = 0)
    {
        var ctor = predicateType.GetConstructors().FirstOrDefault();
        if (ctor == null) ThrowError(ZScriptErrorType.SyntaxError, $"No public constructor found for predicate '{predicateType.Name}'.", baseCol);

        var parameters = ctor.GetParameters();
        var argValues = new List<object>();

        if (predicateType == typeof(Angle))
        {
            if (args.Count == 3)
            {
                var points1 = GetPointsFromStr(args[0], baseCol);
                var vertex = GetPointsFromStr(args[1], baseCol);
                var points2 = GetPointsFromStr(args[2], baseCol);
                if (vertex.Count != 1) ThrowError(ZScriptErrorType.ParameterMismatch, "Angle vertex must be a single point.", baseCol);
                argValues.Add(points1); argValues.Add(vertex[0]); argValues.Add(points2);
            }
            else if (args.Count == 1)
            {
                var points = GetPointsFromStr(args[0], baseCol);
                if (points.Count != 3) ThrowError(ZScriptErrorType.ParameterMismatch, "Angle requires exactly 3 points when passed as a single argument.", baseCol);
                argValues.Add(new List<Point> { points[0] }); argValues.Add(points[1]); argValues.Add(new List<Point> { points[2] });
            }
            else ThrowError(ZScriptErrorType.ParameterMismatch, "Angle parameter error. Expected 1 or 3 arguments.", baseCol);
        }
        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Point[]))
        {
            if (args.Count != 1) ThrowError(ZScriptErrorType.ParameterMismatch, $"Expected 1 argument for Point[] in '{predicateType.Name}', got {args.Count}.", baseCol);
            argValues.Add(GetPointsFromStr(args[0], baseCol).ToArray());
        }
        else if (parameters.Length > 0 && parameters.All(p => p.ParameterType == typeof(Point)))
        {
            if (args.Count != 1) ThrowError(ZScriptErrorType.ParameterMismatch, $"Expected 1 argument containing multiple points for '{predicateType.Name}'.", baseCol);
            var points = GetPointsFromStr(args[0], baseCol);
            if (points.Count != parameters.Length) ThrowError(ZScriptErrorType.ParameterMismatch, $"Expected {parameters.Length} points, got {points.Count}.", baseCol);
            argValues.AddRange(points);
        }
        else ProcessRemainingParameters(parameters, args, argValues, baseCol);

        try { return (Predicate)ctor.Invoke(argValues.ToArray()); }
        catch (Exception ex) { ThrowError(ZScriptErrorType.ParameterMismatch, $"Failed to instantiate '{predicateType.Name}': {ex.InnerException?.Message ?? ex.Message}", baseCol); return null!; }
    }

    private Knowledge ProcessInnerKnowledge(Type predicateType, List<string> args, int baseCol = 0)
    {
        var ctor = predicateType.GetConstructors().FirstOrDefault();
        if (ctor == null) ThrowError(ZScriptErrorType.SyntaxError, $"No constructor for inner type '{predicateType.Name}'.", baseCol);

        var parameters = ctor.GetParameters();
        var argValues = new List<object>();

        if (predicateType == typeof(Angle))
        {
            if (args.Count == 0) ThrowError(ZScriptErrorType.ParameterMismatch, "Missing arguments for Angle.", baseCol);
            var points = GetPointsFromStr(args[0], baseCol);
            args.RemoveAt(0);
            if (points.Count == 3) { argValues.Add(new List<Point> { points[0] }); argValues.Add(points[1]); argValues.Add(new List<Point> { points[2] }); }
            else ThrowError(ZScriptErrorType.ParameterMismatch, "Inner Angle requires exactly 3 points.", baseCol);
        }
        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Point[]))
        {
            if (args.Count == 0) ThrowError(ZScriptErrorType.ParameterMismatch, "Missing Point[] argument.", baseCol);
            argValues.Add(GetPointsFromStr(args[0], baseCol).ToArray());
            args.RemoveAt(0);
        }
        else if (parameters.Length > 0 && parameters.All(p => p.ParameterType == typeof(Point)))
        {
            if (args.Count == 0) ThrowError(ZScriptErrorType.ParameterMismatch, "Missing point arguments.", baseCol);
            argValues.AddRange(GetPointsFromStr(args[0], baseCol));
            args.RemoveAt(0);
        }
        else ProcessRemainingParameters(parameters, args, argValues, baseCol);

        try { return (Knowledge)ctor.Invoke(argValues.ToArray()); }
        catch (Exception ex) { ThrowError(ZScriptErrorType.ParameterMismatch, $"Inner knowledge instantiation error: {ex.InnerException?.Message ?? ex.Message}", baseCol); return null!; }
    }

    private void ProcessRemainingParameters(IEnumerable<ParameterInfo> parameters, List<string> args, List<object> argValues, int baseCol = 0)
    {
        foreach (var param in parameters)
        {
            if (args.Count == 0) ThrowError(ZScriptErrorType.ParameterMismatch, $"Missing argument for parameter '{param.Name}' of type '{param.ParameterType.Name}'.", baseCol);

            if (param.ParameterType == typeof(Point))
            {
                var pointName = args[0]; args.RemoveAt(0);
                if (!PointRefs.TryGetValue(pointName, out var point)) ThrowError(ZScriptErrorType.UndefinedPoint, $"Point '{pointName}' not found.", baseCol);
                argValues.Add(point);
            }
            else if (param.ParameterType == typeof(Expr))
            {
                var arg = args[0]; args.RemoveAt(0);
                argValues.Add(arg == "value" ? arg : ParseExpr(arg));
            }
            else if (param.ParameterType == typeof(string))
            {
                argValues.Add(args[0]); args.RemoveAt(0);
            }
            else
            {
                var innerKnowledge = ProcessInnerKnowledge(param.ParameterType, args, baseCol);
                argValues.Add((Predicate)updater.Add(innerKnowledge));
            }
        }
    }

    #endregion

    #region Tools

    private List<Point> GetPointsFromStr(string arg, int baseCol = 0)
    {
        var points = new List<Point>();
        var remaining = arg;
        int currentOffset = 0;

        while (!string.IsNullOrEmpty(remaining))
        {
            var pointName = _sortedPointNames?.FirstOrDefault(p => remaining.StartsWith(p));
            if (pointName != null)
            {
                if (!PointRefs.TryGetValue(pointName, out var pt)) ThrowError(ZScriptErrorType.UndefinedPoint, $"Point '{pointName}' is not defined.", baseCol + currentOffset);
                points.Add(pt);
                remaining = remaining.Substring(pointName.Length);
                currentOffset += pointName.Length;
            }
            else
            {
                var invalidChunk = new string(remaining.TakeWhile(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
                if (string.IsNullOrEmpty(invalidChunk)) invalidChunk = remaining[0].ToString();

                if (points.Count > 0) ThrowError(ZScriptErrorType.SyntaxError, $"Point list contains non-point values or missing point near '{invalidChunk}'", baseCol + currentOffset);
                else ThrowError(ZScriptErrorType.UndefinedPoint, $"Unknown point identifier '{invalidChunk}'", baseCol + currentOffset);
            }
        }
        return points;
    }

    private void CollectQuantities(ZExpr expr, List<Quantity> quantities)
    {
        switch (expr)
        {
            case QuantityNode qNode: quantities.Add(qNode.Quantity); break;
            case SumNode sum: foreach (var child in sum.Addends.Concat(sum.Subtrahends)) CollectQuantities(child, quantities); break;
            case ProductNode product: foreach (var child in product.Multipliers.Concat(product.Divisors)) CollectQuantities(child, quantities); break;
            case PowerNode power: CollectQuantities(power.Base, quantities); break;
        }
    }

    private List<string> ExtractPointsFromScript(string script)
    {
        var tmp = script.Replace(ZScriptInput.ProveDef, "")
                        .Replace(ZScriptInput.SolveDef, "")
                        .Replace(ZScriptInput.MinDef, "")
                        .Replace(ZScriptInput.MaxDef, "")
                        .Replace("∠", "角");
        return PointNameRegex.Matches(tmp).Cast<Match>().Select(m => m.Value).Distinct().ToList();
    }

    private Expr ParseExpr(string content) => throw new NotImplementedException();

    #endregion

    #region Geometric Quantity Handlers

    private List<(string, Func<Match, GeoQuantity>)> BuildGeoQuantityHandlers()
    {
        return new List<(string, Func<Match, GeoQuantity>)>
        {
            (@"^[A-Z]\d?[A-Z]\d?$", match =>
            {
                var points = GetPointsFromStr(match.Value);
                if (points.Count != 2) ThrowError(ZScriptErrorType.InvalidFormat, "Line segment must have exactly 2 points");
                var segment = (Segment)updater.Add(new Segment(points[0], points[1]));
                return segment.Length;
            }),
            (@"^角(\w+)的Tan", match => GetAngleTan(match.Groups[1].Value)),
            (@"^角(\w+)的Cos", match => GetAngleCos(match.Groups[1].Value)),
            (@"^角(\w+_\w+_\w+)的Cos", match => GetAngleCos(match.Groups[1].Value.Replace('_', ','))),
            (@"^角(\w+)的Sin", match => GetAngleSize(match.Groups[1].Value)),
            (@"^角(\w+_\w+_\w+)的Sin", match => GetAngleSize(match.Groups[1].Value.Replace('_', ','))),
            (@"^∠(\w+)", match => GetAngleSize(match.Groups[1].Value)),
            (@"^∠(\w+_\w+_\w+)", match => GetAngleSize(match.Groups[1].Value.Replace('_', ','))),
            (@"^角(\w+)", match => GetAngleSize(match.Groups[1].Value)),
            (@"^角(\w+_\w+_\w+)", match => GetAngleSize(match.Groups[1].Value.Replace('_', ','))),
            (@"弧(\w+)的大小", match => GetArcSize(match.Groups[1].Value)),
            (@"弧(\w+)", match => GetArcLength(match.Groups[1].Value)),
            (@"扇形(\w+)的面积", match => GetSectorArea(match.Groups[1].Value)),
            (@"扇形(\w+)的周长", match => GetSectorPerimeter(match.Groups[1].Value)),
            (@"圆(\w+)的面积", match => GetCircleArea(match.Groups[1].Value)),
            (@"圆(\w+)的周长", match => GetCirclePerimeter(match.Groups[1].Value)),
            (@"圆(\w+)的半径", match => GetCircleRadius(match.Groups[1].Value)),
            (@"圆(\w+)的直径", match => GetCircleDiameter(match.Groups[1].Value)),
            (@"三角形(\w+)的面积", match => GetTriangleArea(match.Groups[1].Value)),
            (@"三角形(\w+)的周长", match => GetTrianglePerimeter(match.Groups[1].Value)),
            (@"四边形(\w+)的面积", match => GetQuadrilateralArea(match.Groups[1].Value)),
            (@"四边形(\w+)的周长", match => GetQuadrilateralPerimeter(match.Groups[1].Value)),
            (@"^Angle_(\w+)_Tan", match => GetAngleTan(match.Groups[1].Value)),
            (@"^Angle_(\w+)_Cos", match => GetAngleCos(match.Groups[1].Value)),
            (@"^Angle_(\w+_\w+_\w+)_Cos", match => GetAngleCos(match.Groups[1].Value.Replace('_', ','))),
            (@"^Angle_(\w+)_Sin", match => GetAngleSize(match.Groups[1].Value)),
            (@"^Angle_(\w+_\w+_\w+)_Sin", match => GetAngleSize(match.Groups[1].Value.Replace('_', ','))),
            (@"^Angle_(\w+)", match => GetAngleSize(match.Groups[1].Value)),
            (@"^Angle_(\w+_\w+_\w+)", match => GetAngleSize(match.Groups[1].Value.Replace('_', ','))),
            (@"Arc_(\w+)_Size", match => GetArcSize(match.Groups[1].Value)),
            (@"Arc_(\w+)", match => GetArcLength(match.Groups[1].Value)),
            (@"Sector_(\w+)_Area", match => GetSectorArea(match.Groups[1].Value)),
            (@"Sector_(\w+)_Perimeter", match => GetSectorPerimeter(match.Groups[1].Value)),
            (@"Circle_(\w+)_Area", match => GetCircleArea(match.Groups[1].Value)),
            (@"Circle_(\w+)_Perimeter", match => GetCirclePerimeter(match.Groups[1].Value)),
            (@"Circle_(\w+)_Radius", match => GetCircleRadius(match.Groups[1].Value)),
            (@"Circle_(\w+)_Diameter", match => GetCircleDiameter(match.Groups[1].Value)),
            (@"Tri_(\w+)_Area", match => GetTriangleArea(match.Groups[1].Value)),
            (@"Tri_(\w+)_Perimeter", match => GetTrianglePerimeter(match.Groups[1].Value)),
            (@"Triangle_(\w+)_Area", match => GetTriangleArea(match.Groups[1].Value)),
            (@"Triangle_(\w+)_Perimeter", match => GetTrianglePerimeter(match.Groups[1].Value)),
            (@"Quad_(\w+)_Area", match => GetQuadrilateralArea(match.Groups[1].Value)),
            (@"Quad_(\w+)_Perimeter", match => GetQuadrilateralPerimeter(match.Groups[1].Value)),
            (@"Quadriliateral_(\w+)_Area", match => GetQuadrilateralArea(match.Groups[1].Value)),
            (@"Quadriliateral_(\w+)_Perimeter", match => GetQuadrilateralPerimeter(match.Groups[1].Value)),
        };
    }

    private GeoQuantity GetArcSize(string angleStr) => ((Arc)updater.Add((Arc)ParseKnowledge($"弧 {angleStr}"))).Size;
    private GeoQuantity GetArcLength(string angleStr) => ((Arc)updater.Add((Arc)ParseKnowledge($"弧 {angleStr}"))).MinorArcLength;
    private GeoQuantity GetAngleTan(string angleStr) => ((Angle)updater.Add((Angle)ParseKnowledge($"角 {angleStr}"))).Tan;
    private GeoQuantity GetAngleCos(string angleStr) => ((Angle)updater.Add((Angle)ParseKnowledge($"角 {angleStr}"))).Cos;
    private GeoQuantity GetAngleSize(string angleStr) => ((Angle)updater.Add((Angle)ParseKnowledge($"角 {angleStr}"))).Size;
    private GeoQuantity GetSectorArea(string sectorName) => ((Sector)updater.Add((Sector)ParseKnowledge($"扇形 {sectorName}"))).Area;
    private GeoQuantity GetSectorPerimeter(string sectorName) => ((Sector)updater.Add((Sector)ParseKnowledge($"扇形 {sectorName}"))).Perimeter;
    private GeoQuantity GetCircleArea(string circleName) => ((Circle)updater.Add((Circle)ParseKnowledge($"圆 {circleName}"))).Area;
    private GeoQuantity GetCirclePerimeter(string circleName) => ((Circle)updater.Add((Circle)ParseKnowledge($"圆 {circleName}"))).Perimeter;
    private GeoQuantity GetCircleRadius(string circleName) => ((Circle)updater.Add((Circle)ParseKnowledge($"圆 {circleName}"))).Radius;
    private GeoQuantity GetCircleDiameter(string circleName) => ((Circle)updater.Add((Circle)ParseKnowledge($"圆 {circleName}"))).Diameter;
    private GeoQuantity GetTriangleArea(string triName) => ((Triangle)updater.Add((Triangle)ParseKnowledge($"三角形 {triName}"))).Area;
    private GeoQuantity GetTrianglePerimeter(string triName) => ((Triangle)updater.Add((Triangle)ParseKnowledge($"三角形 {triName}"))).Perimeter;
    private GeoQuantity GetQuadrilateralArea(string quadName) => ((Quadriliateral)updater.Add((Quadriliateral)ParseKnowledge($"四边形 {quadName}"))).Area;
    private GeoQuantity GetQuadrilateralPerimeter(string quadName) => ((Quadriliateral)updater.Add((Quadriliateral)ParseKnowledge($"四边形 {quadName}"))).Perimeter;

    #endregion
}