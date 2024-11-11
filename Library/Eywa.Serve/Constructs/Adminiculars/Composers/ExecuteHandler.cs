namespace Eywa.Serve.Constructs.Adminiculars.Composers;
public abstract class ExecuteHandler<T1> : IRequestHandler<T1> where T1 : IRequest
{
    protected readonly struct Options
    {
        public required T1 Import { get; init; }
        public required SqlMapper.GridReader? Reader { get; init; }
        public required NpgsqlConnection Connection { get; init; }
        public required CancellationToken CancellationToken { get; init; }
    }
    protected abstract void Configure();
    protected abstract Task HandleAsync(Options options);
    public async Task Handle(T1 import, CancellationToken cancellationToken)
    {
        Configure();
        await DurableSetup.ExecuteAsync(async (connection, reader) => await HandleAsync(new()
        {
            Import = import,
            Connection = connection,
            CancellationToken = cancellationToken,
            Reader = reader,
        }).ConfigureAwait(false), new()
        {
            QueryFields = QueryFields,
            CancellationToken = cancellationToken,
        }).ConfigureAwait(false);
    }
    public required IDurableSetup DurableSetup { get; init; }
    public required ICacheMediator CacheMediator { get; init; }
    public required ICiphertextPolicy CiphertextPolicy { get; init; }
    public required IEnumerable<string> QueryFields { get; set; }
}
public abstract class ExecuteHandler<T1, T2> : IRequestHandler<T1, T2> where T1 : IRequest<T2> where T2 : notnull
{
    protected readonly struct Options
    {
        public required T1 Import { get; init; }
        public required SqlMapper.GridReader? Reader { get; init; }
        public required NpgsqlConnection Connection { get; init; }
        public required CancellationToken CancellationToken { get; init; }
    }
    protected abstract void Configure();
    protected abstract Task<T2> HandleAsync(Options options);
    public async Task<T2> Handle(T1 import, CancellationToken cancellationToken)
    {
        Configure();
        T2 result = default!;
        await DurableSetup.ExecuteAsync(async (connection, reader) =>
        {
            result = await HandleAsync(new()
            {
                Import = import,
                Connection = connection,
                CancellationToken = cancellationToken,
                Reader = reader,
            }).ConfigureAwait(false);
        }, new()
        {
            QueryFields = QueryFields,
            CancellationToken = cancellationToken,
        }).ConfigureAwait(false);
        return result!;
    }
    public required IDurableSetup DurableSetup { get; init; }
    public required ICacheMediator CacheMediator { get; init; }
    public required ICiphertextPolicy CiphertextPolicy { get; init; }
    public required IEnumerable<string> QueryFields { get; set; }
}