namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionGroups;
internal sealed class AddProductionGroupVehicle() : NodeEnlarge<AddProductionGroupVehicle, AddProductionGroupInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddProductionGroupInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new ProductionGroup
                {
                    Id = id,
                    GroupNo = req.Body.GroupNo,
                    GroupName = req.Body.GroupName,
                    Modifier = userName,
                    Creator = userName,
                    ModifyTime = DateTime.UtcNow,
                }, ct)).ConfigureAwait(false);
            }
            catch (NpgsqlException e)
            {
                e.MakeException([
                    (ProductionGroup.GroupNoIndex, DurableSetup.Link(ProductionControlFlag.ProduceGroupNoIndex)),
                ]);
            }
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetProductionGroupVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}