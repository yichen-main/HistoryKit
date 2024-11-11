namespace Eywa.Serve.Constructs.Grindstones.Composers;
public abstract class PageQueryer
{
    short Page = 10;
    readonly short MaxSize = 10000;
    public string? Search { get; init; }
    public string? TimeInterval { get; init; }
    public short PageIndex { get; init; } = 1;
    public short PageSize
    {
        get => Page;
        set => Page = value > MaxSize ? MaxSize : value;
    }
}