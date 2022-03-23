namespace DotnetNewUI.Services;

using DotnetNewUI.NuGet;

public interface ITemplatesService
{
    Task<IReadOnlyList<CompositeTemplateManifest>> GetInstalledTemplatesAsync();

    Task CreateNewFromTemplateAsync(string templateShortName, string name, string outputPath);
}
