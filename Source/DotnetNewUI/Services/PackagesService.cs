namespace DotnetNewUI.Services;

using DotnetNewUI.NuGet;

public class PackagesService : IPackagesService
{
    private readonly INuGetClient nuGetClient;

    private Task<IReadOnlyList<NuGetPackageInfo>>? cachedOnlinePackages;

    public PackagesService(INuGetClient nuGetClient) => this.nuGetClient = nuGetClient;

    public async Task<IReadOnlyList<NuGetPackageInfo>> GetTemplatePackagesAsync()
    {
        if (this.cachedOnlinePackages is null)
        {
            this.cachedOnlinePackages = this.nuGetClient.GetNuGetTemplatesAsync();
        }

        var onlineTemplates = await this.cachedOnlinePackages.ConfigureAwait(false);

        var installedTemplates = InstalledTemplatePackageProvider
            .GetAllTemplatePackages()
            .Select(x => PackageInspector.GetPackageNameAndVersion(x))
            .ToDictionary(x => x.PackageName, x => x.Version);

        return onlineTemplates
            .Select(x => installedTemplates.TryGetValue(x.Id, out var installedVersion)
                ? x with { IsInstalled = true, InstalledVersion = installedVersion }
                : x with { IsInstalled = false })
            .ToList();
    }

    public async Task InstallTemplatePackageAsync(string packageId)
        => await DotNetCli.InstallTemplatePackageAsync(packageId).ConfigureAwait(false);

    public async Task UninstallTemplatePackageAsync(string packageId)
        => await DotNetCli.UninstallTemplatePackageAsync(packageId).ConfigureAwait(false);

    public Task UpdateTemplatePackageAsync(string packageId)
        => throw new NotImplementedException();
}
