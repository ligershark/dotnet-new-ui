namespace DotnetNewUI.Controllers;

using DotnetNewUI.NuGet;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TemplatesController
{
    private readonly INuGetClient nuGetClient;

    public TemplatesController(INuGetClient nuGetClient) => this.nuGetClient = nuGetClient;

    // Returns template packages (one package might contain multiple templates)
    [HttpGet("online")]
    public async Task<IReadOnlyList<NuGetPackageInfo>> GetOnlineTemplatePackagesAsync() =>
        await this.nuGetClient.GetNuGetTemplatesAsync().ConfigureAwait(false);

    // Returns templates (multiple templates might belong to the same package)
    [HttpGet("installed")]
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

    [HttpPost("installed/{packageId}")]
    public Task InstallTemplatePackageAsync(string packageId) => throw new NotImplementedException();

    [HttpDelete("installed/{packageId}")]
    public Task UninstallTemplatePackageAsync(string packageId) => throw new NotImplementedException();

    [HttpPatch("installed/{packageId}")]
    public Task UpdateTemplatePackageAsync(string packageId) => throw new NotImplementedException();
}
