namespace DotnetNewUI.NuGet;

using System.Text.RegularExpressions;
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
        var packages = new List<string>();
        foreach (var templateFolder in await GetTemplateFoldersAsync().ConfigureAwait(false))
        {
            foreach (var nupkgPath in Directory.EnumerateFiles(templateFolder, "*.nupkg", SearchOption.TopDirectoryOnly))
            {
                packages.Add(nupkgPath);
            }
        }

        return packages;
    }

    private static async Task<(string DotnetRootPath, SemanticVersion SdkVersion)> GetSdkDirectoryPathAsync()
    {
        // Example output:
        //
        // 2.1.526 [C:\Program Files\dotnet\sdk]
        // 2.2.110 [C:\Program Files\dotnet\sdk]
        // 2.2.207 [C:\Program Files\dotnet\sdk]
        // 5.0.406 [C:\Program Files\dotnet\sdk]
        // 6.0.201 [C:\Program Files\dotnet\sdk]
        var (stdOut, stdErr) = await SimpleExec.Command.ReadAsync("dotnet", "--list-sdks").ConfigureAwait(false);

        var sdk = stdOut.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Last(); // Take latest SDK
        var regexMatch = Regex.Match(sdk, "^(?<version>.*) \\[(?<path>.*)\\]$");
        var version = regexMatch.Groups["version"].Value;
        var path = regexMatch.Groups["path"].Value;

        var dotnetRootPath = Path.GetDirectoryName(path); // Get to parent directory
        var sdkVersion = SemanticVersion.Parse(version);

        return (dotnetRootPath!, sdkVersion!);
    }

    private static async Task<IEnumerable<string>> GetTemplateFoldersAsync()
    {
        var templateFoldersToInstall = new List<string>();

        var (dotnetRootPath, sdkVersion) = await GetSdkDirectoryPathAsync().ConfigureAwait(false);
        var sdkPath = Path.Combine(dotnetRootPath, "sdk", sdkVersion.ToNormalizedString());

        // First grab templates from dotnet\templates\M.m folders, in ascending order, up to our version
        var templatesRootFolder = Path.GetFullPath(Path.Combine(dotnetRootPath!, "templates"));
        if (Directory.Exists(templatesRootFolder))
        {
            var parsedNames = GetVersionDirectoriesInDirectory(templatesRootFolder);
            var versionedFolders = GetBestVersionsByMajorMinor(parsedNames, sdkVersion);

            templateFoldersToInstall.AddRange(versionedFolders
                .Select(versionedFolder => Path.Combine(templatesRootFolder, versionedFolder)));
        }

        // Now grab templates from our base folder, if present.
        var templatesDir = Path.Combine(sdkPath, "Templates");
        if (Directory.Exists(templatesDir))
        {
            templateFoldersToInstall.Add(templatesDir);
        }

        return templateFoldersToInstall;
    }

    // Returns a dictionary of fileName -> Parsed version info
    // including all the directories in the input directory whose names are parse-able as versions.
    private static IReadOnlyDictionary<string, SemanticVersion> GetVersionDirectoriesInDirectory(string fullPath)
    {
        var versionFileInfo = new Dictionary<string, SemanticVersion>();

        foreach (var directory in Directory.EnumerateDirectories(fullPath, "*.*", SearchOption.TopDirectoryOnly))
        {
            if (SemanticVersion.TryParse(Path.GetFileName(directory), out var versionInfo))
            {
                versionFileInfo.Add(directory, versionInfo);
            }
        }

        return versionFileInfo;
    }

    private static IList<string> GetBestVersionsByMajorMinor(IReadOnlyDictionary<string, SemanticVersion> versionDirInfo, SemanticVersion sdkVersion)
    {
        var bestVersionsByBucket = new Dictionary<string, (string Path, SemanticVersion Version)>();

        foreach (var dirInfo in versionDirInfo)
        {
            var majorMinorDirVersion = new Version(dirInfo.Value.Major, dirInfo.Value.Minor);
            var majorMinorSdkVersion = new Version(sdkVersion.Major, sdkVersion.Minor);

            // restrict the results to not include from higher versions of the runtime/templates then the SDK
            if (majorMinorDirVersion <= majorMinorSdkVersion)
            {
                var coreAppVersion = $"{dirInfo.Value.Major}.{dirInfo.Value.Minor}";
                if (!bestVersionsByBucket.TryGetValue(coreAppVersion, out var currentHighest)
                    || dirInfo.Value.CompareTo(currentHighest.Version) > 0)
                {
                    bestVersionsByBucket[coreAppVersion] = (dirInfo.Key, dirInfo.Value);
                }
            }
        }

        return bestVersionsByBucket.OrderBy(x => x.Key).Select(x => x.Value.Path).ToList();
    }
}
