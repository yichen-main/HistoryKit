namespace Eywa.Serve.Loader.Constructs.Preliminaries.Protections;
public static class LoadedHost
{
    public static ValueTask ServerAsync<T>(Action<Exception>? e = default) where T : FactoryBuilder
    {
        return AttackExpand.CreateAsync<ServerPlatform, T>(e: exception =>
        {
            if (e is not null) e(exception);
        });
    }
    public static ValueTask MiddleAsync<T>(Action<Exception>? e = default) where T : FactoryBuilder
    {
        return AttackExpand.CreateAsync<MiddlePlatform, T>(e: exception =>
        {
            if (e is not null) e(exception);
        });
    }
}