namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionShifts;
internal sealed class PutProductionShiftVehicle() : NodeEnlarge<PutProductionShiftVehicle, PutProductionShiftInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutProductionShiftInput req, CancellationToken ct)
    {
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var startTime = TimeSpan.Parse(req.Body.StartTimePoint, CultureInfo.InvariantCulture);
                var endTime = TimeSpan.Parse(req.Body.EndTimePoint, CultureInfo.InvariantCulture);
                if (startTime >= endTime) throw new Exception(DurableSetup.Link(AccountAccessFlag.StartTimeMismatchEndTime));

                var shift = await reader!.ReadFirstOrDefaultAsync<ProductionShift>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(ProductionControlFlag.ProduceShiftIdDoesNotExist));

                await connection.ExecuteAsync(DurableSetup.DelimitUpdate(shift.Id, new ProductionShift
                {
                    ShiftNo = req.Body.ShiftNo,
                    ShiftName = req.Body.ShiftName,
                    StartTime = startTime,
                    EndTime = endTime,
                    Modifier = GetUserName(),
                    Creator = shift.Creator,
                    ModifyTime = DateTime.UtcNow,
                }, ct)).ConfigureAwait(false);
            }
            catch (NpgsqlException e)
            {
                e.MakeException([
                    (ProductionShift.ShiftNoIndex, DurableSetup.Link(ProductionControlFlag.ProduceShiftNoIndex)),
                ]);
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<ProductionShift>(req.Body.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}