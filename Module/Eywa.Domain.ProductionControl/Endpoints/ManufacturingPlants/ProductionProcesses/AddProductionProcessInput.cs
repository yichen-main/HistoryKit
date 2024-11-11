namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionProcesses;
internal sealed class AddProductionProcessInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string ProcessNo { get; init; }
        public required string ProcessName { get; init; }
        public sealed class Validator : AbstractValidator<AddProductionProcessInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.ProcessNo).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceProcessNoIsRequired));
                RuleFor(x => x.Body.ProcessName).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceProcessNameIsRequired));
            }
        }
    }
}