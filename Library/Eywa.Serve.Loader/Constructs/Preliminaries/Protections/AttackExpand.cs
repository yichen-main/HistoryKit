namespace Eywa.Serve.Loader.Constructs.Preliminaries.Protections;
internal static class AttackExpand
{
    public record ListenInfo(string Name, int Port, Action<ListenOptions>? ListenOptions);
    public static void Listening(this KestrelServerOptions options, IEnumerable<ListenInfo> endpoints)
    {
        foreach (var endpoint in endpoints)
        {
            if (!IPAddress.Loopback.VerifyPort(port: endpoint.Port))
            {
                switch (endpoint.ListenOptions)
                {
                    case not null:
                        options.ListenAnyIP(endpoint.Port, endpoint.ListenOptions);
                        break;

                    default:
                        options.ListenAnyIP(endpoint.Port);
                        break;
                }
                ListenBanners.Add((endpoint.Name, endpoint.Port, true, "main entrance"));
            }
            else PortExisted = true;
        }
    }
    public static async ValueTask CloseMarqueeAsync()
    {
        Display = default;
        if (Missions.TryGetValue(nameof(ListenInfo), out var mission)) await mission.ConfigureAwait(false);
    }
    public static async ValueTask CreateAsync<T1, T2>(Action<Exception> e) where T1 : PlatformBuilder where T2 : FactoryBuilder
    {
        try
        {
            var signalAsync = SignalAsync();
            var services = new ServiceCollection().AddSingleton<T1>();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(x =>
            {
                return x.GetInterfaces().Contains(typeof(IEditionRepository)) && x.IsClass;
            })) services.AddKeyedSingleton(typeof(IEditionRepository), type.Name, type);

            var factory = typeof(T2);
            var provider = services.AddSingleton(factory).BuildServiceProvider();
            var modular = factory.GetSpecificTypes<ModularAttribute>().First(x => factory.Name.IsFuzzy(x.Name));
            var method = modular.GetCustomAttributes<ModularAttribute>().First().Name;
            var station = modular.GetMethod(method)?.Invoke(provider.GetService(modular), parameters: [
                Assembly.GetAssembly(factory),
            ]);

            if (station is Task task) await task.ConfigureAwait(false);
            var platform = provider.GetService<T1>();
            if (platform is not null)
            {
                await signalAsync.ConfigureAwait(false);
                await platform.WebApp.RunAsync().ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            e(exception);
            exception.Fatal(typeof(T2).GetType());
            await Log.CloseAndFlushAsync().ConfigureAwait(false);
        }
        async ValueTask SignalAsync(int frequency = 100)
        {
            Console.Title = nameof(System);
            Console.CursorVisible = default;
            await Print(GetNameplate()).PrintAsync(ConsoleColor.Yellow).ConfigureAwait(false);
            AnsiConsole.Write(new FigletText($"{nameof(Eywa)} {nameof(Serve)}").LeftJustified().Color(Color.White));
            await new[]
            {
                new string('*', 75), Environment.NewLine,
            }.Merge().PrintAsync().ConfigureAwait(false);
            if (!Missions.Keys.Any(x => x.IsMatch(nameof(ListenInfo)))) Missions[nameof(ListenInfo)] = Task.Run(async () =>
            {
                PeriodicTimer timer = new(TimeSpan.FromMilliseconds(frequency));
                string name = string.Empty, stroke = "/", dash = "-", backslash = "\\", or = "|";
                while (await timer.WaitForNextTickAsync(default).ConfigureAwait(false))
                {
                    Console.SetCursorPosition(default, Console.CursorTop);
                    Console.Write(name switch
                    {
                        var x when x.IsMatch(or) => name = stroke,
                        var x when x.IsMatch(stroke) => name = dash,
                        var x when x.IsMatch(backslash) => name = or,
                        _ => name = backslash,
                    });
                    if (!Display)
                    {
                        Console.SetCursorPosition(default, Console.CursorTop);
                        timer.Dispose();
                    }
                }
                Console.SetCursorPosition(default, Console.CursorTop);
            }, default);
            static string Print(in (string tag, string content)[] menus)
            {
                List<string> lines = [];
                var space = $"\u00A0\u00A0\u00A0";
                Console.InputEncoding = Encoding.UTF8;
                Console.OutputEncoding = Encoding.UTF8;
                foreach (var (key, content) in menus) lines.Add($"{key,16}{space}=>{space}{content,-10}{Environment.NewLine}");
                return ((string[])[.. lines]).Merge();
            }
            static (string tag, string content)[] GetNameplate() => [
                ("Host name", Dns.GetHostName()),
                ("User name", Environment.UserName),
                (".NET SDK", Environment.Version.ToString()),
                ("Internet", NetworkInterface.GetIsNetworkAvailable().ToString()),
                ("Language tag", Thread.CurrentThread.CurrentCulture.IetfLanguageTag),
                ("Language name", Thread.CurrentThread.CurrentCulture.DisplayName),
                ("IPv4 physical", NetworkInterfaceType.Ethernet.GetLocalIPv4().FirstOrDefault() ?? string.Empty),
                ("IPv4 wireless", NetworkInterfaceType.Wireless80211.GetLocalIPv4().FirstOrDefault() ?? string.Empty),
                ("Project name", FileLayout.GetProjectName<T2>()),
                ("OS version", Environment.OSVersion.ToString()),
            ];
        }
    }
    public static bool PortExisted { get; private set; }
    public static IList<(string tag, int port, bool enabled, string description)> ListenBanners { get; private set; } = [];
    static Dictionary<string, Task> Missions { get; } = new Dictionary<string, Task>(StringComparer.Ordinal);
    static bool Display { get; set; } = true;
}