namespace Eywa.Domain.FacilityMachinery.Endpoints.ProcessMachineries.IndustrialEquipments;
internal sealed class DelIndustrialEquipmentVehicle() : NodeEnlarge<DelIndustrialEquipmentVehicle, DelIndustrialEquipmentInput>(RolePolicy.Owner)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(DelIndustrialEquipmentInput req, CancellationToken ct)
    {
        await VerifyAsync(FieldModule.FacilityManagement, ct).ConfigureAwait(false);
        await PublishAsync(new DelIndustrialEquipmentEvent
        {
            Id = req.Id,
        }, cancellation: ct).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}