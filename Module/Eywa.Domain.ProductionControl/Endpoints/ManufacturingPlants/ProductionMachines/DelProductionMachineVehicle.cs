namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionMachines;
internal sealed class DelProductionMachineVehicle() : NodeEnlarge<DelProductionMachineVehicle, DelProductionMachineInput>(RolePolicy.Owner)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(DelProductionMachineInput req, CancellationToken ct)
    {
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await PublishAsync(new DelProductionMachineEvent
        {
            Id = req.Id,
        }, cancellation: ct).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}