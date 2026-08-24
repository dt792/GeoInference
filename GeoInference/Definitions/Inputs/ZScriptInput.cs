
using System.Text.RegularExpressions;

public class ZScriptInput
{
    public ZScriptInput()
    {
        
    }
    public ZScriptInput(string script)
    {
        Script = script;
    }
    public string Script { get; set; } = string.Empty;
    public static Func<string, Quantity> ParseQuantity { get; set; }

    #region Tool Functions
    /// <summary>
    /// Standard Formatting
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    public static string FormatStandard(string script)
    {
        script = script.Trim('\n');
        script = Regex.Replace(script, @" +", " ");
        script = script.Replace("！", "");
        script = script.Replace("\r", "");
        script = script.Replace('）', ')').Replace('（', '(');
        script = script.Replace('，', ',');
        script = script.Replace("pi", "Pi");
        script = script.Replace('：', ':');
        script = script.Replace(" : ", ":");
        script = script.Replace(": ", ":");
        script = script.Replace(" :", ":");
        script = script.Replace("∠", "角");
        string standardScript = string.Empty;
        foreach (var line in script.Split('\n'))
        {
            standardScript += $"{line.Trim()}\n";
        }
        return standardScript;
    }
    #endregion

    #region KeyWords
    public const string VarsDef = "Vars:";
    public const string PointsDef = "Points:";
    public const string LinesDef = "Lines:";
    public const string AuxDef = "+";
    public const string DisproveDef = "Disprove:";
    public const string ProveDef = "Prove:";
    public const string SolveDef = "Solve:";
    public const string DetermineDef = "Determine:";
    public const string MinDef = "Min:";
    public const string MaxDef = "Max:";
    public const string Value = "value";
    #endregion
}
