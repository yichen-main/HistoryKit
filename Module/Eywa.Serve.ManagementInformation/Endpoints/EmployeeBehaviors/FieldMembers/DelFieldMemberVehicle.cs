namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.FieldMembers;
internal sealed class DelFieldMemberVehicle() : NodeEnlarge<DelFieldMemberVehicle, DelFieldMemberInput>(RolePolicy.Owner)
{
    public override void Configure()
    {
        RequiredConfiguration();
    }
    public override async Task HandleAsync(DelFieldMemberInput req, CancellationToken ct)
    {
        await PublishAsync(new DelFieldMemberEvent
        {
            Id = req.Id,
        }, cancellation: ct).ConfigureAwait(false);
        await SendNoContentAsync(cancellation: ct).ConfigureAwait(false);
    }
}