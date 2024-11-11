namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.TokenExtensions;
internal sealed class AddTokenExtensionInput : NodeHeader
{
    public required string NameIdentifier { get; init; }
    public required string RefreshToken { get; init; }
    public sealed class Validator : AbstractValidator<AddTokenExtensionInput>
    {
        public Validator(IDurableSetup culture)
        {
            RuleFor(x => x.NameIdentifier).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierNameIdentifierIsRequired));
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage(culture.Link(EnterpriseIntegrationFlag.CarrierRefreshTokenIsRequired));
        }
    }
}