namespace Eywa.Serve.Loader.Antisepsis.Dynamizers.Quarterlies;
public interface IEditionRepository
{
    ValueTask InitialAsync(Assembly assembly);
    void Add(in Action<KestrelServerOptions> options);
    WebApplication Add(Action<WebApplication, IEndpointRouteBuilder> options);
    WebApplicationBuilder Builder { get; }
}