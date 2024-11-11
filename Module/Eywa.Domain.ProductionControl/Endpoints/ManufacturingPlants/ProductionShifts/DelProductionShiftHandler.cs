namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionShifts;
internal sealed class DelProductionShiftHandler : NodeConsumer<DelProductionShiftEvent>
{
    protected override Task HolderAsync(DelProductionShiftEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var elder = TableLayout.LetDelete<ProductionShift>(@event.Id);
            await connection.ExecuteAsync(elder, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}