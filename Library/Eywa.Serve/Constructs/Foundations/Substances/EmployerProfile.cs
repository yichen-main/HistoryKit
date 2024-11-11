namespace Eywa.Serve.Constructs.Foundations.Substances;
public sealed class EmployerProfile
{
    public required string Id { get; init; }
    public required string Salt { get; init; }
    public required string Hash { get; init; }
}