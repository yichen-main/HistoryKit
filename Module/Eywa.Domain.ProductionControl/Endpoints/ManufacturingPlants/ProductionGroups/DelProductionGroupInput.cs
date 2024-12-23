﻿namespace Eywa.Domain.ProductionControl.Endpoints.ManufacturingPlants.ProductionGroups;
internal sealed class DelProductionGroupInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<DelProductionGroupInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(ProductionControlFlag.ProduceGroupIdIsRequired));
        }
    }
}