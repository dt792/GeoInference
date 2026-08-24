
public class InferenceMetrics
{
    #region Time-Related-Data
    public double TotalDuration { get; set; }
    public double RuleDuration { get; set; }
    public double SemiRuleDuration { get; set; }
    public double EqSymDuration { get; set; }
    public double MatrixDuration { get; set; }
    public double OtherDuration { get => TotalDuration - RuleDuration - SemiRuleDuration - EqSymDuration - MatrixDuration; }
    #endregion
    #region Knowledge-Related-Data
    public ulong MergedFigureCount { get; set; } = 0;
    public ulong MergedRelationCount { get; set; } = 0;
    public ulong MergedQvCount { get; set; } = 0;
    public ulong MergedQrCount { get; set; } = 0;
    public ulong MergedLinearEquationCount { get; set; } = 0;
    public ulong MergedProductionEquationCount { get; set; } = 0;
    public ulong MergedResidualEquationCount { get; set; } = 0;

    public ulong MergedPredicateCount { get => MergedFigureCount + MergedRelationCount; }
    public ulong MergedEquationCount { get => MergedQrCount + MergedQvCount + MergedLinearEquationCount + MergedProductionEquationCount + MergedResidualEquationCount; }
    public ulong MergedKnowledgeCount { get => MergedPredicateCount + MergedEquationCount; }

    public ulong ClassicFigureCount { get; set; } = 0;
    public ulong ClassicRelationCount { get; set; } = 0;
    public ulong ClassicQrCount { get; set; } = 0;
    public ulong ClassicQvCount { get; set; } = 0;
    public ulong ClassicLinearEquationCount { get; set; } = 0;
    public ulong ClassicProductionEquationCount { get; set; } = 0;
    public ulong ClassicResidualEquationCount { get; set; } = 0;

    public ulong ClassicPredicateCount { get => ClassicFigureCount + ClassicRelationCount; }
    public ulong ClassicEquationCount { get => ClassicQrCount + ClassicQvCount + ClassicLinearEquationCount + ClassicProductionEquationCount + ClassicResidualEquationCount; }
    public ulong ClassicKnowledgeCount { get => ClassicPredicateCount + ClassicEquationCount; }

    public ulong UnrelatedFigureCount { get; set; } = 0;
    public ulong UnrelatedRelationCount { get; set; } = 0;
    public ulong UnrelatedQrCount { get; set; } = 0;
    public ulong UnrelatedQvCount { get; set; } = 0;
    public ulong UnrelatedLinearEquationCount { get; set; } = 0;
    public ulong UnrelatedProductionEquationCount { get; set; } = 0;
    public ulong UnrelatedResidualEquationCount { get; set; } = 0;

    public ulong UnrelatedPredicateCount { get => UnrelatedFigureCount + UnrelatedRelationCount; }
    public ulong UnrelatedEquationCount { get => UnrelatedQrCount + UnrelatedQvCount + UnrelatedLinearEquationCount + UnrelatedProductionEquationCount + UnrelatedResidualEquationCount; }
    public ulong UnrelatedKnowledgeCount { get => UnrelatedPredicateCount + UnrelatedEquationCount; }

    public ulong OverallMergedKnowledgeCount { get => MergedKnowledgeCount + UnrelatedKnowledgeCount; }
    public ulong OverallClassicKnowledgeCount { get => ClassicKnowledgeCount + UnrelatedKnowledgeCount; }

    public Dictionary<string, ulong> ClassicPredicateDistribution { get; set; } = [];
    public Dictionary<string, ulong> MergedPredicateDistribution { get; set; } = [];
    public Dictionary<string, ulong> UnrelatedPredicateDistribution { get; set; } = [];
    public double FigureCompressionRate { get => ClassicFigureCount == MergedFigureCount ? 0 : 1 - (double)MergedFigureCount / ClassicFigureCount; }
    public double RelationCompressionRate { get => ClassicRelationCount == MergedRelationCount ? 0 : 1 - (double)MergedRelationCount / ClassicRelationCount; }
    public double PredicateCompressionRate { get => ClassicPredicateCount == MergedPredicateCount ? 0 : 1 - (double)MergedPredicateCount / ClassicPredicateCount; }
    public double EquationCompressionRate { get => ClassicEquationCount == MergedEquationCount ? 0 : 1 - (double)MergedEquationCount / ClassicEquationCount; }
    public double TargetCompressionRate { get => ClassicKnowledgeCount == MergedKnowledgeCount ? 0 : 1 - (double)MergedKnowledgeCount / ClassicKnowledgeCount; }

    public double OverallKnowlegeCompressionRate { get => 1 - (double)(MergedFigureCount + MergedRelationCount + UnrelatedFigureCount + UnrelatedRelationCount) / (ClassicFigureCount + ClassicRelationCount + UnrelatedFigureCount + UnrelatedRelationCount); }
    #endregion
    #region Rule-Related-Data
    public ulong ClassicRulePairCount { get; set; } = 0;
    public ulong MergedRulePairCount { get; set; } = 0;
    public ulong ClassicSemiRulePairCount { get; set; } = 0;
    public ulong SemiRulePairCount { get; set; } = 0;
    public ulong UnrelatedRulePairCount { get; set; } = 0;
    public Dictionary<string, ulong> ClassicRulePairDistribution { get; set; } = [];
    public Dictionary<string, ulong> MergeRulePairDistribution { get; set; } = [];
    public Dictionary<string, ulong> ClassicSemiRulePairDistribution { get; set; } = [];
    public Dictionary<string, ulong> SemiRulePairDistribution { get; set; } = [];
    public Dictionary<string, ulong> UnrelatedRulePairDistribution { get; set; } = [];
    public double MaxMergedRulePairCompressionRate { get => ClassicRulePairCount == MergedRulePairCount ? 0 : 1 - (double)MergedRulePairCount / ClassicRulePairCount; }
    public double MaxSemiRulePairCompressionRate { get => ClassicSemiRulePairCount == SemiRulePairCount ? 0 : 1 - (double)SemiRulePairCount / ClassicSemiRulePairCount; }
    public double MaxRulePairOverallCompressionRate { get => 1 - (double)(MergedRulePairCount + SemiRulePairCount + UnrelatedPredicateCount) / (ClassicRulePairCount + ClassicSemiRulePairCount + UnrelatedPredicateCount); }

    #endregion
    #region EqualityChain-Related-Data
    public List<double> MatrixSparsities { get; set; } = [];
    public int QuantityCount { get; set; } = 0;
    public int InEqualityChainQuantityCount { get; set; } = 0;
    public double InEqualityChainQuantityRatio { get => QuantityCount == 0 ? 0 : (double)InEqualityChainQuantityCount / QuantityCount; }
    public double QvQrProportion
    {
        get
        {
            ulong denominator = MergedEquationCount + UnrelatedEquationCount;
            return denominator == 0 ? 0 : (double)(MergedQvCount + MergedQrCount + UnrelatedQvCount + UnrelatedQrCount) / denominator;
        }
    }

    public double SemiRule_Check { get; internal set; }
    #endregion
}
