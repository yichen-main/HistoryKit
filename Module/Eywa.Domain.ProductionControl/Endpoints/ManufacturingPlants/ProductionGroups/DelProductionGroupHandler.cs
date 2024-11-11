namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionGroups;
internal sealed class DelProductionGroupHandler : NodeConsumer<DelProductionGroupEvent>
{
    protected override Task HolderAsync(DelProductionGroupEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var elder = TableLayout.LetDelete<ProductionGroup>(@event.Id);
            await connection.ExecuteAsync(elder, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}