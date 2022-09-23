namespace DotnetNewUI.Services;

using DotnetNewUI.NuGet;

public class PackagesService : IPackagesService
{
    private readonly INuGetClient nuGetClient;
    private readonly DotNetCli dotNetCli;
    private Task<IReadOnlyList<NuGetPackageInfo>>? cachedOnlinePackages;

    public PackagesService(INuGetClient nuGetClient, DotNetCli dotNetCli)
    {
        this.nuGetClient = nuGetClient;
        this.dotNetCli = dotNetCli;
    }

    public async Task<IReadOnlyList<NuGetPackageInfo>> GetTemplatePackagesAsync()
    {
        this.cachedOnlinePackages ??= this.nuGetClient.GetNuGetTemplatesAsync();

        var onlineTemplatePackages = await this.cachedOnlinePackages.ConfigureAwait(false);
        var builtInTemplatePackages = await BuiltInTemplatePackageProvider.GetAllTemplatePackagesAsync(this.dotNetCli).ConfigureAwait(false);
        var installedTemplatePackages = InstalledTemplatePackageProvider.GetAllTemplatePackages();

        return PackageServiceHelper.MergeResults(onlineTemplatePackages, builtInTemplatePackages, installedTemplatePackages);
    }

    public async Task InstallTemplatePackageAsync(string packageId)
        => await this.dotNetCli.InstallTemplatePackageAsync(packageId).ConfigureAwait(false);

    public async Task UninstallTemplatePackageAsync(string packageId)
        => await this.dotNetCli.UninstallTemplatePackageAsync(packageId).ConfigureAwait(false);

    public Task UpdateTemplatePackageAsync(string packageId)
        => throw new NotImplementedException();

    internal static class PackageServiceHelper
    {
        public static IReadOnlyList<NuGetPackageInfo> MergeResults(IReadOnlyList<NuGetPackageInfo> onlinePackages, IReadOnlyList<string> builtInPackagePaths, IReadOnlyList<string> installedPackagePaths)
        {
            var builtInTemplateDictionary = builtInPackagePaths
                .Select(x => PackageInspector.PackageInspectorHelper.GetPackageNameAndVersion(x))
                .GroupBy(x => x.PackageName)
                .ToDictionary(g => g.Key.ToLowerInvariant(), g => g.Select(x => x.Version).Max());

            var installedTemplateDictionary = installedPackagePaths
                .Select(x => PackageInspector.PackageInspectorHelper.GetPackageNameAndVersion(x))
                .GroupBy(x => x.PackageName)
                .ToDictionary(g => g.Key.ToLowerInvariant(), g => g.Select(x => x.Version).Max());

            return onlinePackages
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
    }
}
