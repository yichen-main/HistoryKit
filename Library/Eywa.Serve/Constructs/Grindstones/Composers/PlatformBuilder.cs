namespace Eywa.Serve.Constructs.Grindstones.Composers;
public abstract class PlatformBuilder
{
    public abstract ValueTask<PlatformBuilder> BuildAsync(Assembly assembly);
    public abstract ValueTask<PlatformBuilder> StartAsync(Action<WebApplication, IEndpointRouteBuilder>? options = default);
    public WebApplication WebApp { get; protected set; } = null!;
}