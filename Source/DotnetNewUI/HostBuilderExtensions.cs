namespace DotnetNewUI;

using System.Diagnostics;
using DotnetNewUI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
            .Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddSingleton<IPortService, PortService>()
            .AddSingleton<IUrlOpenerService, UrlOpenerService>();

    private static void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder) =>
        webHostBuilder
            .UseKestrel(
                options =>
                {
                    // We need at least one binding to actually start the server.
                    options.ListenAnyIP(
                        4999,
                        listenOptions => listenOptions.UseHttps());

                    var portService = options.ApplicationServices.GetRequiredService<IPortService>();
                    options.Configure(portService.Configuration, reloadOnChange: true);
                })
            .Configure(
                app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpointBuilder => endpointBuilder.MapControllers());
                    app.UseSwagger();
                    app.UseSwaggerUI();
                });
}