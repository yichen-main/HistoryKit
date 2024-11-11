namespace Eywa.Domain.ProductionControl.Endpoints.ProductInformations.ManufactureOrders;
internal sealed class DelManufactureOrderVehicle() : NodeEnlarge<DelManufactureOrderVehicle, DelManufactureOrderInput>(RolePolicy.Owner)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(DelManufactureOrderInput req, CancellationToken ct)
    {
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await PublishAsync(new DelManufactureOrderEvent
        {
            Id = req.Id,
        }, cancellation: ct).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}