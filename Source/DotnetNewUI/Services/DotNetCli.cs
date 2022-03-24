namespace DotnetNewUI.Services;
using System.Text.RegularExpressions;
using global::NuGet.Versioning;
using SimpleExec;

public class DotNetCli
{
    private readonly ILogger<DotNetCli> logger;

    public DotNetCli(ILogger<DotNetCli> logger) => this.logger = logger;

    public async Task<IReadOnlyList<(SemanticVersion SdkVersion, string ParentDirectory)>> ListSdksAsync()
    {
        var name = "dotnet";
        var arguments = "--list-sdks";
        this.logger.Executed(name, arguments);
        var (output, _) = await Command.ReadAsync(name, arguments).ConfigureAwait(false);
        return DotNetCliHelper.ParseDotNetListSdksOutput(output.Trim());
    }

    public async Task<SemanticVersion> GetSdkVersionAsync()
    {
        var name = "dotnet";
        var arguments = "--version";
        this.logger.Executed(name, arguments);
        var (output, _) = await Command.ReadAsync(name, arguments).ConfigureAwait(false);
        return SemanticVersion.Parse(output.Trim());
    }

    public async Task InstallTemplatePackageAsync(string packageId)
    {
        var name = "dotnet";
        var arguments = $"new --install {packageId}";
        this.logger.Executed(name, arguments);
        await Command.RunAsync(name, arguments).ConfigureAwait(false);
    }

    public async Task UninstallTemplatePackageAsync(string packageId)
    {
        var name = "dotnet";
        var arguments = $"new --uninstall {packageId}";
        this.logger.Executed(name, arguments);
        await Command.RunAsync(name, arguments).ConfigureAwait(false);
    }

    public async Task CreateNewFromTemplateAsync(string templateShortName, IReadOnlyDictionary<string, string?> arguments)
    {
        var name = "dotnet";
        var args = $"new {templateShortName} {DotNetCliHelper.FormatAsCliArguments(arguments)}";
        this.logger.Executed(name, args);
        await Command.RunAsync(name, args).ConfigureAwait(false);
    }

    internal static class DotNetCliHelper
    {
        public static IReadOnlyList<(SemanticVersion SdkVersion, string DotnetRootPath)> ParseDotNetListSdksOutput(string listSdksOutput)
        {
            var output = listSdksOutput
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(outputLine =>
                {
                    var regexMatch = Regex.Match(outputLine, "^(?<version>.*) \\[(?<path>.*)\\]$");
                    var version = regexMatch.Groups["version"].Value;
                    var path = regexMatch.Groups["path"].Value;

                    var sdkVersion = SemanticVersion.Parse(version);

                    return (sdkVersion!, path!);
                })
                .ToList();

            return output;
        }

        public static string FormatAsCliArguments(IReadOnlyDictionary<string, string?> arguments)
            => string.Join(' ', arguments
                .Where(x => x.Value is not null)
                .Select(x => $"--{x.Key} \"{x.Value}\""));
    }
}
