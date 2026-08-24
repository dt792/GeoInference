using System.Text;
public class LogContent
{
    public string Class { get; set; }

    public string Method { get; set; }

    public string Content { get; set; }

    public string Level { get; set; }

    public DateTime Time { get; set; } = DateTime.Now;
    public override string ToString()
    {
        return $"{Class}:{Method} [{Level}] {Content} {Time.ToString("T")}";
    }
}
[Alias("日志输出")]
public class InferenceLog 
{
    public List<LogContent> LogContents { get; set; } = new();
    public Dictionary<string, Dictionary<string, List<LogContent>>> CateLogContents { get; set; } = new();
    public List<ZPair<string, DateTime>> KeyPoints { get; set; } = new();

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < LogContents.Count; i++)
        {
            sb.AppendLine(LogContents[i].ToString());
        }
        return sb.ToString();
    }
}

