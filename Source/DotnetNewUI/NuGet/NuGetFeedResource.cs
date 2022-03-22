namespace DotnetNewUI.NuGet;

using System.Text.Json.Serialization;

public record class NuGetFeedResource(string Id, string Type)
{
    [JsonPropertyName("@id")]
    public string Id { get; } = Id;

    [JsonPropertyName("@type")]
    public string Type { get; } = Type;
}
