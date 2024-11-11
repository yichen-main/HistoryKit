namespace Eywa.Serve.Loader.Constructs.Preliminaries.Assemblies;
internal sealed class ExceptionMiddle(IHttpContextAccessor httpContextAccessor) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Detail = exception.Message,
            Status = StatusCodes.Status404NotFound,
            TraceId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
            Instance = httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty,
        }, cancellationToken).ConfigureAwait(false);
        return true;
    }
}