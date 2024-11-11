namespace Eywa.Domain.ProductionControl.Databases.Masterdatas.Workshops;
internal sealed class WorkshopMachinery : NpgsqlBase
{
    [RowInfo(ForeignKey = true)] public required string StationId { get; init; }
    [RowInfo(ForeignKey = true)] public required string MachineId { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
    public static IAsyncEnumerable<string> GetStationChildDeleter(NpgsqlConnection connection, string id)
    {
        return TableLayout.LetDeleteSubtag<WorkshopMachinery>(connection, nameof(StationId), id);
    }
    public static IAsyncEnumerable<string> GetMachineChildDeleter(NpgsqlConnection connection, string id)
    {
        return TableLayout.LetDeleteSubtag<WorkshopMachinery>(connection, nameof(MachineId), id);
    }
}