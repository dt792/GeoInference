
using System.Text;
using System.Text.RegularExpressions;

public class MapleBaseLinearMatrix
{
    public static Func<string, Quantity> ParseQuantity;
    public bool IsLog = false;
    #region MapleScript
    public const string MapleFile = @"CreateLinearSystem := proc()
    local sys := module()
        local QIndex := table(), QList := Array(1..0), EqList := Array(1..0), 
              ReducedMat := NULL, RankVal := 0, IsReduced := false, HasContradictionFlag := false,
              HasNewChanges := true; 

        export Init, AddEquation, Eliminate, IsRepresentable, 
               TryEvaluateExpression, ReduceColumnsByContinuedEquality, HasContradiction, DiscoverRelations,
               GetVariableNames, GetMatrixString, GetConstantsString, CalculateSparsity, GetMatrixSize;

        Init := proc()
            QIndex := table(); QList := Array(1..0); EqList := Array(1..0);
            ReducedMat := NULL; RankVal := 0; IsReduced := false; HasContradictionFlag := false;
            HasNewChanges := true; 
        end proc;

        AddEquation := proc(coeffDict::table, constant::algebraic)
            local q, idxList, oldNVars, nVars, i, j, oldRow, newRow;
            IsReduced := false;
            HasNewChanges := true;
            idxList := [indices(coeffDict, 'nolist')];
            oldNVars := numelems(QList);
            nVars := oldNVars;

            for q in idxList do
                if not assigned(QIndex[q]) then
                    nVars := nVars + 1; QIndex[q] := nVars; QList ,= q;
                end if;
            end do;

            if nVars > oldNVars then
                for i from 1 to numelems(EqList) do
                    oldRow := EqList[i];
                    newRow := Array(1..nVars+1, 0);
                    for j from 1 to oldNVars do newRow[j] := oldRow[j]; end do;
                    newRow[nVars+1] := oldRow[oldNVars+1];
                    EqList[i] := newRow;
                end do;
            end if;

            newRow := Array(1..nVars+1, 0);
            for q in idxList do newRow[QIndex[q]] := coeffDict[q]; end do;
            newRow[nVars+1] := constant;
            EqList ,= newRow;
        end proc;

        Eliminate := proc()
            local M, R, m, n, i, j, constPart, isZero, validRows, nVars, isZeroRow;
            if IsReduced then return end if;
            if numelems(EqList) = 0 then RankVal:=0; IsReduced:=true; HasContradictionFlag:=false; return; end if;
            
            nVars := numelems(QList);
            M := Matrix(numelems(EqList), nVars+1, (i,j) -> EqList[i][j]);
            m, n := LinearAlgebra:-Dimension(M);
            R := LinearAlgebra:-ReducedRowEchelonForm(M);
            R := map(normal, R);
            isZero := x -> evalb(x = 0) or evalb(normal(x) = 0);
            RankVal := 0; HasContradictionFlag := false; validRows := Array(1..0);
            
            for i from 1 to m do
                constPart := R[i, n];
                isZeroRow := true;
                for j from 1 to n-1 do
                    if not isZero(R[i,j]) then isZeroRow := false; break; end if;
                end do;
                
                if isZeroRow then
                    if not isZero(constPart) then
                        HasContradictionFlag := true; RankVal := 0; EqList := Array(1..0);
                        ReducedMat := Matrix(0, n); IsReduced := true; return; 
                    end if;
                else
                    RankVal := RankVal + 1;
                    validRows ,= Array(1..n, j -> R[i,j]);
                end if;
            end do;
            EqList := validRows;
            if numelems(validRows) = 0 then
                ReducedMat := Matrix(0, n);
            else
                ReducedMat := Matrix(numelems(validRows), n, (i,j) -> validRows[i][j]);
            end if;
            IsReduced := true;
        end proc;

