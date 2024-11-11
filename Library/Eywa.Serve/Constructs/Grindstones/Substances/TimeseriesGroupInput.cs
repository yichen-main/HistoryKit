namespace Eywa.Serve.Constructs.Grindstones.Substances;
public readonly ref struct TimeseriesGroupInput
{
    public required string TagName { get; init; }
    public required string TagValue { get; init; }
    public required string DataNo { get; init; }
    public required double DataValue { get; init; }
    public required DateTime CreateTime { get; init; }
}