namespace Eywa.Domain.ProductionControl.Databases.Masterdatas.Productions;
internal sealed class ProductionStation : NpgsqlBase
{
    public const string StationNoIndex = $"{nameof(ProductionStation)}{nameof(StationNo)}";
    [RowInfo(UniqueIndex = true)] public required string StationNo { get; init; }
    public required string StationName { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
}