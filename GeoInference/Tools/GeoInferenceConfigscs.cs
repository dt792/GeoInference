
public class GeoInferenceConfigs
{
    public static Dictionary<string, GeoInferenceConfig> Comfigs = new Dictionary<string, GeoInferenceConfig>()
    {
        { "default",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine))],
            OutputMakers=[typeof(HumanLikeAnswerMaker)]
        }
        },
          { "SolvingDebug",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine))],
            OutputMakers=[typeof(HumanLikeAnswerMaker),typeof(KnowledgeGraphMaker), typeof(InferenceMetricsMaker), typeof(InferenceLogMaker)]
        }
        },
           { "SolvingNoSimplifyEqSymDebug",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine)),(typeof(EquationSolver), typeof(EquationSolverNoSimplifyEqSym))],
            OutputMakers=[typeof(HumanLikeAnswerMaker),typeof(KnowledgeGraphMaker), typeof(InferenceMetricsMaker), typeof(InferenceLogMaker)]
        }
        },
            { "SolvingNoCompassMatrixDebug",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine)),(typeof(EquationSolver), typeof(EquationSolverNoCompassMatrix))],
             OutputMakers=[typeof(HumanLikeAnswerMaker),typeof(KnowledgeGraphMaker), typeof(InferenceMetricsMaker), typeof(InferenceLogMaker)]
        }
        },

               { "DiscoveringDebug",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_DiscoveringEngine))],
            OutputMakers=[typeof(KnowledgeGraphMaker), typeof(InferenceMetricsMaker), typeof(InferenceLogMaker)]
        }
        },
           { "DiscoveringNoSimplifyEqSymDebug",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine)),(typeof(EquationSolver), typeof(EquationSolverNoSimplifyEqSym))],
            OutputMakers=[typeof(KnowledgeGraphMaker), typeof(InferenceMetricsMaker), typeof(InferenceLogMaker)]
        }
        },
            { "DiscoveringNoCompassMatrixDebug",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_DiscoveringEngine)),(typeof(EquationSolver), typeof(EquationSolverNoCompassMatrix))],
             OutputMakers=[typeof(KnowledgeGraphMaker), typeof(InferenceMetricsMaker), typeof(InferenceLogMaker)]
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
        },
           { "DiscoveringNoSimplifyEqSym",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_SolvingEngine)),(typeof(EquationSolver), typeof(EquationSolverNoSimplifyEqSym))],
            OutputMakers=[typeof(InferenceMetricsMaker)]
        }
        },
            { "DiscoveringNoCompassMatrix",new GeoInferenceConfig()
        {
            Components=[(typeof(IInferenceEngine),typeof(KM_SCR_DiscoveringEngine)),(typeof(EquationSolver), typeof(EquationSolverNoCompassMatrix))],
             OutputMakers=[typeof(InferenceMetricsMaker)]
        }
        },
    };
    public static GeoInferenceConfig QuickSolving { get => Comfigs["default"]; }
    public static GeoInferenceConfig SolvingDebug => Comfigs["SolvingDebug"];
    public static GeoInferenceConfig SolvingNoSimplifyEqSymDebug => Comfigs["SolvingNoSimplifyEqSymDebug"];
    public static GeoInferenceConfig SolvingNoCompassMatrixDebug => Comfigs["SolvingNoCompassMatrixDebug"];
    public static GeoInferenceConfig DiscoveringDebug => Comfigs["DiscoveringDebug"];
    public static GeoInferenceConfig DiscoveringNoSimplifyEqSymDebug => Comfigs["DiscoveringNoSimplifyEqSymDebug"];
    public static GeoInferenceConfig DiscoveringNoCompassMatrixDebug => Comfigs["DiscoveringNoCompassMatrixDebug"];

    public static GeoInferenceConfig Solving => Comfigs["Solving"];
    public static GeoInferenceConfig SolvingNoSimplifyEqSym => Comfigs["SolvingNoSimplifyEqSym"];
    public static GeoInferenceConfig SolvingNoCompassMatrix => Comfigs["SolvingNoCompassMatrix"];
    public static GeoInferenceConfig Discovering => Comfigs["Discovering"];
    public static GeoInferenceConfig DiscoveringNoSimplifyEqSym => Comfigs["DiscoveringNoSimplifyEqSym"];
    public static GeoInferenceConfig DiscoveringNoCompassMatrix=> Comfigs["DiscoveringNoCompassMatrix"];

}