namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionGroups;
internal sealed class ListProductionGroupVehicle : NodeEnlarge<ListProductionGroupVehicle, ListProductionGroupInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListProductionGroupInput req, CancellationToken ct)
    {
        List<GetProductionGroupVehicle.Output> outputs = [];
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var groups = await reader!.ReadAsync<ProductionGroup>().ConfigureAwait(false);
            foreach (var group in groups)
            {
                outputs.Add(new()
                {
                    Id = group.Id,
                    GroupNo = group.GroupNo,
                    GroupName = group.GroupName,
                    Creator = group.Creator,
                    CreateTime = DurableSetup.LocalTime(group.CreateTime, req.TimeZone, req.TimeFormat),
                    Modifier = group.Modifier,
                    ModifyTime = DurableSetup.LocalTime(group.ModifyTime, req.TimeZone, req.TimeFormat),
                });
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionGroup>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetProductionGroupVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}