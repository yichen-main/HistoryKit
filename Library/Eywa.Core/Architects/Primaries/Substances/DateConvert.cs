namespace Eywa.Core.Architects.Primaries.Substances;
public class DateConvert : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType is JsonTokenType.String && DateTime.TryParse(reader.GetString(),
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) ? result : reader.GetDateTime();
    }
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture));
}