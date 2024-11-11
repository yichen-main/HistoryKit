namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountRegisters;
internal sealed class GetAccountRegisterInput : NodeHeader
{
    public required string Id { get; init; }
    public sealed class Validator : AbstractValidator<GetAccountRegisterInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierUserIdIsRequired));
        }
    }
}