
public class TimeoutPlugInConfig : IInferenceConfig
{
    public int TimeoutSeconds { get; set; } = 600;
    public IEnumerable<Type> PlugInTypes => [typeof(TimeoutPlugIn)];
}
internal class TimeoutPlugIn : ISingletonInit, IInferencePlugIn
{
    [DI] TimeoutPlugInConfig Config { get; set; }
    [DI] KnowledgeBaseUpdater Updater { get; set; }
    [DI] GeoInferenceApp App;
    public DateTime StartTime;
    public void Init()
    {
        StartTime= DateTime.Now;
        Updater.KnowledgeAdding += CheckTimeout;
    }

    private void CheckTimeout()
    {
        if((DateTime.Now -StartTime ).TotalSeconds> Config.TimeoutSeconds)
        {
            App.IsActivedStop = true;
            App.ActivedStopReasons.Add($"Timeout: The inference engine has been running for more than {Config.TimeoutSeconds} seconds.");
            Updater.KnowledgeAdding -= CheckTimeout;
        }
    }
}
