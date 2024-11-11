namespace Eywa.Domain.ProductionControl.Databases.Masterdatas.Workshops;
internal sealed class WorkshopLocation : NpgsqlBase
{
    [RowInfo(ForeignKey = true)] public required string StationId { get; init; }
    [RowInfo(ForeignKey = true)] public required string OperatorId { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
    public static IAsyncEnumerable<string> GetStationChildDeleter(NpgsqlConnection connection, string id)
    {
        return TableLayout.LetDeleteSubtag<WorkshopLocation>(connection, nameof(StationId), id);
    }
    public static IAsyncEnumerable<string> GetOperatorChildDeleter(NpgsqlConnection connection, string id)
    {
        return TableLayout.LetDeleteSubtag<WorkshopLocation>(connection, nameof(OperatorId), id);
    }
}