        IsRepresentable := proc(coeffDict::table, constant::algebraic)
            local v, nVars, q, M_test, r_new, idxList, vecArr,j;
            if not IsReduced then Eliminate(); end if;
            if HasContradictionFlag then return false; end if;
            nVars := numelems(QList);
            vecArr := Array(1..nVars+1, 0);
            idxList := [indices(coeffDict, 'nolist')];
            for q in idxList do
                if not assigned(QIndex[q]) then return false; end if;
                vecArr[QIndex[q]] := coeffDict[q];
            end do;
            vecArr[nVars+1] := constant;
            v := Vector[row](nVars+1, j -> vecArr[j]);
            if ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then
                return evalb(normal(add(vecArr[j]^2, j=1..nVars+1)) = 0);
            end if;
            M_test := <ReducedMat | v>;
            r_new := LinearAlgebra:-Rank(map(normal, M_test));
            return evalb(r_new = RankVal);
        end proc;

        TryEvaluateExpression := proc(coeffDict::table, constant::algebraic)
            local nVars, expr, q, R, m, pivotCols, freeCols, i, j, pCol, pVar, rhs, 
                  subsEqns, evalExpr, freeVars, idxList, pivotCols_arr, subsEqns_arr, isZero, hasSysVar;
            if not IsReduced then Eliminate(); end if;
            if HasContradictionFlag then return [false, NULL]; end if;
            nVars := numelems(QList);
            expr := constant;
            idxList := [indices(coeffDict, 'nolist')];
            for q in idxList do
                if not assigned(QIndex[q]) then return [false, NULL]; end if;
                expr := expr + coeffDict[q] * q;
            end do;
            if nVars = 0 then return [true, normal(expr)]; end if;
            R := ReducedMat; m := LinearAlgebra:-RowDimension(R);
            if m = 0 then
                hasSysVar := false;
                for i from 1 to nVars do
                    if member(QList[i], indets(expr, name)) then hasSysVar := true; break; end if;
                end do;
                if hasSysVar then return [false, NULL]; end if;
                return [true, normal(expr)];
            end if;
            
            pivotCols_arr := Array(1..0);
            isZero := x -> evalb(normal(x)=0);
            for i from 1 to m do
                for j from 1 to nVars do 
                    if not isZero(R[i,j]) then pivotCols_arr ,= j; break; end if; 
                end do;
            end do;
            pivotCols := pivotCols_arr;
            
            freeCols := Array(1..0);
            for j from 1 to nVars do
                if not member(j, pivotCols) then freeCols ,= j; end if;
            end do;
            
            subsEqns_arr := Array(1..0);
            for i from 1 to m do
                pCol := pivotCols[i]; pVar := QList[pCol]; rhs := R[i, nVars+1];
                for j in freeCols do 
                    if not isZero(R[i,j]) then rhs := rhs - R[i,j] * QList[j]; end if; 
                end do;
                subsEqns_arr ,= (pVar = rhs);
            end do;
            subsEqns := {seq(subsEqns_arr[i], i=1..numelems(subsEqns_arr))};
            
            evalExpr := normal(subs(subsEqns, expr));
            freeVars := {seq(QList[freeCols[i]], i=1..numelems(freeCols))};
            if nops(indets(evalExpr, name) intersect freeVars) > 0 then return [false, NULL]; end if;
            return [true, evalExpr];
        end proc;

        ReduceColumnsByContinuedEquality := proc(ce::table)
            local coeffDict, actualValue, hasValue, validQs, q, mainQ, mainCoeff,
                  otherQ, otherCoeff, ratio, val, subsRules, removedQs,
                  newExpr, newConst, newEqList, newQList, newQIndex,
                  i, j, rowVec, expr, idxList, zeroSubs, nVars, origIdx, c, k, vList, orderedQs, newNVars, newRow, newCoeffs;

            if not assigned(ce[CoeffDict]) then error ""Missing CoeffDict in ce""; end if;
            coeffDict := ce[CoeffDict];
            hasValue := false;
            if assigned(ce[ActualValue]) and ce[ActualValue] <> NULL then
                hasValue := true; actualValue := ce[ActualValue];
            end if;

            idxList := [indices(coeffDict, 'nolist')];
            validQs := Array(1..0);
            for q in idxList do
                if assigned(QIndex[q]) then validQs ,= q; end if;
            end do;
            if numelems(validQs) = 0 then return false; end if;

            if assigned(ce[BaseVar]) and member(ce[BaseVar], validQs) then
                mainQ := ce[BaseVar];
            else
                vList := [seq(validQs[i], i=1..numelems(validQs))];
                vList := sort(vList, (a,b) -> String(a) < String(b));
                mainQ := vList[1];
                validQs := Array(vList);
            end if;

