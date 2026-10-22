﻿
[Alias("三角形的中位线")]
public class MidsegmentOfTriangle : Predicate
{
    public MidsegmentOfTriangle(Segment midSegment, Segment parallelSegment, Triangle triangle)
    {
        Add(midSegment, parallelSegment, triangle);
        Normalize();
        SetHashCode();

    }

    public override string ToString() => GeoInferenceApp.UseChinese
      ? $"{Properties[2]}的中位线是{Properties[0]}"
      : $"{Properties[0]} is the midline of {Properties[2]}";

    public override void Normalize()
    {

    }
}
