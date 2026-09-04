using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

/// <summary>

/// </summary>
public class MapleBaseProductionMatrix
{
    public bool IsLog = false;
    public static Func<string, Quantity> ParseQuantity;
    #region Maple Script
    public const string MapleFile = @"with(LinearAlgebra):
CreateMultiplicativeSystem := proc()
    local sys := module()
        local QIndex := table(), QList := Array(1..0), EqList := Array(1..0), 
              ReducedMat := NULL, RankVal := 0, IsReduced := false, HasContradictionFlag := false;

        export Init, AddEquation, Eliminate, IsRepresentable, 
               TryEvaluateExpression, ReduceColumnsByContinuedEquality, HasContradiction, DiscoverRelations,
               GetVariableNames, GetMatrixString, GetConstantsString, CalculateSparsity, GetMatrixSize;

        Init := proc()
            QIndex := table(); QList := Array(1..0); EqList := Array(1..0);
            ReducedMat := NULL; RankVal := 0; IsReduced := false; HasContradictionFlag := false;
        end proc;

        AddEquation := proc(coeffDict::table, constant::algebraic)
            local q, idxList, oldNVars, nVars, i, j, oldRow, newRow;
            IsReduced := false;
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
            local m, n, i, j, pivotRow, pivot, invPivot, factor, temp, validRows, isZeroRow, constPart, row, col, M, isZero, x;
            if IsReduced then return end if;
            m := numelems(EqList);
            if m = 0 then RankVal:=0; IsReduced:=true; HasContradictionFlag:=false; ReducedMat:=Matrix(0,0); return; end if;
            
            n := numelems(QList);
            M := Matrix(m, n+1, (i,j) -> EqList[i][j]);
            
            RankVal := 0;
            HasContradictionFlag := false;
            isZero := x -> evalb(normal(x) = 0);
            
            row := 1;
            for col from 1 to n do
                if row > m then break; end if;
                
                pivotRow := -1;
                for i from row to m do
                    if not isZero(M[i, col]) then
                        pivotRow := i;
                        break;
                    end if;
                end do;
                
                if pivotRow <> -1 then
                    if pivotRow <> row then
                        for j from col to n+1 do
                            temp := M[row, j]; M[row, j] := M[pivotRow, j]; M[pivotRow, j] := temp;
                        end do;
                    end if;
                    
                    pivot := M[row, col];
                    if not evalb(normal(pivot) = 1) then
                        invPivot := 1 / pivot;
                        for j from col to n do M[row, j] := normal(M[row, j] * invPivot); end do;
                        M[row, n+1] := simplify(M[row, n+1]^invPivot, symbolic);
                    end if;
                    
                    for i from 1 to m do
                        if i <> row then
                            factor := M[i, col];
                            if not isZero(factor) then
                                for j from col to n do M[i, j] := normal(M[i, j] - factor * M[row, j]); end do;
                                M[i, n+1] := simplify(M[i, n+1] / (M[row, n+1]^factor), symbolic);
                            end if;
                        end if;
                    end do;
                    row := row + 1; RankVal := RankVal + 1;
                end if;
            end do;
            
            validRows := Array(1..0);
            for i from 1 to m do
                isZeroRow := true;
                for j from 1 to n do
                    if not isZero(M[i, j]) then isZeroRow := false; break; end if;
                end do;
                
                if isZeroRow then
                    constPart := M[i, n+1];
                    if not evalb(normal(constPart) = 1) then
                        HasContradictionFlag := true; RankVal := 0; 
                        EqList := Array(1..0); QList := Array(1..0); QIndex := table();
                        ReducedMat := Matrix(0, 0); IsReduced := true; return; 
                    end if;
                else
                    validRows ,= Array(1..n+1, j -> M[i, j]);
                end if;
            end do;
            
            EqList := validRows;
            if numelems(validRows) = 0 then ReducedMat := Matrix(0, n+1);
            else ReducedMat := Matrix(numelems(validRows), n+1, (i,j) -> validRows[i][j]); end if;
            IsReduced := true;
        end proc;

        IsRepresentable := proc(coeffDict::table, constant::algebraic)
            local nVars, q, idxList, vecArr, i, j, factor, targetConst, calcConst, isZero, pCol, allZero, x;
            if not IsReduced then Eliminate(); end if;
            if HasContradictionFlag then return false; end if;
            
