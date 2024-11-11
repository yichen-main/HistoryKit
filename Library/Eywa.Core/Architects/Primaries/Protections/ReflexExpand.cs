using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Eywa.Core.Architects.Primaries.Protections;
public static class ReflexExpand
{
    public static Type[] GetAssemblyTypes(this Type type)
    {
        var assembly = Assembly.GetAssembly(type);
        return assembly is null ? [] : assembly.GetTypes();
    }
    public static Type[] GetAssemblyTypes<T>() => typeof(T).GetAssemblyTypes();
    public static IEnumerable<Type> GetAssemblyTypes<T1, T2>() => GetAssemblyTypes<T1>().Where(x => x.BaseType == typeof(T2));
    public static IEnumerable<Type> GetAssemblyTypes<T>(this Type type) => type.GetAssemblyTypes().Where(x => x.BaseType == typeof(T));
    public static IEnumerable<Type> GetSpecificTypes<T>(this Type type) where T : Attribute => type.Assembly.GetSpecificTypes<T>();
    public static IEnumerable<Type> GetSpecificTypes<T>(this Type[] types) where T : Attribute => types.Where(x => x.GetCustomAttributes<T>().Any()) ?? [];
    public static IEnumerable<Type> GetSpecificTypes<T>(this Assembly assembly) where T : Attribute => assembly.GetTypes().GetSpecificTypes<T>();
    public static IEnumerable<Stream> GetDialectResourceStreams(this Assembly assembly)
    {
        foreach (var name in assembly.GetManifestResourceNames().Where(x => x.EndsWith(FileExtension.Jaon, StringComparison.OrdinalIgnoreCase)))
        {
            var stream = assembly.GetManifestResourceStream(name);
            if (stream is not null) yield return stream;
        }
    }
    public static IEnumerable<Type> GetSubclasses<T>(this Type parentType) => typeof(T).GetAssemblyTypes().Where(x => x.IsSubclassOf(parentType));
    public static IEnumerable<Type> GetSubInterfaces<T>(this Type parentType) => typeof(T).GetAssemblyTypes().Where(x =>
    x.IsClass && !x.IsAbstract && x.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == parentType));
    public static string GetDesc(this Enum @enum) => @enum.GetType().GetDesc(@enum.ToString());
    public static string GetDesc(this Type type, in string name) => type.GetRuntimeField(name)!.GetCustomAttribute<DescriptionAttribute>()!.Description;
    public static string ToJson<T>(this T content, in bool indented = true) => JsonSerializer.Serialize(content, typeof(T), FormatLayout.SerialOption(indented));
    public static T? ToObject<T>(this string content)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(content, FormatLayout.SerialOption());
        }
        catch
        {
            return default;
        }
    }
    public static IDictionary<string, object>? ToDictionary(this JsonNode node) => node.Deserialize<Dictionary<string, object>>();
    public static string? ToValue(this JsonNode node, string name) => node.ToDictionary()?.FirstOrDefault(x => x.Key.IsMatch(name)).Value?.ToString();
    public static DateTime ToLocalTime(this DateTime dateTime, in int? offset) => dateTime != default ? dateTime.AddHours(offset.GetValueOrDefault()) : default;
    public static T ToGenerics<T>(this string? value) => value is null ? Activator.CreateInstance<T>() : (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
    public static string GetString(this ArraySegment<byte> bytes) => Encoding.UTF8.GetString(bytes);
    public static int GetLine(this Exception e)
    {
        if (e.StackTrace is not null)
        {
            var line = e.StackTrace.Split(Environment.NewLine)[^1];
            var index = line.IndexOf(nameof(line), StringComparison.Ordinal);
            if (int.TryParse(line[(index + 4)..], CultureInfo.CurrentCulture, out var value)) return value;
        }
        return Timeout.Infinite;
    }
}