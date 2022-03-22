namespace DotnetNewUI.NuGet;

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

    public static string GetAvatarIconUrl(string profile)
        => $"https://www.nuget.org/profiles/{profile}/avatar?imageSize=64";
}
