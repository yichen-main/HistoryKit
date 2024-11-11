namespace Eywa.Vehicle.Defender.Backstages;
public sealed class EveryMinuteJob : HostedService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        UseSecondJob<Invocable>();
        return Task.CompletedTask;
    }
    public sealed class Invocable() : IInvocable
    {
        public Task Invoke()
        {
            try
            {
                //using GarnetClient client = new("127.0.0.1", 6379);
                //await client.ConnectAsync().ConfigureAwait(false);

                //string key = "testKey";
                //string value = "Hello, Garnet!";
                //await client.StringSetAsync(key, value).ConfigureAwait(false);

                //string retrievedValue = await client.StringGetAsync(key).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                e.Error(typeof(EveryMinuteJob));
            }
            return Task.CompletedTask;
        }
    }
}