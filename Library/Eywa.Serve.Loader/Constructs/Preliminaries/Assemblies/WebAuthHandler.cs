namespace Eywa.Serve.Loader.Constructs.Preliminaries.Assemblies;
internal sealed class WebAuthHandler(IOptionsMonitor<WebAuthOption> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<WebAuthOption>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            if (Request.Headers.TryGetValue(HeaderName.Authorization, out var value))
            {
                var token = GetAccessToken(value);
                if (token.ValidTo >= DateTime.UtcNow)
                {
                    ClaimsIdentity claimsIdentity = new(token.Claims, nameof(Eywa));
                    ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
                    AuthenticationTicket authTicket = new(claimsPrincipal, Scheme.Name);
                    return Task.FromResult(AuthenticateResult.Success(authTicket));
                }
            }
            return Task.FromResult(AuthenticateResult.NoResult());
        }
        catch (Exception e)
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }
        static JwtSecurityToken GetAccessToken(in StringValues value)
        {
            var names = value.ToString().Split(' ');
            return names.Length != 2 || string.IsNullOrEmpty(names[1]) || names[1].IsMatch("null") || !names[0].IsMatch(
                JwtBearerDefaults.AuthenticationScheme) ? throw new Exception() : new JwtSecurityTokenHandler().ReadJwtToken(names[1]);
        }
    }
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await Context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Detail = JwtBearerDefaults.FailureMessage,
            Status = StatusCodes.Status401Unauthorized,
        }, cancellationToken: Context.RequestAborted).ConfigureAwait(false);
    }
}