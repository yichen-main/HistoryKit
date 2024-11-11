namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionStations;
internal sealed class DelProductionStationInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<DelProductionStationInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceProcessIdIsRequired));
        }
    }
}