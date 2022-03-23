namespace DotnetNewUI.NuGet;

internal static class InstalledTemplatePackageProvider
{
    public static IReadOnlyList<string> GetAllTemplatePackages()
    {
        var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var templatesRelativePath = ".templateengine/packages";
        var templatesFolder = Path.Combine(userFolder, templatesRelativePath);

        return Directory.EnumerateFiles(templatesFolder, "*.nupkg", SearchOption.TopDirectoryOnly).ToList();
    }
}
