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

        var builtInTemplates = await BuiltInTemplatePackageProvider.GetAllTemplatePackagesAsync().ConfigureAwait(false);
        var builtInTemplateDictionary = builtInTemplates
            .Select(x => PackageInspector.GetPackageNameAndVersion(x))
            .GroupBy(x => x.PackageName)
            .ToDictionary(g => g.Key.ToLowerInvariant(), g => g.Select(x => x.Version).Max());

        var installedTemplateDictionary = InstalledTemplatePackageProvider
            .GetAllTemplatePackages()
            .Select(x => PackageInspector.GetPackageNameAndVersion(x))
            .GroupBy(x => x.PackageName)
            .ToDictionary(g => g.Key.ToLowerInvariant(), g => g.Select(x => x.Version).Max());

        return onlineTemplates
            .Select(x =>
            {
                if (builtInTemplateDictionary.TryGetValue(x.Id.ToLowerInvariant(), out var builtInVersion))
                {
                    return x with { IsInstalled = true, InstalledVersion = builtInVersion, IsBuiltIn = true };
                }
                else if (installedTemplateDictionary.TryGetValue(x.Id.ToLowerInvariant(), out var installedVersion))
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

    public async Task InstallTemplatePackageAsync(string packageId)
        => await DotNetCli.InstallTemplatePackageAsync(packageId).ConfigureAwait(false);

    public async Task UninstallTemplatePackageAsync(string packageId)
        => await DotNetCli.UninstallTemplatePackageAsync(packageId).ConfigureAwait(false);

    public Task UpdateTemplatePackageAsync(string packageId)
        => throw new NotImplementedException();
}