            orderedQs := Array(1..numelems(validQs));
            orderedQs[1] := mainQ; k := 2;
            for i from 1 to numelems(validQs) do
                if validQs[i] <> mainQ then orderedQs[k] := validQs[i]; k := k+1; end if;
            end do;
            validQs := orderedQs;

            subsRules := Array(1..0);
            if hasValue then
                for i from 1 to numelems(validQs) do
                    q := validQs[i]; val := normal(actualValue / coeffDict[q]);
                    subsRules ,= (q = val);
                end do;
                removedQs := {seq(validQs[i], i=1..numelems(validQs))};
            else
                if numelems(validQs) < 2 then return false; end if;
                mainCoeff := coeffDict[mainQ];
                for i from 2 to numelems(validQs) do
                    otherQ := validQs[i]; otherCoeff := coeffDict[otherQ];
                    ratio := normal(mainCoeff / otherCoeff);
                    subsRules ,= (otherQ = ratio * mainQ);
                end do;
                removedQs := {seq(validQs[i], i=2..numelems(validQs))};
            end if;

            nVars := numelems(QList);
            newEqList := Array(1..0);
            zeroSubs := {seq(QList[i]=0, i=1..nVars)};
            for i from 1 to numelems(EqList) do
                rowVec := EqList[i];
                expr := add(rowVec[j] * QList[j], j=1..nVars) - rowVec[nVars+1];
                newExpr := expand(normal(subs({seq(subsRules[k], k=1..numelems(subsRules))}, expr)));
                newCoeffs := Array(1..nVars);
                for j from 1 to nVars do newCoeffs[j] := coeff(newExpr, QList[j], 1); end do;
                newConst := -normal(eval(newExpr, zeroSubs));
                newRow := Array(1..nVars+1);
                for j from 1 to nVars do newRow[j] := newCoeffs[j]; end do;
                newRow[nVars+1] := newConst;
                newEqList ,= newRow;
            end do;

            newQList := Array(1..0);
            for i from 1 to nVars do
                q := QList[i];
                if not member(q, removedQs) then newQList ,= q; end if;
            end do;

            newQIndex := table();
            for i from 1 to numelems(newQList) do newQIndex[newQList[i]] := i; end do;

            EqList := Array(1..0);
            newNVars := numelems(newQList);
            for i from 1 to numelems(newEqList) do
                rowVec := newEqList[i];
                newRow := Array(1..newNVars+1, 0);
                for j from 1 to newNVars do
                    origIdx := QIndex[newQList[j]];
                    newRow[j] := normal(rowVec[origIdx]);
                end do;
                newRow[newNVars+1] := normal(rowVec[nVars+1]);
                EqList ,= newRow;
            end do;

            QList := newQList; QIndex := newQIndex;
            IsReduced := false;
            HasNewChanges := true; # 成功压缩列，标记状态改变
            return true;
        end proc;

        HasContradiction := proc()
            if not IsReduced then Eliminate(); end if;
            return HasContradictionFlag;
        end proc;

        GetVariableNames := proc()
            StringTools:-Join(
                map(x -> sprintf(""%a"", x), convert(QList, list)), 
                "",""
            );
        end proc;

        GetMatrixString := proc()
            local m, n, i, j, rowStrs, rowStr, res;
            if not IsReduced then Eliminate(); end if;
            if HasContradictionFlag or ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then
                return ""[]"";
            end if;
            m := LinearAlgebra:-RowDimension(ReducedMat);
            n := LinearAlgebra:-ColumnDimension(ReducedMat) - 1;
            if n <= 0 then return ""[]""; end if;
            
            rowStrs := Array(1..0);
            for i from 1 to m do
                rowStr := ""["";
                for j from 1 to n do
                    rowStr := cat(rowStr, sprintf(""%a"", normal(ReducedMat[i,j])));
                    if j < n then rowStr := cat(rowStr, "",""); end if;
                end do;
                rowStrs ,= cat(rowStr, ""]"");
            end do;
            
            res := ""["";
            for i from 1 to numelems(rowStrs) do
                res := cat(res, rowStrs[i]);
                if i < numelems(rowStrs) then res := cat(res, "",""); end if;
            end do;
            return cat(res, ""]"");
        end proc;

