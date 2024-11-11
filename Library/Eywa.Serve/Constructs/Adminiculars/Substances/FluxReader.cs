namespace Eywa.Serve.Constructs.Adminiculars.Substances;
internal readonly struct FluxReader
{
    public required TimeseriesBucket Bucket { get; init; }
    public required string MeasurementName { get; init; }
    public required DateTime EndTime { get; init; }
    public required DateTime StartTime { get; init; }
    public required string[] Conditions { get; init; }
}