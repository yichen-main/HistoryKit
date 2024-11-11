namespace Eywa.Core.Architects.Primaries.Substances;
public sealed class EnumConvert : JsonConverter<Enum>
{
    public override void Write(Utf8JsonWriter writer, Enum value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Convert.ToInt32(value, CultureInfo.CurrentCulture));
    }
    public override Enum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return (Enum)Enum.ToObject(typeToConvert, reader.GetInt32());
    }
}