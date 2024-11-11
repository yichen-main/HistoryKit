namespace Eywa.Serve.Loader.Constructs.Preliminaries.Assemblies;
internal sealed class FiberRepository(WebApplicationBuilder builder, ITubeRepository tubeRepository) : TubeDecorator(tubeRepository)
{
    public override async ValueTask<WebApplicationBuilder> AddAsync(Func<WebApplicationBuilder, ValueTask>? builder)
    {
        if (builder is not null) await builder(Builder).ConfigureAwait(false);
        return Builder;
    }
    WebApplicationBuilder Builder { get; init; } = builder;
}