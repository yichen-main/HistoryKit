namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionGroups;
internal sealed class GetProductionGroupVehicle : NodeEnlarge<GetProductionGroupVehicle, GetProductionGroupInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetProductionGroupInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var group = await reader!.ReadSingleAsync<ProductionGroup>().ConfigureAwait(false);
            output = new()
            {
                Id = group.Id,
                GroupNo = group.GroupNo,
                GroupName = group.GroupName,
                Creator = group.Creator,
                CreateTime = DurableSetup.LocalTime(group.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = group.Modifier,
                ModifyTime = DurableSetup.LocalTime(group.ModifyTime, req.TimeZone, req.TimeFormat),
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionGroup>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
        public required string GroupNo { get; init; }
        public required string GroupName { get; init; }
        public required string Creator { get; init; }
        public required string CreateTime { get; init; }
        public required string Modifier { get; init; }
        public required string ModifyTime { get; init; }
    }
}