        GetConstantsString := proc()
            local m, i, res;
            if not IsReduced then Eliminate(); end if;
            if HasContradictionFlag or ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then
                return ""[]"";
            end if;
            m := LinearAlgebra:-RowDimension(ReducedMat);
            res := ""["";
            for i from 1 to m do
                res := cat(res, sprintf(""%a"", normal(ReducedMat[i, LinearAlgebra:-ColumnDimension(ReducedMat)])));
                if i < m then res := cat(res, "",""); end if;
            end do;
            return cat(res, ""]"");
        end proc;

        CalculateSparsity := proc(useReduced::boolean := false)
            local nVars, nEqs, i, j, totalElements, zeroCount, val, isZero;
            isZero := x -> evalb(x = 0) or evalb(normal(x) = 0);

            if useReduced then
                if not IsReduced then Eliminate(); end if;
                if HasContradictionFlag or ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then
                    return 1.0; 
                end if;
                nEqs := LinearAlgebra:-RowDimension(ReducedMat);
                nVars := LinearAlgebra:-ColumnDimension(ReducedMat) - 1; 
                if nVars = 0 then return 1.0; end if;
                totalElements := nEqs * nVars;
                zeroCount := 0;
                for i from 1 to nEqs do
                    for j from 1 to nVars do
                        val := ReducedMat[i, j];
                        if isZero(val) then zeroCount := zeroCount + 1; end if;
                    end do;
                end do;
            else
                nEqs := numelems(EqList);
                if nEqs = 0 then return 1.0; end if;
                nVars := numelems(QList);
                if nVars = 0 then return 1.0; end if;
                totalElements := nEqs * nVars;
                zeroCount := 0;
                for i from 1 to nEqs do
                    for j from 1 to nVars do
                        val := EqList[i][j];
                        if isZero(val) then zeroCount := zeroCount + 1; end if;
                    end do;
                end do;
            end if;

            return evalf(zeroCount / totalElements);
        end proc;

        DiscoverRelations := proc()
            local m, n, nVars, i, j, pCol, pivotCols, freeCols, freeVars,
                  varExprs, v, e, rhs, c, valList, relList,
                  exprList, v1, v2, e1, e2, diffExpr, ratio, outputList, isZero;

            if not HasNewChanges then return []; end if;

            if not IsReduced then Eliminate(); end if;
            if HasContradictionFlag or ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then
                HasNewChanges := false; 
                return [];
            end if;

            m := LinearAlgebra:-RowDimension(ReducedMat);
            n := LinearAlgebra:-ColumnDimension(ReducedMat);
            nVars := n - 1;
            isZero := x -> evalb(normal(x)=0);

            pivotCols := Array(1..0);
            for i from 1 to m do
                for j from 1 to nVars do
                    if not isZero(ReducedMat[i,j]) then pivotCols ,= j; break; end if;
                end do;
            end do;

            freeCols := Array(1..0);
            for j from 1 to nVars do
                if not member(j, pivotCols) then freeCols ,= j; end if;
            end do;
            freeVars := {seq(QList[freeCols[i]], i=1..numelems(freeCols))};

            varExprs := table();
            for i from 1 to numelems(freeCols) do
                j := freeCols[i]; varExprs[QList[j]] := QList[j];
            end do;
            for i from 1 to m do
                pCol := pivotCols[i]; v := QList[pCol];
                rhs := normal(ReducedMat[i, n]); e := rhs;
                for j in freeCols do
                    c := normal(ReducedMat[i, j]);
                    if not isZero(c) then e := e - c * QList[j]; end if;
                end do;
                varExprs[v] := normal(expand(e));
            end do;

            valList := Array(1..0); exprList := Array(1..0); relList := Array(1..0);
            for v in indices(varExprs, 'nolist') do
                e := varExprs[v];
                if nops(indets(e, name) intersect freeVars) = 0 then
                    valList ,= Array([v, normal(e)]);
                else
                    exprList ,= Array([v, e]);
                end if;
            end do;

