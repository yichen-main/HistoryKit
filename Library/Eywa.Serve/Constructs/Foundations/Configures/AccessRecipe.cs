namespace Eywa.Serve.Constructs.Foundations.Configures;
public sealed class AccessRecipe
{
    public required string DefaultCulture { get; init; }
    public required string[] SupportedCultures { get; init; }
    public required TextCrossOrigin CrossOrigin { get; init; }
    public required TextAuthentication Authentication { get; init; }
    public readonly struct TextCrossOrigin
    {
        public required string AllowedMethods { get; init; }
        public required string[] Whitelist { get; init; }
    }
    public readonly struct TextAuthentication
    {
        public required string Secret { get; init; }
        public required int ExpirySeconds { get; init; }
        public required int ExpirationDays { get; init; }
    }
}