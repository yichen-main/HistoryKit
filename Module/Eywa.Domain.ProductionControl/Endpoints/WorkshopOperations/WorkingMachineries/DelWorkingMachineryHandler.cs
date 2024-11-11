namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingMachineries;
internal sealed class DelWorkingMachineryHandler : NodeConsumer<DelWorkingMachineryEvent>
{
    protected override Task HolderAsync(DelWorkingMachineryEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var sql = TableLayout.LetDelete<WorkshopMachinery>(@event.Id);
            await connection.ExecuteAsync(sql, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}