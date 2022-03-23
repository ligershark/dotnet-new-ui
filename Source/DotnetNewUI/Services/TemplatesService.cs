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
}
