namespace Eywa.Serve.Constructs.Grindstones.Composers;
public abstract class ModuleBase : Autofac.Module
{
    protected Type Initialize<T>(in T content, ContainerBuilder builder) where T : notnull
    {
        var type = content.GetType();
        if (type.Assembly is not null)
        {
            builder.RegisterAssemblyTypes(type.Assembly).Where(x =>
            typeof(ControllerBase).IsAssignableFrom(x)).AsSelf().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(type.Assembly).Where(x =>
            typeof(IInvocable).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface).InstancePerDependency();

            var streamReaders = type.Assembly.GetDialectResourceStreams()?.Select(x => new Lazy<StreamReader>(() => new StreamReader(x)));
            if (streamReaders is not null)
            {
                foreach (var culture in streamReaders)
                {
                    using var reader = culture.Value;
                    var texts = JsonDocument.Parse(reader.ReadToEnd());
                    var cultureName = texts.RootElement.GetProperty(nameof(culture)).GetString();
                    var @object = texts.RootElement.GetProperty(nameof(texts)).EnumerateObject();
                    var contents = @object.ToDictionary(x => x.Name, x => x.Value.GetString() ?? string.Empty, StringComparer.Ordinal);
                    if (cultureName is not null)
                    {
                        if (InternalExpand.Dialects.TryGetValue(cultureName, out var oldContents))
                        {
                            foreach (var (key, value) in contents)
                            {
                                if (value is not null) oldContents[key] = value;
                            }
                        }
                        else InternalExpand.Dialects[cultureName] = contents;
                    }
                }
            }
        }
        var assemblyTypes = type.GetAssemblyTypes();
        for (var i = default(int); i < assemblyTypes.Length; i++)
        {
            var assemblyType = assemblyTypes[i];
            switch (assemblyType)
            {
                case var x when x.GetCustomAttributes<DependentAttribute>().Any():
                    {
                        var registration = builder.RegisterType(assemblyType);
                        var dependent = assemblyType.GetCustomAttribute<DependentAttribute>();
                        if (dependent is not null)
                        {
                            switch (dependent.ServiceLifetime)
                            {
                                case ServiceLifetime.Singleton:
                                    registration.SingleInstance();
                                    break;

                                case ServiceLifetime.Transient:
                                    registration.InstancePerDependency();
                                    break;

                                case ServiceLifetime.Scoped:
                                    registration.InstancePerLifetimeScope();
                                    break;
                            }
                        }
                        registration.AsImplementedInterfaces();
                    }
                    break;

                case var x when x.IsSubclassOf(typeof(HostedService)):
                    builder.RegisterType(assemblyType).As<IHostedService>().InstancePerDependency();
                    break;
            }
        }
        return type;
    }
}