namespace DotnetNewUI.NuGet;

using System.Text.Json.Serialization;

public record class TemplateManifest(
    string Identity,
    string Name,
    string Author,
    string[] Classifications,
    string Description,
    string[] ShortName,
    TemplateTags Tags)
{
    [JsonConverter(typeof(StringToStringArrayConverter))]
    public string[] ShortName { get; init; } = ShortName;
}

public record class TemplateTags(string Language, string Type);

public record class TemplateIdeHostManifest(string? Icon);

public record CompositeTemplateManifest(
    string PackageName,
    string? Base64Icon,
    TemplateManifest TemplateManifest,
    TemplateIdeHostManifest? TemplateIdeHostManifest);