            nVars := numelems(QList);
            vecArr := Array(1..nVars, 0);
            idxList := [indices(coeffDict, 'nolist')];
            for q in idxList do
                if not assigned(QIndex[q]) then return false; end if;
                vecArr[QIndex[q]] := coeffDict[q];
            end do;
            
            isZero := x -> evalb(normal(x) = 0);
            targetConst := constant; calcConst := 1;
            
            for i from 1 to LinearAlgebra:-RowDimension(ReducedMat) do
                pCol := -1;
                for j from 1 to nVars do
                    if not isZero(ReducedMat[i, j]) then pCol := j; break; end if;
                end do;
                if pCol <> -1 then
                    factor := vecArr[pCol];
                    if not isZero(factor) then
                        for j from 1 to nVars do vecArr[j] := normal(vecArr[j] - factor * ReducedMat[i, j]); end do;
                        calcConst := simplify(calcConst * (ReducedMat[i, nVars+1]^factor), symbolic);
                    end if;
                end if;
            end do;
            
            allZero := true;
            for j from 1 to nVars do if not isZero(vecArr[j]) then allZero := false; break; end if; end do;
            return allZero and evalb(normal(calcConst) = normal(targetConst));
        end proc;

        TryEvaluateExpression := proc(coeffDict::table, constant::algebraic)
            local nVars, expr, q, R, m, pivotCols, freeCols, i, j, pCol, pVar, rhs, 
                  subsEqns, evalExpr, freeVars, idxList, pivotCols_arr, subsEqns_arr, isZero, hasSysVar, pExpr, x;
            if not IsReduced then Eliminate(); end if;
            if HasContradictionFlag then return [false, NULL]; end if;
            nVars := numelems(QList);
            expr := constant;
            idxList := [indices(coeffDict, 'nolist')];
            for q in idxList do
                if not assigned(QIndex[q]) then return [false, NULL]; end if;
                expr := expr * q^coeffDict[q];
            end do;
            if nVars = 0 then return [true, normal(expr)]; end if;
            R := ReducedMat; m := LinearAlgebra:-RowDimension(R);
            if m = 0 then
                hasSysVar := false;
                for i from 1 to nVars do if member(QList[i], indets(expr, name)) then hasSysVar := true; break; end if; end do;
                if hasSysVar then return [false, NULL]; end if;
                return [true, normal(expr)];
            end if;
            
            pivotCols_arr := Array(1..0); isZero := x -> evalb(normal(x)=0);
            for i from 1 to m do
                for j from 1 to nVars do if not isZero(R[i,j]) then pivotCols_arr ,= j; break; end if; end do;
            end do;
            pivotCols := pivotCols_arr;
            freeCols := Array(1..0);
            for j from 1 to nVars do if not member(j, pivotCols) then freeCols ,= j; end if; end do;
            
            subsEqns_arr := Array(1..0);
            for i from 1 to m do
                pCol := pivotCols[i]; pVar := QList[pCol]; rhs := R[i, nVars+1]; pExpr := rhs;
                for j in freeCols do if not isZero(R[i,j]) then pExpr := pExpr * QList[j]^(-R[i,j]); end if; end do;
                subsEqns_arr ,= (pVar = normal(pExpr));
            end do;
            subsEqns := {seq(subsEqns_arr[i], i=1..numelems(subsEqns_arr))};
            evalExpr := normal(subs(subsEqns, expr));
            freeVars := {seq(QList[freeCols[i]], i=1..numelems(freeCols))};
            if nops(indets(evalExpr, name) intersect freeVars) > 0 then return [false, NULL]; end if;
            return [true, evalExpr];
        end proc;

        ReduceColumnsByContinuedEquality := proc(ce::table)
            local coeffDict, actualValue, hasValue, validQs, q, mainQ, mainCoeff,
                  otherQ, otherCoeff, ratio, val, removedQs,
                  i, j, rowVec, idxList, nVars, origIdx, k, vList, orderedQs, newNVars, newRow, 
                  newEqList, newQList, newQIndex, exponent, factor, mainIdx, otherIdx, otherExp, isZero, x;

            if not assigned(ce[CoeffDict]) then error ""Missing CoeffDict in ce""; end if;
            coeffDict := ce[CoeffDict]; hasValue := false;
            if assigned(ce[ActualValue]) and ce[ActualValue] <> NULL then hasValue := true; actualValue := ce[ActualValue]; end if;

            idxList := [indices(coeffDict, 'nolist')]; validQs := Array(1..0);
            for q in idxList do if assigned(QIndex[q]) then validQs ,= q; end if; end do;
            if numelems(validQs) = 0 then return false; end if;

