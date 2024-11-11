namespace Eywa.Core.Architects.Primaries.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RowInfoAttribute : Attribute
{
    public bool ForeignKey { get; init; }
    public bool UniqueIndex { get; init; }
}