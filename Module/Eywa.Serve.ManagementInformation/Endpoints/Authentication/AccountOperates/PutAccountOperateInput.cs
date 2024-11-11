namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountOperates;
internal sealed class PutAccountOperateInput : NodeHeader
{
    public required string Password { get; init; }
    public sealed class Validator : AbstractValidator<PutAccountOperateInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Password).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierWrongPassword));
        }
    }
}