namespace Eywa.Domain.HumanResources.PeopleManagements.Stockpiles.Accounts;
internal sealed class AccountRegistration : NpgsqlBase
{
    public const string EmailIndex = $"{nameof(AccountRegistration)}{nameof(Email)}";
    [RowInfo(UniqueIndex = true)] public required string Email { get; init; }
    public required string Salt { get; init; }
    public required string HashedText { get; init; }
}