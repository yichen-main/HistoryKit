namespace Eywa.Domain.ProductionControl.Endpoints.ProductInformations.ManufactureOrders;
internal sealed class DelManufactureOrderInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<DelManufactureOrderInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ManufactureOrderIdIsRequired));
        }
    }
}