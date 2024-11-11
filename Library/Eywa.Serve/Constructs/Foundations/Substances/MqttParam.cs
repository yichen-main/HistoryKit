namespace Eywa.Serve.Constructs.Foundations.Substances;
public readonly struct MqttParam
{
    public required string ClientId { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Ip { get; init; }
    public required int Port { get; init; }
}