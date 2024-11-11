namespace Eywa.Serve.Constructs.Foundations.Substances;
public sealed class PageContents<T> : List<T>
{
    public PageContents(in IEnumerable<T> sources, in int currentPage, in int pageSize)
    {
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalEntries = sources.Count();
        var totalPage = TotalEntries / PageSize + (TotalEntries % PageSize == default ? default : 1);
        totalPage = totalPage > 10000 ? 10000 : totalPage;
        TotalPages = totalPage == default ? 1 : totalPage;
        AddRange(sources.Skip((CurrentPage - 1) * PageSize).Take(PageSize));
    }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalEntries { get; private set; }
    public int CurrentPage { get; private set; }
}