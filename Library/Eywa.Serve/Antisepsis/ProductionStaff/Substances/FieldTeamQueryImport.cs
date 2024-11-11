namespace Eywa.Serve.Antisepsis.ProductionStaff.Substances;
public readonly struct FieldTeamQueryImport
{
    public readonly struct Owner : IRequest<IEnumerable<Output>>;
    public readonly struct Member : IRequest<IEnumerable<Output>>;
    public readonly struct Output
    {
        public required string Id { get; init; }
        public required string Email { get; init; }
        public required string UserNo { get; init; }
        public required string UserName { get; init; }
        public required string DomainId { get; init; }
        public required FieldModule FieldType { get; init; }
        public required RolePolicy RoleType { get; init; }
    }
}