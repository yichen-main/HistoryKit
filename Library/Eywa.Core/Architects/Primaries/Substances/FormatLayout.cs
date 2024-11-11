namespace Eywa.Core.Architects.Primaries.Substances;
public readonly ref struct FormatLayout
{
    public static JsonSerializerOptions SerialOption(in bool indented = false) => new()
    {
        MaxDepth = 100,
        WriteIndented = indented,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new DateConvert(), new EnumConvert() },
    };
}