namespace DotnetNewUI.Controllers;

using DotnetNewUI.NuGet;
using DotnetNewUI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PackagesController
{
    private readonly IPackagesService packagesService;

    public PackagesController(IPackagesService packagesService)
        => this.packagesService = packagesService;

    [HttpGet]
    public async Task<IReadOnlyList<NuGetPackageInfo>> GetTemplatePackagesAsync()
        => await this.packagesService.GetTemplatePackagesAsync().ConfigureAwait(false);

    [HttpPost("{packageId}")]
    public async Task InstallTemplatePackageAsync(string packageId)
        => await this.packagesService.InstallTemplatePackageAsync(packageId).ConfigureAwait(false);

    [HttpDelete("{packageId}")]
    public async Task UninstallTemplatePackageAsync(string packageId)
        => await this.packagesService.UninstallTemplatePackageAsync(packageId).ConfigureAwait(false);

    [HttpPatch("{packageId}")]
    public async Task UpdateTemplatePackageAsync(string packageId)
        => await this.packagesService.UpdateTemplatePackageAsync(packageId).ConfigureAwait(false);
}
