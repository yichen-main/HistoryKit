namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingLocations;
internal sealed class AddWorkingLocationVehicle() : NodeEnlarge<AddWorkingLocationVehicle, AddWorkingLocationInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddWorkingLocationInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                var members = await CacheMediator.ListFieldMemberAsync(ct).ConfigureAwait(false);
                var groups = await reader!.ReadAsync<ProductionGroup>().ConfigureAwait(false);

                var station = await reader.ReadFirstOrDefaultAsync<ProductionStation>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceStationIdDoesNotExist));

                var @operator = await reader.ReadFirstOrDefaultAsync<ProductionOperator>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceOperatorIdDoesNotExist));

                var group = groups.FirstOrDefault(x => x.Id.IsMatch(@operator.GroupId))
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceGroupIdDoesNotExist));

                var member = members.FirstOrDefault(x => x.Id.IsMatch(@operator.MemberId));
                if (member.Id == default) throw new Exception(DurableSetup.Link(HumanResourcesFlag.HumanMemberIdDoesNotExist));

                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new WorkshopLocation
                {
                    Id = id,
                    StationId = req.Body.StationId,
                    OperatorId = req.Body.OperatorId,
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
                TableLayout.LetSelect<ProductionGroup>(),
                TableLayout.LetSelect<ProductionStation>(req.Body.StationId),
                TableLayout.LetSelect<ProductionOperator>(req.Body.OperatorId),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetWorkingLocationVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}