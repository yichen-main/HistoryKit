namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingLocations;
internal sealed class AddWorkingLocationInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string StationId { get; init; }
        public required string OperatorId { get; init; }
        public sealed class Validator : AbstractValidator<AddWorkingLocationInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.StationId).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceStationIdIsRequired));
                RuleFor(x => x.Body.OperatorId).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceOperatorIdIsRequired));
            }
        }
    }
}