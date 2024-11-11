namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountOperates;
internal sealed class PutAccountOperateVehicle : NodeEnlarge<PutAccountOperateVehicle, PutAccountOperateInput>
{
    public override void Configure()
    {
        AllowFormData(urlEncoded: true);
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutAccountOperateInput req, CancellationToken ct)
    {
        var userId = GetUserId();
        var rootFileAsync = CiphertextPolicy.GetRootFileAsync(ct);
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var salt = CiphertextPolicy.GenerateSalt();
                var profile = await rootFileAsync.ConfigureAwait(false);
                switch (userId)
                {
                    case var x when x.IsMatch(profile.Id):
                        if (!File.Exists(CiphertextPolicy.RootFilePath))
                        {
                            throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierSystemAccountIsRequired));
                        }
                        await CiphertextPolicy.SetRootAsync(new()
                        {
                            Id = FileLayout.GetSnowflakeId(),
                            Salt = salt,
                            Hash = CiphertextPolicy.HmacSHA256ToHex(req.Password, salt),
                        }, ct).ConfigureAwait(false);
                        break;

                    default:
                        var user = await reader!.ReadFirstOrDefaultAsync<UserRegistration>().ConfigureAwait(false)
                        ?? throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierUserIdDoesNotExist));

                        if (user is not null) await connection.ExecuteAsync(DurableSetup.DelimitUpdate(user.Id, new UserRegistration
                        {
                            Email = user.Email,
                            UserNo = user.UserNo,
                            UserName = user.UserName,
                            Disable = user.Disable,
                            Salt = salt,
                            HashedText = CiphertextPolicy.HmacSHA256ToHex(req.Password, salt),
                        }, ct)).ConfigureAwait(false);
                        break;
                }
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
                TableLayout.LetSelect<UserRegistration>(userId),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}