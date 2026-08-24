
public class GeoInferenceConfig
{
    public List<(Type, Type)> Components { get; set; } = [];
    public List<Type> OutputMakers { get; set; } = [];
    public List<Type> PlugIns { get; set; } = [];
    public List<object> Settings { get; set; } = [];
    public void SetComponent<Target, Actual>()
    {
        Components.Add((typeof(Target), typeof(Actual)));
    }
    public void AddOutputMaker<T>()
    {
        OutputMakers.Add(typeof(T));
    }
    public void AddPlugIn<T>()
    {
        PlugIns.Add(typeof(T));
    }
    public void AddSetting(object obj)
    {
        Settings.Add(obj);
    }


}
