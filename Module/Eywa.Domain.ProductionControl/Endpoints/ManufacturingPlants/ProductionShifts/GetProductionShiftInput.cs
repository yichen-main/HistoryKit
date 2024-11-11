namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionShifts;
internal sealed class GetProductionShiftInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<GetProductionShiftInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceShiftIdIsRequired));
        }
    }
}