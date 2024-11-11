namespace Eywa.Core.Architects.Primaries.Composers;
public abstract class LiteBase
{
    [BsonId] public string Id { get; init; } = null!;
    public DateTime CreateTime { get; protected set; } = DateTime.UtcNow;
}