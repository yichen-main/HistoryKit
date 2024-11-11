namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionStations;
internal sealed class ListProductionStationVehicle : NodeEnlarge<ListProductionStationVehicle, ListProductionStationInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListProductionStationInput req, CancellationToken ct)
    {
        List<GetProductionStationVehicle.Output> outputs = [];
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var stations = await reader!.ReadAsync<ProductionStation>().ConfigureAwait(false);
            foreach (var station in stations)
            {
                outputs.Add(new()
                {
                    Id = station.Id,
                    StationNo = station.StationNo,
                    StationName = station.StationName,
                    Creator = station.Creator,
                    CreateTime = DurableSetup.LocalTime(station.CreateTime, req.TimeZone, req.TimeFormat),
                    Modifier = station.Modifier,
                    ModifyTime = DurableSetup.LocalTime(station.ModifyTime, req.TimeZone, req.TimeFormat),
                });
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionStation>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetProductionStationVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}