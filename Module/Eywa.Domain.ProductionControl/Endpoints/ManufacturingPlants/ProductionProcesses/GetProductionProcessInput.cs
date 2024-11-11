namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionProcesses;
internal sealed class GetProductionProcessInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<GetProductionProcessInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceProcessIdIsRequired));
        }
    }
}