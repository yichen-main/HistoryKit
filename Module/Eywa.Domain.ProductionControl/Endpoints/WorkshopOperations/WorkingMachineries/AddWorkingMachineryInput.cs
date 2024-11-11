namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingMachineries;
internal sealed class AddWorkingMachineryInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string StationId { get; init; }
        public required string MachineId { get; init; }
        public sealed class Validator : AbstractValidator<AddWorkingMachineryInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.StationId).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceStationIdIsRequired));
                RuleFor(x => x.Body.MachineId).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceMachineIdIsRequired));
            }
        }
    }
}