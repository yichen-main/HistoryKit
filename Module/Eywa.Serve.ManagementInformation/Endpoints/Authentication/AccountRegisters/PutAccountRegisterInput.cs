namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountRegisters;
internal sealed class PutAccountRegisterInput : NodeHeader
{
    [FromBody] public required RawBody Body { get; init; }
    public sealed class RawBody
    {
        public required string Id { get; init; }
        public required string Email { get; init; }
        public required string UserNo { get; init; }
        public required string UserName { get; init; }
        public required bool Disable { get; init; }
        public sealed class Validator : AbstractValidator<PutAccountRegisterInput>
        {
            public Validator(IDurableSetup culture)
            {
                RuleFor(x => x.Body.Id).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierUserIdIsRequired));
                RuleFor(x => x.Body.Email).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierEmailIsRequired))
                    .EmailAddress().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierEmailFormatError));
                RuleFor(x => x.Body.UserNo).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierUserNoIsRequired));
                RuleFor(x => x.Body.UserName).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierUserNameIsRequired));
            }
        }
    }
}