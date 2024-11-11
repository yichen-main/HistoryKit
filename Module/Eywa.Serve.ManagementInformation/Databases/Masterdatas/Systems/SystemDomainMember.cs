namespace Eywa.Serve.ManagementInformation.Databases.Masterdatas.Systems;
internal sealed class SystemDomainMember : NpgsqlBase
{
    [RowInfo(ForeignKey = true)] public required string UserId { get; init; }
    public required FieldModule FieldModule { get; init; }
    public required RolePolicy RolePolicy { get; init; }
    public required bool Disable { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
    public static IAsyncEnumerable<string> GetUserChildDeleter(NpgsqlConnection connection, string id)
    {
        return TableLayout.LetDeleteSubtag<SystemDomainMember>(connection, nameof(UserId), id);
    }
}