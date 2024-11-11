namespace Eywa.Serve.Constructs.Grindstones.Substances;
public readonly struct EnvironmentParamGroup
{
    public required EnvironmentMeasure Measure { get; init; }
    public required string TagKey { get; init; }
    public required string TagValue { get; init; }
    public required string DataNo { get; init; }
}