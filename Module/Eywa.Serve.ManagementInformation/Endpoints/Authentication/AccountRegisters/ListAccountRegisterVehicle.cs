namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountRegisters;
internal sealed class ListAccountRegisterVehicle : NodeEnlarge<ListAccountRegisterVehicle, ListAccountRegisterInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListAccountRegisterInput req, CancellationToken ct)
    {
        List<GetAccountRegisterVehicle.Output> outputs = [];
        await VerifyAsync(ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var users = await reader!.ReadAsync<UserRegistration>().ConfigureAwait(false);
            foreach (var user in users) outputs.Add(new()
            {
                Id = user.Id,
                Email = user.Email,
                UserNo = user.UserNo,
                UserName = user.UserName,
                Disable = user.Disable,
                CreateTime = DurableSetup.LocalTime(user.CreateTime, req.TimeZone, req.TimeFormat),
            });
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<UserRegistration>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);

        await SendAsync(Pagination(new PageContents<GetAccountRegisterVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
        ]), cancellation: ct).ConfigureAwait(false);
    }
}