namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionMachines;
internal sealed class DelProductionMachineHandler : NodeConsumer<DelProductionMachineEvent>
{
    protected override Task HolderAsync(DelProductionMachineEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var sql = TableLayout.LetDelete<ProductionMachine>(@event.Id);
            await connection.ExecuteAsync(sql, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}