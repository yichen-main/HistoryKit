namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.FieldMembers;
internal sealed class DelFieldMemberInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<DelFieldMemberInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(HumanResourcesFlag.HumanPositionIdIsRequired));
        }
    }
}