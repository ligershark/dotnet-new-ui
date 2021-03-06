namespace DotnetNewUI.Controllers;

using DotnetNewUI.NuGet;
using DotnetNewUI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TemplatesController
{
    private readonly ITemplatesService templatesService;

    public TemplatesController(ITemplatesService templatesService) => this.templatesService = templatesService;

    [HttpGet]
    public async Task<IReadOnlyList<CompositeTemplateManifest>> GetAsync()
        => await this.templatesService.GetInstalledTemplatesAsync().ConfigureAwait(false);

    [HttpPost("{templateShortName}")]
    public async Task CreateNewFromTemplateAsync(string templateShortName, string outputPath, string? name, string? language)
        => await this.templatesService.CreateNewFromTemplateAsync(templateShortName, outputPath, name, language).ConfigureAwait(false);
}
