namespace Eywa.Domain.FacilityMachinery.Databases.Masterdatas.Industrials;
internal sealed class IndustrialEquipment : NpgsqlBase
{
    public const string EquipmentNoIndex = $"{nameof(IndustrialEquipment)}{nameof(EquipmentNo)}";
    [RowInfo(UniqueIndex = true)] public required string EquipmentNo { get; init; }
    public required string EquipmentName { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
}