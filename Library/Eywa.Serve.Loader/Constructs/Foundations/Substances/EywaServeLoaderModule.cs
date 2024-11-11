namespace Eywa.Serve.Loader.Constructs.Foundations.Substances;

[DependsOn(typeof(EywaServeModule))]
public sealed class EywaServeLoaderModule : ModuleBase
{
    protected override void Load(ContainerBuilder builder)
    {
        Initialize(this, builder);
        builder.RegisterBuildCallback(x =>
        {

        });
    }
}