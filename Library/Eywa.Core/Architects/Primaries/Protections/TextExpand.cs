namespace Eywa.Core.Architects.Primaries.Protections;
public static class TextExpand
{
    public static string LetConvertPath(this string input) => input.ToSnakeCase('-');
    public static string ToSnakeCase(this string input, in char delimiter = '_')
    {
        StringBuilder result = new();
        if (string.IsNullOrEmpty(input)) return input;
        result.Append(char.ToLowerInvariant(input[default]));
        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                result.Append(delimiter);
                result.Append(char.ToLowerInvariant(input[i]));
            }
            else result.Append(input[i]);
        }
        return result.ToString();
    }
    public static bool TryParse(this string timeString, out DateTime startTime, out DateTime endTime, in string timeFormat = "yyyyMMddHHmmss")
    {
        endTime = DateTime.MinValue;
        startTime = DateTime.MinValue;
        var startString = timeString[..14];
        var endString = timeString.Substring(15, 14);
        if (timeString.Length != 29 || timeString[14] != '@') return false;
        if (DateTime.TryParseExact(startString, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out startTime) &&
            DateTime.TryParseExact(endString, timeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out endTime)) return true;
        return false;
    }
    public static string[] RemoveDuplicates(this string[] array)
    {
        HashSet<string> set = new(array, StringComparer.Ordinal);
        return new List<string>(set).ToArray();
    }
    public static void Error(this Exception e, in Type type)
    {
        Log.Error(LogTemplate, type.Name, e.GetLine(), e.Message);
    }
    public static void Fatal(this Exception e, in Type type)
    {
        Log.Fatal(LogTemplate, type.Name, e.GetLine(), e.Message);
    }
    public static void Error<T>(this Exception e, in Type type, params T[] properties)
    {
        Log.Error(LogsTemplate, type.Name, e.GetLine(), e.Message, properties);
    }
    public static void Fatal<T>(this Exception e, in Type type, params T[] properties)
    {
        Log.Fatal(LogsTemplate, type.Name, e.GetLine(), e.Message, properties);
    }
    static string LogTemplate => "[{@Type}] ({@Line}) {@Message}";
    static string LogsTemplate => "[{@Type}] ({@Line}) {@Message} {@Property}";
}