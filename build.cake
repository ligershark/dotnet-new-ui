#addin nuget:?package=Cake.Npm&version=2.0.0

var target = Argument("Target", "Default");
var configuration =
    HasArgument("Configuration") ? Argument<string>("Configuration") :
    EnvironmentVariable("Configuration", "Release");

var artefactsDirectory = Directory("./Artefacts");
var frontendDirectory = GetDirectories("Source/DotnetNewUI/Frontend").First();
var frontendDistDirectory = GetDirectories("Source/DotnetNewUI/Frontend/dist").First();

Task("Clean")
    .Description("Cleans the artefacts, bin and obj directories.")
    .Does(() =>
    {
        CleanDirectory(frontendDistDirectory);
        CleanDirectory(artefactsDirectory);
        DeleteDirectories(GetDirectories("**/bin"), new DeleteDirectorySettings() { Force = true, Recursive = true });
        DeleteDirectories(GetDirectories("**/obj"), new DeleteDirectorySettings() { Force = true, Recursive = true });
    });

Task("RestoreNPM")
    .Description("Restores NPM packages.")
    .Does(() =>
    {
        NpmCi(x => x.FromPath(frontendDirectory));
    });

Task("BuildNPM")
    .Description("Builds the NPM project.")
    .IsDependentOn("RestoreNPM")
    .Does(() =>
    {
        NpmRunScript("build", x => x.FromPath(frontendDirectory));
    });

Task("Restore")
    .Description("Restores NuGet packages.")
    .Does(() =>
    {
        DotNetRestore();
    });

Task("Build")
    .Description("Builds the solution.")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetBuild(
            ".",
            new DotNetBuildSettings()
            {
                Configuration = configuration,
                NoRestore = true,
            });
    });

Task("Test")
    .Description("Runs unit tests and outputs test results to the artefacts directory.")
    .DoesForEach(GetFiles("./Tests/**/*.csproj"), project =>
    {
        DotNetTest(
            project.ToString(),
            new DotNetTestSettings()
            {
                Blame = true,
                Collectors = new string[] { "Code Coverage", "XPlat Code Coverage" },
                Configuration = configuration,
                Loggers = new string[]
                {
                    $"trx;LogFileName={project.GetFilenameWithoutExtension()}.trx",
                    $"junit;LogFileName={project.GetFilenameWithoutExtension()}.xml",
                    $"html;LogFileName={project.GetFilenameWithoutExtension()}.html",
                },
                NoBuild = true,
                NoRestore = true,
                ResultsDirectory = artefactsDirectory,
            });
    });

Task("Pack")
    .Description("Creates NuGet packages and outputs them to the artefacts directory.")
    .Does(() =>
    {
        DotNetPack(
            ".",
            new DotNetPackSettings()
            {
                Configuration = configuration,
                IncludeSymbols = true,
                MSBuildSettings = new DotNetMSBuildSettings()
                {
                    ContinuousIntegrationBuild = !BuildSystem.IsLocalBuild,
                },
                NoBuild = true,
                NoRestore = true,
                OutputDirectory = artefactsDirectory,
            });
    });

Task("Install")
    .Description("Install the dotnet tool globally.")
    .Does(() =>
    {
        StartProcess("powershell", new ProcessSettings().WithArguments(x => x.Append("./ClearCache.ps1")));
        StartProcess(
            "dotnet",
            new ProcessSettings()
                .WithArguments(x => x
                    .Append("tool")
                    .Append("install")
                    .Append("--global")
                    .Append("--no-cache")
                    .AppendSwitch("--add-source", "./Artefacts")
                    .Append("new-ui")));
    });

Task("Uninstall")
    .Description("Uninstall the dotnet tool globally.")
    .Does(() =>
    {
        StartProcess(
            "dotnet",
            new ProcessSettings()
                .WithArguments(x => x
                    .Append("tool")
                    .Append("uninstall")
                    .Append("--global")
                    .Append("new-ui")));
    });

Task("Default")
    .Description("Cleans, restores NuGet packages, builds the solution, runs unit tests and then creates NuGet packages.")
    .IsDependentOn("Clean")
    .IsDependentOn("RestoreNPM")
    .IsDependentOn("BuildNPM")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("Pack");

RunTarget(target);
