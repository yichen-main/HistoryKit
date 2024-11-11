namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.FieldMembers;
internal sealed class PutFieldMemberInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string Id { get; init; }
        public required string UserId { get; init; }
        public required bool Disable { get; init; }
        public RolePolicy AccessType { get; init; }
        public sealed class Validator : AbstractValidator<PutFieldMemberInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.Id).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierModuleIdIsRequired));
                RuleFor(x => x.Body.UserId).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierUserIdIsRequired));
                RuleFor(x => x.Body.AccessType)
                    .Must(x => Enum.IsDefined(typeof(RolePolicy), x))
                    .WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierRolePolicyMismatch))
                    .Must(x => x is RolePolicy.Editor || x is RolePolicy.Viewer)
                    .WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierAccessTypeMismatch));
            }
        }
    }
}