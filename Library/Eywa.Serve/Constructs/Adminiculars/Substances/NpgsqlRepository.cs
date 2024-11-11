namespace Eywa.Serve.Constructs.Adminiculars.Substances;
internal class NpgsqlRepository<T>(NpgsqlConnection connection) : IFiberRepository<T> where T : NpgsqlBase
{
    public virtual Task<T?> GetAsync(string id, CancellationToken ct) => connection.QueryFirstOrDefaultAsync<T>(new(
        commandText: TableLayout.LetSelect<T>(id), commandType: CommandType.Text, cancellationToken: ct
    ));
    public virtual Task<IEnumerable<T>> GetAsync(CancellationToken ct) => connection.QueryAsync<T>(new(
        commandText: TableLayout.LetSelect<T>(), commandType: CommandType.Text, cancellationToken: ct
    ));
    public virtual Task AddAsync(T entity, CancellationToken ct)
    {
        var (sql, param) = TableLayout.LetInsert(entity);
        return connection.ExecuteAsync(new(commandText: sql, parameters: param, commandType: CommandType.Text, cancellationToken: ct));
    }
    public virtual Task PutAsync(string id, T entity, CancellationToken ct, string[]? fields = null)
    {
        List<string> filters = [nameof(NpgsqlBase.Id), nameof(NpgsqlBase.CreateTime)];
        if (fields is not null) foreach (var field in fields) filters.Add(field);
        var (sql, param) = TableLayout.LetUpdate(id, entity, nameof(NpgsqlBase.Id), filters);
        return connection.ExecuteAsync(new(commandText: sql, parameters: param, commandType: CommandType.Text, cancellationToken: ct));
    }
    public virtual Task ClearAsync(string id) => connection.ExecuteAsync(TableLayout.LetDelete<T>(id));
}