namespace Eywa.Serve.ManagementInformation;

[DependsOn(
    typeof(EywaDomainHumanResourcesModule),
    typeof(EywaInfrastructureBrowseMountModule))]
public sealed class EywaServeManagementInformationModule : BaseModule<EywaServeManagementInformationModule>
{
    public override void ConfigureServices(IServiceCollection services)
    {

    }
}