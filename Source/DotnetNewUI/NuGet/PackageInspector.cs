namespace DotnetNewUI.NuGet;

using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

public static class PackageInspector
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    public static IReadOnlyList<CompositeTemplateManifest> GetTemplateManifestsFromPackage(string packagePath, bool isBuiltIn)
    {
        var (packageName, packageVersion) = GetPackageNameAndVersion(packagePath);

        using var file = File.OpenRead(packagePath);
        using var archive = new ZipArchive(file);

        var metadataFilePath = $"{packageName}.nuspec";
        var metadataFile = archive.Entries.Single(x => x.FullName.Equals(metadataFilePath, StringComparison.OrdinalIgnoreCase));

        var metadata = GetPackageMetadata(metadataFile!);
        var base64PackageIcon = metadata.Metadata!.Icon is string packageIconPath ? GetBase64PackageIcon(archive, packageIconPath) : null;

        var templateManifestRegex = new Regex("^(content/)?(?<template>.*)/\\.template\\.config/template\\.json$");

        var templateManifests = archive.Entries
            .Select(x => templateManifestRegex.Match(x.FullName) is { Success: true } ? x : null)
            .Where(x => x is not null)
            .Select(x => x!)
            .Select(x =>
            {
                var parentDirectory = Path.GetDirectoryName(x.FullName)!;

                var templateManifest = GetTemplateManifest(x);
                var ideHostManifest = TryGetIdeHostManifest(archive, parentDirectory);
                var base64TemplateIcon = ideHostManifest?.Icon is string templateIconPath ? GetBase64TemplateIcon(archive, parentDirectory, ideHostManifest.Icon) : null;

                return new CompositeTemplateManifest(packageName, packageVersion, base64TemplateIcon ?? base64PackageIcon, isBuiltIn, templateManifest, ideHostManifest);
            })
            .ToList();

        return templateManifests;
    }

    public static (string PackageName, string Version) GetPackageNameAndVersion(string filePath)
    {
        var fileName = Path.GetFileName(filePath);

        var regex = new Regex("^(?<packagename>.*)\\.(?<version>\\d*\\.\\d*\\.\\d*-?.*)\\.nupkg$");
        var match = regex.Match(fileName);
        var packageName = match.Groups["packagename"].Value;
        var version = match.Groups["version"].Value;

        return (packageName, version);
    }

    private static PackageManifest GetPackageMetadata(ZipArchiveEntry metadataFile)
    {
        using var metadataFileStream = metadataFile.Open();
        var xDoc = XDocument.Load(metadataFileStream);
        var packageElement = xDoc.Document!.Elements().First();
        var metadataElement = packageElement!.Elements().First();
        var id = metadataElement!.Elements().First(e => e.Name.LocalName == "id").Value;
        var version = metadataElement!.Elements().First(e => e.Name.LocalName == "version").Value;
        var icon = metadataElement!.Elements().FirstOrDefault(e => e.Name.LocalName == "icon")?.Value;
        return new PackageManifest(new PackageMetadata(id, version, icon));
    }

    private static TemplateManifest GetTemplateManifest(ZipArchiveEntry templateFile)
    {
        using var templateFileStream = templateFile.Open();
        var templateFileContent = JsonSerializer.Deserialize<TemplateManifest>(templateFileStream, JsonSerializerOptions);
        return templateFileContent!;
    }

    private static TemplateIdeHostManifest? TryGetIdeHostManifest(ZipArchive archive, string parentDirectory)
    {
        var ideHostFilePath = Path.Combine(parentDirectory, "ide.host.json").Replace("\\", "/");
        var ideHostFile = archive.Entries.FirstOrDefault(e => e.FullName == ideHostFilePath);
        if (ideHostFile is not null)
        {
            using var ideHostFileStream = ideHostFile.Open();
            var ideHostFileContent = JsonSerializer.Deserialize<TemplateIdeHostManifest>(ideHostFileStream, JsonSerializerOptions)!;

            return ideHostFileContent;
        }

        return null;
    }

    private static string GetBase64TemplateIcon(ZipArchive archive, string parentDirectory, string relativeIconPath)
    {
        var iconFilePath = Path.Combine(parentDirectory, relativeIconPath).Replace("\\", "/");
        return GetBase64Icon(archive, iconFilePath);
    }

    private static string GetBase64PackageIcon(ZipArchive archive, string iconPath)
        => GetBase64Icon(archive, iconPath);

    private static string GetBase64Icon(ZipArchive archive, string iconPath)
    {
        var iconFile = archive.Entries.Single(e => e.FullName == iconPath);
        var iconFileType = Path.GetExtension(iconPath)[1..];

        using var iconFileStream = iconFile.Open();
        using var memoryStream = new MemoryStream();
        iconFileStream.CopyTo(memoryStream);
        var bytes = memoryStream.ToArray();
        var base64Icon = Convert.ToBase64String(bytes);

        return $"data:image/{iconFileType};base64,{base64Icon}";
    }
}
