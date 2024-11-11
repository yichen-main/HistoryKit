namespace Eywa.Serve.Constructs.Grindstones.Assemblies;
internal sealed class DialectLocalizer(FrozenDictionary<string, Dictionary<string, string>> dialects) : IStringLocalizer
{
    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(CultureInfo.CurrentUICulture.Name, name);
            return new(name, value ?? name, resourceNotFound: value is null);
        }
    }
    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = GetString(CultureInfo.CurrentUICulture.Name, name);
            return new(name, string.Format(format ?? name, arguments), resourceNotFound: format is null);
        }
    }
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return dialects.TryGetValue(CultureInfo.CurrentUICulture.Name, out var texts) ? texts.Select(x =>
        new LocalizedString(x.Key, x.Value ?? string.Empty, resourceNotFound: false)) : [];
    }
    string? GetString(in string culture, in string name)
    {
        if (dialects.TryGetValue(culture, out var texts))
        {
            texts.TryGetValue(name, out var value);
            return value;
        }
        return null;
    }
}