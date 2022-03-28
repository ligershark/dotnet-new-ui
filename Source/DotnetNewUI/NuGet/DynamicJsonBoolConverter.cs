namespace DotnetNewUI.NuGet;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

// In the DotnetCli.Host.json file, in the SymbolInfo collection the "IsHidden" property is sometimes a bool, but sometimes a string
public class DynamicJsonBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType switch
        {
            JsonTokenType.True or JsonTokenType.False => reader.GetBoolean(),
            JsonTokenType.String => bool.Parse(reader.GetString()!),
            _ => throw new NotSupportedException($"Couldn't convert value '{reader.TokenType}' to bool."),
        };

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        => writer.WriteBooleanValue(value);
}
