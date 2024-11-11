namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountRegisters;
internal sealed class DelAccountRegisterVehicle : NodeEnlarge<DelAccountRegisterVehicle, DelAccountRegisterInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(DelAccountRegisterInput req, CancellationToken ct)
    {
        await VerifyAsync(ct).ConfigureAwait(false);
        await PublishAsync(new DelAccountRegisterEvent
        {
            Id = req.Id,
        }, cancellation: ct).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}