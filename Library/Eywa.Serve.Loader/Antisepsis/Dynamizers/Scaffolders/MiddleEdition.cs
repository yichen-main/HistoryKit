namespace Eywa.Serve.Loader.Antisepsis.Dynamizers.Scaffolders;
internal sealed class MiddleEdition : IEditionRepository
{
    public async ValueTask InitialAsync(Assembly assembly)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = Environment.GetCommandLineArgs(),
            ContentRootPath = AppContext.BaseDirectory,
        });
        builder.Configuration.AddEnvironmentVariables();
        builder.Logging.AddFilter<ConsoleLoggerProvider>(nameof(Microsoft), LogLevel.Error);
        builder.Logging.AddFilter<ConsoleLoggerProvider>(nameof(FastEndpoints), LogLevel.Error);
        builder.Services.Configure<AccessRecipe>(builder.Configuration.GetSection(nameof(AccessRecipe)));
        builder.Services.Configure<HostInformation>(builder.Configuration.GetSection(nameof(HostInformation)));
        builder.Services.Configure<ServiceTerritory>(builder.Configuration.GetSection(nameof(ServiceTerritory)));
        builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
        builder.Services.Configure<RequestLocalizationOptions>(x =>
        {
            List<CultureInfo> cultureInfos = [];
            var settings = builder.Configuration.GetSection(nameof(AccessRecipe)).Get<AccessRecipe>();
            if (settings is not null)
            {
                cultureInfos.AddRange(settings.SupportedCultures.Select(x => new CultureInfo(x)));
                x.DefaultRequestCulture = new(settings.DefaultCulture);
                x.SupportedCultures = cultureInfos;
                x.SupportedUICultures = cultureInfos;
                x.RequestCultureProviders.Insert(default, new AcceptLanguageHeaderRequestCultureProvider());
            }
        });
        builder.Services.AddControllers();
        builder.Services.AddLocalization();
        builder.Services.AddSingleton<IStringLocalizerFactory>(x => new DialectFactory());
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>((context, builder) =>
        {
            foreach (var type in assembly.GetTypes().Where(x =>
            {
                return typeof(ModuleBase).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface;
            })) LoadModule(type);
            void LoadModule(in Type type)
            {
                if (LoadedModules.Contains(type)) return;
                var dependsOn = type.GetCustomAttribute<DependsOnAttribute>();
                if (dependsOn is not null)
                {
                    foreach (var dependency in dependsOn.Dependencies) LoadModule(dependency);
                }
                var module = Activator.CreateInstance(type) as ModuleBase;
                if (module is not null)
                {
                    builder.RegisterModule(module);
                    LoadedModules.Add(type);
                }
            }
        });
        Builder = await new FiberRepository(builder, new TubeRepository(builder)).AddAsync(x =>
        {
            var info = x.Configuration.GetSection(nameof(HostInformation)).Get<HostInformation>();
            x.Services.AddRateLimiter(x =>
            {
                x.AddFixedWindowLimiter(nameof(Eywa), x =>
                {
                    x.QueueLimit = 2;
                    x.PermitLimit = 4;
                    x.Window = TimeSpan.FromSeconds(10);
                    x.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
                x.RejectionStatusCode = 429;
            });
            AddValidator(x);
            static void AddValidator(WebApplicationBuilder builder)
            {
                var settings = builder.Configuration.GetSection(nameof(AccessRecipe)).Get<AccessRecipe>()!;
                builder.Services.AddCors(x =>
                {
                    x.AddDefaultPolicy(x =>
                    {
                        List<string> origins = [];
                        foreach (var address in settings.CrossOrigin.Whitelist)
                        {
                            if (IPAddress.TryParse(address, out IPAddress? ip) && ip.AddressFamily is AddressFamily.InterNetwork)
                            {
                                var host = ip.ToString();
                                origins.Add(new UriBuilder()
                                {
                                    Host = host,
                                    Scheme = nameof(HostInformation.HTTP).ToLower(CultureInfo.InvariantCulture),
                                }.Uri.ToString());
                                origins.Add(new UriBuilder()
                                {
                                    Host = host,
                                    Scheme = nameof(HostInformation.HTTPS).ToLower(CultureInfo.InvariantCulture),
                                }.Uri.ToString());
                            }
                        }
                        x.WithOrigins([.. origins]).AllowCredentials().WithMethods(settings.CrossOrigin.AllowedMethods.Split(','))
                        .WithHeaders(HeaderName.Authorization, HeaderName.AcceptTimeZone, HeaderName.AcceptTimeFormat);
                    });
                });
                builder.Services.AddAuthentication().AddScheme<WebAuthOption, WebAuthHandler>(nameof(Eywa), option =>
                {

                });
            }
            return new();
        }).ConfigureAwait(false);
    }
    public void Add(in Action<KestrelServerOptions> options)
    {
        Builder.Services.AddSoapCore();
        Builder.Services.AddScheduler();
        Builder.Services.AddHttpClient();
        Builder.Services.AddProblemDetails();
        Builder.Services.AddHttpContextAccessor();
        Builder.Services.AddEndpointsApiExplorer();
        Builder.Services.AddExceptionHandler<ExceptionMiddle>();
        Builder.Host.ConfigureHostOptions(x =>
        {
            x.ServicesStopConcurrently = true;
            x.ServicesStartConcurrently = true;
            x.ShutdownTimeout = TimeSpan.FromSeconds(30);
        }).UseSystemd();
        Builder.Services.ConfigureHttpJsonOptions(x =>
        {
            x.SerializerOptions.Converters.Add(new DateConvert());
            x.SerializerOptions.Converters.Add(new EnumConvert());
            x.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        Builder.Services.AddControllers(x =>
        {
            x.ReturnHttpNotAcceptable = true;
        }).ConfigureApiBehaviorOptions(x =>
        {
            x.SuppressModelStateInvalidFilter = default;
        }).AddMvcOptions(x => x.Conventions.Add(new ModelConvention())).AddControllersAsServices();
        Builder.WebHost.UseKestrel(options).ConfigureLogging((context, builder) =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Critical);
            {
                var seq = context.Configuration.GetSection(nameof(ServiceTerritory)).Get<ServiceTerritory>()!.Seq;
                Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.Debug()
                .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Error)
                .MinimumLevel.Override(nameof(FastEndpoints), LogEventLevel.Error)
                .WriteTo.Console().WriteTo.Seq(seq.Endpoint, apiKey: seq.ApiKey).CreateLogger();
            }
        });
    }
    public WebApplication Add(Action<WebApplication, IEndpointRouteBuilder> options)
    {
        var app = Builder.Build();
        app.UseMiddleware<ResponseMiddle>();
        app.UseExceptionHandler();
        app.UseRouting();
        app.UseCors();
        app.UseCookiePolicy();
        app.UseRateLimiter();
        app.UseAuthorization();
        app.UseResponseCaching();
        app.UseRequestLocalization();
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        });
        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromMinutes(5),
        });
        app.UseEndpoints(x => options(app, x));
        return app;
    }
    List<Type> LoadedModules { get; set; } = [];
    public WebApplicationBuilder Builder { get; private set; } = null!;
}