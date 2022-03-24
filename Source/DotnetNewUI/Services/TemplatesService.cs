namespace DotnetNewUI.Services;

using DotnetNewUI.NuGet;

public class TemplatesService : ITemplatesService
{
    public async Task<IReadOnlyList<CompositeTemplateManifest>> GetInstalledTemplatesAsync()
    {
        var builtInTemplatePackages = await BuiltInTemplatePackageProvider.GetAllTemplatePackagesAsync().ConfigureAwait(false);
        var installedTemplatePackages = InstalledTemplatePackageProvider.GetAllTemplatePackages();

        var builtInTemplates = builtInTemplatePackages
            .SelectMany(path => PackageInspector.GetTemplateManifestsFromPackage(path, true));

        var installedTemplates = installedTemplatePackages
            .SelectMany(path => PackageInspector.GetTemplateManifestsFromPackage(path, false));

        return TemplatesServiceHelper.MergeResults(builtInTemplates, installedTemplates).ToList();
    }

    public async Task CreateNewFromTemplateAsync(string templateShortName, string outputPath, string? name, string? language)
    {
        var arguments = new Dictionary<string, string?>
        {
            { "output", outputPath },
            { "name", name },
            { "language", language },
        };

        await DotNetCli.CreateNewFromTemplateAsync(templateShortName, arguments).ConfigureAwait(false);
    }

    internal static class TemplatesServiceHelper
    {
        public static IEnumerable<CompositeTemplateManifest> MergeResults(IEnumerable<CompositeTemplateManifest> builtInTemplates, IEnumerable<CompositeTemplateManifest> installedTemplates)
            => Enumerable
                .Concat(builtInTemplates, installedTemplates)

                // Deduping templates with different versions and languages
                .GroupBy(x => x.TemplateManifest.GroupIdentity)
                .Select(g =>
                {
                    // Select templates of latest version
                    var manifestsOfLatestVersion = g
                        .GroupBy(x => x.Version)
                        .MaxBy(x => x.Key)!
                        .ToList();

                    var baseLine = manifestsOfLatestVersion.First();

                    // Aggregate Language tags
                    var languages = manifestsOfLatestVersion
                        .Select(x => x.TemplateManifest.Tags?.Language)
                        .Where(x => x is not null)
                        .Select(x => x!)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToArray();

                    return baseLine with { Languages = languages };
                });
    }
}
