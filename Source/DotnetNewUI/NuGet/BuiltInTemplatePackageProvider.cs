namespace DotnetNewUI.NuGet;

using DotnetNewUI.Services;
using global::NuGet.Versioning;

/// <summary>
/// Returns list of *.nupkg files from C:\Program Files\dotnet\templates\x.x.x.x\ (on Windows) to be installed.
/// <para>
/// Based on <see href="https://github.com/dotnet/sdk/blob/main/src/Cli/dotnet/commands/dotnet-new/BuiltInTemplatePackageProvider.cs">'dotnet new' source code</see>.
/// </para>
/// </summary>
internal static class BuiltInTemplatePackageProvider
{
    public static async Task<IReadOnlyList<string>> GetAllTemplatePackagesAsync()
    {
        var templateFolders = await GetTemplateFoldersAsync().ConfigureAwait(false);

        return templateFolders
            .SelectMany(folder => Directory.EnumerateFiles(folder, "*.nupkg", SearchOption.TopDirectoryOnly))
            .ToList();
    }

    private static async Task<IEnumerable<string>> GetTemplateFoldersAsync()
    {
        var sdks = await DotNetCli.ListSdksAsync().ConfigureAwait(false);
        var currentSdkVersion = await DotNetCli.GetSdkVersionAsync().ConfigureAwait(false);

        var (sdkVersion, sdkInstallDir) = sdks.Single(x => x.SdkVersion == currentSdkVersion);
        var dotnetRootPath = Path.GetDirectoryName(sdkInstallDir)!;

        var templateFolders = new List<string>();

        var globalTemplateFolders = GetGlobalTemplateFolders(dotnetRootPath, sdkVersion);
        if (globalTemplateFolders is not null)
        {
            templateFolders.AddRange(globalTemplateFolders);
        }

        var sdkTemplateFolder = GetSdkTemplateFolder(dotnetRootPath, sdkVersion);
        if (sdkTemplateFolder is not null)
        {
            templateFolders.Add(sdkTemplateFolder);
        }

        return templateFolders;
    }

    private static IEnumerable<string>? GetGlobalTemplateFolders(string dotnetRootPath, SemanticVersion sdkVersion)
    {
        var templatesRootFolder = Path.Combine(dotnetRootPath, "templates");

        if (Directory.Exists(templatesRootFolder))
        {
            var templateDirs = Directory
                .EnumerateDirectories(templatesRootFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Select(path => (Path: path, Version: SemanticVersion.Parse(Path.GetFileName(path))))
                .OrderBy(x => x.Version)
                .TakeWhile(x => x.Version <= sdkVersion)
                .GroupBy(x => new Version(x.Version.Major, x.Version.Minor))
                .Select(g => g.Last())
                .Select(x => x.Path);

            return templateDirs;
        }

        return null;
    }

    private static string? GetSdkTemplateFolder(string dotnetRootPath, SemanticVersion sdkVersion)
    {
        var templatesDir = Path.Combine(dotnetRootPath, "sdk", sdkVersion.ToNormalizedString(), "templates");

        if (Directory.Exists(templatesDir))
        {
            return templatesDir;
        }

        return null;
    }
}
