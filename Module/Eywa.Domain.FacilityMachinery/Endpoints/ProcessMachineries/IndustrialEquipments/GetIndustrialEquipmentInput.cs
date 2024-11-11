namespace Eywa.Domain.FacilityMachinery.Endpoints.ProcessMachineries.IndustrialEquipments;
internal sealed class GetIndustrialEquipmentInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<GetIndustrialEquipmentInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(FacilityManagementFlag.IndustrialEquipmentIdIsRequired));
        }
    }
}