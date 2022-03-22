namespace DotnetNewUI.NuGet;

using System.Text.Json.Serialization;

internal record class NuGetFeed(NuGetFeedResource[] Resources)
{
    public NuGetFeedResource GetResource(string type) => this.Resources.First(x => x.Type == type);
}

internal record class NuGetFeedResource(string Id, string Type)
{
    [JsonPropertyName("@id")]
    public string Id { get; } = Id;

    [JsonPropertyName("@type")]
    public string Type { get; } = Type;
}

internal record class NuGetQueryResponse(int TotalHits, NuGetPackageInfo[] Data);

internal record class NuGetPackageInfo
(
    string Id,
    string Version,
    int TotalDownloads,
    bool Verified,
    string[] Author,
    string[] Owners,
    string Title,
    string Description,
    string Summary,
    string[] Tags,
    string ProjectUrl,
    string? IconUrl
);