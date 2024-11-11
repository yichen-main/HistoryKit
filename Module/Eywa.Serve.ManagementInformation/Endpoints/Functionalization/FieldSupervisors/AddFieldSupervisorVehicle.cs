namespace Eywa.Serve.ManagementInformation.Endpoints.Functionalization.FieldSupervisors;
internal sealed class AddFieldSupervisorVehicle() : NodeEnlarge<AddFieldSupervisorVehicle, AddFieldSupervisorInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddFieldSupervisorInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await VerifyAsync(ct).ConfigureAwait(false);
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                var domains = await reader!.ReadAsync<SystemDomainModule>().ConfigureAwait(false);
                var user = await reader.ReadSingleAsync<UserRegistration>().ConfigureAwait(false);
                if (domains.Any(x => x.UserId.IsMatch(user.Id) && x.FieldType == req.Body.FieldType))
                    throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierFieldTypeExisted));
                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new SystemDomainModule
                {
                    Id = id,
                    UserId = user.Id,
                    FieldType = req.Body.FieldType,
                    Disable = req.Body.Disable,
                    Modifier = userName,
                    Creator = userName,
                    ModifyTime = DateTime.UtcNow,
                }, ct)).ConfigureAwait(false);
            }
            catch (NpgsqlException e)
            {
                e.MakeException();
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<SystemDomainModule>(),
                TableLayout.LetSelect<UserRegistration>(req.Body.UserId),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetFieldSupervisorVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}