namespace Eywa.Serve.Loader.Constructs.Preliminaries.Scaffolders;
public readonly record struct JwtBearerDefaults
{
    public const string AuthenticationScheme = "Bearer";
    public const string FailureMessage = "Identity authentication failed";
}