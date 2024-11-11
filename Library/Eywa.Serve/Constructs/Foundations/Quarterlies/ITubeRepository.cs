namespace Eywa.Serve.Constructs.Foundations.Quarterlies;
public interface ITubeRepository
{
    ValueTask<WebApplicationBuilder> AddAsync(Func<WebApplicationBuilder, ValueTask> builder);
}