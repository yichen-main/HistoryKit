namespace Eywa.Serve.Constructs.Adminiculars.Composers;
public abstract class NodeHeader : PageQueryer
{
    [FastEndpoints.FromHeader(HeaderName.AcceptTimeZone)] public required int TimeZone { get; init; }
    [FastEndpoints.FromHeader(HeaderName.AcceptTimeFormat)] public required string TimeFormat { get; init; }
}