using System.Diagnostics;

/// <summary>
/// </summary>
public static class SemiRuleDiagnostics
{
    public static bool Enabled { get; set; } = false;

    public static Dictionary<string, long> RuleInvokeCounts { get; } = [];
    public static Dictionary<string, long> RuleInvokeTicks { get; } = [];

    public static long CkAddCalls;
    public static long CkEstablishedCount;
    public static long CkWaitingCount;
    public static long CkDedupCompares;
    public static long CkDedupTicks;

    static long nextProgressReport = 20_000_000;

    public static void Reset()
    {
        RuleInvokeCounts.Clear();
        RuleInvokeTicks.Clear();
        CkAddCalls = CkEstablishedCount = CkWaitingCount = CkDedupCompares = CkDedupTicks = 0;
        nextProgressReport = 20_000_000;
    }

    public static long OnRuleInvokeStart(RuleInfo info)
    {
        if (!Enabled) return 0;
        RuleInvokeCounts.TryGetValue(info.Name, out var c);
        RuleInvokeCounts[info.Name] = c + 1;
        return Stopwatch.GetTimestamp();
    }

    public static void OnRuleInvokeEnd(RuleInfo info, long startTimestamp)
    {
        if (!Enabled || startTimestamp == 0) return;
        var elapsed = Stopwatch.GetTimestamp() - startTimestamp;
        RuleInvokeTicks.TryGetValue(info.Name, out var t);
        RuleInvokeTicks[info.Name] = t + elapsed;
    }

    public static long OnCkAddStart()
    {
        if (!Enabled) return 0;
        CkAddCalls++;
        return Stopwatch.GetTimestamp();
    }

    public static void OnCkDedupCompare()
    {
        if (!Enabled) return;
        CkDedupCompares++;
        if (CkDedupCompares >= nextProgressReport)
        {
            Console.WriteLine($"[SemiRuleDiag] 去重比较 {CkDedupCompares:N0} 次 | CK调用 {CkAddCalls:N0} | 成立 {CkEstablishedCount:N0} | 挂起 {CkWaitingCount:N0}");
            nextProgressReport += 20_000_000;
        }
    }

    public static void OnCkDedupEnd(long startTimestamp)
    {
        if (!Enabled || startTimestamp == 0) return;
        CkDedupTicks += Stopwatch.GetTimestamp() - startTimestamp;
    }

    public static void OnCkEstablished()
    {
        if (Enabled) CkEstablishedCount++;
    }

    public static void OnCkWaiting()
    {
        if (Enabled) CkWaitingCount++;
    }

    public static string TicksToMs(long ticks) => (ticks * 1000.0 / Stopwatch.Frequency).ToString("F1");
}
