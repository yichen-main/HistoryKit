namespace Eywa.Serve.Loader.Constructs.Preliminaries.Assemblies;
internal sealed class TubeRepository(WebApplicationBuilder webApplicationBuilder) : ITubeRepository
{
    public async ValueTask<WebApplicationBuilder> AddAsync(Func<WebApplicationBuilder, ValueTask> builder)
    {
        await builder(webApplicationBuilder).ConfigureAwait(false);
        return webApplicationBuilder;
    }
}