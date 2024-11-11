namespace Eywa.Domain.ProductionControl.Databases.Masterdatas.Productions;
internal sealed class ProductionMachine : NpgsqlBase
{
    [RowInfo(ForeignKey = true)] public required string EquipmentId { get; init; }
    [RowInfo(ForeignKey = true)] public required string ProcessId { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
    public static IAsyncEnumerable<string> GetEquipmentChildDeleter(NpgsqlConnection connection, string id)
    {
        return TableLayout.LetDeleteSubtag<ProductionMachine>(connection, nameof(EquipmentId), id);
    }
    public static IAsyncEnumerable<string> GetProcessChildDeleter(NpgsqlConnection connection, string id)
    {
        return TableLayout.LetDeleteSubtag<ProductionMachine>(connection, nameof(ProcessId), id);
    }
}