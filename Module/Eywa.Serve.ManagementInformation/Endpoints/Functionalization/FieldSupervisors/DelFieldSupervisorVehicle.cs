namespace Eywa.Serve.ManagementInformation.Endpoints.Functionalization.FieldSupervisors;
internal sealed class DelFieldSupervisorVehicle : NodeEnlarge<DelFieldSupervisorVehicle, DelFieldSupervisorInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(DelFieldSupervisorInput req, CancellationToken ct)
    {
        await VerifyAsync(ct).ConfigureAwait(false);
        await PublishAsync(new DelFieldSupervisorEvent
        {
            Id = req.Id,
        }, cancellation: ct).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}