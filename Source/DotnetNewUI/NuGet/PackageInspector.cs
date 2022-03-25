namespace DotnetNewUI.NuGet;

using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

public static class PackageInspector
{
    public static IReadOnlyList<CompositeTemplateManifest> GetTemplateManifestsFromPackage(string packagePath, bool isBuiltIn)
    {
        var (packageName, packageVersion) = PackageInspectorHelper.GetPackageNameAndVersion(packagePath);

        using var file = File.OpenRead(packagePath);
        using var archive = new ZipArchive(file);

        var packageManifest = GetPackageManifest(archive, packageName);
        var base64PackageIcon = TryGetBase64PackageIcon(archive, packageManifest.Metadata.Icon);

        return PackageInspectorHelper
            .GetTemplateConfigDirs(archive.Entries.Select(x => x.FullName))
            .Select(templateConfigDir =>
            {
                var templateManifest = TryGetTemplateManifest(archive, templateConfigDir);
                var ideHostManifest = TryGetIdeHostManifest(archive, templateConfigDir);
                var base64TemplateIcon = TryGetBase64TemplateIcon(archive, templateConfigDir, ideHostManifest?.Icon);

                // templateManifest! -> possibly forcing null into a non-nullable property, but filtering those cases out immediately
                // it makes it easier for the rest of app, so we don't have to check for nulls
                return new CompositeTemplateManifest(packageName, packageVersion, base64TemplateIcon ?? base64PackageIcon, isBuiltIn, templateManifest!, ideHostManifest);
            })
            .Where(x => x.TemplateManifest is not null)
            .ToList();
    }

    private static PackageManifest GetPackageManifest(ZipArchive archive, string packageName)
    {
        var packageManifestPath = PackageInspectorHelper.GetPackageManifestPath(packageName);
        var packageManifestFile = archive.Entries.Single(x => string.Equals(x.FullName, packageManifestPath, StringComparison.OrdinalIgnoreCase));
        using var packageManifestFileStream = packageManifestFile.Open();
        return PackageInspectorHelper.ParseXmlPackageManifest(packageManifestFileStream);
    }

    private static TemplateManifest? TryGetTemplateManifest(ZipArchive archive, string templateConfigDir)
    {
        var templateManifestPath = PackageInspectorHelper.GetTemplateManifestPath(templateConfigDir);
        var templateManifestFile = archive.Entries.SingleOrDefault(x => string.Equals(x.FullName, templateManifestPath, StringComparison.OrdinalIgnoreCase));
        if (templateManifestFile is not null)
        {
            using var templateManifestFileStream = templateManifestFile.Open();
            return PackageInspectorHelper.ParseJsonTemplateManifest(templateManifestFileStream);
        }

        return null;
    }

    private static TemplateIdeHostManifest? TryGetIdeHostManifest(ZipArchive archive, string templateConfigDir)
    {
        var ideHostManifestPath = PackageInspectorHelper.GetIdeHostManifestPath(templateConfigDir);
        var ideHostManifestFile = archive.Entries.SingleOrDefault(x => string.Equals(x.FullName, ideHostManifestPath, StringComparison.OrdinalIgnoreCase));
        if (ideHostManifestFile is not null)
        {
            using var ideHostManifestFileStream = ideHostManifestFile.Open();
            return PackageInspectorHelper.ParseJsonIdeHostManifest(ideHostManifestFileStream);
        }

        return null;
    }

    private static string? TryGetBase64PackageIcon(ZipArchive archive, string? iconPath)
    {
        if (iconPath is not null)
        {
            var packageIconFile = archive.Entries.Single(x => string.Equals(x.FullName, iconPath, StringComparison.OrdinalIgnoreCase));
            using var packageIconFileStream = packageIconFile.Open();
            return PackageInspectorHelper.GetBase64Icon(packageIconFileStream, packageIconFile.Name);
        }

        return null;
    }

    private static string? TryGetBase64TemplateIcon(ZipArchive archive, string templateConfigDir, string? templateIconRelativePath)
    {
        if (templateIconRelativePath is not null)
        {
            var templateIconPath = PackageInspectorHelper.GetTemplateIconPath(templateConfigDir, templateIconRelativePath);
            var templateIconFile = archive.Entries.Single(x => string.Equals(x.FullName, templateIconPath, StringComparison.OrdinalIgnoreCase));
            return PackageInspectorHelper.GetBase64Icon(templateIconFile.Open(), templateIconFile.Name);
        }

        return null;
    }

    internal static class PackageInspectorHelper
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static (string PackageName, string Version) GetPackageNameAndVersion(string filePath)
        {
            var fileName = Path.GetFileName(filePath);

            var regex = new Regex("^(?<packagename>.*)\\.(?<version>\\d*\\.\\d*\\.\\d*-?.*)\\.nupkg$");
            var match = regex.Match(fileName);
            var packageName = match.Groups["packagename"].Value;
            var version = match.Groups["version"].Value;

            return (packageName, version);
        }

        public static string GetPackageManifestPath(string packageName)
            => $"{packageName}.nuspec";

        public static IEnumerable<string> GetTemplateConfigDirs(IEnumerable<string> archive)
        {
            var templateConfigDirRegex = new Regex("^(content/)?(?<template>.*)/\\.template\\.config");

            return archive
                .Select(x => templateConfigDirRegex.Match(x))
                .Where(x => x.Success)
                .Select(x => x.Value)
                .Distinct();
        }

        public static string GetTemplateManifestPath(string templateConfigDir)
            => Path.Combine(templateConfigDir, "template.json").Replace("\\", "/");

        public static string GetIdeHostManifestPath(string templateConfigDir)
            => Path.Combine(templateConfigDir, "ide.host.json").Replace("\\", "/");

        public static string GetTemplateIconPath(string templateConfigDir, string iconRelativePath)
            => Path.Combine(templateConfigDir, iconRelativePath).Replace("\\", "/");

        public static PackageManifest ParseXmlPackageManifest(Stream stream)
        {
            var xDoc = XDocument.Load(stream);
            var packageElement = xDoc.Document!.Elements().First();
            var metadataElement = packageElement!.Elements().First();
            var id = metadataElement!.Elements().First(e => string.Equals(e.Name.LocalName, "id", StringComparison.OrdinalIgnoreCase)).Value;
            var version = metadataElement!.Elements().First(e => string.Equals(e.Name.LocalName, "version", StringComparison.OrdinalIgnoreCase)).Value;
            var icon = metadataElement!.Elements().FirstOrDefault(e => string.Equals(e.Name.LocalName, "icon", StringComparison.OrdinalIgnoreCase))?.Value;
            return new PackageManifest(new PackageMetadata(id, version, icon));
        }

        public static TemplateManifest ParseJsonTemplateManifest(Stream stream)
            => JsonSerializer.Deserialize<TemplateManifest>(stream, JsonSerializerOptions)!;

        public static TemplateIdeHostManifest ParseJsonIdeHostManifest(Stream stream)
            => JsonSerializer.Deserialize<TemplateIdeHostManifest>(stream, JsonSerializerOptions)!;

        public static string GetBase64Icon(Stream stream, string fileName)
        {
            var fileType = Path.GetExtension(fileName);

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var bytes = memoryStream.ToArray();
            var base64Icon = Convert.ToBase64String(bytes);

            return $"data:image/{fileType};base64,{base64Icon}";
        }
    }
}
