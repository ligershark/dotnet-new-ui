namespace DotnetNewUI.NuGet;

using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

internal static class NuGetClient
{
    private const string NuGetV3FeedEndpoint = "https://api.nuget.org/v3/index.json";
    private const string QueryResourceName = "SearchQueryService";
    private const string IconResourceName = "PackageBaseAddress/3.0.0";

    private const int PageSize = 100;

    public static async Task<NuGetPackageInfo[]> GetNuGetTemplates()
    {
        var stopWatch = Stopwatch.StartNew();

        var nuGetFeed = await GetNuGetFeed(NuGetV3FeedEndpoint);
        var queryEndpoint = nuGetFeed.GetResource(QueryResourceName);
        var iconEndpoint = nuGetFeed.GetResource(IconResourceName);

        var firstPageOfTemplates = await GetTemplates(queryEndpoint.Id);
        var remainingTemplates = await GetRemainingTemplates(queryEndpoint.Id, firstPageOfTemplates.TotalHits);

        stopWatch.Stop();

        var allTemplates = Enumerable
            .Concat(firstPageOfTemplates.Data, remainingTemplates)
            .Select(t => t with { IconUrl = GetPackageIconUrl(iconEndpoint.Id, t.Id, t.Version) })
            .ToArray();

        Console.WriteLine($"Total number of templates: {firstPageOfTemplates.TotalHits}");
        Console.WriteLine($"Total number of downloaded templates: {allTemplates.Length}");
        Console.WriteLine($"Download took {stopWatch.ElapsedMilliseconds}ms");
        Console.WriteLine();

        return allTemplates;
    }

    private static async Task<NuGetFeed> GetNuGetFeed(string url)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var feedResponse = await response.Content.ReadFromJsonAsync<NuGetFeed>();

        return feedResponse ?? throw new NullReferenceException("No response");
    }

    private static async Task<NuGetQueryResponse> GetTemplates(string queryEndpoint, int page = 0)
    {
        const string packageType = "template";

        var skip = page * PageSize;
        var take = PageSize;

        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"{queryEndpoint}?q=&packagetype={packageType}&skip={skip}&take={take}");
        var typedResponse = await response.Content.ReadFromJsonAsync<NuGetQueryResponse>();

        return typedResponse ?? throw new NullReferenceException("No response");
    }

    private static async Task<NuGetPackageInfo[]> GetRemainingTemplates(string queryEndpoint, int totalHits)
    {
        var numberOfPagesToDownload = (totalHits / PageSize) + 1;

        var pages = Enumerable
            .Range(0, numberOfPagesToDownload)
            .Skip(1)
            .Select(i => GetTemplates(queryEndpoint, i));

        var downloadedPages = await Task.WhenAll(pages);

        return downloadedPages.SelectMany(p => p.Data).ToArray();
    }

    private static string GetPackageIconUrl(string baseUrl, string packageId, string packageVersion)
        => $"{baseUrl}/{packageId}/{packageVersion}/icon";

    private static string GetAvatarIconUrl(string baseUrl, string profile)
        => $"https://www.nuget.org/profiles/{profile}/avatar?imageSize=64";
}
