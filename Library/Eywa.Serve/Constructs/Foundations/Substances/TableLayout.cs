namespace Eywa.Serve.Constructs.Foundations.Substances;
public readonly ref struct TableLayout
{
    public record ColumnFilter(in string Name, in string Value);
    public record IntervalFilter(in DateTime StartTime, in DateTime EndTime);
    public static string LetCreate(in Type type, in IList<string> columnNames)
    {
        return $"CREATE TABLE IF NOT EXISTS {type.Name.ToSnakeCase()}({Join(columnNames)})";
    }
    public static string LetDelete<T>(in string id) where T : notnull
    {
        return $"DELETE FROM {typeof(T).Name.ToSnakeCase()} WHERE {nameof(SQLiteBase.Id).ToSnakeCase()} = '{id}'";
    }
    public static string LetSelect<T>(in ColumnFilter? column = null, in IntervalFilter? interval = null)
    {
        var type = typeof(T);
        List<string> columns = [];
        foreach (var property in type.GetProperties()) columns.Add(property.Name.ToSnakeCase());
        StringBuilder builder = new($"SELECT {Join(columns)} FROM {type.Name.ToSnakeCase()} ");
        if (column is not null) builder.Append($"WHERE {column.Name.ToSnakeCase()} = '{column.Value}' ");
        var createTimeTag = nameof(NpgsqlBase.CreateTime).ToSnakeCase();
        if (interval is not null)
        {
            builder.Append($"WHERE {createTimeTag} BETWEEN '{To(interval.StartTime)}' AND '{To(interval.EndTime)}' ");
            static string To(in DateTime time) => time.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture);
        }
        builder.Append($"ORDER BY {createTimeTag} DESC");
        return builder.ToString();
    }
    public static string LetSelect<T>(in string id) => LetSelect<T>(new ColumnFilter(nameof(SQLiteBase.Id), id));
    public static string LetInterval<T>(in string? timeString, in ColumnFilter? column = null)
    {
        IntervalFilter? interval = default;
        if (!string.IsNullOrEmpty(timeString) && timeString.TryParse(out var startTime, out var endTime)) interval = new(startTime, endTime);
        return LetSelect<T>(column, interval);
    }
    public static (string sql, DynamicParameters param) LetInsert<T>(in T entity) where T : notnull
    {
        List<string> columns = [];
        var type = entity.GetType();
        DynamicParameters parameters = new();
        foreach (var property in type.GetProperties())
        {
            var value = property.GetValue(entity);
            if (value is not null)
            {
                var name = property.Name.ToSnakeCase();
                columns.Add(name);
                parameters.Add(name, value);
            }
        }
        return ($"INSERT INTO {type.Name.ToSnakeCase()}({Join(columns)}) VALUES ({Join(columns.Select(x => AtTag(x)))})", parameters);
    }
    public static (string sql, DynamicParameters param) LetUpdate<T>(in string id, in T entity, in string columnKey, IEnumerable<string> filters) where T : notnull
    {
        List<string> columns = [];
        var type = entity.GetType();
        DynamicParameters parameters = new();
        parameters.Add(AtTag(columnKey), id);
        foreach (var property in type.GetProperties().Where(property => !filters.Any(x => x.IsMatch(property.Name))))
        {
            var value = property.GetValue(entity);
            if (value is not null)
            {
                var name = property.Name.ToSnakeCase();
                columns.Add(Condition(name));
                parameters.Add(name, value);
            }
        }
        return ($"UPDATE {type.Name.ToSnakeCase()} SET {Join(columns)} WHERE {Condition(columnKey)}", parameters);
        static string Condition(in string column) => $"{column.ToSnakeCase()}={AtTag(column)}";
    }
    public static string LetUniqueIndex(in string tableName, in string columnName)
    {
        return $"CREATE UNIQUE INDEX IF NOT EXISTS {tableName}_{columnName} ON {tableName} ({columnName});";
    }
    public static async IAsyncEnumerable<string> LetDeleteSubtag<T>(NpgsqlConnection connection, string columnName, string value) where T : NpgsqlBase
    {
        var entities = await connection.QueryAsync<T>(LetSelect<T>(new ColumnFilter(columnName, value))).ConfigureAwait(false);
        foreach (var entity in entities ?? []) yield return LetDelete<T>(entity.Id);
    }
    public static string? JoinQueries(IEnumerable<string>? contents)
    {
        contents ??= [];
        if (contents.Any())
        {
            var counter = (int)default;
            DefaultInterpolatedStringHandler handler = new(default, contents.Count());
            foreach (var content in contents)
            {
                handler.AppendFormatted(content);
                if (contents.Count() - counter++ is not 1) handler.AppendFormatted($";\u00A0");
            }
            return handler.ToStringAndClear();
        }
        return default;
    }
    static string AtTag(in string columnName) => $"@{columnName}";
    static string Join(in IEnumerable<string> columns)
    {
        var counter = (int)default;
        var formatCount = columns.Count();
        DefaultInterpolatedStringHandler handler = new(default, formatCount);
        foreach (var column in columns)
        {
            handler.AppendFormatted(column);
            if (formatCount - counter++ is not 1) handler.AppendFormatted(',');
        }
        return handler.ToStringAndClear();
    }
}