namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountRegisters;
internal sealed class PutAccountRegisterVehicle : NodeEnlarge<PutAccountRegisterVehicle, PutAccountRegisterInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutAccountRegisterInput req, CancellationToken ct)
    {
        await VerifyAsync(ct).ConfigureAwait(false);
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var user = await reader!.ReadFirstOrDefaultAsync<UserRegistration>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierUserIdDoesNotExist));

                if (user is not null) await connection.ExecuteAsync(DurableSetup.DelimitUpdate(user.Id, new UserRegistration
                {
                    Email = req.Body.Email,
                    UserNo = req.Body.UserNo,
                    UserName = req.Body.UserName,
                    Disable = req.Body.Disable,
                    Salt = user.Salt,
                    HashedText = user.HashedText,
                }, ct)).ConfigureAwait(false);
            }
            catch (NpgsqlException e)
            {
                e.MakeException([
                    (UserRegistration.EmailIndex, DurableSetup.Link(EnterpriseIntegrationFlag.CarrierEmailIndex)),
                    (UserRegistration.UserNoIndex, DurableSetup.Link(EnterpriseIntegrationFlag.CarrierUserNoIndex)),
                ]);
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<UserRegistration>(req.Body.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}