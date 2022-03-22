namespace DotnetNewUI.NuGet;

public interface INuGetClient
{
    Task<IReadOnlyList<NuGetPackageInfo>> GetNuGetTemplatesAsync();
}
