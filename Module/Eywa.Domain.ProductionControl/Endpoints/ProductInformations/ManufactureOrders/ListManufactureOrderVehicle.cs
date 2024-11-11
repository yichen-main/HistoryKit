namespace Eywa.Domain.ProductionControl.Endpoints.ProductInformations.ManufactureOrders;
internal sealed class ListManufactureOrderVehicle : NodeEnlarge<ListManufactureOrderVehicle, ListManufactureOrderInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListManufactureOrderInput req, CancellationToken ct)
    {
        List<GetManufactureOrderVehicle.Output> outputs = [];
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var orders = await reader!.ReadAsync<ManufactureOrder>().ConfigureAwait(false);
            foreach (var order in orders) outputs.Add(new()
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                OrderName = order.OrderName,
                Creator = order.Creator,
                CreateTime = DurableSetup.LocalTime(order.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = order.Modifier,
                ModifyTime = DurableSetup.LocalTime(order.ModifyTime, req.TimeZone, req.TimeFormat),
            });
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ManufactureOrder>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetManufactureOrderVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}