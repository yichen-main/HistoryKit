namespace Eywa.Serve.Transports.HumanResources.PeopleManagements;
public readonly struct AccountSystemRegistrationImport : IRequest
{
    public required string Email { get; init; }
}