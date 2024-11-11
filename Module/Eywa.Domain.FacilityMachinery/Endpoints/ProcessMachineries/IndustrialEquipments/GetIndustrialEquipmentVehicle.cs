namespace Eywa.Domain.FacilityMachinery.Endpoints.ProcessMachineries.IndustrialEquipments;
internal sealed class GetIndustrialEquipmentVehicle : NodeEnlarge<GetIndustrialEquipmentVehicle, GetIndustrialEquipmentInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(GetIndustrialEquipmentInput req, CancellationToken ct)
    {
        Output output = default;
        await VerifyAsync(FieldModule.FacilityManagement, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var equipment = await reader!.ReadSingleAsync<IndustrialEquipment>().ConfigureAwait(false);
            output = new()
            {
                Id = equipment.Id,
                EquipmentNo = equipment.EquipmentNo,
                EquipmentName = equipment.EquipmentName,
                Creator = equipment.Creator,
                CreateTime = DurableSetup.LocalTime(equipment.CreateTime, req.TimeZone, req.TimeFormat),
                Modifier = equipment.Modifier,
                ModifyTime = DurableSetup.LocalTime(equipment.ModifyTime, req.TimeZone, req.TimeFormat),
            };
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<IndustrialEquipment>(req.Id),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(output, cancellation: ct).ConfigureAwait(false);
    }
    public readonly record struct Output
    {
        public required string Id { get; init; }
        public required string EquipmentNo { get; init; }
        public required string EquipmentName { get; init; }
        public required string Creator { get; init; }
        public required string CreateTime { get; init; }
        public required string Modifier { get; init; }
        public required string ModifyTime { get; init; }
    }
}