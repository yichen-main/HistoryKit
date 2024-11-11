namespace Eywa.Serve.Constructs.Adminiculars.Composers;
public abstract class NodeConsumer<TEvent> : IEventHandler<TEvent> where TEvent : struct
{
    protected abstract Task HolderAsync(TEvent @event, CancellationToken ct);
    protected Task ExecuteAsync(in Func<NpgsqlConnection, NpgsqlTransaction, SqlMapper.GridReader?, Task> options, in NpgsqlProvider provider)
    {
        return DurableSetup.TransactionAsync(options, provider);
    }
    public Task HandleAsync(TEvent eventModel, CancellationToken ct) => HolderAsync(eventModel, ct);
    public required IDurableSetup DurableSetup { get; init; }
}