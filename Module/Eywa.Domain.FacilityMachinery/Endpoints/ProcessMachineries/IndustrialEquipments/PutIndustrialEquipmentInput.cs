namespace Eywa.Domain.FacilityMachinery.Endpoints.ProcessMachineries.IndustrialEquipments;
internal sealed class PutIndustrialEquipmentInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string Id { get; init; }
        public required string EquipmentNo { get; init; }
        public required string EquipmentName { get; init; }
        public sealed class Validator : AbstractValidator<PutIndustrialEquipmentInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.Id).NotEmpty().WithMessage(culture.Link(FacilityManagementFlag.IndustrialEquipmentIdIsRequired));
                RuleFor(x => x.Body.EquipmentNo).NotEmpty().WithMessage(culture.Link(FacilityManagementFlag.IndustrialEquipmentNoIsRequired));
                RuleFor(x => x.Body.EquipmentName).NotEmpty().WithMessage(culture.Link(FacilityManagementFlag.IndustrialEquipmentNameIsRequired));
            }
        }
    }
}