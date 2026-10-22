using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using ClosedXML.Excel;

using ScottPlot;

namespace GeoInference.Tests;

internal class IntegratedResultReporter
{
    public const string DefaultResultPath = "..\\..\\..\\..\\Result";

    /// <summary>
    /// 解决方案文件名，用于定位解决方案根目录。
    /// </summary>
    private const string SolutionFileName = "GeoInference.slnx";

    /// <summary>
    /// 报告目录名（不绑定单一层级）：报告覆盖 FormalGeo7K 的 L1~L6 全部层级。
    /// </summary>
    private const string ReportGroupName = "FormalGeo7K";

    /// <summary>
    /// 推理结果与报告的根目录（绝对路径）。推理结果的读写与报告输出共用此目录，
    /// 避免因进程工作目录不同而导致结果与报告落在不同位置。
    /// </summary>
    public static string ResultRoot { get; } = ResolveResultRoot();

    /// <summary>
    /// 从可执行文件所在目录向上定位解决方案根目录，返回其中的 Result 目录；
    /// 找不到解决方案文件时退回 <see cref="DefaultResultPath"/> 的原有相对路径语义。
    /// </summary>
    public static string ResolveResultRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, SolutionFileName)))
                return Path.Combine(dir.FullName, "Result");
            dir = dir.Parent;
        }
        return Path.GetFullPath(DefaultResultPath);
    }

    public static readonly string[] AllDatasets =
    {
        GeoProblemDataset.FormalGeo7KL1,
        GeoProblemDataset.FormalGeo7KL2,
        GeoProblemDataset.FormalGeo7KL3,
        GeoProblemDataset.FormalGeo7KL4,
        GeoProblemDataset.FormalGeo7KL5,
        GeoProblemDataset.FormalGeo7KL6,
    };

    /// <summary>
    /// 报告输出目录：<c>Result/FormalGeo7K_Reports</c>（含 L1~L6 层级子目录）。
    /// </summary>
    public string ReportDir => Path.Combine(ResultRoot, $"{ReportGroupName}_Reports");

    private string _levelOutputDir = null;
    private string CurrentOutputDir => _levelOutputDir ?? ReportDir;

    public static string LevelOf(string datasetName) => datasetName.Replace("FormalGeo7K", "");

    /// <summary>
    /// 全部层级（L1~L6）的题目总数。
    /// </summary>
    public int Total { get; private set; }

    private readonly Dictionary<string, int> _levelTotals = new();

    /// <summary>
    /// 统计各层级的题目总数，并把 <see cref="Total"/> 设为全部层级之和。
    /// </summary>
    public void InitTotals()
    {
        _levelTotals.Clear();
        foreach (var datasetName in AllDatasets)
        {
            _levelTotals[LevelOf(datasetName)] = GeoProblemDataset.Instance[datasetName]?.ProblemNames.Count ?? 0;
        }
        Total = _levelTotals.Values.Sum();
    }

    /// <summary>
    /// 获取指定层级的题目总数（用于计算该层级的解题成功率）。
    /// </summary>
    public int GetLevelTotal(string level)
        => _levelTotals.TryGetValue(level, out int count) ? count : 0;

    public Dictionary<string, GeoInferenceResult> SolvingResults { get; } = new();
    public Dictionary<string, GeoInferenceResult> SolvingNoCompassMatrixResults { get; } = new();
    public Dictionary<string, GeoInferenceResult> SolvingNoSimplifyEqSymResults { get; } = new();
    public Dictionary<string, GeoInferenceResult> DiscoveringResults { get; } = new();
    public Dictionary<string, GeoInferenceResult> DiscoveringNoCompassMatrixResults { get; } = new();
    public Dictionary<string, GeoInferenceResult> DiscoveringNoSimplifyEqSymResults { get; } = new();

    public Dictionary<string, string> ShorterMap = new Dictionary<string, string>()
    {
        {nameof(RuleCongTri.RuleCT003TriangleCongruenceASA),"TriCongASA" },
        {nameof(RuleCongTri.RuleCT004TriangleCongruenceAAS),"TriCongAAS" },
        {nameof(RuleCongTri.RuleCT008TriangleSimilaritySSS),"TriSimSSS" },
        {nameof(RuleCongTri.RuleCT006TriangleSimilarityAA),"TriSimAA" },
        {nameof(RuleCongTri.RuleCT002TriangleCongruenceSAS),"TriCongSAS" },
        {nameof(RuleCongTri.RuleCT007TriangleSimilaritySAS),"TriSimSAS" },
        {nameof(DistanceQuantityRule.RuleDQ007ProportionalSegmentsOnTransversals),"ProSeg" },
        {nameof(AngleQuantityRule.RuleAQ009CorrespondingAnglesEqual),"CorrAng" },
        {nameof(AngleQuantityRule.RuleAQ008ConsecutiveInteriorAnglesSupplementary),"ConAng" },
        {nameof(AngleQuantityRule.RuleAQ005AngleBisectorInference),"AngleBisector" },
        {nameof(AngleQuantityRule.RuleAQ007AlternateInteriorAnglesEqual),"AlterAnglesEqual" },
    };

    public void LoadAllData(string datasetName = GeoProblemDataset.FormalGeo7KL6)
    {
        LoadData(datasetName, nameof(GeoInferenceConfigs.Solving), SolvingResults);
        LoadData(datasetName, nameof(GeoInferenceConfigs.SolvingNoCompassMatrix), SolvingNoCompassMatrixResults);
        LoadData(datasetName, nameof(GeoInferenceConfigs.SolvingNoSimplifyEqSym), SolvingNoSimplifyEqSymResults);
        LoadData(datasetName, nameof(GeoInferenceConfigs.Discovering), DiscoveringResults);
    }

    public void GenerateAllLevelReports()
    {
        Directory.CreateDirectory(ReportDir);
        InitTotals();
        var mriPlots = new List<(string Level, Plot Plot)>();
        var smriPlots = new List<(string Level, Plot Plot)>();
        var levelResourceUsage = new Dictionary<string, double[]>();
        var allSolvingTime = new List<double>();
        var allDiscoveringTime = new List<double>();
        var allSolvingMem = new List<double>();
        var allDiscoveringMem = new List<double>();
        var allSolvingRi = new List<double>();
        var allDiscoveringRi = new List<double>();
        using var metricsWorkbook = new XLWorkbook();
        foreach (var datasetName in AllDatasets)
        {
            LoadAllData(datasetName);
            _currentLevel = LevelOf(datasetName);
            _levelOutputDir = Path.Combine(ReportDir, _currentLevel);
            Directory.CreateDirectory(_levelOutputDir);
            AddMetricsTables(metricsWorkbook, _currentLevel);

            var solvingTime = SolvingResults.Values.Select(r => r.InferenceWallClockTime.TotalSeconds).ToList();
            var discoveringTime = DiscoveringResults.Values.Select(r => r.InferenceWallClockTime.TotalSeconds).ToList();
            // 峰值内存 / 规则实例化次数仅统计成功解出的实例（fix2：明确口径，成功 = HumanLikeAnswer.IsAllSuccess）。
            var solvingMem = SolvingResults.Values
                .Where(r => (r.HumanLikeAnswer?.IsAllSuccess ?? false) && r.PeakMemoryBytes > 0)
                .Select(r => r.PeakMemoryBytes / 1048576.0).ToList();
            var discoveringMem = DiscoveringResults.Values
                .Where(r => (r.HumanLikeAnswer?.IsAllSuccess ?? false) && r.PeakMemoryBytes > 0)
                .Select(r => r.PeakMemoryBytes / 1048576.0).ToList();
            var solvingRi = SolvingResults.Values
                .Where(r => (r.HumanLikeAnswer?.IsAllSuccess ?? false) && r.InferenceMetrics?.ActualRuleInstantiationCount > 0)
                .Select(r => (double)r.InferenceMetrics.ActualRuleInstantiationCount).ToList();
            var discoveringRi = DiscoveringResults.Values
                .Where(r => (r.HumanLikeAnswer?.IsAllSuccess ?? false) && r.InferenceMetrics?.ActualRuleInstantiationCount > 0)
                .Select(r => (double)r.InferenceMetrics.ActualRuleInstantiationCount).ToList();
            allSolvingTime.AddRange(solvingTime);
            allDiscoveringTime.AddRange(discoveringTime);
            allSolvingMem.AddRange(solvingMem);
            allDiscoveringMem.AddRange(discoveringMem);
            allSolvingRi.AddRange(solvingRi);
            allDiscoveringRi.AddRange(discoveringRi);
            levelResourceUsage[_currentLevel] = new[]
            {
                AvgOrNaN(solvingTime), AvgOrNaN(discoveringTime),
                AvgOrNaN(solvingMem), AvgOrNaN(discoveringMem),
                AvgOrNaN(solvingRi), AvgOrNaN(discoveringRi)
            };
            var (mriPlot, smriPlot) = GenerateMetricsCharts();
            if (mriPlot != null)
                mriPlots.Add((_currentLevel, mriPlot));
            if (smriPlot != null)
                smriPlots.Add((_currentLevel, smriPlot));
        }

        AddResourceUsageSummarySheets(metricsWorkbook, levelResourceUsage, new[]
        {
            AvgOrNaN(allSolvingTime), AvgOrNaN(allDiscoveringTime),
            AvgOrNaN(allSolvingMem), AvgOrNaN(allDiscoveringMem),
            AvgOrNaN(allSolvingRi), AvgOrNaN(allDiscoveringRi)
        });

        string metricsPath = Path.Combine(ReportDir, "MetricsTables.xlsx");
        metricsWorkbook.SaveAs(metricsPath);
        Console.WriteLine($"Metrics Tables Saved: {metricsPath}");

        _levelOutputDir = null;
        _currentLevel = "L6";
        LoadAllData();

        GenerateCombinedCompressionRateChart(mriPlots, "Discovered");
        GenerateCombinedCompressionRateChart(smriPlots, "Solved-state");
    }

    public void LoadData(string datasetName, string config, Dictionary<string, GeoInferenceResult> results, string resultDir = null)
    {
        resultDir ??= ResultRoot;
        var dataDir = Path.Combine(resultDir, $"{datasetName}_{config}");
        if (!Directory.Exists(dataDir)) return;
        results.Clear();
        var files = Directory.EnumerateFiles(dataDir, "*.json").ToList();

        foreach (var file in files)
        {
            string json = File.ReadAllText(file);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<GeoInferenceResult>(json);
            if (result != null)
            {
                string key = Path.GetFileNameWithoutExtension(file);
                results[key] = result;
            }
        }
    }

    public void GenerateStatuAndInferenceTimeReportExcel()
    {
        using var wb = new XLWorkbook();
        foreach (var datasetName in AllDatasets)
        {
            var solving = new Dictionary<string, GeoInferenceResult>();
            var solvingNoCompass = new Dictionary<string, GeoInferenceResult>();
            var solvingNoSimplify = new Dictionary<string, GeoInferenceResult>();
            var discovering = new Dictionary<string, GeoInferenceResult>();

            LoadData(datasetName, nameof(GeoInferenceConfigs.Solving), solving);
            LoadData(datasetName, nameof(GeoInferenceConfigs.SolvingNoCompassMatrix), solvingNoCompass);
            LoadData(datasetName, nameof(GeoInferenceConfigs.SolvingNoSimplifyEqSym), solvingNoSimplify);
            LoadData(datasetName, nameof(GeoInferenceConfigs.Discovering), discovering);

            AddCombinedSheet(wb, datasetName, solving, solvingNoCompass, solvingNoSimplify, discovering);
        }

        string reportPath = Path.Combine(ReportDir, "StatuAndInferenceTimeReport.xlsx");
        var outputDir = Path.GetDirectoryName(reportPath);
        if (!string.IsNullOrEmpty(outputDir))
            Directory.CreateDirectory(outputDir);
        wb.SaveAs(reportPath);
    }

    private void AddCombinedSheet(
        XLWorkbook wb,
        string datasetName,
        Dictionary<string, GeoInferenceResult> solving,
        Dictionary<string, GeoInferenceResult> solvingNoCompass,
        Dictionary<string, GeoInferenceResult> solvingNoSimplify,
        Dictionary<string, GeoInferenceResult> discovering)
    {
        var ws = wb.Worksheets.Add(datasetName);

        var allDicts = new Dictionary<string, GeoInferenceResult>[]
        {
            solving,
            solvingNoCompass,
            solvingNoSimplify,
            discovering
        };
        var solvingDicts = new Dictionary<string, GeoInferenceResult>[]
        {
            solving,
            solvingNoCompass,
            solvingNoSimplify
        };

        var allQuestions = new HashSet<string>();
        foreach (var dict in allDicts)
        {
            foreach (var key in dict.Keys) allQuestions.Add(key);
        }
        var sortedQuestions = allQuestions.OrderBy(q => q, StringComparer.Ordinal).ToList();

        string[] headers =
        {
            "ProblemName",
            "Solving\n(Finished)", "Solving_NoCompass\n(Finished)", "Solving_NoSimplify\n(Finished)", "Discovering\n(Finished)",
            "Solving\n(Solved)", "Solving_NoCompass\n(Solved)", "Solving_NoSimplify\n(Solved)",
            "Solving\nInference Wall-Clock(s)", "Solving_NoCompass\nInference Wall-Clock(s)", "Solving_NoSimplify\nInference Wall-Clock(s)", "Discovering\nInference Wall-Clock(s)",
            "Solving\nMemory(MB)", "Solving_NoCompass\nMemory(MB)", "Solving_NoSimplify\nMemory(MB)", "Discovering\nMemory(MB)",
            "Solving\nActual RI"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = false;
            cell.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }
        ws.Row(1).Height = 30;

        const int statusStartCol = 2;
        int solvedStartCol = statusStartCol + allDicts.Length;
        int timeStartCol = solvedStartCol + solvingDicts.Length;
        int memoryStartCol = timeStartCol + allDicts.Length;
        int actualRiCol = memoryStartCol + allDicts.Length;

        for (int i = 0; i < sortedQuestions.Count; i++)
        {
            int row = i + 2;
            string question = sortedQuestions[i];

            var nameCell = ws.Cell(row, 1);
            nameCell.Value = question;
            nameCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            nameCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            for (int col = 0; col < allDicts.Length; col++)
            {
                var cell = ws.Cell(row, statusStartCol + col);
                if (allDicts[col].TryGetValue(question, out var result))
                {
                    if (!result.IsCracked)
                    {
                        cell.Value = "Pass";
                        cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                    }
                    else
                    {
                        cell.Value = "Cracked";
                        cell.Style.Fill.BackgroundColor = XLColor.LightCoral;
                    }
                }
                else
                {
                    cell.Value = "-";
                }
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            for (int col = 0; col < solvingDicts.Length; col++)
            {
                var cell = ws.Cell(row, solvedStartCol + col);
                if (solvingDicts[col].TryGetValue(question, out var result))
                {
                    bool isSuccess = result.HumanLikeAnswer?.IsAllSuccess ?? false;

                    if (isSuccess)
                    {
                        cell.Value = "Success";
                        cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                    }
                    else
                    {
                        cell.Value = "Fail";
                        cell.Style.Fill.BackgroundColor = XLColor.LightCoral;
                    }
                }
                else
                {
                    cell.Value = "-";
                }
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            for (int col = 0; col < allDicts.Length; col++)
            {
                var cell = ws.Cell(row, timeStartCol + col);
                if (allDicts[col].TryGetValue(question, out var result))
                {
                    double seconds = result.InferenceWallClockTime.TotalSeconds;
                    cell.Value = seconds;
                    cell.Style.NumberFormat.Format = "0.00";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else
                {
                    cell.Value = "-";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }

            for (int col = 0; col < allDicts.Length; col++)
            {
                var cell = ws.Cell(row, memoryStartCol + col);
                if (allDicts[col].TryGetValue(question, out var result) && result.PeakMemoryBytes > 0)
                {
                    cell.Value = result.PeakMemoryBytes / 1048576.0;
                    cell.Style.NumberFormat.Format = "0.0";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else
                {
                    cell.Value = "-";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }

            {
                var cell = ws.Cell(row, actualRiCol);
                if (solving.TryGetValue(question, out var result)
                    && result.InferenceMetrics != null
                    && result.InferenceMetrics.ActualRuleInstantiationCount > 0)
                {
                    cell.Value = (double)result.InferenceMetrics.ActualRuleInstantiationCount;
                    cell.Style.NumberFormat.Format = "#,##0";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                }
                else
                {
                    cell.Value = "-";
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }
            }
        }

        if (sortedQuestions.Count > 0)
        {
            ws.Range(1, 1, sortedQuestions.Count + 1, headers.Length).SetAutoFilter();
        }

        ws.Columns().AdjustToContents();

        foreach (var col in ws.Columns())
        {
            if (col.Width > 40) col.Width = 40;
        }

        ws.SheetView.FreezeRows(1);
        ws.SheetView.FreezeColumns(1);
    }

    /// <summary>
    /// 向已有工作簿追加指定层级的指标表格（两个 sheet：层级主表 + InferenceEfficiency 子表）。
    /// </summary>
    public void AddMetricsTables(XLWorkbook workbook, string level)
    {
        // One sheet per level: main tables + inference-efficiency Top-5 tables merged into a single page
        var ws = workbook.AddWorksheet(level);

        int currentRow = 1;

        currentRow = GenerateResourceUsageTable(ws, currentRow, SolvingResults, DiscoveringResults, "Table 1: Resource Usage (Inference Wall-Clock / Memory / Actual RI)");
        currentRow += 2;

        currentRow = GenerateCombinedCumulativeTimeTable(ws, currentRow, SolvingResults, "Table 2: Cumulative Time Distribution (Solved-State)");
        currentRow += 2;

        currentRow = GenerateTop5InferenceEfficiencyTables(ws, currentRow, SolvingResults, "Solved-State", 3);
        currentRow += 2;

        currentRow = GenerateAblationTable(ws, currentRow, SolvingResults, SolvingNoCompassMatrixResults, SolvingNoSimplifyEqSymResults, GetLevelTotal(_currentLevel), "Table 6: Ablation Study on Success Rate and Efficiency");
        currentRow += 2;

        currentRow = GenerateTop5InferenceEfficiencyTables(ws, currentRow, DiscoveringResults, "Discovering", 7);
        currentRow += 2;

        currentRow = GenerateChainedEqualityNecessityTable(ws, currentRow, DiscoveringResults, "Table 10: Matrix Sparsity and Equality Chain Metrics");

        ws.Columns().AdjustToContents();
    }

    /// <summary>
    /// </summary>
    public (Plot Mri, Plot Smri) GenerateMetricsCharts()
    {
        if (DiscoveringResults.Count == 0 && SolvingResults.Count == 0)
        {
            Console.WriteLine("[GenerateMetricsCharts] 结果为空，跳过图表生成。");
            return (null, null);
        }

        Plot mriPlot = null;
        if (DiscoveringResults.Count > 0)
        {
            RulePairGroupedByParamsCountChart();
            var a = PreidcateCountCompare(DiscoveringResults);
            var b = SemiRulePairCountCompare(DiscoveringResults);
            var c = MergedRulePairCountCompare(DiscoveringResults);
            Top5CompressionRate(new[] { a, b, c }, name: "Discovering");

            mriPlot = CompressionRate();
        }

        if (SolvingResults.Count > 0)
        {
            var a = PreidcateCountCompare(SolvingResults);
            var b = SemiRulePairCountCompare(SolvingResults);
            var c = MergedRulePairCountCompare(SolvingResults);
            Top5CompressionRate(new[] { a, b, c }, name: "Solving");
        }

        Plot smriPlot = SolvingResults.Count > 0 ? SolvingCompressionRate() : null;
        return (mriPlot, smriPlot);
    }

    #region Tables

    private int GenerateChainedEqualityNecessityTable(IXLWorksheet ws, int startRow, Dictionary<string, GeoInferenceResult> source, string title)
    {
        if (source.Count == 0) return startRow;

        var matrixSparsityData = source
            .SelectMany(r => r.Value.InferenceMetrics.MatrixSparsities)
            .ToList();

        var qvQrProportionData = source
            .Select(r => r.Value.InferenceMetrics.QvQrProportion)
            .ToList();

        var inEqualityChainRatioData = source
            .Select(r => r.Value.InferenceMetrics.InEqualityChainQuantityRatio)
            .ToList();

        var (avgSparsity, varSparsity) = CalStats(matrixSparsityData);
        var (avgQvQr, varQvQr) = CalStats(qvQrProportionData);
        var (avgInEquality, varInEquality) = CalStats(inEqualityChainRatioData);

        var headers = new[] { "Metric", "Average", "Population Variance" };
        var data = new List<string[]>
        {
            new[] { "Matrix Sparsity",              avgSparsity.ToString("P2"),   varSparsity.ToString("F4") },
            new[] { "QvQr Proportion",              avgQvQr.ToString("P2"),       varQvQr.ToString("F4") },
            new[] { "InEquality Chain Quantity Ratio", avgInEquality.ToString("P2"), varInEquality.ToString("F4") }
        };

        return WriteThreeLineTable(ws, startRow, title, headers, data);
    }

    private string _currentLevel = "L6";

    /// <summary>
    /// </summary>
    private static readonly (string Method, IReadOnlyDictionary<string, double> Pssr)[] BaselinePssr =
    {
        ("PN-BFS",         new Dictionary<string, double> { ["L1"] = 0.5687, ["L2"] = 0.3921, ["L3"] = 0.3281, ["L4"] = 0.2277, ["L5"] = 0.1169, ["L6"] = 0.0632 }),
        ("FGeo-DRL",       new Dictionary<string, double> { ["L1"] = 0.9797, ["L2"] = 0.8836, ["L3"] = 0.7261, ["L4"] = 0.6039, ["L5"] = 0.5542, ["L6"] = 0.3962 }),
        ("FGeo-HyperGNet", new Dictionary<string, double> { ["L1"] = 0.9596, ["L2"] = 0.9417, ["L3"] = 0.9067, ["L4"] = 0.7465, ["L5"] = 0.6552, ["L6"] = 0.5882 }),
        ("DeepSeekv3",     new Dictionary<string, double> { ["L1"] = 0.7319, ["L2"] = 0.5889, ["L3"] = 0.6119, ["L4"] = 0.4507, ["L5"] = 0.4310, ["L6"] = 0.4118 }),
        ("FGeo-NSS",       new Dictionary<string, double> { ["L1"] = 0.9692, ["L2"] = 0.9306, ["L3"] = 0.9241, ["L4"] = 0.8099, ["L5"] = 0.7255, ["L6"] = 0.4844 }),
        ("FGeo-ISRL",      new Dictionary<string, double> { ["L1"] = 0.9820, ["L2"] = 0.9635, ["L3"] = 0.8831, ["L4"] = 0.7821, ["L5"] = 0.6022, ["L6"] = 0.2921 }),
    };

    private int GenerateSuccessRateTable(IXLWorksheet ws, int startRow)
    {
        if (SolvingResults.Count == 0) return startRow;

        var solvingSuccessCount = SolvingResults.Count(r => r.Value.HumanLikeAnswer?.IsAllSuccess ?? false);
        int levelTotal = GetLevelTotal(_currentLevel);
        double ourSuccessRate = levelTotal > 0 ? (double)solvingSuccessCount / levelTotal : 0;

        var headers = new[] { "Method", "PSSR" };

        var data = BaselinePssr
            .Select(b => new[] { b.Method, b.Pssr[_currentLevel].ToString("P2") })
            .ToList();
        data.Add(new[] { "KM-SCR(Ours)", ourSuccessRate.ToString("P2") });

        return WriteThreeLineTable(ws, startRow, $"Table 2: FormalGeo7K {_currentLevel} PSSR ({solvingSuccessCount}/{levelTotal})", headers, data);
    }

    private static double AvgOrNaN(List<double> values)
        => values.Count > 0 ? values.Average() : double.NaN;

    /// <summary>
    /// Resource usage summary sheet (averages only) for the Solving mode.
    /// </summary>
    public void AddResourceUsageSummarySheets(XLWorkbook workbook,
        IReadOnlyDictionary<string, double[]> levelAverages, double[] overallAverages)
    {
        AddResourceUsageSummarySheet(workbook, "Solving", 0, levelAverages, overallAverages);

        // Place the summary sheet at the front, before the per-level sheets
        workbook.Worksheet("Solving").Position = 1;
    }

    /// <summary>
    /// 单个模式的资源使用汇总页。<paramref name="offset"/> 表示该模式在
    /// [Solving 时间, Discovering 时间, Solving 内存, Discovering 内存, Solving RI, Discovering RI]
    /// 数组中的起始下标。
    /// </summary>
    private void AddResourceUsageSummarySheet(XLWorkbook workbook, string mode, int offset,
        IReadOnlyDictionary<string, double[]> levelAverages, double[] overallAverages)
    {
        var ws = workbook.AddWorksheet(mode);

        string[] levels = AllDatasets.Select(LevelOf).ToArray();
        var headers = new[] { "Metric" }.Concat(levels).Concat(new[] { "Overall" }).ToArray();

        string[] metricNames =
        {
            "Inference Wall-Clock Time (s)",
            "Peak Memory (MB)",
            "Actual Rule Instantiations"
        };

        string Fmt(double v) => double.IsNaN(v) ? "-" : v.ToString("F2");

        var data = new List<string[]>();
        for (int m = 0; m < metricNames.Length; m++)
        {
            int index = offset + m * 2;
            var row = new List<string> { metricNames[m] };
            foreach (var level in levels)
            {
                row.Add(levelAverages.TryGetValue(level, out var avgs) && index < avgs.Length
                    ? Fmt(avgs[index])
                    : "-");
            }
            row.Add(overallAverages != null && index < overallAverages.Length ? Fmt(overallAverages[index]) : "-");
            data.Add(row.ToArray());
        }

        int nextRow = WriteThreeLineTable(ws, 1, $"Resource Usage Summary (Average) - {mode}", headers, data);

        // fix2 脚注：明确统计口径（Peak Memory / Rule Instantiations 仅统计成功实例；Wall-Clock Time 为全部实例）。
        var footnote = ws.Cell(nextRow + 1, 1);
        footnote.Value = "Note: Peak Memory and Actual Rule Instantiations are averaged over successfully solved instances only; Inference Wall-Clock Time is averaged over all instances.";
        footnote.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        ws.Range(nextRow + 1, 1, nextRow + 1, headers.Length).Merge();

        ws.Columns().AdjustToContents();
    }

    private int GenerateResourceUsageTable(IXLWorksheet ws, int startRow,
        Dictionary<string, GeoInferenceResult> solving, Dictionary<string, GeoInferenceResult> discovering, string title)
    {
        if (solving.Count == 0 && discovering.Count == 0) return startRow;

        var solvingTime = solving.Values.Select(r => r.InferenceWallClockTime.TotalSeconds).ToList();
        var discoveringTime = discovering.Values.Select(r => r.InferenceWallClockTime.TotalSeconds).ToList();
        // 峰值内存 / 规则实例化次数仅统计成功解出的实例（fix2：明确口径）。
        var solvingMem = solving.Values
            .Where(r => (r.HumanLikeAnswer?.IsAllSuccess ?? false) && r.PeakMemoryBytes > 0)
            .Select(r => r.PeakMemoryBytes / 1048576.0).ToList();
        var discoveringMem = discovering.Values
            .Where(r => (r.HumanLikeAnswer?.IsAllSuccess ?? false) && r.PeakMemoryBytes > 0)
            .Select(r => r.PeakMemoryBytes / 1048576.0).ToList();
        var solvingRi = solving.Values
            .Where(r => (r.HumanLikeAnswer?.IsAllSuccess ?? false) && r.InferenceMetrics?.ActualRuleInstantiationCount > 0)
            .Select(r => (double)r.InferenceMetrics.ActualRuleInstantiationCount).ToList();
        var discoveringRi = discovering.Values
            .Where(r => (r.HumanLikeAnswer?.IsAllSuccess ?? false) && r.InferenceMetrics?.ActualRuleInstantiationCount > 0)
            .Select(r => (double)r.InferenceMetrics.ActualRuleInstantiationCount).ToList();

        string[] Row(string name, List<double> data)
        {
            if (data.Count == 0) return new[] { name, "-", "-", "-" };
            var (avg, variance) = CalStats(data);
            return new[] { name, avg.ToString("F2"), variance.ToString("F2"), data.Max().ToString("F2") };
        }

        var headers = new[] { "Metric", "Average", "Population Variance", "Peak" };
        var data = new List<string[]>
        {
            Row("Inference Wall-Clock Time (s) - Solving", solvingTime),
            Row("Inference Wall-Clock Time (s) - Discovering", discoveringTime),
            Row("Peak Memory (MB) - Solving", solvingMem),
            Row("Peak Memory (MB) - Discovering", discoveringMem),
            Row("Actual Rule Instantiations - Solving", solvingRi),
            Row("Actual Rule Instantiations - Discovering", discoveringRi),
        };

        int nextRow = WriteThreeLineTable(ws, startRow, title, headers, data);

        // fix2 脚注：Peak Memory / Actual Rule Instantiations 仅统计成功实例；Wall-Clock Time 为全部实例。
        var footnote = ws.Cell(nextRow + 1, 1);
        footnote.Value = "Note: Peak Memory and Actual Rule Instantiations are averaged over successfully solved instances only; Inference Wall-Clock Time is averaged over all instances.";
        footnote.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        ws.Range(nextRow + 1, 1, nextRow + 1, headers.Length).Merge();

        return nextRow + 2;
    }

    private int GenerateCombinedCumulativeTimeTable(IXLWorksheet ws, int startRow, Dictionary<string, GeoInferenceResult> solving, string title)
    {
        var solvingTimes = solving
            .Select(r => r.Value.InferenceWallClockTime.TotalSeconds)
            .OrderBy(t => t).ToList();

        int solvingTotal = solvingTimes.Count;

        var thresholds = new (string Label, double Seconds, bool GreaterThan)[]
        {
            ("< 1 s",   1,  false),
            ("< 10 s",  10, false),
            ("< 30 s",  30, false),
            ("< 60 s",  60, false),
            ("> 60 s",  60, true)
        };

        string[] headers = new string[] { "Metric" }
            .Concat(thresholds.Select(t => t.Label))
            .ToArray();

        if (solvingTotal == 0)
        {
            var data2 = new List<string[]>
            {
                new string[] { "Count" }.Concat(thresholds.Select(_ => "0")).ToArray(),
                new string[] { "Ratio" }.Concat(thresholds.Select(_ => "0%")).ToArray()
            };
            return WriteThreeLineTable(ws, startRow, title, headers, data2);
        }

        var counts = new List<string>();
        var ratios = new List<string>();

        foreach (var (_, seconds, greaterThan) in thresholds)
        {
            int count = greaterThan
                ? solvingTotal - (BinarySearchUpperBound(solvingTimes, seconds) + 1)
                : BinarySearchUpperBound(solvingTimes, seconds) + 1;

            double ratio = (double)count / solvingTotal;

            counts.Add(count.ToString());
            ratios.Add(ratio.ToString("P2"));
        }

        var data = new List<string[]>
        {
            new string[] { "Count" }.Concat(counts).ToArray(),
            new string[] { "Ratio" }.Concat(ratios).ToArray()
        };

        return WriteThreeLineTable(ws, startRow, title, headers, data);
    }

    private int GenerateInferenceEfficiencyTable(IXLWorksheet ws, int startRow)
    {
        if (DiscoveringResults.Count == 0) return startRow;

        var figComp = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.FigureCompressionRate));
        var relComp = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.RelationCompressionRate));
        var qvComp = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.EquationCompressionRate));
        var allComp = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.OverallKnowlegeCompressionRate));

        var mriSource = SolvingResults.Count > 0 ? SolvingResults : DiscoveringResults;
        var mergedRed = CalStats(mriSource.Select(d => d.Value.InferenceMetrics.MaxMergedRulePairCompressionRate));
        var semiRed = CalStats(mriSource.Select(d => d.Value.InferenceMetrics.MaxSemiRulePairCompressionRate));
        var overallRed = CalStats(mriSource.Select(d => d.Value.InferenceMetrics.MaxRulePairOverallCompressionRate));

        var headers = new[] { "Category", "Metric", "Mean", "Variance" };

        var data = new List<string[]>
        {
            new[] { "Knowledge Compression", "Figure", figComp.Mean.ToString("F4"), figComp.Variance.ToString("F4") },
            new[] { "", "Relation", relComp.Mean.ToString("F4"), relComp.Variance.ToString("F4") },
            new[] { "", "Equation", qvComp.Mean.ToString("F4"), qvComp.Variance.ToString("F4") },
            new[] { "", "Overall", allComp.Mean.ToString("F4"), allComp.Variance.ToString("F4") },

            new[] { "Rule Pair Reduction", "Merged Factor", mergedRed.Mean.ToString("F4"), mergedRed.Variance.ToString("F4") },
            new[] { "", "Semi Factor", semiRed.Mean.ToString("F4"), semiRed.Variance.ToString("F4") },
            new[] { "", "Overall Factor", overallRed.Mean.ToString("F4"), overallRed.Variance.ToString("F4") }
        };

        return WriteThreeLineTable(ws, startRow, "Table 5: Knowledge Compression & Rule Pair Reduction", headers, data);
    }

    private static Dictionary<string, string> _zhToEnPredicates;

    // Map Chinese predicate aliases (e.g. "角") to English class names (e.g. "Angle")
    private static string PredicateToEnglish(string name)
    {
        _zhToEnPredicates ??= ZhEn.ExtractKnowledgeAliases(typeof(GeoInferenceResult).Assembly)
            .GroupBy(kv => kv.Value)
            .ToDictionary(g => g.Key, g => g.First().Key);
        return _zhToEnPredicates.TryGetValue(name, out var en) ? en : name;
    }

    // Short display names for the most frequent rules; unmapped rules keep their IDs
    private static readonly Dictionary<string, string> RuleDisplayNames = new()
    {
        ["RuleCT001TriangleCongruenceSSS"] = "SSS Congruence",
        ["RuleCT002TriangleCongruenceSAS"] = "SAS Congruence",
        ["RuleCT003TriangleCongruenceASA"] = "ASA Congruence",
        ["RuleCT004TriangleCongruenceAAS"] = "AAS Congruence",
        ["RuleCT006TriangleSimilarityAA"] = "AA Similarity",
        ["RuleCT007TriangleSimilaritySAS"] = "SAS Similarity",
        ["RuleCT008TriangleSimilaritySSS"] = "SSS Similarity",
        ["RuleAQ005AngleBisectorInference"] = "Angle Bisector",
        ["RuleAQ007AlternateInteriorAnglesEqual"] = "Alt. Angles Eq.",
        ["RuleAQ008ConsecutiveInteriorAnglesSupplementary"] = "Co-int. Suppl.",
        ["RuleAQ009CorrespondingAnglesEqual"] = "Corr. Angles Eq.",
        ["RuleDQ007ProportionalSegmentsOnTransversals"] = "Segment Ratio",
        ["RuleGdl01ParallelJudgmentByCorrespondingAngles"] = "Corr. Angles Par.",
        ["RuleGdl02ParallelJudgmentByAlternateInteriorAngles"] = "Alt. Angles Par.",
        ["RuleGdl10CircularPowerTangentAndSecantAngle"] = "Tangent-Secant",
        ["RuleTriH001OrthocenterDetermination"] = "Orthocenter",
        ["RuleTriH004IncenterDetermination"] = "Incenter",
    };

    private static string RuleToDisplayName(string ruleId)
        => RuleDisplayNames.TryGetValue(ruleId, out var display) ? display : ruleId;

    private int GenerateTop5InferenceEfficiencyTables(IXLWorksheet ws, int startRow,
        Dictionary<string, GeoInferenceResult> source, string stateLabel, int firstTableNo)
    {
        if (source.Count == 0) return startRow;

        var data = source.ToList();

        var classicSemi = new Dictionary<string, ulong>();
        var semi = new Dictionary<string, ulong>();
        var classicMerged = new Dictionary<string, ulong>();
        var merged = new Dictionary<string, ulong>();
        var classicPredicate = new Dictionary<string, ulong>();
        var mergedPredicate = new Dictionary<string, ulong>();

        void AddToDict(Dictionary<string, ulong> dict, string key, ulong val)
        {
            dict.TryGetValue(key, out ulong curr);
            dict[key] = curr + val;
        }

        for (int i = 1; i < data.Count; i++)
        {
            var metrics = data[i].Value.InferenceMetrics;

            foreach (var item in metrics.ClassicSemiRulePairDistribution) AddToDict(classicSemi, item.Key, item.Value);
            foreach (var item in metrics.SemiRulePairDistribution) AddToDict(semi, item.Key, item.Value);

            foreach (var item in metrics.ClassicRulePairDistribution) AddToDict(classicMerged, item.Key, item.Value);
            foreach (var item in metrics.MergeRulePairDistribution) AddToDict(merged, item.Key, item.Value);

            foreach (var item in metrics.ClassicPredicateDistribution) AddToDict(classicPredicate, item.Key, item.Value);
            foreach (var item in metrics.MergedPredicateDistribution) AddToDict(mergedPredicate, item.Key, item.Value);
        }

        int WriteClassicDistributionTable(
            IXLWorksheet sheet, int currentRow,
            Dictionary<string, ulong> classicDict, Dictionary<string, ulong> targetDict,
            string itemName, string tableName, string targetDisplayName, int topN,
            Func<string, string> nameMap = null)
        {
            var sortedClassic = classicDict.OrderByDescending(kv => kv.Value).ToList();
            var itemsToProcess = topN > 0 ? sortedClassic.Take(topN) : sortedClassic;

            var headers = new[] { "Rank", itemName, "Classic Count", $"{targetDisplayName} Count", "Compression Rate (%)" };
            var dataRows = new List<string[]>();

            int rank = 1;
            foreach (var item in itemsToProcess)
            {
                targetDict.TryGetValue(item.Key, out ulong targetVal);
                double compressionRate = item.Value > 0 ? (1.0 - (double)targetVal / item.Value) * 100 : 0;

                dataRows.Add(new[] {
                    rank.ToString(),
                    nameMap?.Invoke(item.Key) ?? item.Key,
                    item.Value.ToString(),
                    targetVal.ToString(),
                    compressionRate.ToString("F2")
                });
                rank++;
            }

            currentRow = WriteThreeLineTable(sheet, currentRow, tableName, headers, dataRows);
            return currentRow;
        }

        startRow = WriteClassicDistributionTable(ws, startRow, classicSemi, semi,
            "Rule Pair",
            $"Table {firstTableNo}: Top-5 Classic Semi Rule Pairs ({stateLabel})",
            "Semi",
            5,
            nameMap: RuleToDisplayName);

        startRow = WriteClassicDistributionTable(ws, startRow, classicMerged, merged,
            "Rule Pair",
            $"Table {firstTableNo + 1}: Top-5 Classic Merged Rule Pairs ({stateLabel})",
            "Merged",
            5,
            nameMap: RuleToDisplayName);

        startRow = WriteClassicDistributionTable(ws, startRow, classicPredicate, mergedPredicate,
            "Predicate",
            $"Table {firstTableNo + 2}: Top-5 Classic Predicates ({stateLabel})",
            "Merged",
            5,
            nameMap: PredicateToEnglish);

        return startRow;
    }

    private int GenerateAblationTable(IXLWorksheet ws, int startRow,
        Dictionary<string, GeoInferenceResult> solving,
        Dictionary<string, GeoInferenceResult> noCompass,
        Dictionary<string, GeoInferenceResult> noSimplify,
        int totalProblems, string title)
    {
        if (solving.Count == 0 && noCompass.Count == 0 && noSimplify.Count == 0) return startRow;

        int solvingSuccessCount = solving.Count(r => r.Value.HumanLikeAnswer.IsAllSuccess);
        int withoutCompassSuccessCount = noCompass.Count(r => r.Value.HumanLikeAnswer.IsAllSuccess);
        int noSimplifySuccessCount = noSimplify.Count(r => r.Value.HumanLikeAnswer.IsAllSuccess);

        if (totalProblems <= 0) return startRow;

        double baselineSuccessRate = (double)solvingSuccessCount / totalProblems;
        double noCompassSuccessRate = (double)withoutCompassSuccessCount / totalProblems;
        double noSimplifySuccessRate = (double)noSimplifySuccessCount / totalProblems;

        var withCompassDurations = solving.Select(r => r.Value.InferenceMetrics.TotalDuration).ToList();
        var withoutCompassDurations = noCompass.Select(r => r.Value.InferenceMetrics.TotalDuration).ToList();
        // w/o Simplify Eq Sym 的时间统计仅考虑成功解出的题目，避免未解题目的耗时污染平均值。
        var noSimplifyDurations = noSimplify
            .Where(r => r.Value.HumanLikeAnswer.IsAllSuccess)
            .Select(r => r.Value.InferenceMetrics.TotalDuration)
            .ToList();

        double avgFull = withCompassDurations.Any() ? withCompassDurations.Average() : 0;
        double medFull = withCompassDurations.Any() ? GetMedian(withCompassDurations.OrderBy(t => t).ToList()) : 0;
        double totalFull = withCompassDurations.Sum();

        double avgNoCompass = withoutCompassDurations.Any() ? withoutCompassDurations.Average() : 0;
        double medNoCompass = withoutCompassDurations.Any() ? GetMedian(withoutCompassDurations.OrderBy(t => t).ToList()) : 0;
        double totalNoCompass = withoutCompassDurations.Sum();

        double avgNoSimplify = noSimplifyDurations.Any() ? noSimplifyDurations.Average() : 0;
        double medNoSimplify = noSimplifyDurations.Any() ? GetMedian(noSimplifyDurations.OrderBy(t => t).ToList()) : 0;
        double totalNoSimplify = noSimplifyDurations.Sum();

        var headers = new[]
        {
            "Method",
            "Success Rate",
            "Avg Time (s)",
            "Median Time (s)",
            "Total Time (s)"
        };

        var data = new List<string[]>
        {
            new[]
            {
                "Full Solving (Ours)",
                baselineSuccessRate.ToString("P2"),
                avgFull.ToString("F2"),
                medFull.ToString("F2"),
                totalFull.ToString("F2")
            },
            new[]
            {
                "w/o Compass Matrix",
                noCompassSuccessRate.ToString("P2") + FormatDelta(noCompassSuccessRate, baselineSuccessRate, isRate: true),
                avgNoCompass.ToString("F2") + FormatDelta(avgNoCompass, avgFull, isRate: false),
                medNoCompass.ToString("F2") + FormatDelta(medNoCompass, medFull, isRate: false),
                totalNoCompass.ToString("F2") + FormatDelta(totalNoCompass, totalFull, isRate: false)
            },
            new[]
            {
                "w/o Simplify Eq Sym",
                noSimplifySuccessRate.ToString("P2") + FormatDelta(noSimplifySuccessRate, baselineSuccessRate, isRate: true),
                avgNoSimplify.ToString("F2") + FormatDelta(avgNoSimplify, avgFull, isRate: false),
                medNoSimplify.ToString("F2") + FormatDelta(medNoSimplify, medFull, isRate: false),
                totalNoSimplify.ToString("F2") + FormatDelta(totalNoSimplify, totalFull, isRate: false)
            }
        };

        return WriteThreeLineTable(ws, startRow, title, headers, data);
    }

    private static double GetMedian(List<double> sortedValues)
    {
        int n = sortedValues.Count;
        if (n == 0) return 0.0;
        if (n % 2 == 1) return sortedValues[n / 2];
        return (sortedValues[n / 2 - 1] + sortedValues[n / 2]) / 2.0;
    }

    private string FormatDelta(double current, double baseline, bool isRate)
    {
        if (baseline == 0) return "";

        double diff;
        if (isRate)
        {
            diff = (current - baseline) * 100.0;
        }
        else
        {
            diff = ((current - baseline) / baseline) * 100.0;
        }

        string sign = diff > 0 ? "+" : "";
        return $" ({sign}{diff:F1}%)";
    }

    private static int BinarySearchUpperBound(List<double> sortedList, double target)
    {
        int lo = 0, hi = sortedList.Count - 1, result = -1;
        while (lo <= hi)
        {
            int mid = lo + (hi - lo) / 2;
            if (sortedList[mid] <= target)
            {
                result = mid;
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }
        return result;
    }

    #region Table Tools
    private int WriteThreeLineTable(IXLWorksheet ws, int startRow, string title, string[] headers, List<string[]> data)
    {
        int colStart = 1;
        int colEnd = headers.Length;

        var titleCell = ws.Cell(startRow, colStart);
        titleCell.Value = title;
        titleCell.Style.Font.Bold = false;
        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        ws.Range(startRow, colStart, startRow, colEnd).Merge();
        startRow++;

        int headerRowNum = startRow;
        var headerRange = ws.Range(startRow, colStart, startRow, colEnd);
        for (int c = 0; c < headers.Length; c++)
            ws.Cell(startRow, colStart + c).Value = headers[c];
        headerRange.Style.Font.Bold = false;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        startRow++;

        int dataStartRow = startRow;
        foreach (var row in data)
        {
            for (int c = 0; c < row.Length && c < headers.Length; c++)
            {
                var cell = ws.Cell(startRow, colStart + c);
                cell.Value = row[c];
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            startRow++;
        }
        int dataEndRow = startRow - 1;

        ApplyThreeLineTableStyle(ws, headerRowNum, dataEndRow, colStart, colEnd);

        return startRow;
    }

    private void ApplyThreeLineTableStyle(IXLWorksheet ws, int headerRow, int lastDataRow, int firstCol, int lastCol)
    {
        var tableRange = ws.Range(headerRow, firstCol, lastDataRow, lastCol);

        tableRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.None);
        tableRange.Style.Border.SetInsideBorder(XLBorderStyleValues.None);

        var topRange = ws.Range(headerRow, firstCol, headerRow, lastCol);
        topRange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
        topRange.Style.Border.TopBorderColor = XLColor.Black;

        topRange.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        topRange.Style.Border.BottomBorderColor = XLColor.Black;

        var bottomRange = ws.Range(lastDataRow, firstCol, lastDataRow, lastCol);
        bottomRange.Style.Border.BottomBorder = XLBorderStyleValues.Thick;
        bottomRange.Style.Border.BottomBorderColor = XLColor.Black;

        tableRange.Style.Border.LeftBorder = XLBorderStyleValues.None;
        tableRange.Style.Border.RightBorder = XLBorderStyleValues.None;
    }
    #endregion

    #endregion

    #region Charts

    public void RulePairGroupedByParamsCountChart()
    {
        var data = DiscoveringResults.ToList();
        Dictionary<string, ulong> before = [];
        for (int i = 1; i < data.Count(); i++)
        {
            foreach (var item in data[i].Value.InferenceMetrics.ClassicSemiRulePairDistribution)
            {
                if (before.ContainsKey(item.Key))
                    before[item.Key] += item.Value;
                else
                    before[item.Key] = item.Value;
            }
            foreach (var item in data[i].Value.InferenceMetrics.ClassicRulePairDistribution)
            {
                if (before.ContainsKey(item.Key))
                    before[item.Key] += item.Value;
                else
                    before[item.Key] = item.Value;
            }
            foreach (var item in data[i].Value.InferenceMetrics.UnrelatedRulePairDistribution)
            {
                if (before.ContainsKey(item.Key))
                    before[item.Key] += item.Value;
                else
                    before[item.Key] = item.Value;
            }
        }
        Dictionary<string, (int paramsCount, int semiCount)> reff = InferenceMetricsMaker.Get();
        Dictionary<string, List<(string name, ulong count)>> groupedByParamsCount2 = before
            .Where(kvp => reff.ContainsKey(kvp.Key))
            .GroupBy(kvp => reff[kvp.Key].paramsCount)
            .OrderByDescending(g => g.Key)
            .ToDictionary(
                g => g.Key.ToString(),
                g => g.Select(kvp => (name: kvp.Key, count: kvp.Value))
                        .OrderByDescending(x => x.count)
                        .ToList()
            );
        GenerateLogGroupedBarChart(groupedByParamsCount2);
    }

    public Plot SemiRulePairCountCompare(Dictionary<string, GeoInferenceResult> source)
    {
        var data = source.ToList();
        Dictionary<string, ulong> ClassicSemiRulePairDistribution = [];
        Dictionary<string, ulong> SemiRulePairDistribution = [];

        for (int i = 1; i < data.Count; i++)
        {
            var metrics = data[i].Value.InferenceMetrics;

            foreach (var item in metrics.ClassicSemiRulePairDistribution)
            {
                ClassicSemiRulePairDistribution.TryGetValue(item.Key, out ulong currentCount);
                ClassicSemiRulePairDistribution[item.Key] = currentCount + item.Value;
            }

            foreach (var item in metrics.SemiRulePairDistribution)
            {
                SemiRulePairDistribution.TryGetValue(item.Key, out ulong currentCount);
                SemiRulePairDistribution[item.Key] = currentCount + item.Value;
            }
        }

        var sortedClassic = ClassicSemiRulePairDistribution.OrderByDescending(kv => kv.Value).ToList();

        var a = new Dictionary<string, ulong>();
        var b = new Dictionary<string, ulong>();
        foreach (var item in sortedClassic.Take(5))
        {
            // Use short display names on the chart's x axis
            string display = RuleToDisplayName(item.Key);
            a[display] = item.Value;
            SemiRulePairDistribution.TryGetValue(item.Key, out ulong semiVal);
            b[display] = semiVal;
        }
        return Bar(a, b);
    }

    public Plot MergedRulePairCountCompare(Dictionary<string, GeoInferenceResult> source)
    {
        var data = source.ToList();
        Dictionary<string, ulong> ClassicRulePairDistribution = [];
        Dictionary<string, ulong> MergedRulePairDistribution = [];

        for (int i = 1; i < data.Count; i++)
        {
            var metrics = data[i].Value.InferenceMetrics;

            foreach (var item in metrics.ClassicRulePairDistribution)
            {
                ClassicRulePairDistribution.TryGetValue(item.Key, out ulong currentCount);
                ClassicRulePairDistribution[item.Key] = currentCount + item.Value;
            }

            foreach (var item in metrics.MergeRulePairDistribution)
            {
                MergedRulePairDistribution.TryGetValue(item.Key, out ulong currentCount);
                MergedRulePairDistribution[item.Key] = currentCount + item.Value;
            }
        }

        var sortedClassic = ClassicRulePairDistribution.OrderByDescending(kv => kv.Value).ToList();

        var a = new Dictionary<string, ulong>();
        var b = new Dictionary<string, ulong>();
        foreach (var item in sortedClassic.Take(5))
        {
            // Use short display names on the chart's x axis
            string display = RuleToDisplayName(item.Key);
            a[display] = item.Value;
            MergedRulePairDistribution.TryGetValue(item.Key, out ulong mergedVal);
            b[display] = mergedVal;
        }
        return Bar(a, b);
    }

    public Plot PreidcateCountCompare(Dictionary<string, GeoInferenceResult> source)
    {
        var data = source.ToList();
        Dictionary<string, ulong> ClassicPredicateDistribution = [];
        Dictionary<string, ulong> MergedPredicateDistribution = [];

        for (int i = 1; i < data.Count; i++)
        {
            var metrics = data[i].Value.InferenceMetrics;

            foreach (var item in metrics.ClassicPredicateDistribution)
            {
                ClassicPredicateDistribution.TryGetValue(item.Key, out ulong currentCount);
                ClassicPredicateDistribution[item.Key] = currentCount + item.Value;
            }

            foreach (var item in metrics.MergedPredicateDistribution)
            {
                MergedPredicateDistribution.TryGetValue(item.Key, out ulong currentCount);
                MergedPredicateDistribution[item.Key] = currentCount + item.Value;
            }
        }

        var sortedClassic = ClassicPredicateDistribution.OrderByDescending(kv => kv.Value).ToList();

        var a = new Dictionary<string, ulong>();
        var b = new Dictionary<string, ulong>();
        foreach (var item in sortedClassic.Take(5))
        {
            // Use English predicate names on the chart's x axis
            string enName = PredicateToEnglish(item.Key);
            a[enName] = item.Value;
            MergedPredicateDistribution.TryGetValue(item.Key, out ulong mergedVal);
            b[enName] = mergedVal;
        }

        return Bar(a, b);
    }

    /// <summary>
    /// </summary>
    public Plot CompressionRate() => CompressionRate(DiscoveringResults, nameof(CompressionRate));

    /// <summary>
    /// </summary>
    public Plot SolvingCompressionRate() => CompressionRate(SolvingResults, nameof(SolvingCompressionRate));

    internal static readonly string[] CompressionRateGroupNames =
        { "Figure", "Relation", "Global Pred", "Merged MRI", "Semi MRI", "Global MRI" };

    // Grouped palette: cool blues for knowledge/object level (Figure/Relation/Global Pred),
    // same-hue warm oranges with different depth for the MRI variants (Merged/Semi/Global MRI)
    internal static readonly Color[] CompressionRateGroupedColors =
    {
        Color.FromHex("#9DC3E6"),
        Color.FromHex("#4E79A7"),
        Color.FromHex("#1F3B5C"),
        Color.FromHex("#F4B183"),
        Color.FromHex("#ED7D31"),
        Color.FromHex("#C55A11")
    };

    private static List<List<double>> GetCompressionRateData(Dictionary<string, GeoInferenceResult> source)
    {
        return new List<List<double>>
        {
            source.Select(d => d.Value.InferenceMetrics.FigureCompressionRate).ToList(),
            source.Select(d => d.Value.InferenceMetrics.RelationCompressionRate).ToList(),
            source.Select(d => d.Value.InferenceMetrics.OverallKnowlegeCompressionRate).ToList(),
            source.Select(d => d.Value.InferenceMetrics.MaxMergedRulePairCompressionRate).ToList(),
            source.Select(d => d.Value.InferenceMetrics.MaxSemiRulePairCompressionRate).ToList(),
            source.Select(d => d.Value.InferenceMetrics.MaxRulePairOverallCompressionRate).ToList()
        };
    }

    private Plot CompressionRate(Dictionary<string, GeoInferenceResult> source, string title)
    {
        var data = GetCompressionRateData(source);
        string[] groupNames = CompressionRateGroupNames;

        Plot plot = DrawBoxViolinMean(data, groupNames, CompressionRateGroupedColors);
        PrintStatisticalSummary(data, groupNames);

        plot.Axes.Left.TickLabelStyle.FontSize = 60;
        plot.Axes.Bottom.TickLabelStyle.FontSize = 60;
        plot.Axes.Left.Label.FontSize = 60;
        plot.Axes.Bottom.Label.FontSize = 60;
        plot.Axes.Left.Label.Text = "Compression Rate";
        plot.Axes.Bottom.Label.Text = "Category";
        SavePlot(plot, title);
        return plot;
    }

    /// <summary>
    /// </summary>
    public void GenerateCombinedCompressionRateChart(IReadOnlyList<(string Level, Plot Plot)> levelPlots, string fileName, int columns = 3)
    {
        if (levelPlots == null || levelPlots.Count == 0) return;

        ScottPlot.Multiplot multiplot = new();
        multiplot.Subplots.RemoveAt(0);

        char labelChar = 'a';
        int index = 0;
        foreach (var (level, plot) in levelPlots)
        {
            plot.Axes.Left.TickLabelStyle.FontSize = 50;
            plot.Axes.Bottom.TickLabelStyle.FontSize = 50;
            plot.Axes.Left.Label.FontSize = 54;

            // Tighten the spacing between subplots: remove the minimum axis sizes and
            // the outer label padding so adjacent subplots sit closer together.
            plot.Axes.Left.MinimumSize = 0;
            plot.Axes.Right.MinimumSize = 0;
            plot.Axes.Bottom.MinimumSize = 0;
            plot.Axes.Top.MinimumSize = 0;
            ((ScottPlot.AxisPanels.AxisBase)plot.Axes.Left).PaddingOutsideAxisLabels = new ScottPlot.PixelPadding(0);
            ((ScottPlot.AxisPanels.AxisBase)plot.Axes.Right).PaddingOutsideAxisLabels = new ScottPlot.PixelPadding(0);
            ((ScottPlot.AxisPanels.AxisBase)plot.Axes.Bottom).PaddingOutsideAxisLabels = new ScottPlot.PixelPadding(0);
            ((ScottPlot.AxisPanels.AxisBase)plot.Axes.Top).PaddingOutsideAxisLabels = new ScottPlot.PixelPadding(0);

            // Only the left-column subplots keep the y-axis tick labels and "Compression Rate" label
            if (index % columns != 0)
            {
                plot.Axes.Left.TickLabelStyle.IsVisible = false;
                plot.Axes.Left.Label.Text = string.Empty;
            }

            plot.Axes.Bottom.Label.Text = $"({labelChar}) {level}";
            plot.Axes.Bottom.Label.FontSize = 56;
            plot.Axes.Bottom.Label.Bold = true;
            plot.Axes.Bottom.Label.ForeColor = ScottPlot.Colors.Black;

            multiplot.AddPlot(plot);
            labelChar++;
            index++;
        }

        int count = levelPlots.Count;
        int rows = (int)Math.Ceiling((double)count / columns);
        multiplot.Layout = new ScottPlot.MultiplotLayouts.Grid(rows, columns);

        int width = 1700 * columns;
        int height = 1300 * rows;
        string path = Path.Combine(ReportDir, $"{fileName}.png");
        Directory.CreateDirectory(ReportDir);
        multiplot.SavePng(path, width, height);
        Console.WriteLine($"Plot Saved: {path}");
    }

    #endregion

    #region Plotting Tools
    public Plot GenerateLogGroupedBarChart(
    Dictionary<string, List<(string name, ulong count)>> groupedData,
    [CallerMemberName] string title = "",
    bool useGroupPalette = false)
    {
        Plot plot = new();

        double GetLogValue(ulong v) =>
            v == 0 ? 0 : Math.Log10((double)v);

        var allBars = new List<Bar>();

        /* ================================
         * Color scheme
         * ================================ */

        // IEEE / Nature style scientific blue
        ScottPlot.Color barMainColor =
            ScottPlot.Color.FromHex("#356AA0");

        // Orange used to mark values greater than 10^7
        ScottPlot.Color orangeHighlight =
            ScottPlot.Color.FromHex("#FF7F0E");

        // Group mode alternative colors
        ScottPlot.Color[] groupPalette =
        {
        ScottPlot.Color.FromHex("#356AA0"),
        ScottPlot.Color.FromHex("#D95F02"),
        ScottPlot.Color.FromHex("#4D9221"),
        ScottPlot.Color.FromHex("#7570B3"),
        ScottPlot.Color.FromHex("#E7298A")
    };

        double maxLogValue = 0;

        foreach (var group in groupedData)
        {
            foreach (var item in group.Value)
            {
                double val = GetLogValue(item.count);
                if (val > maxLogValue)
                    maxLogValue = val;
            }
        }

        /* ================================
         * Bar parameters
         * ================================ */

        double fixedBarWidth = 0.8;
        double groupGap = 0.3;

        double currentX = 0;

        List<double> tickPositions = new();
        List<string> tickLabels = new();

        int groupIndex = 0;

        foreach (var group in groupedData)
        {
            var items = group.Value;

            int itemCount = items.Count;

            if (itemCount == 0)
                continue;

            double groupStartX = currentX;

            ScottPlot.Color groupColor =
                groupPalette[groupIndex++ % groupPalette.Length];

            for (int j = 0; j < itemCount; j++)
            {
                double position =
                    currentX + (j + 0.5) * fixedBarWidth;

                double logVal = GetLogValue(items[j].count);

                // Check whether the value exceeds 10^7 (10,000,000)
                bool isOver10M = items[j].count > 10000000;

                // Use orange if greater than 10^7, otherwise choose color based on grouping
                ScottPlot.Color barColor =
                    isOver10M
                    ? orangeHighlight
                    : (useGroupPalette ? groupColor : barMainColor);

                allBars.Add(new Bar()
                {
                    Position = position,
                    Value = logVal,
                    FillColor = barColor,
                    LineWidth = 0,
                    LineColor = barColor,
                    Size = fixedBarWidth * 0.98
                });
            }

            double groupCenterX =
                groupStartX +
                (itemCount * fixedBarWidth) / 2.0;

            tickPositions.Add(groupCenterX);
            tickLabels.Add(group.Key);

            currentX +=
                itemCount * fixedBarWidth +
                groupGap;
        }

        plot.Add.Bars(allBars.ToArray());

        /* ================================
         * X axis
         * ================================ */

        plot.Axes.Bottom.TickGenerator =
            new ScottPlot.TickGenerators.NumericManual(
                tickPositions.ToArray(),
                tickLabels.ToArray()
            );

        // Place the vertical axis (Y axis) at X=0, flush against the left side
        plot.Axes.Bottom.Min = 0;
        plot.Axes.Bottom.Max = currentX - groupGap / 2;

        plot.Axes.Bottom.TickLabelStyle.Rotation = 0;

        plot.Axes.Bottom.TickLabelStyle.FontSize = 40;
        plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleRight;

        plot.Axes.Bottom.Label.FontSize = 44;
        plot.Axes.Bottom.Label.Text = "Params Count";

        /* ================================
         * Y axis Log10 display
         * ================================ */

        int maxLogInt = (int)Math.Ceiling(maxLogValue);

        if (maxLogInt < 0)
            maxLogInt = 0;

        List<double> yTickPositions = new();
        List<string> yTickLabels = new();

        string GetSuperscript(int n)
        {
            string s = n.ToString();
            char[] superscripts =
            {
            '\u2070', '\u00B9', '\u00B2', '\u00B3', '\u2074',
            '\u2075', '\u2076', '\u2077', '\u2078', '\u2079'
        };
            return new string(s.Select(c => superscripts[c - '0']).ToArray());
        }

        for (int i = 0; i <= maxLogInt; i++)
        {
            yTickPositions.Add(i);
            yTickLabels.Add($"10{GetSuperscript(i)}");
        }

        if (yTickPositions.Count == 0)
        {
            yTickPositions.Add(0);
            yTickLabels.Add("10⁰");
        }

        plot.Axes.Left.TickGenerator =
            new ScottPlot.TickGenerators.NumericManual(
                yTickPositions.ToArray(),
                yTickLabels.ToArray()
            );

        plot.Axes.Left.Min = 0;
        plot.Axes.Left.Max = maxLogInt + 0.5;
        plot.Axes.Left.Label.Text = "Maximum Rule Instantiations (Log Scale)";

        plot.Axes.Left.TickLabelStyle.FontSize = 40;
        plot.Axes.Left.Label.FontSize = 44;

        /* ================================
         * Paper style
         * ================================ */

        plot.FigureBackground.Color = ScottPlot.Colors.White;
        plot.DataBackground.Color = ScottPlot.Colors.White;
        plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#E8E8E8");
        plot.Grid.MajorLineWidth = 0.6f;

        foreach (var ax in new ScottPlot.IAxis[] { plot.Axes.Bottom, plot.Axes.Left })
        {
            ax.FrameLineStyle.Color = ScottPlot.Colors.Black;
            ax.MajorTickStyle.Color = ScottPlot.Colors.Black;
            ax.TickLabelStyle.ForeColor = ScottPlot.Colors.Black;
            ax.Label.ForeColor = ScottPlot.Colors.Black;
        }

        plot.Axes.Top.FrameLineStyle.Color = ScottPlot.Colors.Transparent;
        plot.Axes.Right.FrameLineStyle.Color = ScottPlot.Colors.Transparent;

        // Remove the top title
        plot.Title("");

        /* ================================
         * Add legend
         * ================================ */
        var legendItems = new List<ScottPlot.LegendItem>();

        // Regular color legend item
        if (!useGroupPalette)
        {
            legendItems.Add(new ScottPlot.LegendItem()
            {
                LabelText = "Count \u2264 10⁷",
                FillColor = barMainColor,
                OutlineColor = barMainColor,
                OutlineWidth = 1,
                MarkerShape = ScottPlot.MarkerShape.FilledSquare,
                MarkerFillColor = barMainColor,
                MarkerLineColor = barMainColor,
                MarkerSize = 12,
            });
        }

        // Orange highlight legend item
        legendItems.Add(new ScottPlot.LegendItem()
        {
            LabelText = "Count > 10⁷",
            FillColor = orangeHighlight,
            OutlineColor = orangeHighlight,
            OutlineWidth = 1,
            MarkerShape = ScottPlot.MarkerShape.FilledSquare,
            MarkerFillColor = orangeHighlight,
            MarkerLineColor = orangeHighlight,
            MarkerSize = 12,
        });

        plot.ShowLegend(legendItems.ToArray());

        /* ================================
         * Legend position: inset from the top-right corner
         * ================================ */
        plot.Legend.Alignment = Alignment.UpperRight;

        // PixelPadding(left, right, bottom, top)
        // Increase the right and top values to inset the legend into the plot and avoid it sticking to the edge
        plot.Legend.Margin = new ScottPlot.PixelPadding(10, 25, 10, 50);

        // Fine-tune legend style
        plot.Legend.FontSize = 30;
        plot.Legend.BackgroundColor = ScottPlot.Colors.White;
        plot.Legend.OutlineColor = ScottPlot.Colors.Black;
        plot.Legend.OutlineWidth = 1;

        SavePlot(plot, title + "_Log");

        return plot;
    }

    //            double ratio = maxLogValue > 0 ? logVal / maxLogValue : 0;
    //            ScottPlot.Color barColor = useGroupPalette
    //                ? groupColor   // Mathematica default BarChart: same color for the whole group
    //                : GetSciQuadColor(ratio, thresholds: new double[] { 0.4, 0.3, 0.3 });

    //    plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
    //        tickPositions.ToArray(),
    //        tickLabels.ToArray()
    //    );

    //    plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
    //        yTickPositions.ToArray(),
    //        yTickLabels.ToArray()
    //    );

    /* ====== viridis: perceptually uniform and colorblind-safe (Mathematica ColorData["Viridis"]) ====== */
    ScottPlot.Color GetViridisColor(double ratio)
    {
        ratio = Math.Max(0.0, Math.Min(1.0, ratio));

        // viridis five key anchor points
        (int r, int g, int b)[] anchors =
        {
        ( 68,   1,  84),  // #440154 dark purple (low)
        ( 59,  82, 139),  // #3B528B blue
        ( 33, 145, 140),  // #21918C cyan-green
        ( 94, 201,  98),  // #5EC962 yellow-green
        (253, 231,  37),  // #FDE725 bright yellow (high)
    };

        double t = ratio * (anchors.Length - 1);
        int i = (int)Math.Floor(t);
        if (i >= anchors.Length - 1) i = anchors.Length - 2;
        double f = t - i;

        int r = (int)(anchors[i].r + (anchors[i + 1].r - anchors[i].r) * f);
        int g = (int)(anchors[i].g + (anchors[i + 1].g - anchors[i].g) * f);
        int b = (int)(anchors[i].b + (anchors[i + 1].b - anchors[i].b) * f);

        return ScottPlot.Color.FromHex($"#{r:X2}{g:X2}{b:X2}");
    }
    public Plot Bar(Dictionary<string, ulong> before, Dictionary<string, ulong> after, [CallerMemberName] string title = "untitled")
    {
        Plot plot = new();
        double[] GetLogValues(Dictionary<string, ulong> dict) =>
            dict.Values.Select(v => v == 0 ? 0 : Math.Log10((double)v)).ToArray();

        double[] beforeLog = GetLogValues(before);
        double[] afterLog = GetLogValues(after);

        int seriesCount = 2;
        double barWidth = 0.6 / seriesCount;

        var allBars = new List<Bar>();

        void AddSeries(double[] values, int seriesIndex, ScottPlot.Color color)
        {
            for (int i = 0; i < values.Length; i++)
            {
                double offset = (seriesIndex - (seriesCount - 1.0) / 2) * barWidth;
                double position = i + offset;

                allBars.Add(new Bar()
                {
                    Position = position,
                    Value = values[i],
                    FillColor = color,
                    LineWidth = 0,
                    Size = barWidth * 0.8,
                });
            }
        }

        ScottPlot.Color beforeColor = new ScottPlot.Color(255, 126, 13);
        ScottPlot.Color afterColor = new ScottPlot.Color(30, 118, 179);
        AddSeries(beforeLog, 0, beforeColor);
        AddSeries(afterLog, 1, afterColor);

        plot.Add.Bars(allBars.ToArray());

        double[] tickPositions = Enumerable.Range(0, before.Count).Select(i => (double)i).ToArray();
        string[] tickLabels = before.Keys.Select(
            s => ShorterMap.ContainsKey(s) ? ShorterMap[s] : s).ToArray();

        plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(tickPositions, tickLabels);

        plot.Axes.Bottom.TickLabelStyle.Rotation = -45;
        plot.Axes.Bottom.TickLabelStyle.FontSize = 75;

        double maxLogValue = 0;
        if (beforeLog.Length > 0) maxLogValue = Math.Max(maxLogValue, beforeLog.Max());
        if (afterLog.Length > 0) maxLogValue = Math.Max(maxLogValue, afterLog.Max());

        int maxLogInt = (int)Math.Ceiling(maxLogValue);
        if (maxLogInt < 0) maxLogInt = 0;

        List<double> yTickPositions = new List<double>();
        List<string> yTickLabels = new List<string>();

        string GetSuperscript(int n)
        {
            string s = n.ToString();
            char[] superscripts = { '\u2070', '\u00B9', '\u00B2', '\u00B3', '\u2074', '\u2075', '\u2076', '\u2077', '\u2078', '\u2079' };
            return new string(s.Select(c => superscripts[c - '0']).ToArray());
        }

        for (int i = 0; i <= maxLogInt; i++)
        {
            yTickPositions.Add(i);
            yTickLabels.Add($"10{GetSuperscript(i)}");
        }

        if (yTickPositions.Count == 0)
        {
            yTickPositions.Add(0);
            yTickLabels.Add("10⁰");
        }

        plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
            yTickPositions.ToArray(),
            yTickLabels.ToArray()
        );
        plot.Axes.Left.Min = 0;
        plot.Axes.Left.Max = maxLogInt + 0.5;
        plot.Axes.Left.Label.Text = "Count (Log Scale)";

        plot.Axes.Left.TickLabelStyle.FontSize = 78;
        plot.Axes.Left.Label.FontSize = 78;
        plot.Axes.Bottom.Label.FontSize = 78;

        plot.Title(title);
        plot.Axes.Title.Label.FontSize = 0;
        plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleRight;
        // Shift the rotated tick labels downward so they do not touch the axis frame
        plot.Axes.Bottom.TickLabelStyle.OffsetY = 18;
        // Reserve extra vertical space so the rotated tick labels do not overlap the (a)/(b)/(c) label below
        plot.Axes.Bottom.MinimumSize = 580;
        SavePlot(plot, title + "_Log");
        return plot;
    }

    public static void SetTimesNewRoman(Plot plot)
    {
        const string font = "Times New Roman";

        plot.Axes.Title.Label.FontName = font;

        plot.Axes.Bottom.Label.FontName = font;
        plot.Axes.Top.Label.FontName = font;
        plot.Axes.Left.Label.FontName = font;
        plot.Axes.Right.Label.FontName = font;

        plot.Axes.Bottom.TickLabelStyle.FontName = font;
        plot.Axes.Top.TickLabelStyle.FontName = font;
        plot.Axes.Left.TickLabelStyle.FontName = font;
        plot.Axes.Right.TickLabelStyle.FontName = font;

        plot.Legend.FontName = font;
    }

    private void SavePlot(Plot plt, [CallerMemberName] string title = "")
    {
        plt.Font.Automatic();
        // Apply the journal font after Font.Automatic() so it is never overridden
        SetTimesNewRoman(plt);
        // Tighten the automatic margins (default is 10% on each side) to reduce whitespace around the plot
        plt.Axes.AutoScaler = new ScottPlot.AutoScalers.FractionalAutoScaler(.02, .02, .05, .08);
        plt.Axes.AutoScale();
        plt.Grid.MajorLineColor = Colors.Gray.WithOpacity(0.2);
        plt.Grid.MinorLineColor = Colors.Gray.WithOpacity(0.1);

        string path = Path.Combine(CurrentOutputDir, $"{title}.png");
        Directory.CreateDirectory(CurrentOutputDir);
        plt.SavePng(path, 2400, height: 1600);
        Console.WriteLine($"Plot Saved: {path}");
    }

    public void Top5CompressionRate(IEnumerable<Plot> plots, int columns = 3, string name = null)
    {
        ScottPlot.Multiplot multiplot = new();
        multiplot.Subplots.RemoveAt(0);

        char labelChar = 'a';
        foreach (var plot in plots)
        {
            plot.Axes.Bottom.Label.Text = $"({labelChar})";

            plot.Axes.Bottom.Label.FontSize = 100;
            plot.Axes.Bottom.Label.Bold = true;
            plot.Axes.Bottom.Label.ForeColor = ScottPlot.Colors.Black;

            multiplot.AddPlot(plot);
            labelChar++;
        }

        int count = plots.Count();
        int rows = (int)Math.Ceiling((double)count / columns);
        multiplot.Layout = new ScottPlot.MultiplotLayouts.Grid(rows, columns);

        // Render each subplot at 1800x1440 (height +20% over the 3:2 base) for higher clarity
        int width = 1800 * columns;
        int height = 1440 * rows;
        string path = Path.Combine(CurrentOutputDir,
            string.IsNullOrEmpty(name) ? "CompassionAndReduction.png" : $"CompassionAndReduction_{name}.png");
        Directory.CreateDirectory(CurrentOutputDir);

        multiplot.SavePng(path, width, height: height);
    }

    #endregion

    #region SciPlotter Utilities

    public static readonly Color[] SciColors = new Color[]
    {
        Color.FromHex("#E64B35"),
        Color.FromHex("#4DBBD5"),
        Color.FromHex("#00A087"),
        Color.FromHex("#3C5488"),
        Color.FromHex("#F39B7F")
    };

    public static Plot DrawBoxViolinMean(List<List<double>> dataGroups, string[] groupNames = null, Color[] customColors = null)
    {
        var myPlot = new Plot();
        Color[] palette = customColors ?? SciColors;

        for (int i = 0; i < dataGroups.Count; i++)
        {
            double[] values = dataGroups[i].ToArray();
            if (values.Length == 0) continue;

            double x = i;
            Color baseColor = palette[i % palette.Length];

            DrawViolin(myPlot, values, x, baseColor, maxDensityWidth: 0.38);
            DrawBoxPlot(myPlot, values, x, baseColor);

            double mean = values.Average();
            var meanMarker = myPlot.Add.Marker(x, mean);
            meanMarker.Shape = MarkerShape.FilledCircle;
            meanMarker.Size = 25;
            meanMarker.Color = Colors.White;
            meanMarker.MarkerLineColor = Colors.Gray;
            meanMarker.LineWidth = 2.5f;
        }

        double[] tickPositions = Enumerable.Range(0, dataGroups.Count).Select(v => (double)v).ToArray();
        string[] tickLabels = groupNames ?? tickPositions.Select(v => $"Group {v + 1}").ToArray();
        myPlot.Axes.Bottom.SetTicks(tickPositions, tickLabels);
        myPlot.Axes.Bottom.MajorTickStyle.Length = 0;
        myPlot.HideGrid();

        myPlot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
            new Tick[] { new Tick(0, 0.ToString()), new Tick(0.2, 0.2.ToString()), new Tick(0.4, 0.4.ToString()), new Tick(0.6, 0.6.ToString()), new Tick(0.8, 0.8.ToString()), new Tick(1.0, 1.0.ToString()) }
        );
        myPlot.Axes.SetLimitsY(-0.1, 1);

        myPlot.Axes.Top.FrameLineStyle.Width = 0;
        myPlot.Axes.Right.FrameLineStyle.Width = 0;

        myPlot.Axes.Bottom.TickLabelStyle.FontSize = 30;
        myPlot.Axes.Left.TickLabelStyle.FontSize = 30;

        myPlot.Axes.Bottom.Label.FontSize = 35;
        myPlot.Axes.Left.Label.FontSize = 35;
        return myPlot;
    }

    private static void DrawBoxPlot(Plot myPlot, double[] values, double x, Color baseColor)
    {
        var sorted = values.OrderBy(v => v).ToArray();
        double q1 = GetPercentile(sorted, 0.25);
        double median = GetPercentile(sorted, 0.50);
        double q3 = GetPercentile(sorted, 0.75);
        double iqr = q3 - q1;

        double lowerWhiskerLimit = q1 - 1.5 * iqr;
        double upperWhiskerLimit = q3 + 1.5 * iqr;

        double lowerWhisker = sorted.Where(v => v >= lowerWhiskerLimit).DefaultIfEmpty(sorted.First()).First();
        double upperWhisker = sorted.Where(v => v <= upperWhiskerLimit).DefaultIfEmpty(sorted.Last()).Last();

        double boxWidth = 0.16;
        double capWidth = boxWidth * 0.6;

        double left = x - boxWidth / 2;
        double right = x + boxWidth / 2;

        var box = myPlot.Add.Rectangle(left, right, q1, q3);
        box.FillColor = baseColor.WithAlpha(0.7);
        box.LineColor = baseColor;
        box.LineWidth = 2.5f;

        var medLine = myPlot.Add.Line(left, median, right, median);
        medLine.LineColor = Colors.Black;
        medLine.LineWidth = 3.0f;

        var lwLine = myPlot.Add.Line(x, q1, x, lowerWhisker);
        lwLine.LineColor = baseColor;
        lwLine.LineWidth = 2.0f;

        var uwLine = myPlot.Add.Line(x, q3, x, upperWhisker);
        uwLine.LineColor = baseColor;
        uwLine.LineWidth = 2.0f;

        var lCap = myPlot.Add.Line(x - capWidth / 2, lowerWhisker, x + capWidth / 2, lowerWhisker);
        lCap.LineColor = baseColor;
        lCap.LineWidth = 2.0f;

        var uCap = myPlot.Add.Line(x - capWidth / 2, upperWhisker, x + capWidth / 2, upperWhisker);
        uCap.LineColor = baseColor;
        uCap.LineWidth = 2.0f;

        var outliers = sorted.Where(v => v < lowerWhisker || v > upperWhisker).ToArray();
        foreach (var outlier in outliers)
        {
            var m = myPlot.Add.Marker(x, outlier);
            m.Shape = MarkerShape.OpenCircle;
            m.Color = baseColor;
            m.Size = 7;
            m.LineWidth = 1.5f;
        }
    }

    private static double GetPercentile(double[] sortedValues, double percentile)
    {
        if (sortedValues.Length == 0) return 0;
        if (sortedValues.Length == 1) return sortedValues[0];

        double index = (sortedValues.Length - 1) * percentile;
        int lower = (int)Math.Floor(index);
        int upper = (int)Math.Ceiling(index);
        if (lower == upper) return sortedValues[lower];

        double weight = index - lower;
        return sortedValues[lower] * (1 - weight) + sortedValues[upper] * weight;
    }

    private static void DrawViolin(Plot myPlot, double[] values, double x, Color baseColor, double maxDensityWidth)
    {
        double mean = values.Average();
        double variance = values.Average(v => Math.Pow(v - mean, 2));
        double stdDev = Math.Sqrt(variance);
        double bandwidth = 1.06 * stdDev * Math.Pow(values.Length, -0.2);
        if (bandwidth <= 0) bandwidth = 0.05;

        int resolution = 100;
        double minVal = Math.Max(0.0, values.Min() - bandwidth * 3);
        double maxVal = Math.Min(1.0, values.Max() + bandwidth * 3);
        double step = (maxVal - minVal) / resolution;
        if (step <= 0) return;

        List<double> ys = new List<double>();
        List<double> densities = new List<double>();

        for (double y = minVal; y <= maxVal + 1e-9; y += step)
        {
            ys.Add(y);
            double density = 0;
            foreach (double val in values)
            {
                double u = (y - val) / bandwidth;
                density += Math.Exp(-0.5 * u * u) / Math.Sqrt(2 * Math.PI);
            }
            density /= (values.Length * bandwidth);
            densities.Add(density);
        }

        double maxDensity = densities.Max();
        if (maxDensity == 0) return;

        List<Coordinates> leftPoints = new List<Coordinates>();
        List<Coordinates> rightPoints = new List<Coordinates>();

        for (int i = 0; i < ys.Count; i++)
        {
            double d = (densities[i] / maxDensity) * maxDensityWidth;
            leftPoints.Add(new Coordinates(x - d, ys[i]));
            rightPoints.Add(new Coordinates(x + d, ys[i]));
        }

        rightPoints.Reverse();
        var polygonPoints = leftPoints.Concat(rightPoints).ToArray();

        var poly = myPlot.Add.Polygon(polygonPoints);
        poly.FillColor = baseColor.WithAlpha(0.4);
        poly.LineColor = baseColor.WithAlpha(0.85);
        poly.LineWidth = 1.5f;
    }

    public static void PrintStatisticalSummary(List<List<double>> dataGroups, string[] groupNames = null, Color[] customColors = null)
    {
        Console.WriteLine("================================================================================");
        Console.WriteLine("  Descriptive Statistics Report for SCI Paper");
        Console.WriteLine("================================================================================");

        for (int i = 0; i < dataGroups.Count; i++)
        {
            double[] values = dataGroups[i].ToArray();
            if (values.Length == 0) continue;

            string groupName = (groupNames != null && i < groupNames.Length) ? groupNames[i] : $"Group {i + 1}";
            int n = values.Length;

            double mean = values.Average();
            double min = values.Min();
            double max = values.Max();

            double variance = values.Sum(v => Math.Pow(v - mean, 2)) / (n - 1);
            double stdDev = Math.Sqrt(variance);
            double sem = stdDev / Math.Sqrt(n);

            var sorted = values.OrderBy(v => v).ToArray();
            double median = GetPercentile(sorted, 0.50);
            double q1 = GetPercentile(sorted, 0.25);
            double q3 = GetPercentile(sorted, 0.75);
            double iqr = q3 - q1;

            double lowerLimit = q1 - 1.5 * iqr;
            double upperLimit = q3 + 1.5 * iqr;
            int outlierCount = values.Count(v => v < lowerLimit || v > upperLimit);

            double skewness = CalculateSkewness(values, mean, stdDev, n);
            double kurtosis = CalculateExcessKurtosis(values, mean, stdDev, n);

            Console.WriteLine($"\n▶ [{groupName}] (N = {n})");
            Console.WriteLine("--------------------------------------------------------------------------------");

            Console.WriteLine($"  [Normal data]       Mean ± SD   : {mean:F3} ± {stdDev:F3}");
            Console.WriteLine($"                      Mean ± SEM  : {mean:F3} ± {sem:F3}");

            Console.WriteLine($"  [Non-normal data]   Median (IQR) : {median:F3} ({iqr:F3})  [Q1: {q1:F3}, Q3: {q3:F3}]");

            Console.WriteLine($"  [Extremes & range]  Range       : [{min:F3}, {max:F3}]");
            Console.WriteLine($"  [Outliers]          Outliers    : {outlierCount} (based on the 1.5×IQR rule)");

            Console.WriteLine($"  [Normality]         Skewness    : {skewness:F3}  (near 0 = normal)");
            Console.WriteLine($"                      Kurtosis    : {kurtosis:F3}  (near 0 = normal, >0 peaked, <0 flat)");

            Console.WriteLine("  [SCI statistical advice]:");
            if (Math.Abs(skewness) <= 2 && Math.Abs(kurtosis) <= 2)
            {
                Console.WriteLine("                  -> Data are approximately normal; use Mean±SD and compare groups with a t-test/ANOVA.");
            }
            else
            {
                Console.WriteLine("                  -> Data show skewness or kurtosis; use Median(IQR) and compare groups with Mann-Whitney/Kruskal-Wallis.");
            }
        }
        Console.WriteLine("\n================================================================================");
    }

    private static double CalculateSkewness(double[] values, double mean, double stdDev, int n)
    {
        if (n < 3 || stdDev == 0) return 0;
        double sumCubed = values.Sum(v => Math.Pow((v - mean) / stdDev, 3));
        return (n * sumCubed) / ((n - 1) * (n - 2));
    }

    private static double CalculateExcessKurtosis(double[] values, double mean, double stdDev, int n)
    {
        if (n < 4 || stdDev == 0) return 0;
        double sumFourth = values.Sum(v => Math.Pow((v - mean) / stdDev, 4));
        double term1 = (n * (n + 1) * sumFourth) / ((n - 1) * (n - 2) * (n - 3));
        double term2 = (3 * Math.Pow(n - 1, 2)) / ((n - 2) * (n - 3));
        return term1 - term2;
    }

    #endregion

    public static (double Mean, double Variance) CalStats(IEnumerable<double> data, bool isSample = false)
    {
        if (data == null || !data.Any())
            return (0, 0);
        if (isSample && data.Count() < 2)
            return (0, 0);

        double sum = 0;
        foreach (var x in data) sum += x;
        double mean = sum / data.Count();

        double sumSqDiff = 0;
        foreach (var x in data) sumSqDiff += (x - mean) * (x - mean);

        double variance = sumSqDiff / (isSample ? data.Count() - 1 : data.Count());
        return (mean, variance);
    }
}