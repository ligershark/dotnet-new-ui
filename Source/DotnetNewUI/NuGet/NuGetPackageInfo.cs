namespace DotnetNewUI.NuGet;

public record class NuGetPackageInfo(
    string Id,
    string Version,
    int TotalDownloads,
    bool Verified,
    string[] Authors,
    string[] Owners,
    string Title,
    string Description,
    string Summary,
    string[] Tags,
    string ProjectUrl,
    string? IconUrl);
