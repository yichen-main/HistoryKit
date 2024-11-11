namespace Eywa.Serve.Constructs.Foundations.Quarterlies;
public interface IQueueJanitor
{
    (string topic, T? payload) Parse<T>(in MqttApplicationMessage message);
    (string[] paths, T? payload) ParseLattice<T>(in MqttApplicationMessage message, in char divider = '/');
    Task<IManagedMqttClient> InitialAsync(Func<IManagedMqttClient, Task> options, MqttParam param, int reconnectSeconds = 5);
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class QueueJanitor : IQueueJanitor
{
    public (string topic, T? payload) Parse<T>(in MqttApplicationMessage message) =>
        (message.Topic, message.ConvertPayloadToString().ToObject<T>());
    public (string[] paths, T? payload) ParseLattice<T>(in MqttApplicationMessage message, in char divider = '/') =>
        (message.Topic.Split(divider), message.ConvertPayloadToString().ToObject<T>());
    public async Task<IManagedMqttClient> InitialAsync(Func<IManagedMqttClient, Task> options, MqttParam param, int reconnectSeconds = 5)
    {
        var option = new MqttClientOptionsBuilder()
            .WithTcpServer(param.Ip, param.Port)
            .WithCredentials(param.Username, param.Password)
            .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
            .WithClientId(param.ClientId).Build();
        var managed = new ManagedMqttClientOptionsBuilder().WithClientOptions(option)
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(reconnectSeconds));
        var client = new MqttFactory().CreateManagedMqttClient();
        await options(client).ConfigureAwait(false);
        await client.StartAsync(managed.Build()).ConfigureAwait(false);
        return client;
    }
}