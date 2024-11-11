namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountRegisters;
internal sealed class GetAccountRegisterVehicle : NodeEnlarge<GetAccountRegisterVehicle, GetAccountRegisterInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetAccountRegisterInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var user = await reader!.ReadFirstAsync<UserRegistration>().ConfigureAwait(false);
            output = new()
            {
                Id = user.Id,
                Email = user.Email,
                UserNo = user.UserNo,
                UserName = user.UserName,
                Disable = user.Disable,
                CreateTime = DurableSetup.LocalTime(user.CreateTime, req.TimeZone, req.TimeFormat),
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<UserRegistration>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
        public required string Email { get; init; }
        public required string UserNo { get; init; }
        public required string UserName { get; init; }
        public required bool Disable { get; init; }
        public required string CreateTime { get; init; }
    }
}