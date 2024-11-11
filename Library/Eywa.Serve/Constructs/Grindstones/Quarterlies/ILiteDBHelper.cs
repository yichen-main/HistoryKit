namespace Eywa.Serve.Constructs.Grindstones.Quarterlies;
public interface ILiteDBHelper
{
    T Select<T>(in string id) where T : LiteBase;
    IEnumerable<T> Select<T>() where T : LiteBase;
    void Insert<T>(in T entity) where T : LiteBase;
    void Insert<T, K>(in T entity, Expression<Func<T, K>> selector) where T : LiteBase where K : class;
    void Update<T>(in string id, in T entity) where T : LiteBase;
    void Update<T, K>(in string id, in T entity, Expression<Func<T, K>> selector) where T : LiteBase;
    void Delete<T>(in string id) where T : LiteBase;
}

[Dependent(ServiceLifetime.Singleton)]
file sealed class LiteDBHelper : ILiteDBHelper
{
    public T Select<T>(in string id) where T : LiteBase
    {
        using LiteDatabase db = new(GetConnectionString());
        var collection = db.GetCollection<T>(typeof(T).Name.ToSnakeCase());
        return collection.FindById(id);
    }
    public IEnumerable<T> Select<T>() where T : LiteBase
    {
        using LiteDatabase db = new(GetConnectionString());
        var collection = db.GetCollection<T>(typeof(T).Name.ToSnakeCase());
        return collection.FindAll().ToArray();
    }
    public void Insert<T>(in T entity) where T : LiteBase
    {
        try
        {
            using LiteDatabase db = new(GetConnectionString());
            var collection = db.GetCollection<T>(typeof(T).Name.ToSnakeCase());
            foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<UniqueIndexAttribute>() is not null))
            {
                collection.EnsureIndex(property.Name, unique: true);
            }
            collection.Insert(entity);
        }
        catch (LiteException e)
        {
            var message = e.Message;
            if (message.Contains("Cannot insert duplicate key in unique index", StringComparison.Ordinal))
            {
                var fieldNameStart = message.IndexOf('\'', StringComparison.Ordinal) + 1;
                var fieldNameEnd = message.IndexOf('\'', fieldNameStart);
                var fieldName = message[fieldNameStart..fieldNameEnd];

                var valueStart = message.IndexOf('\'', fieldNameEnd + 1) + 1;
                var valueEnd = message.IndexOf('\'', valueStart);
                var value = message[valueStart..valueEnd];

                value = value.Replace("\"", "", StringComparison.Ordinal)
                    .Replace("\\", "", StringComparison.Ordinal);

                throw new LiteException(e.ErrorCode, $"已經存在：欄位名稱 = {fieldName}, 值 = {value}");
            }
            throw new LiteException(e.ErrorCode, message);
        }
    }
    public void Insert<T, K>(in T entity, Expression<Func<T, K>> selector) where T : LiteBase where K : class
    {
        try
        {
            using LiteDatabase db = new(GetConnectionString());
            var collection = db.GetCollection<T>(typeof(T).Name.ToSnakeCase());
            collection.EnsureIndex(GetIndexName(selector), selector, unique: true);
            foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<UniqueIndexAttribute>() is not null))
            {
                collection.EnsureIndex(property.Name, unique: true);
            }
            collection.Insert(entity);
        }
        catch (LiteException e)
        {
            var message = e.Message;
            if (message.Contains("Cannot insert duplicate key in unique index", StringComparison.Ordinal))
            {
                var fieldNameStart = message.IndexOf('\'', StringComparison.Ordinal) + 1;
                var fieldNameEnd = message.IndexOf('\'', fieldNameStart);
                var fieldName = message[fieldNameStart..fieldNameEnd];

                var valueStart = message.IndexOf('\'', fieldNameEnd + 1) + 1;
                var valueEnd = message.IndexOf('\'', valueStart);
                var value = message[valueStart..valueEnd];

                value = value.Replace("\"", "", StringComparison.Ordinal)
                    .Replace("\\", "", StringComparison.Ordinal);

                throw new LiteException(e.ErrorCode, $"已經存在：欄位名稱 = {fieldName}, 值 = {value}");
            }
            throw new LiteException(e.ErrorCode, message);
        }
    }
    public void Update<T>(in string id, in T entity) where T : LiteBase
    {
        try
        {
            var oldEntity = Select<T>(id);
            if (oldEntity is not null)
            {
                var properties = entity.GetType().GetProperties();
                for (int i = default; i < properties.Length; i++)
                {
                    var property = properties[i];
                    if (!property.Name.IsMatch(nameof(LiteBase.Id)) && !property.Name.IsMatch(nameof(LiteBase.CreateTime)))
                    {
                        var oldType = oldEntity.GetType();
                        var newValue = property.GetValue(entity);
                        var oldValue = oldType.GetProperty(property.Name)!.GetValue(oldEntity);
                        switch (newValue)
                        {
                            case not null when oldValue is not null && !newValue.Equals(oldValue):
                                oldType.GetProperty(property.Name)!.SetValue(oldEntity, newValue);
                                break;

                            default:
                                oldType.GetProperty(property.Name)!.SetValue(oldEntity, newValue);
                                break;
                        }
                    }
                }
                using LiteDatabase db = new(GetConnectionString());
                var collection = db.GetCollection<T>(typeof(T).Name.ToSnakeCase());
                collection.Update(oldEntity);
            }
        }
        catch (LiteException e)
        {
            var message = e.Message;
            if (message.Contains("Cannot insert duplicate key in unique index", StringComparison.Ordinal))
            {
                var fieldNameStart = message.IndexOf('\'', StringComparison.Ordinal) + 1;
                var fieldNameEnd = message.IndexOf('\'', fieldNameStart);
                var fieldName = message[fieldNameStart..fieldNameEnd];

                var valueStart = message.IndexOf('\'', fieldNameEnd + 1) + 1;
                var valueEnd = message.IndexOf('\'', valueStart);
                var value = message[valueStart..valueEnd];

                value = value.Replace("\"", "", StringComparison.Ordinal)
                    .Replace("\\", "", StringComparison.Ordinal);

                throw new LiteException(e.ErrorCode, $"已經存在：欄位名稱 = {fieldName}, 值 = {value}");
            }
            throw new LiteException(e.ErrorCode, message);
        }
    }
    public void Update<T, K>(in string id, in T entity, Expression<Func<T, K>> selector) where T : LiteBase
    {
        try
        {
            var oldEntity = Select<T>(id);
            if (oldEntity is not null)
            {
                var properties = entity.GetType().GetProperties();
                for (int i = default; i < properties.Length; i++)
                {
                    var property = properties[i];
                    if (!property.Name.IsMatch(nameof(LiteBase.Id)) && !property.Name.IsMatch(nameof(LiteBase.CreateTime)))
                    {
                        var oldType = oldEntity.GetType();
                        var newValue = property.GetValue(entity);
                        var oldValue = oldType.GetProperty(property.Name)!.GetValue(oldEntity);
                        switch (newValue)
                        {
                            case not null when oldValue is not null && !newValue.Equals(oldValue):
                                oldType.GetProperty(property.Name)!.SetValue(oldEntity, newValue);
                                break;

                            default:
                                oldType.GetProperty(property.Name)!.SetValue(oldEntity, newValue);
                                break;
                        }
                    }
                }
                using LiteDatabase db = new(GetConnectionString());
                var collection = db.GetCollection<T>(typeof(T).Name.ToSnakeCase());
                collection.EnsureIndex(GetIndexName(selector), selector, unique: true);
                collection.Update(oldEntity);
            }
        }
        catch (LiteException e)
        {
            var message = e.Message;
            if (message.Contains("Cannot insert duplicate key in unique index", StringComparison.Ordinal))
            {
                var fieldNameStart = message.IndexOf('\'', StringComparison.Ordinal) + 1;
                var fieldNameEnd = message.IndexOf('\'', fieldNameStart);
                var fieldName = message[fieldNameStart..fieldNameEnd];

                var valueStart = message.IndexOf('\'', fieldNameEnd + 1) + 1;
                var valueEnd = message.IndexOf('\'', valueStart);
                var value = message[valueStart..valueEnd];

                value = value.Replace("\"", "", StringComparison.Ordinal)
                    .Replace("\\", "", StringComparison.Ordinal);

                throw new LiteException(e.ErrorCode, $"已經存在：欄位名稱 = {fieldName}, 值 = {value}");
            }
            throw new LiteException(e.ErrorCode, message);
        }
    }
    public void Delete<T>(in string id) where T : LiteBase
    {
        using LiteDatabase db = new(GetConnectionString());
        var collection = db.GetCollection<T>(typeof(T).Name.ToSnakeCase());
        collection.Delete(id);
    }
    static string GetIndexName<T, K>(Expression<Func<T, K>> expression)
    {
        var members = ((NewExpression)expression.Body).Members;
        switch (members)
        {
            case not null:
                {
                    return string.Join(string.Empty, members.Select(x => x.Name));
                }

            default:
                return string.Empty;
        }
    }
    string GetConnectionString()
    {
        return LiteDB($"{nameof(LiteDB)}{FileExtension.Pile}");
        static string LiteDB(in string name) => SystemPath.Combine(FileLayout.RetentionFolderLocation, name);
    }
}