namespace Eywa.Serve.Constructs.Adminiculars.Substances;
internal readonly struct FluxDataGroup
{
    public required string MeasurementName { get; init; }
    public required string TagKey { get; init; }
    public required string TagValue { get; init; }
    public required string DataNo { get; init; }
    public required DateTime EndTime { get; init; }
    public required DateTime StartTime { get; init; }
}