namespace Eywa.Serve.ManagementInformation.Endpoints.SystemAccounts;
internal sealed class AddSystemAccountInput : NodeHeader
{
    public required string Password { get; init; }
    public sealed class Validator : AbstractValidator<AddSystemAccountInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierWrongPassword));
        }
    }
}