namespace Eywa.Domain.ProductionControl.Databases.Masterdatas.Manufactures;
internal sealed class ManufactureOrder : NpgsqlBase
{
    public const string OrderNoIndex = $"{nameof(ManufactureOrder)}{nameof(OrderNo)}";
    [RowInfo(UniqueIndex = true)] public required string OrderNo { get; init; }
    public required string OrderName { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
}