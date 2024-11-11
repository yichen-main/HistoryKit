namespace Eywa.Serve.Constructs.Grindstones.Quarterlies;
public interface ISQLiteHelper
{
    Task CreateAsync(in Type type);
    Task ConnectAsync(Type type, Func<SqliteConnection, ValueTask> sqlite);
    Task ConnectAsync<T>(Func<SqliteConnection, ValueTask> sqlite) where T : notnull;
    Task<TResult> ConnectAsync<T, TResult>(Func<SqliteConnection, Task<TResult>> sqlite);
    Task<T?> SelectAsync<T>(in SqliteConnection sqlite, in string id) where T : SQLiteBase;
    Task<IEnumerable<T>> SelectAsync<T>(in SqliteConnection sqlite, in string column, in string id) where T : SQLiteBase;
    Task InsertAsync<T>(in SqliteConnection sqlite, in T entity) where T : SQLiteBase;
    Task UpdateAsync<T>(in SqliteConnection sqlite, in string id, in T entity, in string[]? fields = default) where T : SQLiteBase;
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class SQLiteHelper : ISQLiteHelper
{
    public Task CreateAsync(in Type type)
    {
        List<string> columns = [];
        List<string> indexes = [];
        var properties = type.GetProperties();
        for (int i = default; i < properties.Length; i++)
        {
            var property = properties[i];
            var columnName = property.Name.ToSnakeCase();
            var rowInfo = property.GetCustomAttribute<RowInfoAttribute>();
            if (rowInfo is { UniqueIndex: true }) indexes.Add(TableLayout.LetUniqueIndex(type.Name.ToSnakeCase(), columnName));
            columns.Add(property.Name switch
            {
                nameof(SQLiteBase.Id) => $"{columnName} TEXT PRIMARY KEY",
                nameof(SQLiteBase.CreateTime) => $"{columnName} TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL",
                _ => $"{columnName} {property.PropertyType switch
                {
                    var x when x.IsEnum => "INTEGER",
                    var x when x.Equals(typeof(byte[])) => "BLOB",
                    var x when x.Equals(typeof(float)) => "REAL",
                    var x when x.Equals(typeof(double)) => "REAL",
                    var x when x.Equals(typeof(bool)) => "INTEGER",
                    var x when x.Equals(typeof(byte)) => "INTEGER",
                    var x when x.Equals(typeof(short)) => "INTEGER",
                    var x when x.Equals(typeof(int)) => "INTEGER",
                    var x when x.Equals(typeof(long)) => "INTEGER",
                    var x when x.Equals(typeof(sbyte)) => "INTEGER",
                    var x when x.Equals(typeof(ushort)) => "INTEGER",
                    var x when x.Equals(typeof(uint)) => "INTEGER",
                    var x when x.Equals(typeof(ulong)) => "INTEGER",
                    _ => "TEXT",
                }} NOT NULL",
            });
        }
        var sql = TableLayout.LetCreate(type, columns);
        return ConnectAsync(type, async sqlite =>
        {
            await sqlite.ExecuteAsync(sql).ConfigureAwait(false);
            foreach (var index in indexes) await sqlite.ExecuteAsync(index).ConfigureAwait(false);
        });
    }
    public async Task ConnectAsync(Type type, Func<SqliteConnection, ValueTask> sqlite)
    {
        var connection = GetSqliteConnection(type);
        await using (connection.ConfigureAwait(false))
        {
            await sqlite(connection).ConfigureAwait(false);
        }
    }
    public async Task ConnectAsync<T>(Func<SqliteConnection, ValueTask> sqlite) where T : notnull
    {
        var connection = GetSqliteConnection<T>();
        await using (connection.ConfigureAwait(false))
        {
            await sqlite(connection).ConfigureAwait(false);
        }
    }
    public async Task<TResult> ConnectAsync<T, TResult>(Func<SqliteConnection, Task<TResult>> sqlite)
    {
        var connection = GetSqliteConnection<T>();
        await using (connection.ConfigureAwait(false))
        {
            return await sqlite(connection).ConfigureAwait(false);
        }
    }
    public Task<T?> SelectAsync<T>(in SqliteConnection sqlite, in string id) where T : SQLiteBase =>
        sqlite.QueryFirstOrDefaultAsync<T>(TableLayout.LetSelect<T>(id));
    public Task<IEnumerable<T>> SelectAsync<T>(in SqliteConnection sqlite, in string column, in string id) where T : SQLiteBase =>
        sqlite.QueryAsync<T>(TableLayout.LetSelect<T>(new TableLayout.ColumnFilter(column, id)));
    public Task InsertAsync<T>(in SqliteConnection sqlite, in T entity) where T : SQLiteBase
    {
        var (sql, param) = TableLayout.LetInsert(entity);
        return sqlite.ExecuteAsync(sql, param);
    }
    public Task UpdateAsync<T>(in SqliteConnection sqlite, in string id, in T entity, in string[]? fields = default) where T : SQLiteBase
    {
        List<string> filters = [nameof(SQLiteBase.Id), nameof(SQLiteBase.CreateTime)];
        if (fields is not null) foreach (var field in fields) filters.Add(field);
        var (sql, param) = TableLayout.LetUpdate(id, entity, nameof(SQLiteBase.Id), filters);
        return sqlite.ExecuteAsync(sql, param);
    }
    static SqliteConnection GetSqliteConnection<T>() => GetSqliteConnection(typeof(T));
    static SqliteConnection GetSqliteConnection(Type type)
    {
        return new($"Data Source={SQLite($"{FileLayout.GetProjectName(type)}{FileExtension.Pile}")}");
        static string SQLite(in string name) => SystemPath.Combine(FileLayout.GetRootFolderPath(nameof(SQLite)), name);
    }
}