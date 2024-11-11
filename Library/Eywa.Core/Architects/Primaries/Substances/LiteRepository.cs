namespace Eywa.Core.Architects.Primaries.Substances;
internal class LiteRepository<T>(LiteDatabase database) where T : LiteBase
{
    readonly string name = typeof(T).Name.ToSnakeCase();
    public virtual T Get(in string id)
    {
        var collection = database.GetCollection<T>(name);
        return collection.FindById(id);
    }
    public virtual IEnumerable<T> Get()
    {
        var collection = database.GetCollection<T>(name);
        return collection.FindAll();
    }
    public virtual void Add(in T entity)
    {
        var collection = database.GetCollection<T>(name);
        foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<UniqueIndexAttribute>() is not null))
        {
            collection.EnsureIndex(property.Name, unique: true);
        }
        collection.Insert(entity);
    }
    public virtual void Add<K>(in T entity, in Expression<Func<T, K>> keySelector)
    {
        var collection = database.GetCollection<T>(name);
        collection.EnsureIndex(keySelector, unique: true);
        foreach (var property in typeof(T).GetProperties().Where(x => x.GetCustomAttribute<UniqueIndexAttribute>() is not null))
        {
            collection.EnsureIndex(property.Name, unique: true);
        }
        collection.Insert(entity);
    }
    public virtual void Put(in string id, in T entity)
    {
        var oldEntity = Get(id);
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
            var collection = database.GetCollection<T>(name);
            collection.Update(oldEntity);
        }
    }
    public virtual void Put<K>(in string id, in T entity, in Expression<Func<T, K>> keySelector)
    {
        var oldEntity = Get(id);
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
            var collection = database.GetCollection<T>(name);
            collection.EnsureIndex(keySelector, unique: true);
            collection.Update(oldEntity);
        }
    }
    public virtual void Clear(in string id)
    {
        var collection = database.GetCollection<T>(name);
        collection.Delete(id);
    }
}