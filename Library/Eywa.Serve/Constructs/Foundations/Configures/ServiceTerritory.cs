namespace Eywa.Serve.Constructs.Foundations.Configures;
public sealed class ServiceTerritory
{
    public required TextInfluxDB InfluxDB { get; init; }
    public required TextRabbitMQ RabbitMQ { get; init; }
    public required TextSeq Seq { get; init; }
    public readonly struct TextInfluxDB
    {
        public required string Endpoint { get; init; }
        public required string Token { get; init; }
        public required string Organization { get; init; }
    }
    public readonly struct TextRabbitMQ
    {
        public required string Endpoint { get; init; }
        public required string Username { get; init; }
        public required string Password { get; init; }
    }
    public readonly struct TextSeq
    {
        public required string Endpoint { get; init; }
        public required string ApiKey { get; init; }
    }
}