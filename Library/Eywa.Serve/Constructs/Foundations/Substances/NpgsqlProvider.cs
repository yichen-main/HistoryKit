namespace Eywa.Serve.Constructs.Foundations.Substances;
public readonly struct NpgsqlProvider
{
    public required IEnumerable<string> QueryFields { get; init; }
    public required CancellationToken CancellationToken { get; init; }
}