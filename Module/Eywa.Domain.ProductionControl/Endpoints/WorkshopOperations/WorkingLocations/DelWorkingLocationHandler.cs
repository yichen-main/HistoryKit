namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingLocations;
internal sealed class DelWorkingLocationHandler : NodeConsumer<DelWorkingLocationEvent>
{
    protected override Task HolderAsync(DelWorkingLocationEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var sql = TableLayout.LetDelete<WorkshopLocation>(@event.Id);
            await connection.ExecuteAsync(sql, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}