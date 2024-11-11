namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountRegisters;
internal sealed class DelAccountRegisterHandler : NodeConsumer<DelAccountRegisterEvent>
{
    protected override Task HolderAsync(DelAccountRegisterEvent @event, CancellationToken ct)
    {
        return ExecuteAsync(async (connection, transaction, reader) =>
        {
            var user = await reader!.ReadFirstOrDefaultAsync<UserRegistration>().ConfigureAwait(false);
            if (user is not null)
            {
                await foreach (var child in GetChilds().WithCancellation(ct).ConfigureAwait(false))
                {
                    await connection.ExecuteAsync(child, transaction).ConfigureAwait(false);
                }
                var elder = TableLayout.LetDelete<UserRegistration>(user.Id);
                await connection.ExecuteAsync(elder, transaction).ConfigureAwait(false);
            }
            async IAsyncEnumerable<string> GetChilds()
            {
                var activityRecords = UserActivityRecord.GetUserChildDeleter(connection, user.Id);
                await foreach (var activityRecord in activityRecords.ConfigureAwait(false).WithCancellation(ct))
                {
                    yield return activityRecord;
                }
                var domainModules = SystemDomainModule.GetUserChildDeleter(connection, user.Id);
                await foreach (var domainModule in domainModules.ConfigureAwait(false).WithCancellation(ct))
                {
                    yield return domainModule;
                }
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<UserRegistration>(@event.Id),
            ],
            CancellationToken = ct,
        });
    }
}