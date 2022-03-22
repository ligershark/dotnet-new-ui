namespace DotnetNewUI.Services;

using DotnetNewUI.Options;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Dynamically changes the port that Kestrel is listening on.
/// See https://github.com/dotnet/aspnetcore/issues/21244.
/// </summary>
public class PortService : IPortService
{
    public const int DefaultPort = 4999;

    private readonly ILogger<PortService> logger;

    public PortService(ILogger<PortService> logger)
    {
        var configBuilder = new ConfigurationBuilder();

        // A custom source/provider is required because the InMemory one doesn't support key removal.
        configBuilder.Add(new MutableSource());
        this.Configuration = configBuilder.Build();
        this.logger = logger;
    }

    public IConfigurationRoot Configuration { get; }

    public int Port
    {
        set
        {
            if (value == DefaultPort)
            {
                this.logger.AlreadyOnDefaultPort(value);
            }
            else
            {
                var key = $"Endpoints:{value}:Url";
                var url = this.Configuration[key];
                if (string.IsNullOrEmpty(url))
                {
                    url = $"http://*:" + value;
                    this.Configuration[key] = url;
                    this.logger.AddedEndpoint(url, this.Configuration.GetDebugView());
                }
                else
                {
                    this.Configuration[key] = null;
                    this.logger.RemovedEndpoint(url, this.Configuration.GetDebugView());
                }

                this.Configuration.Reload();
            }
        }
    }
}
