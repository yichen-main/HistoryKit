namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionProcesses;
internal sealed class AddProductionProcessVehicle() : NodeEnlarge<AddProductionProcessVehicle, AddProductionProcessInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddProductionProcessInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new ProductionProcess
                {
                    Id = id,
                    ProcessNo = req.Body.ProcessNo,
                    ProcessName = req.Body.ProcessName,
                    Modifier = userName,
                    Creator = userName,
                    ModifyTime = DateTime.UtcNow,
                }, ct)).ConfigureAwait(false);
            }
            catch (NpgsqlException e)
            {
                e.MakeException([
                    (ProductionProcess.ProcessNoIndex, DurableSetup.Link(ProductionControlFlag.ProduceProcessNoIndex)),
                ]);
            }
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetProductionProcessVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}