            for i from 1 to numelems(exprList) do
                for j from i+1 to numelems(exprList) do
                    v1 := exprList[i][1]; e1 := exprList[i][2];
                    v2 := exprList[j][1]; e2 := exprList[j][2];
                    diffExpr := normal(e1 - e2);
                    if evalb(diffExpr = 0) then
                        relList ,= Array([v1, v2, 1]);
                    elif not isZero(e2) then
                        ratio := normal(e1 / e2);
                        if nops(indets(ratio, name) intersect freeVars) = 0 then
                            if not evalb(ratio = 1) and not evalb(ratio = 0) then
                                relList ,= Array([v1, v2, ratio]);
                            end if;
                        end if;
                    end if;
                end do;
            end do;

            for i from 1 to numelems(valList) do
                for j from i+1 to numelems(valList) do
                    if evalb(normal(valList[i][2] - valList[j][2]) = 0) then
                        relList ,= Array([valList[i][1], valList[j][1], 1]);
                    end if;
                end do;
            end do;

            outputList := [seq(convert(relList[i], list), i=1..numelems(relList)),
                           seq(convert(valList[i], list), i=1..numelems(valList))];
            
            HasNewChanges := false; 
            return outputList;
        end proc;

        GetMatrixSize := proc()
            local m, n;
            if not IsReduced then Eliminate(); end if;
            if ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then
                return [0, numelems(QList)];
            end if;
            m := LinearAlgebra:-RowDimension(ReducedMat);
            n := LinearAlgebra:-ColumnDimension(ReducedMat) - 1; 
            return [m, n];
        end proc;
    end module;
    return sys;
end proc:";
    #endregion

    #region Fields
    public static readonly MapleApp _mapleApp = MapleApp.Instance;
    public static bool _isMapleCodeLoaded = false;
    public readonly string _matrixName;
    public readonly List<string> _log = new();
    public readonly List<string> _commands = new();
    public IReadOnlyList<string> Log => _log;
    public string LastLog => StringTool.ComposeList(Log, "\n");
    public string LastCommand => StringTool.ComposeList(_commands, "\n");
    #endregion

    #region Init
    public static void EnsureMapleLoaded()
    {
        if (_isMapleCodeLoaded) return;
        var result = _mapleApp.Run(MapleFile);
        if (result != null && result.Contains("Error"))
            throw new InvalidOperationException($"Failed to load Maple script:{result}");
        _isMapleCodeLoaded = true;
    }
    public MapleBaseLinearMatrix()
    {
        EnsureMapleLoaded();
        _matrixName = $"sys_{Guid.NewGuid().ToString("N")[..8]}";
        RunCommand($"{_matrixName} := CreateLinearSystem();");
    }
    public string MatrixName => _matrixName;
    #endregion

    #region 
    public virtual void AddEquation(Dictionary<Quantity, Expr> coeffDict, Expr constant, Knowledge knowledge)
    {
        if (coeffDict == null || coeffDict.Count == 0) return;

        var pairs = string.Join(", ", coeffDict.Select(kv => $"{kv.Key}={kv.Value}"));
        string cmd = $"{_matrixName}:-AddEquation(table([{pairs}]), {constant}):";
        RunCommand(cmd);
        if (IsLog)
        {

            _log.Add(cmd);
            _log.Add(GetCompleteMatrixString());
        }
    }
    public (int Rows, int Cols) GetMatrixSize()
    {
        int rows; int cols;
        var res = RunCommand($"{_matrixName}:-GetMatrixSize();").Trim();
        var match = Regex.Match(res, @"\[(\d+),\s*(\d+)\]");
        if (match.Success)
        {
            rows = int.Parse(match.Groups[1].Value);
            cols = int.Parse(match.Groups[2].Value);
        }
        else
        {
            rows = 0; cols = 0;
        }
        return (rows, cols);
    }
    public virtual void Eliminate()
    {
        RunCommand($"{_matrixName}:-Eliminate();");
        //_log.Add(GetCompleteMatrixString()); 
    }
    public bool HasContradiction()
    {
        var res = RunCommand($"{_matrixName}:-HasContradiction();");
        return bool.TryParse(res.Trim(), out bool val) && val;
    }
    public bool IsRepresentable(Dictionary<Quantity, Expr> coeffDict, Expr constant)
    {
        var pairs = string.Join(", ", coeffDict.Select(kv => $"{kv.Key}={kv.Value}"));
        var res = RunCommand($"{_matrixName}:-IsRepresentable(table([{pairs}]), {constant});");
        if ((res.Contains("Error")))
        {
            return false;
        }
        return bool.TryParse(res.Trim(), out bool val) && val;
    }
    #endregion

