namespace Eywa.Core.Architects.Primaries.Inventories;
public readonly record struct HistoryLetter
{
    public required int Line { get; init; }
    public required Type Type { get; init; }
    public required string Name { get; init; }
    public required string Message { get; init; }
    public object? Content { get; init; }
}