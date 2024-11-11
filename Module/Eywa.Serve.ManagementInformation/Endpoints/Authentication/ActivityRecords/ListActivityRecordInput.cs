namespace Eywa.Serve.ManagementInformation.Endpoints.Authentication.ActivityRecords;
internal sealed class ListActivityRecordInput : NodeHeader
{
    public string? UserId { get; init; }
    public sealed class Validator : AbstractValidator<ListActivityRecordInput>;
};