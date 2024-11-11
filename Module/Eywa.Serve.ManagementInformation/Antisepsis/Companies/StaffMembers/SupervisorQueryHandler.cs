namespace Eywa.Serve.ManagementInformation.Antisepsis.Companies.StaffMembers;
internal sealed class SupervisorQueryHandler : ExecuteHandler<FieldTeamQueryImport.Owner, IEnumerable<FieldTeamQueryImport.Output>>
{
    protected override void Configure()
    {
        QueryFields = [
            TableLayout.LetSelect<SystemDomainModule>(),
            TableLayout.LetSelect<UserRegistration>(),
        ];
    }
    protected override async Task<IEnumerable<FieldTeamQueryImport.Output>> HandleAsync(Options combine)
    {
        List<FieldTeamQueryImport.Output> outputs = [];
        var modules = await combine.Reader!.ReadAsync<SystemDomainModule>().ConfigureAwait(false);
        var users = await combine.Reader.ReadAsync<UserRegistration>().ConfigureAwait(false);
        foreach (var module in modules.Where(x => x is { Disable: false }))
        {
            var user = users.First(x => x.Id.IsMatch(module.UserId));
            outputs.Add(new()
            {
                Id = user.Id,
                Email = user.Email,
                UserNo = user.UserNo,
                UserName = user.UserName,
                DomainId = module.Id,
                FieldType = module.FieldType,
                RoleType = RolePolicy.Owner,
            });
        }
        return outputs.AsEnumerable();
    }
}