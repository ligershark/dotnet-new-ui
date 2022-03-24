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

        var manifests = Enumerable
            .Concat(builtInTemplates, installedTemplates)
            .ToList();

        return manifests;
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
}
