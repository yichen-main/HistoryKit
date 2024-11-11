namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionShifts;
internal sealed class GetProductionShiftVehicle : NodeEnlarge<GetProductionShiftVehicle, GetProductionShiftInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetProductionShiftInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var shift = await reader!.ReadSingleAsync<ProductionShift>().ConfigureAwait(false);
            output = new()
            {
                Id = shift.Id,
                ShiftNo = shift.ShiftNo,
                ShiftName = shift.ShiftName,
                StartTimePoint = shift.StartTime.ConvertHoursMinutes(),
                EndTimePoint = shift.EndTime.ConvertHoursMinutes(),
                Creator = shift.Creator,
                CreateTime = DurableSetup.LocalTime(shift.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = shift.Modifier,
                ModifyTime = DurableSetup.LocalTime(shift.ModifyTime, req.TimeZone, req.TimeFormat),
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionShift>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
        public required string ShiftNo { get; init; }
        public required string ShiftName { get; init; }
        public required string StartTimePoint { get; init; }
        public required string EndTimePoint { get; init; }
        public required string Creator { get; init; }
        public required string CreateTime { get; init; }
        public required string Modifier { get; init; }
        public required string ModifyTime { get; init; }
    }
}