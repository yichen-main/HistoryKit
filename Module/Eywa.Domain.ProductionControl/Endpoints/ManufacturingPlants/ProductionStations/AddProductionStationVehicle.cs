namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionStations;
internal sealed class AddProductionStationVehicle() : NodeEnlarge<AddProductionStationVehicle, AddProductionStationInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddProductionStationInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new ProductionStation
                {
                    Id = id,
                    StationNo = req.Body.StationNo,
                    StationName = req.Body.StationName,
                    Modifier = userName,
                    Creator = userName,
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
            QueryFields = [],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetProductionStationVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}