namespace Eywa.Serve.Loader.Constructs.Preliminaries.Quarterlies;
public interface IUserAuthenticator
{
    void VerifyRefreshToken(in HttpContext context, in string refreshToken);
    BearerTokenInfo GetBearerToken(HttpContext context, ClaimParameter parameter);
    ValueTask<IEnumerable<ClaimParameter.DomainRole>> GetDomainRolesAsync(string userId, CancellationToken ct);
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class UserAuthenticator : IUserAuthenticator
{
    const string _refreshCookieName = "refresh_token";
    public void VerifyRefreshToken(in HttpContext context, in string refreshToken)
    {
        var cookieToken = context.Request.Cookies[_refreshCookieName];
        if (!cookieToken.IsMatch(refreshToken)) throw new Exception($"""
        {DurableSetup.Link(EnterpriseIntegrationFlag.CarrierTokenInvalid)}: input[{refreshToken}], cookie[{cookieToken}]
        """);
    }
    public BearerTokenInfo GetBearerToken(HttpContext context, ClaimParameter parameter)
    {
        var settings = AccessRecipe.Value.Authentication;
        return new()
        {
            AccessToken = GetAccessToken(),
            Administrator = parameter.Administrator,
            ExpiresIn = settings.ExpirySeconds,
            TokenType = JwtBearerDefaults.AuthenticationScheme,
            RefreshToken = GetRefreshToken(),
        };
        string GetAccessToken()
        {
            SymmetricSecurityKey securityKey = new(Convert.FromBase64String(settings.Secret));
            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(claims: [
                new Claim(ClaimTypes.NameIdentifier, parameter.Id),
                new Claim(ClaimTypes.Name, parameter.UserName),
                new Claim(ClaimTypes.Role, parameter.DomainRoles.ToJson(indented: false)),
            ],
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
            expires: DateTime.UtcNow.AddSeconds(settings.ExpirySeconds)));
        }
        string GetRefreshToken()
        {
            var value = Guid.NewGuid().ToString("N");
            context.Response.Cookies.Append(_refreshCookieName, value, new CookieOptions
            {
                Secure = false,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(settings.ExpirationDays),
            });
            return value;
        }
    }
    public async ValueTask<IEnumerable<ClaimParameter.DomainRole>> GetDomainRolesAsync(string userId, CancellationToken ct)
    {
        List<ClaimParameter.DomainRole> results = [];
        var owners = await CacheMediator.ListFieldOwnerAsync(ct).ConfigureAwait(false);
        var members = await CacheMediator.ListFieldMemberAsync(ct).ConfigureAwait(false);
        foreach (var fieldModule in BeforeExpand.GetFieldModules())
        {
            var roleType = RolePolicy.None;
            var fieldType = FieldModule.Unrecognizable;
            if (owners.Any(x => x.Id.IsMatch(userId) && x.FieldType == fieldModule))
            {
                fieldType = fieldModule;
                roleType = RolePolicy.Owner;
            }
            else
            {
                var member = members.FirstOrDefault(x => x.Id.IsMatch(userId) && x.FieldType == fieldModule);
                if (member.Id != default)
                {
                    fieldType = member.FieldType;
                    roleType = member.RoleType;
                }
            }
            if (fieldType is not FieldModule.Unrecognizable) results.Add(new ClaimParameter.DomainRole
            {
                FieldType = fieldType,
                RoleType = roleType,
            });
        }
        return results;
    }
    public required IDurableSetup DurableSetup { get; init; }
    public required ICacheMediator CacheMediator { get; init; }
    public required IOptions<AccessRecipe> AccessRecipe { get; init; }
}