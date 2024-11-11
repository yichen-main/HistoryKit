namespace Eywa.Core.Architects.Primaries.Substances;
internal sealed class YamlConfigure : FileConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new YamlProvider(this);
    }
}