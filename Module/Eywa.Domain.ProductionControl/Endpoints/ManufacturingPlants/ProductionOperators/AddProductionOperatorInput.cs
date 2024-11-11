namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionOperators;
internal sealed class AddProductionOperatorInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string GroupId { get; init; }
        public required string MemberId { get; init; }
        public sealed class Validator : AbstractValidator<AddProductionOperatorInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.GroupId).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceGroupIdIsRequired));
                RuleFor(x => x.Body.MemberId).NotEmpty().WithMessage(culture.Link(HumanResourcesFlag.HumanMemberIdIsRequired));
            }
        }
    }
}