    #region Tools
    public List<Quantity> GetVariableNames()
    {
        var res = RunCommand($"{_matrixName}:-GetVariableNames();").Trim();
        if (string.IsNullOrEmpty(res) || res == "\"\"") return new List<Quantity>();

        
        var names = res.Trim('"').Split(',');
        return names.Select(n => Quantity.Parse(n.Trim())).ToList(); 
    }
    public string[,] GetMatrix()
    {
        var matStr = RunCommand($"{_matrixName}:-GetMatrixString();").Trim();
        if (matStr == "\"[]\"") return new string[0, 0];
        return ParseMapleMatrix(matStr);
    }
    public string[] GetConstants()
    {
        var constStr = RunCommand($"{_matrixName}:-GetConstantsString();").Trim();

        
        if (constStr == "\"[]\"") return Array.Empty<string>();

        
        var clean = constStr.Trim('"', ' ', '[', ']');
        if (string.IsNullOrEmpty(clean)) return Array.Empty<string>();

        
        return clean.Split(',').Select(s => s.Trim()).ToArray();
    }
    public string GetCompleteMatrixString()
    {
        var variables = GetVariableNames();
        var matrix = GetMatrix();
        var constants = GetConstants();

        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        if (rows == 0 || cols == 0 || variables.Count == 0)
        {
            return "The matrix is empty or the data is incomplete.";
        }

        var sb = new StringBuilder();

        int[] colWidths = new int[cols + 1];
        for (int j = 0; j < cols; j++)
        {
            colWidths[j] = variables[j].ToString().Length;
            for (int i = 0; i < rows; i++)
            {
                colWidths[j] = Math.Max(colWidths[j], matrix[i, j].Length);
            }
        }
        colWidths[cols] = "Constants".Length;
        for (int i = 0; i < rows; i++)
        {
            colWidths[cols] = Math.Max(colWidths[cols], constants[i].ToString().Length);
        }

        for (int j = 0; j < cols; j++)
        {
            sb.Append(variables[j].ToString().PadRight(colWidths[j] + 2));
        }
        sb.Append("| ").AppendLine("Constants".PadRight(colWidths[cols]));

        int totalWidth = colWidths.Sum() + (cols + 1) * 2 + 2;
        sb.AppendLine(new string('-', totalWidth));

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                sb.Append(matrix[i, j].PadRight(colWidths[j] + 2));
            }
            sb.Append("| ").AppendLine(constants[i].ToString().PadRight(colWidths[cols]));
        }

        return sb.ToString();
    }
  
    public (bool success, Expr value) TryEvaluateExpression(Dictionary<Quantity, Expr> coeffDict, Expr constant)
    {
        var pairs = string.Join(", ", coeffDict.Select(kv => $"{kv.Key}={kv.Value}"));
        var res = RunCommand($"{_matrixName}:-TryEvaluateExpression(table([{pairs}]), {constant});");
        var match = Regex.Match(res, @"\[true,\s*(.+)\]|\[false,\s*NULL\]");
        if (!match.Success) return (false, null);

        if (match.Groups[1].Success)
        {
            var valStr = match.Groups[1].Value.Trim();
            return (true, valStr); 
        }
        return (false, null);
    }
    public virtual bool ReduceColumnsByContinuedEquality(
        Dictionary<Quantity, Expr> coeffDict,
        Expr actualValue = null,
        Quantity baseVar = null)
    {
        var pairs = string.Join(", ", coeffDict.Select(kv => $"{kv.Key}={kv.Value}"));
        string avParam = actualValue is null ? "NULL" : actualValue.ToString();
        string bvParam = baseVar == null ? "NULL" : baseVar.ToString();

        string cmd = $"{_matrixName}:-ReduceColumnsByContinuedEquality(table([CoeffDict=table([{pairs}]), ActualValue={avParam}, BaseVar={bvParam}]));";
        var res = RunCommand(cmd);
        return bool.TryParse(res.Trim(), out bool val) && val;
    }
    public double CalculateSparsity(bool onlyActiveRegion = true)
    {
        var res = RunCommand($"{_matrixName}:-CalculateSparsity();").Trim();
        return double.TryParse(res, out double val) ? val : 1;
    }
    public double CalculateDensity(bool onlyActiveRegion = true)
        => 1.0 - CalculateSparsity(onlyActiveRegion);
    public virtual List<Knowledge> DiscoverRelations()
    {
        List<Knowledge> result = [];
        var res = RunCommand($"{_matrixName}:-DiscoverRelations();").Trim();
        var relations = new List<(Quantity, Quantity, double)>();
        var values = new List<(Quantity, double)>();

        var clean = res.Trim('[', ']', '"');
        var a = NestedListParser.Parse(res);
        foreach (var item in a)
        {
            if (item.Count == 2)
            {
                Quantity var = Quantity.Parse(item[0]);
                QuantityValue pred = new QuantityValue(var, item[1]);
                if (GeoInferenceApp.IsZhOrEn)
                    pred.Reason = "线性矩阵发现";
                else
                    pred.Reason = "DiscoveredByLinearMatrix";
                result.Add(pred);
            }
            else if (item.Count == 3)
            {
                Quantity pVar = Quantity.Parse(item[0]);
                Quantity qVar = Quantity.Parse(item[1]);
                QuantityRatio pred = new QuantityRatio(pVar, qVar, item[2]);
                if (GeoInferenceApp.IsZhOrEn)
                    pred.Reason = "线性矩阵发现";
                else
                    pred.Reason = "DiscoveredByLinearMatrix";
                result.Add(pred);
            }
        }
        return result;
    }
    public string RunCommand(string command)
    {
        _commands.Add(command);
        var result = _mapleApp.Run(command);
        if (result != null && result.Contains("Error"))
            throw new MapleExecutionException($"Maple run command failed: {command}\n Detail: {result}");
        return result ?? "";
    }


    public static string[,] ParseMapleMatrix(string mapleStr)
    {
        var clean = mapleStr.Trim('"', ' ');
        if (clean == "[]" || string.IsNullOrWhiteSpace(clean))
            return new string[0, 0];
        var rowMatches = Regex.Matches(clean, @"\[([^\[\]]*)\]");

        if (rowMatches.Count == 0)
            return new string[0, 0];

        int rows = rowMatches.Count;

        var firstRowContent = rowMatches[0].Groups[1].Value.Trim();
        int cols = 0;
        if (!string.IsNullOrEmpty(firstRowContent))
        {
            cols = firstRowContent.Split(',').Length;
        }

        var matrix = new string[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            var rowContent = rowMatches[i].Groups[1].Value;
            if (cols > 0)
            {
                var elements = rowContent.Split(',');
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = j < elements.Length ? elements[j].Trim() : "";
                }
            }
        }

        return matrix;
    }
    #endregion
}
public class MapleExecutionException : Exception
{
    public MapleExecutionException(string message) : base(message) { }
}

