namespace Eywa.Domain.ProductionControl.Databases.Masterdatas.Productions;
internal sealed class ProductionShift : NpgsqlBase
{
    public const string ShiftNoIndex = $"{nameof(ProductionShift)}{nameof(ShiftNo)}";
    [RowInfo(UniqueIndex = true)] public required string ShiftNo { get; init; }
    public required string ShiftName { get; init; }
    public required TimeSpan StartTime { get; init; }
    public required TimeSpan EndTime { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
}