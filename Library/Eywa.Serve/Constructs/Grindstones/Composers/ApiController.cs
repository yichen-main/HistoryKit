namespace Eywa.Serve.Constructs.Grindstones.Composers;
public abstract class ApiController : ControllerBase
{
    protected string GetUserName() => User.Identity is not null ? User.Identity.Name ?? string.Empty : string.Empty;
    protected IEnumerable<T> Pagination<T>(in PageContents<T> contents)
    {
        HttpContext.Pagination(contents, $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
        return contents;
    }
    public required IDurableSetup DurableSetup { get; init; }
    public required ICacheMediator CacheMediator { get; init; }
}