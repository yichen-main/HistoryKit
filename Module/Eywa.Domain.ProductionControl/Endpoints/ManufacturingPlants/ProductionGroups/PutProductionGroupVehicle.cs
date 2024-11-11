namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionGroups;
internal sealed class PutProductionGroupVehicle() : NodeEnlarge<PutProductionGroupVehicle, PutProductionGroupInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutProductionGroupInput req, CancellationToken ct)
    {
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var group = await reader!.ReadFirstOrDefaultAsync<ProductionGroup>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceGroupIdDoesNotExist));

                if (group is not null) await connection.ExecuteAsync(DurableSetup.DelimitUpdate(group.Id, new ProductionGroup
                {
                    GroupNo = group.GroupNo,
                    GroupName = group.GroupName,
                    Modifier = GetUserName(),
                    Creator = group.Creator,
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
            QueryFields = [
                TableLayout.LetSelect<ProductionGroup>(req.Body.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}