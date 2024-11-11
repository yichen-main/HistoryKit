namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountRegisters;
internal sealed class AddAccountRegisterInput : NodeHeader
{
    public required bool Disable { get; init; }
    public required string Email { get; init; }
    public required string UserNo { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public sealed class Validator : AbstractValidator<AddAccountRegisterInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierEmailIsRequired))
                .EmailAddress().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierEmailFormatError));
            RuleFor(x => x.UserNo).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierUserNoIsRequired));
            RuleFor(x => x.UserName).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierUserNameIsRequired));
            RuleFor(x => x.Password).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierPasswordIsRequired));
        }
    }
}