namespace Eywa.Core.Architects.Primaries.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SignedAttribute(string name) : Attribute
{
    public string Name { get; init; } = name;
}