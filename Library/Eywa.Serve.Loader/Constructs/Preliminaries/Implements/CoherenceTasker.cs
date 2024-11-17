namespace Eywa.Serve.Loader.Constructs.Preliminaries.Implements;
internal sealed class CoherenceTasker : HostedService
{
    bool _enable;
    protected override Task ExecuteAsync(CancellationToken stoppingToken) => KeepAsync(this, async () =>
    {
        foreach (var modular in InscribedExpand.Modules)
        {
            foreach (var type in modular.Value.GetAssemblyTypes<NpgsqlBase>())
            {
                await BaseCreator.CreateNpgsqlAsync(type, stoppingToken).ConfigureAwait(false);
            }
            foreach (var type in modular.Value.GetAssemblyTypes<SQLiteBase>())
            {
                await SQLiteHelper.CreateAsync(type).ConfigureAwait(false);               
            }
            InscribedExpand.RemoveFieldModule(modular.Value);
        }
        if (!_enable)
        {
            SetInterval(this, Interval.TenSeconds);
            await BaseCreator.CreateBucketAsync().ConfigureAwait(false);
            BaseCreator.SetEnableDatabase();
            _enable = true;
        }
    });
    public required IBaseCreator BaseCreator { get; init; }
    public required ISQLiteHelper SQLiteHelper { get; init; }
}