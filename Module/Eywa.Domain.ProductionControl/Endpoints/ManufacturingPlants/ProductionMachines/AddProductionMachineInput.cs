namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionMachines;
internal sealed class AddProductionMachineInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string EquipmentId { get; init; }
        public required string ProcessId { get; init; }
        public sealed class Validator : AbstractValidator<AddProductionMachineInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.EquipmentId).NotEmpty().WithMessage(culture.Link(FacilityManagementFlag.IndustrialEquipmentIdIsRequired));
                RuleFor(x => x.Body.ProcessId).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceProcessIdIsRequired));
            }
        }
    }
}