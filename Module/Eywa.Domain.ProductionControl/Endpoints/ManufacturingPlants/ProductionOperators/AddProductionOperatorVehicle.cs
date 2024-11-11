namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionOperators;
internal sealed class AddProductionOperatorVehicle() : NodeEnlarge<AddProductionOperatorVehicle, AddProductionOperatorInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddProductionOperatorInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.TransactionAsync(async (connection, transaction, reader) =>
        {
            try
            {
                var userName = GetUserName();
                var members = await CacheMediator.ListFieldMemberAsync(ct).ConfigureAwait(false);
                var groups = await reader!.ReadAsync<ProductionGroup>().ConfigureAwait(false);

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
                else await connection.ExecuteAsync(DurableSetup.DelimitInsert(new ProductionOperator
                {
                    Id = id,
                    MemberId = member.Id,
                    GroupId = group.Id,
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
                TableLayout.LetSelect<ProductionOperator>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetProductionOperatorVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}