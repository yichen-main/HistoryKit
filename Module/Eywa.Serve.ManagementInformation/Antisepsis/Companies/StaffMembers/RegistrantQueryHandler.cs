namespace Eywa.Serve.ManagementInformation.Antisepsis.Companies.StaffMembers;
internal sealed class RegistrantQueryHandler : ExecuteHandler<RegistrantQueryImport.Everyone, IEnumerable<RegistrantQueryImport.Output>>
{
    protected override void Configure()
    {
        QueryFields = [
            TableLayout.LetSelect<UserRegistration>(),
        ];
    }
    protected override async Task<IEnumerable<RegistrantQueryImport.Output>> HandleAsync(Options combine)
    {
        List<RegistrantQueryImport.Output> outputs = [];
        var users = await combine.Reader!.ReadAsync<UserRegistration>().ConfigureAwait(false);
        foreach (var user in users) outputs.Add(new()
        {
            Id = user.Id,
            Email = user.Email,
            UserNo = user.UserNo,
            UserName = user.UserName,
        });
        return outputs.AsEnumerable();
    }
}