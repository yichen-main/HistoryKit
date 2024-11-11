namespace Eywa.Domain.ProductionControl.Databases.Masterdatas.Productions;
internal sealed class ProductionGroup : NpgsqlBase
{
    public const string GroupNoIndex = $"{nameof(ProductionGroup)}{nameof(GroupNo)}";
    [RowInfo(UniqueIndex = true)] public required string GroupNo { get; init; }
    public required string GroupName { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
}