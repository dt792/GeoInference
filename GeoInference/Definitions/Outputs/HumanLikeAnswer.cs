
using System.Text;

public class QuestionHumanLikeAnswer
{
    public int Index { get; set; }
    public bool IsSuccess { get; set; }
    public string Question { get; set; } = "";
    public string InferenceSteps { get; set; }
    public string Answer { get; set; }
    public override string ToString()
    {
        if (GeoInferenceApp.IsZhOrEn)
        {
            if (IsSuccess) return $"第{Index}问：{Question}，已成功解决\n{InferenceSteps}";
            else return $"第{Index}问：{Question}，未解决";
        }
        else
        {
            if (IsSuccess) return $"Q{Index}: {Question}, solved successfully\n{InferenceSteps}";
            else return $"Q{Index}: {Question}, not solved";
        }
    }
}
[Alias("类人答题输出")]
public class HumanLikeAnswer
{
    public bool IsAllSuccess { get => Answers.TrueForAll(a => a.IsSuccess); }
    public List<QuestionHumanLikeAnswer> Answers { get; set; } = new();

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var item in Answers)
        {
            sb.Append(item.ToString() + "\n");
        }
        return sb.ToString();
    }
}