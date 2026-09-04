
public abstract class IInferenceEngine
{
    public Action Starting { get; set; }
    public Action Finished { get; set; }
    public abstract void Init();
    public abstract void Run();
}
