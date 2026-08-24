using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using ClosedXML.Excel;
using GeoInference.MergeKnowledges;

using ScottPlot;

namespace GeoInference.Tests;

internal class IntegratedResultReporter
{
    public const string DefaultResultPath = "..\\..\\..\\..\\Result";

    private const string DatasetName = GeoProblemDataset.FormalGeo7KL6;

    public string ReportDir => Path.Combine(DefaultResultPath, $"{DatasetName}_Reports");

    public int Total { get; set; }
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

    public void LoadAllData()
    {
        LoadData(GeoProblemDataset.FormalGeo7KL6, nameof(GeoInferenceConfigs.Solving), SolvingResults);
        LoadData(GeoProblemDataset.FormalGeo7KL6, nameof(GeoInferenceConfigs.SolvingNoCompassMatrix), SolvingNoCompassMatrixResults);
        LoadData(GeoProblemDataset.FormalGeo7KL6, nameof(GeoInferenceConfigs.SolvingNoSimplifyEqSym), SolvingNoSimplifyEqSymResults);
        LoadData(GeoProblemDataset.FormalGeo7KL6, nameof(GeoInferenceConfigs.Discovering), DiscoveringResults);
    }

    public void LoadData(string datasetName, string config, Dictionary<string, GeoInferenceResult> results, string resultDir = GeoInferenceTester.DefaultResultPath)
    {
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

        void AddInferenceStatuSheet(XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("InferenceStatus");

            var allQuestions = new HashSet<string>();
            var allDicts = new Dictionary<string, GeoInferenceResult>[]
            {
                SolvingResults,
                SolvingNoCompassMatrixResults,
                SolvingNoSimplifyEqSymResults,
                DiscoveringResults
            };
            var solvingDicts = new Dictionary<string, GeoInferenceResult>[]
            {
                SolvingResults,
                SolvingNoCompassMatrixResults,
                SolvingNoSimplifyEqSymResults
            };

            foreach (var dict in allDicts)
            {
                foreach (var key in dict.Keys) allQuestions.Add(key);
            }
            var sortedQuestions = allQuestions.OrderBy(q => q).ToList();

            string[] headers =
            {
                "ProblemName",
                "Solving(Finished)\n", "Solving_NoCompass(Finished)\n", "Solving_NoSimplify(Finished)\n",
                "Discovering\n(Finished)",
                "Solving\n(Solved)", "Solving_NoCompass\n(Solved)", "Solving_NoSimplify\n(Solved)"
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
                    var cell = ws.Cell(row, col + 2);
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
                    var cell = ws.Cell(row, col + 2 + allDicts.Length);
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

        void AddInferenceTimeSheet(XLWorkbook wb)
        {
            var ws = wb.Worksheets.Add("Inference time statistics");

            var allQuestions = new HashSet<string>();
            var dictionaries = new Dictionary<string, GeoInferenceResult>[]
            {
                SolvingResults,
                SolvingNoCompassMatrixResults,
                SolvingNoSimplifyEqSymResults,
                DiscoveringResults
            };

            foreach (var dict in dictionaries)
            {
                foreach (var key in dict.Keys)
                {
                    allQuestions.Add(key);
                }
            }
            var sortedQuestions = allQuestions.OrderBy(q => q).ToList();

            string[] headers =
            {
                "ProblemName",
                "Solving",
                "Solving_NoCompassMatrix",
                "Solving_NoSimplifyEqSym",
                "Discovering"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = false;
                cell.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            }

            for (int i = 0; i < sortedQuestions.Count; i++)
            {
                int row = i + 2;
                string question = sortedQuestions[i];

                ws.Cell(row, 1).Value = question;
                ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                for (int col = 0; col < dictionaries.Length; col++)
                {
                    var dict = dictionaries[col];
                    var cell = ws.Cell(row, col + 2);

                    if (dict.TryGetValue(question, out var result))
                    {
                        double seconds = result.InferenceTime.TotalSeconds;
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
            }

            if (sortedQuestions.Count > 0)
            {
                ws.Range(1, 1, sortedQuestions.Count + 1, headers.Length).SetAutoFilter();
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(1);
        }

        AddInferenceStatuSheet(wb);
        AddInferenceTimeSheet(wb);
        Directory.CreateDirectory(ReportDir);
        wb.SaveAs(Path.Combine(ReportDir, "StatuAndInferenceTimeReport.xlsx"));
    }

    public void GenerateMetricsTables(string outputPath = null)
    {
        outputPath ??= Path.Combine(ReportDir, "MetricsTables.xlsx");
        using var workbook = new XLWorkbook();
        var ws = workbook.AddWorksheet("Experimental Results");

        int currentRow = 1;

        currentRow = GenerateChainedEqualityNecessityTable(ws, currentRow);
        currentRow += 2;

        currentRow = GenerateSuccessRateTable(ws, currentRow);
        currentRow += 2;

        currentRow = GenerateAverageTimeStatisticsTable(ws, currentRow);
        currentRow += 2;

        currentRow = GenerateCombinedCumulativeTimeTable(ws, currentRow);
        currentRow += 2;

        currentRow = GenerateInferenceEfficiencyTable(ws, currentRow);
        currentRow += 2;

        currentRow = GenerateAblationTable(ws, currentRow);
        currentRow += 2;

        ws.Columns().AdjustToContents();

        ws = workbook.AddWorksheet("InferenceEfficiencyTables");
        currentRow = 1;
        currentRow = GenerateTop5InferenceEfficiencyTables(ws, currentRow);
        ws.Columns().AdjustToContents();

        var outputDir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(outputDir))
            Directory.CreateDirectory(outputDir);
        workbook.SaveAs(outputPath);
    }

    public void GenerateMetricsCharts()
    {
        RulePairGroupedByParamsCountChart();
        var a = PreidcateCountCompare();
        var b = SemiRulePairCountCompare();
        var c = MergedRulePairCountCompare();
        Top5CompressionRate(new[] { a, b, c });
        CompressionRate();
    }

    #region Tables

    private int GenerateChainedEqualityNecessityTable(IXLWorksheet ws, int startRow)
    {
        var matrixSparsityData = DiscoveringResults
            .SelectMany(r => r.Value.InferenceMetrics.MatrixSparsities)
            .ToList();

        var qvQrProportionData = DiscoveringResults
            .Select(r => r.Value.InferenceMetrics.QvQrProportion)
            .ToList();

        var inEqualityChainRatioData = DiscoveringResults
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

        return WriteThreeLineTable(ws, startRow, "Table 1: GenerateChainedEqualityNecessityTable", headers, data);
    }

    private int GenerateSuccessRateTable(IXLWorksheet ws, int startRow)
    {
        var solvingSuccessCount = SolvingResults.Count(r => r.Value.HumanLikeAnswer.IsAllSuccess);
        double ourSuccessRate = Total > 0 ? (double)solvingSuccessCount / Total : 0;

        var headers = new[]
        {
            "Method",
            "PN-BFS",
            "FGeo-DRL",
            "FGeo-HyperGNet",
            "DeepSeekv3",
            "FGeo-NSS",
            "FGeo-ISRL",
            "KM-SCR(Ours)"
        };

        var data = new List<string[]>
        {
            new[]
            {
                "PSSR",
                0.1355.ToString("P2"),
                0.3962.ToString("P2"),
                0.5882.ToString("P2"),
                0.4118.ToString("P2"),
                0.4844.ToString("P2"),
                0.2921.ToString("P2"),
                ourSuccessRate.ToString("P2")
            }
        };

        return WriteThreeLineTable(ws, startRow, "Table 2:FormalGeo7K v2 PSSR", headers, data);
    }

    private int GenerateAverageTimeStatisticsTable(IXLWorksheet ws, int startRow)
    {
        var solvingTime = SolvingResults.Select(r => r.Value.InferenceTime.TotalSeconds).ToList();
        solvingTime.Sort();

        var (avgSolveTime, varSolveTime) = CalStats(solvingTime);

        var discoverTime = DiscoveringResults.Select(r => r.Value.InferenceTime.TotalSeconds).ToList();
        var (avgDiscTime, varDiscTime) = CalStats(discoverTime);

        var headers = new[] { "Phase", "Average Time (s)", "Population Variance" };
        var data = new List<string[]>
        {
            new[] { "Solving", avgSolveTime.ToString("F2"), varSolveTime.ToString("F2") },
            new[] { "Discovering", avgDiscTime.ToString("F2"), varDiscTime.ToString("F2") }
        };

        return WriteThreeLineTable(ws, startRow, "Table 3: Average Time StatisticsTable", headers, data);
    }

    private int GenerateCombinedCumulativeTimeTable(IXLWorksheet ws, int startRow)
    {
        var solvingTimes = SolvingResults
            .Select(r => r.Value.InferenceTime.TotalSeconds)
            .OrderBy(t => t).ToList();

        int solvingTotal = solvingTimes.Count;

        var thresholds = new (string Label, double Seconds)[]
        {
            ("< 1 s",   1),
            ("< 10 s",  10),
            ("< 30 s",  30),
            ("< 60 s",  60),
            ("< 120 s", 120)
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
            return WriteThreeLineTable(ws, startRow, "Table: Cumulative Time Distribution", headers, data2);
        }

        var counts = new List<string>();
        var ratios = new List<string>();

        foreach (var (_, seconds) in thresholds)
        {
            int count = seconds == double.MaxValue
                ? solvingTotal
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

        return WriteThreeLineTable(ws, startRow, "Table: Cumulative Time Distribution", headers, data);
    }

    private int GenerateInferenceEfficiencyTable(IXLWorksheet ws, int startRow)
    {
        var figComp = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.FigureCompressionRate));
        var relComp = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.RelationCompressionRate));
        var qvComp = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.EquationCompressionRate));
        var allComp = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.OverallKnowlegeCompressionRate));

        var mergedRed = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.MaxMergedRulePairCompressionRate));
        var semiRed = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.MaxSemiRulePairCompressionRate));
        var overallRed = CalStats(DiscoveringResults.Select(d => d.Value.InferenceMetrics.MaxRulePairOverallCompressionRate));

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

        return WriteThreeLineTable(ws, startRow, "Table X: Knowledge Compression & Rule Pair Reduction", headers, data);
    }

    private int GenerateTop5InferenceEfficiencyTables(IXLWorksheet ws, int startRow)
    {
        var data = DiscoveringResults.ToList();

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
            string itemName, string tableName, string targetDisplayName, int topN)
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
                    item.Key,
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
            "Table X: Top-5 Classic Semi Rule Pairs",
            "Semi",
            5);

        startRow = WriteClassicDistributionTable(ws, startRow, classicMerged, merged,
            "Rule Pair",
            "Table Y: Top-5 Classic Merged Rule Pairs",
            "Merged",
            5);

        startRow = WriteClassicDistributionTable(ws, startRow, classicPredicate, mergedPredicate,
            "Predicate",
            "Table Z: Classic Predicates Distribution",
            "Merged",
            5);

        return startRow;
    }

    private int GenerateAblationTable(IXLWorksheet ws, int startRow)
    {
        int solvingSuccessCount = SolvingResults.Count(r => r.Value.HumanLikeAnswer.IsAllSuccess);
        int withoutCompassSuccessCount = SolvingNoCompassMatrixResults.Count(r => r.Value.HumanLikeAnswer.IsAllSuccess);
        int noSimplifySuccessCount = SolvingNoSimplifyEqSymResults.Count(r => r.Value.HumanLikeAnswer.IsAllSuccess);

        double baselineSuccessRate = (double)solvingSuccessCount / Total;
        double noCompassSuccessRate = (double)withoutCompassSuccessCount / Total;
        double noSimplifySuccessRate = (double)noSimplifySuccessCount / Total;

        var withCompassDurations = SolvingResults.Select(r => r.Value.InferenceMetrics.TotalDuration).ToList();
        var withoutCompassDurations = SolvingNoCompassMatrixResults.Select(r => r.Value.InferenceMetrics.TotalDuration).ToList();
        var noSimplifyDurations = SolvingNoSimplifyEqSymResults.Select(r => r.Value.InferenceMetrics.TotalDuration).ToList();

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

        return WriteThreeLineTable(ws, startRow, "Table N: Ablation Study on Success Rate and Efficiency", headers, data);
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

    public Plot SemiRulePairCountCompare()
    {
        var data = DiscoveringResults.ToList();
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
            a.Add(item.Key, item.Value);
            SemiRulePairDistribution.TryGetValue(item.Key, out ulong semiVal);
            b.Add(item.Key, semiVal);
        }
        return Bar(a, b);
    }

    public Plot MergedRulePairCountCompare()
    {
        var data = DiscoveringResults.ToList();
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
            a.Add(item.Key, item.Value);
            MergedRulePairDistribution.TryGetValue(item.Key, out ulong mergedVal);
            b.Add(item.Key, mergedVal);
        }
        return Bar(a, b);
    }

    public Plot PreidcateCountCompare()
    {
        var data = DiscoveringResults.ToList();
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
            a.Add(item.Key, item.Value);
            MergedPredicateDistribution.TryGetValue(item.Key, out ulong mergedVal);
            b.Add(item.Key, mergedVal);
        }

        return Bar(a, b);
    }

    public void CompressionRate()
    {
        var data = new List<List<double>>();
        var figComp = DiscoveringResults.Select(d => d.Value.InferenceMetrics.FigureCompressionRate).ToList();
        var relComp = DiscoveringResults.Select(d => d.Value.InferenceMetrics.RelationCompressionRate).ToList();
        var allComp = DiscoveringResults.Select(d => d.Value.InferenceMetrics.OverallKnowlegeCompressionRate).ToList();

        var mergedRed = DiscoveringResults.Select(d => d.Value.InferenceMetrics.MaxMergedRulePairCompressionRate).ToList();
        var semiRed = DiscoveringResults.Select(d => d.Value.InferenceMetrics.MaxSemiRulePairCompressionRate).ToList();
        var overallRed = DiscoveringResults.Select(d => d.Value.InferenceMetrics.MaxRulePairOverallCompressionRate).ToList();

        data.Add(figComp);
        data.Add(relComp);
        data.Add(allComp);
        data.Add(mergedRed);
        data.Add(semiRed);
        data.Add(overallRed);

        string[] groupNames = { "Figure", "Relation", "Global Pred", "Merged MRI", "Semi MRI", "Global MRI" };
        Plot plot = DrawBoxViolinMean(data, groupNames);
        PrintStatisticalSummary(data, groupNames);

        plot.Axes.Left.TickLabelStyle.FontSize = 60;
        plot.Axes.Bottom.TickLabelStyle.FontSize = 60;
        plot.Axes.Left.Label.FontSize = 60;
        plot.Axes.Bottom.Label.FontSize = 60;
        plot.Axes.Left.Label.Text = "Compression Rate";
        plot.Axes.Bottom.Label.Text = "Category";
        SavePlot(plot);
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
    //public Plot GenerateLogGroupedBarChart(
    //Dictionary<string, List<(string name, ulong count)>> groupedData,
    //[CallerMemberName] string title = "",
    //bool useGroupPalette = false)   // true = Mathematica default BarChart style: single solid color per group
    //{
    //    Plot plot = new();

    //    double GetLogValue(ulong v) => v == 0 ? 0 : Math.Log10((double)v);

    //    var allBars = new List<Bar>();

    //    /* ====== Mathematica journal style: ColorData[97] default palette ======
    //     * #5E81B5 Blue ( 94,129,181)  -- low value
    //     * #8FB032 Green (143,176, 50)
    //     * #E19C24 Orange (225,156, 36)
    //     * #EB6235 Red (235, 98, 53)  -- high value
    //     * Discrete palette backup: #8778B3 Purple, #C56E1A Brown, #5D9EC8 Light blue        */
    //    ScottPlot.Color[] mmaPalette =
    //    {
    //    ScottPlot.Color.FromHex("#5E81B5"),
    //    ScottPlot.Color.FromHex("#E19C24"),
    //    ScottPlot.Color.FromHex("#8FB032"),
    //    ScottPlot.Color.FromHex("#EB6235"),
    //    ScottPlot.Color.FromHex("#8778B3"),
    //    ScottPlot.Color.FromHex("#C56E1A"),
    //    ScottPlot.Color.FromHex("#5D9EC8"),
    //};

    //    ScottPlot.Color GetSciQuadColor(double ratio, double[] thresholds = null)
    //    {
    //        ratio = Math.Max(0.0, Math.Min(1.0, ratio));

    //        // Gradient color anchor points: Mathematica ColorData[97] hues (blue -> green -> orange -> red)
    //        var c0 = (r: 94, g: 129, b: 181); // #5E81B5
    //        var c1 = (r: 143, g: 176, b: 50);  // #8FB032
    //        var c2 = (r: 225, g: 156, b: 36);  // #E19C24
    //        var c3 = (r: 235, g: 98, b: 53);  // #EB6235

    //        double[] stops;

    //        if (thresholds == null)
    //        {
    //            stops = new double[] { 0.0, 1.0 / 3.0, 2.0 / 3.0, 1.0 };
    //        }
    //        else if (thresholds.Length == 3)
    //        {
    //            double sum = thresholds[0] + thresholds[1] + thresholds[2];
    //            stops = new double[] {
    //            0.0,
    //            thresholds[0] / sum,
    //            (thresholds[0] + thresholds[1]) / sum,
    //            1.0
    //        };
    //        }
    //        else if (thresholds.Length == 4)
    //        {
    //            stops = (double[])thresholds.Clone();
    //        }
    //        else
    //        {
    //            stops = new double[] { 0.0, 1.0 / 3.0, 2.0 / 3.0, 1.0 };
    //        }

    //        stops[0] = 0.0;
    //        stops[3] = 1.0;
    //        if (stops[1] <= stops[0]) stops[1] = stops[0] + 0.001;
    //        if (stops[2] <= stops[1]) stops[2] = stops[1] + 0.001;
    //        if (stops[2] >= stops[3]) stops[2] = stops[3] - 0.001;
    //        if (stops[1] >= stops[2]) stops[1] = stops[2] - 0.001;

    //        int r = 0, g = 0, b = 0;

    //        if (ratio <= stops[1])
    //        {
    //            double t = (ratio - stops[0]) / (stops[1] - stops[0]);
    //            r = (int)(c0.r + (c1.r - c0.r) * t);
    //            g = (int)(c0.g + (c1.g - c0.g) * t);
    //            b = (int)(c0.b + (c1.b - c0.b) * t);
    //        }
    //        else if (ratio <= stops[2])
    //        {
    //            double t = (ratio - stops[1]) / (stops[2] - stops[1]);
    //            r = (int)(c1.r + (c2.r - c1.r) * t);
    //            g = (int)(c1.g + (c2.g - c1.g) * t);
    //            b = (int)(c1.b + (c2.b - c1.b) * t);
    //        }
    //        else
    //        {
    //            double t = (ratio - stops[2]) / (stops[3] - stops[2]);
    //            r = (int)(c2.r + (c3.r - c2.r) * t);
    //            g = (int)(c2.g + (c3.g - c2.g) * t);
    //            b = (int)(c2.b + (c3.b - c2.b) * t);
    //        }

    //        return ScottPlot.Color.FromHex($"#{r:X2}{g:X2}{b:X2}");
    //    }

    //    double maxLogValue = 0;
    //    foreach (var group in groupedData)
    //        foreach (var item in group.Value)
    //        {
    //            double val = GetLogValue(item.count);
    //            if (val > maxLogValue) maxLogValue = val;
    //        }

    //    double fixedBarWidth = 0.8;
    //    double groupGap = 0.6;

    //    double currentX = 0;
    //    List<double> tickPositions = new List<double>();
    //    List<string> tickLabels = new List<string>();
    //    int groupIndex = 0;

    //    foreach (var group in groupedData)
    //    {
    //        var items = group.Value;
    //        int itemCount = items.Count;
    //        if (itemCount == 0) continue;

    //        double groupStartX = currentX;
    //        ScottPlot.Color groupColor = mmaPalette[groupIndex++ % mmaPalette.Length];

    //        for (int j = 0; j < itemCount; j++)
    //        {
    //            double position = currentX + (j + 0.5) * fixedBarWidth;
    //            double logVal = GetLogValue(items[j].count);

    //            double ratio = maxLogValue > 0 ? logVal / maxLogValue : 0;
    //            ScottPlot.Color barColor = useGroupPalette
    //                ? groupColor   // Mathematica default BarChart: same color for the whole group
    //                : GetSciQuadColor(ratio, thresholds: new double[] { 0.4, 0.3, 0.3 });

    //            allBars.Add(new Bar()
    //            {
    //                Position = position,
    //                Value = logVal,
    //                FillColor = barColor,
    //                LineWidth = 1.0f,
    //                LineColor = ScottPlot.Colors.White, // thin white outline to simulate the gap between bars in Mathematica
    //                Size = fixedBarWidth * 0.95,
    //            });
    //        }

    //        double groupCenterX = groupStartX + (itemCount * fixedBarWidth) / 2.0;
    //        tickPositions.Add(groupCenterX);
    //        tickLabels.Add(group.Key);

    //        currentX += itemCount * fixedBarWidth + groupGap;
    //    }

    //    plot.Add.Bars(allBars.ToArray());

    //    plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
    //        tickPositions.ToArray(),
    //        tickLabels.ToArray()
    //    );

    //    plot.Axes.Bottom.Min = -groupGap / 2;
    //    plot.Axes.Bottom.Max = currentX - groupGap / 2;

    //    plot.Axes.Bottom.TickLabelStyle.Rotation = 0;
    //    plot.Axes.Bottom.TickLabelStyle.FontSize = 40;
    //    plot.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleRight;
    //    plot.Axes.Bottom.Label.FontSize = 40;
    //    plot.Axes.Bottom.Label.Text = "Params Count";

    //    int maxLogInt = (int)Math.Ceiling(maxLogValue);
    //    if (maxLogInt < 0) maxLogInt = 0;

    //    List<double> yTickPositions = new List<double>();
    //    List<string> yTickLabels = new List<string>();

    //    string GetSuperscript(int n)
    //    {
    //        string s = n.ToString();
    //        char[] superscripts = { '\u2070', '\u00B9', '\u00B2', '\u00B3', '\u2074', '\u2075', '\u2076', '\u2077', '\u2078', '\u2079' };
    //        return new string(s.Select(c => superscripts[c - '0']).ToArray());
    //    }

    //    for (int i = 0; i <= maxLogInt; i++)
    //    {
    //        yTickPositions.Add(i);
    //        yTickLabels.Add($"10{GetSuperscript(i)}");
    //    }

    //    if (yTickPositions.Count == 0)
    //    {
    //        yTickPositions.Add(0);
    //        yTickLabels.Add("10⁰");
    //    }

    //    plot.Axes.Left.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
    //        yTickPositions.ToArray(),
    //        yTickLabels.ToArray()
    //    );

    //    plot.Axes.Left.Min = 0;
    //    plot.Axes.Left.Max = maxLogInt + 0.5;
    //    plot.Axes.Left.Label.Text = "Maximum Rule Instantiations(Log Scale)";
    //    plot.Axes.Left.TickLabelStyle.FontSize = 40;
    //    plot.Axes.Left.Label.FontSize = 40;

    //    /* ====== Mathematica journal style: white background, black axes, light gray grid ====== */
    //    plot.FigureBackground.Color = ScottPlot.Colors.White;
    //    plot.DataBackground.Color = ScottPlot.Colors.White;
    //    plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#DBDBDB");
    //    plot.Grid.MajorLineWidth = 1;

    //    // Fix: explicitly specify the common base interface IAxis to avoid IXAxis / IYAxis array type inference failure
    //    foreach (var ax in new ScottPlot.IAxis[] { plot.Axes.Bottom, plot.Axes.Left })
    //    {
    //        ax.FrameLineStyle.Color = ScottPlot.Colors.Black;
    //        ax.MajorTickStyle.Color = ScottPlot.Colors.Black;
    //        ax.TickLabelStyle.ForeColor = ScottPlot.Colors.Black;
    //        ax.Label.ForeColor = ScottPlot.Colors.Black;
    //    }

    //    plot.Title(title);
    //    plot.Axes.Title.Label.FontSize = 0;

    //    SavePlot(plot, title + "_Log");
    //    return plot;
    //}

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

        string path = Path.Combine(ReportDir, $"{title}.png");
        Directory.CreateDirectory(ReportDir);
        plt.SavePng(path, 2400, height: 1600);
        Console.WriteLine($"Plot Saved: {path}");
    }

    public void Top5CompressionRate(IEnumerable<Plot> plots, int columns = 3)
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
        string path = Path.Combine(ReportDir, $"CompassionAndReduction.png");
        Directory.CreateDirectory(ReportDir);

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
        if (data == null || data.Count() == 0)
            throw new ArgumentException("Data list cannot be empty.");
        if (isSample && data.Count() < 2)
            throw new ArgumentException("Calculating sample variance requires at least 2 data points.");

        double sum = 0;
        foreach (var x in data) sum += x;
        double mean = sum / data.Count();

        double sumSqDiff = 0;
        foreach (var x in data) sumSqDiff += (x - mean) * (x - mean);

        double variance = sumSqDiff / (isSample ? data.Count() - 1 : data.Count());
        return (mean, variance);
    }
}