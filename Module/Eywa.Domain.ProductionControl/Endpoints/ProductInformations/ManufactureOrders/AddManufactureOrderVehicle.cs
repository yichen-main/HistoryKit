namespace Eywa.Domain.ProductionControl.Endpoints.ProductInformations.ManufactureOrders;
internal sealed class AddManufactureOrderVehicle() : NodeEnlarge<AddManufactureOrderVehicle, AddManufactureOrderInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddManufactureOrderInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new ManufactureOrder
                {
                    Id = id,
                    OrderNo = req.Body.OrderNo,
                    OrderName = req.Body.OrderName,
                    Modifier = userName,
                    Creator = userName,
                    ModifyTime = DateTime.UtcNow,
                }, ct)).ConfigureAwait(false);
            }
            catch (NpgsqlException e)
            {
                e.MakeException([
                    (ManufactureOrder.OrderNoIndex, DurableSetup.Link(ProductionControlFlag.ManufactureOrderNoIndex)),
                ]);
            }
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetManufactureOrderVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}