            nVars := numelems(QList); isZero := x -> evalb(x = 0) or evalb(normal(x) = 0);

            if hasValue then
                for i from 1 to numelems(validQs) do
                    q := validQs[i]; val := simplify(actualValue / coeffDict[q]); origIdx := QIndex[q];
                    for j from 1 to numelems(EqList) do
                        rowVec := EqList[j]; exponent := rowVec[origIdx];
                        if not isZero(exponent) then
                            factor := simplify(val^exponent);
                            rowVec[nVars+1] := simplify(rowVec[nVars+1] / factor);
                        end if;
                    end do;
                end do;
                removedQs := {seq(validQs[i], i=1..numelems(validQs))};
            else
                if numelems(validQs) < 2 then return false; end if;
                mainQ := validQs[1]; mainCoeff := coeffDict[mainQ]; mainIdx := QIndex[mainQ]; removedQs := {};
                for i from 2 to numelems(validQs) do
                    otherQ := validQs[i]; otherCoeff := coeffDict[otherQ]; otherIdx := QIndex[otherQ];
                    ratio := simplify(mainCoeff / otherCoeff);
                    for j from 1 to numelems(EqList) do
                        rowVec := EqList[j]; otherExp := rowVec[otherIdx];
                        if not isZero(otherExp) then
                            rowVec[mainIdx] := simplify(rowVec[mainIdx] + otherExp);
                            factor := simplify(ratio^otherExp);
                            rowVec[nVars+1] := simplify(rowVec[nVars+1] / factor);
                        end if;
                    end do;
                    removedQs := removedQs union {otherQ};
                end do;
            end if;

            newQList := Array(1..0);
            for i from 1 to nVars do q := QList[i]; if not member(q, removedQs) then newQList ,= q; end if; end do;
            newQIndex := table();
            for i from 1 to numelems(newQList) do newQIndex[newQList[i]] := i; end do;

            newNVars := numelems(newQList); newEqList := Array(1..0);
            for i from 1 to numelems(EqList) do
                rowVec := EqList[i]; newRow := Array(1..newNVars+1, 0);
                for j from 1 to newNVars do origIdx := QIndex[newQList[j]]; newRow[j] := rowVec[origIdx]; end do;
                newRow[newNVars+1] := rowVec[nVars+1]; newEqList ,= newRow;
            end do;

            EqList := newEqList; QList := newQList; QIndex := newQIndex;
            IsReduced := false; ReducedMat := NULL; return true;
        end proc;

        HasContradiction := proc() 
            if not IsReduced then Eliminate(); end if; 
            return HasContradictionFlag; 
        end proc;

        GetVariableNames := proc()
            local x;
            StringTools:-Join(
                map(x -> sprintf(""%a"", x), convert(QList, list)), 
                "",""
            );
        end proc;

