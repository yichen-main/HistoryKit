namespace Eywa.Serve.Loader.Constructs.Preliminaries.Quarterlies;
public interface IConveyance
{
    ValueTask PushEventAsync<T>(in T message, in CancellationToken ct);
    ValueTask PushEventAsync<T>(in IEnumerable<T> messages, in CancellationToken ct);
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class Conveyance : IConveyance
{
    public ValueTask PushEventAsync<T>(in T message, in CancellationToken ct) => TopicEventSender.SendAsync(typeof(T).Name, message, ct);
    public ValueTask PushEventAsync<T>(in IEnumerable<T> messages, in CancellationToken ct) => TopicEventSender.SendAsync(typeof(T).Name, messages, ct);
    public required ITopicEventSender TopicEventSender { get; init; }
}