namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.FieldMembers;
internal sealed class PutFieldMemberVehicle() : NodeEnlarge<PutFieldMemberVehicle, PutFieldMemberInput>(RolePolicy.Owner)
{
    public override void Configure()
    {
        AllowFormData(urlEncoded: true);
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutFieldMemberInput req, CancellationToken ct)
    {
        var ownersAsync = CacheMediator.ListFieldOwnerAsync(ct);
        var registrarsAsync = CacheMediator.ListAllRegistrantsAsync(ct);
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var owners = await ownersAsync.ConfigureAwait(false);
            var registrars = await registrarsAsync.ConfigureAwait(false);
            var loginUser = registrars.First(x => x.Id.IsMatch(GetUserId()));
            var loginOwner = owners.First(x => x.Id.IsMatch(loginUser.Id));
            var registrar = registrars.FirstOrDefault(x => x.Id.IsMatch(req.Body.UserId));
            if (registrar.Id is null) throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierUserIdDoesNotExist));

            var domainMembers = await reader!.ReadAsync<SystemDomainMember>().ConfigureAwait(false);
            var loginUserModule = domainMembers.First(x => x.UserId.IsMatch(loginUser.Id));

            if (domainMembers.Any(x => x.UserId.IsMatch(req.Body.UserId) && x.RolePolicy == loginOwner.RoleType))
                throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierModuleUserAlreadyExists));

            var domainMember = domainMembers.First(x => x.Id.IsMatch(req.Body.Id));
            await connection.ExecuteAsync(DurableSetup.DelimitUpdate(registrar.Id, new SystemDomainMember
            {
                UserId = req.Body.UserId,
                FieldModule = loginOwner.FieldType,
                RolePolicy = loginOwner.RoleType,
                Disable = req.Body.Disable,
                Modifier = GetUserName(),
                Creator = domainMember.Creator,
                ModifyTime = DateTime.UtcNow,
            }, ct)).ConfigureAwait(false);
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<SystemDomainMember>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}