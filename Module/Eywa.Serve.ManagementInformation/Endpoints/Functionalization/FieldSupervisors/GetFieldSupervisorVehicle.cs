namespace Eywa.Serve.ManagementInformation.Endpoints.Functionalization.FieldSupervisors;
internal sealed class GetFieldSupervisorVehicle : NodeEnlarge<GetFieldSupervisorVehicle, GetFieldSupervisorInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetFieldSupervisorInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var users = await reader!.ReadAsync<UserRegistration>().ConfigureAwait(false);
            var module = await reader.ReadSingleAsync<SystemDomainModule>().ConfigureAwait(false);
            var user = users.First(x => x.Id.IsMatch(module.UserId));
            output = new()
            {
                Id = module.Id,
                Disable = user.Disable,
                FieldType = module.FieldType,
                FieldName = DurableSetup.Link(module.FieldType),
                UserId = user.Id,
                Email = user.Email,
                UserNo = user.UserNo,
                UserName = user.UserName,
                Creator = module.Creator,
                CreateTime = DurableSetup.LocalTime(module.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = module.Modifier,
                ModifyTime = DurableSetup.LocalTime(module.ModifyTime, req.TimeZone, req.TimeFormat),
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<UserRegistration>(),
                TableLayout.LetSelect<SystemDomainModule>(req.Id),
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