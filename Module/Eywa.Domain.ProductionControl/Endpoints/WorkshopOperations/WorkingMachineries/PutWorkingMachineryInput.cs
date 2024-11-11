namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingMachineries;
internal sealed class PutWorkingMachineryInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string Id { get; init; }
        public required string StationId { get; init; }
        public required string MachineId { get; init; }
        public sealed class Validator : AbstractValidator<PutWorkingMachineryInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.WorkingMachineryIdIsRequired));
                RuleFor(x => x.Body.StationId).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceStationIdIsRequired));
                RuleFor(x => x.Body.MachineId).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceMachineIdIsRequired));
            }
        }
    }
}