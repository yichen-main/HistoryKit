namespace Eywa.Domain.FacilityMachinery.Endpoints.ProcessMachineries.IndustrialEquipments;
internal sealed class PutIndustrialEquipmentVehicle() : NodeEnlarge<PutIndustrialEquipmentVehicle, PutIndustrialEquipmentInput>(RolePolicy.Editor)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(PutIndustrialEquipmentInput req, CancellationToken ct)
    {
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await VerifyAsync(FieldModule.FacilityManagement, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            try
            {
                var equipment = await reader!.ReadFirstOrDefaultAsync<IndustrialEquipment>().ConfigureAwait(false)
                ?? throw new Exception(DurableSetup.Link(FacilityManagementFlag.IndustrialEquipmentIdDoesNotExist));

                if (equipment is not null) await connection.ExecuteAsync(DurableSetup.DelimitUpdate(equipment.Id, new IndustrialEquipment
                {
                    EquipmentNo = equipment.EquipmentNo,
                    EquipmentName = equipment.EquipmentName,
                    Modifier = GetUserName(),
                    Creator = equipment.Creator,
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
            QueryFields = [
                TableLayout.LetSelect<IndustrialEquipment>(req.Body.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}