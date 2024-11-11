namespace Eywa.Serve.ManagementInformation.Databases.Masterdatas.Users;
internal sealed class UserRegistration : NpgsqlBase
{
    public const string EmailIndex = $"{nameof(UserRegistration)}{nameof(Email)}";
    public const string UserNoIndex = $"{nameof(UserRegistration)}{nameof(UserNo)}";
    [RowInfo(UniqueIndex = true)] public required string Email { get; init; }
    [RowInfo(UniqueIndex = true)] public required string UserNo { get; init; }
    public required string UserName { get; init; }
    public required bool Disable { get; init; }
    public required string Salt { get; init; }
    public required string HashedText { get; init; }
}