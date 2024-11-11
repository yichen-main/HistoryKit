namespace Eywa.Vehicle.Defender.Foundations.Entities;
internal class LiteDemo : LiteBase
{
    [UniqueIndex] public required string Name { get; init; }
}