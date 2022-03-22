namespace DotnetNewUI.Controllers;

using DotnetNewUI.NuGet;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class TemplatesController
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
{
    private readonly INuGetClient nuGetClient;

    public TemplatesController(INuGetClient nuGetClient)
    {
        this.nuGetClient = nuGetClient;
    }

    // Returns template packages (one package might contain multiple templates)
    [HttpGet("online")]
    public async Task<NuGetPackageInfo[]> GetOnlineTemplatePackagesAsync()
    {
        return await this.nuGetClient.GetNuGetTemplates();
    }

    // Returns templates (multiple templates might belong to the same package)
    [HttpGet("installed")]
    public async Task GetInstalledTemplatesAsync()
    {
        throw new NotImplementedException();
    }

    [HttpPut("installed/{packageId}")]
    public async Task InstallTemplatePackageAsync(string packageId)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("installed/{packageId}")]
    public async Task UninstallTemplatePackageAsync(string packageId)
    {
        throw new NotImplementedException();
    }

    [HttpPatch("installed/{packageId}")]
    public async Task UpdateTemplatePackageAsync(string packageId)
    {
        throw new NotImplementedException();
    }
}
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
