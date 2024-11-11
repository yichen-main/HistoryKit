namespace Eywa.Core.Architects.Primaries.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class DependsOnAttribute(params Type[] dependencies) : Attribute
{
    public Type[] Dependencies { get; } = dependencies;
}