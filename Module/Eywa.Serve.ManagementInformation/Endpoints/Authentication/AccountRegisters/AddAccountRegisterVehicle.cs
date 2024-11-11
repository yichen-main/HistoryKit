namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountRegisters;
internal sealed class AddAccountRegisterVehicle : NodeEnlarge<AddAccountRegisterVehicle, AddAccountRegisterInput>
{
    public override void Configure()
    {
        AllowFormData(urlEncoded: true);
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddAccountRegisterInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await VerifyAsync(ct).ConfigureAwait(false);
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var salt = CiphertextPolicy.GenerateSalt();
                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new UserRegistration
                {
                    Id = id,
                    Email = req.Email,
                    UserNo = req.UserNo,
                    UserName = req.UserName,
                    Disable = req.Disable,
                    Salt = salt,
                    HashedText = CiphertextPolicy.HmacSHA256ToHex(req.Password, salt),
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
            QueryFields = [],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetAccountRegisterVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}