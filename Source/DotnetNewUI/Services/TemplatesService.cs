namespace DotnetNewUI.Services;

using DotnetNewUI.NuGet;

public class TemplatesService : ITemplatesService
{
    public async Task<IReadOnlyList<CompositeTemplateManifest>> GetInstalledTemplatesAsync()
    {
        var builtInTemplatePackages = await BuiltInTemplatePackageProvider.GetAllTemplatePackagesAsync().ConfigureAwait(false);
        var installedTemplatePackages = InstalledTemplatePackageProvider.GetAllTemplatePackages();

        var builtInTemplates = builtInTemplatePackages
            .SelectMany(path => PackageInspector.GetTemplateManifestsFromPackage(path, true));

        var installedTemplates = installedTemplatePackages
            .SelectMany(path => PackageInspector.GetTemplateManifestsFromPackage(path, false));

        var manifests = Enumerable
            .Concat(builtInTemplates, installedTemplates)
            .ToList();

        return manifests;
    }

    public async Task CreateNewFromTemplateAsync(string templateShortName, string outputPath, string? name, string? language)
    {
        var arguments = new (string Name, string? Value)[]
        {
            ("output", outputPath),
            ("name", name),
            ("language", language),
        };

        var argumentsStr = string.Join(' ', arguments.Where(x => x.Value is not null).Select(x => $"--{x.Name} \"{x.Value}\""));

        await SimpleExec.Command.RunAsync("dotnet", $"new {templateShortName} {argumentsStr}").ConfigureAwait(false);
    }
}
