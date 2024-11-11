namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionStations;
internal sealed class PutProductionStationInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string Id { get; init; }
        public required string StationNo { get; init; }
        public required string StationName { get; init; }
        public sealed class Validator : AbstractValidator<PutProductionStationInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceStationIdIsRequired));
                RuleFor(x => x.Body.StationNo).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceStationNoIsRequired));
                RuleFor(x => x.Body.StationName).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceStationNameIsRequired));
            }
        }
    }
}