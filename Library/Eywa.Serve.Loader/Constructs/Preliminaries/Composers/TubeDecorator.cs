namespace Eywa.Serve.Loader.Constructs.Preliminaries.Composers;
internal abstract class TubeDecorator(ITubeRepository tubeRepository) : ITubeRepository
{
    public virtual ValueTask<WebApplicationBuilder> AddAsync(Func<WebApplicationBuilder, ValueTask> builder) => tubeRepository.AddAsync(builder);
}