namespace Eywa.Core.Architects.Primaries.Substances;
public readonly ref struct LiteLayout
{
    public static T Select<T>(in string id) where T : LiteBase
    {
        using var database = GetDatabase<T>();
        var name = typeof(T).Name.ToSnakeCase();
        var collection = database.GetCollection<T>(name);
        return collection.FindById(id);
    }
    public static IEnumerable<T> Select<T>() where T : LiteBase
    {
        using var database = GetDatabase<T>();
        var name = typeof(T).Name.ToSnakeCase();
        var collection = database.GetCollection<T>(name);
        return collection.FindAll();
    }
    public static void Insert<T>(in T entity) where T : LiteBase
    {
        using var database = GetDatabase<T>();
        var name = typeof(T).Name.ToSnakeCase();
        var collection = database.GetCollection<T>(name);
        foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<UniqueIndexAttribute>() is not null))
        {
            collection.EnsureIndex(property.Name, unique: true);
        }
        collection.Insert(entity);
    }
    public static void Insert<T, K>(in T entity, in Expression<Func<T, K>> keySelector) where T : LiteBase
    {
        using var database = GetDatabase<T>();
        var name = typeof(T).Name.ToSnakeCase();
        var collection = database.GetCollection<T>(name);
        collection.EnsureIndex(keySelector, unique: true);
        foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<UniqueIndexAttribute>() is not null))
        {
            collection.EnsureIndex(property.Name, unique: true);
        }
        collection.Insert(entity);
    }
    public static void Update<T>(in string id, in T entity) where T : LiteBase
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
            using var database = GetDatabase<T>();
            var name = typeof(T).Name.ToSnakeCase();
            var collection = database.GetCollection<T>(name);
            collection.Update(oldEntity);
        }
    }
    public static void Update<T, K>(in string id, in T entity, in Expression<Func<T, K>> keySelector) where T : LiteBase
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
            using var database = GetDatabase<T>();
            var name = typeof(T).Name.ToSnakeCase();
            var collection = database.GetCollection<T>(name);
            collection.EnsureIndex(keySelector, unique: true);
            collection.Update(oldEntity);
        }
    }
    public static void Delete<T>(in string id) where T : LiteBase
    {
        using var database = GetDatabase<T>();
        var name = typeof(T).Name.ToSnakeCase();
        var collection = database.GetCollection<T>(name);
        collection.Delete(id);
    }
    static LiteDatabase GetDatabase<T>() where T : class
    {
        return new($"{LiteDB($"{FileLayout.GetProjectName(typeof(T))}{FileExtension.Pile}")}");
        static string LiteDB(in string name) => Path.Combine(FileLayout.RetentionFolderLocation, name);
    }
}