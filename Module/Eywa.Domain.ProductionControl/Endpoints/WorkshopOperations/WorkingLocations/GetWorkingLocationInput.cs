namespace Eywa.Domain.ProductionControl.Endpoints.WorkshopOperations.WorkingLocations;
internal sealed class GetWorkingLocationInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<GetWorkingLocationInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.WorkingLocationIdIsRequired));
        }
    }
}