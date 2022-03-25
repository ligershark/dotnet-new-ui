namespace DotnetNewUI.NuGet;

using DotnetNewUI.Services;
using global::NuGet.Versioning;

/// <summary>
/// Returns list of *.nupkg files from C:\Program Files\dotnet\templates\x.x.x.x\ (on Windows) to be installed.
/// <para>
/// Inspired by <see href="https://github.com/dotnet/sdk/blob/main/src/Cli/dotnet/commands/dotnet-new/BuiltInTemplatePackageProvider.cs">'dotnet new' source code</see>.
/// </para>
/// </summary>
internal static class BuiltInTemplatePackageProvider
{
    public static async Task<IReadOnlyList<string>> GetAllTemplatePackagesAsync(DotNetCli dotNetCli)
    {
        var (sdkVersion, sdkInstallDir) = await GetCurrentSdkInfoAsync(dotNetCli).ConfigureAwait(false);

        return GetTemplateFolders(sdkVersion, sdkInstallDir)
            .Where(folder => Directory.Exists(folder))
            .SelectMany(folder => Directory.EnumerateFiles(folder, "*.nupkg", SearchOption.TopDirectoryOnly))
            .ToList();
    }

    private static async Task<(SemanticVersion Version, string InstallDir)> GetCurrentSdkInfoAsync(DotNetCli dotNetCli)
    {
        var sdks = await dotNetCli.ListSdksAsync().ConfigureAwait(false);
        var currentSdkVersion = await dotNetCli.GetSdkVersionAsync().ConfigureAwait(false);
        return sdks.Single(x => x.SdkVersion == currentSdkVersion);
    }

    private static IEnumerable<string> GetTemplateFolders(SemanticVersion sdkVersion, string sdkInstallDir)
    {
        var dotnetRootDir = BuiltInTemplatePackageProviderHelper.GetDotnetRootDirectory(sdkInstallDir);

        var templateFolders = new List<string>();

        var globalTemplateFolders = GetGlobalTemplateFolders(dotnetRootDir, sdkVersion);
        if (globalTemplateFolders is not null)
        {
            templateFolders.AddRange(globalTemplateFolders);
        }

        templateFolders.Add(BuiltInTemplatePackageProviderHelper.GetSdkTemplateDir(dotnetRootDir, sdkVersion));

        return templateFolders;
    }

    private static IEnumerable<string>? GetGlobalTemplateFolders(string dotnetRootDir, SemanticVersion sdkVersion)
    {
        var templatesRootDir = BuiltInTemplatePackageProviderHelper.GetGlobalTemplatesRootDir(dotnetRootDir);

        if (Directory.Exists(templatesRootDir))
        {
            var templateVersionDirs = Directory.EnumerateDirectories(templatesRootDir, "*.*", SearchOption.TopDirectoryOnly);
            return BuiltInTemplatePackageProviderHelper.SelectAppropriateTemplateDirs(templateVersionDirs, sdkVersion);
        }

        return null;
    }

    internal static class BuiltInTemplatePackageProviderHelper
    {
        public static string GetDotnetRootDirectory(string sdkInstallDir)
            => Path.GetDirectoryName(sdkInstallDir)!;

        public static string GetGlobalTemplatesRootDir(string dotnetRootDir)
            => Path.Combine(dotnetRootDir, "templates");

        public static string GetSdkTemplateDir(string dotnetRootDir, SemanticVersion sdkVersion)
            => Path.Combine(dotnetRootDir, "sdk", sdkVersion.ToNormalizedString(), "templates");

        public static IEnumerable<string> SelectAppropriateTemplateDirs(IEnumerable<string> templateVersionDirs, SemanticVersion sdkVersion)
            => templateVersionDirs
                .Select(dir => (Dir: dir, Version: SemanticVersion.Parse(Path.GetFileName(dir))))
                .OrderBy(x => x.Version)
                .TakeWhile(x => x.Version <= sdkVersion)
                .GroupBy(x => new Version(x.Version.Major, x.Version.Minor))
                .Select(g => g.Last())
                .Select(x => x.Dir);
    }
}
