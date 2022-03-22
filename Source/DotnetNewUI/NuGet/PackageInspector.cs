namespace DotnetNewUI.NuGet;

using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;

public static class PackageInspector
{
    public static IReadOnlyList<TemplateManifest> GetTemplateManifestsFromPackage(string packagePath)
    {
        using var file = File.OpenRead(packagePath);
        var package = new ZipArchive(file);

        // Example: "content/ViewImports/.template.config/template.json"
        var regex = new Regex("^(content/)?(?<template>.*)/\\.template\\.config/template\\.json$");

        var templateManifests = package.Entries
            .Select(x => regex.Match(x.FullName) is { Success: true } ? x : null)
            .Where(x => x is not null)
            .Select(x => ReadZipArchiveEntry(x!))
            .ToList();

        return templateManifests;
    }

    private static TemplateManifest ReadZipArchiveEntry(ZipArchiveEntry entry)
    {
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        using var entryStream = entry.Open();
        var result = JsonSerializer.Deserialize<TemplateManifest>(entryStream, jsonSerializerOptions);
        return result!;
    }
}
