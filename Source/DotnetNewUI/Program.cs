namespace DotnetNewUI;

using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using DotnetNewUI.Services;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

public class Program
{
    private static readonly Option<int> PortOption = new(
        new string[] { "-p", "--port" },
        () => PortService.DefaultPort,
        "The port to run the application on.");

    private static readonly Option<bool> NoBrowserOption = new(
        new string[] { "-n", "--no-browser" },
        "Disable opening the browser.");

    public static async Task<int> Main(string[] arguments)
    {
        var rootCommand = new RootCommand("Start dotnet new-ui")
        {
            PortOption,
            NoBrowserOption,
        };

        rootCommand.SetHandler(OnHandleAsync);

        var commandLineBuilder = new CommandLineBuilder(rootCommand)
            .UseHost(hostBuilder => hostBuilder.ConfigureHost())
            .CancelOnProcessTermination()
            .UseDefaults();

        var parser = commandLineBuilder.Build();
        return await parser.InvokeAsync(arguments).ConfigureAwait(false);
    }

    private static async Task OnHandleAsync(InvocationContext context)
    {
        var host = context.GetHost();
        var port = context.ParseResult.GetValueForOption(PortOption);
        var noBrowser = context.ParseResult.GetValueForOption(NoBrowserOption);
        var cancellationToken = context.GetCancellationToken();
        var console = host.Services.GetRequiredService<IAnsiConsole>();

        try
        {
            {
                // Warm up the online package cache
                var packageService = host.Services.GetRequiredService<IPackagesService>();
                _ = packageService.GetTemplatePackagesAsync();
            }

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
                urlOpenerService.Open(url);
            }

            await host.WaitForShutdownAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            // Do nothing. The user closed the app.
        }
        catch (Exception exception)
        {
            console.WriteLine("Oops, something went wrong, we'd appreciate a bug report.");
            console.MarkupLine("  - [bold green]https://github.com/ligershark/dotnet-new-ui/issues[/]");
            console.WriteLine();
            console.WriteException(exception);
        }
    }
}
