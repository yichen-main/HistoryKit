namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.FieldMembers;
internal sealed class GetFieldMemberVehicle : NodeEnlarge<GetFieldMemberVehicle, GetFieldMemberInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetFieldMemberInput req, CancellationToken ct)
    {
        Output output = default;
        var usersAsync = CacheMediator.ListAllRegistrantsAsync(ct);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var users = await usersAsync.ConfigureAwait(false);

            var domainMember = await reader!.ReadFirstOrDefaultAsync<SystemDomainMember>().ConfigureAwait(false)
            ?? throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierModuleIdDoesNotExist));

            var user = users.First(x => x.Id.IsMatch(domainMember.UserId));
            output = new()
            {
                Id = domainMember.Id,
                Disable = domainMember.Disable,
                FieldType = domainMember.FieldModule,
                FieldName = DurableSetup.Link(domainMember.FieldModule),
                AccessType = domainMember.RolePolicy,
                AccessName = DurableSetup.Link(domainMember.RolePolicy),
                UserId = user.Id,
                Email = user.Email,
                UserNo = user.UserNo,
                UserName = user.UserName,
                Creator = domainMember.Creator,
                CreateTime = DurableSetup.LocalTime(domainMember.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = domainMember.Modifier,
                ModifyTime = DurableSetup.LocalTime(domainMember.ModifyTime, req.TimeZone, req.TimeFormat),
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<SystemDomainMember>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
        public required bool Disable { get; init; }
        public required FieldModule FieldType { get; init; }
        public required string FieldName { get; init; }
        public required RolePolicy AccessType { get; init; }
        public required string AccessName { get; init; }
        public required string UserId { get; init; }
        public required string Email { get; init; }
        public required string UserNo { get; init; }
        public required string UserName { get; init; }
        public required string Creator { get; init; }
        public required string CreateTime { get; init; }
        public required string Modifier { get; init; }
        public required string ModifyTime { get; init; }
    }
}