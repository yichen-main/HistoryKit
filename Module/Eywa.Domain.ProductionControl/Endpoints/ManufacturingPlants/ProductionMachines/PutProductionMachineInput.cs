namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionMachines;
internal sealed class PutProductionMachineInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string Id { get; init; }
        public required string EquipmentId { get; init; }
        public required string ProcessId { get; init; }
        public sealed class Validator : AbstractValidator<PutProductionMachineInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceMachineIdIsRequired));
                RuleFor(x => x.Body.EquipmentId).NotEmpty().WithMessage(culture.Link(FacilityManagementFlag.IndustrialEquipmentIdDoesNotExist));
                RuleFor(x => x.Body.ProcessId).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceProcessIdIsRequired));
            }
        }
    }
}