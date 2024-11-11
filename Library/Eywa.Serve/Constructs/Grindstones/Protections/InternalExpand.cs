namespace Eywa.Serve.Constructs.Grindstones.Protections;
internal static class InternalExpand
{
    public static void Pagination<T>(this HttpContext httpContext, in PageContents<T> contents, string uri, IEnumerable<(string key, string? value)>? queries = default)
    {
        var previousLink = Link(Upper(contents.CurrentPage), contents.PageSize);
        var nextLink = Link(Down(contents.CurrentPage), contents.PageSize);
        var firstLink = Link(1, contents.PageSize);
        var lastLink = Link(contents.TotalPages, contents.PageSize);
        httpContext.Response.Headers.Append(HeaderName.Pagination, new
        {
            contents.PageSize,
            contents.TotalPages,
            contents.TotalEntries,
            contents.CurrentPage,
            PreviousLink = contents.CurrentPage > 1 ? previousLink : firstLink,
            NextLink = contents.CurrentPage < contents.TotalPages ? nextLink : lastLink,
            FirstLink = firstLink,
            LastLink = lastLink,
        }.ToJson(indented: false));
        int Down(in int count) => count + 1;
        int Upper(in int count) => count is 1 ? 1 : count - 1;
        string Link(in int pageCount, in int pageSize)
        {
            UriBuilder uriBuilder = new(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[nameof(pageCount)] = HttpUtility.UrlEncode(pageCount.ToString(CultureInfo.InvariantCulture));
            query[nameof(pageSize)] = HttpUtility.UrlEncode(pageSize.ToString(CultureInfo.InvariantCulture));
            foreach (var (key, value) in queries ?? [])
            {
                if (!string.IsNullOrEmpty(value)) query.Add(key.FirstCharLowercase(), HttpUtility.UrlEncode(value));
            }
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
    }
    public static ConcurrentDictionary<string, Dictionary<string, string>> Dialects { get; } = [];
}