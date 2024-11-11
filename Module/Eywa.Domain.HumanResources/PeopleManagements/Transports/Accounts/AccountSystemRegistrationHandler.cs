namespace Eywa.Domain.HumanResources.PeopleManagements.Transports.Accounts;
internal sealed class AccountSystemRegistrationHandler : TransactHandler<AccountSystemRegistrationImport>
{
    protected override void Configure()
    {
        QueryFields = [
            TableLayout.LetSelect<AccountSystem>(),
        ];
    }
    protected override async Task HandleAsync(Options options)
    {
        var subject = PeopleManagementTool.Link(PeopleManagementCulture.HumanEmailRegistrationSubject);
        var system = await options.Reader!.ReadSingleOrDefaultAsync<AccountSystem>().ConfigureAwait(false);
        if (system is not null) throw new Exception(PeopleManagementTool.Link(PeopleManagementCulture.HumanSystemAccountIndex));
        try
        {
            var salt = CiphertextPolicy.GenerateSalt();
            var password = CiphertextPolicy.GenerateRandomPassword();
            await options.Connection.ExecuteAsync(DurableSetup.DelimitInsert(new AccountRegistration
            {
                Salt = salt,
                Email = options.Import.Email,
                Id = FileLayout.GetSnowflakeId(),
                HashedText = CiphertextPolicy.HmacSHA256ToHex(password, salt),
            }, options.Transaction, options.CancellationToken)).ConfigureAwait(false);
            var content = password.GetRegistrationContent(HostInformation.Value.SMTP.LoginPageUrl);
            await DurableSetup.SendEmailAsync(options.Import.Email, subject, content).ConfigureAwait(false);
        }
        catch (NpgsqlException e)
        {
            e.MakeException([
                (AccountRegistration.EmailIndex, PeopleManagementTool.Link(PeopleManagementCulture.HumanEmailIndex)),
            ]);
        }
    }
    public required IOptions<HostInformation> HostInformation { get; init; }
    public required IPeopleManagementTool PeopleManagementTool { get; init; }
}