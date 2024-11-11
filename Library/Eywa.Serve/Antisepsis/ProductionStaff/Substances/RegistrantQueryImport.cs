namespace Eywa.Serve.Antisepsis.ProductionStaff.Substances;
public readonly struct RegistrantQueryImport
{
    public readonly struct Everyone : IRequest<IEnumerable<Output>>;
    public readonly struct Output
    {
        public required string Id { get; init; }
        public required string Email { get; init; }
        public required string UserNo { get; init; }
        public required string UserName { get; init; }
    }
}