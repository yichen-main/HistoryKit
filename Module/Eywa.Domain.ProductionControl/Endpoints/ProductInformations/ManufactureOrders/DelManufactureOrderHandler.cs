namespace Eywa.Domain.ProductionControl.Endpoints.ProductInformations.ManufactureOrders;
internal sealed class DelManufactureOrderHandler : NodeConsumer<DelManufactureOrderEvent>
{
    protected override Task HolderAsync(DelManufactureOrderEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var elder = TableLayout.LetDelete<ManufactureOrder>(@event.Id);
            await connection.ExecuteAsync(elder, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}