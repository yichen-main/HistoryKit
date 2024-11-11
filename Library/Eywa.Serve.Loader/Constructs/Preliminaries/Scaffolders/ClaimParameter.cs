namespace Eywa.Serve.Loader.Constructs.Preliminaries.Scaffolders;
public readonly record struct ClaimParameter
{
    public required string Id { get; init; }
    public required string UserName { get; init; }
    public required bool Administrator { get; init; }
    public required IEnumerable<DomainRole> DomainRoles { get; init; }
    public sealed class DomainRole
    {
        public required FieldModule FieldType { get; init; }
        public required RolePolicy RoleType { get; init; }
    }
}