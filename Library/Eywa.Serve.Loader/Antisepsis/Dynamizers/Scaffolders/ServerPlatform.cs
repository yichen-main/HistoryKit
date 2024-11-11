namespace Eywa.Serve.Loader.Antisepsis.Dynamizers.Scaffolders;
public sealed class ServerPlatform([FromKeyedServices(nameof(ServerEdition))] IEditionRepository editionRepository) : PlatformBuilder
{
    public override ValueTask<PlatformBuilder> BuildAsync(Assembly assembly)
    {
        return new ValueTask<PlatformBuilder>();
    }
    public override ValueTask<PlatformBuilder> StartAsync(Action<WebApplication, IEndpointRouteBuilder>? options = null)
    {
        return new ValueTask<PlatformBuilder>();
    }
}