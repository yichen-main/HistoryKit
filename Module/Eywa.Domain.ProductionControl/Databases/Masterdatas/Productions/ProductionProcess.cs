namespace Eywa.Domain.ProductionControl.Databases.Masterdatas.Productions;
internal sealed class ProductionProcess : NpgsqlBase
{
    public const string ProcessNoIndex = $"{nameof(ProductionProcess)}{nameof(ProcessNo)}";
    [RowInfo(UniqueIndex = true)] public required string ProcessNo { get; init; }
    public required string ProcessName { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
}