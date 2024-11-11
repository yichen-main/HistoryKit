namespace Eywa.Serve.ManagementInformation.Endpoints.Functionalization.FieldSupervisors;
internal sealed class PutFieldSupervisorVehicle() : NodeEnlarge<PutFieldSupervisorVehicle, PutFieldSupervisorInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutFieldSupervisorInput req, CancellationToken ct)
    {
        await VerifyAsync(ct).ConfigureAwait(false);
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var domains = await reader!.ReadAsync<SystemDomainModule>().ConfigureAwait(false);
                var user = await reader.ReadSingleAsync<UserRegistration>().ConfigureAwait(false);
                if (domains.Any(x => x.UserId.IsMatch(user.Id) && x.FieldType == req.Body.FieldType))
                    throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierFieldTypeExisted));
                var domain = domains.First(x => x.Id.IsMatch(req.Body.Id));
                await connection.ExecuteAsync(DurableSetup.DelimitUpdate(user.Id, new SystemDomainModule
                {
                    Id = domain.Id,
                    UserId = user.Id,
                    FieldType = req.Body.FieldType,
                    Disable = req.Body.Disable,
                    Modifier = GetUserName(),
                    Creator = domain.Creator,
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
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}