namespace Eywa.Vehicle.Defender.Foundations;
internal sealed class TableContext(DbContextOptions<TableContext> options) : DbContext(options)
{
    public DbSet<UserShiftGroup> UserShiftGroup { get; set; }
}