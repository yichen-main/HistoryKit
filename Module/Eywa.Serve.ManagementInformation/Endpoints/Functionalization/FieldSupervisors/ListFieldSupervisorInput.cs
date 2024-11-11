namespace Eywa.Serve.ManagementInformation.Endpoints.Functionalization.FieldSupervisors;
internal sealed class ListFieldSupervisorInput : NodeHeader
{
    public string? UserId { get; init; }
    public FieldModule? FieldType { get; init; }
    public sealed class Validator : AbstractValidator<ListFieldSupervisorInput>;
}