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
        var (output, _) = await this.RunAsync("dotnet", "--list-sdks").ConfigureAwait(false);
        return DotNetCliHelper.ParseDotNetListSdksOutput(output.Trim());
    }

    public async Task<SemanticVersion> GetSdkVersionAsync()
    {
        var (output, _) = await this.RunAsync("dotnet", "--version").ConfigureAwait(false);
        return SemanticVersion.Parse(output.Trim());
    }

    public async Task InstallTemplatePackageAsync(string packageId) =>
        await this
            .RunAsync("dotnet", $"new --install {packageId}")
            .ConfigureAwait(false);

    public async Task UninstallTemplatePackageAsync(string packageId) =>
        await this
            .RunAsync("dotnet", $"new --uninstall {packageId}")
            .ConfigureAwait(false);

    public async Task CreateNewFromTemplateAsync(string templateShortName, IReadOnlyDictionary<string, string?> arguments) =>
        await this.RunAsync(
            "dotnet",
            $"new {templateShortName} {DotNetCliHelper.FormatAsCliArguments(arguments)}")
            .ConfigureAwait(false);

    private async Task<(string Output, string Error)> RunAsync(string name, string arguments)
    {
        try
        {
            var (output, error) = await Command.ReadAsync(name, arguments).ConfigureAwait(false);
            this.logger.ExecutedSuccessfully(name, arguments, output, error);
            return (output, error);
        }
        catch (Exception exception)
        {
            this.logger.ExecutedFailed(name, arguments, exception);
            throw;
        }
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
                .Select(x => $"--{x.Key} \"{x.Value!.Replace('\\', '/')}\""));
    }
}
