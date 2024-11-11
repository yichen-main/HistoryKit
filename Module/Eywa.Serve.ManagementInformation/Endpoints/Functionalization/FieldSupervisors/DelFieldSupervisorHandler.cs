namespace Eywa.Serve.ManagementInformation.Endpoints.Functionalization.FieldSupervisors;
internal sealed class DelFieldSupervisorHandler : NodeConsumer<DelFieldSupervisorEvent>
{
    protected override Task HolderAsync(DelFieldSupervisorEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var elder = TableLayout.LetDelete<SystemDomainModule>(@event.Id);
            await connection.ExecuteAsync(elder, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}