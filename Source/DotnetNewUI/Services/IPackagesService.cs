namespace DotnetNewUI.Services;

using DotnetNewUI.NuGet;

public interface IPackagesService
{
    Task<IReadOnlyList<NuGetPackageInfo>> GetTemplatePackagesAsync();

    Task InstallTemplatePackageAsync(string packageId);

    Task UninstallTemplatePackageAsync(string packageId);

    Task UpdateTemplatePackageAsync(string packageId);
}
