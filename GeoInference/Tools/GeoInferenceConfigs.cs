
public class GeoInferenceConfigs
{
    public static Dictionary<string, GeoInferenceConfig> Configs = new Dictionary<string, GeoInferenceConfig>()
    {
        { "default",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine))],
            OutputMakers=[typeof(HumanLikeAnswerMaker)],
        }
        },
            { "Solving",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine))],
            OutputMakers=[typeof(HumanLikeAnswerMaker), typeof(InferenceMetricsMaker)]
        }
        },
           { "SolvingNoSimplifyEqSym",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine)),(typeof(EquationSolver), typeof(EquationSolverNoSimplifyEqSym))],
            OutputMakers=[typeof(HumanLikeAnswerMaker), typeof(InferenceMetricsMaker)]
        }
        },
            { "SolvingNoCompassMatrix",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine)),(typeof(EquationSolver), typeof(EquationSolverNoCompassMatrix))],
             OutputMakers=[typeof(HumanLikeAnswerMaker), typeof(InferenceMetricsMaker)]

        }
        },
               { "Discovering",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_DiscoveringEngine))],
            OutputMakers=[typeof(InferenceMetricsMaker)]
        }
        }
        ,
          { "SolvingDebug",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine))],
            OutputMakers=[typeof(HumanLikeAnswerMaker),typeof(KnowledgeGraphMaker), typeof(InferenceMetricsMaker), typeof(InferenceLogMaker)]
             , PlugIns=[typeof(KnowledgeCheckPlugIn)]
        }
        },
           { "SolvingNoSimplifyEqSymDebug",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine)),(typeof(EquationSolver), typeof(EquationSolverNoSimplifyEqSym))],
            OutputMakers=[typeof(HumanLikeAnswerMaker),typeof(KnowledgeGraphMaker), typeof(InferenceMetricsMaker), typeof(InferenceLogMaker)]
             , PlugIns=[typeof(KnowledgeCheckPlugIn)]
        }
        },
            { "SolvingNoCompassMatrixDebug",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine)),(typeof(EquationSolver), typeof(EquationSolverNoCompassMatrix))],
             OutputMakers=[typeof(HumanLikeAnswerMaker),typeof(KnowledgeGraphMaker), typeof(InferenceMetricsMaker), typeof(InferenceLogMaker)]
              , PlugIns=[typeof(KnowledgeCheckPlugIn)]
        }
        },

               { "DiscoveringDebug",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_DiscoveringEngine))],
            OutputMakers=[typeof(KnowledgeGraphMaker), typeof(InferenceMetricsMaker), typeof(InferenceLogMaker)]
             , PlugIns=[typeof(KnowledgeCheckPlugIn)]
        }
        }
    };
    public static GeoInferenceConfig QuickSolving { get => Configs["default"]; }
    public static GeoInferenceConfig SolvingDebug => Configs["SolvingDebug"];
    public static GeoInferenceConfig SolvingNoSimplifyEqSymDebug => Configs["SolvingNoSimplifyEqSymDebug"];
    public static GeoInferenceConfig SolvingNoCompassMatrixDebug => Configs["SolvingNoCompassMatrixDebug"];
    public static GeoInferenceConfig DiscoveringDebug => Configs["DiscoveringDebug"];
    public static GeoInferenceConfig DiscoveringNoSimplifyEqSymDebug => Configs["DiscoveringNoSimplifyEqSymDebug"];
    public static GeoInferenceConfig DiscoveringNoCompassMatrixDebug => Configs["DiscoveringNoCompassMatrixDebug"];

    public static GeoInferenceConfig Solving => Configs["Solving"];
    public static GeoInferenceConfig SolvingNoSimplifyEqSym => Configs["SolvingNoSimplifyEqSym"];
    public static GeoInferenceConfig SolvingNoCompassMatrix => Configs["SolvingNoCompassMatrix"];
    public static GeoInferenceConfig Discovering => Configs["Discovering"];
    public static GeoInferenceConfig DiscoveringNoSimplifyEqSym => Configs["DiscoveringNoSimplifyEqSym"];
    public static GeoInferenceConfig DiscoveringNoCompassMatrix => Configs["DiscoveringNoCompassMatrix"];

}
