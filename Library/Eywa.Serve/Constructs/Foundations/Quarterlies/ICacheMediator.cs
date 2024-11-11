namespace Eywa.Serve.Constructs.Foundations.Quarterlies;
public interface ICacheMediator
{
    bool ExistFieldModule(FieldModule type);
    ValueTask<IEnumerable<FieldTeamQueryImport.Output>> ListFieldMemberAsync(CancellationToken ct);
    ValueTask<IEnumerable<FieldTeamQueryImport.Output>> ListFieldOwnerAsync(CancellationToken ct);
    ValueTask<IEnumerable<RegistrantQueryImport.Output>> ListAllRegistrantsAsync(CancellationToken ct);
    ValueTask<IEnumerable<EquipmentQueryOutput>> ListFacilityEquipmentAsync(CancellationToken ct);
}

[Dependent(ServiceLifetime.Singleton)]
file sealed partial class CacheMediator : ICacheMediator
{
    public bool ExistFieldModule(FieldModule type) => BeforeExpand.GetFieldModules().Any(x => x == type);
    public async ValueTask<IEnumerable<FieldTeamQueryImport.Output>> ListFieldMemberAsync(CancellationToken ct)
    {
        if (ExistFieldModule(FieldModule.HumanResources))
        {
            return await CacheAsync(new FieldTeamQueryImport.Member(), ct).ConfigureAwait(false) ?? [];
        }
        return [];
    }
    public async ValueTask<IEnumerable<FieldTeamQueryImport.Output>> ListFieldOwnerAsync(CancellationToken ct)
    {
        return await CacheAsync(new FieldTeamQueryImport.Owner(), ct).ConfigureAwait(false) ?? [];
    }
    public async ValueTask<IEnumerable<RegistrantQueryImport.Output>> ListAllRegistrantsAsync(CancellationToken ct)
    {
        return await CacheAsync(new RegistrantQueryImport.Everyone(), ct).ConfigureAwait(false) ?? [];
    }
    public async ValueTask<IEnumerable<EquipmentQueryOutput>> ListFacilityEquipmentAsync(CancellationToken ct)
    {
        if (ExistFieldModule(FieldModule.FacilityManagement))
        {
            return await CacheAsync(new EquipmentQueryImport(), ct).ConfigureAwait(false) ?? [];
        }
        return [];
    }
    async ValueTask<TResult?> CacheAsync<TResult>(IRequest<TResult> import, CancellationToken ct)
    {
        var key = import.GetType().Name;
        if (!MemoryCache.TryGetValue(key, out TResult? result))
        {
            result = await Mediator.Send(import, ct).ConfigureAwait(false);
            MemoryCache.Set(key, result, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
            });
        }
        return result;
    }
    public required IMediator Mediator { get; init; }
    public required IMemoryCache MemoryCache { get; init; }
}