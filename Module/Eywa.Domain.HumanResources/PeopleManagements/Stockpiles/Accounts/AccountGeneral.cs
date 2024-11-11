namespace Eywa.Domain.HumanResources.PeopleManagements.Stockpiles.Accounts;
internal sealed class AccountGeneral : NpgsqlBase
{
    public const string UserNoIndex = $"{nameof(AccountGeneral)}{nameof(UserNo)}";
    [RowInfo(ForeignKey = true)] public required string RegistrationId { get; init; }
    [RowInfo(UniqueIndex = true)] public required string UserNo { get; init; }
    public required bool Disable { get; init; }
}