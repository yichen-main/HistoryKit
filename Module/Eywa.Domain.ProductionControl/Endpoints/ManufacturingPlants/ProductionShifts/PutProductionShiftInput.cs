namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionShifts;
internal sealed class PutProductionShiftInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string Id { get; init; }
        public required string ShiftNo { get; init; }
        public required string ShiftName { get; init; }
        public required string StartTimePoint { get; init; }
        public required string EndTimePoint { get; init; }
        public sealed class Validator : AbstractValidator<PutProductionShiftInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceShiftIdIsRequired));
                RuleFor(x => x.Body.ShiftNo).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceShiftNoIsRequired));
                RuleFor(x => x.Body.ShiftName).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceShiftNameIsRequired));
                RuleFor(x => x.Body.StartTimePoint)
                  .Must(x => TimeOnly.TryParse(x, CultureInfo.InvariantCulture, out _))
                  .WithMessage(culture.Link(ProductionControlFlag.ProduceInvalidStartTimePointFormat));
                RuleFor(x => x.Body.EndTimePoint)
                  .Must(x => TimeOnly.TryParse(x, CultureInfo.InvariantCulture, out _))
                  .WithMessage(culture.Link(ProductionControlFlag.ProduceInvalidEndTimePointFormat));
            }
        }
    }
}