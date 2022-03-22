namespace DotnetNewUI;

using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using DotnetNewUI.Services;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

public class Program
{
    private const int DefaultPort = 5432;

    public static async Task<int> Main(string[] arguments)
    {
        var portOption = new Option<int>(
            new string[] { "-p", "--port" },
            () => DefaultPort,
            "The port to run the application on.");
        var noBrowserOption = new Option<bool>(
            new string[] { "-n", "--no-browser" },
            "Disable opening the browser.");

        var rootCommand = new RootCommand("desc...")
        {
            portOption,
            noBrowserOption,
        };
        rootCommand.SetHandler(
            (Func<IHost, int, bool, CancellationToken, Task>)OnHandleAsync,
            portOption,
            noBrowserOption);

        var commandLineBuilder = new CommandLineBuilder(rootCommand)
            .UseHost(hostBuilder => hostBuilder.ConfigureHost())
            .UseDefaults();
        var parser = commandLineBuilder.Build();
        return await parser.InvokeAsync(arguments).ConfigureAwait(false);
    }

    private static async Task OnHandleAsync(IHost host, int port, bool noBrowser, CancellationToken cancellationToken)
    {
        var console = host.Services.GetRequiredService<IAnsiConsole>();

        try
        {
            var portService = host.Services.GetRequiredService<IPortService>();
            portService.Port = port;

            var url = new Uri($"http://localhost:{port}");
            var swaggerUrl = new Uri($"{url}swagger");

            console.MarkupLine("[blueviolet]    ▄▄▄▄   ▄▄▄▄ ▄▄▄▄▄▄▄▄▄▄▄ ▄▄▄▄▄▄▄▄▄▄▄   ▄▄▄▄   ▄▄▄▄                            ▄▄▄▄▄  ▄▄▄▄ ▄▄▄▄▄[/]");
            console.MarkupLine("[blueviolet]     ████▄  ██   ███    ██  ██  ███  ██    ████▄  ██  ▄▄▄▄▄▄▄▄▄█ ▄▄▄▄  ▄  ▄▄▄▄    ███    ██   ███[/]");
            console.MarkupLine("[blueviolet]     ██ ███▄██   ███▄▄▄█        ███        ██ ███▄██ ███▄▄▄▄▄▄█   ███ ███ ███     ███    ██   ███[/]");
            console.MarkupLine("[blueviolet]     ██   ████   ███    ▄▄      ███        ██   ████ ███           █████████      ███    ██   ███[/]");
            console.MarkupLine("[blueviolet] ██ ▄██▄    ██  ▄███▄▄▄████    ▄███▄      ▄██▄    ██   ██▄▄▄▄███    ██   ██        ███▄▄██   ▄███▄[/]");
            console.WriteLine();
            console.WriteLine("Now running at:");
            console.WriteLine();
            console.MarkupLine($"  - UI:      [bold green]{url}[/]");
            console.MarkupLine($"  - Swagger: [bold green]{swaggerUrl}[/]");
            console.WriteLine();
            console.WriteLine("Enter CTRL+C to exit.");

            if (!noBrowser)
            {
                var urlOpenerService = host.Services.GetRequiredService<IUrlOpenerService>();
                urlOpenerService.Open(swaggerUrl);
            }

            while (true)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    await Task
                        .Delay(100, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }
        catch (Exception exception)
        {
            console.WriteException(exception);
        }
    }
}