public static class NestedListParser
{
    public static List<List<string>> Parse(string input)
    {
        var result = new List<List<string>>();
        if (string.IsNullOrWhiteSpace(input)) return result;

        int i = 0;
        int n = input.Length;

        while (i < n && char.IsWhiteSpace(input[i])) i++;
        if (i >= n || input[i] != '[')
            throw new FormatException("Format error: Input must start with an outer '['.");
        i++;
        while (i < n)
        {
            while (i < n && (char.IsWhiteSpace(input[i]) || input[i] == ',')) i++;
            if (i >= n) break;

            if (input[i] == '[')
            {
                i++;
                var currentList = new List<string>();
                var sb = new StringBuilder();

                while (i < n && input[i] != ']')
                {
                    if (input[i] == ',')
                    {
                        string val = sb.ToString().Trim();
                        if (val.Length > 0) currentList.Add(val);
                        sb.Clear();
                        i++;
                    }
                    else
                    {
                        sb.Append(input[i]);
                        i++;
                    }
                }

                string lastVal = sb.ToString().Trim();
                if (lastVal.Length > 0) currentList.Add(lastVal);

                result.Add(currentList);

                if (i < n && input[i] == ']') i++;
            }
            else if (input[i] == ']')
            {
                i++;
            }
            else
            {
                i++;
            }
        }
        return result;
    }
}