        GetMatrixString := proc()
            local m, n, i, j, rowStrs, rowStr, res;
            if not IsReduced then Eliminate(); end if;
            if HasContradictionFlag or ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then return ""[]""; end if;
            m := LinearAlgebra:-RowDimension(ReducedMat); n := LinearAlgebra:-ColumnDimension(ReducedMat) - 1;
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
                res := cat(res, rowStrs[i]); if i < numelems(rowStrs) then res := cat(res, "",""); end if;
            end do;
            return cat(res, ""]"");
        end proc;

        GetConstantsString := proc()
            local m, i, res;
            if not IsReduced then Eliminate(); end if;
            if HasContradictionFlag or ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then return ""[]""; end if;
            m := LinearAlgebra:-RowDimension(ReducedMat); res := ""["";
            for i from 1 to m do
                res := cat(res, sprintf(""%a"", normal(ReducedMat[i, LinearAlgebra:-ColumnDimension(ReducedMat)])));
                if i < m then res := cat(res, "",""); end if;
            end do;
            return cat(res, ""]"");
        end proc;

        CalculateSparsity := proc(useReduced::boolean := false)
            local nVars, nEqs, i, j, totalElements, zeroCount, val, isZero, x;
            isZero := x -> evalb(x = 0) or evalb(normal(x) = 0);
            if useReduced then
                if not IsReduced then Eliminate(); end if;
                if HasContradictionFlag or ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then return 1.0; end if;
                nEqs := LinearAlgebra:-RowDimension(ReducedMat); nVars := LinearAlgebra:-ColumnDimension(ReducedMat) - 1; 
                if nVars = 0 then return 1.0; end if;
                totalElements := nEqs * nVars; zeroCount := 0;
                for i from 1 to nEqs do for j from 1 to nVars do if isZero(ReducedMat[i, j]) then zeroCount := zeroCount + 1; end if; end do; end do;
            else
                nEqs := numelems(EqList); if nEqs = 0 then return 1.0; end if;
                nVars := numelems(QList); if nVars = 0 then return 1.0; end if;
                totalElements := nEqs * nVars; zeroCount := 0;
                for i from 1 to nEqs do for j from 1 to nVars do if isZero(EqList[i][j]) then zeroCount := zeroCount + 1; end if; end do; end do;
            end if;
            return evalf(zeroCount / totalElements);
        end proc;

        DiscoverRelations := proc()
            local m, n, nVars, i, j, k, idx, isZero, nonZeroCols, c1, c2, q1, q2, constVal, finalVal,
                  valList, relList, outputList, pivotCols, exprOfVar, keys, colA, colB, rowA, rowB, constA, constB, coeffsA, coeffsB, equal, qA, qB, j1, j2, x, coeffsEqual, ratio;
            if not IsReduced then Eliminate(); end if;
            if HasContradictionFlag or ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then return []; end if;
            m := LinearAlgebra:-RowDimension(ReducedMat); n := LinearAlgebra:-ColumnDimension(ReducedMat); nVars := n - 1;
            isZero := x -> evalb(normal(x) = 0);
            valList := Array(1..0); relList := Array(1..0);

            for i from 1 to m do
                nonZeroCols := Array(1..0);
                for j from 1 to nVars do if not isZero(ReducedMat[i, j]) then nonZeroCols ,= j; end if; end do;
                if numelems(nonZeroCols) = 1 then
                    j := nonZeroCols[1]; q1 := QList[j]; c1 := ReducedMat[i, j]; constVal := ReducedMat[i, n];
                    if evalb(normal(c1) = 1) then finalVal := constVal; else finalVal := simplify(constVal^(1/c1)); end if;
                    valList ,= [q1, finalVal];
                elif numelems(nonZeroCols) = 2 then
                    j1 := nonZeroCols[1]; j2 := nonZeroCols[2]; q1 := QList[j1]; q2 := QList[j2];
                    c1 := ReducedMat[i, j1]; c2 := ReducedMat[i, j2]; constVal := ReducedMat[i, n];
                    if evalb(normal(c1) = 1) and evalb(normal(c2) = -1) then relList ,= [q1, q2, constVal];
                    elif evalb(normal(c1) = -1) and evalb(normal(c2) = 1) then relList ,= [q2, q1, constVal]; end if;
                end if;
            end do;

            pivotCols := table();
            for i from 1 to m do
                for j from 1 to nVars do if not isZero(ReducedMat[i, j]) then pivotCols[j] := i; break; end if; end do;
            end do;
            exprOfVar := table(); keys := [indices(pivotCols, 'nolist')];
            for idx from 1 to numelems(keys) do
                colA := keys[idx]; rowA := pivotCols[colA]; constA := ReducedMat[rowA, n]; coeffsA := Array(1..nVars, 0);
                for j from 1 to nVars do if j <> colA then coeffsA[j] := normal(-ReducedMat[rowA, j]); end if; end do;
                exprOfVar[colA] := [constA, coeffsA];
            end do;
            
            # 增强跨行知识发现：仅对比变元系数，不强制要求常量相等
            for i from 1 to numelems(keys) do
                colA := keys[i]; constA := exprOfVar[colA][1]; coeffsA := exprOfVar[colA][2];
                for j from i+1 to numelems(keys) do
                    colB := keys[j]; constB := exprOfVar[colB][1]; coeffsB := exprOfVar[colB][2];
                    
                    coeffsEqual := true;
                    for k from 1 to nVars do
                        if not evalb(normal(coeffsA[k]) = normal(coeffsB[k])) then
                            coeffsEqual := false;
                            break;
                        end if;
                    end do;
                    
                    if coeffsEqual then
                        qA := QList[colA]; 
                        qB := QList[colB]; 
                        
                        if evalb(normal(constB) = 0) then
                            if evalb(normal(constA) = 0) then
                                relList ,= [qA, qB, 1];
                            end if;
                        else
                            ratio := simplify(constA / constB);
                            relList ,= [qA, qB, ratio]; 
                        end if;
                    end if;
                end do;
            end do;
            return [seq(valList[i], i=1..numelems(valList)), seq(relList[i], i=1..numelems(relList))];
        end proc;

        GetMatrixSize := proc()
            local m, n; 
            if not IsReduced then Eliminate(); end if;
            if ReducedMat = NULL or LinearAlgebra:-RowDimension(ReducedMat) = 0 then return [0, numelems(QList)]; end if;
            return [LinearAlgebra:-RowDimension(ReducedMat), LinearAlgebra:-ColumnDimension(ReducedMat) - 1];
        end proc;
    end module;
    return sys;
