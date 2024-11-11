namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionGroups;
internal sealed class PutProductionGroupInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string Id { get; init; }
        public required string GroupNo { get; init; }
        public required string GroupName { get; init; }
        public sealed class Validator : AbstractValidator<PutProductionGroupInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceGroupIdIsRequired));
                RuleFor(x => x.Body.GroupNo).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceGroupNoIsRequired));
                RuleFor(x => x.Body.GroupName).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceGroupNameIsRequired));
            }
        }
    }
}