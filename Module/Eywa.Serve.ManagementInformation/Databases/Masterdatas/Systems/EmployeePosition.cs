namespace Eywa.Serve.ManagementInformation.Databases.Masterdatas.Systems;
internal sealed class EmployeePosition : NpgsqlBase
{
    public const string PositionNoIndex = $"{nameof(EmployeePosition)}{nameof(PositionNo)}";
    [RowInfo(UniqueIndex = true)] public required string PositionNo { get; init; }
    public required string PositionName { get; init; }
    public required string Modifier { get; init; }
    public required DateTime ModifyTime { get; init; }
    public required string Creator { get; init; }
}