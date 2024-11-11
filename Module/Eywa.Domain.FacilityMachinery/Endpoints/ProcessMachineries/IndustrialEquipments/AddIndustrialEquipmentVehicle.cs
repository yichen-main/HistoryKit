namespace Eywa.Domain.FacilityMachinery.Endpoints.ProcessMachineries.IndustrialEquipments;
internal sealed class AddIndustrialEquipmentVehicle() : NodeEnlarge<AddIndustrialEquipmentVehicle, AddIndustrialEquipmentInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddIndustrialEquipmentInput req, CancellationToken ct)
    {
        var id = FileLayout.GetSnowflakeId();
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.FacilityManagement, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var userName = GetUserName();
                await connection.ExecuteAsync(DurableSetup.DelimitInsert(new IndustrialEquipment
                {
                    Id = id,
                    EquipmentNo = req.Body.EquipmentNo,
                    EquipmentName = req.Body.EquipmentName,
                    Modifier = userName,
                    Creator = userName,
                    ModifyTime = DateTime.UtcNow,
                }, ct)).ConfigureAwait(false);
            }
            catch (NpgsqlException e)
            {
                e.MakeException([
                    (IndustrialEquipment.EquipmentNoIndex, DurableSetup.Link(FacilityManagementFlag.IndustrialEquipmentNoIndex)),
                ]);
            }
        }, new()
        {
            QueryFields = [],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendCreatedAtAsync<GetIndustrialEquipmentVehicle>(new
        {
            id,
        }, default, cancellation: ct).ConfigureAwait(false);
    }
}