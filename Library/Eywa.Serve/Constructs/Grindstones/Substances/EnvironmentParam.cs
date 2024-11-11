namespace Eywa.Serve.Constructs.Grindstones.Substances;
public readonly struct EnvironmentParam
{
    public required EnvironmentMeasure Measure { get; init; }
    public required string DataNo { get; init; }
}