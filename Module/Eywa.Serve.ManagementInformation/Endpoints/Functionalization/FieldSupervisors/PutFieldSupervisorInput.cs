namespace Eywa.Serve.ManagementInformation.Endpoints.Functionalization.FieldSupervisors;
internal sealed class PutFieldSupervisorInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string Id { get; init; }
        public required string UserId { get; init; }
        public required bool Disable { get; init; }
        public required FieldModule FieldType { get; init; }
        public sealed class Validator : AbstractValidator<PutFieldSupervisorInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.Id).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierModuleIdIsRequired));
                RuleFor(x => x.Body.UserId).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierUserIdIsRequired));
                RuleFor(x => x.Body.FieldType).Must(type => BeforeExpand.GetFieldModules().Any(x => x == type))
                 .WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierFieldTypeMismatch));
            }
        }
    }
}