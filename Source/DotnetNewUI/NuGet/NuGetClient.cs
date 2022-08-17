namespace DotnetNewUI.NuGet;

using System;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class NuGetClient : INuGetClient
{
    private const int PageSize = 500;
    private readonly HttpClient httpClient;

    public NuGetClient(HttpClient httpClient)
        => this.httpClient = httpClient;

    public async Task<IReadOnlyList<NuGetPackageInfo>> GetNuGetTemplatesAsync()
    {
        var nuGetFeed = await this.GetNuGetFeedAsync(NuGetUrlHelper.NuGetV3FeedUrl).ConfigureAwait(false);
        var queryEndpoint = nuGetFeed.QueryUrl;

        var firstPageUrl = NuGetUrlHelper.GetTemplatePackageQueryFirstPageUrl(queryEndpoint, PageSize);
        var firstPage = await this.GetTemplatePackagesAsync(firstPageUrl).ConfigureAwait(false);

        var remainingPagesUrls = NuGetUrlHelper.GetTemplatePackageQueryRemainingPagesUrls(queryEndpoint, firstPage.TotalHits, PageSize);
        var remainingPages = await Task.WhenAll(remainingPagesUrls.Select(this.GetTemplatePackagesAsync)).ConfigureAwait(false);

        var allTemplates = Enumerable
            .Concat(firstPage.Data, remainingPages.SelectMany(p => p.Data))
            .Select(x => x with { NuGetUrl = NuGetUrlHelper.GetNuGetUrl(x.Id) })
            .ToList();

        return allTemplates;
    }

    private async Task<NuGetFeed> GetNuGetFeedAsync(string feedEndpoint)
    {
        var nuGetFeed = await this.httpClient.GetFromJsonAsync<NuGetFeed>(feedEndpoint).ConfigureAwait(false);
        return nuGetFeed ?? throw new InvalidOperationException("No response");
    }

    private async Task<NuGetQueryResponse> GetTemplatePackagesAsync(string url)
    {
        var queryResponse = await this.httpClient.GetFromJsonAsync<NuGetQueryResponse>(url).ConfigureAwait(false);
        return queryResponse ?? throw new InvalidOperationException("No response");
    }
}