end proc:";
    #endregion

    #region Fields
    private static readonly MapleApp _mapleApp = MapleApp.Instance;
    private static bool _isMapleCodeLoaded = false;
    private readonly string _matrixName;
    private readonly List<string> _log = new();
    private readonly List<string> _commands = new();
    public IReadOnlyList<string> Log => _log;
    public string LastLog=> StringTool.ComposeList(Log, "\n");
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
    public MapleBaseProductionMatrix()
    {
        EnsureMapleLoaded();
        _matrixName = $"sys_{Guid.NewGuid().ToString("N")[..8]}";
        RunCommand($"{_matrixName} := CreateMultiplicativeSystem();");
    }
    public string MatrixName => _matrixName;
    #endregion

    #region 
    public void AddEquation(Dictionary<Quantity, Expr> coeffDict, Expr constant)
    {
        if (coeffDict == null || coeffDict.Count == 0) return;

        var pairs = string.Join(", ", coeffDict.Select(kv => $"{kv.Key}={kv.Value}"));
        string cmd = $"{_matrixName}:-AddEquation(table([{pairs}]), {constant}):";
        RunCommand(cmd);
        if (IsLog)
        {
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
    public void Eliminate() { RunCommand($"{_matrixName}:-Eliminate();"); _log.Add(GetCompleteMatrixString()); }
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
    public bool ReduceColumnsByContinuedEquality(
        Dictionary<Quantity, Expr> coeffDict,
        Expr actualValue = null,
        Quantity baseVar = null)
    {
        var pairs = string.Join(", ", coeffDict.Select(kv => $"{kv.Key}={kv.Value}"));
        string avParam = actualValue is null ? "NULL" : actualValue.ToString();
        string bvParam = baseVar == null ? "NULL" : baseVar.ToString();

        string cmd = $"{_matrixName}:-ReduceColumnsByContinuedEquality(table([CoeffDict=table([{pairs}]), ActualValue={avParam}, BaseVar={bvParam}]));";
        var res = RunCommand(cmd);
        if (IsLog)
            _log.Add(GetCompleteMatrixString());
        return bool.TryParse(res.Trim(), out bool val) && val;
    }
    #endregion

    #region Matrix And Variable Export

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
    public double CalculateSparsity(bool onlyActiveRegion=true)
    {
        var res = RunCommand($"{_matrixName}:-CalculateSparsity();").Trim();
        return double.TryParse(res, out double val) ? val : 1;
    }
    public double CalculateDensity(bool onlyActiveRegion = true)
        => 1.0 - CalculateSparsity(onlyActiveRegion);
    public List<Knowledge> DiscoverRelations()
    {
        List<Knowledge> result = [];
        var res = RunCommand($"{_matrixName}:-DiscoverRelations();").Trim();
        if (IsLog)
        {
            _log.Add($"DiscoverRelations 输出: {res}");
        }
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
                    pred.Reason = "乘积矩阵发现";
                else
                    pred.Reason = "DiscoveredByProductionMatrix";
                result.Add(pred);
            }
            else if (item.Count == 3)
            {
                Quantity pVar = Quantity.Parse(item[0]);
                Quantity qVar = Quantity.Parse(item[1]);
                QuantityRatio pred = new QuantityRatio(pVar, qVar, item[2]);
                if (GeoInferenceApp.IsZhOrEn)
                    pred.Reason = "乘积矩阵发现";
                else
                    pred.Reason = "DiscoveredByProductionMatrix";
                result.Add(pred);
            }
        }
        return result;
    }
    private string RunCommand(string command)
    {
        _commands.Add(command);
        if (IsLog)
            _log.Add($"Run command: {command}");
        var result = _mapleApp.Run(command);
        if (result != null && result.Contains("Error"))
            throw new MapleExecutionException($"Maple execution error: {command}\nDetails: {result}");
        return result ?? "";
    }
    private static string[,] ParseMapleMatrix(string mapleStr)
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