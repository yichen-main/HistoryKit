namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionOperators;
internal sealed class PutProductionOperatorVehicle() : NodeEnlarge<PutProductionOperatorVehicle, PutProductionOperatorInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutProductionOperatorInput req, CancellationToken ct)
    {
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            try
            {
                var members = await CacheMediator.ListFieldMemberAsync(ct).ConfigureAwait(false);
                var groups = await reader!.ReadAsync<ProductionGroup>().ConfigureAwait(false);

                var @operator = await reader.ReadFirstOrDefaultAsync<ProductionOperator>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceOperatorIdDoesNotExist));

                var group = groups.FirstOrDefault(x => x.Id.IsMatch(req.Body.GroupId))
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceGroupIdDoesNotExist));

                var member = members.FirstOrDefault(x => x.Id.IsMatch(req.Body.MemberId));
                if (member.Id == default)
                {
                    var memberChilds = ProductionOperator.GetMemberChildDeleter(connection, req.Body.MemberId);
                    await foreach (var memberChild in memberChilds.ConfigureAwait(false).WithCancellation(ct))
                    {
                        await connection.ExecuteAsync(memberChild, transaction).ConfigureAwait(false);
                    }
                }
                else await connection.ExecuteAsync(DurableSetup.DelimitUpdate(@operator.Id, new ProductionOperator
                {
                    MemberId = member.Id,
                    GroupId = group.Id,
                    Modifier = GetUserName(),
                    Creator = @operator.Creator,
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
                TableLayout.LetSelect<ProductionOperator>(req.Body.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}