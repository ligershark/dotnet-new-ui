namespace DotnetNewUI.NuGet;

public record class PackageManifest(PackageMetadata Metadata);

public record class PackageMetadata(string Id, string Version, string? Icon);
