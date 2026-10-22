namespace GeoInference.Knowledges;

public abstract class Figure : Predicate
{
    public virtual string ForQuantity()
    {
        return "default";
    }

    protected GeoQuantity GetQuantity(ref GeoQuantity cache, string propName)
    {
        if (ReferenceEquals(cache, null))
        {
            cache = new GeoQuantity([this], propName);
        }
        return cache;
    }
}
