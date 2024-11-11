namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.FieldMembers;
internal sealed class EnumAccessTypeVehicle : NodeEnlarge<EnumAccessTypeVehicle, EnumAccessTypeInput>
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(EnumAccessTypeInput req, CancellationToken ct)
    {
        List<EnumCistern> results = [];
        foreach (var type in Enum.GetValues<RolePolicy>())
        {
            if (type is RolePolicy.Editor || type is RolePolicy.Viewer) results.Add(new()
            {
                TypeNo = type.GetHashCode(),
                TypeName = DurableSetup.Link(type),
            });
        }
        await SendAsync(results, cancellation: ct).ConfigureAwait(false);
    }
}