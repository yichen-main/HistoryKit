namespace Eywa.Vehicle.Audience;

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

    }
}