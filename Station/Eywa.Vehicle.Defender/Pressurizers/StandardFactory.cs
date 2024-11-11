namespace Eywa.Vehicle.Defender.Pressurizers;

[Modular]
public sealed class StandardFactory(MiddlePlatform middlePlatform) : FactoryBuilder
{
    public override async ValueTask<FactoryBuilder> AddAsync()
    {
        await middlePlatform.BuildAsync(Assembly).ConfigureAwait(false);
        return this;
    }
    public override async ValueTask<FactoryBuilder> RunAsync()
    {
        await middlePlatform.StartAsync((app, endpoint) =>
        {
            var hostInfo = app.Configuration.GetSection(nameof(HostInformation)).Get<HostInformation>();
            if (hostInfo is not null)
            {
                endpoint.MapGraphQL(hostInfo.HTTP.Path.GraphQL);
            }
        }).ConfigureAwait(false);
        return this;
    }
}