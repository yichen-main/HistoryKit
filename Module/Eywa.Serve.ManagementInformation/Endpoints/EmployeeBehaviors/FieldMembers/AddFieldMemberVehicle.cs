namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.FieldMembers;
internal sealed class AddFieldMemberVehicle() : NodeEnlarge<AddFieldMemberVehicle, AddFieldMemberInput>(RolePolicy.Owner)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddFieldMemberInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        var ownersAsync = CacheMediator.ListFieldOwnerAsync(ct);
        var registrarsAsync = CacheMediator.ListAllRegistrantsAsync(ct);
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(req.Body.FieldType, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                var owners = await ownersAsync.ConfigureAwait(false);
                var registrars = await registrarsAsync.ConfigureAwait(false);
                var loginUser = registrars.First(x => x.Id.IsMatch(GetUserId()));
                var loginOwner = owners.First(x => x.Id.IsMatch(loginUser.Id));
                var registrar = registrars.FirstOrDefault(x => x.Id.IsMatch(req.Body.UserId));
                if (registrar.Id is null) throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierUserIdDoesNotExist));
                var domainMembers = await reader!.ReadAsync<SystemDomainMember>().ConfigureAwait(false);
                if (domainMembers.Any(x => x.UserId.IsMatch(registrar.Id) && x.RolePolicy == loginOwner.RoleType))
                {
                    throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierModuleUserAlreadyExists));
                }
                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new SystemDomainMember
                {
                    Id = id,
                    UserId = registrar.Id,
                    FieldModule = loginOwner.FieldType,
                    RolePolicy = req.Body.AccessType,
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
                TableLayout.LetSelect<SystemDomainMember>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetFieldMemberVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}