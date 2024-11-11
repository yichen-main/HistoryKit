namespace Eywa.Domain.FacilityMachinery.Antisepsis.Companies.WorkingFacilities;
internal sealed class EquipmentQueryHandler : ExecuteHandler<EquipmentQueryImport, IEnumerable<EquipmentQueryOutput>>
{
    protected override void Configure()
    {
        QueryFields = [
            TableLayout.LetSelect<IndustrialEquipment>(),
        ];
    }
    protected override async Task<IEnumerable<EquipmentQueryOutput>> HandleAsync(Options combine)
    {
        List<EquipmentQueryOutput> outputs = [];
        var equipments = await combine.Reader!.ReadAsync<IndustrialEquipment>().ConfigureAwait(false);
        foreach (var equipment in equipments) outputs.Add(new()
        {
            Id = equipment.Id,
            EquipmentNo = equipment.EquipmentNo,
            EquipmentName = equipment.EquipmentName,
        });
        return outputs.AsEnumerable();
    }
}