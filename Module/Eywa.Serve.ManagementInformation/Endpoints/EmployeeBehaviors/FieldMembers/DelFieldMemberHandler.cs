namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.FieldMembers;
internal sealed class DelFieldMemberHandler : NodeConsumer<DelFieldMemberEvent>
{
    protected override Task HolderAsync(DelFieldMemberEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var elder = TableLayout.LetDelete<SystemDomainMember>(@event.Id);
            await connection.ExecuteAsync(elder, transaction).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        });
    }
}