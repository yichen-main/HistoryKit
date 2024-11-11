namespace Eywa.Domain.FacilityMachinery.Endpoints.ProcessMachineries.IndustrialEquipments;
internal sealed class ListIndustrialEquipmentVehicle : NodeEnlarge<ListIndustrialEquipmentVehicle, ListIndustrialEquipmentInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(ListIndustrialEquipmentInput req, CancellationToken ct)
    {
        List<GetIndustrialEquipmentVehicle.Output> outputs = [];
        await VerifyAsync(FieldModule.FacilityManagement, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var equipments = await reader!.ReadAsync<IndustrialEquipment>().ConfigureAwait(false);
            foreach (var equipment in equipments)
            {
                outputs.Add(new()
                {
                    Id = equipment.Id,
                    EquipmentNo = equipment.EquipmentNo,
                    EquipmentName = equipment.EquipmentName,
                    Creator = equipment.Creator,
                    CreateTime = DurableSetup.LocalTime(equipment.CreateTime, req.TimeZone, req.TimeFormat),
                    Modifier = equipment.Modifier,
                    ModifyTime = DurableSetup.LocalTime(equipment.ModifyTime, req.TimeZone, req.TimeFormat),
                });
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetInterval<IndustrialEquipment>(req.TimeInterval),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        Pagination(new PageContents<GetIndustrialEquipmentVehicle.Output>(outputs, req.PageIndex, req.PageSize), [
            (nameof(req.Search), req.Search),
        ]);
        await SendAsync(outputs, cancellation: ct).ConfigureAwait(false);
    }
}