namespace Eywa.Serve.Loader.Constructs.Preliminaries.Scaffolders;
public readonly record struct BearerTokenInfo
{
    public required string AccessToken { get; init; }
    public required bool Administrator { get; init; }
    public required int ExpiresIn { get; init; }
    public required string TokenType { get; init; }
    public required string RefreshToken { get; init; }
}