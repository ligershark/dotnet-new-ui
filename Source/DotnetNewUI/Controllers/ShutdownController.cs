namespace DotnetNewUI.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class ShutdownController
{
    private readonly IHost host;

    public ShutdownController(IHost host)
        => this.host = host;

    [HttpPost]
    public async Task ShutdownAsync()
        => await this.host.StopAsync().ConfigureAwait(false);
}
