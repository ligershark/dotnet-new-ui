namespace DotnetNewUI.NuGet;

using System.Text.Json;
using System.Text.Json.Serialization;

// The 'shortName' property is inconsistent in the JSON, sometimes it's a string, sometimes it's a string array
public class StringToStringArrayConverter : JsonConverter<IReadOnlyCollection<string>>
{
    public override IReadOnlyCollection<string>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            return new[] { str! };
        }
        else if (reader.TokenType == JsonTokenType.StartArray)
        {
            var items = new List<string>();

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                reader.Read();

                if (reader.TokenType == JsonTokenType.String)
                {
                    items.Add(reader.GetString()!);
                }
            }

            return items;
        }
        else
        {
            throw new NotSupportedException("Unexpected data type. Only string or string arrays are supported.");
        }
    }

    public override void Write(Utf8JsonWriter writer, IReadOnlyCollection<string> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var item in value)
        {
            writer.WriteStringValue(item);
        }

        writer.WriteEndArray();
    }
}
