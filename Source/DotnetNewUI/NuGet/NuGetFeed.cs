namespace DotnetNewUI.NuGet;

public record class NuGetFeed(NuGetFeedResource[] Resources)
{
    public string QueryUrl => this.Resources.First(r => r.Type == "SearchQueryService").Id;

    public string PackageIconUrl => this.Resources.First(r => r.Type == "PackageBaseAddress/3.0.0").Id;
}
