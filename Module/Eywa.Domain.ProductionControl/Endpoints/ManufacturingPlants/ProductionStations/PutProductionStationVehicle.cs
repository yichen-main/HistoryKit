namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionStations;
internal sealed class PutProductionStationVehicle() : NodeEnlarge<PutProductionStationVehicle, PutProductionStationInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutProductionStationInput req, CancellationToken ct)
    {
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var station = await reader!.ReadFirstOrDefaultAsync<ProductionStation>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceStationIdDoesNotExist));

                if (station is not null) await connection.ExecuteAsync(DurableSetup.DelimitUpdate(station.Id, new ProductionStation
                {
                    StationNo = station.StationNo,
                    StationName = station.StationName,
                    Modifier = GetUserName(),
                    Creator = station.Creator,
                    ModifyTime = DateTime.UtcNow,
                }, ct)).ConfigureAwait(false);
            }
            catch (NpgsqlException e)
            {
                e.MakeException([
                    (ProductionStation.StationNoIndex, DurableSetup.Link(ProductionControlFlag.ProduceStationNoIndex)),
                ]);
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionStation>(req.Body.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}