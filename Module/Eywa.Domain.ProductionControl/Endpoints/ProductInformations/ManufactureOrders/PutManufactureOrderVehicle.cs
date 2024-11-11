namespace Eywa.Domain.ProductionControl.Endpoints.ProductInformations.ManufactureOrders;
internal sealed class PutManufactureOrderVehicle() : NodeEnlarge<PutManufactureOrderVehicle, PutManufactureOrderInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutManufactureOrderInput req, CancellationToken ct)
    {
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var order = await reader!.ReadFirstOrDefaultAsync<ManufactureOrder>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ManufactureOrderIdDoesNotExist));

                await connection.ExecuteAsync(DurableSetup.DelimitUpdate(order.Id, new ManufactureOrder
                {
                    OrderNo = order.OrderNo,
                    OrderName = order.OrderName,
                    Modifier = GetUserName(),
                    Creator = order.Creator,
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
            QueryFields = [
                TableLayout.LetSelect<ManufactureOrder>(req.Body.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}