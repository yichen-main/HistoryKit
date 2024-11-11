namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionProcesses;
internal sealed class DelProductionProcessVehicle() : NodeEnlarge<DelProductionProcessVehicle, DelProductionProcessInput>(RolePolicy.Owner)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(DelProductionProcessInput req, CancellationToken ct)
    {
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await PublishAsync(new DelProductionProcessEvent
        {
            Id = req.Id,
        }, cancellation: ct).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}