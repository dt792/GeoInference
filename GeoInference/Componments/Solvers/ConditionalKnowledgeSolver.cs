
public class ConditionalKnowledgeSolver
{
    [DI] KnowledgeBase knowledgeBase { get; set; }
    [DI] KnowledgeBaseUpdater Updater { get; set; }
    public void MakeNew()
    {
        foreach (var item in knowledgeBase.LastRoundKnowledges.Where(p => p is Predicate))
        {
            Preds.Enqueue((Predicate)item);
        }
    }
    public Queue<Predicate> Preds { get; set; } = [];
    public bool HasNext { get => Preds.Count > 0; }
    public Predicate Next() => Preds.Dequeue();
    public void Solve(Predicate pred)
    {
        List<Knowledge> result = [];
        if (knowledgeBase.InversedConditionalKnowledgePairDict.ContainsKey(pred.HashCode))
        {
            foreach (var ck in knowledgeBase.InversedConditionalKnowledgePairDict[pred.HashCode].ToArray())
            {
                if (ck.Update(pred))
                {
                    if (ck.IsEstablish)
                    {
                        ck.Knowledge.Conditions.AddRange(ck.ConditionDict.Values);
                        Updater.Add(ck.Knowledge);
                    }
                    knowledgeBase.InversedConditionalKnowledgePairDict[pred.HashCode].Remove(ck);
                }
            }
        }
    }
}
