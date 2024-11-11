namespace Eywa.Domain.FacilityMachinery.Endpoints.ProcessMachineries.IndustrialEquipments;
internal sealed class DelIndustrialEquipmentHandler : NodeConsumer<DelIndustrialEquipmentEvent>
{
    protected override Task HolderAsync(DelIndustrialEquipmentEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var elder = TableLayout.LetDelete<IndustrialEquipment>(@event.Id);
            await connection.ExecuteAsync(elder, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}