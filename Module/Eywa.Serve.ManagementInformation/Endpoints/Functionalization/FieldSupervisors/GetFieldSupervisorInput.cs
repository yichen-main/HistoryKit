namespace Eywa.Serve.ManagementInformation.Endpoints.Functionalization.FieldSupervisors;
internal sealed class GetFieldSupervisorInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<GetFieldSupervisorInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierModuleIdIsRequired));
        }
    }
}