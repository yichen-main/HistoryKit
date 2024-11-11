namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionProcesses;
internal sealed class GetProductionProcessVehicle : NodeEnlarge<GetProductionProcessVehicle, GetProductionProcessInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetProductionProcessInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var process = await reader!.ReadSingleAsync<ProductionProcess>().ConfigureAwait(false);
            output = new()
            {
                Id = process.Id,
                ProcessNo = process.ProcessNo,
                ProcessName = process.ProcessName,
                Creator = process.Creator,
                CreateTime = DurableSetup.LocalTime(process.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = process.Modifier,
                ModifyTime = DurableSetup.LocalTime(process.ModifyTime, req.TimeZone, req.TimeFormat),
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionProcess>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
        public required string ProcessNo { get; init; }
        public required string ProcessName { get; init; }
        public required string Creator { get; init; }
        public required string CreateTime { get; init; }
        public required string Modifier { get; init; }
        public required string ModifyTime { get; init; }
    }
}