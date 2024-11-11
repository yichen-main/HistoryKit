namespace Eywa.Serve.ManagementInformation.Endpoints.SystemAccounts;
internal sealed class AddSystemAccountVehicle : NodeEnlarge<AddSystemAccountVehicle, AddSystemAccountInput>
{
    public override void Configure()
    {
        AllowAnonymous();
        AllowFormData(urlEncoded: true);
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddSystemAccountInput req, CancellationToken ct)
    {
        if (File.Exists(CiphertextPolicy.RootFilePath))
        {
            throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierSystemAccountAlreadyExists));
        }
        var leachAsync = Validator.LeachAsync(req, ct);
        var salt = CiphertextPolicy.GenerateSalt();
        var rootAsync = CiphertextPolicy.SetRootAsync(new()
        {
            Id = FileLayout.GetSnowflakeId(),
            Salt = salt,
            Hash = CiphertextPolicy.HmacSHA256ToHex(req.Password, salt),
        }, ct);
        await leachAsync.ConfigureAwait(false);
        await rootAsync.ConfigureAwait(false);
        await SendCreatedAtAsync<ListSystemAccountVehicle>(new(), default, cancellation: ct).ConfigureAwait(false);
    }
}