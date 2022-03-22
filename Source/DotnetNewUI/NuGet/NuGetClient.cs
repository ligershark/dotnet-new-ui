namespace DotnetNewUI.NuGet;

using System;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

internal static class NuGetClient
{
    private const int PageSize = 100;
    private static readonly HttpClient HttpClient = new();

    public static async Task<NuGetPackageInfo[]> GetNuGetTemplates()
    {
        var nuGetFeed = await GetNuGetFeed(NuGetUrlHelper.NuGetV3FeedUrl);
        var queryEndpoint = nuGetFeed.QueryUrl;
        var iconEndpoint = nuGetFeed.PackageIconUrl;

        var firstPageUrl = NuGetUrlHelper.GetTemplatePackageQueryFirstPageUrl(queryEndpoint, PageSize);
        var firstPage = await GetPackages(firstPageUrl);

        var remainingPagesUrls = NuGetUrlHelper.GetTemplatePackageQueryRemainingPagesUrls(queryEndpoint, firstPage.TotalHits, PageSize);
        var remainingPages = await Task.WhenAll(remainingPagesUrls.Select(url => GetPackages(url)));

        var allTemplates = Enumerable
            .Concat(firstPage.Data, remainingPages.SelectMany(p => p.Data))
            .Select(t => t with { IconUrl = NuGetUrlHelper.GetPackageIconUrl(iconEndpoint, t.Id, t.Version) })
            .ToArray();

        return allTemplates;
    }

    private static async Task<NuGetFeed> GetNuGetFeed(string feedEndpoint)
    {
        var nuGetFeed = await HttpClient.GetFromJsonAsync<NuGetFeed>(feedEndpoint);
        return nuGetFeed ?? throw new NullReferenceException("No response");
    }

    private static async Task<NuGetQueryResponse> GetPackages(string url)
    {
        var queryResponse = await HttpClient.GetFromJsonAsync<NuGetQueryResponse>(url);
        return queryResponse ?? throw new NullReferenceException("No response");
    }
}

internal static class NuGetUrlHelper
{
    public const string NuGetV3FeedUrl = "https://api.nuget.org/v3/index.json";

    public static string GetTemplatePackageQueryUrl(string queryEndpoint, int skip, int take)
        => $"{queryEndpoint}?q=&skip={skip}&take={take}&packagetype=template";

    public static string GetTemplatePackageQueryFirstPageUrl(string queryEndpoint, int pageSize)
        => GetTemplatePackageQueryUrl(queryEndpoint, 0, pageSize);

    public static string[] GetTemplatePackageQueryRemainingPagesUrls(string queryEndpoint, int totalNumberOfItems, int pageSize)
    {
        var numberOfPagesToDownload = NuGetPagingHelper.GetNumberOfPages(totalNumberOfItems, pageSize);

        var pageUrls = Enumerable
            .Range(0, numberOfPagesToDownload)
            .Skip(1)
            .Select(pageNumber =>
            {
                var (skip, take) = NuGetPagingHelper.GetRangeOfPage(pageNumber, pageSize);
                return GetTemplatePackageQueryUrl(queryEndpoint, skip, take);
            })
            .ToArray();

        return pageUrls;
    }

    public static string GetPackageIconUrl(string baseUrl, string packageId, string packageVersion)
        => $"{baseUrl}/{packageId}/{packageVersion}/icon";

    public static string GetAvatarIconUrl(string profile)
        => $"https://www.nuget.org/profiles/{profile}/avatar?imageSize=64";
}

internal static class NuGetPagingHelper
{
    public static int GetNumberOfPages(int numberOfItems, int pageSize)
        => (numberOfItems / pageSize) + 1;

    public static (int Skip, int Take) GetRangeOfPage(int pageNumber, int pageSize)
        => (pageNumber * pageSize, pageSize);
}
