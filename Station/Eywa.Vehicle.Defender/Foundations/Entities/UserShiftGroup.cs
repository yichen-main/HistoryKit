namespace Eywa.Vehicle.Defender.Foundations.Entities;

[Table("user_shift_group")]
internal sealed class UserShiftGroup
{
    [Key, Column("id")] public required Guid Id { get; init; }
    [Column("user_name")] public required string UserName { get; set; }
    [Column("create_time")] public required DateTime CreateTime { get; init; }
}