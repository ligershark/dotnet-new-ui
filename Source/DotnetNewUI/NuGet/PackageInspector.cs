namespace DotnetNewUI.NuGet;

using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

public static class PackageInspector
{
    public static IReadOnlyList<TemplateManifest> GetTemplateManifestsFromPackage(string packagePath)
    {
        using var file = File.OpenRead(packagePath);
        var package = new ZipArchive(file);

        // Example: "content/ViewImports/.template.config/template.json"
        var regex = new Regex("^(content/)?(?<template>.*)/\\.template\\.config/template\\.json$");

        var templateManifests = package.Entries
            .Select(x => regex.Match(x.FullName) is { Success: true } ? x : null)
            .Where(x => x is not null)
            .Select(x => ReadZipArchiveEntry(x!))
            .ToList();

        return templateManifests;
    }

    private static TemplateManifest ReadZipArchiveEntry(ZipArchiveEntry entry)
    {
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        using var entryStream = entry.Open();
        var result = JsonSerializer.Deserialize<TemplateManifest>(entryStream, jsonSerializerOptions);
        return result!;
    }
}

public record class TemplateManifest
(
    string Identity,
    string Name,
    string Author,
    string[] Classifications,
    string Description,
    string[] ShortName
)
{
    [JsonConverter(typeof(StringToStringArrayConverter))]
    public string[] ShortName { get; init; } = ShortName;
}

// The 'shortName' property is inconsistent in the JSON, sometimes it's a string, sometimes it's a string array
public class StringToStringArrayConverter : JsonConverter<string[]>
{
    public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

            return items.ToArray();
        }
        else
        {
            throw new NotSupportedException("Unexpected data type. Only string or string arrays are supported.");
        }
    }

    public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var item in value)
        {
            writer.WriteStringValue(item);
        }

        writer.WriteEndArray();
    }
}
