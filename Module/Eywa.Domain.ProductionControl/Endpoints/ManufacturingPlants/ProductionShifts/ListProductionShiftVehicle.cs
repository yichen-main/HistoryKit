namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionShifts;
internal sealed class ListProductionShiftVehicle : NodeEnlarge<ListProductionShiftVehicle, ListProductionShiftInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListProductionShiftInput req, CancellationToken ct)
    {
        List<GetProductionShiftVehicle.Output> outputs = [];
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var shifts = await reader!.ReadAsync<ProductionShift>().ConfigureAwait(false);
            foreach (var shift in shifts)
            {
                outputs.Add(new()
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
                });
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionShift>(),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetProductionShiftVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}