using FromHeaderAttribute = Microsoft.AspNetCore.Mvc.FromHeaderAttribute;

namespace Eywa.Serve.Loader.Constructs.Preliminaries.Composers;
public sealed class AppHeader
{
    [FromHeader(Name = HeaderName.AcceptTimeZone)] public required int TimeZone { get; init; }
    [FromHeader(Name = HeaderName.AcceptTimeFormat)] public required string TimeFormat { get; init; }
}