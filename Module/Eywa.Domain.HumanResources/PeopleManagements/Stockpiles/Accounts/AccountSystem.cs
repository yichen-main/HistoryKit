namespace Eywa.Domain.HumanResources.PeopleManagements.Stockpiles.Accounts;
internal sealed class AccountSystem : NpgsqlBase
{
    [RowInfo(ForeignKey = true)] public required string RegistrationId { get; init; }
    public required string Organization { get; init; }
}