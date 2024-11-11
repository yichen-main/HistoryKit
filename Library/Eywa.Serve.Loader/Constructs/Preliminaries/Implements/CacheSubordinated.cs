namespace Eywa.Serve.Loader.Constructs.Preliminaries.Implements;
internal sealed class CacheSubordinated : HostedService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        new GarnetServer([]).Start();
        UseFiveMinutesJob<Invocable>();
        await string.Empty.PrintAsync(ConsoleColor.White).ConfigureAwait(false);
    }
    public sealed class Invocable : IInvocable
    {
        public Task Invoke()
        {
            try
            {

            }
            catch (Exception e)
            {
                e.Error(typeof(CacheSubordinated));
            }
            return Task.CompletedTask;
        }
    }
}