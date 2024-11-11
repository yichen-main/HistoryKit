namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountSignIn;
internal sealed class AddAccountSignInInput : NodeHeader
{
    public required string Account { get; init; }
    public required string Password { get; init; }
    public sealed class Validator : AbstractValidator<AddAccountSignInInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Account).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierAccountIsRequired));
            RuleFor(x => x.Password).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierPasswordIsRequired));
        }
    }
}