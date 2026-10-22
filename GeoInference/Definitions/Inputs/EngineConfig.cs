using System;
using System.Collections.Generic;
using System.Text;

public class EngineConfig : IInferenceConfig
{
    public bool IsSolveEqSym { get; set; } = true;
    public bool IsSolveMatrix { get; set; } = true;
}