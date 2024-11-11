namespace Eywa.Serve.Constructs.Foundations.Substances;
public sealed class EywaServeModule : ModuleBase
{
    protected override void Load(ContainerBuilder builder)
    {
        Initialize(this, builder);
        builder.RegisterBuildCallback(x =>
        {
            var configuration = x.Resolve<IConfiguration>();
            var settings = x.Resolve<IOptions<ServiceTerritory>>().Value;
            var baseCreator = new Lazy<IBaseCreator>(() => x.Resolve<IBaseCreator>()).Value;
            baseCreator.Configure(configuration.GetConnectionString(nameof(Npgsql)), settings.InfluxDB);
            baseCreator.Initialize();
        });
    }
}