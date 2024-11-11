namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.CompanyPositions;
internal sealed class GetCompanyPositionInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<GetCompanyPositionInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(HumanResourcesFlag.HumanPositionIdIsRequired));
        }
    }
}