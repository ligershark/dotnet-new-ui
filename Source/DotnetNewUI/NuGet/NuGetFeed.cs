namespace DotnetNewUI.NuGet;

public record class NuGetFeed(NuGetFeedResource[] Resources)
{
    public string QueryUrl => this.Resources.First(r => r.Type == "SearchQueryService").Id;
}
