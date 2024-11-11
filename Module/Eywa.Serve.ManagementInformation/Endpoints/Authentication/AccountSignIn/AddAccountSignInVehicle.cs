namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.AccountSignIn;
internal sealed class AddAccountSignInVehicle : NodeEnlarge<AddAccountSignInVehicle, AddAccountSignInInput>
{
    public override void Configure()
    {
        AllowAnonymous();
        AllowFormData(urlEncoded: true);
        RequiredConfiguration();
    }
    public override async Task HandleAsync(AddAccountSignInInput req, CancellationToken ct)
    {
        BearerTokenInfo result = default;
        await Validator.LeachAsync(req, ct).ConfigureAwait(false);
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            switch (req.Account)
            {
                case var x when x.IsMatch("reformtek"):
                    var profile = await CiphertextPolicy.GetRootFileAsync(ct).ConfigureAwait(false);
                    if (!profile.Hash.IsMatch(CiphertextPolicy.HmacSHA256ToHex(req.Password, profile.Salt)))
                    {
                        throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierWrongPassword));
                    }
                    result = Authenticator.GetBearerToken(HttpContext, new()
                    {
                        Id = profile.Id,
                        UserName = req.Account,
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
                    if (!user.HashedText.IsMatch(CiphertextPolicy.HmacSHA256ToHex(req.Password, user.Salt)))
                    {
                        throw new Exception(DurableSetup.Link(EnterpriseIntegrationFlag.CarrierSignInFailed));
                    }
                    result = Authenticator.GetBearerToken(HttpContext, new()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Administrator = default,
                        DomainRoles = await domainRolesAsync.ConfigureAwait(false),
                    });
                    await connection.ExecuteAsync(DurableSetup.DelimitInsert(new UserActivityRecord
                    {
                        Id = FileLayout.GetSnowflakeId(),
                        UserId = user.Id,
                        LoginStatus = LoginStatus.SignIn,
                    }, ct)).ConfigureAwait(false);
                    break;
            }
        }, new()
        {
            QueryFields = [
                TableLayout.LetSelect<UserRegistration>(new TableLayout.ColumnFilter(nameof(UserRegistration.Email), req.Account)),
            ],
            CancellationToken = ct,
        }).ConfigureAwait(false);
        await SendAsync(result, cancellation: ct).ConfigureAwait(false);
    }
    public required IUserAuthenticator Authenticator { get; init; }
}