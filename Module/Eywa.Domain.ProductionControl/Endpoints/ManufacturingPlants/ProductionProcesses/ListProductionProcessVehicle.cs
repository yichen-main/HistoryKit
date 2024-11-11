namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionProcesses;
internal sealed class ListProductionProcessVehicle : NodeEnlarge<ListProductionProcessVehicle, ListProductionProcessInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListProductionProcessInput req, CancellationToken ct)
    {
        List<GetProductionProcessVehicle.Output> outputs = [];
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var processes = await reader!.ReadAsync<ProductionProcess>().ConfigureAwait(false);
            foreach (var process in processes) outputs.Add(new()
            {
                Id = process.Id,
                ProcessNo = process.ProcessNo,
                ProcessName = process.ProcessName,
                Creator = process.Creator,
                CreateTime = DurableSetup.LocalTime(process.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = process.Modifier,
                ModifyTime = DurableSetup.LocalTime(process.ModifyTime, req.TimeZone, req.TimeFormat),
            });
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionProcess>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetProductionProcessVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}