namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionStations;
internal sealed class GetProductionStationVehicle : NodeEnlarge<GetProductionStationVehicle, GetProductionStationInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetProductionStationInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var station = await reader!.ReadSingleAsync<ProductionStation>().ConfigureAwait(false);
            output = new()
            {
                Id = station.Id,
                StationNo = station.StationNo,
                StationName = station.StationName,
                Creator = station.Creator,
                CreateTime = DurableSetup.LocalTime(station.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = station.Modifier,
                ModifyTime = DurableSetup.LocalTime(station.ModifyTime, req.TimeZone, req.TimeFormat),
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionStation>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
        public required string StationNo { get; init; }
        public required string StationName { get; init; }
        public required string Creator { get; init; }
        public required string CreateTime { get; init; }
        public required string Modifier { get; init; }
        public required string ModifyTime { get; init; }
    }
}