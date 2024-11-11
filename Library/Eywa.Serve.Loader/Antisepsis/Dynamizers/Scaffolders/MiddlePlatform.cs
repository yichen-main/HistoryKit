namespace Eywa.Serve.Loader.Antisepsis.Dynamizers.Scaffolders;
public sealed class MiddlePlatform([FromKeyedServices(nameof(MiddleEdition))] IEditionRepository editionRepository) : PlatformBuilder
{
    public override async ValueTask<PlatformBuilder> BuildAsync(Assembly assembly)
    {
        await editionRepository.InitialAsync(assembly).ConfigureAwait(false);
        editionRepository.Add(x =>
        {
            List<AttackExpand.ListenInfo> listens = [];
            var hostInfo = editionRepository.Builder.Configuration.GetSection(nameof(HostInformation)).Get<HostInformation>();
            if (hostInfo is not null)
            {
                if (hostInfo.HTTPS.Enabled)
                {
                    x.ConfigureHttpsDefaults(x => x.SslProtocols = SslProtocols.Tls12);
                    listens.Add(new(nameof(HostInformation.HTTPS), hostInfo.HTTPS.Port, x =>
                    {
                        x.UseHttps(hostInfo.HTTPS.Certificate.Location, hostInfo.HTTPS.Certificate.Password);
                    }));
                }
                listens.Add(new(nameof(HostInformation.HTTP), hostInfo.HTTP.Port, default));
            }
            if (listens.Count is not 0) x.Listening(listens);
        });
        return this;
    }
    public override async ValueTask<PlatformBuilder> StartAsync(Action<WebApplication, IEndpointRouteBuilder>? options = default)
    {
        WebApp = editionRepository.Add((app, endpoint) =>
        {
            endpoint.MapControllers();
            endpoint.MapFastEndpoints(x =>
            {
                x.Serializer.Options.PropertyNameCaseInsensitive = true;
            });
            if (options is not null) options(app, endpoint);
        });
        Table table = new();
        table.Border(TableBorder.Rounded);
        table.AddColumn(new TableColumn("[bold chartreuse3]Tag name[/]").LeftAligned());
        table.AddColumn(new TableColumn("[bold chartreuse3]Port number[/]").LeftAligned());
        table.AddColumn(new TableColumn("[bold chartreuse3]Service enabled[/]").LeftAligned());
        table.AddColumn(new TableColumn("[bold chartreuse3]Description[/]").LeftAligned());
        await AttackExpand.CloseMarqueeAsync().ConfigureAwait(false);
        foreach (var (tag, port, enabled, description) in AttackExpand.ListenBanners)
        {
            table.AddRow($"[bold]{tag}[/]", $"[bold]{port.ToString(CultureInfo.InvariantCulture)}[/]",
                $"[bold]{enabled}[/]", $"[bold]{description}[/]");
        }
        AnsiConsole.Write(table);
        await string.Empty.PrintAsync(ConsoleColor.White).ConfigureAwait(false);
        return this;
    }
}