﻿namespace Eywa.Domain.ProductionControl.Endpoints.ProductInformations.ManufactureOrders;
internal sealed class PutManufactureOrderInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string Id { get; init; }
        public required string OrderNo { get; init; }
        public required string OrderName { get; init; }
        public sealed class Validator : AbstractValidator<PutManufactureOrderInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ManufactureOrderIdIsRequired));
                RuleFor(x => x.Body.OrderNo).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ManufactureOrderNoIsRequired));
                RuleFor(x => x.Body.OrderName).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ManufactureOrderNameIsRequired));
            }
        }
    }
}