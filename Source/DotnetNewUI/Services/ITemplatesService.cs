namespace DotnetNewUI.Services;

using DotnetNewUI.NuGet;

public interface ITemplatesService
{
    Task<IReadOnlyList<CompositeTemplateManifest>> GetInstalledTemplatesAsync();

    Task CreateNewFromTemplateAsync(string templateShortName, IReadOnlyDictionary<string, string>? parameters);
}
