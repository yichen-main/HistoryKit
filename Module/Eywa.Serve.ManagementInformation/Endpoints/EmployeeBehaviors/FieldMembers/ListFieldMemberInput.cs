namespace Eywa.Serve.ManagementInformation.Endpoints.EmployeeBehaviors.FieldMembers;
internal sealed class ListFieldMemberInput : NodeHeader
{
    public string? UserId { get; init; }
    public FieldModule? FieldType { get; init; }
    public sealed class Validator : AbstractValidator<ListFieldMemberInput>;
}