namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionStations;
internal sealed class DelProductionStationVehicle() : NodeEnlarge<DelProductionStationVehicle, DelProductionStationInput>(RolePolicy.Owner)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(DelProductionStationInput req, CancellationToken ct)
    {
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await PublishAsync(new DelProductionStationEvent
        {
            Id = req.Id,
        }, cancellation: ct).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}