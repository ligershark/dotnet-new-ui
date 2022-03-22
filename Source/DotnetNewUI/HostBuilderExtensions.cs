namespace DotnetNewUI;

using System.Diagnostics;
using System.Reflection;
using DotnetNewUI.NuGet;
using DotnetNewUI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

public static class HostBuilderExtensions
{
    public static void ConfigureHost(this IHostBuilder hostBuilder) =>
        hostBuilder
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.AddDebug();
                if (Debugger.IsAttached)
                {
                    loggingBuilder.AddConsole();
                }
            })
            .ConfigureServices(ConfigureServices)
            .UseDefaultServiceProvider(
                (context, options) =>
                {
                    var isDevelopment = context.HostingEnvironment.IsDevelopment();
                    options.ValidateScopes = isDevelopment;
                    options.ValidateOnBuild = isDevelopment;
                })
            .ConfigureWebHost(ConfigureWebHostBuilder);

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services) =>
        services
            .AddControllers()
            .AddApplicationPart(Assembly.GetExecutingAssembly()) // Needed for some reason to actually discover the API Controller
            .Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddSingleton(AnsiConsole.Console)
            .AddSingleton<IPortService, PortService>()
            .AddSingleton<IUrlOpenerService, UrlOpenerService>()
            .AddSingleton<INuGetClient, NuGetClient>()
            .AddHttpClient();

    private static void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder) =>
        webHostBuilder
            .UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "Frontend", "dist"))
            .UseKestrel(
                options =>
                {
                    var portService = options.ApplicationServices.GetRequiredService<IPortService>();

                    // We need at least one binding to actually start the server.
                    options.ListenAnyIP(PortService.DefaultPort);

                    options.Configure(portService.Configuration, reloadOnChange: true);
                })
            .Configure(
                app => app
                    .UseRouting()
                    .UseDefaultFiles()
                    .UseStaticFiles()
                        new StaticFileOptions()
                        {
                        })
                    .UseEndpoints(
                        builder =>
                        {
                            builder.MapControllers();
                            builder.MapSwagger();
                        })
                    .UseSwaggerUI());
}
