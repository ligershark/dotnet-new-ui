namespace DotnetNewUI;

using System.Diagnostics;
using System.Reflection;
using DotnetNewUI.Constants;
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
            .AddCors(
                options => options.AddPolicy(
                    CorsPolicyName.AllowAny,
                    x => x
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()))
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddSingleton(AnsiConsole.Console)
            .AddSingleton<IPortService, PortService>()
            .AddSingleton<IUrlOpenerService, UrlOpenerService>()
            .AddSingleton<INuGetClient, NuGetClient>()
            .AddSingleton<IPackagesService, PackagesService>()
            .AddSingleton<ITemplatesService, TemplatesService>()
            .AddHttpClient();

    private static void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder) =>
        webHostBuilder
            .UseWebRoot(GetFrontendDistDirectoryPath())
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
                    .UseCors()
                    .UseDefaultFiles()
                    .UseStaticFiles()
                    .UseEndpoints(
                        builder =>
                        {
                            builder.MapControllers().RequireCors(CorsPolicyName.AllowAny);
                            builder.MapSwagger();
                        })
                    .UseSwaggerUI());

    private static string GetFrontendDistDirectoryPath()
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        var directories = currentDirectory.GetDirectories("*", SearchOption.AllDirectories);
        var distDirectory = directories
            .Where(x => !x.FullName.Contains("node_modules") &&
                !x.FullName.Contains("bin") &&
                !x.FullName.Contains("obj") &&
                x.FullName.EndsWith(@"\Frontend\dist", StringComparison.Ordinal))
            .FirstOrDefault();
        if (distDirectory is not null)
        {
            Console.WriteLine(distDirectory.FullName);
            return distDirectory.FullName;
        }

        throw new DirectoryNotFoundException($"Unable to find Frontend/dist directory under {currentDirectory}");
    }
}
