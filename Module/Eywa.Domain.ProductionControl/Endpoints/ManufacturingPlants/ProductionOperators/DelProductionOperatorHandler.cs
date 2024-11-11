namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionOperators;
internal sealed class DelProductionOperatorHandler : NodeConsumer<DelProductionOperatorEvent>
{
    protected override Task HolderAsync(DelProductionOperatorEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var sql = TableLayout.LetDelete<ProductionOperator>(@event.Id);
            await connection.ExecuteAsync(sql, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}