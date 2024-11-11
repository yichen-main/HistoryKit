namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingLocations;
internal sealed class PutWorkingLocationVehicle() : NodeEnlarge<PutWorkingLocationVehicle, PutWorkingLocationInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutWorkingLocationInput req, CancellationToken ct)
    {
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            try
            {
                var members = await CacheMediator.ListFieldMemberAsync(ct).ConfigureAwait(false);
                var groups = await reader!.ReadAsync<ProductionGroup>().ConfigureAwait(false);

                var station = await reader.ReadFirstOrDefaultAsync<ProductionStation>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceStationIdDoesNotExist));

                var @operator = await reader.ReadFirstOrDefaultAsync<ProductionOperator>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceOperatorIdDoesNotExist));

                var location = await reader.ReadFirstOrDefaultAsync<WorkshopLocation>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.WorkingLocationIdDoesNotExist));

                var group = groups.FirstOrDefault(x => x.Id.IsMatch(@operator.GroupId))
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceGroupIdDoesNotExist));

                var member = members.FirstOrDefault(x => x.Id.IsMatch(@operator.MemberId));
                if (member.Id == default) throw new Exception(DurableSetup.Link(HumanResourcesFlag.HumanMemberIdDoesNotExist));

                await connection.ExecuteAsync(DurableSetup.DelimitUpdate(location.Id, new WorkshopLocation
                {
                    StationId = station.Id,
                    OperatorId = @operator.Id,
                    Modifier = GetUserName(),
                    Creator = location.Creator,
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
                TableLayout.LetSelect<WorkshopLocation>(req.Body.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}