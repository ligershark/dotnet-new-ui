namespace DotnetNewUI.Controllers;

using DotnetNewUI.NuGet;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PackagesController
{
    private readonly INuGetClient nuGetClient;

    public PackagesController(INuGetClient nuGetClient) => this.nuGetClient = nuGetClient;

    [HttpGet]
    public async Task<IReadOnlyList<NuGetPackageInfo>> GetAsync()
    {
        var onlineTemplates = await this.nuGetClient.GetNuGetTemplatesAsync().ConfigureAwait(false);

        var installedTemplates = InstalledTemplatePackageProvider
            .GetAllTemplatePackages()
            .Select(x => PackageInspector.GetPackageNameAndVersion(x))
            .ToDictionary(x => x.PackageName, x => x.Version);

        return onlineTemplates
            .Select(x =>
            {
                if (installedTemplates.TryGetValue(x.Id, out var installedVersion))
                {
                    return x with { IsInstalled = true, InstalledVersion = installedVersion };
                }
                else
                {
                    return x with { IsInstalled = false };
                }
            })
            .ToList();
    }

    [HttpPost("{packageId}")]
    public Task InstallTemplatePackageAsync(string packageId) => throw new NotImplementedException();

    [HttpDelete("{packageId}")]
    public Task UninstallTemplatePackageAsync(string packageId) => throw new NotImplementedException();

    [HttpPatch("{packageId}")]
    public Task UpdateTemplatePackageAsync(string packageId) => throw new NotImplementedException();
}
