namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.TokenExtensions;
internal sealed class AddTokenExtensionVehicle : NodeEnlarge<AddTokenExtensionVehicle, AddTokenExtensionInput>
{
    public override void Configure()
    {
        AllowAnonymous();
        AllowFormData(urlEncoded: true);
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddTokenExtensionInput req, CancellationToken ct)
    {
        BearerTokenInfo result = default;
        var profileAsync = CiphertextPolicy.GetRootFileAsync(ct);
        Authenticator.VerifyRefreshToken(HttpContext, req.RefreshToken);
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            var profile = await profileAsync.ConfigureAwait(false);
            switch (req.NameIdentifier)
            {
                case var x when x.IsMatch(profile.Id):
                    result = Authenticator.GetBearerToken(HttpContext, new()
                    {
                        Id = profile.Id,
                        UserName = "reformtek",
                        Administrator = true,
                        DomainRoles = [],
                    });
                    break;

                default:
                    var user = await reader!.ReadFirstOrDefaultAsync<UserRegistration>().ConfigureAwait(false)
                    ?? throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierAccountDoesNotExist));

                    var domainRolesAsync = Authenticator.GetDomainRolesAsync(user.Id, ct);
                    if (user is { Disable: true })
                    {
                        throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierAccountInvalid));
                    }
                    result = Authenticator.GetBearerToken(HttpContext, new()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Administrator = default,
                        DomainRoles = await domainRolesAsync.ConfigureAwait(false),
                    });
                    break;
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<UserRegistration>(req.NameIdentifier),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(result, cancellation: ct).ConfigureAwait(false);
    }
    public required IUserAuthenticator Authenticator { get; init; }
}