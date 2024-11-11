namespace Eywa.Serve.Loader.Antisepsis.Dynamizers.Scaffolders;
internal sealed class ServerEdition : IEditionRepository
{
    public ValueTask InitialAsync(Assembly assembly)
    {
        return new();
    }
    public void Add(in Action<KestrelServerOptions> options)
    {

    }
    public WebApplication Add(Action<WebApplication, IEndpointRouteBuilder> options)
    {
        throw new NotImplementedException();
    }
    public WebApplicationBuilder Builder { get; private set; } = null!;
}