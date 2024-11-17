namespace Eywa.Serve.Constructs.Grindstones.Composers;
public abstract class HostedModule<TModule> : ModuleBase where TModule : ModuleBase, IModularization
{
    protected void Execute(in ContainerBuilder builder)
    {
        LocalType = Initialize(this, builder);
        Lazy<ServiceCollection> services = new(() => new ServiceCollection().AddServices<TModule>(LocalType));
        {
            FieldModule = GetFieldModule();
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
        foreach (var module in LocalType.Assembly.GetTypes().Where(x => typeof(IModularization).IsAssignableFrom(x) && !x.IsAbstract))
        {
            if (module is not null) (Activator.CreateInstance(module) as IModularization)?.ConfigureServices(services.Value);
        }
        builder.Populate(services.Value);
        FieldModule GetFieldModule()
        {
            foreach (var name in Enum.GetNames<FieldModule>().Where(x => LocalType.Name.IsFuzzy(x)))
            {
                return Enum.Parse<FieldModule>(name, ignoreCase: true).Archive();
            }
            return FieldModule.Unrecognizable;
        }
    }
    protected FieldModule FieldModule { get; private set; }
    protected Type LocalType { get; private set; } = null!;
}