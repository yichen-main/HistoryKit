namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingLocations;
internal sealed class DelWorkingLocationVehicle() : NodeEnlarge<DelWorkingLocationVehicle, DelWorkingLocationInput>(RolePolicy.Owner)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(DelWorkingLocationInput req, CancellationToken ct)
    {
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await PublishAsync(new DelWorkingLocationEvent
        {
            Id = req.Id,
        }, cancellation: ct).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}