namespace DotnetNewUI.Controllers;

using DotnetNewUI.NuGet;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TemplatesController
{
    [HttpGet]
    public async Task<IReadOnlyList<CompositeTemplateManifest>> GetAsync()
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
