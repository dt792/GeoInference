﻿
using GeoInference.Definitions.Knowledges;

public class KnowledgeBase
{
    #region AR
    public Dictionary<string, Quantity> Quantities { get; set; } = [];
    public Dictionary<ulong, QuantityValue> QuantityValues { get; set; } = [];
    public Dictionary<ulong, QuantityRatio> QuantityRatios { get; set; } = [];
    public Dictionary<ulong, LinearEquation> LinearEquations { get; set; } = [];
    public Dictionary<ulong, ProductionEquation> ProductionEquations { get; set; } = [];
    public Dictionary<ulong, Equation> RemainingEquations { get; set; } = [];
    public Dictionary<ulong, Equation> Equations { get; set; } = [];
    #endregion
    #region DD
    public Dictionary<ulong, Predicate> Predicates { get; set; } = [];
    public Dictionary<Type, List<Predicate>> Categories { get; set; } = [];
    public List<ConditionalKnowledge> ConditionalKnowledgePairs { get; set; } = [];
    /// <summary>
    /// </summary>
    public Dictionary<ulong, List<ConditionalKnowledge>> ConditionalKnowledgePairDict { get; set; } = [];
    public Dictionary<ulong, List<ConditionalKnowledge>> InversedConditionalKnowledgePairDict { get; set; } = [];
    #endregion

    public Dictionary<QuantityClassifications, List<EqualityChain>> EqualityChains { get; set; } =
       new Dictionary<QuantityClassifications, List<EqualityChain>>()
   {
        { QuantityClassifications.Distance, new List<EqualityChain>() },
            { QuantityClassifications.Angle, new List<EqualityChain>() },
            { QuantityClassifications.Sin, new List<EqualityChain>() },
            { QuantityClassifications.Cos, new List<EqualityChain>() },
             { QuantityClassifications.Tan, new List<EqualityChain>() },
   };
    public Dictionary<Quantity, EqualityChain> IndexContinuedDict { get; set; } = [];

    public List<Knowledge> LastRoundKnowledges { get; set; } = [];
    public List<Knowledge> NewKnowledges { get; set; } = [];
}
