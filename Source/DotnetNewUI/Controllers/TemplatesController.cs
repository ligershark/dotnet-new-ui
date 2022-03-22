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
    public async Task<IReadOnlyList<TemplateManifest>> GetInstalledTemplatesAsync()
    {
        var templatePackages = await BuiltInTemplatePackageProvider.GetAllTemplatePackagesAsync().ConfigureAwait(false);

        var manifests = templatePackages
            .SelectMany(path => PackageInspector.GetTemplateManifestsFromPackage(path))
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
