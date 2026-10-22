
public enum EqSymStates
{
    Init,
    NoValid,
    ReadyToTry,
    Tried,
    TooComplex,
}
public class EquationSystem
{
    public static int CurIndex;
    public EquationSystem()
    {
        PosIndex = CurIndex++;
    }
    public int PosIndex;
    public EqSymStates State { get; set; } = EqSymStates.Init;
    public List<Quantity> Quantities { get; set; } = [];
    public List<string> Equations { get; set; } = [];
    public List<string> RelatedInequations { get; set; } = [];
    public List<Knowledge> Conditions { get; set; } = [];
    public string Eqs => StringTool.ComposeList(Equations);
    /// <summary>
    /// </summary>
    public EquationSystem Clone()
    {
        return new EquationSystem
        {
            PosIndex = PosIndex,
            State = State,
            Quantities = new List<Quantity>(Quantities),
            Equations = new List<string>(Equations),
            RelatedInequations = new List<string>(RelatedInequations),
            Conditions = new List<Knowledge>(Conditions),
        };
    }
    public override string ToString()
    {
        return StringTool.ComposeList(Equations);
    }
}
