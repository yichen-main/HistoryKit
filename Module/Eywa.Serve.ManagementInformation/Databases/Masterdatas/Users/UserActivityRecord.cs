namespace Eywa.Serve.ManagementInformation.Databases.Masterdatas.Users;
internal sealed class UserActivityRecord : NpgsqlBase
{
    [RowInfo(ForeignKey = true)] public required string UserId { get; init; }
    public required LoginStatus LoginStatus { get; init; }
    public static IAsyncEnumerable<string> GetUserChildDeleter(NpgsqlConnection connection, string id)
    {
        return TableLayout.LetDeleteSubtag<UserActivityRecord>(connection, nameof(UserId), id);
    }
}