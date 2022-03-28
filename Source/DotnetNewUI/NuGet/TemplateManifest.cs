namespace DotnetNewUI.NuGet;

public record class TemplateManifest(
    string Identity,
    string GroupIdentity,
    string Name,
    string Author,
    IReadOnlyCollection<string> Classifications,
    string Description,
    IReadOnlyCollection<string> ShortName,
    TemplateTags? Tags,
    IReadOnlyDictionary<string, Symbol>? Symbols);

public record class TemplateTags(
    string? Language,
    string Type);

public record class Symbol(
    string Type,
    string DataType,
    string DisplayName,
    string Description,
    string DefaultValue,
    string? ParameterName,
    IReadOnlyCollection<ChoiceOption>? Choices);

public record class ChoiceOption(
    string? DisplayName,
    string? Description,
    string Choice);

public record DotnetCliHostManifest(IReadOnlyDictionary<string, SymbolInfo>? SymbolInfo);

public record class SymbolInfo(
    string LongName,
    bool? IsHidden);

public record class TemplateIdeHostManifest(
    string? Icon,
    string? LearnMoreLink);

public record CompositeTemplateManifest(
    string PackageName,
    string Version,
    string? Base64Icon,
    bool IsBuiltIn,
    TemplateManifest TemplateManifest,
    TemplateIdeHostManifest? IdeHostManifest)
{
    public IReadOnlyCollection<string>? Languages { get; init; }
}
