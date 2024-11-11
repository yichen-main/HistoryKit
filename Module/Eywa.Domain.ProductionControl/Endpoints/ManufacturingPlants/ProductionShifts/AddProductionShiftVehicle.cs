namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionShifts;
internal sealed class AddProductionShiftVehicle() : NodeEnlarge<AddProductionShiftVehicle, AddProductionShiftInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddProductionShiftInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.ProductionControl, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                var startTime = TimeSpan.Parse(req.Body.StartTimePoint, CultureInfo.InvariantCulture);
                var endTime = TimeSpan.Parse(req.Body.EndTimePoint, CultureInfo.InvariantCulture);
                if (startTime >= endTime) throw new Exception(DurableSetup.Link(AccountAccessFlag.StartTimeMismatchEndTime));
                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new ProductionShift
                {
                    Id = id,
                    ShiftNo = req.Body.ShiftNo,
                    ShiftName = req.Body.ShiftName,
                    StartTime = startTime,
                    EndTime = endTime,
                    Modifier = userName,
                    Creator = userName,
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
            QueryFields = [],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetProductionShiftVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}