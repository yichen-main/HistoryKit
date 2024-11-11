namespace Eywa.Domain.ProductionControl.Databases.Masterdatas.Productions;
internal sealed class ProductionOperator : NpgsqlBase
{
    [RowInfo(ForeignKey = true)] public required string GroupId { get; init; }
    [RowInfo(ForeignKey = true)] public required string MemberId { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
    public static IAsyncEnumerable<string> GetGroupChildDeleter(NpgsqlConnection connection, string id)
    {
        return TableLayout.LetDeleteSubtag<ProductionOperator>(connection, nameof(GroupId), id);
    }
    public static IAsyncEnumerable<string> GetMemberChildDeleter(NpgsqlConnection connection, string id)
    {
        return TableLayout.LetDeleteSubtag<ProductionOperator>(connection, nameof(MemberId), id);
    }
}