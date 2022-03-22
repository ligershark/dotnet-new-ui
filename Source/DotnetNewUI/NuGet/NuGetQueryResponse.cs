namespace DotnetNewUI.NuGet;

public record class NuGetQueryResponse(int TotalHits, NuGetPackageInfo[] Data);
