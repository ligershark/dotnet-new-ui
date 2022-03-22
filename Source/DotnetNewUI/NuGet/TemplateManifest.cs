namespace DotnetNewUI.NuGet;

using System.Text.Json.Serialization;

public record class TemplateManifest(
    string Identity,
    string Name,
    string Author,
    string[] Classifications,
    string Description,
    string[] ShortName)
{
    [JsonConverter(typeof(StringToStringArrayConverter))]
    public string[] ShortName { get; init; } = ShortName;
}
