namespace Eywa.Domain.ProductionControl.Endpoints.ProductInformations.ManufactureOrders;
internal sealed class GetManufactureOrderInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<GetManufactureOrderInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ManufactureOrderIdIsRequired));
        }
    }
}