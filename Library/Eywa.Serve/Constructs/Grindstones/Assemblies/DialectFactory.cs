namespace Eywa.Serve.Constructs.Grindstones.Assemblies;
public sealed class DialectFactory : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource) => new DialectLocalizer(InternalExpand.Dialects.ToFrozenDictionary());
    public IStringLocalizer Create(string baseName, string location) => new DialectLocalizer(InternalExpand.Dialects.ToFrozenDictionary());
}