using System;
using System.Collections.Generic;
using System.Text;

using ZTool.Infrastructures.Mapper;

using static ZTool.Infrastructures.Log.ZLog;


[Alias("日志输出生成器")]
public class InferenceLogMaker : IOutputMaker
{
    string name;
    public string Name { get => name; set => name = value; }

    public object Make()
    {
        InferenceLog logOutput = new();
        logOutput.KeyPoints = KeyPoints.Select(x => (ZPair<string, DateTime>)x).ToList();
        foreach (var kv in CateLogContents)
        {
            logOutput.CateLogContents.Add(kv.Key, []);
            foreach (var m in kv.Value)
            {
                List<LogContent> lc = [];
                logOutput.CateLogContents[kv.Key].Add(m.Key, lc);

                foreach (var zlogContent in m.Value)
                {
                    var log = zlogContent.To<LogContent>();
                    lc.Add(log);
                }
            }
        }
        foreach (var zlogContent in LogContents)
        {
            var log = zlogContent.To<LogContent>();
            logOutput.LogContents.Add(log);
        }
        string timerStr = "timerStr";
        logOutput.CateLogContents.Add(timerStr, []);
        List<LogContent> lc2 = [];
        foreach (var item in ZLog.GetTimeSpans())
        {
            LogContent log = new LogContent();
            log.Class = timerStr;
            log.Method = item.Key;
            log.Level = "Trace";
            log.Content = item.Value.ToString();
            lc2.Add(log);
            logOutput.LogContents.Add(log);
        }
        logOutput.CateLogContents[timerStr].Add(timerStr, lc2);
        return logOutput;
    }
}
