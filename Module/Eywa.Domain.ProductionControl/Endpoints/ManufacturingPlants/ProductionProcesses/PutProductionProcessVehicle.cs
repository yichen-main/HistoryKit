namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionProcesses;
internal sealed class PutProductionProcessVehicle() : NodeEnlarge<PutProductionProcessVehicle, PutProductionProcessInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutProductionProcessInput req, CancellationToken ct)
    {
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var process = await reader!.ReadFirstOrDefaultAsync<ProductionProcess>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceProcessIdDoesNotExist));

                if (process is not null) await connection.ExecuteAsync(DurableSetup.DelimitUpdate(process.Id, new ProductionProcess
                {
                    ProcessNo = process.ProcessNo,
                    ProcessName = process.ProcessName,
                    Modifier = GetUserName(),
                    Creator = process.Creator,
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
            QueryFields = [
                TableLayout.LetSelect<ProductionProcess>(req.Body.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}