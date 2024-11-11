namespace Eywa.Serve.Constructs.Foundations.Quarterlies;
public interface IFiberRepository<T> where T : class
{
    Task<T?> GetAsync(string id, CancellationToken ct);
    Task<IEnumerable<T>> GetAsync(CancellationToken ct);
    Task AddAsync(T entity, CancellationToken ct);
    Task PutAsync(string id, T entity, CancellationToken ct, string[]? fields = default);
    Task ClearAsync(string id);
}