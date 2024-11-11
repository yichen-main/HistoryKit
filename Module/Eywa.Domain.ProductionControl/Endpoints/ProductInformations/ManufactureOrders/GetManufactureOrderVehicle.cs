namespace Eywa.Domain.ProductionControl.Endpoints.ProductInformations.ManufactureOrders;
internal sealed class GetManufactureOrderVehicle : NodeEnlarge<GetManufactureOrderVehicle, GetManufactureOrderInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetManufactureOrderInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var order = await reader!.ReadSingleAsync<ManufactureOrder>().ConfigureAwait(false);
            output = new()
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                OrderName = order.OrderName,
                Creator = order.Creator,
                CreateTime = DurableSetup.LocalTime(order.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = order.Modifier,
                ModifyTime = DurableSetup.LocalTime(order.ModifyTime, req.TimeZone, req.TimeFormat),
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ManufactureOrder>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
        public required string OrderNo { get; init; }
        public required string OrderName { get; init; }
        public required string Creator { get; init; }
        public required string CreateTime { get; init; }
        public required string Modifier { get; init; }
        public required string ModifyTime { get; init; }
    }
}