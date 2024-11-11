namespace Eywa.Serve.Constructs.Grindstones.Protections;
public static class InscribedExpand
{
    public static FieldModule Archive(this FieldModule type)
    {
        try
        {
            ReaderWriterLock.EnterWriteLock();
            FieldModules.Add(type);
            return type;
        }
        finally
        {
            ReaderWriterLock.ExitWriteLock();
        }
    }
    public static bool Contains(this FieldModule type)
    {
        try
        {
            ReaderWriterLock.EnterReadLock();
            return FieldModules.Contains(type);
        }
        finally
        {
            ReaderWriterLock.ExitReadLock();
        }
    }
    public static ServiceCollection AddServices<T>(this ServiceCollection services, Type type) where T : IModularization
    {
        foreach (var validatorType in typeof(IValidator<>).GetSubInterfaces<T>())
        {
            var interfaceType = validatorType.GetInterfaces().First(x =>
            x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IValidator<>));
            services.AddTransient(interfaceType, validatorType);
        }
        foreach (var eventType in typeof(IEventHandler<>).GetSubInterfaces<T>())
        {
            var interfaceType = eventType.GetInterfaces().First(x =>
            x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>));
            services.AddTransient(interfaceType, eventType);
        }
        services.AddFastEndpoints().AddResponseCaching().AddMemoryCache();
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<T>());
        Modules[type.Name] = type;
        return services;
    }
    public static void RemoveFieldModule(in Type type) => Modules.TryRemove(type.Name, out _);
    public static ConcurrentDictionary<string, Type> Modules { get; private set; } = [];
    public static ICollection<FieldModule> FieldModules { get; } = [];
    static ReaderWriterLockSlim ReaderWriterLock { get; } = new();
}