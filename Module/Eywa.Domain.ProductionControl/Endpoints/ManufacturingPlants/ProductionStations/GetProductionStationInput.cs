namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionStations;
internal sealed class GetProductionStationInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<GetProductionStationInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceStationIdIsRequired));
        }
    }
}