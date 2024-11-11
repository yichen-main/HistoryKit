namespace Eywa.Vehicle.Defender;

[DependsOn(
    typeof(EywaServeEnergyManagementModule),
    typeof(EywaServeManagementInformationModule),
    typeof(EywaServeIndustrialInternetThingsModule),
    typeof(EywaServeManufacturingExecutionModule),
    typeof(EywaServeQualityManagementModule),
    typeof(EywaServeWarehouseManagementModule))]
public sealed class AppModule : BaseModule<AppModule>
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddGraphQLServer().AddQueryType<OneselfQueryer>()
            .AddMutationType<OneselfMutator>().AddMutationConventions()
            .AddSubscriptionType<OneselfSubscriber>().AddInMemorySubscriptions()
            .DisableIntrospection(disable: false);
    }
}