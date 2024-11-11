namespace Eywa.Serve.Loader.Antisepsis.Dynamizers.Composers;
public abstract class FactoryBuilder
{
    public abstract ValueTask<FactoryBuilder> AddAsync();
    public abstract ValueTask<FactoryBuilder> RunAsync();
    public async Task AsyncCallback(Assembly assembly)
    {
        Assembly = assembly;
        await AddAsync().ConfigureAwait(false);
        await RunAsync().ConfigureAwait(false);
    }
    protected Assembly Assembly { get; set; } = null!;
}