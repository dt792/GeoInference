

public class EquationSolverNoSimplifyEqSym : EquationSolver
{
    public override void ECUpdated(EqualityChain ce)
    {
        ContinuedEqualityUpdated?.Invoke(ce);
    }
}
