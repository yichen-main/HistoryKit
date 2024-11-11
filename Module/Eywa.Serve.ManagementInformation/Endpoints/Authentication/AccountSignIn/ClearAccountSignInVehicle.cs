namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountSignIn;
internal sealed class ClearAccountSignInVehicle : NodeEnlarge<ClearAccountSignInVehicle, ClearAccountSignInInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ClearAccountSignInInput req, CancellationToken ct)
    {
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            var query = TableLayout.LetSelect<UserRegistration>(new TableLayout.ColumnFilter(nameof(UserRegistration.Email), GetUserId()));
            var user = await connection.QueryFirstOrDefaultAsync<UserRegistration>(query, transaction).ConfigureAwait(false);
            if (user is not null)
            {
                var (sql, param) = TableLayout.LetInsert(new UserActivityRecord
                {
                    Id = FileLayout.GetSnowflakeId(),
                    UserId = user.Id,
                    LoginStatus = LoginStatus.SignOut,
                });
                await connection.ExecuteAsync(sql, param, transaction).ConfigureAwait(false);
            }
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}