namespace DotnetNewUI;

using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Microsoft.Extensions.Hosting;

public class Program
{
    private const int DefaultPort = 6000;

    // new-ui --port 6000
    public static async Task<int> Main(string[] arguments)
    {
        var portOption = new Option<int>(
            new string[] { "-p", "--port" },
            () => DefaultPort,
            "The port to run the application on.");

        var rootCommand = new RootCommand("desc...")
        {
            portOption,
        };
        rootCommand.SetHandler(
            (Func<IHost, int, CancellationToken, Task>)OnHandleAsync,
            portOption);

        var commandLineBuilder = new CommandLineBuilder(rootCommand)
            .UseHost(hostBuilder => hostBuilder.ConfigureHost())
            .UseDefaults();
        var parser = commandLineBuilder.Build();
        return await parser.InvokeAsync(arguments).ConfigureAwait(false);
    }

    private static async Task OnHandleAsync(IHost host, int port, CancellationToken cancellationToken)
    {
        Console.WriteLine("Site running at:");
        Console.WriteLine($"    -UI:      http://localhost:{port}");
        Console.WriteLine($"    -Swagger: http://localhost:{port}/swagger");
        Console.WriteLine("Enter CTRL+C to exit.");

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
}
