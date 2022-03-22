namespace DotnetNewUI.NuGet;

internal static class NuGetPagingHelper
{
    public static int GetNumberOfPages(int numberOfItems, int pageSize)
        => (numberOfItems / pageSize) + 1;

    public static (int Skip, int Take) GetRangeOfPage(int pageNumber, int pageSize)
        => (pageNumber * pageSize, pageSize);
}
