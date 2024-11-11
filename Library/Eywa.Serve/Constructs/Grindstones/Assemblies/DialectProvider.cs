namespace Eywa.Serve.Constructs.Grindstones.Assemblies;
internal sealed class DialectProvider(string defaultCulture, CultureInfo[] cultureInfos) : RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
    {
        var acceptLanguage = httpContext.Request.Headers.AcceptLanguage.ToString();
        if (string.IsNullOrEmpty(acceptLanguage)) return ActionAsync(defaultCulture);
        return ActionAsync(GetCulture(acceptLanguage));
    }
    string GetCulture(in string text)
    {
        var cultures = text.Split(',').Select(x => x.Split(';')[default]).Select(x => x.Trim());
        var culture = cultures.FirstOrDefault(x => cultureInfos.Any(info => info.Name.Equals(x, StringComparison.OrdinalIgnoreCase)));
        return culture ?? defaultCulture;
    }
    static Task<ProviderCultureResult?> ActionAsync(in string culture) => Task.FromResult(new ProviderCultureResult(culture))!;
}