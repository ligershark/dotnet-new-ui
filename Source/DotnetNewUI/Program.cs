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
    public static async Task<int> Main(string[] arguments)
    {
        var portOption = new Option<int>(
            new string[] { "-p", "--port" },
            () => PortService.DefaultPort,
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
                urlOpenerService.Open(url);
            }

            await WaitForCancellationAsync(cancellationToken).ConfigureAwait(false);
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

    private static Task WaitForCancellationAsync(CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource();
        if (cancellationToken.IsCancellationRequested)
        {
            tcs.SetResult();
        }
        else
        {
            var registration = cancellationToken.Register(() => tcs.SetResult());
        }

        return tcs.Task;
    }
}
