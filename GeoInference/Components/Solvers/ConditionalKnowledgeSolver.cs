
public class ConditionalKnowledgeSolver
{
    [DI] public KnowledgeBase knowledgeBase { get; set; }
    [DI] public KnowledgeBaseUpdater Updater { get; set; }
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
    public virtual void Solve(Predicate pred)
    {
        List<Knowledge> result = [];
        if (knowledgeBase.InversedConditionalKnowledgePairDict.TryGetValue(pred.HashCode, out var bucket))
        {
            foreach (var ck in bucket.ToArray())
            {
                if (ck.Update(pred))
                {
                    Updater.Add(ck.Knowledge);
                    bucket.Remove(ck);
                }
            }
        }
    }
}
