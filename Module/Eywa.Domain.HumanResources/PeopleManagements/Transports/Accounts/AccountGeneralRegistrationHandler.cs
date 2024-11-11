namespace Eywa.Domain.HumanResources.PeopleManagements.Transports.Accounts;
internal sealed class AccountGeneralRegistrationHandler : TransactHandler<AccountGeneralRegistrationImport>
{
    protected override void Configure()
    {
        QueryFields = [
            TableLayout.LetSelect<AccountGeneral>(),
        ];
    }
    protected override Task HandleAsync(Options options)
    {
        throw new NotImplementedException();
    }
}