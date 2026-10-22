using System.Diagnostics;
using System.Globalization;
using System.Text;

/// <summary>
/// </summary>
public static class KnowledgeTimeline
{
    static readonly Stopwatch _sw = new();
    static readonly List<double> _times = new();
    static readonly object _lock = new();
    static int _flushedCount = 0;
    static long _lastFlushTicks = 0;

    public static string OutputPath { get; set; }

    public static void Start()
    {
//#if DEBUG
//        lock (_lock)
//        {
//            _times.Clear();
//            _flushedCount = 0;
//            _lastFlushTicks = 0;
//            try
//            {
//                if (OutputPath is not null && File.Exists(OutputPath))
//                    File.Delete(OutputPath);
//            }
//            catch { }
//            _sw.Restart();
//        }
//#endif
    }

    public static void Record()
    {
//#if DEBUG
//        if (!_sw.IsRunning)
//            return;
//        lock (_lock)
//        {
//            _times.Add(_sw.Elapsed.TotalSeconds);
//            bool needFlush = _times.Count - _flushedCount >= 200
//                || _sw.ElapsedTicks - _lastFlushTicks >= Stopwatch.Frequency * 2;
//            if (needFlush)
//                FlushLocked();
//        }
//#endif
    }

    public static void Flush()
    {
        lock (_lock)
            FlushLocked();
    }

    static void FlushLocked()
    {
        if (OutputPath is null || _times.Count <= _flushedCount)
            return;
        try
        {
            var dir = Path.GetDirectoryName(OutputPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            var sb = new StringBuilder();
            for (int i = _flushedCount; i < _times.Count; i++)
                sb.Append(_times[i].ToString("G17", CultureInfo.InvariantCulture)).Append('\n');
            File.AppendAllText(OutputPath, sb.ToString());
            _flushedCount = _times.Count;
            _lastFlushTicks = _sw.ElapsedTicks;
        }
        catch { }
    }

    public static List<double> Snapshot()
    {
        lock (_lock)
            return new List<double>(_times);
    }
}
