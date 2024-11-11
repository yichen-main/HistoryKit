namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.ActivityRecords;
internal sealed class DelActivityRecordHandler : NodeConsumer<DelActivityRecordEvent>
{
    protected override Task HolderAsync(DelActivityRecordEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var elder = TableLayout.LetDelete<UserActivityRecord>(@event.Id);
            await connection.ExecuteAsync(elder, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}