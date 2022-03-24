namespace DotnetNewUI.Services;

using System.Text.RegularExpressions;
using global::NuGet.Versioning;

public static class DotNetCli
{
    public static async Task<(string DotnetRootPath, SemanticVersion SdkVersion)> ListSdksAsync()
    {
        var (output, _) = await SimpleExec.Command.ReadAsync("dotnet", "--list-sdks").ConfigureAwait(false);
        return DotNetCliHelper.ParseDotNetListSdksOutput(output);
    }

    public static async Task InstallTemplatePackageAsync(string packageId)
        => await SimpleExec.Command.RunAsync("dotnet", $"new --install {packageId}").ConfigureAwait(false);

    public static async Task UninstallTemplatePackageAsync(string packageId)
        => await SimpleExec.Command.RunAsync("dotnet", $"new --uninstall {packageId}").ConfigureAwait(false);

    public static async Task CreateNewFromTemplateAsync(string templateShortName, IReadOnlyDictionary<string, string?> arguments)
        => await SimpleExec.Command.RunAsync("dotnet", $"new {templateShortName} {DotNetCliHelper.FormatAsCliArguments(arguments)}").ConfigureAwait(false);
}

public static class DotNetCliHelper
{
    public static (string DotnetRootPath, SemanticVersion SdkVersion) ParseDotNetListSdksOutput(string listSdksOutput)
    {
        var outputLine = listSdksOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Last(); // Take latest SDK
        var regexMatch = Regex.Match(outputLine, "^(?<version>.*) \\[(?<path>.*)\\]$");
        var version = regexMatch.Groups["version"].Value;
        var path = regexMatch.Groups["path"].Value;

        var dotnetRootPath = Path.GetDirectoryName(path); // Get to parent directory
        var sdkVersion = SemanticVersion.Parse(version);

        return (dotnetRootPath!, sdkVersion!);
    }

    public static string FormatAsCliArguments(IReadOnlyDictionary<string, string?> arguments)
        => string.Join(' ', arguments
            .Where(x => x.Value is not null)
            .Select(x => $"--{x.Key} \"{x.Value}\""));
}
