
internal class HumanLikeAnswerMaker : IOutputMaker
{
    [DI]
    TargetBase tBase;
    string MakeConditionStr(Knowledge knowledge)
    {
        var answer = "";
        var conditions = new List<Knowledge>();
        GetList(conditions, knowledge);
        conditions.Reverse();

        int index = 1;
        foreach (var condition in conditions)
        {
            if (condition.Conditions.Count == 0)
            {
                answer += $"[{index++}]∵\t{condition}\t({condition.Reason})\n";
            }
            else
            {
                List<int> condictionIndexs = new List<int>();
                foreach (var item in condition.Conditions.Distinct().OrderByDescending(c => c.ToString().Length))
                {
                    condictionIndexs.Add(conditions.IndexOf(item) + 1);
                }
                answer += $"[{index++}]∴\t{condition}\t({condition.Reason}\t{StringTool.ComposeList(condictionIndexs)})\n";
            }
        }
        answer += "\n";
        return answer;
    }
    void GetList(in List<Knowledge> historyKnowledges, Knowledge knowledge)
    {
        if (historyKnowledges.Contains(knowledge))
        {
            historyKnowledges.Remove(knowledge);
            historyKnowledges.Add(knowledge);
        }
        else
        {
            historyKnowledges.Add(knowledge);
        }
        for (int i = knowledge.Conditions.Count - 1; i >= 0; i--)
        {
            GetList(historyKnowledges, knowledge.Conditions[i]);
        }
    }
    public object Make()
    {
        var output = new HumanLikeAnswer();
        foreach (var item in tBase.Targets)
        {
            if (item.IsSuccess)
            {
                output.Answers.Add(new()
                {
                    Index = item.Index + 1,
                    IsSuccess = item.IsSuccess,
                    Question = item.ToString(),
                    Answer = item.Answer,
                    InferenceSteps = MakeConditionStr(item.Conclusion).Trim()
                });
            }
            else
            {
                output.Answers.Add(new()
                {
                    Index = item.Index + 1,
                    IsSuccess = item.IsSuccess,
                    Question = item.ToString(),
                });
            }
        }
        output.Answers.Sort((a, b) => a.Index.CompareTo(b.Index));
        return output;
    }
}
